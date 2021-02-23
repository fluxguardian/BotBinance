using Model.Enums;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BotBinanceBL
{
    public class HttpUtilities
    {
        private HttpClient _httpClient { get; set; }
        private string _secretKey { get; set; }

        public HttpUtilities(string url, string key, string secretKey)
        {
            _secretKey = secretKey;

            ConfigureHttpClient(url, key);
        }

        private void ConfigureHttpClient(string url, string key)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };

            _httpClient.DefaultRequestHeaders
                 .Add("X-MBX-APIKEY", key);

            _httpClient.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> GetHttpResponse(ApiMethod method, string finalEndpoint)
        {
            var request = new HttpRequestMessage(CreateHttpMethod(method.ToString()), finalEndpoint);

            return await _httpClient.SendAsync(request).ConfigureAwait(false);
        }

        public string GetFinalPoint(string endpoint, bool isSigned, ref string parameters)
        {
            string finalEndpoint = endpoint + (string.IsNullOrWhiteSpace(parameters) ? "" : $"?{parameters}");

            if (isSigned)
            {
                parameters += (!string.IsNullOrWhiteSpace(parameters) ? "&timestamp=" : "timestamp=") + GenerateTimeStamp(DateTime.Now.ToUniversalTime());

                string signature = GenerateSignature(_secretKey, parameters);
                finalEndpoint = $"{endpoint}?{parameters}&signature={signature}";
            }

            return finalEndpoint;
        }

        private HttpMethod CreateHttpMethod(string method)
        {
            return (method.ToUpper()) switch
            {
                "DELETE" => HttpMethod.Delete,
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                "GET" => HttpMethod.Get,
                _ => throw new NotImplementedException(),
            };
        }

        public string GenerateSignature(string secretKey, string parametres)
        {
            var key = Encoding.UTF8.GetBytes(secretKey);
            string stringHash;
            using (var hmac = new HMACSHA256(key))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(parametres));
                stringHash = BitConverter.ToString(hash).Replace("-", "").ToLower();
            }

            return stringHash;
        }

        public string GenerateTimeStamp(DateTime baseDateTime)
        {
            var dtOffset = new DateTimeOffset(baseDateTime);
            return dtOffset.ToUnixTimeMilliseconds().ToString();
        }
    }
}
