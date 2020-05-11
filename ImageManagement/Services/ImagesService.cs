using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageManagement.Services
{
    public class ImagesService
    {
        #region ctor&fields
        private readonly IWebHostEnvironment _env;

        public ImagesService(IWebHostEnvironment env)
        {
            _env = env;
        }
        #endregion

        #region ImgCrudFunx
        public async Task<string> SaveImage(IFormFile file, string folderName)
        {
            return await ResizeImage(file, folderName);
        }

        public async Task<string> ReplaceImage(string oldFileName, IFormFile newFile, string folderName)
        {
            await DeleteImage(oldFileName, folderName);
            return await SaveImage(newFile, folderName);
        }

        public async Task DeleteImage(string oldFileName, string folderName)
        {
            await Task.Delay(0);
            string fullPath = Path.Combine(_env.WebRootPath, folderName, oldFileName);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
        #endregion

        #region HelperFuncs
        private async Task<string> ResizeImage(IFormFile file, string folderName)
        {
            await Task.Delay(0);
            Bitmap originalBMP = new Bitmap(file.OpenReadStream());

            double origWidth = originalBMP.Width;
            double origHeight = originalBMP.Height;

            double aspectRatio = origWidth / origHeight;

            if (aspectRatio <= 0)
                aspectRatio = 1;
      
            float newHeight = 600;

            float newWidth = (float)(newHeight * aspectRatio);

            Bitmap newBMP = new Bitmap(originalBMP, (int)newWidth, (int)newHeight);

            Graphics graphics = Graphics.FromImage(newBMP);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            graphics.DrawImage(originalBMP, 0, 0, newWidth, newHeight);

            var uniqueFileName = Guid.NewGuid().ToString("N") + file.FileName;
            var filePath = Path.Combine(_env.WebRootPath, folderName, uniqueFileName);
            var folderPath = Path.Combine(_env.WebRootPath, folderName);
            Directory.CreateDirectory(folderPath);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                newBMP.Save(fileStream, GetImageFormat(Path.GetExtension(file.FileName)));
            }

            originalBMP.Dispose();
            newBMP.Dispose();
            graphics.Dispose();

            return uniqueFileName;
        }

        private ImageFormat GetImageFormat(string extension)
        {
            switch (extension.ToLower())
            {
                case "jpg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                default:
                    break;
            }
            return ImageFormat.Jpeg;
        }
        #endregion
    }
}
