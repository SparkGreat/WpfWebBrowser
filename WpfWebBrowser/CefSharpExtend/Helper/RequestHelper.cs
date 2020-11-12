using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WpfWebBrowser.CefSharpExtend.Enums;

namespace WpfWebBrowser.CefSharpExtend.Helper
{
    /// <summary>
    /// 请求URL的一个辅助类
    /// </summary>
    public static class RequestHelper
    {
        /// <summary>
        /// 请求的封装，封装了默认请求方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpType"></param>
        /// <param name="query"></param>
        /// <param name="postdata"></param>
        /// <returns></returns>
        public static ReturnMessage Request(string url, MethodType method = MethodType.Get, string cookStr = "", Dictionary<string, string> query = null, object postdata = null)
        {
            Args httpArgs = new Args();
            httpArgs.Url = url;
            httpArgs.Method = method;
            httpArgs.cookStr = cookStr;
            if (query != null)
                httpArgs.QueryDic = query;
            if (postdata != null)
            {
                httpArgs.postData.Data = postdata;
                httpArgs.postData.contentType = ContentType.FromData;
            }
            return Request(httpArgs);
        }

        /// <summary>
        /// http请求的集合体
        /// </summary>
        /// <param name="httpArgs">请求类</param>
        /// <returns></returns>
        public static ReturnMessage Request(Args httpArgs)
        {
            var request = CreateHttpWebRequest(httpArgs);
            bool iscatch = false;
            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                iscatch = true;
                response = ex.Response as HttpWebResponse;
            }
            if (httpArgs.isFile && httpArgs.FileInfo != null)
            {
                if (iscatch)
                {
                    return new ReturnMessage { Status = Convert.ToInt32(response.StatusCode), HtmlSource = "下载失败" };
                }
                else
                {

                    if (SaveFile(httpArgs.FileInfo, response))
                    {
                        return new ReturnMessage { Status = Convert.ToInt32(response.StatusCode), HtmlSource = "下载成功" };
                    }
                    else
                    {
                        return new ReturnMessage { Status = Convert.ToInt32(response.StatusCode), HtmlSource = "下载失败" };
                    }
                }
            }
            else
            {
                return HandleHttpWebRequest(response);
            }



        }

