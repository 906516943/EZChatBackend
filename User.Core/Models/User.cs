using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Core.Repos;

namespace User.Core.Models
{
    public class User
    {
        private Guid _id;
        private IUserRepo _repo;


        public User(IUserRepo repo, Guid id) 
        {
            _repo = repo;
            _id = id;
        }
    }
}
