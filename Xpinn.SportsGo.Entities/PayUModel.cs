namespace Xpinn.SportsGo.Entities
{
    public class PayUModel
    {
        public string merchantId { get; set; }
        public string accountId { get; set; }
        public string description { get; set; }
        public string referenceCode { get; set; }
        public decimal amount { get; set; }
        public decimal tax { get; set; }
        public decimal taxReturnBase { get; set; }
        public string currency { get; set; }
        public string buyerEmail { get; set; }
        public string signature { get; set; }
        public int test { get; set; }
        public string Url { get; set; }
    }
}