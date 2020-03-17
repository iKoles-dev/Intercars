using System;
using System.IO;
using System.Net;
using System.Text;

namespace Homebrew
{

    /// <summary>
    /// Парсер ссылок.
    /// Принимает HttpWebRequest в конструктор
    /// </summary>
    /// <seealso cref="ReqParametres"/>
    public class LinkParser
    {
        public string Data { get; private set; }
        public CookieContainer Cookies { get; private set; }
        public bool IsError { get; private set; }
        private HttpWebRequest request;
        private Encoding _encoding = Encoding.GetEncoding(1251);
        public LinkParser(HttpWebRequest httpRequest)
        {
            request = httpRequest;
            StartParsing();
        }
        public LinkParser(HttpWebRequest httpRequest, Encoding encoding)
        {
            _encoding = encoding;
            request = httpRequest;
            StartParsing();
        }
        private void StartParsing()
        {

            if (request.CookieContainer == null || request.CookieContainer.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
            }
            try
            {
                string data;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                IsError = response.StatusCode == HttpStatusCode.OK ? false : true;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream;
                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, _encoding);
                    }
                    data = readStream.ReadToEnd();
                    CookieContainer cookieContainer = new CookieContainer();
                    foreach (Cookie cookie in response.Cookies)
                    {
                        cookieContainer.Add(cookie);
                    }
                    Cookies = cookieContainer;
                    response.Close();
                    readStream.Close();
                }
                else
                {
                    data = "";
                }
                Data = data;
            }
            catch (Exception)
            {
                IsError = true;
                Data = "";
            }
        }
    }
}
