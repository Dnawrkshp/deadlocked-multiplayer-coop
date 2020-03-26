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

        protected float _time = 0f;
        protected float _timeLastForceWeaponChange = 0f;

        // Messages
        protected PadUpdate _cachedLocalPadUpdate = new PadUpdate();
        protected PlayerUpdate _cachedLocalPlayerUpdate = new PlayerUpdate();
        protected MenuUpdate _cachedLocalMenuUpdate = new MenuUpdate();
        protected ProgressUpdate _cachedLocalProgressUpdate = new ProgressUpdate();
        protected EquipmentUpdate _cachedLocalEquipmentUpdate = new EquipmentUpdate();

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
            // increment time
            _time += LogicTimer.FixedDelta;

            if (IsConnected)
            {
                _mapId = Deadlocked.GetMapId();
                bool isInMenu = Deadlocked.IsInMenu();
                ushort missionId = PCSX2.Read<ushort>(Deadlocked.MISSION_ID);
                
                // Send pad
                _cachedLocalPadUpdate.Read(Deadlocked.GetPadPointer(_logic.LocalPlayerId));
                _logic.Send(_cachedLocalPadUpdate);

                if (missionId != 0 && missionId != 0xFFFF)
                {
                    // Send player
                    _cachedLocalPlayerUpdate.Read(_logic.LocalPlayerId);
                    _logic.Send(_cachedLocalPlayerUpdate);
                }

                // Send menu
                if (isInMenu || _mapId == MapId.MainMenu)
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
                case MessageId.ProgressUpdate:
                    {
                        OnProgressUpdate(message as ProgressUpdate);
                        break;
                    }
                case MessageId.EquipmentUpdate:
                    {
                        OnEquipmentUpdate(message as EquipmentUpdate);
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
            // Ignore if we're not in a mission yet
            ushort missionId = PCSX2.Read<ushort>(Deadlocked.MISSION_ID);
            if (missionId == 0 || missionId == 0xFFFF)
                return;

            // Get current equipped weapon
            byte equippedPrimary = PCSX2.Read<byte>(_logic.RemotePlayerId == 0 ? Deadlocked.PLAYER_1_EQUIPPED_ITEM : Deadlocked.PLAYER_2_EQUIPPED_ITEM);

            // Apply player update
            player.Lerp(_logic.RemotePlayerId);

            // If weapon changed remotely, tap R2
            if (equippedPrimary != player.EquippedItem)
            {
                if (_time - _timeLastForceWeaponChange > Config.ForceWeaponChangeInterval)
                {
                    // Set R2 if it isn't already set
                    ushort pad = PCSX2.Read<ushort>(Deadlocked.PAD_OVERWRITE_BUFFER + 2);
                    if ((pad & ~0xFDFF) != 0)
                    {
                        pad &= 0xFDFF;
                        PCSX2.Write(Deadlocked.PAD_OVERWRITE_BUFFER + 2, pad);
                    }

                    _timeLastForceWeaponChange = _time;
                }
            }
        }

        protected virtual void OnMenuUpdate(MenuUpdate menu)
        {

        }

        protected virtual void OnProgressUpdate(ProgressUpdate progress)
        {
            
        }

        protected virtual void OnEquipmentUpdate(EquipmentUpdate equipment)
        {

        }
    }
}
