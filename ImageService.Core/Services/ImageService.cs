using ImageService.Core.Core;
using ImageService.Core.Models;
using ImageService.Core.Repos;
using IronSoftware.Drawing;
using Microsoft.Extensions.DependencyInjection;
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
        private IImageRepo _imgRedisRepo;
        private IImageRepo _imgDbRepo;
        private List<IImageRepo> _imgRepos;
        private List<IImageStorageRepo> _imgStorageRepos;

        public ImageService(IOptions<ImageConfig> config, 
            [FromKeyedServices("db")] IImageRepo dbRepo, 
            [FromKeyedServices("redis")] IImageRepo redisRepo, 
            [FromKeyedServices("disk")] IImageStorageRepo diskStorage,
            [FromKeyedServices("redis")] IImageStorageRepo redisStorage) 
        { 
            _config = config.Value;
            _imgRedisRepo = redisRepo;
            _imgDbRepo = dbRepo;

            _imgRepos = new List<IImageRepo>() {_imgRedisRepo, _imgDbRepo };
            _imgStorageRepos = new List<IImageStorageRepo>() { redisStorage, diskStorage };
        }

        public async Task<Image> FindImgFromMd5(string md5)
        {
            var res = await _imgRepos.AnyMethodAsync(x => x.FindImageIdFromMd5, (Func<string, Task<string>> x) => x(md5));

            if (res.Item == null)
                throw new InvalidOperationException("Image id not found");

            //cache to redis
            if (res.From == 1)
                await _imgRedisRepo.InsertImageIdFromMd5(md5, res.Item!);

            return new Image(_config, res.Item, _imgRepos, _imgStorageRepos);
        }


        public async Task<Image> MakeImg(byte[] img)
        {
            return new Image(_config, false, img, _imgRepos, _imgStorageRepos);
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

                return new Image(_config, true, ret, _imgRepos, _imgStorageRepos);
            });
        }


    }
}
