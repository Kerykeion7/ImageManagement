using System;
using System.Collections.Generic;
using System.Text;

namespace ImageManagement.Services
{
    public class ImageResult
    {
        public static ImageResult Successfull { get; set; } = new ImageResult(true);
        public static ImageResult Failed { get; set; } = new ImageResult(false);

        public bool Success { get; }
        public string ImgUrl { get; set; }

        public ImageResult(bool isSuccess)
        {
            Success = isSuccess;
        }
    }
}
