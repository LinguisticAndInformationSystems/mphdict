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
        private async Task<dictParams> prepaireData(incParams incp, filter f)
        {
            dictParams dp = new dictParams() { incp = incp, f = f };
            dp.count = await db.CountWords(f);
            dp.maxpage = (dp.count / 100);
            if (dp.incp.currentPage < 0) dp.incp.currentPage = 0;
            if (dp.incp.currentPage > dp.maxpage) dp.incp.currentPage = dp.maxpage;
            return dp;
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index(incParams incp, filter f)
        {
            var dp = await prepaireData(incp, f);
            ViewBag.dp = dp;
            return View(dp);
        }
        public async Task<IActionResult> toPrev(incParams incp, filter f)
        {
            incp.currentPage = incp.currentPage - 1;
            var dp = await prepaireData(incp, f);
            ViewBag.dp = dp;
            return View("Index", dp);
        }
        public async Task<IActionResult> toNext(incParams incp, filter f)
        {
            incp.currentPage = incp.currentPage + 1;
            var dp = await prepaireData(incp, f);
            ViewBag.dp = dp;
            return View("Index", dp);
        }
        public async Task<IActionResult> toPage(incParams incp, filter f)
        {
            incp.currentPage = incp.currentPage - 1;
            var dp = await prepaireData(incp, f);
            ViewBag.dp = dp;
            return View("Index", dp);
        }
        public async Task<ActionResult> Search(incParams incp, filter f)
        {
            var w = await db.searchWord(f, incp.wordSearch);
            incp.currentPage = w.wordsPageNumber;
            incp.id = w.nom_old;
            var dp = new dictParams() { incp = incp, f = f};
            dp.count = w.CountOfWords;
            dp.maxpage = (dp.count / 100);
            ViewBag.dp = dp;
            return Redirect(Url.Action("SearchWord", "inflection", 
                new { isStrFiltering= f.isStrFiltering, str=f.str, fetchType=f.fetchType, isInverse = f.isInverse, currentPage= incp.currentPage, wordSearch= incp.wordSearch, id= incp.id, count= dp.count, maxpage = dp.maxpage }, null, null, $"wid-{incp.id}"));
            //return RedirectToAction("Index", routeValues: setParams(p, fl));

        }
        public ActionResult SearchWord(incParams p, filter fl, int count, int maxpage)
        {
            var dp = new dictParams() { incp = p, f = fl };
            dp.count=count;
            dp.maxpage = maxpage;
            ViewBag.dp = dp;
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
            d.Add(nameof(p.id), p.id);
            return d;
        }

    }
}
