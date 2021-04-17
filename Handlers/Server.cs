using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System.Linq;
using Plugin = SCP_Bit.BitPlugin;

namespace SCP_Bit.Handlers
{
    public class Server
    {

        private readonly Plugin _plugin;

        public Server(Plugin instance)
        {
            this._plugin = instance;
        }

        public void OnWaitingForPlayers()
        {
            Log.Info("Standing by for players...");
        }

        public async void OnRoundStarted()
        {
            var playerCount = Exiled.API.Features.Player.List.Count();
            var playerText = playerCount > 1 ? "players" : "player";
            await _plugin.PublicWebhookExecutor.ExecuteWebhook(
                $"Round started with {playerCount} {playerText}!",
                "SCP-Bot",
                false);
        }

        public async void OnRoundEnded(RoundEndedEventArgs ev)
        {
            var playerCount = Exiled.API.Features.Player.List.Count();
            var playerText = playerCount > 1 ? "players" : "player";
            await _plugin.PublicWebhookExecutor.ExecuteWebhook(
                $"Round ended with {playerCount} {playerText}, restarting in {ev.TimeToRestart} seconds",
                "SCP-Bot",
                false);
        }
    }
}