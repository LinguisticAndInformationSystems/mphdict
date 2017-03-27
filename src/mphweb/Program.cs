using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using mphdict.Models.morph;
using mphdict;
using Microsoft.AspNetCore.Mvc.Rendering;
using uSofTrod.generalTypes.Models;

namespace mphweb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
    public enum viewtype { dict, synsets, pclass, aclass, analyze, error };
    public static class variables
    {
        private static SelectList _pclass;
        public static SelectList pclass
        {
            get {
                if (_pclass == null)
                {
                    _pclass= new SelectList(((mphObj)ApplicationVariables.services.GetService(typeof(mphObj))).pclass);
                }
                return _pclass;
            }
        }
        private static SelectList _pofs =null;
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
                    catch (Exception ex) {
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

    }
}