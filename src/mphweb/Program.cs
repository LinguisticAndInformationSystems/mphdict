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
        private static SelectList _pofs=null;
        public static SelectList pofs
        {
            get
            {
                if (_pofs == null)
                {
                    _pofs = new SelectList(((mphObj)ApplicationVariables.services.GetService(typeof(mphObj))).pofs, "id", "name");
                }
                return _pofs;
            }
        }
    }
}