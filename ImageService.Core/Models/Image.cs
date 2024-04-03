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

        private string _id;

        public Image(ImageConfig config, bool thumbnail, byte[] data) 
        { 
            _config  = config;
            _isThumbnail = thumbnail;
            _byteData = data;
            _id = Guid.NewGuid().ToString().Replace("-", "") + "_" + (thumbnail ? "0" : "1");
        }

        public Image(ImageConfig config, string id) 
        {
            _config = config;
            _isThumbnail = id.Last() == '0';
            _id = id;

        }

        public async Task<byte[]> GetByte() 
        {
            throw new NotImplementedException();
        }

        public async Task Save() 
        {
            throw new NotImplementedException();
        }
    }
}
