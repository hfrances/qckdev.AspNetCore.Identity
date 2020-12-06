using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Helpers
{
    public static class HttpClientHelper
    {

        static JsonSerializerSettings jsettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public async static Task<T> GetAccessToken<T>(Uri baseAddress, string requestUri, object credentials)
        {
            using (var client = new HttpClient() { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return await client.Fetch<T>(HttpMethod.Post, requestUri, credentials);
            }
        }

        public static HttpClient CreateClientWithAuthenticationBearer(Uri baseAddress, string token)
        {
            var client = new HttpClient();

            client.BaseAddress = baseAddress;
            if (token != null)
            {
                client.DefaultRequestHeaders
                            .Authorization = new AuthenticationHeaderValue(
                                "Bearer", token);
            }
            return client;
        }


        public async static Task<T> Fetch<T>(this HttpClient client, HttpMethod method, string requestUri, object content = null)
        {

            return await Fetch<T>(client, method, requestUri,
                content != null ? JsonConvert.SerializeObject(content, jsettings) : null);
        }

        public async static Task<T> Fetch<T>(this HttpClient client, HttpMethod method, string requestUri, string content)
        {
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = (content != null ?
                            new StringContent(
                                content,
                                Encoding.UTF8, "application/json")
                            :
                            null)
            };

            using (request)
            {
                return await Fetch<T>(client, request);
            }
        }

        public async static Task<T> Fetch<T>(this HttpClient client, HttpMethod method, string requestUri, FormUrlEncodedContent content)
        {
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            using (request)
            {
                return await Fetch<T>(client, request);
            }
        }

        private async static Task<T> Fetch<T>(HttpClient client, HttpRequestMessage request)
        {

            using (var response = await client.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content?.ReadAsStringAsync();

                    return jsonString != null ?
                            JsonConvert.DeserializeObject<T>(jsonString, jsettings)
                            :
                            default;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"({response.ReasonPhrase}) {message}");
                }
            }
        }

    }
}
