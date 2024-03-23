using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Persistence.Contexts.Models
{
    [Table("ez_groups")]
    [Index(nameof(Name))]
    public class EzGroup
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public List<EzUser> Users { get; set; } = [];
    }
}
