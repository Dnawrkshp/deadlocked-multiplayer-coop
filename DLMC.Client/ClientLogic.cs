using DLMC.Shared;
using DLMC.Shared.Message;
using DLMC.Shared.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DLMC.Client
{
    public class ClientLogic : BaseLogic, INetEventListener
    {
        // 
        private int _ping;
        public int Ping => _ping;

        // 
        private NetManager _netManager;
        public override NetManager NetMan => _netManager;

        // 
        private NetPeer _server = null;
        public override NetPeer Server => _server;

        // 
        public override bool IsClient => true;
        public override byte LocalPlayerId => 1;
        public override byte RemotePlayerId => 0;

        // 
        private LogicTimer _logicTimer;
        public override LogicTimer Logic => _logicTimer;

        public event Action<MessageBase> OnMessage;
        public event Action OnClientConnected;
        public event Action OnClientDisconnected;
        public event Action OnTick;


        private NetDataWriter _writer;
        private MessageCache _cachedMessages;

        public ClientLogic()
        {
            Singleton = this;
            
            _cachedMessages = new MessageCache();
            _logicTimer = new LogicTimer(OnLogicUpdate);
            _writer = new NetDataWriter();
            _netManager = new NetManager(this)
            {
                AutoRecycle = true,
                SimulateLatency = false,
                SimulationMinLatency = 80,
                SimulationMaxLatency = 110,
                DisconnectTimeout = 5000
            };
            _netManager.Start();
        }

        ~ClientLogic()
        {
            Stop();
        }

        public void Connect(string hostname, int port)
        {
            _netManager.Connect(hostname, port, "");
        }

        public override void Stop()
        {
            if (_netManager != null && _netManager.IsRunning)
                _netManager.Stop();

            if (_logicTimer != null)
                _logicTimer.Stop();
        }

        public override void Update()
        {
            _netManager.PollEvents();
            _logicTimer.Update();
        }

        private void OnLogicUpdate()
        {
            OnTick?.Invoke();
        }


        #region Net Events

        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            Logger.Log("[C] Connected to server: " + peer.EndPoint + " (" + peer.Id + ")");
            _server = peer;

            // Tell listeners we connected
            OnClientConnected?.Invoke();

            _logicTimer.Start();
        }

        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _server = null;
            _logicTimer.Stop();
            Logger.Log("[C] Disconnected from server: " + disconnectInfo.Reason);

            // Tell listeners we disconnected
            OnClientDisconnected?.Invoke();
        }

        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Logger.Log("[C] NetworkError: " + socketError);
        }

        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            MessageId messageId = reader.PeekEnum<MessageId>();
            if (!messageId.IsValid())
                return;

            var msg = _cachedMessages.Deserialize(messageId, reader);
            if (!msg.IsValid())
                return;

            // Raise message
            OnMessage?.Invoke(msg);
        }

        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Logger.Log("[C] Disconnected from remote end point (server): " + messageType);

            // Tell listeners we disconnected
            OnClientDisconnected?.Invoke();
        }

        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            _ping = latency;
        }

        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
            request.Reject();
        }

        #endregion

    }
}
