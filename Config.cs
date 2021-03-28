using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SCP_Bit
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("The server at which all webhook requests should flow to")]
        public string WebhookServer { get; set; } = "http://127.0.0.1:5000/webhook";

        [Description("The webhook token to use")]
        public string WebhookToken { get; set; } = "no";
    }
}