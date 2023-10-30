namespace BitServerClient
{
    public static class BitServerAPI
    {
        public static string BalanceSymbol(string index) => $"/Balance/Value?index={index}";
        public static string KlineSPOT(string index, string interval, int limit) => $"/klines?symbol={index}&interval={interval}&limit={limit}";
        public static string Kline(string index, string interval) => $"/api/v1/contract/kline/{index}?interval={interval}";
    }
}
