using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mphdict;
using mphdict.Models.Etym;
using mphdeck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdeck.ViewComponents
{
    public class wordsListEtym : ViewComponent
    {
        etymContext db;
        public wordsListEtym(etymContext db)
        {
            this.db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(etymdictParams dp)
        {
            try
            {
                dp.page = await db.getPage(dp.f, dp.incp.currentPage, 100);

                return View("wordsListEtym", dp);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<wordsListEtym>().LogError(new EventId(0), ex, ex.Message);
                throw new Exception("Помилка БД. Зверніться до розробника");
            }
        }
    }
}
