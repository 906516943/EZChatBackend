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
    public interface IImageStorageRepo
    {
        Task<byte[]> GetImage(string id);
        Task SetImage(string id, byte[] img);
    }

    public class DiskImageStorage : IImageStorageRepo
    {
        private ImageConfig _config;
        
        public DiskImageStorage(IOptions<ImageConfig> config) 
        {
            _config = config!.Value;
        }

        public Task<byte[]> GetImage(string id)
        {
            return Task.Run(() =>
            {
                var path = MakePath(id);

                using (var fileStream = File.Open(path.Dir + path.File, FileMode.Open))
                {
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        return binaryReader.ReadBytes(int.MaxValue);
                    }
                }
            });
        }

        public Task SetImage(string id, byte[] img)
        {
            return Task.Run(() =>
            {
                var path = MakePath(id);
                Directory.CreateDirectory(path.Dir);

                using (var fileStream = File.Open(path.Dir + path.File, FileMode.Create))
                {
                    using (var binaryWriter = new BinaryWriter(fileStream))
                    {
                        binaryWriter.Write(img);
                    }
                }
            });
        }

        private (string Dir, string File) MakePath(string id) 
        {
            var path = $"{_config.BaseDirectory}{id.Substring(0, 4)}/{id.Substring(4, 8)}/{id.Substring(8, 12)}/{id.Substring(12, 16)}/{id.Substring(16, 20)}/{id.Substring(20, 24)}/{id.Substring(24, 28)}/";
            var file = $"{id.Substring(28)}";

            return (path, file);
        }
    }



    public class RedisImageStorage : IImageStorageRepo
    {
        private IDatabase _redis;
        private RedisConfig _redisConfig;

        public RedisImageStorage(IConnectionMultiplexer connection, IOptions<RedisConfig> redisConfig) 
        {
            _redis = connection.GetDatabase();
            _redisConfig = redisConfig.Value;
        }

        public async Task<byte[]> GetImage(string id)
        {
            var res = (byte[]?)(await _redis.StringGetAsync("image:data:" + id));

            if (res is null)
                throw new InvalidOperationException("image not found");

            //update TTL
            await _redis.KeyExpireAsync("image:data:" + id, _redisConfig.TTL, CommandFlags.FireAndForget);
            return res;
        }

        public async Task SetImage(string id, byte[] img)
        {
            await _redis.StringSetAsync("image:data:" + id, img, _redisConfig.TTL);
        }
    }
}
