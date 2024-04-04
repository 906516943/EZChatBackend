using ImageService.Core.Core;
using ImageService.Core.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Core.Models
{
    public class Image
    {
        private bool? _isThumbnail;
        private byte[]? _byteData;
        private ImageConfig _config;
        private List<IImageRepo> _repos;

        private string _id;

        public Image(ImageConfig config, bool thumbnail, byte[] data, List<IImageRepo> repos) 
        { 
            _config  = config;
            _isThumbnail = thumbnail;
            _byteData = data;
            _id = Guid.NewGuid().ToString().Replace("-", "") + "_" + (thumbnail ? "0" : "1");
            _repos = repos;
        }

        public Image(ImageConfig config, string id, List<IImageRepo> repos) 
        {
            _config = config;
            _isThumbnail = id.Last() == '0';
            _id = id;
            _repos = repos;
        }


        public async Task Save() 
        {
            throw new NotImplementedException();
        }
    }
}
