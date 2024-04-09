using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSender.Core.Models
{
    public record class ImageInfo(string hash, string ImgId, string? ThumbnailImgId);
}
