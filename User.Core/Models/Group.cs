using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Core.Repos;

namespace User.Core.Models
{
    public class Group
    {
        private IUserRepo _repo;
        private Guid _id;
        
        public Guid Id { get => _id; }

        public Group(IUserRepo repo, Guid id) 
        {
            _repo = repo;
            _id = id;
        }


    }
}
