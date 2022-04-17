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

        // returns tlcl in celsius
        public static double GetLiftedCondensationLevelTemperature(double tempC, double dewpoint)
        {
            double tempK = tempC + 273.16;
            double dewpointK = dewpoint + 273.16;
            double partA = 1 / (dewpointK - 56);
            double partB = Math.Log(tempK / dewpointK) / 800;
            double result = 1 / (partA + partB) + 56;
            return result - 273.15;
        }

        public static double GetLiftedCondensationLevelPressure(double tempC, double dewpoint, double pressure)
        {
            double tlclK = GetLiftedCondensationLevelTemperature(tempC, dewpoint) + 273.15;
            double tempK = tempC + 273.16;
            double theta = tempK * Math.Pow(1000/pressure, 0.286);
            return 1000 * Math.Pow(tlclK/theta, 3.48);
        }

        // Function to return virtual temperature given temperature in 
        // kelvin and mixing ratio in g/g
        public static double Virtual(double tempK, double mixratio)
        {
            return tempK * (1 + 0.6 * mixratio);
        }

        // Given vapor pressure and pressure, return mixing ratio
        public static double MixRatio(double vaporPressure, double pressure)
        {
            return 0.622 * vaporPressure / (pressure - 0.377 * vaporPressure);
        }

        // Given temp in Celsius, returns saturation vapor pressure in mb
        public static double SatVap2(double tempC)
        {
            return 6.112 * Math.Exp(17.67 * tempC / (tempC + 243.5));
        }

        // returns relative humidity
        public static double RelativeHumidity(double tempC, double pressure, double mixratio)
        {
            double vap = SatVap2(tempC);
            double mix = MixRatio(vap, pressure);
            return 100 * ((mixratio / 1000) / mix);
        }

        // returns dewpoint temperature
        public static double DewPoint(double tempC, double humidity)
        {
            //double alpha = 6.112;
            double beta = 17.67;
            double lambda = 243.5;
            double part = Math.Log(humidity / 100) + (beta * tempC) / (lambda + tempC);
            return (lambda * part) / (beta - part);
        }

        // Function to return virtual temperature in kelvin given temperature in
        // kelvin and dewpoint in kelvin and pressure in mb
        public static double Virtual2(double tempK, double dewpointK, double pressure)
        {
            double vap = SatVap2(dewpointK - 273.16);
            double mix = MixRatio(vap, pressure);
            return Virtual(tempK, mix);
        }

        // Function to return the latent heat of condensation in J/kg given
        // temperature in degrees Celsius
        public static double LatentC(double tempC)
        {
            return 1000 * (2502.2 - 2.43089 * tempC);
        }

        // Function to calculate the moist adiabatic lapse rate (deg C/Pa) based
        // on the temperature, pressure, and rh of the environment
        public static double GammaW(double tempC, double pressure, double humidity)
        {
            double tempK = tempC + 273.16;
            double es = SatVap2(tempC);
            double ws = MixRatio(es, pressure);
            double w = humidity * ws / 100;
            double tempv = Virtual(tempK, w);
            double latent = LatentC(tempC);
            double partA = 1.0 + latent * ws / (287 * tempK);
            double partB = 1.0 + 0.622 * latent * latent * ws / (1005 * 287 * tempK * tempK);
            double density = 100 * pressure / (287 * tempv);
            return (partA / partB) / (1005 * density);
        }

        public static double InterpolationZ(double startP, double endP, double startVal, double endVal, double level)
        {
            double coeff = (startP - level) / (startP - endP);
            return startVal - coeff * (startVal - endVal);
        }

        // Не валидный
        public static double GetHumidity(double tempK, double dewpointK)
        {
            double tc = tempK - 273.16;
            double tdc = dewpointK - 273.16;
            return 100 * Math.Exp((18.678 - tdc / 234.5) * (tdc / (257.14 + tdc)) - (18.678 - tc / 234.5) * (tc / (257.14 + tc)));
        }

        public static double GetU(double speed, double direction)
        {
            return -1.0 * Math.Abs(speed) * Math.Sin((Math.PI / 180) * direction);
        }

        public static double GetV(double speed, double direction)
        {
            return -1.0 * Math.Abs(speed) * Math.Cos((Math.PI / 180) * direction);
        }
    }
}
