using Auth.Core;
using Auth.Core.Core;
using Auth.Core.Models;
using Auth.Core.Services;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Net;

namespace Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Auth : Controller
    {
        private readonly ILogger<Auth> _logger;
        private readonly AuthCore _core;
        private readonly IAuthService _authService;

        public Auth(ILogger<Auth> logger, IAuthService service) 
        {
            _logger = logger;
            _core = new AuthCore();
            _authService = service;
        }

        [HttpGet("MakeGuestAuthToken")]
        [ProducesResponseType(typeof(string), ((int)HttpStatusCode.OK))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetGuestAuthToken()
        {
            try
            {
                var info = new AuthInfo(DateTime.Now.AddSeconds(30), Guid.NewGuid());
                var auth = await _authService.MakeAuthRecord(info);

                return Ok(auth.Token);
            }
            catch (Exception e) 
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("GetAuthInfo")]
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
