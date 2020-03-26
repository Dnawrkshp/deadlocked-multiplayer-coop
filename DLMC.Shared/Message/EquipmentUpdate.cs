using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLMC.Shared.Message
{
    [MessageId(MessageId.EquipmentUpdate)]
    public class EquipmentUpdate : MessageBase
    {
        public override MessageId Id => MessageId.EquipmentUpdate;
        public override DeliveryMethod TransportType => DeliveryMethod.ReliableUnordered;

        public byte[] Equipment = null;

        public override void Deserialize(NetDataReader reader)
        {
            // Deserialize base
            base.Deserialize(reader);

            // Read class
            Equipment = reader.GetBytesWithLength();
        }

        public override void Serialize(NetDataWriter writer)
        {
            // Serialize base
            base.Serialize(writer);

            // Serialize class
            writer.PutBytesWithLength(Equipment);
        }
    }
}
