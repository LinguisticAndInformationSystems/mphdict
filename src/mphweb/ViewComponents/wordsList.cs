using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mphdict;
using mphdict.Models.morph;
using mphweb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphweb.ViewComponents
{
    public class wordsList: ViewComponent
    {
        mphContext db;
        public wordsList(mphContext db)
        {
            this.db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(grdictParams dp)
        {
            try
            {
                dp.page = await db.getPage(dp.f, dp.incp.currentPage, 100);

                return View("wordsList", dp);
            }
            catch (Exception ex)
            {
                ApplicationLogging.CreateLogger<wordsList>().LogError(new EventId(0), ex, ex.Message);
                throw new Exception("Помилка БД. Зверніться до розробника");
            }
        }

    }
}
