using System.Collections.Generic;

namespace ImageManagement.Services
{
    public class ImageResult
    {
        public static ImageResult Successful(string imgUrl = null) => CreateSuccessfulImageResult(imgUrl, true);

        public static ImageResult Failed(params UploadError[] errors) => new ImageResult(false, errors);

        public bool Success { get; }
        public string ImgUrl { get; set; }
        public IEnumerable<UploadError> Errors { get; set; }

        public ImageResult(bool isSuccess, params UploadError[] errors)
        {
            Success = isSuccess;
            Errors = errors;
        }

        private static ImageResult CreateSuccessfulImageResult(string imgUrl, bool success)
        {
            return new ImageResult(success)
            {
                ImgUrl = imgUrl
            };
        }
    }
}
