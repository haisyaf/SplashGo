using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DotNetEnv;

namespace SplashGoJunpro.Services
{
    public class MidtransService
    {
        private const string BaseUrl = "https://app.sandbox.midtrans.com/snap/v1/transactions";
        private readonly string _serverKey;

        public MidtransService()
        {
            // Load .env file from the project root
            Env.Load();
            
            _serverKey = Environment.GetEnvironmentVariable("MIDTRANS_SERVER_KEY") 
                ?? throw new InvalidOperationException("MIDTRANS_SERVER_KEY environment variable not set");
        }

        public async Task<string> GetSnapTokenAsync(MidtransTransactionRequest request)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_serverKey}:"));
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic {authHeader}");

                    var json = JsonConvert.SerializeObject(request);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(BaseUrl, content);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    return result["token"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Midtrans API call failed: {ex.Message}", ex);
            }
        }

        public async Task<MidtransTransactionStatus> GetTransactionStatusAsync(string orderId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_serverKey}:"));
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic {authHeader}");

                    var response = await client.GetAsync($"{BaseUrl}/{orderId}/status");
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MidtransTransactionStatus>(responseContent);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get transaction status: {ex.Message}", ex);
            }
        }
    }

    public class MidtransTransactionRequest
    {
        public TransactionDetails transaction_details { get; set; }
        public CustomerDetails customer_details { get; set; }
        public List<ItemDetail> item_details { get; set; }
    }

    public class TransactionDetails
    {
        public string order_id { get; set; }
        public long gross_amount { get; set; }
    }

    public class CustomerDetails
    {
        public string first_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string id_number { get; set; }
    }

    public class ItemDetail
    {
        public string id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public long price { get; set; }
    }

    public class MidtransTransactionStatus
    {
        public string transaction_status { get; set; }
        public string fraud_status { get; set; }
        public string order_id { get; set; }
        public long gross_amount { get; set; }
    }
}