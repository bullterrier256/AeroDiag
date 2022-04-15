using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

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
        private double? speedms;
        // доп. параметры
        private double? humidity;
        private double? dewpoint;
        private double? thetae;
        // cape etc
        private double? plcl;
        private double? cape;
        private double? cin;
        private double? lftx;

        public AeroTableElem(string levelStr)
        {
            int pos = 0;
            while (pos < levelStr.Length)
            {
                if (pos + 6 > levelStr.Length)
                {
                    break;
                }
                bool ok = false;
                double val;
                ok = Double.TryParse((levelStr.Substring(pos, 7)).Replace(".", ","), out val);
                switch (pos)
                {
                    case 0:
                        pressure = ok ? val: null;
                        break;
                    case 7:
                        height = ok ? val : null;
                        break;
                    case 14:
                        temperature = ok ? val : null;
                        break;
                    case 21:
                        dewpoint = ok ? val : null;
                        break;
                    case 28:
                        humidity = ok ? val : null;
                        break;
                    case 35:
                        mixratio = ok ? val : null;
                        break;
                    case 42:
                        direction = ok ? val : null;
                        break;
                    case 49:
                        speed = ok ? val : null;
                        break;
                    case 63:
                        thetae = ok ? val : null;
                        break;
                    default:
                        break;
                }
                if (speed is not null)
                {
                    speedms = MeteoMath.KnotsToMetersPerSecond((double)speed);
                }
                pos += 7;
            }
        }
        public bool IsNotNullable()
        {
            bool ok = (
                pressure != null &&
                height != null &&
                temperature != null &&
                dewpoint != null &&
                humidity != null &&
                mixratio != null &&
                direction != null &&
                speed != null &&
                thetae != null
            );
            return ok;
        }
        public static string ValToStr(double? val, int countAfterPoint, bool needAlign = true)
        {
            string result;
            if (val is null)
            {
                result = "       ";
            } 
            else
            {
                switch (countAfterPoint)
                {
                    case 0:
                        result = ((double)val).ToString("f0");
                        break;
                    case 1:
                        result = ((double)val).ToString("f1");
                        break;
                    case 2:
                        result = ((double)val).ToString("f2");
                        break;
                    default:
                        result = "       ";
                        break;
                }
            }
            if (needAlign)
            {
                while (result.Length < 7)
                {
                    result = " " + result;
                }
            }
            return result;
        }

        public string ToStr()
        {
            string result = String.Empty;
            result += ValToStr(pressure, 1);
            result += ValToStr(height, 0);
            result += ValToStr(temperature, 1);
            result += ValToStr(dewpoint, 1);
            result += ValToStr(humidity, 0);
            result += ValToStr(mixratio, 2);
            result += ValToStr(direction, 0);
            result += ValToStr(speed, 0);
            result += ValToStr(speedms, 1);
            result += ValToStr(thetae, 1);
            result += ValToStr(plcl, 1);
            result += ValToStr(cape, 1);
            return result;
        }

        public static string GetHeader()
        {
            string header;
            header =  "====================================================================================\r\n";
            header += "   PRES    HGT   TEMP   DWPT   RELH   MIXR   DRCT   SKNT    SMS   THTE   PLCL   CAPE\r\n";
            header += "    HPA      M      C      C      %   G/KG    DEG   KNOT    M/S      K    HPA   J/KG\r\n";
            header += "====================================================================================\r\n";
            return header;
        }

        public double? GetPres()
        {
            return pressure;
        }

        public double? GetHgt()
        {
            return height;
        }

        public double? GetTmp()
        {
            return temperature;
        }

        public double? GetMixRatio()
        {
            return mixratio;
        }
    }

    internal class AeroTable
    {
        private LinkedList<AeroTableElem> table;

        public AeroTable(string inputStr)
        {
            table = new LinkedList<AeroTableElem>();
            string str = inputStr;
            string[] items = str.Split("\n");
            for (int i = 5; i < items.Length; i++)
            {
                AeroTableElem elem = new AeroTableElem(items[i]);
                table.AddLast(elem);
            }
        }

        public string ToStr()
        {
            string result = AeroTableElem.GetHeader();
            foreach (AeroTableElem elem in table)
            {
                result += $"{elem.ToStr()}\r\n";
            }

            result += "\r\n";
            result += "DIAGNOSTIC VALUES: \r\n";

            double? hgt1000 = GetHeight(1000.0);
            double? hgt500 = GetHeight(500.0);

            result += $"Terrain height [m]: {AeroTableElem.ValToStr(GetTerrainHeight(), 1, false)}\r\n";

            if (hgt1000 != null && hgt500 != null)
            {
                result += $"1000 hPa to 500 hPa thickness[m]: {AeroTableElem.ValToStr((double)hgt500 - (double)hgt1000, 0, false)}\r\n";
            }

            result += $"Precipitable water [mm]: {AeroTableElem.ValToStr(GetPrecipitableWater(), 2, false)}";

            return result;
        }

        private bool HasLevel (double level)
        {
            bool ok = false;
            foreach (AeroTableElem elem in table)
            {
                double? pres;
                pres = elem.GetPres();
                if (pres is null)
                {
                    continue;
                }
                if ((double)pres == level)
                {
                    ok = true;
                    break;
                }
            }
            return ok;
        }

        private AeroTableElem? GetLevel(double level)
        {
            AeroTableElem? res = null;
            foreach (AeroTableElem elem in table)
            {
                double? pres;
                pres = elem.GetPres();
                if (pres is null)
                {
                    continue;
                }
                if ((double)pres == level)
                {
                    res = elem;
                    break;
                }
            }
            return res;
        }

        #region diagnosticVals

        private double? GetTerrainHeight()
        {
            double? res = null;
            double? bottomLevel = GetBottomLevel();
            if (bottomLevel is not null)
            {
                res = GetHeight((double)bottomLevel);
            }
            return res;
        }

        private double? GetBottomLevel()
        {
            double? res = null;
            var el = table.First;
            if (el is not null)
            {
                if (!el.Value.IsNotNullable())
                {
                    el = el.Next;
                    res = el.Value.GetPres();
                }
            }
            return res;
        }

        private double? GetTemperature(double level)
        {
            double? result = null;
            AeroTableElem? elem = GetLevel(level);
            if (elem is not null)
            {
                result = elem.GetTmp();
            }
            return result;
        }

        private double? GetHeight(double level)
        {
            double? result = null;
            AeroTableElem? elem = GetLevel(level);
            if (elem is not null)
            {
                result = elem.GetHgt();
            }
            return result;
        }

        public double? GetPrecipitableWater()
        {
            double? result = null;
            double? bottomLevel = GetBottomLevel();
            var elem = table.First;
            double? prevMixRat = null;
            double? prevPres = null;
            double? currentPres = null;
            double? currentMixRat = null;
            if (bottomLevel is not null) {
                while (elem != null)
                {
                    elem = elem.Next;
                    AeroTableElem tabElem = elem.Value;
                    prevPres = currentPres;
                    currentPres = tabElem.GetPres();
                    if (currentPres is null)
                    {
                        break;
                    }
                    if (currentPres == bottomLevel)
                    {
                        currentMixRat = tabElem.GetMixRatio();
                        if (currentMixRat is null)
                        {
                            break;
                        }
                        result = 0;
                    }
                    if (currentPres < bottomLevel)
                    {
                        prevMixRat = currentMixRat;
                        currentMixRat = tabElem.GetMixRatio();
                        if (currentMixRat is null)
                        {
                            break;
                        }
                        result += (0.5 * (currentMixRat + prevMixRat) * (prevPres - currentPres)) / (9.97 * MeteoMath.GetGravity());
                    }
                }
            }
            return result;
        }

        //TLCL = (((1/(1/(SDP - 56) + Math.log (ST/SDP)/800)) + 56) - 273.16);
        //PLCL = (SP* Math.pow(((TLCL + 273.16) / ST), (7/2) ) ) / 1000;

        //document.lcl.PLCLP.value = PLCL* 1000; //Pa
        // SP - surface pressure kPa
        // ST - surface temperature
        #endregion
    }
}
