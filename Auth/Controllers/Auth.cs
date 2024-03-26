using Auth.Core;
using Auth.Core.Core;
using Auth.Core.Externals;
using Auth.Core.Models;
using Auth.Core.Services;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Net;

namespace Auth.Controllers
{
    [ApiController]
    [Route("[controller]Api")]
    public class Auth : Controller
    {
        private readonly ILogger<Auth> _logger;
        private readonly AuthCore _core;
        private readonly IAuthService _authService;
        private readonly IUserApi _userApi;

        public Auth(ILogger<Auth> logger, IAuthService service, IUserApi userApi) 
        {
            _logger = logger;
            _userApi = userApi;
            _core = new AuthCore();
            _authService = service;
        }

        [HttpPut("MakeGuestAuthToken")]
        [ProducesResponseType(typeof(string), ((int)HttpStatusCode.OK))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetGuestAuthToken()
        {
            try
            {
                var userInfo = new UserInfo(Name: $"User@" + (new Random()).Next());
                var userId = await _userApi.MakeUser(userInfo);

                var info = new AuthInfo(DateTime.Now.AddDays(30), userId);
                var auth = await _authService.MakeAuthRecord(info);

                return Ok(auth.Token);
            }
            catch (Exception e) 
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("AuthInfo")]
        [ProducesResponseType(typeof(AuthInfo), ((int)HttpStatusCode.OK))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAuthInfo(string token)
        {
            try 
            {
                var auth = await _authService.GetAuthRecord(token);

                return Ok(await auth.GetAuthInfo());
            }
            catch (Exception e)
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
