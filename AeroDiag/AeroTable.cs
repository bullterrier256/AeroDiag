using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace AeroDiag
{
    /// <summary>
    /// Данные аэрологического зондирования
    /// </summary>
    internal class AeroTable
    {
        /// <summary>
        /// Двунаправленный список со слоями данных по давлению
        /// </summary>
        private LinkedList<AeroTableElem> table;

        /// <summary>
        /// Конструктор, парсит таблицу
        /// </summary>
        /// <param name="inputStr">Нераспарсенная таблица</param>
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

            // Сразу после парсинга выполняем расчет поднимающейся частицы
            SetGammaW();
        }

        /// <summary>
        /// Данные зондирования в строку
        /// </summary>
        /// <returns>Таблица в текстовом формате лоя вывода</returns>
        public string ToStr()
        {
            string result = AeroTableElem.GetHeader();

            double? plcl = null;
            double? cape = null;
            double? cin = null;
            double? lftx = null;

            double? muplcl = null;
            double? mucape = null;
            double? mucin = null;
            double? mulftx = null;

            double? mlplcl = null;
            double? mlcape = null;
            double? mlcin = null;
            double? mllftx = null;

            double? mplcl = null;
            double? mcape = null;
            double? mcin = null;
            double? mlftx = null;

            double? kndx = null;
            double? tqndx = null;
            double? ttndx = null;
            double? epndx = null;

            double? tndx = null;

            double? uStorm = null;
            double? vStorm = null;

            double? srh90 = null;
            double? srh255 = null;

            double? bottomLevel = GetBottomLevel();
            double? mostUnstableLevel = GetMostUnstableLevel();

            double? hgt1000 = GetHeight(1000.0);
            double? hgt500 = GetHeight(500.0);

            foreach (AeroTableElem elem in table)
            {
                result += $"{elem.ToStr()}\r\n";
                if (bottomLevel is not null)
                {
                    if (bottomLevel == elem.GetPres())
                    {
                        plcl = elem.GetLiftedCondensationLevel();
                        cape = elem.GetCape();
                        cin = elem.GetCin();
                        lftx = elem.GetLftx();
                    }
                }
                if (mostUnstableLevel is not null)
                {
                    if (mostUnstableLevel == elem.GetPres())
                    {
                        muplcl = elem.GetLiftedCondensationLevel();
                        mucape = elem.GetCape();
                        mucin = elem.GetCin();
                        mulftx = elem.GetLftx();
                    }
                }
            }

            GetML(90.0, out mlplcl, out mlcape, out mlcin, out mllftx);
            GetML(30.0, out mplcl, out mcape, out mcin, out mlftx);

            GetInstabilityIndexes(out kndx, out tqndx, out ttndx, out epndx);

            GetStormMotion(out uStorm, out vStorm);

            if (uStorm is not null && vStorm is not null)
            {
                GetStormRelativeHelicity(90.0, (double)uStorm, (double)vStorm, out srh90);
                GetStormRelativeHelicity(255.0, (double)uStorm, (double)vStorm, out srh255);
            }

            result += "\r\n";
            result += "DIAGNOSTIC VALUES: \r\n";

            result += $"Terrain height [m]: {AeroTableElem.ValToStr(GetTerrainHeight(), 1, false)}\r\n";

            if (hgt1000 != null && hgt500 != null)
            {
                result += $"1000 hPa to 500 hPa thickness[m]: {AeroTableElem.ValToStr((double)hgt500 - (double)hgt1000, 0, false)}\r\n";
            }

            result += $"Precipitable water [mm]: {AeroTableElem.ValToStr(GetPrecipitableWater(), 2, false)}\r\n";

            if (ttndx is not null)
            {
                result += $"Total totals index [K]: {AeroTableElem.ValToStr((double)ttndx, 1, false)}\r\n";
            }

            if (kndx is not null)
            {
                result += $"K index [K]: {AeroTableElem.ValToStr((double)kndx, 1, false)}\r\n";
            }

            if (tqndx is not null)
            {
                result += $"TQ index [K]: {AeroTableElem.ValToStr((double)tqndx, 1, false)}\r\n";
            }

            if (kndx is not null && mlftx is not null)
            {
                result += $"Thompson index [K]: {AeroTableElem.ValToStr((double)kndx - (double)mlftx, 1, false)}\r\n";
            }

            if (epndx is not null)
            {
                result += $"EP index [K]: {AeroTableElem.ValToStr((double)epndx, 1, false)}\r\n";
            }

            if (uStorm is not null && vStorm is not null)
            {
                double stormSpeed = MeteoMath.KnotsToMetersPerSecond(MeteoMath.GetWindSpeed((double)uStorm, (double)vStorm));
                double stormDirection = MeteoMath.GetWindDirection((double)uStorm, (double)vStorm);
                result += $"Storm Motion Speed [m/s]: {AeroTableElem.ValToStr(stormSpeed, 1, false)}\r\n";
                result += $"Storm Motion Direction [deg]: {AeroTableElem.ValToStr(stormDirection, 0, false)}\r\n";
            }

            if (srh90 is not null)
            {
                result += $"0-90 hPa above ground Storm relative helicity [J/kg]: {AeroTableElem.ValToStr(srh90, 2, false)}\r\n";
            }

            if (srh255 is not null)
            {
                result += $"0-255 hPa above ground Storm relative helicity [J/kg]: {AeroTableElem.ValToStr(srh255, 2, false)}\r\n";
            }

            if (mplcl is not null)
            {
                result += $"Pressure of lifted condensation level [hPa]: {AeroTableElem.ValToStr(mplcl, 1, false)}\r\n";
            }

            if (mcape is not null)
            {
                result += $"Convective available potential energy [J/kg]: {AeroTableElem.ValToStr(mcape, 2, false)}\r\n";
            }

            if (mcin is not null)
            {
                result += $"Convective inhibition [J/kg]: {AeroTableElem.ValToStr(mcin, 2, false)}\r\n";
            }

            if (mlftx is not null)
            {
                result += $"Lifted index [C]: {AeroTableElem.ValToStr(mlftx, 2, false)}\r\n";
            }

            if (plcl is not null)
            {
                result += $"Pressure of lifted condensation level (Surface-based) [hPa]: {AeroTableElem.ValToStr(plcl, 1, false)}\r\n";
            }

            if (cape is not null)
            {
                result += $"Surface-based Convective available potential energy [J/kg]: {AeroTableElem.ValToStr(cape, 2, false)}\r\n";
            }

            if (cin is not null)
            {
                result += $"Surface-based Convective inhibition [J/kg]: {AeroTableElem.ValToStr(cin, 2, false)}\r\n";
            }

            if (lftx is not null)
            {
                result += $"Surface-based Lifted index [C]: {AeroTableElem.ValToStr(lftx, 2, false)}\r\n";
            }

            if (mostUnstableLevel is not null)
            {
                result += $"Pressure of most unstable layer [hPa]: {AeroTableElem.ValToStr(mostUnstableLevel, 1, false)}\r\n";
            }

            if (muplcl is not null)
            {
                result += $"Pressure of lifted condensation level (Most Unstable) [hPa]: {AeroTableElem.ValToStr(muplcl, 1, false)}\r\n";
            }

            if (mucape is not null)
            {
                result += $"Most Unstable Convective available potential energy [J/kg]: {AeroTableElem.ValToStr(mucape, 2, false)}\r\n";
            }

            if (mucin is not null)
            {
                result += $"Most Unstable Convective inhibition [J/kg]: {AeroTableElem.ValToStr(mucin, 2, false)}\r\n";
            }

            if (mulftx is not null)
            {
                result += $"Most Unstable Lifted index [C]: {AeroTableElem.ValToStr(mulftx, 2, false)}\r\n";
            }

            if (mlplcl is not null)
            {
                result += $"Pressure of lifted condensation level (Mixed layer) [hPa]: {AeroTableElem.ValToStr(mlplcl, 1, false)}\r\n";
            }

            if (mlcape is not null)
            {
                result += $"Mixed Layer Convective available potential energy [J/kg]: {AeroTableElem.ValToStr(mlcape, 2, false)}\r\n";
            }

            if (mlcin is not null)
            {
                result += $"Mixed layer Convective inhibition [J/kg]: {AeroTableElem.ValToStr(mlcin, 2, false)}\r\n";
            }

            if (mllftx is not null)
            {
                result += $"Mixed layer Lifted index [C]: {AeroTableElem.ValToStr(mllftx, 2, false)}\r\n";
            }

            return result;
        }

        /// <summary>
        /// Наличие уровня (НЕ ВАЛИДЕН)
        /// </summary>
        /// <param name="level">уровень</param>
        /// <returns>Есть такой уровень или нет</returns>
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

        /// <summary>
        /// Получить данные уровня по давлению
        /// </summary>
        /// <param name="level">Уровень</param>
        /// <returns>Данные уровня</returns>
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

        /// <summary>
        /// Получить высоту земной поверхности
        /// </summary>
        /// <returns>Высота [м]</returns>
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

        /// <summary>
        /// Получить значения простых индексов неустойчивости
        /// </summary>
        /// <param name="kndx">K INDEX</param>
        /// <param name="tqndx">TQ INDEX</param>
        /// <param name="ttndx">TT INDEX</param>
        /// <param name="epndx">EP INDEX</param>
        private void GetInstabilityIndexes(out double? kndx, out double? tqndx, out double? ttndx, out double? epndx)
        {
            kndx = null;
            tqndx = null;
            ttndx = null;
            epndx = null;

            AeroTableElem? elem;

            double? tmp850 = null;
            double? td850 = null;
            double? th850 = null;

            elem = GetLevel(850.0);
            if (elem is not null)
            {
                tmp850 = elem.GetTmp();
                td850 = elem.GetDewPoint();
                th850 = elem.GetThetaE();
            }

            double? tmp700 = null;
            double? td700 = null;

            elem = GetLevel(700.0);
            if (elem is not null)
            {
                tmp700 = elem.GetTmp();
                td700 = elem.GetDewPoint();
            }

            double? tmp500 = null;
            double? th500 = null;

            elem = GetLevel(500.0);
            if (elem is not null)
            {
                tmp500 = elem.GetTmp();
                th500 = elem.GetThetaE();
            }

            if (tmp850 is not null && td850 is not null && tmp700 is not null && td700 is not null && tmp500 is not null)
            {
                kndx = tmp850 - tmp500 + td850 - (tmp700 - td700);
            }

            if (tmp850 is not null && td850 is not null && tmp700 is not null)
            {
                tqndx = tmp850 + td850 - 1.7 * tmp700;
            }

            if (th500 is not null && th850 is not null)
            {
                epndx = th500 - th850;
            }

            if (tmp850 is not null && td850 is not null && tmp500 is not null)
            {
                ttndx = tmp850 + td850 - 2 * tmp500;
            }
        }

        /// <summary>
        /// Получить давление на нижнем уровне зондировапния
        /// </summary>
        /// <returns>Давление на нижнем уровне зондирования [гПа]</returns>
        private double? GetBottomLevel()
        {
            double? res = null;
            var el = table.First;
            if (el is not null)
            {
                if (!el.Value.IsNotNullable())
                {
                    el = el.Next;
                }
                res = el.Value.GetPres();
            }
            return res;
        }

        /// <summary>
        /// Получить давление наиболее неустойчивого слоя
        /// </summary>
        /// <returns>Давление наиболее неустойчивого слоя [гПа]</returns>
        private double? GetMostUnstableLevel()
        {
            double? res = null;
            double? maxThetaE = null;
            double? thetaE;
            double? maxUnstableLevel = null;
            double? bottomLevel = GetBottomLevel();
            double? pres = null;
            var el = table.First;
            if (bottomLevel is null)
            {
                return res;
            }
            while (el is not null)
            {
                pres = el.Value.GetPres(); 
                if (pres < bottomLevel - 255.0)
                {
                    break;
                }
                if (pres is null)
                {
                    break;
                }
                if (pres <= bottomLevel)
                {
                    thetaE = el.Value.GetThetaE();
                    if (thetaE is null)
                    {
                        break;
                    }
                    if (maxThetaE is null)
                    {
                        maxThetaE = thetaE;
                        maxUnstableLevel = pres;
                    } 
                    else
                    {
                        if(thetaE >= maxThetaE)
                        {
                            maxUnstableLevel = pres;
                            maxThetaE = thetaE;
                        }
                    }
                }
                el = el.Next;
            }
            return maxUnstableLevel;
        }

        /// <summary>
        /// Получить температуру на уровне (НЕ ВАЛИДНЫЙ)
        /// </summary>
        /// <param name="level">Уровень</param>
        /// <returns>Температура [С]</returns>
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

        /// <summary>
        /// Получить высоту уровня над уровнем моря
        /// </summary>
        /// <param name="level">Уровень</param>
        /// <returns>Высота [м]</returns>
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

        /// <summary>
        /// Получить количество воды в столбе атмосферы (вертикальный интеграл отношения смеси)
        /// </summary>
        /// <returns></returns>
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
                            return result;
                        }
                        result = 0;
                    }
                    if (currentPres < bottomLevel)
                    {
                        if (result is null)
                        {
                            result = 0;
                        }
                        prevMixRat = currentMixRat;
                        currentMixRat = tabElem.GetMixRatio();
                        if (currentMixRat is null)
                        {
                            return result;
                        }
                        result += (0.5 * (currentMixRat + prevMixRat) * (prevPres - currentPres)) / (9.97 * MeteoMath.gravity);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Получить значения параметров поднимающейся частицы
        /// </summary>
        private void SetGammaW()
        {
            double? bottomLevel = GetBottomLevel();
            foreach (AeroTableElem elem in table)
            {
                double? temperature = elem.GetTmp();
                double? dewpoint = elem.GetDewPoint();
                double? pressure = elem.GetPres();
                double? mixratio = elem.GetMixRatio();
                double? hgt = elem.GetHgt();

                if (bottomLevel is null)
                {
                    break;
                }

                if (pressure is null)
                {
                    break;
                }
                if (pressure < 100)
                {
                    break;
                }
                else
                {
                    if (bottomLevel < pressure)
                    {
                        continue;
                    }
                }

                double? plcl;
                double? cape;
                double? cin;
                double? lftx;
                if (
                    temperature is null
                    || dewpoint is null
                    || pressure is null
                    || mixratio is null
                    || hgt is null
                )
                {
                    break;
                }
                GammaW((double)temperature, (double)dewpoint, (double)pressure, (double)mixratio, (double)hgt, out plcl, out cape, out cin, out lftx);
                elem.SetGammaW(plcl, cape, cin, lftx);
            }
        }

        /// <summary>
        /// Получить компоненты скорости ведущего потока
        /// </summary>
        /// <param name="u">u-компонента</param>
        /// <param name="v">v-компонента</param>
        private void GetStormMotion(out double? u, out double? v)
        {
            u = null;
            v = null;

            double pBottom = 850.0;
            double pTop = 300.0;

            var elem = table.First;
            if (elem is null)
            {
                return;
            }

            double? tempP = elem.Value.GetPres();
            if (tempP is null)
            {
                return;
            }
            if (tempP > GetBottomLevel())
            {
                elem = elem.Next;
                if (elem is null)
                {
                    return;
                }
                tempP = elem.Value.GetPres();
                if (tempP is null)
                {
                    return;
                }
            }

            double bottomPressure = (double)tempP;
            double p = bottomPressure;

            double? prevP = p;
            double? nextP = p;
            double? prevS = elem.Value.GetSpeed();
            double? prevD = elem.Value.GetDirection();
            double? nextS = prevS;
            double? nextD = prevD;

            if (prevS is null || prevD is null || nextS is null || nextD is null)
            {
                return;
            }

            double avgU = 0;
            double avgV = 0;

            int i = 0;
            while (p >= pTop)
            {
                if (p <= pBottom)
                {
                    i += 1;
                    double speed = (prevP != nextP) ? MeteoMath.InterpolationZ((double)prevP, (double)nextP, (double)prevS, (double)nextS, p) : (double)nextS;
                    double direction = (prevP != nextP) ? MeteoMath.InterpolationZ((double)prevP, (double)nextP, (double)prevD, (double)nextD, p) : (double)nextD;

                    avgU += MeteoMath.GetU(speed, direction);
                    avgV += MeteoMath.GetV(speed, direction);
                }

                if (p <= nextP)
                {
                    elem = elem.Next;
                    if (elem is null)
                    {
                        return;
                    }
                    prevS = nextS;
                    prevD = nextD;
                    prevP = nextP;
                    nextS = elem.Value.GetSpeed();
                    nextD = elem.Value.GetDirection();
                    nextP = elem.Value.GetPres();
                    if (nextD is null || nextS is null || nextP is null)
                    {
                        return;
                    }
                }

                p -= 0.1;
            }

            avgU = avgU / i;
            avgV = avgV / i;

            double speedMean = MeteoMath.GetWindSpeed(avgU, avgV);
            double directionMean = MeteoMath.GetWindDirection(avgU, avgV);

            double reduct = 0.20;
            double rotate = 20.0;

            if (speedMean < 15.0)
            {
                reduct = 0.25;
                rotate = 30.0;
            }

            double uReduce = (1 - reduct) * avgU;
            double vReduce = (1 - reduct) * avgV;

            double stormSpeed = MeteoMath.GetWindSpeed(uReduce, vReduce);
            double stormDirection = MeteoMath.GetWindDirection(uReduce, vReduce) + rotate;

            stormDirection = (stormDirection > 360.0) ? 360.0 - stormDirection : stormDirection;

            u = MeteoMath.GetU(stormSpeed, stormDirection);
            v = MeteoMath.GetV(stormSpeed, stormDirection);
        }

        /// <summary>
        /// Получить значение относительной завихренности (вертикальный интеграл векторного произведения векторов ветра и ведущего потока)
        /// </summary>
        /// <param name="depth">Глубина интегрирования</param>
        /// <param name="stormU">u-компонента скорости ведущего потока</param>
        /// <param name="stormV">v-компонента скорости ведущего потока</param>
        /// <param name="helicity">Значение относительной завихренности (Дж\кг)</param>
        private void GetStormRelativeHelicity(double depth, double stormU, double stormV, out double? helicity)
        {
            helicity = null;

            var elem = table.First;
            if (elem is null)
            {
                return;
            }

            double? tempP = elem.Value.GetPres();
            if (tempP is null)
            {
                return;
            }
            if (tempP > GetBottomLevel())
            {
                elem = elem.Next;
                if (elem is null)
                {
                    return;
                }
                tempP = elem.Value.GetPres();
                if (tempP is null)
                {
                    return;
                }
            }

            double bottomPressure = (double)tempP;
            double p = bottomPressure;

            double? prevP = p;
            double? nextP = p;
            double? prevS = elem.Value.GetSpeed();
            double? prevD = elem.Value.GetDirection();
            double? nextS = prevS;
            double? nextD = prevD;

            if (prevS is null || prevD is null || nextS is null || nextD is null)
            {
                return;
            }

            double srh = 0;
            double? oldU = null;
            double? oldV = null;

            double stormUms = MeteoMath.KnotsToMetersPerSecond(stormU);
            double stormVms = MeteoMath.KnotsToMetersPerSecond(stormV);

            while (p >= (bottomPressure - depth))
            {
                double speed = (prevP != nextP) ? MeteoMath.InterpolationZ((double)prevP, (double)nextP, (double)prevS, (double)nextS, p) : (double)nextS;
                double direction = (prevP != nextP) ? MeteoMath.InterpolationZ((double)prevP, (double)nextP, (double)prevD, (double)nextD, p) : (double)nextD;

                double u = MeteoMath.GetU(MeteoMath.KnotsToMetersPerSecond(speed), direction);
                double v = MeteoMath.GetV(MeteoMath.KnotsToMetersPerSecond(speed), direction);

                if (oldU is not null && oldV is not null)
                {
                    double du = u - (double)oldU;
                    double dv = v - (double)oldV;
                    double ubar = 0.5 * (u + (double)oldU);
                    double vbar = 0.5 * (v + (double)oldV);
                    double uSRH = -1.0 * dv * (ubar - stormUms);
                    double vSRH = du * (vbar - stormVms);
                    srh = srh + uSRH + vSRH;
                }

                oldU = u;
                oldV = v;

                if (p <= nextP)
                {
                    elem = elem.Next;
                    if (elem is null)
                    {
                        return;
                    }
                    prevS = nextS;
                    prevD = nextD;
                    prevP = nextP;
                    nextS = elem.Value.GetSpeed();
                    nextD = elem.Value.GetDirection();
                    nextP = elem.Value.GetPres();
                    if (nextD is null || nextS is null || nextP is null)
                    {
                        return;
                    }
                }
                p -= 0.1;
            }

            helicity = srh;
        }

        /// <summary>
        /// Получение характеристик плавучести для перемешанного слоя
        /// </summary>
        /// <param name="depth">Глубина перемешивания</param>
        /// <param name="plcl">Давление на уровне конденсации</param>
        /// <param name="cape">Потенциальная доступная энергия неустойчивости</param>
        /// <param name="cin">Энергия задерживающего слоя</param>
        /// <param name="lftx">Индекс плавучести</param>
        public void GetML(double depth, out double? plcl, out double? cape, out double? cin, out double? lftx)
        {
            plcl = null;
            cape = null;
            cin = null;
            lftx = null;

            var elem = table.First;
            if (elem is null)
            {
                return;
            }

            double? tempP = elem.Value.GetPres();
            if (tempP is null)
            {
                return;
            }
            if (tempP > GetBottomLevel())
            {
                elem = elem.Next;
                if (elem is null)
                {
                    return;
                }
                tempP = elem.Value.GetPres();
                if (tempP is null)
                {
                    return;
                }
            }

            double bottomPressure = (double)tempP;
            double p = bottomPressure;

            double? prevP = p;
            double? nextP = p;
            double? prevT = elem.Value.GetTmp();
            double? prevH = elem.Value.GetHgt();
            double? prevM = elem.Value.GetMixRatio();
            double? nextT = prevT;
            double? nextH = prevH;
            double? nextM = prevM;

            if (prevT is null || prevM is null || nextT is null || nextH is null || nextM is null || nextH is null)
            {
                return;
            }

            double mlmr = 0;
            double mlt = 0;
            double? hgt = null;
            double center = bottomPressure - depth * 0.5;

            int i = 0;
            while (p >= (bottomPressure - depth))
            {
                i += 1;
                mlt += (prevP != nextP) ? MeteoMath.InterpolationZ((double)prevP, (double)nextP, (double)prevT, (double)nextT, p) : (double)nextT;
                mlmr += (prevP != nextP) ? MeteoMath.InterpolationZ((double)prevP, (double)nextP, (double)prevM, (double)nextM, p) : (double)nextM;

                if (hgt is null && p <= center)
                {
                    hgt = (prevP != nextP) ? MeteoMath.InterpolationZ((double)prevP, (double)nextP, (double)prevH, (double)nextH, p) : (double)nextT;
                    if (hgt is null)
                    {
                        return;
                    }
                }

                if (p <= nextP)
                {
                    elem = elem.Next;
                    if (elem is null)
                    {
                        return;
                    }
                    prevT = nextT;
                    prevM = nextM;
                    prevP = nextP;
                    prevH = nextH;
                    nextT = elem.Value.GetTmp();
                    nextM = elem.Value.GetMixRatio();
                    nextP = elem.Value.GetPres();
                    nextH = elem.Value.GetHgt();
                    if (nextT is null || nextM is null || nextP is null || nextH is null)
                    {
                        return;
                    }
                }
                p -= 0.1;
            }

            if (hgt is null)
            {
                return;
            }

            mlt = mlt / i;
            mlmr = mlmr / i;
            double humidity = MeteoMath.RelativeHumidity(mlt, center, mlmr);
            double dewpoint = MeteoMath.DewPoint(mlt, humidity);
            GammaW(mlt, dewpoint, center, mlmr, (double)hgt, out plcl, out cape, out cin, out lftx);
        }

        /// <summary>
        /// Расчет параметров плавучести (вертикалдьное интегрирование разности температуры окружающего воздуха и поднявшейся частицы)
        /// </summary>
        /// <param name="temperature">Температура</param>
        /// <param name="dewpoint">Температура точки росы</param>
        /// <param name="pressure">Давление</param>
        /// <param name="mixratio">Отношение смеси</param>
        /// <param name="hgt">Высота</param>
        /// <param name="plcl">Давление на уровне конденсации</param>
        /// <param name="cape">Потенциальная доступная энергия неустойчивости</param>
        /// <param name="cin">Энергия задерживающего слоя</param>
        /// <param name="lftx">Индекс плавучести</param>
        private void GammaW(double temperature, double dewpoint, double pressure, double mixratio, double hgt, out double? plcl, out double? cape, out double? cin, out double? lftx)
        {
            plcl = null;
            cape = null;
            cin = null;
            lftx = null;

            double p = pressure;
            double? nextP;

            double? tempP;

            plcl = MeteoMath.GetLiftedCondensationLevelPressure(temperature, dewpoint, pressure);
            double tlcl = MeteoMath.GetLiftedCondensationLevelTemperature(temperature, dewpoint);
            if (plcl is null)
            {
                return;
            }
            double gradient = 0.1 * (temperature - tlcl) / (pressure - (double)plcl);

            if (p <= plcl)
            {
                gradient = 10 * MeteoMath.GammaW(temperature, pressure - 0.05, 100.0);
            }

            var elem = table.First;
            if (elem is null)
            {
                return;
            }

            tempP = elem.Value.GetPres();
            if (tempP is null)
            {
                return;
            }
            if (tempP < GetBottomLevel())
            {
                elem = elem.Next;
                if (elem is null)
                {
                    return;
                }
                tempP = elem.Value.GetPres();
                if (tempP is null)
                {
                    return;
                }
            }

            while (tempP >= p)
            {
                elem = elem.Next;
                if (elem is null)
                {
                    return;
                }
                if (!elem.Value.IsNotNullable())
                {
                    return;
                }
                tempP = elem.Value.GetPres();
                if (tempP is null)
                {
                    return;
                }
            }
            nextP = tempP;

            double? prevP = p;
            double? prevT = temperature;
            double? prevM = mixratio;
            double? prevH = hgt;
            double t = temperature;
            double? tEnv = null;
            double? mEnv = null;

            double? nextT = elem.Value.GetTmp();
            double? nextM = elem.Value.GetMixRatio();
            double? nextH = elem.Value.GetHgt();

            if (nextT is null || nextM is null || nextH is null)
            {
                return;
            }

            cape = 0;
            cin = 0;
            double cinTmp = 0;

            bool hasCape = false;
            double tVirtualDiffOld = 0;
            double tVirtualDiff = 0;

            double capePart = 0;
            double capePartOld = 0;

            while (p >= 100.0)
            {
                t -= gradient;

                double tVirtParcel;
                if (p < plcl)
                {
                    tVirtParcel = MeteoMath.Virtual2(t + 273.16, t + 273.16, p);
                }
                else
                {
                    tVirtParcel = MeteoMath.Virtual2(t + 273.16, dewpoint + 273.16, p);
                }

                double tEnvInterp = MeteoMath.InterpolationZ((double)prevP, (double)nextP, (double)prevT, (double)nextT, p);
                double mEnvInterp = MeteoMath.InterpolationZ((double)prevP, (double)nextP, (double)prevM, (double)nextM, p);
                double tVirtEnv = MeteoMath.Virtual(tEnvInterp + 273.16, mEnvInterp / 1000);

                tVirtualDiffOld = tVirtualDiff;
                tVirtualDiff = tVirtParcel - tVirtEnv;

                capePartOld = capePart;
                capePart = tVirtualDiff / tVirtEnv;
                double hInterp = MeteoMath.InterpolationZ((double)prevP, (double)nextP, (double)prevH, (double)nextH, p);
                double hPrevInterp = MeteoMath.InterpolationZ((double)prevP, (double)nextP, (double)prevH, (double)nextH, p + 0.1);

                if (capePart > 0)
                {                   
                    cape += MeteoMath.gravity * 0.5 * (capePartOld + capePart) * (0.1*(hInterp - hPrevInterp));
                    if (!hasCape)
                    {
                        hasCape = true;
                        cin = cinTmp;
                    }
                } 
                else if (capePart <= 0 || !hasCape)
                {
                    cinTmp += MeteoMath.gravity * 0.5 * (capePartOld + capePart) * (0.1 * (hInterp - hPrevInterp));
                }

                if (lftx is null && p <= 500.0 && p >= 499.9)
                {
                    lftx = -1.0 * tVirtualDiff;
                }

                if (p <= nextP)
                {
                    elem = elem.Next;
                    if (elem is null)
                    {
                        return;
                    }
                    prevT = nextT;
                    prevM = nextM;
                    prevP = nextP;
                    nextT = elem.Value.GetTmp();
                    nextM = elem.Value.GetMixRatio();
                    nextP = elem.Value.GetPres();
                    nextH = elem.Value.GetHgt();
                    if (nextT is null || nextM is null || nextP is null || nextH is null)
                    {
                        return;
                    }
                }

                if (p <= plcl)
                {
                    gradient = 10 * MeteoMath.GammaW(t, p - 0.05, 100.0);
                }

                p -= 0.1;
            }
        }
        #endregion
    }
}
