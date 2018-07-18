using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mphdeck.Models;

namespace mphdeck.Controllers
{
    public class ErrorController: Controller
    {
        public IActionResult StatusCodeInfo(int id)
        {
            var i = new ErrorInfo() { id = id };
            switch (id) {
                case 404:
                    i.verbalInfo = "Вибачте, але ресурс, який Ви шукали, не знайдений";
                    break;
                case 403:
                    i.verbalInfo = "Вибачте, але Ви не маєте прав для доступ до ресурсу";
                    break;
                default:
                    i.verbalInfo = "";
                    break;
            }
            ViewBag.dp = new dictParams() { ei= i, vtype = viewtype.error };
            return View(i); //Content($"Статуcний код помилки: {id}.");
        }
        public IActionResult Info()
        {
            var i = new ErrorInfo() { id = 500, verbalInfo = "В процесі роботи виникла помилка. Зверніться до адміністратора" };
            ViewBag.dp = new dictParams() { ei = i, vtype = viewtype.error };
            return View("StatusCodeInfo", i);
        }

    }
}
