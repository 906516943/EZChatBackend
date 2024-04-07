using ImageService.Core.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Core.Repos
{
    public interface IImageRepo
    {
        Task<string> FindImageIdFromHash(string hash);

        Task<bool> IsThumbnailImg(string imgId);

        Task InsertImageIdFromHash(string hash, string id, bool isThumnail);

        Task LinkImgAndThumbnailImg(string imgId, string thumnailImgId);

        Task<string> GetThumbnailImgId(string imgId);
    }

    public class ImageRepoRedis : IImageRepo
    {
        private readonly IDatabase _redis;
        private readonly RedisConfig _config;

        public ImageRepoRedis(IConnectionMultiplexer connection, IOptions<RedisConfig> config) 
        {
            _redis = connection.GetDatabase();
            _config = config.Value;
        }

        public async Task<string> FindImageIdFromHash(string hash)
        {
            var res = (string?)(await _redis.StringGetAsync("image:hash_id_lookup:" + hash));

            if (res is null)
                throw new InvalidOperationException("image id not found");

            //renew TTL
            await _redis.KeyExpireAsync("image:hash_id_lookup:" + hash, _config.TTL, CommandFlags.FireAndForget);
            return res;
        }

        public async Task<string> GetThumbnailImgId(string imgId)
        {
            var res = (string?)(await _redis.StringGetAsync("image:img_thumbnail_id:" + imgId));

            if (res is null)
                throw new InvalidOperationException("thumbnail Id not found");

            //renew TTL
            await _redis.KeyExpireAsync("image:img_thumbnail_id:" + imgId, _config.TTL, CommandFlags.FireAndForget);
            return res;
        }

        public async Task InsertImageIdFromHash(string hash, string id, bool isThumnail)
        {
            await _redis.StringSetAsync("image:hash_id_lookup:" + hash, id, _config.TTL);
            await _redis.StringSetAsync("image:img_type:" + id, isThumnail, _config.TTL);
        }

        public async Task<bool> IsThumbnailImg(string imgId)
        {
            var res = (bool?)(await _redis.StringGetAsync("image:img_type:" + imgId));

            if (res is null)
                throw new InvalidOperationException("image Id not found");

            //renew TTL
            await _redis.KeyExpireAsync("image:img_type:" + imgId, _config.TTL, CommandFlags.FireAndForget);
            return res!.Value;
        }

        public async Task LinkImgAndThumbnailImg(string imgId, string thumnailImgId)
        {
            await _redis.StringSetAsync("image:img_thumbnail_id:" + imgId, thumnailImgId, _config.TTL);
        }
    }
}
