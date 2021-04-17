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
            builder.AppendLine($"Name: {ev.Target.Nickname} ({ev.Target.UserId})");
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

        // private

        public async void OnHurting(HurtingEventArgs ev)
        {
            // Team damage
            if ((ev.Target.Team == ev.Attacker.Team) && (ev.Target != ev.Attacker))
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("**TEAM DAMAGE**");
                builder.AppendLine($"Attacker: {ev.Attacker.Nickname} ({ev.Attacker.UserId})");
                builder.AppendLine($"Target: {ev.Target.Nickname} ({ev.Target.UserId})");
                builder.AppendLine($"Weapon: {ev.HitInformations.GetDamageName()}");
                builder.AppendLine($"Amount: {ev.HitInformations.Amount}");

                await _privateWebhook.ExecuteWebhook(
                    builder.ToString(),
                    "SCP-Police",
                    false);
            }
        }

        // Record times for how long a player was in the pocket dimension
        private readonly Dictionary<int, DateTime> _pocketDimensionTime = new Dictionary<int, DateTime>();

        public async void OnPocketDimensionEnter(EnteringPocketDimensionEventArgs ev)
        {
            var player = ev.Player;
            var dateTime = DateTime.UtcNow;

            _pocketDimensionTime.Add(player.Id, dateTime);
            await _privateWebhook.ExecuteWebhook(
                $"{player.Nickname} entered the pocket dimension!",
                "SCP-Bot",
                false
            );
        }

        public async void OnPocketDimensionEscape(EscapingPocketDimensionEventArgs ev)
        {
            var player = ev.Player;
            var currentTime = DateTime.UtcNow;
            var initialTime = _pocketDimensionTime[player.Id];
            var relativeTime = TimeUtils.ToRelativeTimeFuture(initialTime, currentTime);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("**Player escaped the pocket dimension**");
            builder.AppendLine($"Name: {player.Nickname} ({player.UserId})");
            builder.AppendLine($"Escaped {relativeTime}");

            _pocketDimensionTime.Remove(player.Id);

            await _privateWebhook.ExecuteWebhook(
                builder.ToString(),
                "SCP-Bot",
                false
            );
        }

        public async void OnPocketDimensionEscapeFailed(FailingEscapePocketDimensionEventArgs ev)
        {
            var player = ev.Player;
            var currentTime = DateTime.UtcNow;
            var initialTime = _pocketDimensionTime[player.Id];
            var relativeTime = TimeUtils.ToRelativeTimeFuture(initialTime, currentTime);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("**Player failed to escape the pocket dimension**");
            builder.AppendLine($"Name: {player.Nickname} ({player.UserId})");
            builder.AppendLine($"Failed escape {relativeTime}");

            _pocketDimensionTime.Remove(player.Id);

            await _privateWebhook.ExecuteWebhook(
                builder.ToString(),
                "SCP-Bot",
                false
            );
        }

        public async void OnWarheadPanelActivating(ActivatingWarheadPanelEventArgs ev)
        {
            if (ev.IsAllowed)
            {

                StringBuilder builder = new StringBuilder();
                builder.AppendLine("**Activating Warhead Panel**");
                builder.AppendLine($"Name: {ev.Player.Nickname} ({ev.Player.UserId})");

                await _privateWebhook.ExecuteWebhook(
                    builder.ToString(),
                    "SCP-Bot",
                    false
                );
            }
        }

        public async void OnEscape(EscapingEventArgs ev)
        {
            if (ev.IsAllowed)
            {
                await _privateWebhook.ExecuteWebhook(
                    $"{ev.Player.Nickname} ({ev.Player.UserId}) escaped!",
                    "SCP-Bot",
                    false
                );
            }
        }

        public async void OnHandcuffing(HandcuffingEventArgs ev)
        {
            if (ev.IsAllowed)
            {
                await _privateWebhook.ExecuteWebhook(
                    $"{ev.Cuffer.Nickname} ({ev.Cuffer.UserId}) is handcuffing {ev.Target.Nickname} ({ev.Target.UserId})!",
                    "SCP-Bot",
                    false
                );
            }
        }

        public async void OnIntercomSpeaking(IntercomSpeakingEventArgs ev)
        {
            if (ev.IsAllowed)
            {
                await _privateWebhook.ExecuteWebhook(
                     $"{ev.Player.Nickname} ({ev.Player.UserId}) is speaking on the intercom",
                     "SCP-Bot",
                     false
                );
            }
        }
    }
}