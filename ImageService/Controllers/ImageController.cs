using Microsoft.AspNetCore.Mvc;

namespace ImageService.Controllers
{
    [ApiController]
    [Route("[controller]Api")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<ImageController> _logger;

        public ImageController(ILogger<ImageController> logger)
        {
            _logger = logger;
        }

        [HttpGet("FindImage")]
        public IActionResult FindImage()
        {
            throw new NotImplementedException();
        }


        [HttpGet("Image/{id}")]
        public IActionResult GetImage(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPut("Image/{id}")]
        public IActionResult PutImage(Guid id) 
        {
            throw new NotImplementedException();
        }
    }
}
