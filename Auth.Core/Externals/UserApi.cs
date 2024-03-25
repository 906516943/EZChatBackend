using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Externals
{
    public interface IUserApi 
    { 
    
    }

    public class UserApi : IUserApi
    {
        private readonly HttpClient _httpClient;

        public UserApi(HttpClient httpClient) 
        { 
            _httpClient = httpClient;
        }
    }
}
