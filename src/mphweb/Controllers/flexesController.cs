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
            ViewBag.dp = new grdictParams() { f=f, incp=incp };
            ViewBag.vtype = viewtype.pclass;
            ViewBag.indents = db.indents;
            if (pclsf.ispofsPcls)
            {
                ViewBag.indents = (from c in db.indents where c.gr_id == pclsf.pofsPcls select c).ToArray();
            }
            pclass_info pcls = await db.getPClass(pclsf.pclassPcls);
            ViewBag.pclsf = pclsf;
            var a = Resources.idispl.bname_ua;
            return View(pcls);
        }
    }
}
