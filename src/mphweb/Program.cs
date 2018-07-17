using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using mphdict;
using Serilog;
using uSofTrod.generalTypes.Models;

namespace mphweb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var ContentRootPath = Directory.GetCurrentDirectory();
            var logFile = Path.Combine(ContentRootPath, "logs/log-{Date}.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.RollingFile(logFile, outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                //.UseDefaultServiceProvider(options =>
                //options.ValidateScopes = false)
                ;
    }

    public enum viewtype { dict, synsets, pclass, aclass, analyze, etym, error };
    public static class variables
    {
        private static SelectList _pclass;
        public static SelectList pclass
        {
            get
            {
                if (_pclass == null)
                {
                    _pclass = new SelectList(((mphObj)ApplicationVariables.services.GetService(typeof(mphObj))).pclass);
                }
                return _pclass;
            }
        }
        private static SelectList _pofs = null;
        public static SelectList pofs
        {
            get
            {
                if (_pofs == null)
                {
                    try
                    {
                        var t = ((mphObj)ApplicationVariables.services.GetService(typeof(mphObj))).pofs;
                        var tpofs = new ps[t[0].Length + t[1].Length];
                        t[0].CopyTo(tpofs, 0);
                        t[1].CopyTo(tpofs, t[0].Length);
                        _pofs = new SelectList(tpofs, "id", "name", null, "category");
                    }
                    catch (Exception ex)
                    {
                        _pofs = null;
                    }
                }
                return _pofs;
            }
        }
        private static SelectList _pofsPcls = null;
        public static SelectList pofsPcls
        {
            get
            {
                if (_pofsPcls == null)
                {
                    try
                    {
                        var t = ((mphObj)ApplicationVariables.services.GetService(typeof(mphObj))).pofs;
                        _pofsPcls = new SelectList(t[0], "id", "name");
                    }
                    catch (Exception ex)
                    {
                        _pofsPcls = null;
                    }
                }
                return _pofsPcls;
            }
        }
        private static langid _lang = null;
        public static langid lang
        {
            get
            {
                if (_lang == null)
                {
                    try
                    {
                        _lang = ((mphObj)ApplicationVariables.services.GetService(typeof(mphObj))).lid;
                    }
                    catch (Exception ex)
                    {
                        _lang = null;
                    }
                }
                return _lang;
            }
        }

        private static SelectList _synpofs = null;
        public static SelectList synpofs
        {
            get
            {
                if (_synpofs == null)
                {
                    try
                    {
                        var t = ((synsetsObj)ApplicationVariables.services.GetService(typeof(synsetsObj))).pofs;
                        var tpofs = new ps[t[0].Length];
                        t[0].CopyTo(tpofs, 0);
                        //t[1].CopyTo(tpofs, t[0].Length);
                        _synpofs = new SelectList(tpofs, "id", "name", null, null);
                    }
                    catch (Exception ex)
                    {
                        _synpofs = null;
                    }
                }
                return _synpofs;
            }
        }
        private static ps[] _etymLang_all = null;
        public static ps[] etymLang_all
        {
            get
            {
                if (_etymLang_all == null)
                {
                    try
                    {
                        var t = ((etymObj)ApplicationVariables.services.GetService(typeof(etymObj))).pofs;
                        _etymLang_all = new ps[t[0].Length];
                        t[0].CopyTo(_etymLang_all, 0);
                    }
                    catch (Exception ex)
                    {
                        _etymLang_all = null;
                    }
                }
                return _etymLang_all;
            }
        }
        private static SelectList _etympofs = null;
        public static SelectList etympofs
        {
            get
            {
                if (_etympofs == null)
                {
                    try
                    {
                        _etympofs = new SelectList(etymLang_all, "id", "name", null, null);
                    }
                    catch (Exception ex)
                    {
                        _etympofs = null;
                    }
                }
                return _etympofs;
            }
        }
    }
}
