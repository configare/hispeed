using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace GeoDo.RSS.Core.DF
{
    //<Stretcher name="" satelliate="FY3A,FY3B" sensor="VIRR,MERSI" bandno="1" stretcher="GeoDo.RSS.Core.DF.DLL,GeoDo.RSS.Core.DF.LinearRgbStretcher" 
    //mindata="0" maxdata="1000" mingray="0" maxgray="255"  defaultbands="6,2,1"/>
    /// <summary>
    /// 修改日期：20120212
    /// 修改内容：
    /// 修改defaultbands属性值支持多值，以|分割，如defaultbands="6,2,1|1,1,1",前边代表白天的默认通道，后面代表夜间的默认通道
    /// 通过添加一个属性来做这种支持:DefaultBandsExt
    /// </summary>
    public class StretcherConfigItem
    {
        public string Name;
        public List<string> Satellites = new List<string>();
        public List<string> Sensors = new List<string>();
        public List<int> BandNo = new List<int>();
        public string StretcherClass;
        /// <summary>
        /// 根据拉伸器的实际数值类型指定
        /// </summary>
        public double MinData;
        /// <summary>
        /// 根据拉伸器的实际数值类型指定
        /// </summary>
        public double MaxData;
        public byte MinGray;
        public byte MaxGray;
        public bool IsUseMap;
        public bool IsOribit;
        public int[] DefaultBands;
        public bool IsProduct;
        public string ProductIdentify;
        public string SubProductidentify;
        public int[] DefaultBandsExt;

        public object CreateStretcher()
        {
            if (StretcherClass == null)
                return null;
            string[] parts = StretcherClass.Split(',');
            if (parts == null || parts.Length != 2)
                return null;
            try
            {
                Assembly assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + parts[0]);
                object[] args = new object[] { MinData, MaxData, (byte)MinGray, (byte)MaxGray, IsUseMap, DefaultBands };
                switch (parts[1])
                {
                    case "GeoDo.RSS.Core.DF.LinearRgbStretcherByte":
                        args = new object[] { (byte)MinData, (byte)MaxData, (byte)MinGray, (byte)MaxGray, IsUseMap, DefaultBands };
                        break;
                    case "GeoDo.RSS.Core.DF.LinearRgbStretcherInt16":
                        args = new object[] { (Int16)MinData, (Int16)MaxData, (byte)MinGray, (byte)MaxGray, IsUseMap, DefaultBands };
                        break;
                    case "GeoDo.RSS.Core.DF.LinearRgbStretcherUInt16":
                        args = new object[] { (UInt16)MinData, (UInt16)MaxData, (byte)MinGray, (byte)MaxGray, IsUseMap, DefaultBands };
                        break;
                    case "GeoDo.RSS.Core.DF.LinearRgbStretcherInt32":
                        args = new object[] { (Int32)MinData, (Int32)MaxData, (byte)MinGray, (byte)MaxGray, IsUseMap, DefaultBands };
                        break;
                    case "GeoDo.RSS.Core.DF.LinearRgbStretcherUInt32":
                        args = new object[] { (UInt32)MinData, (UInt32)MaxData, (byte)MinGray, (byte)MaxGray, IsUseMap, DefaultBands };
                        break;
                    case "GeoDo.RSS.Core.DF.LinearRgbStretcherInt64":
                        args = new object[] { (Int64)MinData, (Int64)MaxData, (byte)MinGray, (byte)MaxGray, IsUseMap, DefaultBands };
                        break;
                    case "GeoDo.RSS.Core.DF.LinearRgbStretcherFloat":
                        args = new object[] { (Single)MinData, (Single)MaxData, (byte)MinGray, (byte)MaxGray, IsUseMap, DefaultBands };
                        break;
                    case "GeoDo.RSS.Core.DF.LinearRgbStretcherUInt64":
                        args = new object[] { (UInt64)MinData, (UInt64)MaxData, (byte)MinGray, (byte)MaxGray, IsUseMap, DefaultBands };
                        break;
                    case "GeoDo.RSS.Core.DF.DLL,GeoDo.RSS.Core.DF.LinearRgbStretcherDouble":
                    default:
                        break;
                }
                return assembly.CreateInstance(parts[1], false, BindingFlags.CreateInstance, null, args, null, null);
            }
            catch (Exception ex)
            {
                throw new Exception("构造原始值拉伸器发生错误", ex);
            }
        }
    }
}
