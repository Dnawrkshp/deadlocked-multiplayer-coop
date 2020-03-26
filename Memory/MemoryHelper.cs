using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DLMC.Launcher.Memory
{
    public class MemoryHelper
    {

        #region Win32 

        const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        #endregion

        private IntPtr _processHandle;
        private int _processId;

        public MemoryHelper(Process proc)
        {
            _processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, _processId = proc.Id);
        }

        public bool IsProcessAlive()
        {
            try
            {
                Process.GetProcessById(_processId);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public void Write<T>(IntPtr address, T value)
        {
            Write(address, ToBuffer(value));
        }

        public void Write(IntPtr address, byte[] value)
        {
            int bytesWritten = 0;
            WriteProcessMemory((int)_processHandle, (int)address, value, value.Length, ref bytesWritten);
        }

        public T Read<T>(IntPtr address)
        {
            Type t = typeof(T);
            byte[] buffer = null;

            if (t == typeof(byte) || t == typeof(sbyte))
            {
                buffer = Read(address, 1);
                return (T)(object)buffer[0];
            }
            else if (t == typeof(short))
            {
                buffer = Read(address, 2);
                return (T)(object)BitConverter.ToInt16(buffer, 0);
            }
            else if (t == typeof(ushort))
            {
                buffer = Read(address, 2);
                return (T)(object)BitConverter.ToUInt16(buffer, 0);
            }
            else if (t == typeof(int))
            {
                buffer = Read(address, 4);
                return (T)(object)BitConverter.ToInt32(buffer, 0);
            }
            else if (t == typeof(uint))
            {
                buffer = Read(address, 4);
                return (T)(object)BitConverter.ToUInt32(buffer, 0);
            }
            else if (t == typeof(float))
            {
                buffer = Read(address, 4);
                return (T)(object)BitConverter.ToSingle(buffer, 0);
            }

            throw new NotImplementedException($"Unable to convert to {typeof(T).Name} from a byte array!");
        }

        public byte[] Read(IntPtr address, int length)
        {
            byte[] buffer = new byte[length];
            int bytesRead = 0;
            ReadProcessMemory((int)_processHandle, (int)address, buffer, length, ref bytesRead);

            return buffer;
        }

        public void Read(IntPtr address, byte[] destination, int length)
        {
            int bytesRead = 0;
            ReadProcessMemory((int)_processHandle, (int)address, destination, length, ref bytesRead);
        }

        private byte[] ToBuffer<T>(T value)
        {
            Type t = typeof(T);

            if (t == typeof(byte[]))
                return (byte[])Convert.ChangeType(value, typeof(byte[]));
            else if (t == typeof(byte))
                return new byte[] { (byte)Convert.ChangeType(value, typeof(byte)) };
            else if (t == typeof(sbyte))
                return new byte[] { (byte)Convert.ChangeType(value, typeof(sbyte)) };
            else if (t == typeof(short))
                return BitConverter.GetBytes((short)Convert.ChangeType(value, typeof(short)));
            else if (t == typeof(ushort))
                return BitConverter.GetBytes((ushort)Convert.ChangeType(value, typeof(ushort)));
            else if (t == typeof(int))
                return BitConverter.GetBytes((int)Convert.ChangeType(value, typeof(int)));
            else if (t == typeof(uint))
                return BitConverter.GetBytes((uint)Convert.ChangeType(value, typeof(uint)));
            else if (t == typeof(float))
                return BitConverter.GetBytes((float)Convert.ChangeType(value, typeof(float)));

            throw new NotImplementedException($"Unable to convert {typeof(T).Name} to a byte array!");
        }

    }
}