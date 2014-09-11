using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace Vevisoft.Utility.Web
{
    public class HttpResponseUtility
    {
        // Methods
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public static HttpWebResponse CreateGetResponse(HttpParam httpparam)
        {
            if (string.IsNullOrEmpty(httpparam.Url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(httpparam.Url) as HttpWebRequest;
            if (request == null)
            {
                throw new ArgumentNullException("request Create Failed!");
            }
            request.Method = "GET";
            return (PrepareParams(request, httpparam).GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreatePostResponse(HttpParam httpParam)
        {
            if (string.IsNullOrEmpty(httpParam.Url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = null;
            if (httpParam.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpResponseUtility.CheckValidationResult);
                request = WebRequest.Create(httpParam.Url) as HttpWebRequest;
                if (request != null)
                {
                    request.ProtocolVersion = HttpVersion.Version11;
                }
            }
            else
            {
                request = WebRequest.Create(httpParam.Url) as HttpWebRequest;
            }
            if (request == null)
            {
                throw new ArgumentNullException("Request Create Failed!");
            }
            request.Method = "POST";
            return (PrepareParams(request, httpParam).GetResponse() as HttpWebResponse);
        }

        public static string GetHtmlStringFromUrlByGet(HttpParam hparam)
        {
            int num = 0;
            HttpWebResponse response = null;
            try
            {
                response = CreateGetResponse(hparam);
            }
            catch
            {
                num++;
                if (num > hparam.ReTryCount)
                {
                    throw new Exception("Url无法打开，请检查网络连接，或者确定网站是否可访问！");
                }
            }
            return GetPostResponseTextFromResponse(response);
        }

        public static string GetHtmlStringFromUrlByGet(string url, string cookies)
        {
            HttpParam hparam = new HttpParam(url, cookies);
            return GetHtmlStringFromUrlByGet(hparam);
        }
        
        public static string GetJsonStringFromUrlByGet(string url, string cookies)
        {
            var hparam = new HttpParam(url,cookies);

            hparam.OtherParams.Add("ContentType", "application/x-www-form-urlencoded");
            return GetHtmlStringFromUrlByGet(hparam);
        }

        public static string GetIPFromBaidu()
        {
            try
            {
                string htmlStringFromUrlByGet = GetHtmlStringFromUrlByGet("http://www.baidu.com/s?wd=ip&tn=baidu&ie=utf-8&f=8&rsv_bp=1&bs=ip&rsv_spt=3", "");
                string pattern = @"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}";
                MatchCollection matchs = Regex.Matches(htmlStringFromUrlByGet, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                if (matchs.Count > 0)
                {
                    return matchs[0].ToString();
                }
                return "";
            }
            catch (Exception)
            {
            }
            return "";
        }

        public static string GetPostResponseTextFromResponse(HttpWebResponse response)
        {
            if ((response.Headers.Get("Content-Disposition") != null) && response.Headers.Get("Content-Disposition").Contains("filename"))
            {
                return ("Response File" + response.Headers.Get("Content-Disposition"));
            }
            if (response.ContentEncoding.ToLower() == "gzip")
            {
                using (Stream stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var stream2 = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            using (var reader = new StreamReader(stream2, Encoding.UTF8))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                    }
                     
                }
            }
            using (Stream stream3 = response.GetResponseStream())
            {
                if (stream3 != null)
                {
                    using (var reader2 = new StreamReader(stream3, Encoding.UTF8))
                    {
                        return reader2.ReadToEnd();
                    }
                }
            }
            return null;
        }

        private static HttpWebRequest PrepareParams(HttpWebRequest request, HttpParam hparam)
        {
            request.UserAgent = hparam.UserAgent;
            request.KeepAlive = true;
            request.Accept = hparam.Accpt;
            request.Headers.Add("Accept-Encoding:" + hparam.AccptEncoding);
            request.Headers.Add("Accept-Language: :" + hparam.AccptLanguage);
            if (hparam.OtherParams.Count > 0)
            {
                foreach (string str in hparam.OtherParams.Keys)
                {
                    request.Headers.Set(str, hparam.OtherParams[str]);
                }
            }
            if (!string.IsNullOrEmpty(hparam.Cookies))
            {
                request.Headers.Set("Cookie", hparam.Cookies);
            }
            return request;
        }

        /// <summary>
        /// 从Json结果中获取数据.
        /// eg:{"dayCounter":0,"monthCounter":0,"weekCounter":0,"isUsing":1,"qqNo":"1519580187","qqPassword":"nuosjiao","isEnable":1}
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetValueFromJson(string jsonStr, string name)
        {
            int idx = jsonStr.IndexOf(name, System.StringComparison.Ordinal);
            if (idx < 0)
                return null;
            int idx2 = jsonStr.IndexOf(":", idx, System.StringComparison.Ordinal);
            if (idx2 < 0)
                return null;
            int idx3 = jsonStr.IndexOf(",", idx2, System.StringComparison.Ordinal);
            if (idx3 < 0 && jsonStr.Length < (idx2 + 2))
                return null;
            if (idx3 < 0)
                idx3 = jsonStr.Length;
            string value = jsonStr.Substring(idx2, idx3 - idx2);
            value = value.Replace("\"", "");
            value = value.Replace(":", "");
            value = value.Replace(",", "");
            return value.Trim();
        }

        public static List<string> GetSubJsonStr(string jsonStr, string name)
        {
            int idx = jsonStr.IndexOf(name, System.StringComparison.Ordinal);
            if (idx < 0)
                return null;
            int idx2 = jsonStr.IndexOf("[", idx, System.StringComparison.Ordinal);
            if (idx2 < 0)
                return null;
            int idx3 = jsonStr.IndexOf("]", idx2, System.StringComparison.Ordinal);
            if (idx3 < 0)
                return null;
            string value = jsonStr.Substring(idx2, idx3 - idx2);
            var array = value.Split('{', '}','[',']');
            return array.ToList();
           
        }
    }
    public class HttpDefaultParam
    {
        // Fields
        public static string DefaultAcceptEncoding = "gzip,deflate,sdch";
        public static string DefaultAcceptLanguage = "zh-CN,zh;q=0.8";
        public static string DefaultAccpt = "*/*";
        public static string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
    }


    public class HttpParam : ICloneable
    {
        // Fields
        public int _reTryCount;

        // Methods
        public HttpParam(string url)
        {
            this._reTryCount = 3;
            this.Url = url;
            this.UserAgent = HttpDefaultParam.DefaultAcceptEncoding;
            this.Accpt = HttpDefaultParam.DefaultAccpt;
            this.AccptEncoding = HttpDefaultParam.DefaultAcceptEncoding;
            this.AccptLanguage = HttpDefaultParam.DefaultAcceptLanguage;
            this.OtherParams = new Dictionary<string, string>();
        }

        public HttpParam(string url, string cookies)
            : this(url)
        {
            this.Cookies = cookies;
        }

        public object Clone()
        {
            return new HttpParam(this.Url, this.Cookies) { UserAgent = this.UserAgent, Accpt = this.Accpt, AccptEncoding = this.AccptEncoding, AccptLanguage = this.AccptLanguage };
        }

        // Properties
        public string Accpt { get; set; }

        public string AccptEncoding { get; set; }

        public string AccptLanguage { get; set; }

        public string Cookies { get; set; }

        public Dictionary<string, string> OtherParams { get; set; }

        public string Refer { get; set; }

        public int ReTryCount
        {
            get
            {
                return this._reTryCount;
            }
            set
            {
                this._reTryCount = value;
            }
        }

        public string Url { get; set; }

        public string UserAgent { get; set; }

        public string VisitMethoed { get; set; }
    }



}
