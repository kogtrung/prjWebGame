using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebGame.Utilities
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> requestData = new SortedList<string, string>();
        private readonly SortedList<string, string> responseData = new SortedList<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                responseData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            var data = requestData;
            var queryString = new StringBuilder();
            var hashData = new StringBuilder();

            foreach (var kv in data)
            {
                queryString.Append($"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}&");
                hashData.Append($"{kv.Key}={kv.Value}&");
            }

            // Remove the last '&'
            queryString.Length -= 1;
            hashData.Length -= 1;

            string secureHash = HmacSHA512(vnp_HashSecret, hashData.ToString());
            return $"{baseUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }

        public string HmacSHA512(string key, string inputData)
        {
            var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public SortedList<string, string> GetResponseData()
        {
            return responseData;
        }

        public bool ValidateSignature(string hashSecret, string inputHash)
        {
            var hashData = new StringBuilder();
            foreach (var kv in responseData)
            {
                if (kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                {
                    hashData.Append($"{kv.Key}={kv.Value}&");
                }
            }
            hashData.Length -= 1;

            string computedHash = HmacSHA512(hashSecret, hashData.ToString());
            return inputHash.Equals(computedHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
