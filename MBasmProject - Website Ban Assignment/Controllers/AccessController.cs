using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MBasmProject.Models;

namespace MBasmProject.Controllers
{
    public class AccessController : Controller
    {
        // GET: Access
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(User model)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var modelFound = db.Users.Count(x => x.Gmail == model.Gmail);
                    if (modelFound == 0)
                    {
                        model.JoinDate = DateTime.Now;
                        model = db.Users.Add(model);
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                        return Json(new { success = false });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
            }
        }

        [HttpPost]
        public ActionResult Signin(string gmail, string password)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var user = db.Users.FirstOrDefault(x => x.Gmail == gmail && x.Password == password);
                    if (user != null)
                    {
                        Session["User"] = user;
                        return Json(new { success = true });
                    }
                    else
                        return Json(new { success = false });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
            }
        }

        public ActionResult Logout()
        {
            Session.Remove("User");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Home");
        }

    }
}