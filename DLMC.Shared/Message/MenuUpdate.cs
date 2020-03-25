using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLMC.Shared.Message
{
    [MessageId(MessageId.MenuUpdate)]
    public class MenuUpdate : MessageBase
    {
        public override MessageId Id => MessageId.MenuUpdate;
        public override DeliveryMethod TransportType => DeliveryMethod.Unreliable;
        
        public byte StartMenuSel;

        public byte WeaponMenuSubmenuIndex;
        public byte WeaponMenuSubmenuSel;
        public byte WeaponMenuWeaponSel;
        public byte WeaponMenuOmegaSel;
        public byte WeaponMenuAlphaSel;
        
        public byte SkillsMenuSel;
        public byte ChallengeMenuSel;
        public byte ChangeChallengeDialogSel;

        public byte PlanetMenuSel;
        public byte ChangePlanetDialogSel;
        public byte PlanetChallengeMenuSel;

        public byte SaveMenuSel;
        public byte DifficultySel;

        public override void Deserialize(NetDataReader reader)
        {
            // Deserialize base
            base.Deserialize(reader);

            // Read class
            StartMenuSel = reader.GetByte();

            WeaponMenuSubmenuIndex = reader.GetByte();
            WeaponMenuSubmenuSel = reader.GetByte();
            WeaponMenuWeaponSel = reader.GetByte();
            WeaponMenuOmegaSel = reader.GetByte();
            WeaponMenuAlphaSel = reader.GetByte();
            
            SkillsMenuSel = reader.GetByte();
            ChallengeMenuSel = reader.GetByte();
            ChangeChallengeDialogSel = reader.GetByte();

            PlanetMenuSel = reader.GetByte();
            ChangePlanetDialogSel = reader.GetByte();
            PlanetChallengeMenuSel = reader.GetByte();

            SaveMenuSel = reader.GetByte();
            DifficultySel = reader.GetByte();
        }

        public override void Serialize(NetDataWriter writer)
        {
            // Serialize base
            base.Serialize(writer);

            // Serialize class
            writer.Put(StartMenuSel);

            writer.Put(WeaponMenuSubmenuIndex);
            writer.Put(WeaponMenuSubmenuSel);
            writer.Put(WeaponMenuWeaponSel);
            writer.Put(WeaponMenuOmegaSel);
            writer.Put(WeaponMenuAlphaSel);

            writer.Put(SkillsMenuSel);
            writer.Put(ChallengeMenuSel);
            writer.Put(ChangeChallengeDialogSel);

            writer.Put(PlanetMenuSel);
            writer.Put(ChangePlanetDialogSel);
            writer.Put(PlanetChallengeMenuSel);

            writer.Put(SaveMenuSel);
            writer.Put(DifficultySel);
        }

        public new string ToString()
        {
            return String.Format(
                "StartMenuSel:{0}\n" +
                "WeaponMenuSubmenuIndex:{1}\n" +
                "WeaponMenuSubmenuSel:{2}\n" +
                "WeaponMenuWeaponSel:{3}\n" +
                "WeaponMenuOmegaSel:{4}\n" +
                "WeaponMenuAlphaSel:{5}\n" +
                "SkillsMenuSel:{6}\n" +
                "ChallengeMenuSel:{7}\n" +
                "ChangeChallengeDialogSel:{8}\n" +
                "PlanetMenuSel:{9}\n" +
                "ChangePlanetDialogSel:{10}\n" +
                "PlanetChallengeMenuSel:{11}\n" +
                "SaveMenuSel:{12}\n" +
                "DifficultySel:{13}",
                StartMenuSel,
                WeaponMenuSubmenuIndex,
                WeaponMenuSubmenuSel,
                WeaponMenuWeaponSel,
                WeaponMenuOmegaSel,
                WeaponMenuAlphaSel,
                SkillsMenuSel,
                ChallengeMenuSel,
                ChangeChallengeDialogSel,
                PlanetMenuSel,
                ChangePlanetDialogSel,
                PlanetChallengeMenuSel,
                SaveMenuSel,
                DifficultySel
                );
        }
    }
}
