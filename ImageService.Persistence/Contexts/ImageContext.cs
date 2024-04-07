using ImageService.Persistence.Contexts.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Persistence.Contexts
{
    public interface IImageContext
    {
        DbContext Ctx { get; }

        DbSet<ImageIdLookup> ImageIdLookups { get; set; }

        DbSet<ImageLinker> ImageLinkers { get; set; }
    }

    public class ImageContext : DbContext, IImageContext 
    { 
        public ImageContext(DbContextOptions options) : base(options) 
        { 
        
        }

        public DbContext Ctx => this;

        public DbSet<ImageIdLookup> ImageIdLookups { get; set; }

        public DbSet<ImageLinker> ImageLinkers { get; set; }
    }
}
