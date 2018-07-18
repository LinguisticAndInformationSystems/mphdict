using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mphdict;
using mphdeck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdeck.ViewComponents
{
    public class wordsList: ViewComponent
    {
        ILogger Logger { get; } = ApplicationLogging.CreateLogger<wordsList>();
        mphObj db;
        public wordsList(mphObj db)
        {
            this.db = db;
            this.db.Logger = Logger;
        }
        public async Task<IViewComponentResult> InvokeAsync(grdictParams dp)
        {
            dp.page = await db.getPage(dp.f, dp.incp.currentPage, 100);
            
            return View("wordsList", dp);
        }

    }
}
