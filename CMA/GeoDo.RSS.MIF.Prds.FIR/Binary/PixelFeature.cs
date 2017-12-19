using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    internal class PixelFeature
    {
        private const string INFO_FORMAT =
            "    是否疑似像元 :  {0}\n" +
            "    是否确认火点 :  {1}\n" +
            "    中红外均值 :  {2}\n" +
            "    远红外均值 :  {3}\n" +
            "    (中红外-远红外)均值 :  {4}\n" +
            "    中红外标准差 :  {5}\n" +
            "    远红外标准差 :  {6}\n" +
            "    (中红外-远红外)标准差 :  {7}\n" +
            "    亚像元面积 :  {8}\n" +
            "    像元面积 :  {9}\n" +
            "    火点强度 :  {10}\n" +
            "    火点强度等级 :  {11}\n" +
            "    火点可信度 :  {12}\n" +
            "    火区号 :  {13}";
        //像元索引号
        public int PixelIndex = 0;
        //是疑似火点
        public bool IsDoubtFirPixel = false;
        //是确认火点
        public bool IsVertified = false;
        //中红外均值
        public float MidIfrAvgValue;
        //远红外均值
        public float FarIfrAvgValue;
        public float MidIfr_FarIfr_Diff_AvgValue;
        //中红外标准差
        public float MidIfr_StdDev;
        //远红外标准差
        public float FarIfr_StdDev;
        //(中红外-远红外)亮温差标准差
        public float MidIfr_FarIfr_Diff_StdDev;
        //亚像元面积
        public float SecondPixelArea;
        //火点强度
        public float FirIntensity;
        //火点强度等级
        public int FireIntensityGrade;
        //火点可信度
        public int FirReliability;
        //像元面积
        public float PixelArea;
        //火区号
        public int FireAreaNum = -1;
        //是否经过云检测
        public bool CloudChecked = false;
        //是否是云
        public bool IsCloud = false;
        //是否经过水体检测
        public bool WaterChecked = false;
        //是否是水
        public bool IsWater = false;

        public override string ToString()
        {
            return string.Format(INFO_FORMAT,
                IsDoubtFirPixel ? "是" : "否",
                IsVertified ? "是" : "否",
                MidIfrAvgValue,
                FarIfrAvgValue,
                MidIfr_FarIfr_Diff_AvgValue,
                MidIfr_StdDev,
                FarIfr_StdDev,
                MidIfr_FarIfr_Diff_StdDev,
                SecondPixelArea,
                PixelArea,
                FirIntensity,
                FireIntensityGrade,
                FirReliability,
                FireAreaNum == -1 ? "" : FireAreaNum.ToString());
        }
    }
}
