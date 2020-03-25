using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DLMC.Shared.Utils
{
    public static class NetDataStreamExtensions
    {
        public static void PutEnum<T>(this NetDataWriter writer, T value) where T : struct, IConvertible
        {
            PutEnum(writer, value, value.GetType());
        }

        private static void PutEnum(this NetDataWriter writer, object value, Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            writer.PutObject(value, type.GetEnumUnderlyingType());
        }

        public static T GetEnum<T>(this NetDataReader reader) where T : struct, IConvertible
        {
            Type t = typeof(T);
            if (t.IsEnum)
                t = t.GetEnumUnderlyingType();

            return (T)reader.GetObject(t);
        }

        public static T PeekEnum<T>(this NetDataReader reader) where T : struct, IConvertible
        {
            Type t = typeof(T);
            if (t.IsEnum)
                t = t.GetEnumUnderlyingType();

            return (T)reader.PeekObject(t);
        }

        private static void PutObject(this NetDataWriter writer, object value, Type type)
        {
            if (value == null || type == null)
                return;

            if (value is INetSerializable)
                (value as INetSerializable).Serialize(writer);
            else if (type.IsEnum)
                writer.PutObject(value, type.GetEnumUnderlyingType());
            else if (type == typeof(bool))
                writer.Put((bool)value);
            else if (type == typeof(byte))
                writer.Put((byte)value);
            else if (type == typeof(sbyte))
                writer.Put((sbyte)value);
            else if (type == typeof(char))
                writer.Put((char)value);
            else if (type == typeof(short))
                writer.Put((short)value);
            else if (type == typeof(ushort))
                writer.Put((ushort)value);
            else if (type == typeof(int))
                writer.Put((int)value);
            else if (type == typeof(uint))
                writer.Put((uint)value);
            else if (type == typeof(long))
                writer.Put((long)value);
            else if (type == typeof(ulong))
                writer.Put((ulong)value);
            else if (type == typeof(float))
                writer.Put((float)value);
            else if (type == typeof(double))
                writer.Put((double)value);
            else if (type == typeof(string))
                writer.Put((string)value);
            else if (type == typeof(IPEndPoint))
                writer.Put((IPEndPoint)value);
            else
                throw new Exception("Unable to serialize object " + value + " of type " + type);
        }

        private static object GetObject(this NetDataReader reader, Type type)
        {
            if (type == null)
                return null;

            if (type.GetInterface("INetSerializable") != null)
            {
                var result = (INetSerializable)Activator.CreateInstance(type);
                result.Deserialize(reader);
                return result;
            }
            else if (type.GetInterface("ISyncObject") != null)
            {
                return reader.GetByte();
            }
            else if (type.IsEnum)
                return Enum.Parse(type, type.GetEnumName(reader.GetObject(type.GetEnumUnderlyingType())));
            else if (type == typeof(bool))
                return reader.GetBool();
            else if (type == typeof(byte))
                return reader.GetByte();
            else if (type == typeof(sbyte))
                return reader.GetSByte();
            else if (type == typeof(char))
                return reader.GetChar();
            else if (type == typeof(short))
                return reader.GetShort();
            else if (type == typeof(ushort))
                return reader.GetUShort();
            else if (type == typeof(int))
                return reader.GetInt();
            else if (type == typeof(uint))
                return reader.GetUInt();
            else if (type == typeof(long))
                return reader.GetLong();
            else if (type == typeof(ulong))
                return reader.GetULong();
            else if (type == typeof(float))
                return reader.GetFloat();
            else if (type == typeof(double))
                return reader.GetBool();
            else if (type == typeof(string))
                return reader.GetDouble();
            else if (type == typeof(IPEndPoint))
                return reader.GetNetEndPoint();
            else
                throw new Exception("Unable to deserialize object of type " + type);
        }

        private static object PeekObject(this NetDataReader reader, Type type)
        {
            if (type == null)
                return null;

            if (type.IsEnum)
                return Enum.Parse(type, type.GetEnumName(reader.PeekObject(type.GetEnumUnderlyingType())));
            else if (type == typeof(bool))
                return reader.PeekBool();
            else if (type == typeof(byte))
                return reader.PeekByte();
            else if (type == typeof(sbyte))
                return reader.PeekSByte();
            else if (type == typeof(char))
                return reader.PeekChar();
            else if (type == typeof(short))
                return reader.PeekShort();
            else if (type == typeof(ushort))
                return reader.PeekUShort();
            else if (type == typeof(int))
                return reader.PeekInt();
            else if (type == typeof(uint))
                return reader.PeekUInt();
            else if (type == typeof(long))
                return reader.PeekLong();
            else if (type == typeof(ulong))
                return reader.PeekULong();
            else if (type == typeof(float))
                return reader.PeekFloat();
            else if (type == typeof(double))
                return reader.PeekBool();
            else if (type == typeof(string))
                return reader.PeekDouble();
            else
                throw new Exception("Unable to deserialize object of type " + type);
        }
    }
}
