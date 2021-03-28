using Exiled.API.Features;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SCP_Bit.Utils
{
    public class Webhook
    {
        private readonly string _token;

        public Webhook(string token)
        {
            _token = token;
        }

        private class WebhookRequest
        {

            [JsonProperty("token")]
            public string Token { get; set; }

            [JsonProperty("content")]
            public string Content { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("wait")]
            public bool WaitForResponse { get; set; }

            public override string ToString()
            {
                return $"Username = '{Username}', Content = '{Content}', ShouldWait = {WaitForResponse}";
            }
        }

        private readonly HttpClient _client = new HttpClient();

        // Issues a request to my private webhook server to queue and handle it.
        public async Task<string> ExecuteWebhook(string content, string username, bool wait)
        {
            Log.Debug("Issuing webhook request...");
            var payload = new WebhookRequest
            {
                Token = _token,
                Content = content,
                Username = username,
                WaitForResponse = wait
            };

            Log.Debug(payload.ToString());

            var stringPayload = JsonConvert.SerializeObject(payload);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(BitPlugin.Instance.Config.WebhookServer, httpContent);

            Log.Debug($"Response Status: {response.StatusCode}");
            
            if (!payload.WaitForResponse) return null;

            return await response.Content.ReadAsStringAsync();
        }
    }
}