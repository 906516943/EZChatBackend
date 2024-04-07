using ImageService.Core.Core;
using ImageService.Core.Repos;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Core.Models
{
    public class Image
    {
        private bool _isThumbnail;
        private byte[]? _byteData;
        private ImageConfig _config;
        private List<IImageRepo> _idRepos;
        private List<IImageStorageRepo> _storageRepos;
        private string? _hash;
        private string _id;


        public string Id { get => _id; }

        public bool IsThumbnail { get => _isThumbnail; }

        public Image(ImageConfig config, bool thumbnail, byte[] data, List<IImageRepo> idRepos, List<IImageStorageRepo> storageRepos, string? id = null) 
        { 
            _config  = config;
            _isThumbnail = thumbnail;
            _byteData = data;
            _id = Guid.NewGuid().ToString().Replace("-", "");
            _idRepos = idRepos;
            _storageRepos = storageRepos;
        }

        public Image(ImageConfig config, bool thumbnail, string id, List<IImageRepo> idRepos, List<IImageStorageRepo> storageRepos) 
        {
            _config = config;
            _isThumbnail = thumbnail;
            _id = id;
            _idRepos = idRepos;
            _storageRepos = storageRepos;
        }

        public async Task<Image> GetThumbnail() 
        {
            var res = await _idRepos.AnyMethodAsync(x => x.GetThumbnailImgId, (Func<string, Task<string>> x) => x(_id));

            //if from disk, cache to redis
            if (res.From == 1) 
            {
                await _idRepos[0].LinkImgAndThumbnailImg(Id, res.Item);
            }

            return new Image(_config, true, res.Item, _idRepos, _storageRepos);
        }

        public async Task<string> GetHash() 
        {
            if (_hash is not null)
                return _hash;


            //gen new hash
            var byteData = await Get();
            _hash = await GenHash(byteData) + "-" + byteData.Length;

            return _hash;    
        }

        public async Task<byte[]> GetBytes() 
        {
            return await Get();
        }

        public async Task Save() 
        {
            var hash = await GetHash();
            var data = await GetBytes();


            //write hash to redis and database
            var res = await _idRepos.AllMethodsAsync(x => x.InsertImageIdFromHash, (Func<string, string, bool, Task> x) => x(hash, _id, _isThumbnail));
            if (!res)
                throw new InvalidOperationException("Save id faild");


            //save binary data to redis and disk
            res = await _storageRepos.AllMethodsAsync(x => x.SetImage, (Func<string, byte[], Task> x) => x(_id, data));
        }

        private async Task<byte[]> Get() 
        { 
            if(_byteData is not null)
                return _byteData;


            //read from redis and disk
            var res = await _storageRepos.AnyMethodAsync(x => x.GetImage, (Func<string, Task<byte[]>> y) => y(_id));


            //if from disk, cache to redis
            if (res.From == 1)
                await _storageRepos[0].SetImage(_id, res.Item);

            _byteData = res.Item;

            return res.Item;
        }

        private Task<string> GenHash(byte[] data) 
        {
            return Task.Run(() =>
            {
                using (var sha = SHA256.Create()) 
                {
                    var hash = sha.ComputeHash(data);
                    return Base64UrlEncoder.Encode(hash);
                }
            });
        }
    }
}
