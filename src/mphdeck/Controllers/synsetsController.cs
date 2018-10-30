using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mphdict;
using mphdeck.Models;
using Microsoft.AspNetCore.Routing;
using mphdict.Models.morph;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using mphdict.Models.SynonymousSets;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace mphdeck.Controllers
{
    public class synsetsController : Controller
    {
        synsetsContext db;
        public synsetsController(synsetsContext db)
        {
            this.db = db;
        }
        private string getStartWordId()
        {
            //var env = HttpContext.RequestServices.GetService(typeof(IHostingEnvironment));
            IConfiguration conf = (IConfiguration)HttpContext.RequestServices.GetService(typeof(IConfiguration));
            return variables.lang.id_lang == 1058 ? conf.GetValue<string>("start_ua_word") : conf.GetValue<string>("start_ru_word");
        }
        private async Task SearchData(string sw, synincParams incp, synsetsfilter f, syndictParams dp)
        {
            var w = await db.searchWord(f, sw);
            if (w != null)
            {
                incp.currentPage = w.wordsPageNumber;
                incp.idset = w.id_set;
                incp.wid = w.id;
                dp.entry = await db.getEntry(incp.idset);
                dp.w = dp.entry != null ? (from c in dp.entry._wlist where c.id == incp.wid select c.word).FirstOrDefault() : "";
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
        private async Task<syndictParams> prepaireData(synincParams incp, synsetsfilter f)
        {
            syndictParams dp = new syndictParams() { incp = incp, f = f, id_lang = db.lid.id_lang };
            if (incp.idset != 0)
            {
                dp.entry = await db.getEntry(incp.idset);
                dp.w = dp.entry != null ? (from c in dp.entry._wlist where c.id == incp.wid select c.word).FirstOrDefault() : ""; //await db.getWord(incp.wid);
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
        // GET: /<controller>/
        public async Task<IActionResult> Index(synincParams incp, synsetsfilter f)
        {
            try
            {
                if ((incp.idset == 0) && (f.isStrFiltering == false) && (f.ispofs == false))
                {
                    return RedirectToAction("Search", routeValues: setParams(incp, f));
                }
                var dps = await prepaireData(incp, f);
                ViewBag.dp = new dictParams() { syn = dps, vtype = viewtype.synsets };
                return View(dps);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<synsetsController>().LogError(new EventId(0), ex, ex.Message);
                return BadRequest("Зверніться до розробника");
            }
        }
        public async Task<IActionResult> toPrev(synincParams incp, synsetsfilter f)
        {
            try
            {
                incp.currentPage = incp.currentPage - 1;
                var dps = await prepaireData(incp, f);
                ViewBag.dp = new dictParams() { syn = dps, vtype = viewtype.synsets };
                return View("Index", dps);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<synsetsController>().LogError(new EventId(0), ex, ex.Message);
                return BadRequest("Зверніться до розробника");
            }
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
            try
            {
                incp.currentPage = incp.currentPage - 1;
                var dps = await prepaireData(incp, f);
                ViewBag.dp = new dictParams() { syn = dps, vtype = viewtype.synsets };
                return View("Index", dps);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<synsetsController>().LogError(new EventId(0), ex, ex.Message);
                return BadRequest("Зверніться до розробника");
            }
        }
        public async Task<ActionResult> Search(synincParams incp, synsetsfilter f)
        {
            try
            {
                var dps = new syndictParams() { f = f, incp = incp };
                await SearchData(incp.wordSearch, incp, f, dps);
                ViewBag.dp = new dictParams() { syn = dps, vtype = viewtype.synsets };
                return Redirect(Url.Action("SearchWord", "synsets",
                    new { wid = incp.wid, isStrFiltering = f.isStrFiltering, str = f.str, ispofs = f.ispofs, pofs = f.pofs, currentPage = incp.currentPage, wordSearch = incp.wordSearch, idset = incp.idset, count = dps.count, maxpage = dps.maxpage }, null, null, $"wid-{incp.wid}"));
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<synsetsController>().LogError(new EventId(0), ex, ex.Message);
                return BadRequest("Зверніться до розробника");
            }
        }
        public async Task<ActionResult> SearchWord(synincParams incp, synsetsfilter f, int count, int maxpage)
        {
            try
            {
                var dps = new syndictParams() { incp = incp, f = f, id_lang = db.lid.id_lang };
                dps.count = count;
                dps.maxpage = maxpage;
                dps.entry = await db.getEntry(incp.idset);
                dps.w = dps.entry != null ? (from c in dps.entry._wlist where c.id == incp.wid select c.word).FirstOrDefault() : "";
                ViewBag.dp = new dictParams() { syn = dps, vtype = viewtype.synsets };
                return View("Index", dps);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<synsetsController>().LogError(new EventId(0), ex, ex.Message);
                return BadRequest("Зверніться до розробника");
            }
        }
        private RouteValueDictionary setParams(synincParams p, synsetsfilter f)
        {
            RouteValueDictionary d = new RouteValueDictionary();
            d.Add(nameof(f.isStrFiltering), f.isStrFiltering);
            d.Add(nameof(f.str), f.str);
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
