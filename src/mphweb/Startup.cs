using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using Serilog;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using mphdict.Models.morph;
using mphdict;
using mphdict.Models.SynonymousSets;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using mphweb.Providers;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.Mvc.Razor;
using Mvc.RenderViewToString;
using mphdict.Models.Etym;

namespace mphweb
{
    public class Startup
    {
        public static string ContentRootPath;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            builder.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"start_ua_word", "вітання"},
                {"start_ru_word", "привет"}
            });

            Configuration = builder.Build();

            var logFile = Path.Combine(env.ContentRootPath, "logs/log-{Date}.txt");
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Error()
                //.WriteTo.RollingFile(new Serilog.Formatting.Json.JsonFormatter(), logFile)
                .WriteTo.RollingFile(logFile, outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();
            ContentRootPath = env.ContentRootPath;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //string connection = Configuration.GetConnectionString("EtymDBWebContext");
            services.AddTransient<mphObj>();
            services.AddTransient<synsetsObj>();
            services.AddTransient<etymObj>();
            services.AddScoped<RazorViewToStringRenderer, RazorViewToStringRenderer>();
            
            services.AddSingleton<IConfiguration>(Configuration);
            services
                .AddEntityFrameworkSqlite()
                //.AddDbContext<mphContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, $"data/{Configuration.GetConnectionString("sqlitedb")}")}"));
                .AddDbContext<mphContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetParent(Startup.ContentRootPath).FullName, $"data/mph_{Configuration.GetConnectionString("sqlitedb")}.db")}"))
                .AddDbContext<synsetsContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetParent(Startup.ContentRootPath).FullName, $"data/synsets_{Configuration.GetConnectionString("sqlitedb")}.db")}"))
                .AddDbContext<etymContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetParent(Startup.ContentRootPath).FullName, $"data/etym.db")}"));
                //.AddDbContext<etymContext>(options => options.UseSqlServer(connection));
            //services.AddEntityFramework()
            //    .AddEntityFrameworkSqlServer()

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            // Add framework services.
            services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/TextAnalyze", "{culture:regex(^[a-z]{{2}}(?:-[A-Z]{{2}})?$)?}/TextAnalyze");
                });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("uk"),
                new CultureInfo("en"),
            };
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("uk"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures,
            };
            localizationOptions.RequestCultureProviders.Insert(0, new UrlRequestCultureProvider(localizationOptions));
            app.UseRequestLocalization(localizationOptions);

            ApplicationVariables.services=app.ApplicationServices;
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            loggerFactory.AddSerilog();
            ApplicationLogging.LoggerFactory = loggerFactory;

            //Microsoft.Extensions.Logging.ILogger Logger = ApplicationLogging.CreateLogger<Startup>();
            //Logger.LogError(new EventId(0), new Exception(), $"Filename={Path.Combine(Directory.GetParent(Startup.ContentRootPath).FullName, $"data/mph_{Configuration.GetConnectionString("sqlitedb")}.db")}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error/Info");
            }

            // http error
            app.UseStatusCodePagesWithReExecute("/error/statuscodeinfo/{0}");

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "cultureRoute",
                    template: "{culture}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "ShowStructure" },
                    constraints: new
                    {
                        culture = new RegexRouteConstraint("^[a-z]{2}(?:-[A-Z]{2})?$")
                    });
                routes.MapRoute(
                    name: "default",
                    template: "{controller=inflection}/{action=Index}/{id?}");
            });
            app.UseSignalR(routes =>
            {
                routes.MapHub<TextAnalyzeHub>("TextAnalyze/analyse");
            });
        }
    }
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; set; } /*= new LoggerFactory();*/
        public static Microsoft.Extensions.Logging.ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    }
    public static class ApplicationVariables
    {
        public static IServiceProvider services { get; set; }
    }
}