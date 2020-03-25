using DLMC.Launcher.Memory;
using DLMC.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMC.Launcher.Memory
{
    public class PCSX2
    {
        private static PCSX2 _singleton = null;
        private MemoryHelper _memoryHelper = null;

        private Dictionary<ulong, byte[]> _frozen = new Dictionary<ulong, byte[]>();

        public PCSX2(string pcsx2Path, string isoPath)
        {
            _memoryHelper = new MemoryHelper(LaunchPCSX2(pcsx2Path, isoPath) ?? FindPCSX2() ?? throw new Exception("Unable to find pcsx2 process!"));
        }

        private void Freeze()
        {
            try
            {
                foreach (var kvp in _frozen)
                    _memoryHelper.Write((IntPtr)kvp.Key, kvp.Value);
            }
            catch (Exception)
            {

            }
        }

        #region Process

        Process LaunchPCSX2(string exePath, string isoPath)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(exePath, "\"" + isoPath + "\" --windowed")
                {
                    
                };
                return System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        Process FindPCSX2()
        {
            return System.Diagnostics.Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains("pcsx2")).FirstOrDefault();
        }

        #endregion

        #region Static

        public static void Start(string pcsx2Path, string isoPath)
        {
            _singleton = new PCSX2(pcsx2Path, isoPath);
        }

        public static bool HasInstance()
        {
            return _singleton != null && _singleton._memoryHelper.IsProcessAlive();
        }

        public static void Update()
        {
            _singleton.Freeze();
        }

        public static void Freeze(IntPtr address, byte[] value)
        {
            address = address.MapToPCSX2();
            if (_singleton._frozen.ContainsKey((ulong)address))
                _singleton._frozen[(ulong)address] = value;
            else
                _singleton._frozen.Add((ulong)address, value);
        }

        public static void Unfreeze(IntPtr address)
        {
            address = address.MapToPCSX2();
            if (_singleton._frozen.ContainsKey((ulong)address))
                _singleton._frozen.Remove((ulong)address);
        }

        public static void Write<T>(IntPtr address, T value)
        {
            try
            {
                _singleton._memoryHelper.Write(address.MapToPCSX2(), value);
            }
            catch (Exception)
            {

            }
        }

        public static T Read<T>(IntPtr address)
        {
            try
            {
                return _singleton._memoryHelper.Read<T>(address.MapToPCSX2());
            }
            catch (Exception)
            {

            }

            return default(T);
        }

        public static byte[] Read(IntPtr address, int length)
        {
            try
            {
                return _singleton._memoryHelper.Read(address.MapToPCSX2(), length);
            }
            catch (Exception)
            {

            }

            return null;
        }
        
        #endregion

    }

    public static class PCSX2Extensions
    {
        #region Memory

        public static IntPtr MapToPCSX2(this IntPtr address)
        {
            return (IntPtr)(((uint)address & 0x0FFFFFFF) | 0x20000000);
        }

        #endregion
    }
}