        /// <summary>
        /// 处理返回值
        /// </summary>
        /// <param name="httpWebResponse"></param>
        /// <returns></returns>
        private static ReturnMessage HandleHttpWebRequest(HttpWebResponse httpWebResponse)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            returnMessage.Hearders = HandleHeaders(httpWebResponse.Headers);
            if (returnMessage.Hearders != null && returnMessage.Hearders.Keys.Contains("Set-Cookie"))
            {
                returnMessage.CookieStr = returnMessage.Hearders["Set-Cookie"];
                returnMessage.Hearders.Remove("Set-Cookie");
            }
            returnMessage.Status = Convert.ToInt32(httpWebResponse.StatusCode);
            returnMessage.HtmlSource = GetHtmlSource(httpWebResponse);
            returnMessage.JsonObject = HandleHtmlSource(returnMessage.HtmlSource);
            return returnMessage;
        }

        /// <summary>
        /// 创建请求HttpClient
        /// </summary>
        /// <param name="httpArgs"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateHttpWebRequest(Args httpArgs)
        {
            HttpWebRequest request = null;
            if (httpArgs.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(httpArgs.Url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(httpArgs.Url) as HttpWebRequest;
            }
            request.Method = httpArgs.Method.ToString();//设置请求方式
            request.UserAgent = httpArgs.UserAgent;//设置浏览器用户头
            request.ReadWriteTimeout = httpArgs.TimeOut * 1000;//设置读取文本时的超时时间
            request.Timeout = httpArgs.TimeOut * 1000;//设置请求时的超时时间
            request.Accept = httpArgs.Accept;//设置请求获取的类型
            request.KeepAlive = httpArgs.KeepAlive;//设置是否保持链接的存活
            request.AllowAutoRedirect = httpArgs.KeepAlive;//如果请求应自动遵循来自 Internet 资源的重定向响应
            if (httpArgs.cookStr.Length > 0)
                request.Headers.Add("Cookie", httpArgs.cookStr);
            if (httpArgs.poxy.Ip != string.Empty)
            {
                WebProxy proxy = new WebProxy(httpArgs.poxy.Ip, httpArgs.poxy.Port);
                request.Proxy = proxy;
            }
            if (httpArgs.Hearders != null && httpArgs.Hearders.Count > 0)
            {
                foreach (var key in httpArgs.Hearders.Keys)
                {
                    request.Headers.Add(key, httpArgs.Hearders[key]);
                }
            }
            if (httpArgs.postData != null && httpArgs.postData.Data != null && httpArgs.Method == MethodType.Post)//是指请求内容
            {
                string parms = string.Empty;
                switch (httpArgs.postData.contentType)
                {
                    case ContentType.FromData://from表单提交
                        {
                            parms = httpArgs.postData.Data.ToString();
                            request.ContentType = "application/x-www-form-urlencoded";
                        }
                        break;
                    case ContentType.Json://json提交
                        {
                            parms = Newtonsoft.Json.JsonConvert.SerializeObject(httpArgs.postData.Data);
                            request.ContentType = "application/json";
                        }
                        break;
                }
                byte[] data = httpArgs.postData.encoding.GetBytes(parms.ToString());
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request;
        }


        /// <summary>
        /// 处理返回的htmlSource
        /// </summary>
        /// <returns></returns>
        private static JToken HandleHtmlSource(string htmlSource)
        {
            JToken jObject = null;
            try
            {
                jObject = JToken.Parse(htmlSource);
                return jObject;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 处理返回值里面的Header
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private static Dictionary<string, string> HandleHeaders(WebHeaderCollection header)
        {
            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            foreach (var key in header.AllKeys)
            {
                string values = string.Empty;
                values = header.Get(key);
                valuePairs.Add(key, values);
            }
            return valuePairs;
        }

        /// <summary>
        /// 保存成文件
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="respones"></param>
        /// <returns></returns>
        private static bool SaveFile(FileInfo savePath, HttpWebResponse respones)
        {
            //获取服务器返回的资源  
            try
            {
                if (!savePath.Directory.Exists)
                {
                    savePath.Directory.Create();
                }
                using (Stream sream = respones.GetResponseStream())
                {
                    List<byte> list = new List<byte>();
                    while (true)
                    {
                        int data = sream.ReadByte();
                        if (data == -1)
                            break;
                        list.Add((byte)data);
                    }
                    File.WriteAllBytes(savePath.FullName, list.ToArray());
                }
                return true;
            }

            catch { return false; }

        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }

        /// <summary>
        /// 获取该网页的源码 //兼容gzip格式的
        /// </summary>
        /// <param name="responseGet">该网页的HttpWebResponse</param>
        /// <returns></returns>
        private static string GetHtmlSource(HttpWebResponse responseGet)
        {
            string retur = string.Empty;
            try
            {
                string encodingStr = responseGet.CharacterSet.ToLower().ToString();
                if (encodingStr == "iso-8859-1")
                    encodingStr = "gb2312";
                Encoding encoding = Encoding.GetEncoding(encodingStr);
                Stream streamReceive = responseGet.GetResponseStream();
                try
                {
                    if (responseGet.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        //gzip格式
                        streamReceive = new GZipStream(streamReceive, CompressionMode.Decompress);
                    }
                }
                catch { }
                using (System.IO.StreamReader sr = new System.IO.StreamReader(streamReceive, encoding))
                {
                    retur = sr.ReadToEnd();
                }
            }
            catch { }
            return retur;
        }


        /// <summary>
        /// MD5摘要算法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMd5Str32(string str)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            char[] temp = str.ToCharArray();
            byte[] buf = new byte[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                buf[i] = (byte)temp[i];
            }
            byte[] data = md5Hasher.ComputeHash(buf);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();

        }
    }


    /// <summary>
    /// http返回值内容的格式化
    /// </summary>
    public class ReturnMessage
    {
        /// <summary>
        /// 请求回来的html详细内容
        /// </summary>
        public string HtmlSource { get; set; } = string.Empty;

        /// <summary>
        /// 请求回来的详细内容如果是Json的话会自动转化为js对象
        /// </summary>
        public JToken JsonObject { get; set; } = null;

        /// <summary>
        /// 请求返回回来的状态码
        /// </summary>
        public int Status { get; set; } = 200;

        /// <summary>
        /// 请求返回回来的所有cookie
        /// </summary>
        public string CookieStr { get; set; } = string.Empty;

        /// <summary>
        /// 请求的Headers
        /// </summary>
        public Dictionary<string, string> Hearders { get; set; } = null;
    }

    /// <summary>
    /// http请求时的参数格式化
    /// </summary>
    public class Args
    {
        /// <summary>
        /// 请求的url
        /// </summary>
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// 请求的query参数
        /// </summary>
        public Dictionary<string, string> QueryDic { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// http请求类型
        /// </summary>
        public MethodType Method { get; set; } = MethodType.Get;
        /// <summary>
        /// http请求postStr
        /// </summary>
        public PostData postData { get; set; } = new PostData();

        /// <summary>
        /// http请求时的hearder
        /// </summary>
        public Dictionary<string, string> Hearders { get; set; } = null;
        /// <summary>
        /// http请求时的cookie
        /// </summary>
        public string cookStr { get; set; } = string.Empty;
        /// <summary>
        /// 代理
        /// </summary>
        public Poxy poxy { get; set; } = new Poxy();

        public int TimeOut { get; set; } = 20;
        /// <summary>
        /// 用户身份信息头
        /// </summary>
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";

        /// <summary>
        /// 表单方式
        /// </summary>
        public string Accept { get; set; } = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";

        /// <summary>
        /// 链接是否保持存活
        /// </summary>
        public bool KeepAlive { get; set; } = false;

        /// <summary>
        /// 如果请求应自动遵循来自 Internet 资源的重定向响应，则为 true；否则为 false。 默认值为 false。
        /// </summary>
        public bool AllowAutoRedirect { get; set; } = false;
        /// <summary>
        /// 是否是文件
        /// </summary>
        public bool isFile { get; set; } = false;
        /// <summary>
        /// 文件存储的路径
        /// </summary>
        public FileInfo FileInfo { get; set; } = null;

    }
    /// <summary>
    /// 代理配置
    /// </summary>
    public class Poxy
    {
        /// <summary>
        /// 代理服务器IP
        /// </summary>
        public string Ip { get; set; } = string.Empty;
        /// <summary>
        /// 代理服务器端口号
        /// </summary>
        public int Port { get; set; } = 80;
    }

    /// <summary>
    /// 提交数据
    /// </summary>
    public class PostData
    {
        /// <summary>
        /// 请求body的方式 默认json
        /// </summary>
        public ContentType contentType { get; set; } = ContentType.Json;
        /// <summary>
        /// post提交的data的内容
        /// </summary>
        public object Data { get; set; } = new object();
        /// <summary>
        /// 编码格式
        /// </summary>
        public Encoding encoding { get; set; } = Encoding.UTF8;


    }
    /// <summary>
    /// postData内容的格式
    /// </summary>
    public enum ContentType
    {
        FromData,//表单提交
        Json     //json方式提交
    }

}
