﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using FS.Extends;

namespace FS.Utils.Common
{
    /// <summary>
    ///     下载文件
    /// </summary>
    public static class Net
    {
        /// <summary>
        ///     下载文件到服务器
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="wc"></param>
        public static int Save(string url, string savePath, WebClient wc = null)
        {
            if (string.IsNullOrWhiteSpace(url)) { return 0; }
            url = url.Replace("\\", "/");

            int fileSize;
            var isNew = wc == null;
            if (wc == null)
            {
                wc = new WebClient {Proxy = null};
                wc.Headers.Add("Accept", "*/*");
                wc.Headers.Add("Referer", url);
                wc.Headers.Add("Cookie", "bid=\"YObnALe98pw\";");
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");
            }

            try
            {
                wc.DownloadFile(url, savePath);
                var f = new FileInfo(savePath);
                fileSize = (int) f.Length;
            }
            finally
            {
                if (!isNew) { Cookies(wc); }
                else
                { wc.Dispose(); }
            }
            return fileSize;
        }

        /// <summary>
        ///     获取远程信息
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="encoding">请求编码</param>
        /// <param name="wc">客户端</param>
        public static string Get(string url, Encoding encoding = null, WebClient wc = null)
        {
            if (string.IsNullOrWhiteSpace(url)) { return string.Empty; }
            url = url.Replace("\\", "/");
            if (encoding == null) encoding = Encoding.UTF8;
            var isNew = wc == null;
            if (wc == null)
            {
                wc = new WebClient();
                wc.Proxy = null;
                wc.Headers.Add("Accept", "*/*");
                wc.Headers.Add("Referer", url);
                wc.Headers.Add("Cookie", "bid=\"YObnALe98pw\";");
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");
            }
            string strResult = null;
            try
            {
                var data = wc.DownloadData(url);
                strResult = encoding.GetString(data);
            }
            catch {
                return string.Empty;
            }
            finally
            {
                if (!isNew) Cookies(wc);
                if (isNew) wc.Dispose();
            }
            return strResult;
        }

        /// <summary>
        ///     传入URL返回网页的html代码
        /// </summary>
        /// <param name="url">要读取的网页URL</param>
        /// <param name="readCode">读取源文件所使用的编码</param>
        /// <param name="cookie">传过去的cookie</param>
        public static string Get(string url, Encoding encoding, ref CookieContainer cookie)
        {
            if (string.IsNullOrWhiteSpace(url)) { return string.Empty; }
            url = url.Replace("\\", "/");
            if (string.IsNullOrWhiteSpace(url)) { return string.Empty; }
            if (encoding == null) { encoding = Encoding.UTF8; }

            var content = string.Empty;
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(url);
                request.Proxy = null;
                request.KeepAlive = false;
                request.CookieContainer = cookie;
                //request.Headers.Add("Accept", "*/*");
                //request.Headers.Add("Referer", url);
                request.Headers.Add("Cookie", "bid=\"YObnALe98pw\"");
                //request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");

                var response = (HttpWebResponse) request.GetResponse();
                response.Cookies = cookie.GetCookies(request.RequestUri);

                var respStream = response.GetResponseStream();
                if (respStream != null)
                {
                    var reader = new StreamReader(respStream, encoding);
                    content = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception ex) {
                content = "error:" + ex.Message;
            }
            return content;
        }

        /// <summary>
        ///     Post信息
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="queryString">QueryString</param>
        /// <param name="encoding">请求编码</param>
        /// <param name="wc">Web客户端</param>
        public static string UploadData(string url, string queryString, Encoding encoding = null, WebClient wc = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            var isNew = false;
            if (wc == null)
            {
                wc = new WebClient();
                wc.Proxy = null;
                isNew = true;
            }
            string strResult = null;
            try
            {
                wc.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                var dataResult = wc.UploadData(url, null, encoding.GetBytes(queryString));
                strResult = encoding.GetString(dataResult);
                wc.Headers.Remove("Content-Type");
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    var reader = new StreamReader(ex.Response.GetResponseStream(), encoding);
                    strResult = string.Format("Error:{0}\n<hr />\n{1}", ex.Message, reader.ReadToEnd());
                }
            }
            finally
            {
                if (!isNew) { Cookies(wc); }
                if (isNew) wc.Dispose();
            }

            return strResult;
        }

        /// <summary>
        ///     判断网络文件是否存在
        /// </summary>
        /// <param name="url">要读取的网页URL</param>
        /// <param name="encoding">读取源文件所使用的编码</param>
        public static bool IsHaving(string url, Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(url)) { return false; }
            url = url.Replace("\\", "/");
            if (encoding == null) { encoding = Encoding.UTF8; }

            bool isHaving;
            try
            {
                using (var web = new WebClient())
                {
                    web.Proxy = null;
                    web.Headers.Add("Accept", "*/*");
                    web.Headers.Add("Referer", url);
                    web.Headers.Add("Cookie", "bid=\"YObnALe98pw\"");
                    web.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");
                    isHaving = web.DownloadData(url).Length > 0;
                }
            }
            catch {
                isHaving = false;
            }
            return isHaving;
        }

        public static string Post(string url, string postData)
        {
            var encoding = new ASCIIEncoding();
            var data = encoding.GetBytes(postData);

            var myRequest = (HttpWebRequest) WebRequest.Create(url);

            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = data.Length;
            using (var newStream = myRequest.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
                newStream.Close();
            }

            using (var myResponse = (HttpWebResponse) myRequest.GetResponse())
            {
                var reader = new StreamReader(myResponse.GetResponseStream(), Encoding.Default);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        ///     把服務器返回的Cookies信息寫入到客戶端中
        /// </summary>
        public static void Cookies(WebClient wc)
        {
            if (wc.ResponseHeaders == null) return;
            var setcookie = wc.ResponseHeaders[HttpResponseHeader.SetCookie];
            if (String.IsNullOrEmpty(setcookie)) return;
            var cookie = wc.Headers[HttpRequestHeader.Cookie];
            var cookieList = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(cookie))
            {
                foreach (var ck in cookie.Split(';'))
                {
                    var key = ck.Substring(0, ck.IndexOf('='));
                    var value = ck.Substring(ck.IndexOf('=') + 1);
                    if (!cookieList.ContainsKey(key)) cookieList.Add(key, value);
                }
            }

            foreach (var ck in setcookie.Split(';'))
            {
                var str = ck;
                while (Enumerable.Contains(str, ',') && str.IndexOf(',') < str.LastIndexOf('=')) { str = str.Substring(str.IndexOf(',') + 1); }
                var key = str.IndexOf('=') != -1 ? str.Substring(0, str.IndexOf('=')) : "";
                var value = str.Substring(str.IndexOf('=') + 1);
                if (!cookieList.ContainsKey(key)) { cookieList.Add(key, value); }
                else
                { cookieList[key] = value; }
            }

            var list = new string[cookieList.Count()];
            var index = 0;
            foreach (var pair in cookieList)
            {
                list[index] = String.Format("{0}={1}", pair.Key, pair.Value);
                index++;
            }

            wc.Headers[HttpRequestHeader.Cookie] = list.ToString(";");
        }

        /// <summary>
        ///     获取网络IP
        /// </summary>
        public static string GetIP()
        {
            var ip = "127.0.0.1";
            var strHostName = Dns.GetHostName();
            var ipHost = Dns.GetHostEntry(strHostName);
            foreach (var item in ipHost.AddressList) { if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) { ip = item.ToString(); } }
            return ip;
        }
    }
}