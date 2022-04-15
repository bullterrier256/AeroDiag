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
        // доп. параметры
        private double? humidity;
        private double? dewpoint;
        private double? thetae;

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
            result += ValToStr(thetae, 1);
            return result;
        }

        public static string GetHeader()
        {
            string header;
            header =  "===============================================================\r\n";
            header += "   PRES    HGT   TEMP   DWPT   RELH   MIXR   DRCT   SKNT   THTE\r\n";
            header += "    HPA      M      C      C      %   G/KG    DEG   KNOT      K\r\n";
            header += "===============================================================\r\n";
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

            bool has1000 = HasLevel(1000.0);
            bool has500 = HasLevel(500.0);

            result += $"Terrain height: {AeroTableElem.ValToStr(GetTerrainHeight(), 1, false)}";

            

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
            var el = table.First;
            if (el is not null)
            {
                if (!el.Value.IsNotNullable())
                {
                    el = el.Next;
                    res = el.Value.GetHgt();
                }
            }
            return res;
        }

        #endregion
    }
}
