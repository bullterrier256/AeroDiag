using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDiag
{
    /// <summary>
    /// Описание 1 уровня в атмосфере
    /// </summary>
    internal class AeroTableElem
    {
        /// <summary>
        /// Атмосферное давление
        /// </summary>
        private double? pressure;

        /// <summary>
        /// Высота
        /// </summary>
        private double? height;

        /// <summary>
        /// Температура
        /// </summary>
        private double? temperature;

        /// <summary>
        /// Отношение смеси
        /// </summary>
        private double? mixratio;

        /// <summary>
        /// Направление ветра
        /// </summary>
        private double? direction;

        /// <summary>
        /// Скорость ветра в узлах
        /// </summary>
        private double? speed;

        /// <summary>
        /// Скорость ветра в м/с
        /// </summary>
        private double? speedms;
        
        /// <summary>
        /// Относительная влажномть
        /// </summary>
        private double? humidity;

        /// <summary>
        /// Температура точки росы
        /// </summary>
        private double? dewpoint;

        /// <summary>
        /// Эквивалентно-потенциальная температура
        /// </summary>
        private double? thetae;
        
        /// <summary>
        /// Давление на уровне конденсации
        /// </summary>
        private double? plcl;

        /// <summary>
        /// Потенциальная доступная энергия неустойчивости
        /// </summary>
        private double? cape;

        /// <summary>
        /// Энергия задерживающего слоя
        /// </summary>
        private double? cin;

        /// <summary>
        /// Индекс плавучести
        /// </summary>
        private double? lftx;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="levelStr">Строка для парсинга</param>
        public AeroTableElem(string levelStr)
        {
            int pos = 0;
            while (pos < levelStr.Length)
            {
                // не добавил в условие цикла для читаемоти
                // Последний символ строки может быть битым
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
                        pressure = ok ? val : null;
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

        /// <summary>
        /// Проверка на пустые поля
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Перевод в строку. Статический метод
        /// </summary>
        /// <param name="val">Значение</param>
        /// <param name="countAfterPoint">Знаков после запятой</param>
        /// <param name="needAlign">Необходимость выравнивания</param>
        /// <returns></returns>
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
            return result.Replace(",", ".");
        }

        /// <summary>
        /// Приведение к табличной строке
        /// </summary>
        /// <returns></returns>
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
            result += ValToStr(cape, 0);
            result += ValToStr(cin, 0);
            result += ValToStr(lftx, 1);
            return result;
        }

        /// <summary>
        /// Получить заголовок таблицы. Статический метод
        /// </summary>
        /// <returns>Заголовок</returns>
        public static string GetHeader()
        {
            string header;
            header = "Data from https://weather.uwyo.edu/upperair/sounding.html\r\n";
            header += "==================================================================================================\r\n";
            header += "   PRES    HGT   TEMP   DWPT   RELH   MIXR   DRCT   SKNT    SMS   THTE   PLCL   CAPE    CIN   LFTX\r\n";
            header += "    HPA      M      C      C      %   G/KG    DEG   KNOT    M/S      K    HPA   J/KG   J/KG      C\r\n";
            header += "==================================================================================================\r\n";
            return header;
        }

        #region GetSet
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

        public double? GetDewPoint()
        {
            return dewpoint;
        }

        public double? GetMixRatio()
        {
            return mixratio;
        }

        public double? GetLiftedCondensationLevel()
        {
            return plcl;
        }

        public double? GetCape()
        {
            return cape;
        }
        public double? GetCin()
        {
            return cin;
        }

        public double? GetLftx()
        {
            return lftx;
        }

        public double? GetThetaE()
        {
            return thetae;
        }

        public double? GetSpeed()
        {
            return speed;
        }

        public double? GetDirection()
        {
            return direction;
        }

        /// <summary>
        /// Установить значения параметров поднимающейся частицы
        /// </summary>
        /// <param name="plclVal">Давление на уровне конденсации</param>
        /// <param name="capeVal">Погтенциальная доступная энергия неустойчивости</param>
        /// <param name="cinVal">Энергия задерживающего слоя</param>
        /// <param name="lftxVal">Индекс плавучести</param>
        public void SetGammaW(double? plclVal, double? capeVal, double? cinVal, double? lftxVal)
        {
            plcl = plclVal;
            cape = capeVal;
            cin = cinVal;
            lftx = lftxVal;
        }
        #endregion
    }
}
