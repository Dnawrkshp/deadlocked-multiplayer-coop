using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLMC.Shared.Message
{
    [MessageId(MessageId.ProgressUpdate)]
    public class ProgressUpdate : MessageBase
    {
        public override MessageId Id => MessageId.ProgressUpdate;
        public override DeliveryMethod TransportType => DeliveryMethod.ReliableUnordered;

        public byte[] LevelProgress = null;
        public byte[] BoltsExpPoints = null;
        public byte[] Skillpoints = null;
        public byte[] AlphaOmegaMods = null;

        public override void Deserialize(NetDataReader reader)
        {
            // Deserialize base
            base.Deserialize(reader);

            // Read class
            LevelProgress = reader.GetBytesWithLength();
            BoltsExpPoints = reader.GetBytesWithLength();
            Skillpoints = reader.GetBytesWithLength();
            AlphaOmegaMods = reader.GetBytesWithLength();
        }

        public override void Serialize(NetDataWriter writer)
        {
            // Serialize base
            base.Serialize(writer);

            // Serialize class
            writer.PutBytesWithLength(LevelProgress);
            writer.PutBytesWithLength(BoltsExpPoints);
            writer.PutBytesWithLength(Skillpoints);
            writer.PutBytesWithLength(AlphaOmegaMods);
        }
    }
}
