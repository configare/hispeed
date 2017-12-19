using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public static class AreaCountHelper
    {
        /// <summary>
        /// 地球半径,单位：千米
        /// </summary>
        public const double cstEarthRadius = 6378.137;

        public static double CalcArea(double lonResolution, double latResolution)
        {
            return ComputeSpheralSurfaceDistance(110, 35, 110 + lonResolution, 35) *
                   ComputeSpheralSurfaceDistance(110, 35, 110, 35 + latResolution);
        }

        /// <summary>
        /// 面积：单位平方米
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="lonResolution"></param>
        /// <param name="latResolution"></param>
        /// <returns></returns>
        public static double CalcArea(double lon,double lat, double lonResolution, double latResolution)
        {
            return ComputeSpheralSurfaceDistance(lon, lat, lon + lonResolution, lat) *
                   ComputeSpheralSurfaceDistance(lon, lat, lon, lat + latResolution);
        }

        /// <summary>
        /// 计算球面上两点之间的距离
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lnn2"></param>
        /// <param name="lat2"></param>
        /// <returns>返回距离，单位:米</returns>
        public static double ComputeSpheralSurfaceDistance(double lon1, double lat1, double lon2, double lat2)
        {
            double radLat1 = DegreeToRadian(lat1);
            double radLat2 = DegreeToRadian(lat2);
            double a = radLat1 - radLat2;
            double b = DegreeToRadian(lon1) - DegreeToRadian(lon2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * cstEarthRadius;
            s = Math.Round(s * 10000) / 10000 * 1000;
            return s;
        }

        /// <summary>
        /// 角度转换为弧度
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180.0;
        }
    }
}
