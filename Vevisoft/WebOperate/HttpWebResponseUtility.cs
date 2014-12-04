using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace Vevisoft.WebOperate
{
    public class HttpWebResponseUtility
    {
        // Fields
        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:27.0) Gecko/20100101 Firefox/27.0";

        // Methods
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public static HttpWebResponse CreateGetAndoridHttpResponse(string url, int? timeout, string wapProfile, string cookieStr, string host, string refer)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Linux; U; Android 4.1.1; zh-CN; HUAWEI C8813 Build/HuaweiC8813) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 UCBrowser/9.5.2.394 U3/0.8.0 Mobile Safari/533.1";
            request.Headers.Add("X-Requested-With: com.android.browser");
            if (string.IsNullOrEmpty(wapProfile))
            {
                wapProfile = "http://wap1.huawei.com/uaprof/HW_HUAWEI_C8813_1_20121018.xml";
            }
            if (!string.IsNullOrEmpty(refer))
            {
                request.Referer = refer;
            }
            if (!string.IsNullOrEmpty(host))
            {
                request.Host = host;
            }
            request.Headers.Add("x-wap-profile: " + wapProfile);
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            else
            {
                request.Timeout = 0x3a98;
            }
            if (!string.IsNullOrEmpty(cookieStr))
            {
                request.Headers.Add("Cookie:" + cookieStr);
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreateGetAndoridHttpResponse(string url, int? timeout, string wapProfile, string cookieStr, string host, string refer, string accptEncoding)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Linux; U; Android 2.2; en-us; DROID2 GLOBAL Build/S273) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1";
            if (!string.IsNullOrEmpty(accptEncoding))
            {
                request.Headers.Add("Accept-Encoding: " + accptEncoding);
            }
            if (!string.IsNullOrEmpty(refer))
            {
                request.Referer = refer;
            }
            if (!string.IsNullOrEmpty(host))
            {
                request.Host = host;
            }
            request.Headers.Add("x-wap-profile: " + wapProfile);
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            else
            {
                request.Timeout = 0x3a98;
            }
            if (!string.IsNullOrEmpty(cookieStr))
            {
                request.Headers.Add("Cookie:" + cookieStr);
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            request.KeepAlive = true;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, string cookieStr)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            request.KeepAlive = true;
            request.Headers.Add("Accept-Language: zh-CN,zh;q=0.8");
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            else
            {
                request.Timeout = 0x3a98;
            }
            if (!string.IsNullOrEmpty(cookieStr))
            {
                request.Headers.Add("Cookie:" + cookieStr);
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies, string host)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            request.Host = host;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, string cookieStr, string acceptEncoding)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ServicePoint.Expect100Continue = false;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            request.KeepAlive = true;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            else
            {
                request.Timeout = 0x3a98;
            }
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            if (!string.IsNullOrEmpty(cookieStr))
            {
                request.Headers.Add("Cookie:" + cookieStr);
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, string cookieStr, string refer, string acceptencoding)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpWebResponseUtility.CheckValidationResult);
            request.Method = "GET";
            request.UserAgent = !string.IsNullOrEmpty(userAgent) ? userAgent : DefaultUserAgent;
            request.KeepAlive = true;
            request.Headers.Add("Accept-Language: zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
            request.Headers.Add("Accept-Encoding: gzip,deflate,sdch");
            if (!string.IsNullOrEmpty(refer))
            {
                request.Referer = refer;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            else
            {
                request.Timeout = 0x3a98;
            }
            if (!string.IsNullOrEmpty(cookieStr))
            {
                request.Headers.Add("Cookie:" + cookieStr);
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreateGetHttpResponse189(string url, int? timeout, string userAgent, string cookieStr, string refer, string acceptencoding)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:27.0) Gecko/20100101 Firefox/27.0";
            request.KeepAlive = true;
            request.Headers.Add("Accept-Language: zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
            request.Headers.Add("Accept-Encoding: gzip, deflate");
            if (!string.IsNullOrEmpty(refer))
            {
                request.Referer = refer;
            }
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            else
            {
                request.Timeout = 0x3a98;
            }
            if (!string.IsNullOrEmpty(cookieStr))
            {
                request.Headers.Add("Cookie:" + cookieStr);
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreateGetHttpResponseProxy(string url, int? timeout, string userAgent, string cookieStr, string refer, string acceptencoding, WebProxy proxy)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpWebResponseUtility.CheckValidationResult);
            request.Method = "GET";
            request.UserAgent = !string.IsNullOrEmpty(userAgent) ? userAgent : DefaultUserAgent;
            request.KeepAlive = true;
            request.Headers.Add("Accept-Language: zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
            request.Headers.Add("Accept-Encoding: gzip,deflate,sdch");
            if (!string.IsNullOrEmpty(refer))
            {
                request.Referer = refer;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            else
            {
                request.Timeout = 0x61a8;
            }
            if (!string.IsNullOrEmpty(cookieStr))
            {
                request.Headers.Add("Cookie:" + cookieStr);
            }
            
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpWebResponseUtility.CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/json, text/javascript, */*";
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            if ((parameters != null) && (parameters.Count != 0))
            {
                StringBuilder builder = new StringBuilder();
                int num = 0;
                foreach (string str in parameters.Keys)
                {
                    if (num > 0)
                    {
                        builder.AppendFormat("&{0}={1}", str, parameters[str]);
                    }
                    else
                    {
                        builder.AppendFormat("{0}={1}", str, parameters[str]);
                    }
                    num++;
                }
                byte[] bytes = requestEncoding.GetBytes(builder.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies, string refer)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpWebResponseUtility.CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Referer = refer;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            if ((parameters != null) && (parameters.Count != 0))
            {
                StringBuilder builder = new StringBuilder();
                int num = 0;
                foreach (string str in parameters.Keys)
                {
                    if (num > 0)
                    {
                        builder.AppendFormat("&{0}={1}", str, parameters[str]);
                    }
                    else
                    {
                        builder.AppendFormat("{0}={1}", str, parameters[str]);
                    }
                    num++;
                }
                byte[] bytes = requestEncoding.GetBytes(builder.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreatePostJsonHttpResponse(string url, string postData, int? timeout, string cookieStr, string userAgent, Encoding requestEncoding, string referer)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                requestEncoding = Encoding.UTF8;
            }
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpWebResponseUtility.CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.ServicePoint.Expect100Continue = false;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Set("X-Requested-With", "XMLHttpRequest");
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Method = "POST";
            request.Referer = referer;
            request.KeepAlive = true;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            else
            {
                request.Timeout = 0x3a98;
            }
            if (!string.IsNullOrEmpty(cookieStr))
            {
                request.Headers.Add("Cookie:" + cookieStr);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreatePostJsonHttpResponse(string url, string postData, int? timeout, string cookieStr, string userAgent, Encoding requestEncoding, string referer, WebProxy proxy)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                requestEncoding = Encoding.UTF8;
            }
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpWebResponseUtility.CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            request.ServicePoint.Expect100Continue = false;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Set("X-Requested-With", "XMLHttpRequest");
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Method = "POST";
            request.Referer = referer;
            request.KeepAlive = true;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            else
            {
                request.Timeout = 0xc350;
            }
            if (!string.IsNullOrEmpty(cookieStr))
            {
                request.Headers.Add("Cookie:" + cookieStr);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreatePostJsonHttpResponse189Login(string url, string postData, int? timeout, string cookieStr, string userAgent, Encoding requestEncoding, string referer)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpWebResponseUtility.CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Cache-Control: max-age=0");
            request.Headers.Add("Accept-Charset: GBK,utf-8;q=0.7,*;q=0.3");
            request.Headers.Add("Accept-Encoding: gzip,deflate,sdch");
            request.Headers.Add("Accept-Language: zh-CN,zh;q=0.8");
            request.Headers.Add("Origin: https://open.e.189.cn");
            request.KeepAlive = true;
            request.Method = "POST";
            request.Referer = referer;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1";
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            else
            {
                request.Timeout = 0x3a98;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            if (!string.IsNullOrEmpty(cookieStr))
            {
                request.Headers.Add("Cookie:" + cookieStr);
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static HttpWebResponse CreatePostXMLHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpWebResponseUtility.CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Headers.Set("X-Requested-With", "XMLHttpRequest");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Accept = "application/json, text/javascript, */*";
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            if ((parameters != null) && (parameters.Count != 0))
            {
                StringBuilder builder = new StringBuilder();
                int num = 0;
                foreach (string str in parameters.Keys)
                {
                    if (num > 0)
                    {
                        builder.AppendFormat("&{0}={1}", str, parameters[str]);
                    }
                    else
                    {
                        builder.AppendFormat("{0}={1}", str, parameters[str]);
                    }
                    num++;
                }
                byte[] bytes = requestEncoding.GetBytes(builder.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            return (request.GetResponse() as HttpWebResponse);
        }

        public static void DownLoadFileFromURL(string url, string filePath)
        {
            HttpWebResponse response = CreateGetHttpResponse(url, null, null, "");
            string str = "download.apk";
            string str2 = response.Headers["Content-Disposition"];
            int index = str2.IndexOf("=", StringComparison.Ordinal);
            if (index > -1)
            {
                index++;
                str = str2.Substring(index, str2.Length - index);
            }
            str = str.Replace("\"", "");
            Stream responseStream = response.GetResponseStream();
            FileStream stream2 = new FileStream(filePath + "//" + str, FileMode.Create);
            long num2 = 0L;
            long contentLength = response.ContentLength;
            byte[] buffer = new byte[0x400];
            for (int i = responseStream.Read(buffer, 0, buffer.Length); i > 0; i = responseStream.Read(buffer, 0, buffer.Length))
            {
                num2 = i + num2;
                stream2.Write(buffer, 0, i);
            }
            stream2.Close();
            responseStream.Close();
        }

        public static void DownLoadFileFromURL(string url, string filePath, string cookie)
        {
            using (HttpWebResponse response = CreateGetHttpResponse(url, null, null, cookie))
            {
                string str = "download.apk";
                string str2 = response.Headers["Content-Disposition"];
                int index = str2.IndexOf("=", StringComparison.Ordinal);
                if (index > -1)
                {
                    index++;
                    str = str2.Substring(index, str2.Length - index);
                }
                str = str.Replace("\"", "");
                Stream responseStream = response.GetResponseStream();
                FileStream stream2 = new FileStream(filePath + "//" + str, FileMode.Create);
                long num2 = 0L;
                long contentLength = response.ContentLength;
                byte[] buffer = new byte[0x400];
                for (int i = responseStream.Read(buffer, 0, buffer.Length); i > 0; i = responseStream.Read(buffer, 0, buffer.Length))
                {
                    num2 = i + num2;
                    stream2.Write(buffer, 0, i);
                }
                stream2.Close();
                responseStream.Close();
            }
        }

        public static List<string> GetAllIP()
        {
            List<string> list = new List<string>();
            foreach (IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (address.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    list.Add(address.ToString());
                }
            }
            return list;
        }

        public static string GetIPFromInternet()
        {
            string url = "http://i.singmusic.cn:8080/getIP.jsp";
            url = "http://www.baidu.com/s?wd=ip&tn=baidu&ie=utf-8&f=8&rsv_bp=1&bs=ip&rsv_spt=3";
            using (HttpWebResponse response = CreateGetHttpResponseProxy(url, null, null, null, "", "", null))
            {
                string postResponseTextFromResponse = GetPostResponseTextFromResponse(response);
                int startIndex = postResponseTextFromResponse.IndexOf("fk=\"") + "fk=\"".Length;
                int index = postResponseTextFromResponse.IndexOf("\"", startIndex);
                return postResponseTextFromResponse.Substring(startIndex, index - startIndex).Trim();
            }
        }

        public static string GetIPFromInternet(WebProxy proxy)
        {
            string uriString = "http://iframe.ip138.com/ic.asp";
            Uri requestUri = new Uri(uriString);
            WebRequest request = WebRequest.Create(requestUri);
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.Default))
            {
                string str2 = reader.ReadToEnd().Trim();
                int startIndex = str2.IndexOf("[") + 1;
                int index = str2.IndexOf("]", startIndex);
                return str2.Substring(startIndex, index - startIndex).Trim();
            }
        }

        public static string GetJsonValue(string jsonResult, string itemName)
        {
            return GetJsonValue(jsonResult, itemName, 0);
        }

        public static string GetJsonValue(string jsonResult, string itemName, int startIdx)
        {
            string str = string.Format("\"{0}\":", itemName);
            int startIndex = jsonResult.IndexOf(str, startIdx, StringComparison.Ordinal);
            if (startIndex >= 0)
            {
                startIndex += str.Length;
                int num2 = jsonResult.IndexOf(",", startIndex, StringComparison.Ordinal);
                if (num2 >= 0)
                {
                    string str2 = jsonResult.Substring(startIndex, num2 - startIndex);
                    if (str2.StartsWith("\""))
                    {
                        str2 = str2.Substring(1);
                    }
                    if (str2.EndsWith("\""))
                    {
                        str2 = str2.Substring(0, str2.Length - 1);
                    }
                    return str2;
                }
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
                    goto Label_00E2;
                }
            }
            using (Stream stream3 = response.GetResponseStream())
            {
                if (stream3 != null)
                {
                    using (StreamReader reader2 = new StreamReader(stream3, Encoding.UTF8))
                    {
                        return reader2.ReadToEnd();
                    }
                }
            }
        Label_00E2:
            return null;
        }

        public static string GetResponseGetResult(string url, int? timeout, string userAgent, CookieCollection cookies)
        {
            cookies = new CookieCollection();
            HttpWebResponse response = CreateGetHttpResponse(url, null, null, cookies);
            try
            {
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                string str = reader.ReadToEnd();
                reader.Close();
                response.Close();
                return str;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetTimeTIcks(DateTime dt)
        {
            DateTime time = new DateTime(0x7b2, 1, 1);
            DateTime time2 = dt.ToUniversalTime();
            TimeSpan span = new TimeSpan(time2.Ticks - time.Ticks);
            return span.TotalMilliseconds.ToString("F0");
        }
        public static string GetTimeStamp(DateTime dt)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (dt - startTime).TotalMilliseconds.ToString("F0");
        }
        public static string GetUrlEncodeValue(string value)
        {
            return HttpUtility.UrlEncode(value, Encoding.UTF8);
        }

        public static double UnixTicks(DateTime dt)
        {
            DateTime time = new DateTime(0x7b2, 1, 1);
            DateTime time2 = dt.ToUniversalTime();
            TimeSpan span = new TimeSpan(time2.Ticks - time.Ticks);
            return span.TotalMilliseconds;
        }
        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }  

        public static DateTime GetJavaTime(string timeStamp)
        {
            //var value = Vevisoft.Utility.Web.HttpResponseUtility.GetJsonStringFromUrlByGet(GetTimeUrl, "");
            //毫秒数
            var dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            var toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow); ;
        }
    }


}
