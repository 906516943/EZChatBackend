using Microsoft.IdentityModel.Tokens;

namespace Auth.Core
{
    public static class Utils
    {
        public static string MakeToken(int len) 
        {
            var guid = Guid.NewGuid();
            var bytes = new byte[len];
            var random = new Random();
            random.NextBytes(bytes);

            return Base64UrlEncoder.Encode(guid.ToByteArray().Concat(bytes).ToArray());
        }
    }
}
