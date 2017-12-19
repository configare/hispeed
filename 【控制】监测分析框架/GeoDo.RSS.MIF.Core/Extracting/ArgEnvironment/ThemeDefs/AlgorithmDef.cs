using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 判识算法
    /// </summary>
    public class AlgorithmDef
    {
        /// <summary>
        /// 算法名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 算法标识
        /// </summary>
        public string Identify;
        /// <summary>
        /// 该算法支持的卫星类型
        /// </summary>
        public string[] Satellites;
        /// <summary>
        /// 该算法支持的传感器类型
        /// </summary>
        public string[] Sensors;
        /// <summary>
        /// 用户标识，如沙尘算法中的“海洋、陆地”等
        /// </summary>
        public string CustomIdentify;
        /// <summary> 
        /// 分组标志，不为空，算法根据组名称进行分tab页显示
        /// </summary>
        public string GroupTypeName="默认组";
        /// <summary>
        /// 该算法中需要用到的波段
        /// </summary>
        public BandDef[] Bands;
        /// <summary>
        /// 该算法中需要用到的参数
        /// </summary>
        public ArgumentBase[] Arguments;

        /// <summary>
        /// 通过标识获取波段号
        /// </summary>
        /// <param name="identify"></param>
        /// <returns></returns>
        public int GetBandNoByIdentify(string identify)
        {
            if (Bands == null || Bands.Length == 0)
                return -1;
            for (int i = 0; i < Bands.Length; i++)
            {
                if (Bands[i].Identify.ToLower().Trim() == identify.ToLower().Trim())
                    return Bands[i].BandNo;
            }
            return -1;
        }

        /// <summary>
        /// 通过标识获取波段缩放因子
        /// </summary>
        /// <param name="identify"></param>
        /// <returns></returns>
        public double GetBandZoomByIdentify(string identify)
        {
            if (Bands == null || Bands.Length == 0)
                return -1;
            for (int i = 0; i < Bands.Length; i++)
            {
                if (Bands[i].Identify.ToLower().Trim() == identify.ToLower().Trim())
                    return Bands[i].Zoom;
            }
            return -1;
        }
    }
}
