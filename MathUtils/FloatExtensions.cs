using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMC.Launcher.MathUtils
{
    public static class FloatExtensions
    {

        public static float Lerp(this float start, float target, float time)
        {
            return (target - start) * time + start;
        }

    }
}
