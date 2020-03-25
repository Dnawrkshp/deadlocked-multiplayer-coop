using DLMC.Launcher.Memory;
using DLMC.Shared;
using DLMC.Shared.Message;
using DLMC.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMC.Launcher.Net
{
    public abstract class BasePeer
    {
        public bool IsConnected { get; private set; } = false;

        protected BaseLogic _logic = null;

        // State
        protected MapId _mapId = MapId.MainMenu;

        // Messages
        protected PadUpdate _cachedLocalPadUpdate = new PadUpdate();
        protected PlayerUpdate _cachedLocalPlayerUpdate = new PlayerUpdate();
        protected MenuUpdate _cachedLocalMenuUpdate = new MenuUpdate();

        public virtual void Update()
        {
            // Update
            _logic?.Update();
        }

        public void Stop()
        {
            _logic?.Stop();
            _logic = null;
        }

        protected virtual void OnClientConnected()
        {
            IsConnected = true;
        }

        protected virtual void OnClientDisconnected()
        {
            IsConnected = false;
        }

        protected virtual void OnTick()
        {
            if (IsConnected)
            {
                _mapId = Deadlocked.GetMapId();
                ushort missionId = PCSX2.Read<ushort>(Deadlocked.MISSION_ID);

                // Send pad
                _cachedLocalPadUpdate.Read(Deadlocked.GetPadPointer(_logic.LocalPlayerId));
                _logic.Send(_cachedLocalPadUpdate);

                if (_mapId != MapId.MainMenu)
                {
                    // Send player
                    _cachedLocalPlayerUpdate.Read(Deadlocked.GetPlayerStructPointer(_mapId, _logic.LocalPlayerId));
                    _logic.Send(_cachedLocalPlayerUpdate);
                }

                // Send menu
                if (Deadlocked.IsInMenu() || _mapId == MapId.MainMenu)
                {
                    _cachedLocalMenuUpdate.Read();
                    _logic.Send(_cachedLocalMenuUpdate);
                }
            }
        }

        protected virtual void OnMessage(MessageBase message)
        {
            switch (message.Id)
            {
                case MessageId.PadUpdate:
                    {
                        OnPadUpdate(message as PadUpdate);
                        break;
                    }
                case MessageId.PlayerUpdate:
                    {
                        OnPlayerUpdate(message as PlayerUpdate);
                        break;
                    }
                case MessageId.MenuUpdate:
                    {
                        OnMenuUpdate(message as MenuUpdate);
                        break;
                    }
            }
        }

        protected virtual void OnPadUpdate(PadUpdate pad)
        {
            // Write hook
            PCSX2.Write(Deadlocked.PAD_OVERWRITE_POINTER, (int)Deadlocked.GetPadPointer(_logic.RemotePlayerId));

            // Write to our buffer
            pad.Write(Deadlocked.PAD_OVERWRITE_BUFFER);
        }

        protected virtual void OnPlayerUpdate(PlayerUpdate player)
        {
            player.Lerp(Deadlocked.GetPlayerStructPointer(_mapId, _logic.RemotePlayerId));
        }

        protected virtual void OnMenuUpdate(MenuUpdate menu)
        {

        }
    }
}
