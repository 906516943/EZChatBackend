using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Persistence.Contexts
{
    public interface IUserContext 
    { 
        DbContext Ctx { get; }

        DbSet<Models.EzGroup> Groups { get; set; }

        DbSet<Models.EzUser> Users { get; set; }

    }

    public class UserContext : DbContext, IUserContext
    {
        public UserContext(DbContextOptions options) :base(options)
        { 
            
        }

        public DbContext Ctx => this;

        public DbSet<Models.EzGroup> Groups { get; set; }

        public DbSet<Models.EzUser> Users { get; set; }
    }
}
