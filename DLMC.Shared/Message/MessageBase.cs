using DLMC.Shared.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLMC.Shared.Message
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class MessageIdAttribute : System.Attribute
    {
        public MessageId Id { get; private set; } = MessageId.Invalid;

        public MessageIdAttribute(MessageId id)
        {
            Id = id;
        }
    }

    public enum MessageId : ushort
    {
        Invalid = 0,

        PadUpdate,
        PlayerUpdate,
        ProgressUpdate,
        EquipmentUpdate,
        MenuUpdate,
    }

    public static class MessageIdExtensions
    {
        public static bool IsValid(this MessageId id)
        {
            return id > MessageId.Invalid && id <=MessageId.MenuUpdate;
        }
    }

    public abstract class MessageBase : INetSerializable
    {
        public virtual MessageId Id { get; } = MessageId.Invalid;
        public virtual DeliveryMethod TransportType { get; } = DeliveryMethod.ReliableOrdered;

        public virtual bool IsValid()
        {
            return Id.IsValid();
        }

        public virtual void Serialize(NetDataWriter writer)
        {
            // Write id
            writer.PutEnum(Id);
        }

        public virtual void Deserialize(NetDataReader reader)
        {
            // Ensure id is correct before modifying stream
            if (reader.GetEnum<MessageId>() != Id)
                throw new Exception("Deserializing message with wrong id!");
        }
    }
}
