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
using System.Reflection;

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
            return variables.lang.id_lang==1058?conf.GetValue<string>("start_ua_word"): conf.GetValue<string>("start_ru_word");
        }
        private async Task<grdictParams> prepaireData(incParams incp, filter f)
        {
            grdictParams dp = new grdictParams() { incp = incp, f = f, id_lang = db.lid.id_lang };
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
                incp.currentPage = w.wordsPageNumber;
                incp.wid = w.nom_old;
                dp.entry = await db.getEntry(incp.wid);
                dp.count = w.CountOfWords;
                int count_plus = dp.count % 100;
                dp.maxpage = count_plus > 0 ? (dp.count / 100) + 1 : (dp.count / 100);
                if (dp.incp.currentPage >= dp.maxpage) dp.incp.currentPage = dp.maxpage - 1;
                if (dp.incp.currentPage < 0) dp.incp.currentPage = 0;
            }
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
            var dpg = await prepaireData(incp, f);
            ViewBag.dp = new dictParams() { gr = dpg, vtype = viewtype.dict };
            return View(dpg);
        }
        public async Task<IActionResult> toPrev(incParams incp, filter f)
        {
            incp.currentPage = incp.currentPage-1;
            var dpg = await prepaireData(incp, f);
            ViewBag.dp = new dictParams() { gr = dpg, vtype = viewtype.dict };
            return View("Index", dpg);
        }
        public async Task<IActionResult> toNext(incParams incp, filter f)
        {
            incp.currentPage = incp.currentPage + 1;
            var dpg = await prepaireData(incp, f);
            ViewBag.dp = new dictParams() { gr = dpg, vtype = viewtype.dict };
            return View("Index", dpg);
        }
        public async Task<IActionResult> toPage(incParams incp, filter f)
        {
            incp.currentPage = incp.currentPage-1;
            var dpg = await prepaireData(incp, f);
            ViewBag.dp = new dictParams() { gr = dpg, vtype = viewtype.dict };
            return View("Index", dpg);
        }
        public async Task<ActionResult> Search(incParams incp, filter f)
        {
            var w = await db.searchWord(f, incp.wordSearch);
            incp.currentPage = w.wordsPageNumber;
            incp.wid = w.nom_old;
            var dpg = new grdictParams() { incp = incp, f = f};
            dpg.count = w.CountOfWords;
            int count_plus = dpg.count % 100;
            dpg.maxpage = count_plus > 0 ? (dpg.count / 100) + 1 : (dpg.count / 100);
            if (dpg.incp.currentPage >= dpg.maxpage) dpg.incp.currentPage = dpg.maxpage - 1;
            if (dpg.incp.currentPage < 0) dpg.incp.currentPage = 0;

            ViewBag.dp = new dictParams() { gr = dpg, vtype = viewtype.dict };
            return Redirect(Url.Action("SearchWord", "inflection", 
                new { isStrFiltering= f.isStrFiltering, str=f.str, isInverse = f.isInverse, ispclass=f.ispclass, pclass=f.pclass, ispofs = f.ispofs, pofs = f.pofs, currentPage= incp.currentPage, wordSearch= incp.wordSearch, wid= incp.wid, count= dpg.count, maxpage = dpg.maxpage }, null, null, $"wid-{incp.wid}"));

        }
        public async Task<ActionResult> SearchWord(incParams incp, filter f, int count, int maxpage)
        {
            //global::System.Resources.ResourceManager rm = new global::System.Resources.ResourceManager("mphweb.Resources.idispl", typeof(Resources.idispl).GetTypeInfo().Assembly);
            //System.Resources.ResourceManager rm = new System.Resources.ResourceManager(typeof(mphweb.Resources.idispl));
            //System.Resources.ResourceManager rm = System.Resources.ResourceManager("mphweb.Resources.idispl", typeof(Resources.idispl).GetTypeInfo().Assembly);
            //string test2 = mphweb.Resources.idispl.ResourceManager.GetString("bname_ua", new System.Globalization.CultureInfo("uk"));
            //string test1 = mphweb.Resources.idispl.ResourceManager.GetString("bname_ua", new System.Globalization.CultureInfo("en"));
            //System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("MyResource", Assembly.GetExecutingAssembly
            //string test2 = rm.GetString("bname_ua", new System.Globalization.CultureInfo("uk"));
            //string test1 = rm.GetString("bname_ua", new System.Globalization.CultureInfo("en"));

            var dpg = new grdictParams() { incp = incp, f = f, id_lang = db.lid.id_lang };
            dpg.count=count;
            dpg.maxpage = maxpage;
            dpg.entry = await db.getEntry(incp.wid);
            ViewBag.dp = new dictParams() { gr = dpg, vtype = viewtype.dict };
            return View("Index", dpg);
        }
        private RouteValueDictionary setParams(incParams p, filter f)
        {
            RouteValueDictionary d = new RouteValueDictionary();
            d.Add(nameof(f.isStrFiltering), f.isStrFiltering);
            d.Add(nameof(f.str), f.str);
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
