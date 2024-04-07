using ImageService.Core.Repos;
using ImageService.Persistence.Contexts;
using ImageService.Persistence.Contexts.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageService.Persistence
{
    public class ImageRepoDb : IImageRepo
    {
        private IImageContext _context;

        public ImageRepoDb(IImageContext context) 
        {
            _context = context;
        }

        public async Task<string> FindImageIdFromHash(string hash)
        {
            var rec = await _context.ImageIdLookups.FindAsync(hash);

            if(rec is null)
                throw new InvalidOperationException($"{hash} not found");

            return rec.Id;
        }

        public async Task<string> GetThumbnailImgId(string imgId)
        {
            var rec = await _context.ImageLinkers.FindAsync(imgId);

            if (rec is null)
                throw new InvalidOperationException("Image Id not found");

            return rec.ThumImgId;
        }

        public async Task InsertImageIdFromHash(string hash, string id, bool isThumnail)
        {
            _context.ImageIdLookups.Add(new ImageIdLookup() { Id = id, Hash = hash, IsThumnail = isThumnail });
            await _context.Ctx.SaveChangesAsync();
        }

        public async Task<bool> IsThumbnailImg(string imgId)
        {
            var res = await _context.ImageIdLookups.Where(x => x.Id == imgId).FirstOrDefaultAsync();

            if (res is null)
                throw new InvalidOperationException("Image Id not found");

            return res.IsThumnail;
        }

        public async Task LinkImgAndThumbnailImg(string imgId, string thumnailImgId)
        {
            _context.ImageLinkers.Add(new ImageLinker() {ImgId = imgId, ThumImgId = thumnailImgId});
            await _context.Ctx.SaveChangesAsync();
        }
    }
}
