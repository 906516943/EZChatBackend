using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Persistence.Contexts.Models
{
    [Table("image_id_lookup")]
    [Index(nameof(Id))]
    public class ImageIdLookup
    {
        [Key]
        [Required]
        public string Hash { get; set; } = "";

        [Required]
        public string Id { get; set; } = "";

        [Required]
        public bool IsThumnail { get; set; } = false;
    }
}
