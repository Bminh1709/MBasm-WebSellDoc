using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MBasmProject.Models;
using System.IO;
using System.Reflection;
using MBasmProject.Filter.Helper;
using System.Runtime.Remoting.Messaging;
using System.Data.Entity.Infrastructure;

namespace MBasmProject.Areas.Admin.Controllers
{
    [HandleError]
    public class SubjectController : Controller
    {
        // GET: Admin/Subject
        public ActionResult Category()
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    //Get list Cats
                    List<Category> categories = db.Categories.ToList();
                    return View(categories);
                }
            }
            catch (Exception e)
            {
                //ViewBag.error_msg = e.Message;
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
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
                catch (Exception e)
                {
                    //return Json(new { success = false });
                    return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
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
                catch (Exception e)
                {
                    // return Json(new { success = false });
                    return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
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
                catch (Exception e )
                {
                    //return Json(new { success = false });
                    return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
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
                catch (Exception e)
                {
                    //return Json(new { success = false });
                    return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
                }
            }
        }

        public ActionResult GetAsmById(int id)
        {
            using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
            {
                var assignment = db.Assignments.Find(id);

                if (assignment != null)
                {
                    var categoryName = assignment.Category != null ? assignment.Category.Name : string.Empty;

                    // Fix error: "The ObjectContext instance has been disposed and can no longer be used for operations that require a connection"
                    db.Entry(assignment).State = EntityState.Detached; // Detach the assignment object from the database context

                    return new CustomJsonResult
                    {
                        Data = new { data = assignment, categoryName }
                    };
                }

                return new CustomJsonResult
                {
                    Data = new { data = assignment }
                };
            }
        }

        public ActionResult UpdateAsm()
        {
            try
            {
                using(MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    List<Category> categories = db.Categories.ToList();
                    return View(categories);
                }    
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateAsm(Assignment updateAsm, HttpPostedFileBase[] AddedImgs, HttpPostedFileBase FileDocxUpdate, HttpPostedFileBase FilePdfUpdate, HttpPostedFileBase FilePptxUpdate)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var modelFound = db.Assignments.Find(updateAsm.Id);
                    if (modelFound != null)
                    {
                        modelFound.Title = updateAsm.Title;
                        modelFound.Description = updateAsm.Description;
                        modelFound.Category_id = updateAsm.Category_id;
                        modelFound.Grade = updateAsm.Grade;
                        modelFound.Price = updateAsm.Price;
                        modelFound.UpdatedDate = DateTime.Now;
                        modelFound.Hot = updateAsm.Hot ?? false;

                        List<string> uploadedFileNames = new List<string>();
                        foreach (HttpPostedFileBase file in AddedImgs)
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
                        string newListImgs = string.Join(",", uploadedFileNames);

                        if (string.IsNullOrEmpty(modelFound.Images))
                        {
                            modelFound.Images = newListImgs;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(newListImgs))
                            {
                                modelFound.Images += "," + newListImgs;
                            }    
                        }

                        // Handle file uploads
                        if (FileDocxUpdate != null)
                        {
                            if (modelFound.FileDocx != null)
                            {
                                Helper.DeleteFile("~/App_Data/FileDocx", modelFound.FileDocx);
                            }
                            // Handle DOCX file upload
                            if (Helper.IsValidFile(FileDocxUpdate, ".docx"))
                            {
                                string fileName = Helper.GenerateUniqueFileName(FileDocxUpdate.FileName);
                                string filePath = Path.Combine(Server.MapPath("~/App_Data/FileDocx"), fileName);
                                FileDocxUpdate.SaveAs(filePath);
                                modelFound.FileDocx = fileName;
                            }
                        }
                        if (FilePdfUpdate != null)
                        {
                            if (modelFound.FilePdf != null)
                            {
                                Helper.DeleteFile("~/App_Data/FilePdf", modelFound.FilePdf);
                            }
                            // Handle Pdf file upload
                            if (Helper.IsValidFile(FilePdfUpdate, ".pdf"))
                            {
                                string fileName = Helper.GenerateUniqueFileName(FilePdfUpdate.FileName);
                                string filePath = Path.Combine(Server.MapPath("~/App_Data/FilePdf"), fileName);
                                FilePdfUpdate.SaveAs(filePath);
                                modelFound.FilePdf = fileName;
                            }
                        }
                        if (FilePptxUpdate != null)
                        {
                            if (modelFound.FilePptx != null)
                            {
                                Helper.DeleteFile("~/App_Data/FilePptx", modelFound.FilePptx);
                            }
                            // Handle Pptx file upload
                            if (Helper.IsValidFile(FilePptxUpdate, ".pptx"))
                            {
                                string fileName = Helper.GenerateUniqueFileName(FilePptxUpdate.FileName);
                                string filePath = Path.Combine(Server.MapPath("~/App_Data/FilePptx"), fileName);
                                FilePptxUpdate.SaveAs(filePath);
                                modelFound.FilePptx = fileName;
                            }
                        }

                        db.SaveChanges();
                        return Json(new { success = true });
                    }    
                    else
                    {
                        return Json(new { success = false });
                    }
                }
            }
            catch (Exception e)
            {
                //return Json(new { success = false });
                return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
            }   
        }

        [HttpPost]
        public ActionResult DeleteImg(int id, string imgLink)
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    var asmFound = db.Assignments.Find(id);
                    string img = imgLink.Substring(imgLink.LastIndexOf('/') + 1);
                    if (asmFound != null)
                    {
                        string[] images = asmFound.Images.Split(',');
                        List<string> imageList = new List<string>(images);

                        foreach (string image in images)
                        {
                            if (image == img)
                            {
                                imageList.Remove(image);
                                break;
                            }
                        }
                        string imgPath = Request.MapPath("~/assets/img/Asm/" + img);
                        if (System.IO.File.Exists(imgPath))
                        {
                            System.IO.File.Delete(imgPath);
                        }
                        // Convert the list back to an array
                        images = imageList.ToArray();

                        // Join the array elements with commas to get the updated Images string
                        string updatedImages = string.Join(",", images);

                        // Update the asmFound.Images property with the updatedImages string
                        asmFound.Images = updatedImages;

                        // Save the changes to the database
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
                catch (Exception e)
                {
                    //return Json(new { success = false });
                    return RedirectToAction("Index", "Error", new { num = "", errorMsg = e.Message });
                }
            }
        }
    }
}