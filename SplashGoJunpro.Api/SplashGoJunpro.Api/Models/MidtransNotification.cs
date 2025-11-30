namespace SplashGoJunpro.Api.Models
{
    public class MidtransNotification
    {
        public string order_code { get; set; }
        public string transaction_status { get; set; }
        public string payment_type { get; set; }
        public string gross_amount { get; set; }
        public string status_code { get; set; }
        public string fraud_status { get; set; }
    }
}
