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
using mphweb.FuncModule;
using mphdict.Models.SynonymousSets;

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
            services.AddTransient<mphObj>();
            services.AddTransient<synsetsObj>();
            services.AddScoped<ViewRender, ViewRender>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddEntityFramework()
                .AddEntityFrameworkSqlite()
                //.AddDbContext<mphContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, $"data/{Configuration.GetConnectionString("sqlitedb")}")}"));
                .AddDbContext<mphContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetParent(Startup.ContentRootPath).FullName, $"data/{Configuration.GetConnectionString("sqlitedb")}")}"));
            services.AddEntityFramework()
                .AddEntityFrameworkSqlServer()
                .AddDbContext<synsetsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SQLContext")));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ApplicationVariables.services=app.ApplicationServices;
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            loggerFactory.AddSerilog();
            ApplicationLogging.LoggerFactory = loggerFactory;

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
                    name: "default",
                    template: "{controller=inflection}/{action=Index}/{id?}");
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