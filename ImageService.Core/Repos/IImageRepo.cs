using ImageService.Core.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Core.Repos
{
    public interface IImageRepo
    {
        Task<string> FindImageIdFromMd5(string md5);

        Task InsertImageIdFromMd5(string md5, string id);

        Task DeleteImageIdFromMd5(string md5);
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

        public async Task DeleteImageIdFromMd5(string md5)
        {
            await _redis.StringGetDeleteAsync("image:id_lookup:" + md5);
        }

        public async Task<string> FindImageIdFromMd5(string md5)
        {
            var res = (string?)(await _redis.StringGetAsync("image:id_lookup:" + md5));

            if (res is null)
                throw new InvalidOperationException("image id not found");

            //renew TTL
            await _redis.KeyExpireAsync("image:id_lookup:" + md5, _config.TTL, CommandFlags.FireAndForget);
            return res;
        }

        public async Task InsertImageIdFromMd5(string md5, string id)
        {
            await _redis.StringSetAsync("image:id_lookup:" + md5, id, _config.TTL);
        }
    }
}
