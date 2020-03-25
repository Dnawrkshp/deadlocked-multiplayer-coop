using DLMC.Shared;
using DLMC.Shared.Message;
using DLMC.Shared.Utils;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DLMC.Server
{
    public class ServerLogic : BaseLogic, INetEventListener
    {
        // 
        private NetManager _netManager;
        public override NetManager NetMan => _netManager;

        // 
        public override bool IsServer => true;
        public override NetPeer Server => null;
        public override byte LocalPlayerId => 0;
        public override byte RemotePlayerId => 1;

        // 
        private LogicTimer _logicTimer;
        private MessageCache _cachedMessages;
        public override LogicTimer Logic => _logicTimer;

        // 
        public event Action<MessageBase> OnMessage;
        public event Action OnClientConnected;
        public event Action OnClientDisconnected;
        public event Action OnTick;

        public ServerLogic()
        {
            Singleton = this;
            
            _logicTimer = new LogicTimer(OnLogicUpdate);
            _cachedMessages = new MessageCache();

            _netManager = new NetManager(this)
            {
                AutoRecycle = true,
                DisconnectTimeout = 5000
            };
        }

        ~ServerLogic()
        {
            Stop();
        }

        protected virtual void OnLogicUpdate()
        {
            // Raise tick
            OnTick?.Invoke();
        }

        public override void Update()
        {
            _netManager.PollEvents();
            _logicTimer.Update();
        }

        public void Start(int port)
        {
            if (_netManager.IsRunning)
                return;

            _netManager.Start(port);
            _logicTimer.Start();
        }

        public override void Stop()
        {
            if (_netManager != null && _netManager.IsRunning)
                _netManager.Stop();

            if (_logicTimer != null)
                _logicTimer.Stop();
        }


        #region Net Events

        public virtual void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            MessageId id = reader.PeekEnum<MessageId>();
            if (!id.IsValid())
                return;

            var msg = _cachedMessages.Deserialize(id, reader);
            if (!msg.IsValid())
                return;

            // Raise event
            OnMessage?.Invoke(msg);
        }

        public virtual void OnPeerConnected(NetPeer peer)
        {
            //
            Logger.Log("[S] Player connected: " + peer.EndPoint);

            OnClientConnected?.Invoke();
        }

        public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.Log("[S] Player disconnected: " + disconnectInfo.Reason);

            OnClientDisconnected?.Invoke();
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Logger.Log("[S] NetworkError: " + socketError);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Logger.Log("[S] Player unconnected: " + remoteEndPoint + " (" + messageType + ")");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            peer.Tag = latency;
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Accept();
        }

        #endregion
    }
}
