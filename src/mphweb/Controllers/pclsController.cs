using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mphweb.FuncModule;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using mphdict;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace mphweb.Controllers
{
    [Route("api/pcls")]
    public class pclsController : Controller
    {
        private readonly ViewRender view;
        ILogger Logger { get; } = ApplicationLogging.CreateLogger<inflectionController>();
        mphObj db;
        public pclsController(ViewRender view, mphObj db)
        {
            this.view = view;
            this.db = db;
            this.db.Logger = Logger;
        }
        [HttpGet]
        [Route("GetView/{id}")]
        public async Task<IActionResult> GetView(short id)
        {
            var pcls = await db.getPClass(id);
            if(pcls==null) return NotFound(null);
            var html = RenderPartialViewToString("flexes/Index", pcls);
            //var html = view.Render("flexes/Index", pcls);
            return Ok(new pclsview() { html = html });
        }
        public string RenderPartialViewToString(string viewName, object model)
        {
            //var actionContextAccessor = this.HttpContext.RequestServices.GetService(typeof(IActionContextAccessor)) as IActionContextAccessor;
            var actionContext = GetActionContext();
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.ActionDescriptor.ActionName;

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                var engine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

                ViewEngineResult viewResult = engine.FindView(actionContext, viewName, false);

                ViewContext viewContext = new ViewContext(actionContext, viewResult.View, ViewData, TempData, sw, new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions());

                var t = viewResult.View.RenderAsync(viewContext);
                t.Wait();

                return sw.GetStringBuilder().ToString();
            }
        }
        private ActionContext GetActionContext()
        {
            return new ActionContext(this.HttpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        }
    }
    public class pclsview
    {
        public string html { get; set; }
    }
}
