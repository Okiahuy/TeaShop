using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TeaShop.VNPay
{
    public class VNPayLib
    {
        public const string VERSION = "2.1.0";
        private SortedList<string, string> requestData = new SortedList<string, string>(new VnPayComparer());
        private SortedList<string, string> responseData = new SortedList<string, string>(new VnPayComparer());

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

        public string GetResponseData(string key)
        {
            if (responseData.ContainsKey(key))
            {
                return responseData[key];
            }
            return null;
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(HttpUtility.UrlEncode(kv.Key) + "=" + HttpUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string queryString = data.ToString().TrimEnd('&');
            string vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, queryString);
            return baseUrl + "?" + queryString + "&vnp_SecureHash=" + vnp_SecureHash;
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            string rspRaw = GetResponseData("vnp_SecureHashType") + GetResponseData("vnp_SecureHash");
            string myChecksum = Utils.HmacSHA512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public class Utils
    {
        public static string HmacSHA512(string key, string data)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }
        }

        public static string GetIpAddress()
        {
            string ipAddress;
            try
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown" || ipAddress.Length > 45)
                {
                    ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch (Exception ex)
            {
                ipAddress = "Invalid IP: " + ex.Message;
            }
            return ipAddress;
        }
    }

    public class VnPayComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null || y == null)
            {
                return 0;
            }
            return x.CompareTo(y);
        }
    }
}
