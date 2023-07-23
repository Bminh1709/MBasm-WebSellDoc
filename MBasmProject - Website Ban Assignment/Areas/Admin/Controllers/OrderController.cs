using MBasmProject.Filter.Helper;
using MBasmProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MBasmProject.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        // GET: Admin/Order
        [CustomAuthenticationAdmin]
        public ActionResult Index(string filter)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    List<Order> lstApprovedOrders = new List<Order>();
                    if (filter != null)
                    {
                        ViewBag.AOrFilter = filter;
                        lstApprovedOrders = db.Orders.Where(s => s.Status == true && s.Userpp.Gmail.Contains(filter)).Include(s => s.Userpp).ToList();
                    }    
                    else
                    {
                        lstApprovedOrders = db.Orders.Where(s => s.Status == true).Include(s => s.Userpp).ToList();
                    }    
                    return View(lstApprovedOrders);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [CustomAuthenticationAdmin]
        public ActionResult Unapproved(string filter)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    List<Order> lstUnapprovedOrders = new List<Order>();
                    if (filter != null)
                    {
                        ViewBag.UAOrFilter = filter;
                        lstUnapprovedOrders = db.Orders.Where(s => s.Status == false && s.Userpp.Gmail.Contains(filter)).Include(s => s.Userpp).ToList();
                    }
                    else
                    {
                        lstUnapprovedOrders = db.Orders.Where(s => s.Status == false).Include(s => s.Userpp).ToList();
                    }
                    return View(lstUnapprovedOrders);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [CustomAuthenticationAdmin]
        public ActionResult ConfirmOrder(int id)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var orderFound = db.Orders.FirstOrDefault(s => s.Id == id);
                    if (orderFound != null)
                    {
                        orderFound.Status = true;
                        db.SaveChanges();
                        return Json( new { success = true });
                    }  
                    else
                    {
                        return Json(new { success = false });
                    }  
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
            }
        }
    }
}