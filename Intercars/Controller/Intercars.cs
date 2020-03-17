namespace Intercars.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text.RegularExpressions;

    using Homebrew;

    public class Intercars
    {
        private string _excelPath = "";
        private List<IntercarsProfile> _intercarsProfiles = new List<IntercarsProfile>();

        private string _khcode = "";
        public Intercars(string path, string khcode)
        {
            _excelPath = path;
            _khcode = khcode;
            ExcelWork excel = new ExcelWork();
            _intercarsProfiles = excel.ExcelRead(_excelPath);
            Controls.DebugBox.WriteLine("Приступаем к парсингу данных.");
            StartParsing();
            excel.ExcelWrite(_intercarsProfiles);
        }

        private void StartParsing()
        {
            int position = 0;
            _intercarsProfiles.ForEach(
                profile =>
                    {
                        Parser(profile);
                        position++;
                        Controls.WorkProgress.SetValue(position/(double)_intercarsProfiles.Count*100);
                        Controls.WorkProgressLabel.Set($"{position}/{_intercarsProfiles.Count}");
                    });
        }

        private void Parser(IntercarsProfile profile)
        {
            SetTovarCode(profile);
            if (profile.TovarCode == "")
            {
                return;
            }

            SetParametres(profile);


        }

        private void SetTovarCode(IntercarsProfile profile)
        {
            ReqParametres req = new ReqParametres($"https://ic-ua.intercars.eu/dynamic/uni/ws_towary.php?wit=ICKATALOGWEB&pro=&kraj=UA&oesearch={profile.Number}&ofe=", HttpMethod.POST, $"oesearch={profile.Number}");
            req.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.34 Safari/537.36");
            SetCookies(req);
            req.RowRequest.ContentType = "application/x-www-form-urlencoded";
            LinkParser link = new LinkParser(req.Request);

            List<string> allCodes = link.Data.Replace("\n", "").Replace("\r", "").ParsRegex("Daj_Katalog_Detail_Clob(.*?);", 1);
            List<string> uniqCodes = new List<string>();
            allCodes.ForEach(
                code =>
                    {
                        string rawCode = code.Split(',')[code.Split(',').Length - 1].ParsFromTo("'", "'");
                        if (!uniqCodes.Contains(rawCode) && Regex.IsMatch(rawCode, "[A-Z0-9]+"))
                        {
                            uniqCodes.Add(rawCode);
                        }
                    });
            if (uniqCodes.Count > 0)
            {
                profile.TovarCode = uniqCodes[0];
            }
            else
            {
                profile.TovarCode = "";
            }
        }

        private void SetParametres(IntercarsProfile profile)
        {
            ReqParametres req = new ReqParametres("https://ic-ua.intercars.eu/dynamic/uni/ws_towDetail.php?wit=ICKATALOGWEB&p=F",HttpMethod.POST,$"artnr={profile.Number}&witryna=ICKATALOGWEB&towkod={profile.TovarCode}&lang=RU&nb=N&kraj=UA&typ=&wsk=");
            req.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.34 Safari/537.36");
            SetCookies(req);
            req.RowRequest.ContentType = "application/x-www-form-urlencoded";
            LinkParser link = new LinkParser(req.Request);

            profile.Description = link.Data.ParsFromTo("<title>", "<");

            profile.Image = link.Data.ParsFromTo("<img  src=\"", "\"");
            if (profile.Image != "")
            {
                profile.Image = "https://ic-ua.intercars.eu" + profile.Image;
            }

            SetModel(profile,link.Data);
            SetZaminniki(profile, link.Data);
            SetOriginalNumbers(profile);
            List<string> additionalInformation = link.Data.ParsRegex("dKartaNazClass(.*?)>(.*?)<", 2);
            for (int i = 0; i < additionalInformation.Count; i+=2)
            {
                if (i == 0)
                {
                    profile.AdditionalInformation = additionalInformation[i] + " " + additionalInformation[i + 1];
                }
                else
                {
                    profile.AdditionalInformation += "\n" + additionalInformation[i] + " " + additionalInformation[i + 1];
                }
            }
            SetPrices(profile, link.Data);

        }

        private void SetModel(IntercarsProfile profile, string data)
        {
            string numerOe = data.ParsFromTo("daj_numeryOE('", "'");
            profile.NomerOe = numerOe;
            if (numerOe == "")
            {
                return;
            }
            ReqParametres req = new ReqParametres($"https://ic-ua.intercars.eu/dynamic/ickatalogweb/ws_getsoap.php?call=stos&art={numerOe}&wit=ICKATALOGWEB", HttpMethod.POST, $"call=stos&art={numerOe}&wit=ICKATALOGWEB");
            req.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.34 Safari/537.36");
            SetCookies(req);
            req.RowRequest.ContentType = "application/x-www-form-urlencoded";
            LinkParser link = new LinkParser(req.Request);
            profile.Mark = link.Data.ParsFromTo("class=\"dZB1\" ><u>", "<");
            List<string> models = link.Data.Replace("\n","").Replace("\r","").ParsRegex("class=\"dZB2\"><u>(.*?)<",1);
            for (int i = 0; i < models.Count; i++)
            {
                if (i == 0)
                {
                    profile.Model = models[i];
                }
                else
                {
                    profile.Model += ";\n" + models[i];
                }
            }
        }

        private void SetZaminniki(IntercarsProfile profile, string data)
        {
            string parametres = data.ParsFromTo("&sta=T&fir=UJ8&gru=", "\"");
            profile.Gru = parametres;
            if (parametres == "")
            {
                return;
            }


            ReqParametres req = new ReqParametres($"https://ic-ua.intercars.eu/dynamic/ickatalogweb/ws_zamienniki.php?popup=T&firgru={parametres}&towkod={profile.TovarCode}&zakres=all");
            req.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.34 Safari/537.36");
            SetCookies(req);
            req.RowRequest.ContentType = "application/x-www-form-urlencoded";
            LinkParser link = new LinkParser(req.Request);
            List<string> zaminniki = link.Data.ParsRegex(" <b>(.*?)<", 1);
            for (int i = 0; i < zaminniki.Count; i++)
            {
                if (i == 0)
                {
                    profile.Zaminniki = zaminniki[i];
                }
                else
                {
                    profile.Zaminniki += ";\n" + zaminniki[i];
                }
            }
        }

        private void SetOriginalNumbers(IntercarsProfile profile)
        {
            if (profile.NomerOe == "")
            {
                return;
            }
            ReqParametres req = new ReqParametres($"https://ic-ua.intercars.eu/dynamic/ickatalogweb/ws_getsoap.php?call=numoe&art={profile.NomerOe}&wit=ICKATALOGWEB", HttpMethod.POST, $"call=numoe&art={profile.NomerOe}&wit=ICKATALOGWEB");
            req.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.34 Safari/537.36");
            SetCookies(req);
            req.RowRequest.ContentType = "application/x-www-form-urlencoded";
            LinkParser link = new LinkParser(req.Request);
            List<string> numbers = link.Data.ParsRegex("> ([0-9]+)", 1);
            for (int i = 0; i < numbers.Count; i++)
            {
                if (i == 0)
                {
                    profile.OriginalNumbers = numbers[i];
                }
                else
                {
                    profile.OriginalNumbers += ";\n" + numbers[i];
                }
            }
        }

        private void SetPrices(IntercarsProfile profile, string data)
        {
            if (profile.Gru == "")
            {
                return;
            }
            if (data.Contains("checked id=\"VATNF\""))
            {
                profile.PriceType = "З ПДВ";
            }
            else
            {
                profile.PriceType = "Без ПДВ";
            }

            ReqParametres req;
            if (profile.PriceType == "З ПДВ")
            {
                req = new ReqParametres($"https://ic-ua.intercars.eu/dynamic/ickatalogweb/ws_getsoap.php?towkod={profile.TovarCode}&call=dkc&cen=HB-DB-HN-DN&qty=1&sta=T&fir=UJ8&gru={profile.Gru}");
            }
            else
            {
                req = new ReqParametres($"https://ic-ua.intercars.eu/dynamic/ickatalogweb/ws_getsoap.php?towkod={profile.TovarCode}&call=dkc&cen=HN-DN-HB-DB&qty=1&sta=T&fir=UJ8&gru={profile.Gru}");
            }
            req.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.34 Safari/537.36");
            SetCookies(req);
            req.RowRequest.ContentType = "application/x-www-form-urlencoded";
            LinkParser link = new LinkParser(req.Request);
            string[] prices = link.Data.Split('^');
            if (prices.Length > 0)
            {
                profile.PriceRozdrib = prices[0] + " UAH";
            }

            if (prices.Length > 1)
            {
                profile.PriceOpt = prices[1] + " UAH";
                profile.OnlineAvailability = prices[prices.Length - 3].Replace("&gt;",">");
                profile.AvailabilityInBranchGroup = prices[prices.Length - 2];
                profile.AvailabilityInViddelenni = prices[prices.Length - 1];
            }
        }

        private void SetCookies(ReqParametres req)
        {

            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri("https://ic-ua.intercars.eu/"), new Cookie("khkod", "U" + _khcode));
            cookieContainer.Add(new Uri("https://ic-ua.intercars.eu/"), new Cookie("lang", "UA"));
            cookieContainer.Add(new Uri("https://ic-ua.intercars.eu/"), new Cookie("kraj", "UA"));
            cookieContainer.Add(new Uri("https://ic-ua.intercars.eu/"), new Cookie("PHPSESSID", "elo485mp69q42ejdgjssd23ev4"));
            req.SetCookie(cookieContainer);
        }
    }
}