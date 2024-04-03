using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Core.Models
{
    public class ImageConfig
    {
        public string BaseDirectory { get; set; } = "";

        public int ThumbnailMaxSize = 0;

        public int ThumbnailJpgQuality = 50;
    }
}
