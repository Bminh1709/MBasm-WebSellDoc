using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MBasmProject.Models;
using BCrypt.Net;

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
        public ActionResult Signup(Userpp model)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var modelFound = db.Userpps.Count(x => x.Gmail == model.Gmail);
                    if (modelFound == 0)
                    {
                        model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                        model.JoinDate = DateTime.Now;
                        model = db.Userpps.Add(model);
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
                    var user = db.Userpps.FirstOrDefault(x => x.Gmail == gmail);
                    if (user != null)
                    {
                        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                        if (isPasswordValid)
                        {
                            Session["User"] = user;
                            Session["userId"] = user.Id;
                            // false: valid only for the current session
                            FormsAuthentication.SetAuthCookie(user.Gmail, false);
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = false });
                        }
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
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Session.Remove("User");
                Session.Remove("userId");
                FormsAuthentication.SignOut();
            }
            return RedirectToAction("Index","Home");
        }

    }
}