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
        public Webhook WebhookExecutor;

        public override void OnEnabled()
        {
            WebhookExecutor = new Webhook(Config.WebhookToken);
            RegisterEvents();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            WebhookExecutor = null;
        }

        private void RegisterEvents()
        {
            _playerEvents = new Handlers.Player();
            _serverEvents = new Handlers.Server();

            Server.WaitingForPlayers += _serverEvents.OnWaitingForPlayers;
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