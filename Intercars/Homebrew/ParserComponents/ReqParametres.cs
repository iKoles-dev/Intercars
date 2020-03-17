using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Homebrew
{
    /// <summary>
    /// Класс отвечает за создание и настройку нового запрос.
    /// <para>Не работает без инициализации конструктора
    /// <seealso cref="ReqParametres"/></para>
    /// </summary>
    class ReqParametres
    {
        private HttpWebRequest _request;
        public HttpWebRequest Request
        {
            get
            {
                _request.Method = _httpMethod.ToString();
                if (_reqData.Length > 0)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] byte1 = encoding.GetBytes(_reqData);
                    _request.ContentLength = byte1.Length;
                    Stream newStream = _request.GetRequestStream();
                    newStream.Write(byte1, 0, byte1.Length);
                }
                return _request;
            }
        }
        public HttpWebRequest RowRequest => _request;

        private HttpMethod _httpMethod;
        private string _reqData;

        /// <summary>
        /// Основной метод создания нового запроса.
        /// <para><seealso cref="SetUserAgent(string)"/> - установка Юзер-агента</para>
        /// <para><see cref="SetProxy(string, string, string)"/> - установка прокси</para>
        /// <para><see cref="SetReqAdditionalParametres(bool, int, string, CookieCollection)"/> - установка дополнительных параметров</para>
        /// </summary>
        /// <param name="link">Основная ссылка</param>
        /// <param name="httpMethod">Тип запроса (Get, Post,Put и т.д.)</param>
        /// <param name="reqData">Тело запроса</param>
        public ReqParametres(String link, HttpMethod httpMethod = HttpMethod.GET, String reqData = "")
        {
            _request = (HttpWebRequest)WebRequest.Create(LinkFormatter(link));
            _httpMethod = httpMethod;
            _reqData = reqData;
        }
        /// <summary>
        /// Установка дополнительных параметров
        /// </summary>
        /// <param name="allowAutoRedirect">Включить авторедирект?</param>
        /// <param name="maximumAutomaticRedirections">Максимальное количество редиректов</param>
        /// <param name="contentType">Тип контента задаётся через класс "ParserContentType"</param>
        /// <param name="cookieCollection">Cookie задаются через "CookieCollection"</param>
        public void SetReqAdditionalParametres(bool allowAutoRedirect = true, int maximumAutomaticRedirections = 100,
            String contentType = "application/x-www-form-urlencoded")
        {
            _request.MaximumAutomaticRedirections = maximumAutomaticRedirections;
            _request.AllowAutoRedirect = allowAutoRedirect;
            _request.ContentType = contentType;
        }
        /// <summary>
        /// Установка прокси
        /// </summary>
        /// <param name="proxy">IP-адрес прокси</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void SetProxy(int timeOut = 5000, string proxy = "")
        {
            SetTimout(timeOut);
            try
            {
                if (proxy.Equals(""))
                {
                    _request.Proxy = new WebProxy(new Uri($"http://{Proxies.GetProxy()}"));
                }
                else
                {
                    string tempProxy = proxy.StartsWith("http") ? proxy : $"http://{proxy}";
                }
            }
            catch (Exception)
            {
                SetProxy(timeOut, proxy);
            }
        }
        /// <summary>
        /// Установка юзер-агента
        /// </summary>
        /// <param name="userAgent">Юзер-агент</param>
        public void SetUserAgent(string userAgent)
        {
            _request.UserAgent = userAgent;
        }
        public void SetTimout(int timeout)
        {
            _request.Timeout = timeout;
        }
        public void SetCookie(CookieContainer cookieContainer)
        {
            _request.CookieContainer = cookieContainer;
        }
        public void AddCookie(Cookie cookie)
        {
            if (_request.CookieContainer == null)
            {
                _request.CookieContainer = new CookieContainer();
            }
            _request.CookieContainer.Add(cookie);
        }
        public void AddCookie(CookieCollection cookie)
        {
            if (_request.CookieContainer == null)
            {
                _request.CookieContainer = new CookieContainer();
            }
            _request.CookieContainer.Add(cookie);
        }
        public void AddCookie(string cookie, bool multiline = false)
        {
            if (!multiline)
            {
                string name = "";
                string value = "";
                if (cookie.Contains("=") && cookie.Split('=').Length == 2)
                {
                    name = cookie.Split('=')[0];
                    value = cookie.Split('=')[1];
                    AddCookie(new Cookie(name, value));
                }
            }
            else
            {
                if (cookie.Contains("="))
                {
                    List<string> allCookies = new List<string>(cookie.Split(';'));
                    CookieCollection cookieCollection = new CookieCollection();
                    allCookies.ForEach(cook =>
                    {
                        if (cook.Contains("=") && cook.Split('=').Length == 2)
                        {
                            cookieCollection.Add(
                                new Cookie(
                                    cook.Split('=')[0],
                                    cook.Split('=')[1]
                                    ));
                        }
                    });
                    AddCookie(cookieCollection);
                }
            }
        }
        private static String LinkFormatter(String link)
        {
            String newLink = link;
            if (!newLink.StartsWith("http"))
            {
                if (!newLink.Contains("www."))
                {
                    newLink = $"www.{newLink}";
                }
                newLink = $"http://{newLink}";
            }
            return newLink;
        }
    }
}
