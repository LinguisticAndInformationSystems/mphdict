using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mphdict;
using mphweb.Models;
using Microsoft.AspNetCore.Routing;
using mphdict.Models.morph;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace mphweb.Controllers
{
    public class synsetsController : Controller
    {
        ILogger Logger { get; } = ApplicationLogging.CreateLogger<inflectionController>();
        synsetsObj db;
        public synsetsController(synsetsObj db) {
            this.db = db;
        }
        private string getStartWordId()
        {
            //var env = HttpContext.RequestServices.GetService(typeof(IHostingEnvironment));
            IConfiguration conf = (IConfiguration)HttpContext.RequestServices.GetService(typeof(IConfiguration));
            return variables.lang.id_lang==1058?conf.GetValue<string>("start_ua_word"): conf.GetValue<string>("start_ru_word");
        }
        private async Task<syndictParams> prepaireData(synincParams incp, synsetsfilter f)
        {
            syndictParams dp = new syndictParams() { incp = incp, f = f, id_lang = db.lid.id_lang };
            if (incp.idset != 0) dp.entry = await db.getEntry(incp.idset);
            dp.w = await db.getWord(incp.wid);
            dp.count = await db.CountWords(f);
            int count_plus = dp.count % 100;
            dp.maxpage = count_plus>0? (dp.count / 100)+1: (dp.count / 100);
            if (dp.incp.currentPage >= dp.maxpage) dp.incp.currentPage = dp.maxpage-1;
            if (dp.incp.currentPage < 0) dp.incp.currentPage = 0;
            return dp;
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index(synincParams incp, synsetsfilter f)
        {
            if ((incp.idset == 0)&&(f.isStrFiltering==false) && (f.ispofs == false))
            {
                return RedirectToAction("Search", routeValues: setParams(incp, f));
            }
            var dps = await prepaireData(incp, f);
            ViewBag.dp = new dictParams() { syn = dps, vtype = viewtype.synsets };
            return View(dps);
        }
        public async Task<IActionResult> toPrev(synincParams incp, synsetsfilter f)
        {
            incp.currentPage = incp.currentPage-1;
            var dps = await prepaireData(incp, f);
            ViewBag.dp = new dictParams() { syn = dps, vtype = viewtype.synsets };
            return View("Index", dps);
        }
        public async Task<IActionResult> toNext(synincParams incp, synsetsfilter f)
        {
            incp.currentPage = incp.currentPage + 1;
            var dps = await prepaireData(incp, f);
            ViewBag.dp = new dictParams() { syn = dps, vtype = viewtype.synsets };
            return View("Index", dps);
        }
        public async Task<IActionResult> toPage(synincParams incp, synsetsfilter f)
        {
            incp.currentPage = incp.currentPage-1;
            var dps = await prepaireData(incp, f);
            ViewBag.dp = new dictParams() { syn = dps, vtype = viewtype.synsets };
            return View("Index", dps);
        }
        public async Task<ActionResult> Search(synincParams incp, synsetsfilter f)
        {
            var w = await db.searchWord(f, incp.wordSearch);
            incp.currentPage = w.wordsPageNumber;
            incp.idset = w.id_set;
            incp.wid = w.id;
            var dps = new syndictParams() { incp = incp, f = f};
            dps.count = w.CountOfWords;
            int count_plus = dps.count % 100;
            dps.maxpage = count_plus > 0 ? (dps.count / 100) + 1 : (dps.count / 100);
            if (dps.incp.currentPage >= dps.maxpage) dps.incp.currentPage = dps.maxpage - 1;
            if (dps.incp.currentPage < 0) dps.incp.currentPage = 0;

            ViewBag.dp = new dictParams() { syn = dps, vtype = viewtype.synsets };
            return Redirect(Url.Action("SearchWord", "synsets", 
                new { wid= incp.wid, isStrFiltering= f.isStrFiltering, str=f.str, fetchType=f.fetchType, ispofs = f.ispofs, pofs = f.pofs, currentPage= incp.currentPage, wordSearch= incp.wordSearch, idset= incp.idset, count= dps.count, maxpage = dps.maxpage }, null, null, $"wid-{incp.wid}"));
        }
        public async Task<ActionResult> SearchWord(synincParams incp, synsetsfilter f, int count, int maxpage)
        {
            var dps = new syndictParams() { incp = incp, f = f, id_lang = db.lid.id_lang };
            dps.count=count;
            dps.maxpage = maxpage;
            dps.entry = await db.getEntry(incp.idset);
            dps.w = await db.getWord(incp.wid);
            ViewBag.dp = new dictParams() { syn = dps, vtype = viewtype.synsets };
            return View("Index", dps);
        }
        private RouteValueDictionary setParams(synincParams p, synsetsfilter f)
        {
            RouteValueDictionary d = new RouteValueDictionary();
            d.Add(nameof(f.isStrFiltering), f.isStrFiltering);
            d.Add(nameof(f.str), f.str);
            d.Add(nameof(f.fetchType), (int)f.fetchType);
            d.Add(nameof(p.currentPage), p.currentPage);
            d.Add(nameof(p.wordSearch), p.wordSearch);
            d.Add(nameof(p.wid), p.wid);
            d.Add(nameof(p.idset), p.idset);
            d.Add(nameof(f.ispofs), f.ispofs);
            d.Add(nameof(f.pofs), f.pofs);
            return d;
        }

    }
}
