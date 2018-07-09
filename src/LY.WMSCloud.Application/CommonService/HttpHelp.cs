using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace LY.WMSCloud.CommonService
{
    public class HttpHelp
    {
        public IConfiguration Configuration { get; }

        public HttpHelp(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        string BaseUrl
        {
            get { return Configuration["App:HubService"]; }
        }




        public T Post<T>(string url, object data) where T : class, new()
        {
            return Requset<T>(RequsetMethod.POST, url, data, null, false);
        }

        public ReresultDto<T> PostAbp<T>(string url, object data) where T : class, new()
        {
            return RequsetAbp<T>(RequsetMethod.POST, url, data, null, false);
        }


        public T Get<T>(string url, object data) where T : class, new()
        {
            return Requset<T>(RequsetMethod.GET, url, data, null, false);
        }

        public ReresultDto<T> GetAbp<T>(string url, object data) where T : class, new()
        {
            return RequsetAbp<T>(RequsetMethod.GET, url, data, null, false);
        }

        public ReresultDto<T> RequsetAbp<T>(RequsetMethod method, string url, object data, List<HttpHeader> headers, bool noToken)
        {
            return Requset<ReresultDto<T>>(method, url, data, headers, noToken);
        }

        public T Requset<T>(RequsetMethod method, string url, object data, List<HttpHeader> headers, bool noToken) where T : class, new()
        {

            Encoding encoding = Encoding.UTF8;

            // 如果直接精确到地址不用加服务器基础地址
            if (url.ToLower().Contains("https://") || url.Contains("http://"))
            {

            }
            else
            {
                url = BaseUrl + url;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = method.ToString();

            // 添加token
            //if (!noToken)
            //{
            //    request.Headers.Add("Authorization", "Bearer " + Token.AccessToken);
            //}

            // 添加自定义头
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            // 统一这一种格式数据
            request.ContentType = "application/json;charset=utf-8";

            // 有数据正文，发送数据正文
            if (data != null)
            {
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                byte[] requsetData;
                requsetData = Encoding.UTF8.GetBytes(jsonData);
                request.ContentLength = requsetData.Length;

                using (Stream reStream = request.GetRequestStream())
                {
                    reStream.Write(requsetData, 0, requsetData.Length);
                }
            }


            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var resSrt = reader.ReadToEnd();

                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(resSrt);
                }
            }
        }



        public void PostAsync<T>(string url, object data, Action<T> onResponse) where T : class, new()
        {
            RequsetAsync<T>(RequsetMethod.POST, url, data, null, false, onResponse);
        }

        public void PostAbpAsync<T>(string url, object data, Action<ReresultDto<T>> onResponse) where T : class, new()
        {
            RequsetAbpAsync<T>(RequsetMethod.POST, url, data, null, false, onResponse);
        }


        public void GetAsync<T>(string url, object data, Action<T> onResponse) where T : class, new()
        {
            RequsetAsync<T>(RequsetMethod.GET, url, data, null, false, onResponse);
        }

        public void GetAbpAsync<T>(string url, object data, Action<ReresultDto<T>> onResponse) where T : class, new()
        {
            RequsetAbpAsync<T>(RequsetMethod.GET, url, data, null, false, onResponse);
        }

        public void RequsetAbpAsync<T>(RequsetMethod method, string url, object data, List<HttpHeader> headers, bool noToken, Action<ReresultDto<T>> onResponse) where T : class, new()
        {
            RequsetAsync<ReresultDto<T>>(method, url, data, headers, noToken, onResponse);
        }

        public void RequsetAsync<T>(RequsetMethod method, string url, object data, List<HttpHeader> headers, bool noToken, Action<T> onResponse) where T : class, new()
        {


            Encoding encoding = Encoding.UTF8;

            if (url.ToLower().Contains("https://") || url.Contains("http://"))
            {

            }
            else
            {
                url = BaseUrl + url;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseUrl + url);

            // 如果直接精确到地址不用加服务器基础地址
            if (url.ToLower().Contains("https://") || url.Contains("http://"))
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }

            request.Method = method.ToString();

            // 添加token
            //if (!noToken)
            //{
            //    request.Headers.Add("Authorization", "Bearer " + Token.AccessToken);
            //}

            // 添加自定义头
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            // 统一这一种格式数据
            request.ContentType = "application/json;charset=utf-8";




            request.BeginGetRequestStream(new AsyncCallback((result) =>
            {
                HttpWebRequest req = (HttpWebRequest)result.AsyncState;

                // 有数据正文，发送数据正文
                if (data != null)
                {
                    var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    Stream postStream = req.EndGetRequestStream(result);
                    byte[] requsetData;
                    requsetData = Encoding.UTF8.GetBytes(jsonData);
                    postStream.Write(requsetData, 0, requsetData.Length);
                    postStream.Close();
                }

                req.BeginGetResponse(new AsyncCallback(responseResult =>
                {
                    HttpWebRequest req1 = (HttpWebRequest)responseResult.AsyncState;

                    if (responseResult.IsCompleted)
                    {
                        var webResponse = req1.EndGetResponse(responseResult);
                        using (var stream = webResponse.GetResponseStream())
                        {
                            using (var read = new StreamReader(stream))
                            {
                                if (onResponse != null)
                                {
                                    onResponse.Invoke(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(read.ReadToEnd()));
                                }
                            }
                        }
                    }
                }), req);
            }), request);

        }

        public enum RequsetMethod
        {
            GET = 0, POST, DELETE, PUT
        }

        public class ReresultDto<T>
        {
            public T Result { get; set; }

            public string TargetUrl { get; set; }

            public bool Success { get; set; }

            public Error Error { get; set; }

            public bool UnAuthorizedRequest { get; set; }

            public bool __abp { get; set; }

        }

        public class Error
        {
            public string Code { get; set; }

            public string Message { get; set; }

            public string Details { get; set; }

            public string ValidationErrors { get; set; }
        }

        public class HttpHeader
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

    }
}
