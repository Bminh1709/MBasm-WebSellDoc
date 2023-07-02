using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MBasmProject.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(int num, string errorMsg)
        {
            switch (num)
            {
                case 404:
                    ViewBag.error = "404";
                    ViewBag.error_msg = "The page you are looking for not avaible!";
                    break;
                case 500:
                    ViewBag.error = "500";
                    ViewBag.error_msg = "Internal Server Error!";
                    break;
                case 400:
                    ViewBag.error = "400";
                    ViewBag.error_msg = "Bad Request!";
                    break;
                default:
                    ViewBag.error = "Error";
                    ViewBag.error_msg = errorMsg;
                    break;
            }
            return View();
        }
    }
}