using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace CodeCell.Bricks.Runtime
{
    public class LonOrLatRangeByASC
    {
        public double MinCoord = 0;
        public double MaxCoord = 0;

        public LonOrLatRangeByASC(double minLongitude, double maxLongitude)
        {
            MinCoord = minLongitude;
            MaxCoord = maxLongitude;
        }

        public double Width
        {
            get
            {
                return MaxCoord - MinCoord;
            }
        }

        public bool Contains(double coord)
        {
            return coord >= MinCoord && coord <= MaxCoord;
        }

        public double Cols(double resolution)
        {
            return Width / resolution;
        }

        public double GetColOrRow(double resolution, double coord)
        {
            return (coord - MinCoord) / resolution;
        }
    }

    public static class GeoCoordHelper
    {
        public const double ToleranceOfDouble = 0.00000000001d;

        public static void GetCenterPoint(double leftTopLon, 
                                       double rightBottomLon, 
                                       out double centerLon)
        {
            centerLon =  0;
            //longitude
            if (leftTopLon >= rightBottomLon)
            {
                if (leftTopLon > 0)//leftTopLon is positive,rightBottomLon is negative
                    centerLon = leftTopLon + ((180 - leftTopLon) + (180 - Math.Abs(rightBottomLon))) / 2d;
                else//only ,both is negative,over 180
                    centerLon = leftTopLon + (Math.Abs(leftTopLon) + 180 + (180 - Math.Abs(rightBottomLon))) / 2d;
            }
            else
            {
                centerLon = (leftTopLon + rightBottomLon) / 2d;
            }
        }

        public static void GetCenterPoint(double leftTopLon, double leftTopLat,
                                          double rightBottomLon, double rightBottomLat,
                                          out double centerLon, out double centerLat)
        {
            centerLon = centerLat = 0;
            //longitude
            if (leftTopLon >= rightBottomLon)
            {
                if (leftTopLon > 0)//leftTopLon is positive,rightBottomLon is negative
                    centerLon = leftTopLon + ((180 - leftTopLon) + (180 - Math.Abs(rightBottomLon))) / 2d;
                else//only ,both is negative,over 180
                    centerLon = leftTopLon + (Math.Abs(leftTopLon) + 180 + (180 - Math.Abs(rightBottomLon))) / 2d;
            }
            else
            {
                centerLon = (leftTopLon + rightBottomLon) / 2d;
            }
            //latitude
            centerLat = (leftTopLat + rightBottomLat) / 2d;
        }

        public static void GetPositiveRange(double leftLongitude, double rightLongitude,out double outRightLongitude)
        {
            if (leftLongitude < 0 && rightLongitude < 0)
                outRightLongitude = 180 + 180 -Math.Abs(rightLongitude);
            else if(leftLongitude>=0 && rightLongitude<=0)
                outRightLongitude = (180 - leftLongitude) + Math.Abs(rightLongitude);
            else
                outRightLongitude = rightLongitude;
        }

        public static void GetBoundaryAndPositiveRangeOfLatitude(double centerLat, double latWidth,
                                                                  out double minLatitude, out double maxLatitude,
                                                                  out LonOrLatRangeByASC[] latRangeByASC)
        {
            latRangeByASC = null;
            minLatitude = maxLatitude = 0;
            if (latWidth > 180 + ToleranceOfDouble)
                return;
            double lon = latWidth / 2d;
            minLatitude = centerLat - lon;
            maxLatitude = centerLat + lon;
            if (maxLatitude > 90)
            {
                maxLatitude = maxLatitude - 90 - 90;
            }
            //
            List<LonOrLatRangeByASC> ranges = new List<LonOrLatRangeByASC>();
            if (minLatitude < maxLatitude)
            {
                ranges.Add(new LonOrLatRangeByASC(minLatitude, maxLatitude));
            }
            else
            {
                if (minLatitude < 0)
                {
                    ranges.Add(new LonOrLatRangeByASC(minLatitude, 0));
                    ranges.Add(new LonOrLatRangeByASC(0, 90));
                    ranges.Add(new LonOrLatRangeByASC(-90, maxLatitude));
                }
                else
                {
                    ranges.Add(new LonOrLatRangeByASC(minLatitude, 90));
                    ranges.Add(new LonOrLatRangeByASC(-90, maxLatitude));
                }
            }
            latRangeByASC = ranges.ToArray();
        }

        public static void GetBoundaryAndPositiveRangeOfLongitude(double centerLon, double lonWidth, 
                                                                                                  out double leftTopLon, out double rightBottomLon,
                                                                                                  out LonOrLatRangeByASC[] lonRangeByASC)
        {
            lonRangeByASC = null;
            leftTopLon = rightBottomLon = 0;
            if (lonWidth > 360 + ToleranceOfDouble )
                return;
            double lon = lonWidth / 2d;
            leftTopLon = centerLon - lon;
            rightBottomLon = centerLon + lon;
            if (rightBottomLon > 180)
            {
                rightBottomLon = rightBottomLon - 180 -180;
            }
            //
            List<LonOrLatRangeByASC> ranges = new List<LonOrLatRangeByASC>();
            if (leftTopLon < rightBottomLon)
            {
                ranges.Add(new LonOrLatRangeByASC(leftTopLon, rightBottomLon));
            }
            else
            {
                if (leftTopLon < 0)
                {
                    ranges.Add(new LonOrLatRangeByASC(leftTopLon, 0));
                    ranges.Add(new LonOrLatRangeByASC(0, 180));
                    ranges.Add(new LonOrLatRangeByASC(-180, rightBottomLon));
                }
                else
                {
                    ranges.Add(new LonOrLatRangeByASC(leftTopLon, 180));
                    ranges.Add(new LonOrLatRangeByASC(-180, rightBottomLon));
                }
            }
            lonRangeByASC = ranges.ToArray();
        }

        public static int ComputeRow(double minLatitude, double maxLatitude, int height, double latitude)
        {
            if (latitude < minLatitude && latitude > maxLatitude)
                return -1;
            double resolution = (maxLatitude - minLatitude) / height;
            return (int)((latitude - minLatitude) / resolution);
        }

        public static int ComputeCol(LonOrLatRangeByASC[] ranges, int width, double longitude)
        {
            double span = 0;
            foreach (LonOrLatRangeByASC r in ranges)
                span += r.Width;
            //
            double resolution = span / width;
            double cols = 0;
            foreach (LonOrLatRangeByASC r in ranges)
            {
                if (r.Contains(longitude))
                {
                    cols += r.GetColOrRow(resolution, longitude);
                    return (int)cols;
                }
                else
                {
                    cols += r.Cols(resolution);
                }
            }
            return -1;
        }

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
            return d.ToString().PadLeft(3,' ') + "°" + m.ToString().PadLeft(2,' ') + "′" + s.ToString("#.##").PadLeft(4,' ') + "″";
        }

        public static PointF ExtractGeoCoord(string sValue)
        { 
            string RegExpDegreeMinuteSecond = @"^[+|-]?\d+(.)?\d+(°)\d+(.)?\d+(′)\d+(.)?\d+(″)\s[+|-]?\d+(.)?\d+(°)\d+(.)?\d+(′)\d+(.)?\d+(″)$";
            string RegExpDecimalDegres = @"^[+|-]?\d+((.)?\d+)?\s[+|-]?\d+((.)?\d+)?$";
            if (Regex.IsMatch(sValue, RegExpDegreeMinuteSecond))
            {
                string[] LonLat = sValue.Split(new char[1] { ' ' });
                double x = GetDecimalDegree(LonLat[0]);
                double y = GetDecimalDegree(LonLat[1]);
                return new PointF((float)x, (float)y);
            }
            else if (Regex.IsMatch(sValue, RegExpDecimalDegres))
            {
                string[] LonLat = sValue.Split(new char[1] { ' ' });
                return new PointF(float.Parse(LonLat[0]), float.Parse(LonLat[1]));
            }
            return PointF.Empty;
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
