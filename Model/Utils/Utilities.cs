using Model.Utils.Interfaces;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Model.Utils
{
    public class Utilities : ISignature
    {
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

        public HttpMethod CreateHttpMethod(string method)
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
    }
}
