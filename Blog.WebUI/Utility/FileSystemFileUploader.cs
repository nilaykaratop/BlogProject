using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Blog.WebUI.Utility
{
    public class FileSystemFileUploader : IFileUpload
    {
        private readonly string _filePath;
        public FileSystemFileUploader(string filePath)
        {
            this._filePath = filePath;
        }
        public FileSystemFileUploader()
        {
            this._filePath = "images";
        }
        public FileUploadResult Upload(IFormFile file)
        {
            FileUploadResult result = new FileUploadResult();
            result.FileResult = FileResult.Error;
            result.Message = "Dosya yüklenemedi";
            if (file.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(file.FileName)}";
                result.OriginalName = file.FileName;
                var phsycalPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{_filePath}", fileName);
                if (File.Exists(phsycalPath))
                {
                    result.Message = "Dosya Mevcuttur";
                }
                else
                {
                    result.FileUrl = $"/{_filePath}/{fileName}";
                    result.Base64 = null;
                    try
                    {
                        using var stream = new FileStream(phsycalPath, FileMode.Create);
                        file.CopyTo(stream);
                        result.Message = "Başarıyla kaydedildi";
                        result.FileResult = FileResult.Succeded;
                    }
                    catch
                    {
                        result.Message = "Hata oluştu";
                        result.FileResult = FileResult.Error;
                    }
                }
            }
            return result;
        }
    }
}
