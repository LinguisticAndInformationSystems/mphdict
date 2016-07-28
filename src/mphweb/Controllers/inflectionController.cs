using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mphdict;
using mphweb.Models;
using Microsoft.AspNetCore.Routing;

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
        // GET: /<controller>/
        public async Task<IActionResult> Index(incParams p, filter fl)
        {
            dictParams dp = new dictParams() { incp = p, f = fl };
            dp.count = await db.CountWords(fl);
            dp.maxpage = (dp.count / 100);
            if (dp.incp.currentPage < 0) dp.incp.currentPage = 0;
            if (dp.incp.currentPage > dp.maxpage) dp.incp.currentPage = dp.maxpage;
            ViewBag.dp = dp;
            return View(dp);
        }
        public IActionResult toPrev(incParams p, filter fl)
        {
            p.currentPage = p.currentPage - 1;
            return RedirectToAction("Index", routeValues: setParams(p, fl));
        }
        public IActionResult toNext(incParams p, filter fl)
        {
            p.currentPage = p.currentPage + 1;
            return RedirectToAction("Index", routeValues: setParams(p, fl));
        }
        public IActionResult toPage(incParams p, filter fl)
        {
            p.currentPage = p.currentPage - 1;
            return RedirectToAction("Index", routeValues: setParams(p, fl));
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
