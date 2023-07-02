using MBasmProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MBasmProject.Controllers
{
    [HandleError]
    public class SubjectController : Controller
    {
        // GET: Subject
        public ActionResult Index()
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    List<Assignment> lstAsm = db.Assignments.ToList();
                    return View(lstAsm);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult Detail(int id)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var model = db.Assignments.Find(id);
                    if (model != null)
                    {
                        // to access the specific properties of the related entity, need to cast the proxy class to the actual entity type
                        var category = model.Category;
                        var categoryName = category != null ? category.Name : string.Empty; //  accesses the Name property of the category object using

                        // Pass the model and categoryName to the view
                        ViewBag.CategoryName = categoryName;
                        return View(model);
                    } 
                    return RedirectToAction("Index"); 
                }    
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}