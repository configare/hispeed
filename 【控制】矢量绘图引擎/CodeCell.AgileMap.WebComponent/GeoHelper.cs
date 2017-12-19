using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace CodeCell.AgileMap.WebComponent
{
    public static class GeoHelper
    {
        public static string DegreeToString(double degree)
        {
            if (degree > 1000)
                return degree.ToString();
            degree = Math.Abs(degree);
            int d = Math.Abs((int)Math.Floor(degree));
            int m = Math.Abs((int)((degree - d) * 60));
            if (m > 60)
                m = 60;
            int s = Math.Abs((int)((degree * 3600 - d * 3600 - m * 60)));
            return d.ToString().PadLeft(3, ' ') + "°" + m.ToString().PadLeft(2, ' ') + "′" + s.ToString("#.##").PadLeft(4, ' ') + "″";
        }

        public static bool ExtractGeoCoord(string sValue,out Point geoPt)
        {
            geoPt = new Point();
            if (string.IsNullOrEmpty(sValue))
                return false;
            sValue = sValue.Trim();
            string RegExpDegreeMinuteSecond = @"^[+|-]?\d+(.)?\d+(°)\d+(.)?\d+(′)\d+(.)?\d+(″)\s[+|-]?\d+(.)?\d+(°)\d+(.)?\d+(′)\d+(.)?\d+(″)$";
            string RegExpDecimalDegres = @"^[+|-]?\d+((.)?\d+)?\s[+|-]?\d+((.)?\d+)?$";
            if (Regex.IsMatch(sValue, RegExpDegreeMinuteSecond))
            {
                string[] LonLat = sValue.Split(new char[1] { ' ' });
                double x = GetDecimalDegree(LonLat[0]);
                double y = GetDecimalDegree(LonLat[1]);
                geoPt =  new Point(x, y);
                return true;
            }
            else if (Regex.IsMatch(sValue, RegExpDecimalDegres))
            {
                string[] LonLat = sValue.Split(new char[1] { ' ' });
                geoPt = new Point(double.Parse(LonLat[0]), double.Parse(LonLat[1]));
                return true;
            }
            return false;
        }

        public static char charDegree = '°';
        public static char charMinutes = '′';
        public static char charSeconds = '″';
        public static double GetDecimalDegree(string sDegreeMinuteSecond)
        {
            string[] ps = sDegreeMinuteSecond.Split(new char[1] { charDegree });
            double d = double.Parse(ps[0]);
            //
            ps = ps[1].Split(new char[1] { charMinutes });
            double m = double.Parse(ps[0]);
            //
            ps = ps[1].Split(new char[1] { charSeconds });
            double s = double.Parse(ps[0]);

            return d + m / 60d + s / 3600d;
        }
    }
}
