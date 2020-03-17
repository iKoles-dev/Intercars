namespace Intercars.Controller.Khcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Windows.Controls;

    using Homebrew;

    using Newtonsoft.Json;

    public class KhcodeSearcher
    {
        public string Khcode = "";

        private int _count = 0;
        public KhcodeSearcher()
        {
            Controls.DebugBox.WriteLine("Поиск мета-файла.");
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\meta.meta"))
            {
                File.Create(Directory.GetCurrentDirectory() + "\\meta.meta").Dispose();
                Controls.DebugBox.WriteLine("Файл не обнаружен. Создание мета файла.");
            }

            string file = File.ReadAllText(Directory.GetCurrentDirectory() + "\\meta.meta");
            Khcode khcode = JsonConvert.DeserializeObject<Khcode>(file);
            if (khcode == null || khcode.Code == "")
            {
                Controls.DebugBox.WriteLine("Файл пуст. Начинаем поиск доступного Khcode.");
                Search();
                while (Khcode=="")
                {
                    Thread.Sleep(1000);
                    Controls.WorkProgressLabel.Set($"Проверено {_count} кодов.");
                }
                khcode = new Khcode();
                khcode.Code = Khcode;
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\meta.meta",JsonConvert.SerializeObject(khcode));
            }
            else
            {
                if (!IsValidCode(khcode.Code))
                {
                    Controls.DebugBox.WriteLine("Khcode неверен. Начинаем поиск верного Khcode.");
                    Search();
                    while (Khcode == "")
                    {
                        Thread.Sleep(1000);
                        Controls.WorkProgressLabel.Set($"Проверено {_count} кодов.");
                    }
                }
                else
                {
                    Khcode = khcode.Code;
                    khcode = new Khcode();
                    khcode.Code = Khcode;
                    File.Delete(Directory.GetCurrentDirectory() + "\\meta.meta");
                    File.Create(Directory.GetCurrentDirectory() + "\\meta.meta").Dispose();
                    File.WriteAllText(Directory.GetCurrentDirectory() + "\\meta.meta", JsonConvert.SerializeObject(khcode));
                }
            }
            Controls.DebugBox.WriteLine("Khcode найден!");

        }

        private void Search()
        {
            for (int i = 30000; i < 50000; i += 1000)
            {
                SetKhcodeProcess(i);
            }
        }

        private void SetKhcodeProcess(int count)
        {
            Thread thread = new Thread(
                (() =>
                        {
                            for (int i = count; i < count + 1000; i++)
                            {
                                _count++;
                                if (Khcode != "")
                                {
                                    break;
                                }

                                ReqParametres req = new ReqParametres(
                                    "https://ic-ua.intercars.eu/dynamic/ickatalogweb/ws_zamienniki.php?popup=T&firgru=|UJ5|UJ8|UR1|UR3&towkod=BC421D&zakres=all");
                                req.SetUserAgent(
                                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.34 Safari/537.36");
                                CookieContainer cookieContainer = new CookieContainer();
                                cookieContainer.Add(
                                    new Uri("https://ic-ua.intercars.eu/"),
                                    new Cookie("khkod", "U" + i));
                                cookieContainer.Add(new Uri("https://ic-ua.intercars.eu/"), new Cookie("lang", "UA"));
                                cookieContainer.Add(new Uri("https://ic-ua.intercars.eu/"), new Cookie("kraj", "UA"));
                                cookieContainer.Add(
                                    new Uri("https://ic-ua.intercars.eu/"),
                                    new Cookie("PHPSESSID", "elo485mp69q42ejdgjssd23ev4"));
                                req.SetCookie(cookieContainer);
                                LinkParser link = new LinkParser(req.Request);
                                List<string> zaminniki = link.Data.ParsRegex(" <b>(.*?)<", 1);
                                if (zaminniki.Count > 0)
                                {
                                    Khcode = i.ToString();
                                    break;
                                }
                            }
                        }));
            thread.IsBackground = true;
            thread.Start();
        }

        private bool IsValidCode(string code)
        {
            ReqParametres req = new ReqParametres("https://ic-ua.intercars.eu/dynamic/ickatalogweb/ws_zamienniki.php?popup=T&firgru=|UJ5|UJ8|UR1|UR3&towkod=BC421D&zakres=all");
            req.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.34 Safari/537.36");
            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri("https://ic-ua.intercars.eu/"),new Cookie("khkod","U"+code));
            cookieContainer.Add(new Uri("https://ic-ua.intercars.eu/"), new Cookie("lang", "UA"));
            cookieContainer.Add(new Uri("https://ic-ua.intercars.eu/"), new Cookie("kraj", "UA"));
            cookieContainer.Add(new Uri("https://ic-ua.intercars.eu/"), new Cookie("PHPSESSID", "elo485mp69q42ejdgjssd23ev4"));
            req.SetCookie(cookieContainer);
            LinkParser link = new LinkParser(req.Request);
            List<string> zaminniki = link.Data.ParsRegex(" <b>(.*?)<", 1);
            if (zaminniki.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}