using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using mphdict;
using mphdict.Logging;
using mphdict.Models.morph;
using mphdict.Models.SynonymousSets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mphcon
{
    public class Program
    {
        static public IConfiguration Configuration { get; set; }
        static ILogger Logger { get; } = ApplicationLogging.LoggerFactory.CreateLogger<Program>();

        static Program p;
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = configurationBuilder.Build();

            p = new Program();
            p.InitializeServices();

            if ((args != null) && (args.Length > 0))
            {
                switch (args[0])
                {
                    case "json":
                        generateJson();
                        break;
                    default:
                        Console.WriteLine("Unknown parameters");
                        break;
                }
            }
            prepare_ill_synsets();
            //forming_nrw_synsets();
            //forming_nrw();
            //copyContext();
            //copyFromJson();
            //testAdd();
            //removeData();
            //viewWords();
        }

        static void prepare_ill_synsets()
        {
            p.container.GetService<synsetsObj>().prepare_ill();
        }
        static void forming_nrw_synsets()
        {
            p.container.GetService<synsetsObj>().forming_nrw();
        }
        static void forming_nrw()
        {
            DbContextOptionsBuilder<mphContext> DbContextOptions = new DbContextOptionsBuilder<mphContext>().UseSqlServer(Configuration.GetConnectionString("gramSqlDb"));
            mphDb sourceContext = new mphDb(DbContextOptions, "dbo", ApplicationLogging.LoggerFactory);
            sourceContext.forming_nrw();
        }
        static void removeData()
        {
            p.container.GetService<mphDb>().removeData();
        }
        
        static void viewWords()
        {
            mphContext context = p.container.GetService<mphDb>().getContext();
            Console.WriteLine(context.words_list.Count());
            foreach (var item in context.words_list.OrderBy(c => c.digit).ThenBy(c => c.field2).Take(1000))
            {
                Console.WriteLine($"{item.nom_old}\t\t {item.reestr}\t {item.field2}");
            }
        }
        static void testAdd()
        {
            // creating db context by "Create" method (for working with multiple schemas)
            DbContextOptionsBuilder<mphContext> op_o = new DbContextOptionsBuilder<mphContext>().UseSqlServer(Configuration.GetConnectionString("gramSqlDb"));
            mphDb mphDbContext = new mphDb(op_o, "ukgram", ApplicationLogging.LoggerFactory);
            var ac = mphDbContext.getContext().accents_class.AsNoTracking().OrderBy(c=>c.id).ToArray();
            DbContextOptionsBuilder<mphContext> op = new DbContextOptionsBuilder<mphContext>().UseSqlServer(Configuration.GetConnectionString("gramSqlDb2"));
            mphDb sourceContext = new mphDb(op, "ukgram", ApplicationLogging.LoggerFactory);
            sourceContext.testAddAccents_class();
        }
        private static void copyContext()
        {
            // creating db context by "Create" method (for working with multiple schemas)
            DbContextOptionsBuilder<mphContext>  op = new DbContextOptionsBuilder<mphContext>().UseSqlServer(Configuration.GetConnectionString("gramSqlDb"));
            mphDb sourceContext = new mphDb(op, "ukgram", ApplicationLogging.LoggerFactory);
            //DbContextOptionsBuilder<mphContext> op2 = new DbContextOptionsBuilder<mphContext>().UseSqlServer(Configuration.GetConnectionString("gramSqlDb2"));
            //mphDb resultContext = new mphDb(op2, "ukgram", ApplicationLogging.LoggerFactory);
            Console.WriteLine("Почекайте, будь ласка, дані переносяться...");
            //sourceContext.copyContextTo(resultContext);
            sourceContext.copyContextTo(p.container.GetService<mphDb>());
        }
        private static void copyFromJson()
        {
            mphODInfo o = JsonConvert.DeserializeObject<mphODInfo>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "dict.json"), Encoding.Unicode));
            p.container.GetService<mphDb>().copyFromObj(o);
        }
        private static void generateJson()
        {
            Console.WriteLine("Почекайте, будь ласка, генерується файл «dict.json»...");
            // creating db context by DI
            mphDb service = p.container.GetService<mphDb>();
            //DbContextOptionsBuilder<mphContext> op = new DbContextOptionsBuilder<mphContext>().UseSqlServer(Configuration.GetConnectionString("gramSqlDb"));
            //mphDb service = new mphDb(op, "ukgram", ApplicationLogging.LoggerFactory);
            var o = service.getObj();
            string s = JsonConvert.SerializeObject(o);
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "dict.json"), s, Encoding.Unicode);
        }
        private void InitializeServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<mphDb>();
            services.AddTransient<synsetsObj>();
            services.AddSingleton<ILoggerFactory>(ApplicationLogging.LoggerFactory);

            services.AddEntityFramework()
                .AddEntityFrameworkSqlServer()
                .AddDbContext<synsetsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("gramSqlDb")));
            //services.AddEntityFramework()
            //    .AddEntityFrameworkSqlite()
            //    //.AddDbContext<mphContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetCurrentDirectory(), "mph_c.db")}"));
            //    .AddDbContext<mphContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, $"data/{Configuration.GetConnectionString("sqlitedb")}")}"));

            container = services.BuildServiceProvider();
        }
        public IServiceProvider container { get; private set; }

        #region  "ConfigureServices" for creating database (Migration)
        public void ConfigureServices(IServiceCollection services)
        {
            //dotnet ef migrations add MyFirstMigration 
            //dotnet ef database update
            //dotnet ef migrations remove
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = configurationBuilder.Build();

            //services.AddEntityFramework()
            //    .AddEntityFrameworkSqlServer()
            //    .AddDbContext<mphContext>(options => options.UseSqlServer(Configuration.GetConnectionString("gramSqlDb2"),
            //    b => b.MigrationsAssembly("mphcon")));
            services.AddEntityFramework()
                .AddEntityFrameworkSqlite()
                .AddDbContext<mphContext>(options => options.UseSqlite($"Filename={Path.Combine(Directory.GetCurrentDirectory(), "mph.db")}",
                b => b.MigrationsAssembly("mphcon")));
        }
        #endregion
    }
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory().AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logs\\applogs.txt"), LogLevel.Error);
        //public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    }
}
