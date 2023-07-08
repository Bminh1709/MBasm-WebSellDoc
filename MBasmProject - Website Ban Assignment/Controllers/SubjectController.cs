using MBasmProject.Filter.Helper;
using MBasmProject.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace MBasmProject.Controllers
{
    [HandleError]
    public class SubjectController : Controller
    {
        // GET: Subject
        public ActionResult Index(string filter, int? subject, string grade)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var query = db.Assignments.AsQueryable();

                    if (filter != null)
                    {
                        ViewBag.filter = filter;

                        if (!string.IsNullOrEmpty(grade))
                        {
                            query = query.Where(a => a.Grade == grade);
                            ViewBag.grade = grade;
                        }

                        if (subject.HasValue)
                        {
                            query = query.Where(a => a.Category_id == subject);
                            ViewBag.subject = subject;
                        }

                        query = query.Where(a => a.Title.Contains(filter));
                    }

                    var lstAsm = query.ToList();
                    return View(lstAsm);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error", new { num = "403", errorMsg = e.Message });
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