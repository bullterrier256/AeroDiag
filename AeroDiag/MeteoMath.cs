using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDiag
{
    /// <summary>
    /// Метеорологические формулы
    /// </summary>
    internal class MeteoMath
    {
        /// <summary>
        /// Ускорение свободного падения [м/с^2]
        /// </summary>
        public const double gravity = 9.80665;

        /// <summary>
        /// Коэффициент перевода из узлов в метры в секунду
        /// </summary>
        public const double knotsKoefficient = 0.514444;

        /// <summary>
        /// Коэффициент для перевода из градусов цельсия в фаренгейты
        /// </summary>
        public const double kelvinKoefficient = 273.16;

        /// <summary>
        /// Стандартное давление [гПа]
        /// </summary>
        public const double standardPressure = 1000.0;

        /// <summary>
        /// Частное универсальной газовой постоянной и удельной телоемкости при постоянном давлении
        /// </summary>
        public const double kR = 0.286;

        /// <summary>
        /// Частное универсальной газовой постоянной и газовой постоянной водяного пара
        /// </summary>
        public const double kRV = 3.48;

        /// <summary>
        /// Температура сублимации водяного пара [K]
        /// </summary>
        public const double sublimationConst = 243.5;

        /// <summary>
        /// Безымянная константа для влажностных характеристик атмосферы
        /// </summary>
        public const double c = 17.67;

        /// <summary>
        /// Универсальная газовая постоянная для сухого воздуха [Дж\кг\K]
        /// </summary>
        public const double cD = 1005.0;

        /// <summary>
        /// отношение молекулярной массы водяного пара/сухого воздуха (Парциальное давление водяного пара)
        /// </summary>
        public const double eD = 0.622;

        /// <summary>
        /// Константа для перевода геометрических градусов в географические
        /// </summary>
        public const double geoDegree = 270.0;

        /// <summary>
        /// Перевод скорости ветра в узлах в метры в секунду
        /// </summary>
        /// <param name="val">Скорость ветра в узлах</param>
        /// <returns>Скорость ветра в метрах в секунду</returns>
        public static double KnotsToMetersPerSecond(double val)
        {
            return val * knotsKoefficient;
        }

        /// <summary>
        /// Температура уровня конденсациии
        /// </summary>
        /// <param name="tempC">температура [C]</param>
        /// <param name="dewpoint">температура точки росы [C]</param>
        /// <returns>температура уровня конденсации [C]</returns>
        public static double GetLiftedCondensationLevelTemperature(double tempC, double dewpoint)
        {
            double tempK = tempC + kelvinKoefficient;
            double dewpointK = dewpoint + kelvinKoefficient;
            double partA = 1 / (dewpointK - 56);
            double partB = Math.Log(tempK / dewpointK) / 800;
            double result = 1 / (partA + partB) + 56;
            return result - kelvinKoefficient;
        }

        /// <summary>
        /// Давление на уровне конденсации
        /// </summary>
        /// <param name="tempC">температура [C]</param>
        /// <param name="dewpoint">температура точки росы [C]</param>
        /// <param name="pressure">давление [гПа]</param>
        /// <returns>Давление уровня конденсации [гПа]</returns>
        public static double GetLiftedCondensationLevelPressure(double tempC, double dewpoint, double pressure)
        {
            double tlclK = GetLiftedCondensationLevelTemperature(tempC, dewpoint) + kelvinKoefficient;
            double tempK = tempC + kelvinKoefficient;
            double theta = tempK * Math.Pow(standardPressure/pressure, kR);
            return standardPressure * Math.Pow(tlclK/theta, kRV);
        }

        /// <summary>
        /// Виртуальная температура
        /// </summary>
        /// <param name="tempK">температура [K]</param>
        /// <param name="mixratio">Отношение смеси [кг/кг]</param>
        /// <returns>Виртуальная температура [К]</returns>
        public static double Virtual(double tempK, double mixratio)
        {
            return tempK * (1 + 0.6 * mixratio);
        }

        /// <summary>
        /// Отношение смеси
        /// </summary>
        /// <param name="vaporPressure">Парциальное давление водяного пара [гПа]</param>
        /// <param name="pressure">Давление [гПа]</param>
        /// <returns>Отношение смеси [кг/кг]</returns>
        public static double MixRatio(double vaporPressure, double pressure)
        {
            return eD * vaporPressure / (pressure - 0.377 * vaporPressure);
        }

        /// <summary>
        /// Давление насыщенного водяного пара
        /// </summary>
        /// <param name="tempC">температура [C]</param>
        /// <returns>Давление насыщенного водяного пара [гПа]</returns>
        public static double SatVap2(double tempC)
        {
            return 6.112 * Math.Exp(c * tempC / (tempC + sublimationConst));
        }

        /// <summary>
        /// Относительная влажность
        /// </summary>
        /// <param name="tempC">температура [C]</param>
        /// <param name="pressure">Давление [гПа]</param>
        /// <param name="mixratio">Отношение смеси [кг/кг]</param>
        /// <returns>Относительная влажность [%]</returns>
        public static double RelativeHumidity(double tempC, double pressure, double mixratio)
        {
            double vap = SatVap2(tempC);
            double mix = MixRatio(vap, pressure);
            return 100 * ((mixratio / standardPressure) / mix);
        }

        /// <summary>
        /// Температура точки росы
        /// </summary>
        /// <param name="tempC">температура [C]</param>
        /// <param name="humidity">Относительная влажность [%]</param>
        /// <returns>температура точки росы [C]</returns>
        public static double DewPoint(double tempC, double humidity)
        {
            double beta = c;
            double lambda = sublimationConst;
            double part = Math.Log(humidity / 100) + (beta * tempC) / (lambda + tempC);
            return (lambda * part) / (beta - part);
        }

        /// <summary>
        /// Виртуальная температура
        /// </summary>
        /// <param name="tempK">температура [К]</param>
        /// <param name="dewpointK">температура точки росы [К]</param>
        /// <param name="pressure">Давление [гПа]</param>
        /// <returns>Виртуальная температура [К]</returns>
        public static double Virtual2(double tempK, double dewpointK, double pressure)
        {
            double vap = SatVap2(dewpointK - kelvinKoefficient);
            double mix = MixRatio(vap, pressure);
            return Virtual(tempK, mix);
        }

        /// <summary>
        /// Скрытая теплота конденсации
        /// </summary>
        /// <param name="tempC">температура [C]</param>
        /// <returns>Скрытая теплота конденсации [Дж/кг]</returns>
        public static double LatentC(double tempC)
        {
            return standardPressure * (2502.2 - 2.43089 * tempC);
        }

        /// <summary>
        /// Влажноадиабатический градиент
        /// </summary>
        /// <param name="tempC">температура [C]</param>
        /// <param name="pressure">Давление [гПа]</param>
        /// <param name="humidity"></param>
        /// <returns>Относительная влажность [%]</returns>
        public static double GammaW(double tempC, double pressure, double humidity)
        {
            double tempK = tempC + kelvinKoefficient;
            double es = SatVap2(tempC);
            double ws = MixRatio(es, pressure);
            double w = humidity * ws / 100;
            double tempv = Virtual(tempK, w);
            double latent = LatentC(tempC);
            double partA = 1.0 + latent * ws / ((kR * 1000) * tempK);
            double partB = 1.0 + eD * latent * latent * ws / (cD * (kR * 1000) * tempK * tempK);
            double density = 100 * pressure / ((kR * 1000) * tempv);
            return (partA / partB) / (cD * density);
        }
        
        /// <summary>
        /// Вертикальная линейная интерполяция
        /// </summary>
        /// <param name="startP">Начальное давление</param>
        /// <param name="endP">Конечное давление</param>
        /// <param name="startVal">Значение величины на начальном давлении</param>
        /// <param name="endVal">Значение величины на конечном давлении</param>
        /// <param name="level">Давление для интерполяции</param>
        /// <returns>Значение интерполированной величины</returns>
        public static double InterpolationZ(double startP, double endP, double startVal, double endVal, double level)
        {
            double coeff = (startP - level) / (startP - endP);
            return startVal - coeff * (startVal - endVal);
        }

        /// <summary>
        /// Относительная влажность (НЕ ВАЛИДНЫЙ)
        /// </summary>
        /// <param name="tempK">температура [К]</param>
        /// <param name="dewpointK">температура точки росы [К]</param>
        /// <returns>Относительная влажность [%]</returns>
        private static double GetHumidity(double tempK, double dewpointK)
        {
            double tc = tempK - kelvinKoefficient;
            double tdc = dewpointK - kelvinKoefficient;
            return 100 * Math.Exp((18.678 - tdc / 234.5) * (tdc / (257.14 + tdc)) - (18.678 - tc / 234.5) * (tc / (257.14 + tc)));
        }

        /// <summary>
        /// Получение u-компоненты ветра
        /// </summary>
        /// <param name="speed">Скорость ветра</param>
        /// <param name="direction">Направление ветра</param>
        /// <returns>u-компонента вектора ветра</returns>
        public static double GetU(double speed, double direction)
        {
            double dtr = Math.PI / 180;
            double spd = (speed <= 0) ? 0.01 : speed;
            return spd * Math.Cos(dtr * (geoDegree - direction));
        }

        /// <summary>
        /// Получение v-компоненты ветра
        /// </summary>
        /// <param name="speed">Скорость ветра</param>
        /// <param name="direction">Направление ветра</param>
        /// <returns>v-компонента вектора ветра</returns>
        public static double GetV(double speed, double direction)
        {
            double dtr = Math.PI / 180;
            double spd = (speed <= 0) ? 0.01 : speed;
            return spd * Math.Sin(dtr * (geoDegree - direction));
        }

        /// <summary>
        /// Скорость ветра
        /// </summary>
        /// <param name="u">u-компонента вектора ветра</param>
        /// <param name="v">v-компонента вектора ветра</param>
        /// <returns>Скорость ветра</returns>
        public static double GetWindSpeed(double u, double v)
        {
            return Math.Sqrt(u * u + v * v);
        }

        /// <summary>
        /// Направление ветра
        /// </summary>
        /// <param name="u">u-компонента вектора ветра</param>
        /// <param name="v">v-компонента вектора ветра</param>
        /// <returns>Направление ветра</returns>
        public static double GetWindDirection(double u, double v)
        {
            double dtr = Math.PI / 180;
            double rdt = 1 / dtr;
            double direction = geoDegree - rdt * Math.Atan2(v, u);
            return direction;
        }
    }
}
