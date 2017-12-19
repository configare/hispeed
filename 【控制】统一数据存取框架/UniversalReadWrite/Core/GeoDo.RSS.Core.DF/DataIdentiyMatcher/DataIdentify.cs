using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class DataIdentify
    {
        public string Satellite;
        public string Sensor;
        public bool IsOrbit;
        public bool IsProduct;
        public string ProductIdentify;
        public string SubProductIdentify;
        public DateTime OrbitDateTime;
        /// <summary>
        /// 是否为升轨：
        /// false：降轨Desc【默认值】
        /// true：升轨Asc
        /// </summary>
        public bool IsAscOrbitDirection = false;//
        /// <summary>
        /// 白天或晚上
        /// </summary>
        public enumDayOrNight DayOrNight = enumDayOrNight.Null;

        public DataIdentify()
        {
        }

        public DataIdentify(string satellite, string sensor)
        {
            Satellite = satellite;
            Sensor = sensor;
        }

        /// <summary>
        /// 拷贝非空值
        /// </summary>
        /// <param name="targetDataIdentify"></param>
        public void CopyAttributesToIfNull(DataIdentify targetDataIdentify)
        {
            if (string.IsNullOrEmpty(targetDataIdentify.Satellite))
                targetDataIdentify.Satellite = Satellite;
            if (string.IsNullOrEmpty(targetDataIdentify.Sensor))
                targetDataIdentify.Sensor = Sensor;
            if (DateTime.MinValue == targetDataIdentify.OrbitDateTime)
                targetDataIdentify.OrbitDateTime = OrbitDateTime;
            if (targetDataIdentify.DayOrNight == enumDayOrNight.Null)
                targetDataIdentify.DayOrNight = DayOrNight;
        }
    }

    public enum enumDayOrNight
    {
        Null,
        Day,
        Night
    }
}
