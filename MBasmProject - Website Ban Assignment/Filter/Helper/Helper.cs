using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MBasmProject___Website_Ban_Assignment.Filter.Helper
{
    public class Helper
    {
        // Helper method to validate file and check its extension
        public static bool IsValidFile(HttpPostedFileBase file, string validExtension)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileExtension = Path.GetExtension(file.FileName)?.ToLower();
                return fileExtension == validExtension;
            }
            return false;
        }

        // Helper method to generate a unique file name
        public static string GenerateUniqueFileName(string fileName)
        {
            string uniqueFileName = Guid.NewGuid().ToString("N") + "_" + fileName;
            return uniqueFileName;
        }

        // Helper method to delete Files in Asm Model
        public static void DeleteFile(string fileRoot, string fileName)
        {
            string filePath = Path.Combine(HttpContext.Current.Server.MapPath(fileRoot), fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}