using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DBot
{
    class ProxyLogic
    {
        public static async Task UpdateList()
        {
            var fil = File.GetLastWriteTimeUtc("proxies.txt").DayOfYear;
            if (fil != DateTime.UtcNow.DayOfYear)
            {
                Console.WriteLine($"Updating proxies ({fil} and {DateTime.UtcNow.DayOfYear})");

                WebClient w = new WebClient();
                var res = await w.DownloadStringTaskAsync("https://raw.githubusercontent.com/clarketm/proxy-list/master/proxy-list-raw.txt");

                File.WriteAllText("proxies.txt", res);
            }
        }
    }
}
