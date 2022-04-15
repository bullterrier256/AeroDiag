using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AeroDiag
{
    internal class WebGetter
    {
        public static bool TryGetAeroData(string point, string dateTime, out string result)
        {
            bool ok = false;
            ok = getTextData(point, dateTime, out result);
            return ok;
        }

        private static bool getTextData (string point, string dateTime, out string result)
        {
            bool ok = false;
            string year = dateTime.Substring(0, 4);
            string month = dateTime.Substring(4, 2);
            string dayHour = dateTime.Substring(6);
            result = "";
            string query = $"http://weather.uwyo.edu/cgi-bin/sounding?region=np&TYPE=TEXT%3ALIST&YEAR={year}&MONTH={month}&FROM={dayHour}&TO={dayHour}&STNM={point}";
            try
            {
                HtmlWeb web = new HtmlWeb();
                var htmlDoc = web.Load(query);

                if (htmlDoc.Text == null)
                    throw new Exception("Данные пустые");
                string html = htmlDoc.Text;
                if (!html.Contains("<PRE>") || !html.Contains("</PRE>"))
                    throw new Exception("Нет данных для считывания");
                int firstChar = html.IndexOf("<PRE>") + 5;
                AeroTable tab = new AeroTable(html.Substring(html.IndexOf("<PRE>") + 5, html.IndexOf("</PRE>") - firstChar));
                result = tab.ToStr();
                ok = true;
            }
            catch (Exception e)
            {
                result = $"Error: {e.Message}, source:{e.Source}, trace:{e.StackTrace}";
            }
            return ok;
        }
    }
}
