using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DLMC.Shared.Message
{
    [MessageId(MessageId.PadUpdate)]
    public class PadUpdate : MessageBase
    {
        public override MessageId Id => MessageId.PadUpdate;
        public override DeliveryMethod TransportType => DeliveryMethod.Unreliable;

        public ushort ButtonMask;
        public ushort LeftAnalog;
        public ushort RightAnalog;

        public override void Deserialize(NetDataReader reader)
        {
            // Deserialize base
            base.Deserialize(reader);

            // Read class
            ButtonMask = reader.GetUShort();
            LeftAnalog = reader.GetUShort();
            RightAnalog = reader.GetUShort();
        }

        public override void Serialize(NetDataWriter writer)
        {
            // Serialize base
            base.Serialize(writer);

            // Serialize class
            writer.Put(ButtonMask);
            writer.Put(LeftAnalog);
            writer.Put(RightAnalog);
        }
    }
}
