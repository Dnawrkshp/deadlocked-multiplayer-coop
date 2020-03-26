using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMC.Launcher
{
    public static class Config
    {
        /// <summary>
        /// How sharply the remote player will interpolate between the local position and the remote position.
        /// </summary>
        public static float PositionSharpness = 10f;

        /// <summary>
        /// How far before the remote player will teleport to their actual position.
        /// </summary>
        public static float MaxPositionDeltaBeforeTeleport = 5f;

        /// <summary>
        /// How many seconds after a menu selection change to force an update.
        /// This improves menu sychronization at a performance cost.
        /// </summary>
        public static float MenuForceRefreshPeriod = 0.1f;

        /// <summary>
        /// How many seconds before the host sends all level progress to the client to ensure they've made the same progress.
        /// </summary>
        public static float SendProgressUpdateInterval = 15f;

        /// <summary>
        /// How many seconds before the host sends all equipment levels/ammo/mods to the client.
        /// </summary>
        public static float SendEquipmentUpdateInterval = 5f;

        /// <summary>
        /// How many seconds before sending change weapon input to the client when its equipped item doesn't match the remote.
        /// </summary>
        public static float ForceWeaponChangeInterval = 0.1f;

    }
}
