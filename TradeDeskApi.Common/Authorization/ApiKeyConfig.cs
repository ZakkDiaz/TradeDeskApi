namespace TradeDeskApi.Common.Authorization
{
    public class ApiKeyConfig
    {
        public static string ConfigName = "ApiKeyConfig";
        public Dictionary<string, string> Keys { get; set; }
    }
}
