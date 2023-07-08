using MBasmProject.Filter.Helper;
using MBasmProject.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace MBasmProject.Areas.User.Controllers
{
    public class HomeController : Controller
    {
        // GET: User/Home
        [CustomAuthentication]
        public ActionResult Index()
        {
            return View();
        }
        [BlockDirectAccess]
        public ActionResult MyFiles()
        {
            return View();
        }
        [BlockDirectAccess]
        public ActionResult SavedFiles()
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    // If using this - add "using System.Data.Entity;"
                    List<SavedAsm> lstSavedAsm = db.SavedAsms.Include(s => s.Assignment).ToList();
                    return View(lstSavedAsm);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
            }
        }

        [HttpPost]
        public ActionResult SavedFiles(int asmId)
        {
            if (Session["User"] == null)
            {
                return Json(new { success = false });
            }

            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    int userId = (int)Session["userId"];
                    var userFound = db.Userpps.Find(userId);

                    if (userFound == null)
                    {
                        return Json(new { success = false });
                    }

                    var savedFileFound = db.SavedAsms.Count(s => s.Assignment_id == asmId && s.User_id == userId);

                    if (savedFileFound > 0)
                    {
                        return Json(new { success = false, exist = true });
                    }

                    var savedAsm = new SavedAsm
                    {
                        Assignment_id = asmId,
                        User_id = userId,
                    };

                    // Add the new entity to the DbSet
                    db.SavedAsms.Add(savedAsm);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteSavedFile(int id)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    int userId = (int)Session["userId"];
                    var SavedFileFound = db.SavedAsms.FirstOrDefault(s => s.Assignment_id == id && s.User_id == userId);
                    if (SavedFileFound != null)
                    {
                        db.SavedAsms.Remove(SavedFileFound);
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                        return Json(new { success = true });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [BlockDirectAccess]
        public ActionResult Order()
        {
            return View();
        }
        [BlockDirectAccess]
        public ActionResult MyAccount()
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    if (Session["userId"] != null)
                    {
                        int user_id = (int)Session["userId"];
                        var userFound = db.Userpps.Find(user_id);
                        return View(userFound);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
            }
        }
        [HttpPost]
        public ActionResult MyAccount(Userpp model, string NewPassword, string OldPassword) 
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    int user_id = (int)Session["userId"];
                    var userFound = db.Userpps.Find(user_id);
                    if (userFound != null)
                    {
                        userFound.Fullname = model.Fullname;
                        userFound.Gmail = model.Gmail;
                        if (String.IsNullOrEmpty(NewPassword))
                        {
                            db.SaveChanges();
                            return Json(new { success = true });
                        }
                        else
                        {
                            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(OldPassword, userFound.Password);
                            if (isPasswordValid)
                            {
                                userFound.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
                            }
                            else
                            {
                                return Json(new { success = false, incorrectPass = true });
                            }
                            db.SaveChanges();
                            return Json(new { success = true });
                        }
                    }
                    return Json(new { success = false });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}