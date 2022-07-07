using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Extensions
{
    public static class FileManager
    {
        public static bool ChechFileContentType(this IFormFile file, string contentType)
        {
            return file.ContentType != contentType;
        }

        public static bool CheckFileSize(this IFormFile file, double size)
        {
            return (file.Length / 1024) > size;
        }

        public async static Task<string> CreateFileAsync(this IFormFile file, IWebHostEnvironment env, params string[] folders)
        {
            string fileName = Guid.NewGuid().ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" + file.FileName;

            string fullPath = Path.Combine(env.WebRootPath);

            foreach (string folder in folders)
            {
                fullPath = Path.Combine(fullPath, folder);
            }

            fullPath = Path.Combine(fullPath,fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
