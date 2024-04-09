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

        Task<Image> MakeThumbnailImg(string id);

        Task<Image> MakeImg(byte[] img);

        Task<Image> MakeImg(string id);

        Task<Image?> FindImgFromHash(string hash);

        Task<List<Image?>> FindImgFromHash(List<string> hashes);

        Task PutImage(Image img, Image thumbnailImg);
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

        public async Task<Image?> FindImgFromHash(string hash)
        {
            try
            {
                //find image id & img type
                var imgId = await _imgRepos.AnyMethodAsync(x => x.FindImageIdFromHash, (Func<string, Task<string>> x) => x(hash));
                var imgType = await _imgRepos.AnyMethodAsync(x => x.IsThumbnailImg, (Func<string, Task<bool>> x) => x(imgId.Item));

                //cache to redis
                if (imgId.From == 1)
                    await _imgRedisRepo.InsertImageIdFromHash(hash, imgId.Item, imgType.Item);

                return new Image(_config, imgType.Item, imgId.Item, _imgRepos, _imgStorageRepos);
            }
            catch { }

            return null;
        }

        public async Task<List<Image?>> FindImgFromHash(List<string> hashes)
        {
            var ret = new List<Image?>();

            foreach (var hash in hashes)
            {
                var item = await FindImgFromHash(hash);
                ret.Add(item);
            }

            return ret;
        }

        public async Task<Image> MakeImg(byte[] img)
        {
            return new Image(_config, false, img, _imgRepos, _imgStorageRepos);
        }

        public async Task<Image> MakeImg(string id)
        {
            return new Image(_config, false, id, _imgRepos, _imgStorageRepos);
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
                    var resizedBitMap = AnyBitmap.LoadAnyBitmapFromRGBBuffer(res.Buffer, res.Width, res.Height);


                    ret = resizedBitMap.ExportBytes(AnyBitmap.ImageFormat.Jpeg, _config.ThumbnailJpgQuality);
                }
                else 
                {
                    ret = bitmap.ExportBytes(AnyBitmap.ImageFormat.Jpeg, _config.ThumbnailJpgQuality);
                }

                return new Image(_config, true, ret, _imgRepos, _imgStorageRepos);
            });
        }

        public async Task<Image> MakeThumbnailImg(string id)
        {
            return new Image(_config, true, id, _imgRepos, _imgStorageRepos);
        }

        public async Task PutImage(Image img, Image thumbnailImg)
        {
            //if img already exists
            var imgHash = await img.GetHash();
            if ((await FindImgFromHash(imgHash)) is not null)
                return;


            //save thumbnail & img
            await img.Save();
            await thumbnailImg.Save();


            //link thumbnail to img
            await _imgRepos.AllMethodsAsync(x => x.LinkImgAndThumbnailImg, (Func<string, string, Task> x) => x(img.Id, thumbnailImg.Id));
        }
    }
}
