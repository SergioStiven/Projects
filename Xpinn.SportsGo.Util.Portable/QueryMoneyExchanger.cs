using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.Models;

namespace Xpinn.SportsGo.Util.Portable
{
    public class QueryMoneyExchanger
    {
        /// <summary>
        /// An abbreviation of both currencies is required (Source and Destination)
        /// </summary>
        /// <param name="abreviaturaMonedaParaCambiar">Currency to be changed</param>
        /// <param name="abreviaturaMonedaDestinoCambio">Currency change destination</param>
        /// <returns></returns>
        public async Task<YahooExchangeEntity> QueryMoneyExchange(string abreviaturaMonedaParaCambiar, string abreviaturaMonedaDestinoCambio)
        {
            if (string.IsNullOrWhiteSpace(abreviaturaMonedaParaCambiar)) throw new ArgumentException("You must set the currency you want to change!.");
            if (string.IsNullOrWhiteSpace(abreviaturaMonedaDestinoCambio)) throw new ArgumentException("You must set the destiny currency!.");

            abreviaturaMonedaParaCambiar = abreviaturaMonedaParaCambiar.Trim();
            abreviaturaMonedaDestinoCambio = abreviaturaMonedaDestinoCambio.Trim();

            if (abreviaturaMonedaParaCambiar == abreviaturaMonedaDestinoCambio) throw new ArgumentException("It is not logical to convert the currency to the same currency!.");

            string urlToQuery = string.Format(@"https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.xchange%20where%20pair%20in%20(%22{0}{1}%22)&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=", abreviaturaMonedaParaCambiar, abreviaturaMonedaDestinoCambio);

            return await ExecuteQueryExchange(urlToQuery);
        }

        async Task<YahooExchangeEntity> ExecuteQueryExchange(string urlExchangeToQuery)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(urlExchangeToQuery);

                YahooExchangeEntity exchangeEntity = null;

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    exchangeEntity = JsonConvert.DeserializeObject<YahooExchangeEntity>(content);
                }

                return exchangeEntity;
            }
        }
    }
}