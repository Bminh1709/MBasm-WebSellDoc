using MBasmProject.Filter.Helper;
using MBasmProject.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO.Compression;
using System.IO;

namespace MBasmProject.Areas.User.Controllers
{
    public class HomeController : Controller
    {
        // GET: User/Home
        [CustomAuthentication]
        public ActionResult Index()
        {
            using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
            {
                int userId = (int)Session["userId"];
                var userFound = db.Userpps.FirstOrDefault(u => u.Id == userId);
                return View(userFound);
            }  
        }
        [BlockDirectAccess]
        public ActionResult MyFiles()
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    int userId = (int)Session["userId"];
                    List<Order> lstAsms = db.Orders
                        .Include(o => o.Order_Detail)
                        .Include("Order_Detail.Assignment")
                        .Where(o => o.User_id == userId)
                        .ToList();
                    return View(lstAsms);
                }
            }
            catch (Exception)
            {

                return PartialView("Redirect");
            }
        }
        [BlockDirectAccess]
        public ActionResult SavedFiles()
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    // If using this - add "using System.Data.Entity;"
                    int userId = (int)Session["userId"];

                    var lstSavedAsm = db.SavedAsms
                        .Include(s => s.Assignment)
                        .Where(s => s.User_id == userId)
                        .ToList();
                    return View(lstSavedAsm);
                }
            }
            catch (Exception)
            {
                return PartialView("Redirect");
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
            catch (Exception)
            {
                return Json(new { success = false });
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
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    int userId = (int)Session["userId"];
                    List<Order> lstOrder = db.Orders.Where(s => s.User_id == userId).ToList();
                    return View(lstOrder);
                }
            }
            catch (Exception)
            {
                return PartialView("Redirect");
            }
        }
        [CustomAuthentication]
        [BlockDirectAccess]
        public ActionResult MyAccount()
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    //if (Session["userId"] != null)
                    //{
                        int user_id = (int)Session["userId"];
                        var userFound = db.Userpps.Find(user_id);
                        return View(userFound);
                    //}
                    //else
                    //{
                    //    return PartialView("Redirect");
                    //}
                }
            }
            catch (Exception)
            {
                return PartialView("Redirect");
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

        public FileResult DownloadFiles(string fileNamePdf, string fileNameDocx, string fileNamePptx)
        {
            var appDataFolder = Server.MapPath("~/App_Data");

            // Combine the appDataFolder with the file names to get the full physical paths
            string fullFilePathPdf = null;
            if (fileNamePdf != null)
            {
                fullFilePathPdf = Path.Combine(appDataFolder, "Filepdf", fileNamePdf);
            }
            string fullFilePathDocx = null;
            if (fileNameDocx != null)
            {
                fullFilePathDocx = Path.Combine(appDataFolder, "FileDocx", fileNameDocx);
            }
            string fullFilePathPptx = null;
            if (fileNamePptx != null)
            {
                fullFilePathPptx = Path.Combine(appDataFolder, "FilePptx", fileNamePptx);
            }

            var tempFolder = Server.MapPath("~/Temp"); // Create a temporary folder to store the zip file
            var zipFileName = Path.Combine(tempFolder, "Files.zip");

            // Check if Files.zip already exists and delete it if it does
            if (System.IO.File.Exists(zipFileName))
            {
                System.IO.File.Delete(zipFileName);
            }

            using (var zipArchive = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
            {
                if (System.IO.File.Exists(fullFilePathPdf))
                {
                    // Get the file name without the GUID part
                    var pdfFileNameWithoutGuid = Helper.RemoveGuidFromFileName(Path.GetFileName(fullFilePathPdf));

                    // Create a new entry in the zip archive using the modified file name
                    zipArchive.CreateEntryFromFile(fullFilePathPdf, pdfFileNameWithoutGuid);
                }

                if (System.IO.File.Exists(fullFilePathDocx))
                {
                    // Get the file name without the GUID part
                    var docxFileNameWithoutGuid = Helper.RemoveGuidFromFileName(Path.GetFileName(fullFilePathDocx));

                    // Create a new entry in the zip archive using the modified file name
                    zipArchive.CreateEntryFromFile(fullFilePathDocx, docxFileNameWithoutGuid);
                }

                if (System.IO.File.Exists(fullFilePathPptx))
                {
                    // Get the file name without the GUID part
                    var pptxFileNameWithoutGuid = Helper.RemoveGuidFromFileName(Path.GetFileName(fullFilePathPptx));

                    // Create a new entry in the zip archive using the modified file name
                    zipArchive.CreateEntryFromFile(fullFilePathPptx, pptxFileNameWithoutGuid);
                }
            }

            // Send the zip file to download
            byte[] fileBytes = System.IO.File.ReadAllBytes(zipFileName);
            return File(fileBytes, "application/zip", "Files.zip");
        }


        public ActionResult UpdateProfilePhoto()
        {
            try
            {
                using (MBasm_AssignmentsEntities db = new MBasm_AssignmentsEntities())
                {
                    int userId = (int)Session["userId"];
                    var userFound = db.Userpps.Find(userId);

                    if (userFound == null)
                    {
                        return Json(new { success = false, message = "User not found." });
                    }

                    // Delete the old avatar if it exists
                    if (!string.IsNullOrEmpty(userFound.Avatar))
                    {
                        string oldImagePath = Server.MapPath("~" + userFound.Avatar);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                    {
                        var file = Request.Files[0];
                        string rootFolder = Server.MapPath("~/assets/img/users/");
                        string fileName = Path.GetFileName(file.FileName);
                        string pathImage = Path.Combine(rootFolder, fileName);
                        file.SaveAs(pathImage);

                        string newPhoto = "/assets/img/users/" + fileName;

                        userFound.Avatar = newPhoto;
                        db.SaveChanges();

                        return Json(new { success = true });
                    }
                }

                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}