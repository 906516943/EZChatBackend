using ImageService.Core.Core;
using ImageService.Core.Models;
using IronSoftware.Drawing;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Core.Services
{
    public interface IImageService 
    {
        Task<Image> MakeThumbnailImg(byte[] img);

        Task<Image> MakeImg(byte[] img);

        Task<Image> FindImgFromMd5(string md5);
    }

    public class ImageService : IImageService
    {
        private ImageServiceCore _core = new();
        private ImageConfig _config;

        public ImageService(IOptions<ImageConfig> config) 
        { 
            _config = config.Value;
        }

        public Task<Image> FindImgFromMd5(string md5)
        {
            throw new NotImplementedException();
        }

        public async Task<Image> MakeImg(byte[] img)
        {
            return new Image(_config, false, img);
        }

        public Task<Image> MakeThumbnailImg(byte[] img)
        {
            return Task.Run(() =>
            {
                using var bitmap = AnyBitmap.FromBytes(img);
                byte[] ret;

                //check to see if resize is needed
                if ((bitmap.Width > _config.ThumbnailMaxSize) || (bitmap.Height > _config.ThumbnailMaxSize))
                {
                    var reduceRatio = (double)_config.ThumbnailMaxSize / Math.Max(bitmap.Width, bitmap.Height);
                    var res = _core.ResizeBitmap(bitmap.GetRGBBuffer(), bitmap.Width, bitmap.Height, reduceRatio);

                    var resizedBitMap = AnyBitmap.LoadAnyBitmapFromRGBBuffer(res.Buffer, res.Width, res.Height)
                        .RotateFlip(AnyBitmap.RotateMode.Rotate180, AnyBitmap.FlipMode.None);

                    ret = resizedBitMap.ExportBytes(AnyBitmap.ImageFormat.Jpeg, _config.ThumbnailJpgQuality);   
                }

                ret = bitmap.ExportBytes(AnyBitmap.ImageFormat.Jpeg, _config.ThumbnailJpgQuality);

                return new Image(_config, true, ret);
            });
        }


    }
}
