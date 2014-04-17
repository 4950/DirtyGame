using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TowerSite.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "About Our Project";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact CSCI 4950";

            return View();
        }
    }
}