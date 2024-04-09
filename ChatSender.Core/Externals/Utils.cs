using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatSender.Core.Externals
{
    public static class Utils
    {
        public async static Task<T> DoGet<T>(this HttpClient client, string endPointPath) 
        {
            var respone = await client.GetAsync(endPointPath);

            if (!respone.IsSuccessStatusCode)
                throw new InvalidDataException($"Failed to access {endPointPath}");

            var str = await respone.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(str, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async static Task<T> DoPost<T>(this HttpClient client, string endPointPath, object body)
        {
            var respone = await client.PostAsync(endPointPath, JsonContent.Create(body));

            if (!respone.IsSuccessStatusCode)
                throw new InvalidDataException($"Failed to access {endPointPath}");

            var str = await respone.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(str, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

    }
}
