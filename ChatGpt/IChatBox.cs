namespace ChatGpt
{
    public interface IChatBox
    {
        Task<string> GetResponse(string message);
    }
}