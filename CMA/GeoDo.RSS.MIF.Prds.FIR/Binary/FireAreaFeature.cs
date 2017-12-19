using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    internal class FireAreaFeature
    {
        private const string INFO_FORMAT =
            "    火区号 :  {0}\n" +
            "    经度 :  {1}\n" +
            "    纬度 :  {2}\n" +
            "    火点个数 :  {3}\n" +
            "    像元面积 :  {4}\n" +
            "    亚像元面积 :  {5}\n" +
            "    县界名 :  {6}\n" +
            "    农田百分比 :  {7}\n" +
            "    林地百分比 :  {8}\n" +
            "    草地百分比 :  {9}\n" +
            "    其他百分比 :  {10}\n";
        /// <summary>
        /// 火区号
        /// </summary>
        public int FireReaIndex = 0;
        /// <summary>
        /// 经度
        /// </summary>
        public float Longitude;
        /// <summary>
        /// 纬度
        /// </summary>
        public float Latitude;
        /// <summary>
        /// 火点数
        /// </summary>
        public int FireCount = 0;
        /// <summary>
        /// 火区面积
        /// </summary>
        public float FireArea;
        /// <summary>
        /// 火区亚像元面积
        /// </summary>
        public float SecondryFireArea;
        /// <summary>
        /// 县界名
        /// </summary>
        public string XJName;
        /// <summary>
        /// 农田比分比
        /// </summary>
        public float FarmlandPercent;
        /// <summary>
        /// 林地百分比
        /// </summary>
        public float WoodlandPercent;
        //草地百分比
        /// <summary>
        /// 
        /// </summary>
        public float GrasslandPercent;
        /// <summary>
        /// 其他百分比
        /// </summary>
        public float OtherPercent;
        /// <summary>
        /// 农田个数
        /// </summary>
        public float FarmlandCount;
        /// <summary>
        /// 林地个数
        /// </summary>
        public float WoodlandCount;
        /// <summary>
        /// 草地个数
        /// </summary>
        public float GrasslandCount;
        /// <summary>
        /// 其他个数
        /// </summary>
        public float OtherCount;
        /// <summary>
        /// 火区火点索引值
        /// </summary>
        public List<int> FireIndeies;
        /// <summary>
        /// 省界名
        /// </summary>
        public string SJName;
        /// <summary>
        /// 市界名
        /// </summary>
        public string ShiName;


        public override string ToString()
        {
            return string.Format(INFO_FORMAT,
                 FireReaIndex,
                 Longitude,
                 Latitude,
                 FireCount,
                 FireArea,
                 SecondryFireArea,
                 string.IsNullOrEmpty(XJName) ? "\\" : XJName,
                 FarmlandPercent * 100 + "%",
                 WoodlandPercent * 100 + "%",
                 GrasslandPercent * 100 + "%",
                 OtherPercent * 100 + "%");
        }
    }
}
