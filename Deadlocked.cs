using DLMC.Launcher.MathUtils;
using DLMC.Launcher.Memory;
using DLMC.Shared.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMC.Launcher
{
    public enum MapId : byte
    {
        MainMenu = 0,
        Battledome = 1,
        Catacrom = 2,
        Sarathos = 4,
        DarkCathedral = 5,
        Shaar = 6,
        Valix = 7,
        MiningFacility = 8,
        Torval = 10,
        Tempus = 11,
        Maraxus = 13,
        GhostStation = 14
    }

    public static class Deadlocked
    {
        // Our pad hook reads this address for the pad struct
        // If we enter the player 1's pad pointer, then the hook will replace player 1's inputs
        // with the data stored in the overwrite buffer
        public static readonly IntPtr PAD_OVERWRITE_POINTER = (IntPtr)0x000F00F0;

        // Contents of the pad overwrite buffer
        public static readonly IntPtr PAD_OVERWRITE_BUFFER = (IntPtr)0x000F0100;

        // Start of pad struct
        public static readonly IntPtr PLAYER_1_PAD = (IntPtr)0x001EE600;
        public static readonly IntPtr PLAYER_2_PAD = (IntPtr)0x001EFD00;

        // Contains an array of pointers to each player struct by player id
        // TODO: find pointers for each level
        public static readonly IntPtr PLAYER_STRUCT_PTR_BATTLEDOME = (IntPtr)0x003660F8;
        public static readonly IntPtr PLAYER_STRUCT_PTR_CATACROM = (IntPtr)0x00362C78;
        public static readonly IntPtr PLAYER_STRUCT_PTR_SARATHOS = (IntPtr)0x00362BF8;
        public static readonly IntPtr PLAYER_STRUCT_PTR_DARKCATHEDRAL = (IntPtr)0x00;
        public static readonly IntPtr PLAYER_STRUCT_PTR_SHAAR = (IntPtr)0x00;
        public static readonly IntPtr PLAYER_STRUCT_PTR_VALIX = (IntPtr)0x00;
        public static readonly IntPtr PLAYER_STRUCT_PTR_MININGFACILITY = (IntPtr)0x00;
        public static readonly IntPtr PLAYER_STRUCT_PTR_TORVAL = (IntPtr)0x00;
        public static readonly IntPtr PLAYER_STRUCT_PTR_TEMPUS = (IntPtr)0x00;
        public static readonly IntPtr PLAYER_STRUCT_PTR_MARAXUS = (IntPtr)0x00;
        public static readonly IntPtr PLAYER_STRUCT_PTR_GHOSTSTATION = (IntPtr)0x00;

        // Current mission id
        public static readonly IntPtr MISSION_ID = (IntPtr)0x001711A8;

        // Current map id
        public static readonly IntPtr MAP_ID = (IntPtr)0x001CEBF8;


        #region Map Id

        public static MapId GetMapId()
        {
            byte mapId = PCSX2.Read<byte>(MAP_ID);

            // Convert splitscreen map id to regular map id
            if (mapId >= 21 && mapId <= 34)
                return (MapId)(mapId - 20);

            return (MapId)mapId;
        }

        #endregion


        #region Pad

        public static IntPtr GetPadPointer(int player)
        {
            return player == 0 ? PLAYER_1_PAD : PLAYER_2_PAD;
        }

        #endregion

        #region Pad Update

        public static void Read(this PadUpdate pad, IntPtr address)
        {
            pad.ButtonMask = PCSX2.Read<ushort>(address + 2);
            pad.LeftAnalog = PCSX2.Read<ushort>(address + 4);
            pad.RightAnalog = PCSX2.Read<ushort>(address + 6);
        }

        public static void Write(this PadUpdate pad, IntPtr address)
        {
            PCSX2.Write(address + 0, (ushort)0x7900);
            PCSX2.Write(address + 2, pad.ButtonMask);
            PCSX2.Write(address + 4, pad.LeftAnalog);
            PCSX2.Write(address + 6, pad.RightAnalog);
        }

        #endregion


        #region Player

        public static IntPtr GetPlayerStructPointer(MapId map, int playerIndex)
        {
            int offset = playerIndex * 4;
            switch (map)
            {
                case MapId.Battledome: return (IntPtr)PCSX2.Read<int>(PLAYER_STRUCT_PTR_BATTLEDOME + offset);
                case MapId.Catacrom: return (IntPtr)PCSX2.Read<int>(PLAYER_STRUCT_PTR_CATACROM + offset);
                case MapId.Sarathos: return (IntPtr)PCSX2.Read<int>(PLAYER_STRUCT_PTR_SARATHOS + offset);
                case MapId.DarkCathedral: return (IntPtr)PCSX2.Read<int>(PLAYER_STRUCT_PTR_DARKCATHEDRAL + offset);
                case MapId.Shaar: return (IntPtr)PCSX2.Read<int>(PLAYER_STRUCT_PTR_SHAAR + offset);
                case MapId.Valix: return (IntPtr)PCSX2.Read<int>(PLAYER_STRUCT_PTR_VALIX + offset);
                case MapId.MiningFacility: return (IntPtr)PCSX2.Read<int>(PLAYER_STRUCT_PTR_MININGFACILITY + offset);
                case MapId.Torval: return (IntPtr)PCSX2.Read<int>(PLAYER_STRUCT_PTR_TORVAL + offset);
                case MapId.Tempus: return (IntPtr)PCSX2.Read<int>(PLAYER_STRUCT_PTR_TEMPUS + offset);
                case MapId.Maraxus: return (IntPtr)PCSX2.Read<int>(PLAYER_STRUCT_PTR_MARAXUS + offset);
                case MapId.GhostStation: return (IntPtr)PCSX2.Read<int>(PLAYER_STRUCT_PTR_GHOSTSTATION + offset);
                default: return IntPtr.Zero;
            }
        }

        #endregion
        
        #region Player Update

        public static void Read(this PlayerUpdate player, IntPtr playerStruct)
        {
            if (playerStruct == IntPtr.Zero)
                return;

            byte[] position = PCSX2.Read(playerStruct + 0xA0, 12);
            byte[] rotation = PCSX2.Read(playerStruct + 0xB8, 4);
            byte[] yaw = PCSX2.Read(playerStruct + 0x19B0, 4);
            byte[] pitch = PCSX2.Read(playerStruct + 0x19D0, 4);

            player.PositionX = BitConverter.ToSingle(position, 0);
            player.PositionY = BitConverter.ToSingle(position, 4);
            player.PositionZ = BitConverter.ToSingle(position, 8);
            player.Rotation = BitConverter.ToSingle(rotation, 0);
            player.CameraYaw = BitConverter.ToSingle(yaw, 0);
            player.CameraPitch = BitConverter.ToSingle(pitch, 0);
        }

        public static void Write(this PlayerUpdate player, IntPtr playerStruct)
        {
            if (playerStruct == IntPtr.Zero)
                return;

            PCSX2.Write(playerStruct + 0xA0, player.PositionX);
            PCSX2.Write(playerStruct + 0xA4, player.PositionY);
            PCSX2.Write(playerStruct + 0xA8, player.PositionZ);
            PCSX2.Write(playerStruct + 0xB8, player.Rotation);
            PCSX2.Write(playerStruct + 0x19B0, player.CameraYaw);
            PCSX2.Write(playerStruct + 0x19D0, player.CameraPitch);
        }

        public static void Lerp(this PlayerUpdate player, IntPtr playerStruct)
        {
            PlayerUpdate current = new PlayerUpdate();
            current.Read(playerStruct);

            float pLerpTime = 1 - (float)Math.Exp(-Config.PositionSharpness * Shared.LogicTimer.FixedDelta);
            float rLerpTime = 1f; // 1 - (float)Math.Exp(-Config.PositionSharpness * Shared.LogicTimer.FixedDelta);

            // Lerp or teleport
            if (Math.Abs(current.PositionX - player.PositionX) > Config.MaxPositionDeltaBeforeTeleport)
                current.PositionX = player.PositionX;
            else
                current.PositionX = current.PositionX.Lerp(player.PositionX, pLerpTime);

            // Lerp or teleport
            if (Math.Abs(current.PositionY - player.PositionY) > Config.MaxPositionDeltaBeforeTeleport)
                current.PositionY = player.PositionY;
            else
                current.PositionY = current.PositionY.Lerp(player.PositionY, pLerpTime);

            // Lerp or teleport
            if (Math.Abs(current.PositionZ - player.PositionZ) > Config.MaxPositionDeltaBeforeTeleport)
                current.PositionZ = player.PositionZ;
            else
                current.PositionZ = current.PositionZ.Lerp(player.PositionX, pLerpTime);

            // Lerp rotation
            current.Rotation = current.Rotation.Lerp(player.Rotation, rLerpTime);

            // Lerp camera rotation
            current.CameraPitch = current.CameraPitch.Lerp(player.CameraPitch, 1f);
            current.CameraYaw = current.CameraYaw.Lerp(player.CameraYaw, 1f);
        }

        #endregion

    }
}
