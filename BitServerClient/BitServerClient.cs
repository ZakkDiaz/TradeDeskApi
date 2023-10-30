
namespace BitServerClient
{
    public class BitServerClient
    {
        private static string _apiUrl = "https://contract.mexc.com";
        private HttpClient _httpClient;
        public BitServerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetSymbol(string symbol)
        {
            return await GetAs<decimal>(BitServerAPI.BalanceSymbol(symbol));
        }

        public async Task<Kline> GetKlines(string symbol, IntervalEnum interval)
        {
            if (interval < IntervalEnum.Min1)
                return new Kline();
            return await GetAs<Kline>(BitServerAPI.Kline(symbol, interval.ToString()));
        }

        internal async Task<string> SendGet(string path)
        {
            try {  
                var request = new HttpRequestMessage();
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(string.Join("", _apiUrl, path));
                var res = await _httpClient.SendAsync(request);
                var str = await res.Content.ReadAsStringAsync();
            return str;
            } catch(Exception ex)
            {
                
            }
            return "";
        }

        internal async Task<T> GetAs<T>(string path) => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(await SendGet(path));
    }
}