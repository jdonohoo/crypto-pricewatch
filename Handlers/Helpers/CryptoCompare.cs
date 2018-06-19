using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Handlers.Helpers
{
    public static class CryptoCompare
    {
        public static double GetCurrentCryptoPrice(string symbol)
        {
            var api = AppConfig.Instance.GetParameter("CoinPriceUrl");
            var request = $"price?fsym={symbol.ToUpper()}&tsyms=USD";
            var client = new WebClient();
            

            var resp = client.DownloadString(string.Concat(api,request));         
            var results = JObject.Parse(resp);
            var price = Convert.ToDouble(results["USD"]);

            return price;
        }
    }
}
