using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMChatApp.Models;

namespace IMChatApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            string queryString = Request.QueryString["id"];
            ViewBag.Title = "Home Page";
            ViewBag.Username = string.IsNullOrWhiteSpace(queryString)?"Joorg":queryString;
            return View("Chat");
        }
        public ActionResult Chat(Login model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Title = "Home Page";
                ViewBag.Username = model.UserNick;
                return View();
            }
            return View("Index");
        }
    }
}
