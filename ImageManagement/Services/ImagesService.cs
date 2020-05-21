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
        /// <summary>
        /// Saves an image to the specified foldername under root path and returns a unique file name.
        /// </summary>
        /// <param name="file">The image to be saved.</param>
        /// <param name="folderName">The name of folder to place the image in (folder will be created if not yet exists). Default value is 'images'.</param>
        /// <returns></returns>
        public async Task<ImageResult> SaveImage(IFormFile file, string folderName = "images")
        {
            if (file != null)
            {
                return await ResizeAndUploadImage(file, folderName);
            }
            return ImageResult.Failed;
        }

        /// <summary>
        /// Deletes former image and replaces it with new image.
        /// </summary>
        /// <param name="oldFileName">Name of the former image.</param>
        /// <param name="newFile">New image</param>
        /// <param name="folderName">The name of folder where image is in and where new image will be in. Default value is 'images'.</param>
        /// <returns></returns>
        public async Task<ImageResult> ReplaceImage(string oldFileName, IFormFile newFile, string folderName = "images")
        {
            if (!IsAllowedExtension(newFile)) return ImageResult.Failed;

            var result = await DeleteImage(oldFileName, folderName);
            if (result.Success)
            {
                return await SaveImage(newFile, folderName);
            }

            return ImageResult.Failed;
        }

        /// <summary>
        /// Deletes an image.
        /// </summary>
        /// <param name="oldFileName">Name of the image to be deleted.</param>
        /// <param name="folderName">Name of the folder where image is in. Default value is 'images'.</param>
        /// <returns></returns>
        public async Task<ImageResult> DeleteImage(string oldFileName, string folderName = "images")
        {
            await Task.Delay(0);
            if (string.IsNullOrEmpty(oldFileName))
            {
                return ImageResult.Failed;
            }

            string fullPath = Path.Combine(_env.WebRootPath, folderName, oldFileName);
            if (File.Exists(fullPath)) 
            {
                File.Delete(fullPath);
            }

            return ImageResult.Successfull();
        }
        #endregion

        #region HelperFunx
        private async Task<ImageResult> ResizeAndUploadImage(IFormFile file, string folderName)
        {
            await Task.Delay(0);
            if (!IsAllowedExtension(file)) return ImageResult.Failed;

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
                newBMP.Save(fileStream, GetImageFormat(Path.GetExtension(file.FileName).Substring(1)));
            }

            originalBMP.Dispose();
            newBMP.Dispose();
            graphics.Dispose();

            return ImageResult.Successfull(uniqueFileName);
        }

        private ImageFormat GetImageFormat(string extension)
        {
            return (extension.ToLower()) switch
            {
                "jpg" => ImageFormat.Jpeg,
                "bmp" => ImageFormat.Bmp,
                "png" => ImageFormat.Png,
                _ => ImageFormat.Jpeg,
            };
        }

        private List<string> AllowedExtensions()
        {
            return new List<string>
            {
                "jpg",
                "bmp",
                "png"
            };
        }

        private bool IsAllowedExtension(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).Substring(1).ToLower();
            return AllowedExtensions().Contains(fileExtension);
        }
        #endregion
    }
}
