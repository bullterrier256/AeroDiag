using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace AeroDiag
{
    internal class AeroTableElem
    {
        private double? pressure;
        private double? height;
        private double? temperature;
        private double? mixratio;
        private double? direction;
        private double? speed;
        // доп. параметры
        private double? humidity;
        private double? dewpoint;
        private double? thetae;

        public AeroTableElem(string levelStr)
        {
            int pos = 0;
            while (pos < levelStr.Length)
            {
                if (pos + 6 <= levelStr.Length)
                {
                    break;
                }
                bool ok = false;
                double val;
                ok = Double.TryParse(levelStr.Substring(pos), out val);
                
            }
        }
    }

    internal class AeroTable
    {
        
    }
    
    
}
