using Microsoft.AspNetCore.Mvc;
using System.Net;
using User.Core.Models;
using User.Core.Services;

namespace User.Controllers
{
    [ApiController]
    [Route("[controller]Api")]
    public class User : Controller
    {

        private readonly ILogger<User> _logger;
        private readonly IUserService _userService;

        public User(ILogger<User> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPut("User")]
        [ProducesResponseType(typeof(Guid), ((int)HttpStatusCode.OK))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MakeUser([FromBody]UserInfo userInfo)
        {
            try
            {
                var user = await _userService.MakeNewUser(userInfo);
                return Ok(user.Id);
            }
            catch (Exception e) 
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("User/{id}")]
        [ProducesResponseType(typeof(UserInfo), ((int)HttpStatusCode.OK))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserInfo(Guid id) 
        {
            try
            {
                var user = _userService.GetUser(id);
                return Ok(await user.GetInfo());
            }
            catch (Exception e)
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("User/{id}/Groups")]
        [ProducesResponseType(typeof(List<Guid>), ((int)HttpStatusCode.OK))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserGroups(Guid id)
        {
            try
            {
                var user = _userService.GetUser(id);
                return Ok((await user.GetGroups()).Select(x => x.Id).ToList());
            }
            catch (Exception e) 
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("Group/{id}")]
        [ProducesResponseType(typeof(List<GroupInfo>), ((int)HttpStatusCode.OK))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetGroupInfo(Guid id)
        {
            try
            {
                var group = _userService.GetGroup(id);
                return Ok(await group.GetInfo());
            }
            catch (Exception e)
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("Group/{id}/Users")]
        [ProducesResponseType(typeof(List<Guid>), ((int)HttpStatusCode.OK))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetGroupUsers(Guid id)
        {
            try
            {
                var group = _userService.GetGroup(id);
                return Ok((await group.GetUsers()).Select(x => x.Id).ToList());
            }
            catch (Exception e)
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
