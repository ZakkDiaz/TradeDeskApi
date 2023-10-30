namespace TradeDeskData.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public decimal Funds { get; set; }
        public bool IsTest { get; set; }
    }
}
