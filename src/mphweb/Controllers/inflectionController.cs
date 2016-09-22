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
    public class inflectionController : Controller
    {
        ILogger Logger { get; } = ApplicationLogging.CreateLogger<inflectionController>();
        mphObj db;
        public inflectionController(mphObj db) {
            this.db = db;
            this.db.Logger = Logger;
        }
        private string getStartWordId()
        {
            //var env = HttpContext.RequestServices.GetService(typeof(IHostingEnvironment));
            IConfiguration conf = (IConfiguration)HttpContext.RequestServices.GetService(typeof(IConfiguration));
            return conf.GetValue<string>("start_ua_word");
        }
        private async Task<dictParams> prepaireData(incParams incp, filter f)
        {
            dictParams dp = new dictParams() { incp = incp, f = f, id_lang = db.lid.id_lang };
            if (incp.wid != 0) dp.entry = await db.getEntry(incp.wid);
            dp.count = await db.CountWords(f);
            int count_plus = dp.count % 100;
            dp.maxpage = count_plus>0? (dp.count / 100)+1: (dp.count / 100);
            if (dp.incp.currentPage >= dp.maxpage) dp.incp.currentPage = dp.maxpage-1;
            if (dp.incp.currentPage < 0) dp.incp.currentPage = 0;
            return dp;
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index(incParams incp, filter f)
        {
            if ((incp.wid == 0)&&(f.isStrFiltering==false)&&(f.ispclass == false) && (f.ispofs == false))
            {
                incp.wordSearch = getStartWordId();
                return RedirectToAction("Search", routeValues: setParams(incp, f));
            }
            var dp = await prepaireData(incp, f);
            ViewBag.dp = dp;
            ViewBag.vtype = viewtype.dict;
            return View(dp);
        }
        public async Task<IActionResult> toPrev(incParams incp, filter f)
        {
            incp.currentPage = incp.currentPage-1;
            var dp = await prepaireData(incp, f);
            ViewBag.dp = dp;
            ViewBag.vtype = viewtype.dict;
            return View("Index", dp);
        }
        public async Task<IActionResult> toNext(incParams incp, filter f)
        {
            incp.currentPage = incp.currentPage + 1;
            var dp = await prepaireData(incp, f);
            ViewBag.dp = dp;
            ViewBag.vtype = viewtype.dict;
            return View("Index", dp);
        }
        public async Task<IActionResult> toPage(incParams incp, filter f)
        {
            incp.currentPage = incp.currentPage-1;
            var dp = await prepaireData(incp, f);
            ViewBag.dp = dp;
            ViewBag.vtype = viewtype.dict;
            return View("Index", dp);
        }
        public async Task<ActionResult> Search(incParams incp, filter f)
        {
            var w = await db.searchWord(f, incp.wordSearch);
            incp.currentPage = w.wordsPageNumber;
            incp.wid = w.nom_old;
            var dp = new dictParams() { incp = incp, f = f};
            dp.count = w.CountOfWords;
            int count_plus = dp.count % 100;
            dp.maxpage = count_plus > 0 ? (dp.count / 100) + 1 : (dp.count / 100);
            if (dp.incp.currentPage >= dp.maxpage) dp.incp.currentPage = dp.maxpage - 1;
            if (dp.incp.currentPage < 0) dp.incp.currentPage = 0;

            ViewBag.dp = dp;
            return Redirect(Url.Action("SearchWord", "inflection", 
                new { isStrFiltering= f.isStrFiltering, str=f.str, fetchType=f.fetchType, isInverse = f.isInverse, ispclass=f.ispclass, pclass=f.pclass, ispofs = f.ispofs, pofs = f.pofs, currentPage= incp.currentPage, wordSearch= incp.wordSearch, wid= incp.wid, count= dp.count, maxpage = dp.maxpage }, null, null, $"wid-{incp.wid}"));

        }
        public async Task<ActionResult> SearchWord(incParams incp, filter f, int count, int maxpage)
        {
            var dp = new dictParams() { incp = incp, f = f, id_lang = db.lid.id_lang };
            dp.count=count;
            dp.maxpage = maxpage;
            dp.entry = await db.getEntry(incp.wid);
            ViewBag.dp = dp;
            ViewBag.vtype = viewtype.dict;
            return View("Index", dp);
        }
        private RouteValueDictionary setParams(incParams p, filter f)
        {
            RouteValueDictionary d = new RouteValueDictionary();
            d.Add(nameof(f.isStrFiltering), f.isStrFiltering);
            d.Add(nameof(f.str), f.str);
            d.Add(nameof(f.fetchType), (int)f.fetchType);
            d.Add(nameof(f.isInverse), f.isInverse);
            d.Add(nameof(p.currentPage), p.currentPage);
            d.Add(nameof(p.wordSearch), p.wordSearch);
            d.Add(nameof(p.wid), p.wid);
            d.Add(nameof(f.ispclass), f.ispclass);
            d.Add(nameof(f.pclass), f.pclass);
            d.Add(nameof(f.ispofs), f.ispclass);
            d.Add(nameof(f.pofs), f.pclass);
            return d;
        }

    }
}
