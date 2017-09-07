using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mvc.RenderViewToString;
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
        private readonly RazorViewToStringRenderer view;

        ILogger Logger { get; } = ApplicationLogging.CreateLogger<inflectionController>();
        mphObj db;
        public pclsController(RazorViewToStringRenderer view, mphObj db)
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
            var html = await view.RenderViewToStringAsync<pclass_info>("Views/flexes/Index.cshtml", pcls);
            return Ok(new pclsview() { html = html });
        }
    }
    public class pclsview
    {
        public string html { get; set; }
    }
}
