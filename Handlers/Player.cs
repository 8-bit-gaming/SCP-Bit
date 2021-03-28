using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using scp_bit_vs.Utils;
using Plugin = SCP_Bit.BitPlugin;

namespace SCP_Bit.Handlers
{
    public class Player
    {

        // A List to prevent webhook spam if a player is kicked/banned.
        private readonly List<int> _recentlyRemoved = new List<int>();

        public async void OnVerified(VerifiedEventArgs ev)
        {
            await Plugin.Instance.WebhookExecutor.ExecuteWebhook(
                $"{ev.Player.Nickname} has joined the server.",
                "SCP-Bot",
                false);
        }
       
        public async void OnBanned(BannedEventArgs ev)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("**The ban hammer has fallen!**");
            builder.AppendLine($"Player: {ev.Target.Nickname} ({ev.Target.Id})");
            builder.AppendLine($"Issuer: {ev.Issuer.Nickname} ({ev.Issuer.Id})");

            var expiresDate = new DateTime(ev.Details.Expires);
            var relative = TimeUtils.ToRelativeTimeFuture(expiresDate, DateTime.UtcNow);
            builder.AppendLine($"Expires {relative}");
            builder.AppendLine($"Reason: {ev.Details.Reason}");

            _recentlyRemoved.Add(ev.Target.Id);

            await Plugin.Instance.WebhookExecutor.ExecuteWebhook(
                 builder.ToString(),
                 "SCP-Police",
                 false
                );
        }

        public async void OnKicked(KickedEventArgs ev)
        {
            if (_recentlyRemoved.Contains(ev.Target.Id))
                return;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("**Player kicked**");
            builder.AppendLine($"Name: {ev.Target.Nickname}");
            builder.AppendLine($"Reason: {ev.Reason}");

            _recentlyRemoved.Add(ev.Target.Id);

            await Plugin.Instance.WebhookExecutor.ExecuteWebhook(
                builder.ToString(),
                "SCP-Police",
                false
               );
        }

        public async void OnLeave(LeftEventArgs ev)
        {
            if (_recentlyRemoved.Contains(ev.Player.Id))
            {
                _recentlyRemoved.Remove(ev.Player.Id);
                return;
            }

            await Plugin.Instance.WebhookExecutor.ExecuteWebhook(
                $"{ev.Player.Nickname} has left the server.",
                "SCP-Bot",
                false);

        }
    }
}