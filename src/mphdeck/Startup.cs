using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using mphdict;
using mphdict.Models.Etym;
using mphdict.Models.morph;
using mphdict.Models.SynonymousSets;
using mphdeck.Providers;
using Mvc.RenderViewToString;

namespace mphdeck
{
    public class Startup
    {
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}
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

            ContentRootPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            //string connection = Configuration.GetConnectionString("EtymDBWebContext");
            services.AddScoped<RazorViewToStringRenderer, RazorViewToStringRenderer>();

            services.AddSingleton<IConfiguration>(Configuration);
            services
                .AddDbContext<mphContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetParent(Directory.GetParent(Startup.ContentRootPath).FullName).FullName, $"data/mph_{Configuration.GetConnectionString("sqlitedb")}.db")}"), ServiceLifetime.Transient, ServiceLifetime.Transient)
                .AddDbContext<synsetsContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetParent(Directory.GetParent(Startup.ContentRootPath).FullName).FullName, $"data/synsets_{Configuration.GetConnectionString("sqlitedb")}.db")}"), ServiceLifetime.Transient, ServiceLifetime.Transient)
                .AddDbContext<etymContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetParent(Directory.GetParent(Startup.ContentRootPath).FullName).FullName, $"data/etym.db")}"), ServiceLifetime.Transient, ServiceLifetime.Transient);

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            // Add framework services.
            services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/TextAnalyze", "{culture:regex(^[a-z]{{2}}(?:-[A-z]{{2}})?$)?}/TextAnalyze");
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            #region adding Localization support
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
            #endregion

            #region adding logger support
            ApplicationLogging.LoggerFactory = loggerFactory;
            //var s = $"Filename={Path.Combine(Directory.GetParent(Startup.ContentRootPath).FullName, $"data/mph_{Configuration.GetConnectionString("sqlitedb")}.db")}";
            //loggerFactory.CreateLogger("Debug info").LogError(s);
            #endregion

            ApplicationVariables.services = app.ApplicationServices;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/Info");
            }

            // http error
            app.UseStatusCodePagesWithReExecute("/error/statuscodeinfo/{0}");

            app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "cultureRoute",
                    template: "{culture}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "ShowStructure" },
                    constraints: new
                    {
                        culture = new RegexRouteConstraint("^[a-z]{2}(?:-[A-z]{2})?$")
                    });
                routes.MapRoute(
                    name: "default",
                    template: "{controller=inflection}/{action=Index}/{id?}");
            });

            var options = new BrowserWindowOptions
            {
                WebPreferences = new WebPreferences
                {
                    WebSecurity = false
                }
            };

            Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
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
