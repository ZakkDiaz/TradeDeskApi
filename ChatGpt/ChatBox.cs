using Microsoft.Extensions.Options;

namespace ChatGpt
{
    public class ChatBox : IChatBox
    {
        IOptions<ChatBoxConfig> _config;
        HttpClient _chatClient;
        public ChatBox(IOptions<ChatBoxConfig> config, HttpClient chatClient)
        {
            _config = config;
            _chatClient = chatClient;
        }

        public async Task<string> GetResponse(string message)
        {
            throw new NotImplementedException();
            //var url = ChatGptApi.Completion();
            //var result = await _chatClient.SendAsync(new HttpRequestMessage() { Method = HttpMethod.Get, Headers });
            //var content = await result.Content.ReadAsStringAsync();
        }
    }
}
