using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SCP_Bit
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("The server at which all webhook requests should flow to")]
        public string WebhookServer { get; set; } = "http://127.0.0.1:4001/webhook/execute";

        [Description("Auth token to use when making webhook requests")]
        public string WebhookAuth { get; set; } = "no";

        [Description("The public webhook id for public logs")]
        public string PublicWebhookID { get; set; } = "no";

        [Description("The private webhook id for private logs")]
        public string PrivateWebhookID { get; set; } = "no";
    }
}