using DLMC.Launcher.Memory;
using DLMC.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMC.Launcher.Net
{
    public class Server : BasePeer
    {
        HostOptions _options = null;

        public Server(HostOptions options)
        {
            _options = options;

            ServerLogic server = new ServerLogic();
            _logic = server;

            server.OnClientConnected += OnClientConnected;
            server.OnClientDisconnected += OnClientDisconnected;
            server.OnTick += OnTick;
            server.OnMessage += OnMessage;

            server.Start(_options.Port);
        }

        protected override void OnClientConnected()
        {
            base.OnClientConnected();

            if (!PCSX2.HasInstance())
                PCSX2.Start(_options.PCSX2Path, _options.DLPath);
        }
    }
}
