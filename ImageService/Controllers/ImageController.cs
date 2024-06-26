using ImageService.Core.Services;
using ImageService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ImageService.Controllers
{
    [ApiController]
    [Route("[controller]Api")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<ImageController> _logger;
        private readonly IImageService _imageService;

        public ImageController(ILogger<ImageController> logger, IImageService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }

        [HttpGet("FindImage")]
        [ProducesResponseType(typeof(ImageInfo), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> FindImage(string hash)
        {
            try
            {
                var img = await _imageService.FindImgFromHash(hash);

                if(img is null)
                    return NotFound();

                
                var imgId = img.Id;
                var thumbnailId = img.IsThumbnail ? null : (await img.GetThumbnail()).Id;

                return Ok(new ImageInfo(hash, imgId, thumbnailId));
            }
            catch (Exception e)
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpPost("FindImage")]
        [ProducesResponseType(typeof(List<ImageInfo?>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> FindImage([FromBody]List<string> hashes)
        {
            try
            {
                if (hashes.Count > 100)
                    throw new InvalidDataException("Single request cannot have more than 100 hashes");

                var imgs = await _imageService.FindImgFromHash(hashes);
                var ret = new List<ImageInfo>();

                for (int i = 0; i < hashes.Count; i++) 
                {
                    var img = imgs[i];

                    if (img is null)
                        continue;


                    var imgId = img.Id;
                    var thumbnailId = img.IsThumbnail ? null : (await img.GetThumbnail()).Id;

                    ret.Add(new ImageInfo(hashes[i], imgId, thumbnailId));
                }

                return Ok(ret);
            }
            catch (Exception e) 
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpGet("Image/{id}")]
        [ProducesResponseType(typeof(File), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetImage(string id, [FromQuery] bool thumbnail)
        {
            try
            {
                var img = await (thumbnail ? _imageService.MakeThumbnailImg(id) : _imageService.MakeImg(id));
                return File(await img.GetBytes(), "image/*");
            }
            catch (Exception e) 
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("Image")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PutImage() 
        {
            try
            {
                using var ms = new MemoryStream();
                await Request.Body.CopyToAsync(ms);
                var byteData = ms.ToArray();

                var img = await _imageService.MakeImg(byteData);
                var thumbnailImg = await _imageService.MakeThumbnailImg(byteData);

                await _imageService.PutImage(img, thumbnailImg);
                return Ok();
            }
            catch (Exception e) 
            {
                _logger.LogDebug(e.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
