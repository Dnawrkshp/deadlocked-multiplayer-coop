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
        public static readonly IntPtr LEVEL_PTR = (IntPtr)0x001EEB70;

        // Offset from level pointer to the array of player struct pointers
        public static readonly int PLAYER_STRUCT_ARRAY_OFFSET = -0x5E54;

        // Static address of player equipped weapons
        public static readonly IntPtr PLAYER_1_EQUIPPED_LIST = (IntPtr)0x0020C690;
        public static readonly IntPtr PLAYER_2_EQUIPPED_LIST = (IntPtr)0x0020C69C;

        // Static address of player equipped item
        // This doesn't change the equipped item. It is just for reading.
        public static readonly IntPtr PLAYER_1_EQUIPPED_ITEM = (IntPtr)0x002206D0;
        public static readonly IntPtr PLAYER_2_EQUIPPED_ITEM = (IntPtr)0x002206D4;

        // Menu pointers
        public static readonly IntPtr MENU_PTR_MAIN = (IntPtr)0x00307254;
        public static readonly IntPtr MENU_PTR_BATTLEDOME = (IntPtr)0x0032F294;
        public static readonly IntPtr MENU_PTR_CATACROM = (IntPtr)0x0032BE14;
        public static readonly IntPtr MENU_PTR_SARATHOS = (IntPtr)0x00;
        public static readonly IntPtr MENU_PTR_DARKCATHEDRAL = (IntPtr)0x00;
        public static readonly IntPtr MENU_PTR_SHAAR = (IntPtr)0x00;
        public static readonly IntPtr MENU_PTR_VALIX = (IntPtr)0x00;
        public static readonly IntPtr MENU_PTR_MININGFACILITY = (IntPtr)0x00;
        public static readonly IntPtr MENU_PTR_TORVAL = (IntPtr)0x00;
        public static readonly IntPtr MENU_PTR_TEMPUS = (IntPtr)0x00;
        public static readonly IntPtr MENU_PTR_MARAXUS = (IntPtr)0x00;
        public static readonly IntPtr MENU_PTR_GHOSTSTATION = (IntPtr)0x00;

        // Menu offsets from respective menu pointers
        public static readonly int CHANGECHALLENGEDIALOG_SEL_OFFSET = -0x1CC;
        public static readonly int CHANGEPLANETDIALOG_SEL_OFFSET = -0x20;

        public static readonly int MENU_OPEN_OFFSET = -0x494;
        public static readonly int MENU_ID_OFFSET = -0x490;

        public static readonly int STARTMENU_SEL_OFFSET = 0x2C;

        public static readonly int WEAPONMENU_SUBMENU_INDEX_OFFSET = 0x45A4;
        public static readonly int WEAPONMENU_SUBMENU_REFRESH_OFFSET = 0x45AC;
        public static readonly int WEAPONMENU_WEAPON_SEL_OFFSET = 0x45B0;
        public static readonly int WEAPONMENU_SUBMENU_SEL_OFFSET = 0x45B4;
        public static readonly int WEAPONMENU_OMEGA_SEL_OFFSET = 0x45B8;
        public static readonly int WEAPONMENU_ALPHA_SEL_OFFSET = 0x45BC;

        public static readonly int SKILLSMENU_SEL_OFFSET = 0x95B0;
        public static readonly int CHALLENGEMENU_SEL_OFFSET = 0x528C;

        public static readonly int PLANETMENU_SEL_REFRESH_OFFSET = 0x4108;
        public static readonly int PLANETMENU_SEL_OFFSET = 0x4344;
        public static readonly int PLANETMENU_CHALLENGE_SEL_OFFSET = 0x39B8;

        public static readonly int SAVEMENU_SEL_OFFSET = 0x7788;
        public static readonly int DIFFICULTYMENU_SEL_OFFSET = 0x14460;

        // Current mission id
        public static readonly IntPtr MISSION_ID = (IntPtr)0x001711A8;

        // Current map id
        public static readonly IntPtr MAP_ID = (IntPtr)0x001CEBF8;

        // Skill points
        public static readonly IntPtr GAMESTATE_SKILLPOINTS = (IntPtr)0x00171BA8;
        public static readonly int GAMESTATE_SKILLPOINTS_SIZE = 0x40;
        
        public static readonly IntPtr GAMESTATE_ALPHA_OMEGA_MODS = (IntPtr)0x001D3FB0;
        public static readonly int GAMESTATE_ALPHA_OMEGA_MODS_SIZE = 0xC;

        public static readonly IntPtr GAMESTATE_EQUIPMENT = (IntPtr)0x001D4A48;
        public static readonly int GAMESTATE_EQUIPMENT_SIZE = 0x44 * 14;

        public static readonly IntPtr GAMESTATE_BOLTS = (IntPtr)0x00171B40;
        public static readonly int GAMESTATE_BOLTS_EXP_POINTS_SIZE = 0x100;

        public static readonly IntPtr STATS_WEAPONS = (IntPtr)0x0020B53C;
        public static readonly int STATS_WEAPONS_SIZE = 0x40;

        public static readonly IntPtr LEVEL_PROGRESS_START = (IntPtr)0x001F87F0;
        public static readonly int LEVEL_PROGRESS_SIZE = 0x300 * 14;


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

        public static IntPtr GetPlayerStructPointer(int playerIndex)
        {
            return (IntPtr)PCSX2.Read<int>((IntPtr)PCSX2.Read<int>(LEVEL_PTR) + PLAYER_STRUCT_ARRAY_OFFSET + (playerIndex * 4));
        }

        #endregion

        #region Player Update

        public static void Read(this PlayerUpdate player, int playerIndex)
        {
            var playerStruct = GetPlayerStructPointer(playerIndex);
            if (playerStruct == IntPtr.Zero)
                return;

            byte[] position = PCSX2.Read(playerStruct + 0xA0, 12);
            byte[] rotation = PCSX2.Read(playerStruct + 0xB8, 4);
            byte[] yaw = PCSX2.Read(playerStruct + 0x19B0, 4);
            byte[] pitch = PCSX2.Read(playerStruct + 0x19D0, 4);
            byte[] equipped = PCSX2.Read(playerIndex == 0 ? PLAYER_1_EQUIPPED_LIST : PLAYER_2_EQUIPPED_LIST, 12);

            player.PositionX = BitConverter.ToSingle(position, 0);
            player.PositionY = BitConverter.ToSingle(position, 4);
            player.PositionZ = BitConverter.ToSingle(position, 8);
            player.Rotation = BitConverter.ToSingle(rotation, 0);
            player.CameraYaw = BitConverter.ToSingle(yaw, 0);
            player.CameraPitch = BitConverter.ToSingle(pitch, 0);
            player.Health = PCSX2.Read<float>(playerStruct + 0x2E20);
            player.EquippedItem = PCSX2.Read<byte>(playerStruct + 0x268C);
            player.Primary = equipped[0];
            player.Secondary = equipped[4];
            player.Tertiary = equipped[8];
        }

        public static void Write(this PlayerUpdate player, int playerIndex)
        {
            var playerStruct = GetPlayerStructPointer(playerIndex);
            var playerEquippedAddress = playerIndex == 0 ? PLAYER_1_EQUIPPED_LIST : PLAYER_2_EQUIPPED_LIST;
            if (playerStruct == IntPtr.Zero)
                return;

            PCSX2.Write(playerStruct + 0xA0, player.PositionX);
            PCSX2.Write(playerStruct + 0xA4, player.PositionY);
            PCSX2.Write(playerStruct + 0xA8, player.PositionZ);
            PCSX2.Write(playerStruct + 0xB8, player.Rotation);
            PCSX2.Write(playerStruct + 0x19B0, player.CameraYaw);
            PCSX2.Write(playerStruct + 0x19D0, player.CameraPitch);

            PCSX2.Write(playerStruct + 0x2E20, player.Health);

            PCSX2.Write(playerStruct + 0x24B0, player.EquippedItem);
            PCSX2.Write(playerStruct + 0x268C, player.EquippedItem);

            PCSX2.Write(playerEquippedAddress, player.Primary);
            PCSX2.Write(playerEquippedAddress+4, player.Secondary);
            PCSX2.Write(playerEquippedAddress+8, player.Tertiary);
        }

        public static void Lerp(this PlayerUpdate player, int playerIndex)
        {
            PlayerUpdate current = new PlayerUpdate();
            current.Read(playerIndex);

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
                current.PositionZ = current.PositionZ.Lerp(player.PositionZ, pLerpTime);

            // Lerp rotation
            current.Rotation = current.Rotation.Lerp(player.Rotation, rLerpTime);

            // Lerp camera rotation
            current.CameraPitch = current.CameraPitch.Lerp(player.CameraPitch, 1f);
            current.CameraYaw = current.CameraYaw.Lerp(player.CameraYaw, 1f);

            // 
            current.Health = player.Health;

            // 
            current.EquippedItem = player.EquippedItem;
            current.Primary = player.Primary;
            current.Secondary = player.Secondary;
            current.Tertiary = player.Tertiary;

            // 
            current.Write(playerIndex);
        }

        #endregion


        #region Menu

        public static IntPtr GetMenuStart(MapId map)
        {
            switch (map)
            {
                case MapId.Battledome: return MENU_PTR_BATTLEDOME;
                case MapId.Catacrom: return MENU_PTR_CATACROM;
                case MapId.Sarathos: return MENU_PTR_SARATHOS;
                case MapId.DarkCathedral: return MENU_PTR_DARKCATHEDRAL;
                case MapId.Shaar: return MENU_PTR_SHAAR;
                case MapId.Valix: return MENU_PTR_VALIX;
                case MapId.MiningFacility: return MENU_PTR_MININGFACILITY;
                case MapId.Torval: return MENU_PTR_TORVAL;
                case MapId.Tempus: return MENU_PTR_TEMPUS;
                case MapId.Maraxus: return MENU_PTR_MARAXUS;
                case MapId.GhostStation: return MENU_PTR_GHOSTSTATION;
                default: return MENU_PTR_MAIN;
            }
        }

        public static bool IsInMenu()
        {
            return PCSX2.Read<byte>(GetMenuStart(GetMapId()) + MENU_OPEN_OFFSET) != 0;
        }

        private static bool HandleMenuChange(IntPtr offset, byte newValue, bool writeRefresh = false)
        {
            byte current = PCSX2.Read<byte>(offset);
            if (current != newValue)
            {
                PCSX2.Write(offset, newValue);

                // refresh
                if (writeRefresh)
                    PCSX2.Write(offset + 4, 5);

                return true;
            }

            return false;
        }

        #endregion

        #region Menu Update

        public static void Read(this MenuUpdate player)
        {
            IntPtr menuStart = GetMenuStart(GetMapId());
            if (menuStart == IntPtr.Zero)
                return;

            player.StartMenuSel = PCSX2.Read<byte>(menuStart + STARTMENU_SEL_OFFSET);

            player.WeaponMenuSubmenuIndex = PCSX2.Read<byte>(menuStart + WEAPONMENU_SUBMENU_INDEX_OFFSET);
            player.WeaponMenuSubmenuSel = PCSX2.Read<byte>(menuStart + WEAPONMENU_SUBMENU_SEL_OFFSET);
            player.WeaponMenuWeaponSel = PCSX2.Read<byte>(menuStart + WEAPONMENU_WEAPON_SEL_OFFSET);
            player.WeaponMenuOmegaSel = PCSX2.Read<byte>(menuStart + WEAPONMENU_OMEGA_SEL_OFFSET);
            player.WeaponMenuAlphaSel = PCSX2.Read<byte>(menuStart + WEAPONMENU_ALPHA_SEL_OFFSET);

            player.SkillsMenuSel = PCSX2.Read<byte>(menuStart + SKILLSMENU_SEL_OFFSET);
            player.ChallengeMenuSel = PCSX2.Read<byte>(menuStart + CHALLENGEMENU_SEL_OFFSET);
            player.ChangeChallengeDialogSel = PCSX2.Read<byte>(menuStart + CHANGECHALLENGEDIALOG_SEL_OFFSET);

            player.PlanetMenuSel = PCSX2.Read<byte>(menuStart + PLANETMENU_SEL_OFFSET);
            player.ChangePlanetDialogSel = PCSX2.Read<byte>(menuStart + CHANGEPLANETDIALOG_SEL_OFFSET);
            player.PlanetChallengeMenuSel = PCSX2.Read<byte>(menuStart + PLANETMENU_CHALLENGE_SEL_OFFSET);

            player.SaveMenuSel = PCSX2.Read<byte>(menuStart + SAVEMENU_SEL_OFFSET);
            player.DifficultySel = PCSX2.Read<byte>(menuStart + DIFFICULTYMENU_SEL_OFFSET);
        }

        public static void Write(this MenuUpdate player, bool forceRefresh = false)
        {
            IntPtr menuStart = GetMenuStart(GetMapId());
            if (menuStart == IntPtr.Zero)
                return;

            // Main select
            if (player.StartMenuSel != 0xFF)
                HandleMenuChange(menuStart + STARTMENU_SEL_OFFSET, player.StartMenuSel);

            // Save game select
            HandleMenuChange(menuStart + SAVEMENU_SEL_OFFSET, player.SaveMenuSel, true);

            // Difficulty select
            HandleMenuChange(menuStart + DIFFICULTYMENU_SEL_OFFSET, player.DifficultySel, true);

            // Planet select with custom refresh
            byte planetRefresh = PCSX2.Read<byte>(menuStart + PLANETMENU_SEL_REFRESH_OFFSET);
            if (HandleMenuChange(menuStart + PLANETMENU_SEL_OFFSET, player.PlanetMenuSel, true) || (planetRefresh == 0 && forceRefresh))
                PCSX2.Write(menuStart + PLANETMENU_SEL_REFRESH_OFFSET, 5);

            // Planet challenge select
            HandleMenuChange(menuStart + PLANETMENU_CHALLENGE_SEL_OFFSET, player.PlanetChallengeMenuSel, true);
            
            // Weapon select
            byte weaponRefresh = PCSX2.Read<byte>(menuStart + WEAPONMENU_SUBMENU_REFRESH_OFFSET);
            if (HandleMenuChange(menuStart + WEAPONMENU_WEAPON_SEL_OFFSET, player.WeaponMenuWeaponSel) || (weaponRefresh == 0x80 && forceRefresh))
                PCSX2.Write(menuStart + WEAPONMENU_SUBMENU_REFRESH_OFFSET, 0x84);

            // Mods
            if (HandleMenuChange(menuStart + WEAPONMENU_OMEGA_SEL_OFFSET, player.WeaponMenuOmegaSel))
                PCSX2.Write(menuStart + WEAPONMENU_SUBMENU_REFRESH_OFFSET, 0x88);

            if (HandleMenuChange(menuStart + WEAPONMENU_ALPHA_SEL_OFFSET, player.WeaponMenuAlphaSel))
                PCSX2.Write(menuStart + WEAPONMENU_SUBMENU_REFRESH_OFFSET, 0x88);

            // Weapon submenu select
            if (HandleMenuChange(menuStart + WEAPONMENU_SUBMENU_SEL_OFFSET, player.WeaponMenuSubmenuSel))
                PCSX2.Write(menuStart + WEAPONMENU_SUBMENU_REFRESH_OFFSET, 0x88);
            
            // Submenu index
            if (player.WeaponMenuSubmenuIndex > 1 && player.WeaponMenuSubmenuIndex < 4)
                HandleMenuChange(menuStart + WEAPONMENU_SUBMENU_INDEX_OFFSET, player.WeaponMenuSubmenuIndex);
            
            // 
            HandleMenuChange(menuStart + SKILLSMENU_SEL_OFFSET, player.SkillsMenuSel, true);
            HandleMenuChange(menuStart + CHALLENGEMENU_SEL_OFFSET, player.ChallengeMenuSel, true);

            // Dialog
            HandleMenuChange(menuStart + CHANGECHALLENGEDIALOG_SEL_OFFSET, player.ChangeChallengeDialogSel);
            HandleMenuChange(menuStart + CHANGEPLANETDIALOG_SEL_OFFSET, player.ChangePlanetDialogSel);
        }

        #endregion


        #region Progress Update

        public static void Read(this ProgressUpdate progress)
        {
            if (progress.LevelProgress == null || progress.LevelProgress.Length != LEVEL_PROGRESS_SIZE)
                progress.LevelProgress = new byte[LEVEL_PROGRESS_SIZE];

            if (progress.BoltsExpPoints == null || progress.BoltsExpPoints.Length != GAMESTATE_BOLTS_EXP_POINTS_SIZE)
                progress.BoltsExpPoints = new byte[GAMESTATE_BOLTS_EXP_POINTS_SIZE];

            if (progress.Skillpoints == null || progress.Skillpoints.Length != GAMESTATE_SKILLPOINTS_SIZE)
                progress.Skillpoints = new byte[GAMESTATE_SKILLPOINTS_SIZE];

            if (progress.AlphaOmegaMods == null || progress.AlphaOmegaMods.Length != GAMESTATE_ALPHA_OMEGA_MODS_SIZE)
                progress.AlphaOmegaMods = new byte[GAMESTATE_ALPHA_OMEGA_MODS_SIZE];

            // Read
            PCSX2.Read(LEVEL_PROGRESS_START, progress.LevelProgress, LEVEL_PROGRESS_SIZE);
            PCSX2.Read(GAMESTATE_BOLTS, progress.BoltsExpPoints, GAMESTATE_BOLTS_EXP_POINTS_SIZE);
            PCSX2.Read(GAMESTATE_SKILLPOINTS, progress.Skillpoints, GAMESTATE_SKILLPOINTS_SIZE);
            PCSX2.Read(GAMESTATE_ALPHA_OMEGA_MODS, progress.AlphaOmegaMods, GAMESTATE_ALPHA_OMEGA_MODS_SIZE);
        }

        public static void Write(this ProgressUpdate progress)
        {
            if (progress.LevelProgress != null)
                PCSX2.Write(LEVEL_PROGRESS_START, progress.LevelProgress);

            if (progress.BoltsExpPoints != null)
                PCSX2.Write(GAMESTATE_BOLTS, progress.BoltsExpPoints);

            if (progress.Skillpoints != null)
                PCSX2.Write(GAMESTATE_SKILLPOINTS, progress.Skillpoints);

            if (progress.AlphaOmegaMods != null)
                PCSX2.Write(GAMESTATE_ALPHA_OMEGA_MODS, progress.AlphaOmegaMods);
        }

        #endregion


        #region Equipment Update

        public static void Read(this EquipmentUpdate equipment)
        {
            if (equipment.Equipment == null || equipment.Equipment.Length != GAMESTATE_EQUIPMENT_SIZE)
                equipment.Equipment = new byte[GAMESTATE_EQUIPMENT_SIZE];

            // Read
            PCSX2.Read(GAMESTATE_EQUIPMENT, equipment.Equipment, GAMESTATE_EQUIPMENT_SIZE);
        }

        public static void Write(this EquipmentUpdate equipment)
        {
            if (equipment.Equipment != null)
                PCSX2.Write(GAMESTATE_EQUIPMENT, equipment.Equipment);
        }

        #endregion

    }
}
