using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mphweb.Models;

namespace mphweb.Controllers
{
    public class CultureController : Controller
    {
        public IActionResult ChangeLang(string lang, string c, string a, string qp)
        {
            RouteData.Values["culture"] = lang;
            return Redirect($"{Url.Action(a, c)}{/*System.Net.WebUtility.HtmlEncode(*/qp/*)*/}");
        }
        public IActionResult ChangeRPageLang(string lang, string page, string qp)
        {
            RouteData.Values["culture"] = lang;
            return Redirect(Url.Page(page));
        }
    }
}
