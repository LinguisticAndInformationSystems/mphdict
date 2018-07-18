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
    public class wordsListEtym : ViewComponent
    {
        ILogger Logger { get; } = ApplicationLogging.CreateLogger<wordsList>();
        etymObj db;
        public wordsListEtym(etymObj db)
        {
            this.db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(etymdictParams dp)
        {
            dp.page = await db.getPage(dp.f, dp.incp.currentPage, 100);

            return View("wordsListEtym", dp);
        }
    }
}
