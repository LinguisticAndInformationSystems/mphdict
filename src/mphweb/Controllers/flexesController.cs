using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mphweb.Models;
using mphdict;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace mphweb.Controllers
{
    public class flexesController : Controller
    {
        ILogger Logger { get; } = ApplicationLogging.CreateLogger<inflectionController>();
        mphObj db;
        public flexesController(mphObj db)
        {
            this.db = db;
            this.db.Logger = Logger;

        }
        public async Task<IActionResult> Index(incParams incp, filter f, pclsfilter pclsf)
        {
            var dp = new dictParams() {
                gr = new grdictParams() { f = f, incp = incp },
                pcls = new pclsdictParams() { indents = db.indents, f = pclsf, pclsinfo = await db.getPClass(pclsf.pclassPcls) },
                vtype = viewtype.pclass
            };

            if (pclsf.ispofsPcls)
            {
                dp.pcls.indents = (from c in db.indents where c.gr_id == pclsf.pofsPcls select c).ToArray();
            }
            ViewBag.dp = dp;
            return View(dp.pcls.pclsinfo);
        }
    }
}
