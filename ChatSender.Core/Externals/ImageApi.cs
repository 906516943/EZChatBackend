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
        public Task<ImageInfo> GetImageIdFromHash(string hash);
    }

    public class ImageApi : IImageApi
    {
        private readonly HttpClient _httpClient;

        public ImageApi(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<ImageInfo> GetImageIdFromHash(string hash)
        {
            return await _httpClient.DoGet<ImageInfo>($"/ImageApi/FindImage?hash={hash}");
        }
    }
}
