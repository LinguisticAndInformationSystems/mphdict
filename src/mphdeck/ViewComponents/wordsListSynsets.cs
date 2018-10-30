using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mphdict;
using mphdict.Models.SynonymousSets;
using mphdeck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdeck.ViewComponents
{
    public class wordsListSynsets : ViewComponent
    {
        synsetsContext db;
        public wordsListSynsets(synsetsContext db)
        {
            this.db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(syndictParams dp)
        {
            try
            {
                dp.page = await db.getPage(dp.f, dp.incp.currentPage, 100);

                return View("wordsListSynsets", dp);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<wordsListSynsets>().LogError(new EventId(0), ex, ex.Message);
                throw new Exception("Помилка БД. Зверніться до розробника");
            }
        }
    }
}
