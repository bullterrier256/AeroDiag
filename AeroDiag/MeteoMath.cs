using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDiag
{
    internal class MeteoMath
    {
        public static double GetGravity () {
            return (double)9.80665;
        }

        public static double KnotsToMetersPerSecond(double val)
        {
            return val * 0.514444;
        }
    }
}
