using DLMC.Client;
using DLMC.Launcher.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMC.Launcher.Net
{
    public class Client : BasePeer
    {
        ConnectOptions _options = null;

        public Client(ConnectOptions options)
        {
            _options = options;

            ClientLogic client = new ClientLogic();
            _logic = client;

            client.OnClientConnected += OnClientConnected;
            client.OnClientDisconnected += OnClientDisconnected;
            client.OnTick += OnTick;
            client.OnMessage += OnMessage;

            client.Connect(_options.Hostname, _options.Port);
        }

        protected override void OnClientConnected()
        {
            base.OnClientConnected();

            if (!PCSX2.HasInstance())
                PCSX2.Start(_options.PCSX2Path, _options.DLPath);
        }
    }
}
