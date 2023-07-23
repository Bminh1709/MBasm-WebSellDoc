using MBasmProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MBasmProject.Areas.Admin.Controllers
{
    public class AccessController : Controller
    {
        // GET: Admin/Access
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(string gmail, string password)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var adminFound = db.Adminpps.FirstOrDefault(a => a.Gmail == gmail && a.Password == password);
                    if (adminFound != null)
                    {
                        // false: valid only for the current session
                        FormsAuthentication.SetAuthCookie(adminFound.Gmail, false);
                        return RedirectToAction("index", "Home");
                    }
                    else
                        return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
            }
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}