using DLMC.Launcher.Memory;
using DLMC.Server;
using DLMC.Shared.Message;
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

        private float _timeLastProgressUpdate = 0f;
        private float _timeLastEquipmentUpdate = 0f;

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

        protected override void OnTick()
        {
            // 
            base.OnTick();

            if (IsConnected && _mapId != MapId.MainMenu)
            {
                if (_time - _timeLastProgressUpdate > Config.SendProgressUpdateInterval)
                {
                    // Send progress
                    _cachedLocalProgressUpdate.Read();
                    _logic.Send(_cachedLocalProgressUpdate);
                    _timeLastProgressUpdate = _time;
                }

                if (_time - _timeLastEquipmentUpdate > Config.SendEquipmentUpdateInterval)
                {
                    // Send progress
                    _cachedLocalEquipmentUpdate.Read();
                    _logic.Send(_cachedLocalEquipmentUpdate);
                    _timeLastEquipmentUpdate = _time;
                }
            }
        }

        protected override void OnMenuUpdate(MenuUpdate menu)
        {
            base.OnMenuUpdate(menu);
        }
    }
}
