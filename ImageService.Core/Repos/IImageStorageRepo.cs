using ImageService.Core.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<byte[]> GetImage(string id)
        {
            throw new NotImplementedException();
        }

        public async Task SetImage(string id, byte[] img)
        {
            
        }
    }



    public class RedisImageStorage : IImageStorageRepo
    {
        public async Task<byte[]> GetImage(string id)
        {
            throw new NotImplementedException();
        }

        public async Task SetImage(string id, byte[] img)
        {
            throw new NotImplementedException();
        }
    }
}
