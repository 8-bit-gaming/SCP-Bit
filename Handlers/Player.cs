using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Exiled.Events.EventArgs;
using SCP_Bit.Utils;
using scp_bit_vs.Utils;
using Plugin = SCP_Bit.BitPlugin;

namespace SCP_Bit.Handlers
{
    public class Player
    {
        // A List to prevent webhook spam if a player is kicked/banned.
        private readonly List<int> _recentlyRemoved = new List<int>();
        private readonly Plugin _plugin;
        private readonly Webhook _publicWebhook;
        private readonly Webhook _privateWebhook;

        public Player(Plugin instance)
        {
            this._plugin = instance;
            this._publicWebhook = _plugin.PublicWebhookExecutor;
            this._privateWebhook = _plugin.PrivateWebhookExecutor;
        }

        public async void OnBanned(BannedEventArgs ev)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("**The ban hammer has fallen!**");
            builder.AppendLine($"Player: {ev.Target.Nickname} ({ev.Target.UserId})");
            builder.AppendLine($"Issuer: {ev.Issuer.Nickname} ({ev.Issuer.UserId})");

            var expiresDate = new DateTime(ev.Details.Expires);
            var relative = TimeUtils.ToRelativeTimeFuture(expiresDate, DateTime.UtcNow);
            builder.AppendLine($"Expires {relative}");
            builder.AppendLine($"Reason: {ev.Details.Reason}");

            _recentlyRemoved.Add(ev.Target.Id);

            await _publicWebhook.ExecuteWebhook(
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
            builder.AppendLine($"Name: {ev.Target.Nickname} ({ev.Target.UserId}");
            builder.AppendLine($"Reason: {ev.Reason}");

            _recentlyRemoved.Add(ev.Target.Id);

            await _publicWebhook.ExecuteWebhook(
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

            var playerCount = Exiled.API.Features.Player.List.Count();
            var playerText = playerCount > 1 ? "players" : "player";
            await _publicWebhook.ExecuteWebhook(
               $"{ev.Player.Nickname} has left the server. ({playerCount - 1} {playerText} active)",
                "SCP-Bot",
                false);

        }
    }
}