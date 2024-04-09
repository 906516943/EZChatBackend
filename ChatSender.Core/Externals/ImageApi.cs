using ChatSender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSender.Core.Externals
{
    public interface IImageApi 
    {
        public Task<List<ImageInfo>> GetImageIdFromHash(List<string> hashes);
    }

    public class ImageApi : IImageApi
    {
        private readonly HttpClient _httpClient;

        public ImageApi(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<List<ImageInfo>> GetImageIdFromHash(List<string> hashes)
        {
            return await _httpClient.DoPost<List<ImageInfo>>($"/ImageApi/FindImage", hashes);
        }
    }
}
