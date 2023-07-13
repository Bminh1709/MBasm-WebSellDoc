using MBasmProject.Filter.Helper;
using MBasmProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Rotativa;
using System.Timers;

namespace MBasmProject.Areas.User.Controllers
{
    public class CartController : Controller
    {
        // GET: User/Cart
        [CustomAuthentication]
        public ActionResult Index()
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    int userId = (int)Session["userId"];
                    List<Cart> lstCart = db.Carts.Include(s => s.Assignment)
                              .Include(s => s.Assignment.Category)
                              .Where(s => s.User_id == userId)
                              .ToList();
                    ViewData["lstCart"] = lstCart;
                    return View(lstCart);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
            }
        }
        [HttpPost]
        public ActionResult AddCart(int asmId)
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

                    var addCartFound = db.Carts.Count(s => s.Assignment_id == asmId && s.User_id == userId);

                    // Check if user purchased that assignment or not
                    List<Order_Detail> lstOrder = db.Order_Detail.Include(s => s.Order)
                        .Include(s => s.Assignment)
                        .Where(s => s.Order.User_id == userId)
                        .ToList();

                    foreach (var item in lstOrder)
                    {
                        if (item.Assignment_id == asmId)
                        {
                            return Json(new { success = false, purchase = true });
                        }
                    }

                    if (addCartFound > 0)
                    {
                        return Json(new { success = false, exist = true });
                    }

                    var CartAsm = new Cart
                    {
                        Assignment_id = asmId,
                        User_id = userId,
                    };

                    // Add the new entity to the DbSet
                    db.Carts.Add(CartAsm);
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
        public ActionResult DeleteCart(int id)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var cartFound = db.Carts.Find(id);
                    if (cartFound == null)
                    {
                        return Json(new { success = false });
                    }    
                    else
                    {
                        db.Carts.Remove(cartFound);
                        db.SaveChanges();

                        int user_id = (int)Session["userId"];
                        List<Cart> lstCart = db.Carts.Where(s => s.User_id == user_id).ToList();
                        decimal totalPrice = 0;
                        foreach (var item in lstCart)
                        {
                            if (item.Assignment.Price.HasValue)
                            {
                                totalPrice += item.Assignment.Price.Value;
                            }
                        }
                        return Json(new { success = true, totalPrice = totalPrice });
                    }    
                }    
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
            }
        }
        public ActionResult Payment()
        {
            using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
            {
                int numOfOrder = db.Orders.Count();
                ViewBag.OrderNum = numOfOrder + 1;
                return View();
            }
        }

        public ActionResult ExportPDF()
        {
            var pdfResult = new ViewAsPdf
            {
                ViewName = "Payment",
                FileName = "Payment.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
                CustomSwitches = "--print-media-type"
            };

            return pdfResult;
        }
        [HttpPost]
        public ActionResult Payment(string note)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    int user_id = (int)Session["userId"];
                    List<Cart> lstCart = db.Carts.Where(s => s.User_id == user_id).ToList();

                    if (lstCart.Count > 0)
                    {
                        // -- ORDER --
                        Order newOrder = new Order();
                        newOrder.User_id = user_id;
                        newOrder.Note = note;
                        newOrder.OrderDate = DateTime.Now;

                        decimal totalPrice = 0;
                        foreach (var item in lstCart)
                        {
                            if (item.Assignment.Price.HasValue)
                            {
                                totalPrice += item.Assignment.Price.Value;
                            }
                        }
                        newOrder.TotalPrice = totalPrice;
                        newOrder.Quanitty = lstCart.Count();
                        newOrder.Status = false;

                        db.Orders.Add(newOrder);

                        // -- ORDER DETAIL --
                        foreach (var item in lstCart)
                        {
                            Order_Detail newDetail = new Order_Detail();
                            newDetail.Order_id = newOrder.Id;
                            newDetail.Assignment_id = item.Assignment.Id;
                            newDetail.Price = item.Assignment.Price;
                            db.Order_Detail.Add(newDetail);
                        }

                        //foreach (var item in lstCart)
                        //{
                        //    db.Carts.Remove(item);
                        //}
                        db.SaveChanges();

                        return Json(new { success = true });
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

      
    }
}
