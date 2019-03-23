using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using IEXTrading.Models;
using Newtonsoft.Json;

namespace IEXTrading.Infrastructure.IEXTradingHandler
{
    public class IEXHandler
    {
        static string BASE_URL = "https://api.iextrading.com/1.0/"; //This is the base URL, method specific URL is appended to this.
        HttpClient httpClient;

        public IEXHandler()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        /****
          * Calls the IEX reference API to get the prices 
         ****/
        public List<Price> GetPrices()
        {
            string IEXTrading_API_PATH = BASE_URL + "stock/aapl/batch?types=chart&range=5d";
            string priceList = "";

            List<Price> prices = null;

            httpClient.BaseAddress = new Uri(IEXTrading_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                priceList = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            if (!priceList.Equals(""))
            {
                prices = JsonConvert.DeserializeObject<List<Price>>(priceList);
                prices = prices.GetRange(0, 50);
            }
            return prices;
        }




        /****
         * Calls the IEX reference API to get the list of symbols. 
        ****/
        public List<Company> GetSymbols()
        {
            string IEXTrading_API_PATH = BASE_URL + "ref-data/symbols";
            string companyList = "";

            List<Company> companies = null;

            httpClient.BaseAddress = new Uri(IEXTrading_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                companyList = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            if (!companyList.Equals(""))
            {
                companies = JsonConvert.DeserializeObject<List<Company>>(companyList);
                companies = companies.GetRange(0, 50);
            }
            return companies;
        }

        /****
         * Calls the IEX stock API to get 1 year's chart for the supplied symbol. 
        ****/
        public List<Equity> GetChart(string symbol)
        {
            //Using the format method.
            //string IEXTrading_API_PATH = BASE_URL + "stock/{0}/batch?types=chart&range=1y";
            //IEXTrading_API_PATH = string.Format(IEXTrading_API_PATH, symbol);

            string IEXTrading_API_PATH = BASE_URL + "stock/" + symbol + "/batch?types=chart&range=1y";

            string charts = "";
            List<Equity> Equities = new List<Equity>();
            httpClient.BaseAddress = new Uri(IEXTrading_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                charts = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            if (!charts.Equals(""))
            {
                ChartRoot root = JsonConvert.DeserializeObject<ChartRoot>(charts, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Equities = root.chart.ToList();
            }
            //make sure to add the symbol the chart
            foreach (Equity Equity in Equities)
            {
                Equity.symbol = symbol;
            }

            return Equities;
        }

        /****
         * Calls the IEX reference API to get a JSON string quote for that symbol.
        ****/
        public dynamic GetQuote(string symbol)
        {
            //URL Addition: stock/<symbol here>/quote
            string IEXTrading_API_Quote = BASE_URL + "stock/" + symbol + "/quote";

            //declare a dynamic variable to hold the JSON returned from the API.
            dynamic quote = "";
            //create the API call and call on it.
            httpClient.BaseAddress = new Uri(IEXTrading_API_Quote);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_Quote).GetAwaiter().GetResult();
            //if we get a positive response from the API throw everything into the dynamic variable.
            if (response.IsSuccessStatusCode)
            {
                quote = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            //if we get a negative response from the API, put an error message in the returned variable.
            if (response.IsSuccessStatusCode == false)
            {
                quote = "Symbol NA";
            }
            return quote;
        }
    }
}
