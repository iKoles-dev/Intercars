using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Homebrew
{
    public static class Proxies
    {
        public static List<string> allProxies = new List<string>();
        private static string bestProxyKey = "f29254716f641546667372a7865d28b1";

        public static string GetProxy()
        {
            if (allProxies.Count < 100)
            {
                RefillProxyList();
            }
            string firsProxy = allProxies[0];
            allProxies.RemoveAt(0);
            return firsProxy;
        }
        public static void DeleteProxy(string invalidProxy)
        {
            allProxies.Remove(invalidProxy);
            if (allProxies.Count < 100)
            {
                RefillProxyList();
            }
        }
        private static void RefillProxyList()
        {
            ReqParametres req = new ReqParametres($"https://api.best-proxies.ru/proxylist.txt?key={bestProxyKey}&limit=0&type=http&response=10000");
            LinkParser linkParser = new LinkParser(req.Request);
            Regex regex = new Regex("(\\d{1,3}\\.){3}\\d{1,3}:(\\d+)");
            MatchCollection matches = regex.Matches(linkParser.Data);
            foreach (Match match in matches)
            {
                allProxies.Add(match.Groups[0].Value);
            }
        }

    }
}
