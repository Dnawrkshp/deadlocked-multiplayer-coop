using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLMC.Shared.Utils
{
    public static class Logger
    {
        public static event Action<string> OnLog;

        public static void Log(string message)
        {
            OnLog?.Invoke(message);
        }

        public static void Log(Exception e)
        {
            OnLog?.Invoke(e.ToString());
        }
    }
}
