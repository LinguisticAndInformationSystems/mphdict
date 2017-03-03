using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mphdict;
using mphweb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphweb.ViewComponents
{
    public class wordsListSynsets: ViewComponent
    {
        ILogger Logger { get; } = ApplicationLogging.CreateLogger<wordsList>();
        synsetsObj db;
        public wordsListSynsets(synsetsObj db)
        {
            this.db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(syndictParams dp)
        {
            dp.page = await db.getPage(dp.f, dp.incp.currentPage, 100);
            
            return View("wordsListSynsets", dp);
        }
    }
}
