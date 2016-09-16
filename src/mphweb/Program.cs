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
        private static /*List<ps[]>*/SelectList _pofs =null;
        public static /*List<ps[]>*/SelectList pofs
        {
            get
            {
                if (_pofs == null)
                {
                    try
                    {
                        var t = ((mphObj)ApplicationVariables.services.GetService(typeof(mphObj))).pofs;
                        //_pofs = t;
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
    }
}