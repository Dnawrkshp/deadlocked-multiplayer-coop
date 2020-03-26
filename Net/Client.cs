using DLMC.Client;
using DLMC.Launcher.Memory;
using DLMC.Shared;
using DLMC.Shared.Message;
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

        float _timeSincePlanetSelChanged = 0f;
        float _timeSinceWeaponMenuSelChanged = 0f;

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

        protected override void OnPadUpdate(PadUpdate pad)
        {
            base.OnPadUpdate(pad);
        }

        protected override void OnMenuUpdate(MenuUpdate menu)
        {
            bool forceRefresh = false;

            //
            base.OnMenuUpdate(menu);

            // Planet selection change
            if (menu.PlanetMenuSel != _cachedLocalMenuUpdate.PlanetMenuSel)
                _timeSincePlanetSelChanged = _time;

            // Weapon menu selection change
            if (menu.WeaponMenuWeaponSel != _cachedLocalMenuUpdate.WeaponMenuWeaponSel)
                _timeSinceWeaponMenuSelChanged = _time;

            // 
            forceRefresh = _time - _timeSincePlanetSelChanged < Config.MenuForceRefreshPeriod ||
                            _time - _timeSinceWeaponMenuSelChanged < Config.MenuForceRefreshPeriod;

            menu.Write(forceRefresh);
        }

        protected override void OnProgressUpdate(ProgressUpdate progress)
        {
            // 
            base.OnProgressUpdate(progress);

            // 
            progress.Write();
        }

        protected override void OnEquipmentUpdate(EquipmentUpdate equipment)
        {
            //
            base.OnEquipmentUpdate(equipment);

            //
            equipment.Write();
        }
    }
}
