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
    public abstract class BasePeer
    {
        public bool IsConnected { get; private set; } = false;

        protected BaseLogic _logic = null;

        // State
        protected MapId _mapId = MapId.MainMenu;

        // Messages
        protected PadUpdate _cachedPadUpdate = new PadUpdate();
        protected PlayerUpdate _cachedPlayerUpdate = new PlayerUpdate();

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
                _cachedPadUpdate.Read(Deadlocked.GetPadPointer(_logic.LocalPlayerId));
                _logic.Send(_cachedPadUpdate);

                if (_mapId != MapId.MainMenu)
                {
                    // Send player
                    _cachedPlayerUpdate.Read(Deadlocked.GetPlayerStructPointer(_mapId, _logic.LocalPlayerId));
                    _logic.Send(_cachedPlayerUpdate);
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
    }
}
