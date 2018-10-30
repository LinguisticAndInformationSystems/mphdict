using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mphdict;
using mphweb.Models;
using Microsoft.AspNetCore.Routing;
using mphdict.Models.Etym;

namespace mphweb.Controllers
{
    public class etymController : Controller
    {
        etymContext db;
        public etymController(etymContext db)
        {
            this.db = db;
        }
        private async Task SearchData(string sw, etymincParams incp, etymfilter f, etymdictParams dp)
        {
            var w = await db.searchWord(f, sw);
            if (w != null)
            {
                incp.currentPage = w.wordsPageNumber;
                incp.idclass = w.id_e_classes;
                incp.wid = w.id;
                dp.entry = await db.getEntry(incp.idclass);
                dp.w = dp.entry != null ? dp.entry.e_classes.Where(c => c.id == incp.idclass).FirstOrDefault().etymons.Where(c => c.id == incp.wid).FirstOrDefault().word:"";
                dp.count = w.CountOfWords;
                int count_plus = dp.count % 100;
                dp.maxpage = count_plus > 0 ? (dp.count / 100) + 1 : (dp.count / 100);
                if (dp.incp.currentPage >= dp.maxpage) dp.incp.currentPage = dp.maxpage - 1;
                if (dp.incp.currentPage < 0) dp.incp.currentPage = 0;
            }
            else
            {
                incp.currentPage = 0;
                incp.wid = 0;
                dp.entry = null;
                dp.count = 0;
                dp.maxpage = 0;
                dp.incp.currentPage = 0;
            }
        }
        private async Task<etymdictParams> prepareData(etymincParams incp, etymfilter f)
        {
            etymdictParams dp = new etymdictParams() { incp = incp, f = f};
            if (incp.idclass != 0)
            {
                dp.entry = await db.getEntry(incp.idclass);
                //dp.w = await db.getWord(incp.wid);
                dp.w = dp.entry != null ? dp.entry.e_classes.Where(c => c.id == incp.idclass).FirstOrDefault().etymons.Where(c => c.id == incp.wid).FirstOrDefault().word:"";
                dp.count = await db.CountWords(f);
                int count_plus = dp.count % 100;
                dp.maxpage = count_plus > 0 ? (dp.count / 100) + 1 : (dp.count / 100);
                if (dp.incp.currentPage >= dp.maxpage) dp.incp.currentPage = dp.maxpage - 1;
                if (dp.incp.currentPage < 0) dp.incp.currentPage = 0;
            }
            else
            {
                await SearchData(string.Empty, incp, f, dp);
            }
            return dp;
        }
        public async Task<IActionResult> Index(etymincParams incp, etymfilter f)
        {
            try
            {
                if ((incp.idclass == 0) && (f.isStrFiltering == false) && (f.isLang == false) && (f.isType == false) && (f.isHead == true))
                {
                    return RedirectToAction("Search", routeValues: setParams(incp, f));
                }
                var dps = await prepareData(incp, f);
                ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
                return View(dps);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<etymController>().LogError(new EventId(0), ex, ex.Message);
                return BadRequest("Зверніться до розробника");
            }
        }
        public async Task<IActionResult> toPrev(etymincParams incp, etymfilter f)
        {
            try
            {
                incp.currentPage = incp.currentPage - 1;
                var dps = await prepareData(incp, f);
                ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
                return View("Index", dps);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<etymController>().LogError(new EventId(0), ex, ex.Message);
                return BadRequest("Зверніться до розробника");
            }
        }
        public async Task<IActionResult> toNext(etymincParams incp, etymfilter f)
        {
            try
            {
                incp.currentPage = incp.currentPage + 1;
                var dps = await prepareData(incp, f);
                ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
                return View("Index", dps);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<etymController>().LogError(new EventId(0), ex, ex.Message);
                return BadRequest("Зверніться до розробника");
            }
        }
        public async Task<IActionResult> toPage(etymincParams incp, etymfilter f)
        {
            try
            {
                incp.currentPage = incp.currentPage - 1;
                var dps = await prepareData(incp, f);
                ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
                return View("Index", dps);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<etymController>().LogError(new EventId(0), ex, ex.Message);
                return BadRequest("Зверніться до розробника");
            }
        }
        public async Task<ActionResult> Search(etymincParams incp, etymfilter f)
        {
            try
            {
                var dps = new etymdictParams() { incp = incp, f = f };
                await SearchData(incp.wordSearch, incp, f, dps);
                ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
                return Redirect(Url.Action("SearchWord", "etym",
                    new { wid = incp.wid, isStrFiltering = f.isStrFiltering, str = f.str, isHead = f.isHead, isLang = f.isLang, langId = f.langId, isType = f.isType, typeId = f.typeId, currentPage = incp.currentPage, wordSearch = incp.wordSearch, idclass = incp.idclass, count = dps.count, maxpage = dps.maxpage }, null, null, $"wid-{incp.wid}"));
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<etymController>().LogError(new EventId(0), ex, ex.Message);
                return BadRequest("Зверніться до розробника");
            }
        }
        public async Task<ActionResult> SearchWord(etymincParams incp, etymfilter f, int count, int maxpage)
        {
            try
            {
                var dps = new etymdictParams() { incp = incp, f = f };
                dps.count = count;
                dps.maxpage = maxpage;
                dps.entry = await db.getEntry(incp.idclass);
                dps.w = dps.entry != null ? dps.entry.e_classes.Where(c => c.id == incp.idclass).FirstOrDefault().etymons.Where(c => c.id == incp.wid).FirstOrDefault().word : "";
                ViewBag.dp = new dictParams() { etym = dps, vtype = viewtype.etym };
                return View("Index", dps);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<etymController>().LogError(new EventId(0), ex, ex.Message);
                return BadRequest("Зверніться до розробника");
            }
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