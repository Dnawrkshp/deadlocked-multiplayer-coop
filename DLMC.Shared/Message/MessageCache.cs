using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLMC.Shared.Message
{
    public class MessageCache
    {
        Dictionary<MessageId, Type> _classesById = null;
        Dictionary<MessageId, MessageBase> _messageCache = new Dictionary<MessageId, MessageBase>();

        public MessageBase this[MessageId id]
        {
            get
            {
                // See if we already have it
                MessageBase result = null;
                if (_messageCache.TryGetValue(id, out result))
                    return result;

                // Initialize and create new one
                Initialize();

                if (!_classesById.ContainsKey(id))
                    return null;

                var t = _classesById[id];
                if (t == null)
                    return null;

                return _messageCache[id] = (MessageBase)Activator.CreateInstance(t);
            }
        }

        public T Get<T>() where T : MessageBase, new()
        {
            // Init id->types dictionary
            Initialize();

            // Find id from type
            MessageId id = _classesById.FirstOrDefault(x => x.Value == typeof(T)).Key;
            if (id == MessageId.Invalid)
                return null;

            return (T)this[id];
        }

        public void Set<T>(T obj) where T : MessageBase, new()
        {
            if (obj == null)
                return;

            _messageCache[obj.Id] = obj;
        }

        private void Initialize()
        {
            if (_classesById != null)
                return;

            _classesById = new Dictionary<MessageId, Type>();

            // Populate
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(MessageBase));
            foreach (Type type in assembly.GetTypes())
            {
                var attrs = (MessageIdAttribute[])type.GetCustomAttributes(typeof(MessageIdAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    _classesById.Add(attrs[0].Id, type);
            }
        }

        public T Deserialize<T>(NetPacketReader reader) where T : MessageBase, new()
        {
            T instance = Get<T>();
            if (instance == null)
                return null;

            instance.Deserialize(reader);
            return instance;
        }

        public MessageBase Deserialize(MessageId id, NetPacketReader reader)
        {
            MessageBase instance = this[id];
            if (instance == null)
                return null;

            instance.Deserialize(reader);
            return instance;
        }

        public bool Serialize(MessageId id, NetDataWriter writer)
        {
            MessageBase instance = this[id];
            if (instance == null)
                return false;

            instance.Serialize(writer);
            return true;
        }
    }
}
