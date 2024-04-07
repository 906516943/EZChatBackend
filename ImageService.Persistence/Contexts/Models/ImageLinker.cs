using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Persistence.Contexts.Models
{
    [Table("image_linker")]
    public class ImageLinker
    {
        [Key]
        [Required]
        public string ImgId { get; set; } = "";

        [Required]
        public string ThumImgId { get; set; } = "";
    }
}
