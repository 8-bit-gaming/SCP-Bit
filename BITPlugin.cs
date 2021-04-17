using System;
using Exiled.API.Features;
using SCP_Bit.Utils;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;

namespace SCP_Bit
{
    public class BitPlugin : Plugin<Config>
    {
        private static readonly Lazy<BitPlugin> LazyInstance = new Lazy<BitPlugin>(() => new BitPlugin());
        public static BitPlugin Instance => LazyInstance.Value;

        private Handlers.Player _playerEvents;
        private Handlers.Server _serverEvents;
        public Webhook PublicWebhookExecutor;
        public Webhook PrivateWebhookExecutor;

        public override void OnEnabled()
        {
            PublicWebhookExecutor = new Webhook(Config.PublicWebhookID);
            PrivateWebhookExecutor = new Webhook(Config.PrivateWebhookID);
            RegisterEvents();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            PublicWebhookExecutor = null;
            PrivateWebhookExecutor = null;
        }

        private void RegisterEvents()
        {
            _playerEvents = new Handlers.Player(this);
            _serverEvents = new Handlers.Server(this);

            Server.WaitingForPlayers += _serverEvents.OnWaitingForPlayers;

            // Public events
            Server.RoundStarted += _serverEvents.OnRoundStarted;
            Server.RoundEnded += _serverEvents.OnRoundEnded;

            Player.Verified += _playerEvents.OnVerified;
            Player.Left += _playerEvents.OnLeave;
            Player.Banned += _playerEvents.OnBanned;
            Player.Kicked += _playerEvents.OnKicked;
        }

        private void UnregisterEvents()
        {
            Server.WaitingForPlayers -= _serverEvents.OnWaitingForPlayers;
            Server.RoundStarted -= _serverEvents.OnRoundStarted;
            Server.RoundEnded -= _serverEvents.OnRoundEnded;

            Player.Verified -= _playerEvents.OnVerified;
            Player.Left -= _playerEvents.OnLeave;
            Player.Banned -= _playerEvents.OnBanned;
            Player.Kicked -= _playerEvents.OnKicked;

            _playerEvents = null;
            _serverEvents = null;
        }
    }
}