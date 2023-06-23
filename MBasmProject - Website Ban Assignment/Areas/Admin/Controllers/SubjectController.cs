using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MBasmProject___Website_Ban_Assignment.Models;
using System.IO;
using System.Reflection;
using MBasmProject___Website_Ban_Assignment.Filter.Helper;
using System.Runtime.Remoting.Messaging;

namespace MBasmProject___Website_Ban_Assignment.Areas.Admin.Controllers
{
    public class SubjectController : Controller
    {
        // GET: Admin/Subject
        public ActionResult Category()
        {
            using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
            {
                //Get list Cats
                List<Category> categories = db.Categories.ToList();
                return View(categories);
            }
        }
        [HttpPost]
        public ActionResult AddCat(Category newCat)
        {
            using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
            {
                try
                {
                    int CatCount = db.Categories.Count(m => m.Id == newCat.Id || m.Name == newCat.Name);
                    if (CatCount == 0)
                    {
                        newCat = db.Categories.Add(newCat);
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }

                }
                catch (Exception)
                {
                    return Json(new { success = false });
                }
            }
        }
        public ActionResult GetCatById(int id)
        {
            MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities();
            var item = db.Categories.Find(id);
            return Json(new { data = item }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult UpdateCategory(Category category)
        {
            using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
            {
                try
                {
                    var categoryToUpdate = db.Categories.Find(category.Id);

                    if (categoryToUpdate != null)
                    {
                        categoryToUpdate.Name = category.Name;
                        categoryToUpdate.Description = category.Description;
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                catch (Exception)
                {
                    { return Json(new { success = false }); }
                }
            }
        }

        [HttpPost]
        public ActionResult DeleteCat(int id)
        {
            using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
            {
                try
                {
                    var DeleteCat = db.Categories.Find(id);
                    if (DeleteCat != null)
                    {
                        db.Categories.Remove(DeleteCat);
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                }
            }
        }




        public ActionResult Assignment()
        {
            using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
            {
                List<Assignment> lstAsm = db.Assignments.Include(a => a.Category).ToList();
                return View(lstAsm);
            }
        }
        [HttpPost]
        public ActionResult AddAsm(Assignment newAsm, HttpPostedFileBase[] images, HttpPostedFileBase fileDocx, HttpPostedFileBase filePdf, HttpPostedFileBase filePptx)
        {
            using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
            {
                try
                {
                    int AsmCount = db.Assignments.Count(m => m.Id == newAsm.Id);
                    if (AsmCount == 0)
                    {
                        if (!newAsm.Hot.HasValue)
                        {
                            newAsm.Hot = false; // Set the default value for an unchecked checkbox
                        }
                        List<string> uploadedFileNames = new List<string>();
                        foreach (HttpPostedFileBase file in images)
                        {
                            // Checking file is available to save
                            if (file != null && file.ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(file.FileName);
                                string filePath = Path.Combine(Server.MapPath("~/assets/img/Asm"), fileName);
                                file.SaveAs(filePath);
                                uploadedFileNames.Add(fileName);
                            }
                        }
                        // Save the file names in the model property
                        newAsm.Images = string.Join(",", uploadedFileNames);

                        // Handle DOCX file upload
                        if (Helper.IsValidFile(fileDocx, ".docx"))
                        {
                            string fileName = Helper.GenerateUniqueFileName(fileDocx.FileName);
                            string filePath = Path.Combine(Server.MapPath("~/App_Data/FileDocx"), fileName);
                            fileDocx.SaveAs(filePath);
                            newAsm.FileDocx = fileName;
                        }

                        // Handle PDF file upload
                        if (Helper.IsValidFile(filePdf, ".pdf"))
                        {
                            string fileName = Helper.GenerateUniqueFileName(filePdf.FileName);
                            string filePath = Path.Combine(Server.MapPath("~/App_Data/FilePdf"), fileName);
                            filePdf.SaveAs(filePath);
                            newAsm.FilePdf = fileName;
                        }

                        // Handle PPTX file upload
                        if (Helper.IsValidFile(filePptx, ".pptx"))
                        {
                            string fileName = Helper.GenerateUniqueFileName(filePptx.FileName);
                            string filePath = Path.Combine(Server.MapPath("~/App_Data/FilePptx"), fileName);
                            filePptx.SaveAs(filePath);
                            newAsm.FilePptx = fileName;
                        }
                        newAsm.UpdatedDate = DateTime.Now;
                        newAsm = db.Assignments.Add(newAsm);
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }

                }
                catch (Exception)
                {
                    return Json(new { success = false });
                }
            }
        }

        [HttpPost]
        public ActionResult DeleteAsm(int id)
        {
            using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
            {
                try
                {
                    var DeleteAsm = db.Assignments.Find(id);
                    if (DeleteAsm != null)
                    {
                        string[] images = DeleteAsm.Images.Split(',');
                        for (int i = 0; i < images.Length; i++)
                        {
                            string filePath = Path.Combine(Server.MapPath("~/assets/img/Asm"), images[i]);
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                        if (DeleteAsm.FileDocx != null)
                        {
                            Helper.DeleteFile("~/App_Data/FileDocx", DeleteAsm.FileDocx);
                        }
                        if (DeleteAsm.FilePdf != null)
                        {
                            Helper.DeleteFile("~/App_Data/FilePdf", DeleteAsm.FilePdf);
                        }
                        if (DeleteAsm.FilePptx != null)
                        {
                            Helper.DeleteFile("~/App_Data/FilePptx", DeleteAsm.FilePptx);
                        }
                        db.Assignments.Remove(DeleteAsm);
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                }
            }
        }
    }
}