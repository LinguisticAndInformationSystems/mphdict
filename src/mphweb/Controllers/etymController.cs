using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mphdict;
using mphweb.Models;
using Microsoft.AspNetCore.Routing;

namespace mphweb.Controllers
{
    public class etymController : Controller
    {
        ILogger Logger { get; } = ApplicationLogging.CreateLogger<inflectionController>();
        etymObj db;
        public etymController(etymObj db)
        {
            this.db = db;
        }
        private async Task<etymdictParams> prepareData(etymincParams incp, etymfilter f)
        {
            etymdictParams dp = new etymdictParams() { incp = incp, f = f};
            if (incp.wid != 0)
            {
                dp.entry = await db.getEntry(incp.wid);
                dp.count = await db.CountWords(f);
                int count_plus = dp.count % 100;
                dp.maxpage = count_plus > 0 ? (dp.count / 100) + 1 : (dp.count / 100);
                if (dp.incp.currentPage >= dp.maxpage) dp.incp.currentPage = dp.maxpage - 1;
                if (dp.incp.currentPage < 0) dp.incp.currentPage = 0;
            }
            else
            {
                var w = await db.searchWord(f, "");
                if (w != null)
                {
                    incp.currentPage = w.wordsPageNumber;
                    dp.entry = await db.getEntry(incp.wid);
                    dp.count = w.CountOfWords;
                    int count_plus = dp.count % 100;
                    dp.maxpage = count_plus > 0 ? (dp.count / 100) + 1 : (dp.count / 100);
                    if (dp.incp.currentPage >= dp.maxpage) dp.incp.currentPage = dp.maxpage - 1;
                    if (dp.incp.currentPage < 0) dp.incp.currentPage = 0;
                }
            }
            return dp;
        }
        public async Task<IActionResult> Index(etymincParams incp, etymfilter f)
        {
            if ((incp.idclass == 0) && (f.isStrFiltering == false) && (f.isLang == false) && (f.isType == false) && (f.isHead == true))
            {
                return RedirectToAction("Search", routeValues: setParams(incp, f));
            }
            var dps = await prepareData(incp, f);
            ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
            return View(dps);
        }
        public async Task<IActionResult> toPrev(etymincParams incp, etymfilter f)
        {
            incp.currentPage = incp.currentPage - 1;
            var dps = await prepareData(incp, f);
            ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
            return View("Index", dps);
        }
        public async Task<IActionResult> toNext(etymincParams incp, etymfilter f)
        {
            incp.currentPage = incp.currentPage + 1;
            var dps = await prepareData(incp, f);
            ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
            return View("Index", dps);
        }
        public async Task<IActionResult> toPage(etymincParams incp, etymfilter f)
        {
            incp.currentPage = incp.currentPage - 1;
            var dps = await prepareData(incp, f);
            ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
            return View("Index", dps);
        }
        public async Task<ActionResult> Search(etymincParams incp, etymfilter f)
        {
            var dps = new etymdictParams() { incp = incp, f = f };
            var w = await db.searchWord(f, incp.wordSearch);
            if (w !=null)
            {
                incp.currentPage = w.wordsPageNumber;
                incp.idclass = w.id_e_classes;
                incp.wid = w.id;
                dps.count = w.CountOfWords;
                int count_plus = dps.count % 100;
                dps.maxpage = count_plus > 0 ? (dps.count / 100) + 1 : (dps.count / 100);
                if (dps.incp.currentPage >= dps.maxpage) dps.incp.currentPage = dps.maxpage - 1;
                if (dps.incp.currentPage < 0) dps.incp.currentPage = 0;
            }
            ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
            return Redirect(Url.Action("SearchWord", "etym",
                new { wid = incp.wid, isStrFiltering = f.isStrFiltering, str = f.str, isHead=f.isHead, isLang=f.isLang, langId=f.langId, isType=f.isType, typeId=f.typeId, currentPage = incp.currentPage, wordSearch = incp.wordSearch, idclass = incp.idclass, count = dps.count, maxpage = dps.maxpage }, null, null, $"wid-{incp.wid}"));
        }
        public async Task<ActionResult> SearchWord(etymincParams incp, etymfilter f, int count, int maxpage)
        {
            var dps = new etymdictParams() { incp = incp, f = f};
            dps.count = count;
            dps.maxpage = maxpage;
            dps.entry = await db.getEntry(incp.idclass);
            dps.w = await db.getWord(incp.wid);
            ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
            return View("Index", dps);
        }
        private RouteValueDictionary setParams(etymincParams p, etymfilter f)
        {
            RouteValueDictionary d = new RouteValueDictionary();
            d.Add(nameof(f.isStrFiltering), f.isStrFiltering);
            d.Add(nameof(f.str), f.str);
            d.Add(nameof(p.currentPage), p.currentPage);
            d.Add(nameof(p.wordSearch), p.wordSearch);
            d.Add(nameof(p.wid), p.wid);
            d.Add(nameof(p.idclass), p.idclass);
            d.Add(nameof(f.isLang), f.isLang);
            d.Add(nameof(f.langId), f.langId);
            d.Add(nameof(f.isType), f.isType);
            d.Add(nameof(f.typeId), f.typeId);
            d.Add(nameof(f.isHead), f.isHead);
            return d;
        }
    }
}