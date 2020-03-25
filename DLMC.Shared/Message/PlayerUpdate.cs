using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLMC.Shared.Message
{
    [MessageId(MessageId.PlayerUpdate)]
    public class PlayerUpdate : MessageBase
    {
        public override MessageId Id => MessageId.PlayerUpdate;
        public override DeliveryMethod TransportType => DeliveryMethod.Unreliable;

        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float Rotation;
        public float CameraYaw;
        public float CameraPitch;

        public override void Deserialize(NetDataReader reader)
        {
            // Deserialize base
            base.Deserialize(reader);

            // Read class
            PositionX = reader.GetFloat();
            PositionY = reader.GetFloat();
            PositionZ = reader.GetFloat();
            Rotation = reader.GetFloat();
            CameraYaw = reader.GetFloat();
            CameraPitch = reader.GetFloat();
        }

        public override void Serialize(NetDataWriter writer)
        {
            // Serialize base
            base.Serialize(writer);

            // Serialize class
            writer.Put(PositionX);
            writer.Put(PositionY);
            writer.Put(PositionZ);
            writer.Put(Rotation);
            writer.Put(CameraYaw);
            writer.Put(CameraPitch);
        }
    }
}
