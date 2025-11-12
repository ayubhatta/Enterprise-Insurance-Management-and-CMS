using System.IO;

namespace Enterprise_Insurance_Management___CMS_Platform.Helpers
{
    public static class FileHelper
    {
        private static readonly string RootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public static string GetUploadPath(string entityType, string fileType)
        {
            var entityFolder = entityType switch
            {
                "Policy" => "Policies",
                "Claim" => "Claims",
                "CustomerProfile" => "Customer",
                _ => throw new Exception("Invalid entity type")
            };

            var typeFolder = fileType switch
            {
                "pdf" => "pdfs",
                "image" => "images",
                _ => "others"
            };

            var fullPath = Path.Combine(RootDirectory, entityFolder, typeFolder);

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            return fullPath;
        }
        public static string GetFileType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            return ext switch
            {
                ".pdf" => "pdf",
                ".jpg" => "image",
                ".jpeg" => "image",
                ".png" => "image",
                ".bmp" => "image",
                ".gif" => "image",
                _ => "others"
            };
        }
    }
}