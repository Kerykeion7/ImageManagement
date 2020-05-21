using System;
using System.Collections.Generic;
using System.Text;

namespace ImageManagement.Services
{
    public class ImageResult
    {
        public static ImageResult Successfull(string imgUrl = null) => CreateSuccessfullImageResult(imgUrl, true);

        public static ImageResult Failed { get; set; } = new ImageResult(false);

        public bool Success { get; }
        public string ImgUrl { get; set; }

        public ImageResult(bool isSuccess)
        {
            Success = isSuccess;
        }

        private static ImageResult CreateSuccessfullImageResult(string imgUrl, bool success)
        {
            return new ImageResult(success)
            {
                ImgUrl = imgUrl
            };
        }
    }
}
