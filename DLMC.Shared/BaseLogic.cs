using DLMC.Shared.Message;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLMC.Shared
{
    public abstract class BaseLogic
    {
        public static BaseLogic Singleton = null;


        public abstract NetManager NetMan { get; }
        public abstract NetPeer Server { get; }
        public abstract LogicTimer Logic { get; }

        protected readonly NetDataWriter cachedWriter = new NetDataWriter();

        public virtual bool IsServer => false;
        public virtual bool IsClient => false;
        public abstract byte LocalPlayerId { get; }
        public abstract byte RemotePlayerId { get; }

        public abstract void Update();
        public abstract void Stop();

        public void Send(MessageBase message)
        {
            if (message == null)
                return;

            cachedWriter.Reset();
            message.Serialize(cachedWriter);
            NetMan.SendToAll(cachedWriter, message.TransportType);
        }
    }
}
