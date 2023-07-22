using MBasmProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MBasmProject.Areas.Admin.Controllers
{
    public class ClientController : Controller
    {
        // GET: Admin/Client
        public ActionResult Index(string filter)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    List<Userpp> lstUsers = new List<Userpp> ();
                    if (filter != null)
                    {
                        ViewBag.Usfilter = filter;
                        lstUsers = db.Userpps.Where(g => g.Gmail.Contains(filter)).ToList();
                    }
                    else
                    {
                        lstUsers = db.Userpps.ToList();
                    }    

                    return View(lstUsers);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var userFound = db.Userpps.FirstOrDefault(u => u.Id == id);
                    if (userFound != null)
                    {
                        db.Userpps.Remove(userFound);
                        db.SaveChanges();

                        return Json( new { success = true });
                    }
                    else
                        return Json(new { success = false });

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult SetActive(int id, bool active)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var userFound = db.Userpps.FirstOrDefault(u => u.Id == id);
                    if (userFound != null)
                    {
                        userFound.Active = active;
                        db.SaveChanges();
                        return Json( new { success = true, activeCheck = active  });;
                    }
                    else 
                        return Json( new { success = false });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}