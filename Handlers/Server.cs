using Exiled.API.Features;
using Exiled.Events.EventArgs;

using Plugin = SCP_Bit.BitPlugin;

namespace SCP_Bit.Handlers
{
    public class Server
    {
        public void OnWaitingForPlayers()
        {
            Log.Info("Standing by for players...");
        }

        public async void OnRoundStarted()
        {
            await Plugin.Instance.WebhookExecutor.ExecuteWebhook(
                $"Round started!",
                "SCP-Bot",
                false);
        }

        public async void OnRoundEnded(RoundEndedEventArgs ev)
        {
            await Plugin.Instance.WebhookExecutor.ExecuteWebhook(
                $"Round ended, restarting in {ev.TimeToRestart} seconds",
                "SCP-Bot",
                false);
        }
    }
}