using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.NOAA;

namespace GeoDo.Radiation
{
    /// <summary>
    /// 亮温转换提供者
    /// </summary>
    [Export(typeof(IRadiationProvider)), ExportMetadata("VERSION", "1")]
    internal class NOAA_1BDRadiationProvider : RadiationProvider
    {
        private const double DEG_TO_RAD_P100 = 0.000174532925199432955; // (PI/180)/100;
        private const double C1 = 1.438833;
        private const double C2 = 1.1910659 / 100000;

        #region 定标变量
        //反射通道定标系数（业务用，通道1、2、3A），存储为：[行,3*5] scale1，offset1，scale2，offset2，inflection
        private double[,] _refSB_Coeff = null;
        //发射通道定标系数（业务用，通道3B、4、5），存储为：[行,3*3] scale1，scale2，offset
        private double[,] _emissive_Radiance_Coeff = null;
        //A、B
        private float[] _emmisive_BT_Coefficients = null;
        /// <summary>
        /// 太阳天顶角数据集(有效区间0-18000),角度值放大了100倍
        /// </summary>
        private Int16[] _solarZenithData = null;
        private bool _isDay = false;        //白天
        #endregion

        protected NOAA_1BDRadiationProvider():base()
        {
            _name = "NOAA_1BD";
        }

        public override void InitRadiationArgs(IRasterDataProvider srcRaster, bool isSolarZenith)
        {
            ReadyRadiationArgs(srcRaster);
            if (isSolarZenith)
                ReadySolarZenithArgs(srcRaster);
        }

        #region 定标参数读取
        //准备[辐射定标]参数
        private void ReadyRadiationArgs(IRasterDataProvider srcRaster)
        {
            try
            {
                /*
NOAA15
                A[0] = 1.621256f; A[1] = 0.337810f; A[2] = 0.304558f;
                B[0] = 0.998015f; B[1] = 0.998719f; B[2] = 0.999024f;
                v[0] = 2695.9743f; v[1] = 925.4075f; v[2] = 839.8979f;
NOAA16
                A[0] = 1.592459f; A[1] = 0.332380f; A[2] = 0.674623f;
                B[0] = 0.998147f; B[1] = 0.998522f; B[2] = 0.998363f;
                v[0] = 2700.1148f; v[1] = 917.2289f; v[2] = 838.1255f;
NOAA17
                A[0] = 1.702380f; A[1] = 0.271683f; A[2] = 0.309180f;
                B[0] = 0.997378f; B[1] = 0.998794f; B[2] = 0.999012f;
                v[0] = 2669.3554f; v[1] = 926.2947f; v[2] = 839.8246f;
NOAA18
                A[0] = 1.698704f; A[1] = 0.436645f; A[2] = 0.253179f;
                B[0] = 0.996960f; B[1] = 0.998607f; B[2] = 0.999057f;
                v[0] = 2659.7952f; v[1] = 928.1460f; v[2] = 833.2532f;
NOAA19
                A[0] = 1.698704f; A[1] = 0.436645f; A[2] = 0.253179f;
                B[0] = 0.996960f; B[1] = 0.998607f; B[2] = 0.999057f;
                v[0] = 2659.7952f; v[1] = 928.1460f; v[2] = 833.2532f;
                */
                _emmisive_BT_Coefficients = new float[] { 
                    1.698704f, 0.996960f, 2659.7952f,
                    0.436645f,0.998607f,928.1460f,
                    0.253179f,0.999057f,833.2532f
                };
                _isDay = false;
                ID1BDDataProvider dp = srcRaster as ID1BDDataProvider;
                double[,] operCoef = null;
                double[,] c = null;
                double[,] b = null;
                dp.ReadVisiCoefficient(ref operCoef, ref b, ref c);
                dp.ReadIRCoefficient(ref operCoef, ref b);
                _refSB_Coeff = operCoef;
                _emissive_Radiance_Coeff = operCoef;
            }
            catch (Exception ex)
            {
                throw new Exception("读取亮温计算参数失败", ex);
            }
        }

        //准备[太阳高度角订正]参数,目前还没有太阳高度角订正的公式。
        private void ReadySolarZenithArgs(IRasterDataProvider srcRaster)
        {
            IBandProvider bandPrd = srcRaster.BandProvider;
            IRasterBand[] bands = bandPrd.GetBands("SolarZenith");
            if (bands == null || bands.Length == 0 || bands[0] == null)
                throw new Exception("读取Noaa 1BD太阳天顶角失败：无法获取到SolarZenith波段信息");//读取天顶角数值
            try
            {
                using (IRasterBand band = bands[0])
                {
                    double[] data = new double[band.Width * band.Height];
                    unsafe
                    {
                        fixed (Double* ptr = data)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Write(0, 0, band.Width, band.Height, bufferPtr, enumDataType.Double, band.Width, band.Height);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("读取Noaa 1BD太阳天顶角失败："+ex.Message, ex);
            }
        }

        private float[] ReadFileAttributeToFloat(IBandProvider srcbandpro, string AttrName, int length)
        {
            float[] value = new float[length];
            Dictionary<string, string> dsAtts = srcbandpro.GetAttributes();
            string refSbCalStr = dsAtts[AttrName];
            string[] refSbCals = refSbCalStr.Split(',');
            if (refSbCals.Length >= length)
            {
                for (int i = 0; i < length; i++)
                {
                    value[i] = float.Parse(refSbCals[i]);
                }
                return value;
            }
            else
                return null;
        }

        private Int16[] ReadDataSetToInt16(IBandProvider srcbandpro, Size srcSize, string dataSetName, int bandIndex)
        {
            Int16[] data = new Int16[srcSize.Width * srcSize.Height];
            using (IRasterBand rasterBand = srcbandpro.GetBands(dataSetName)[0])
            {
                unsafe
                {
                    fixed (Int16* ptr = data)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        rasterBand.Read(0, 0, srcSize.Width, srcSize.Height, bufferPtr, enumDataType.Int16, srcSize.Width, srcSize.Height);
                    }
                }
            }
            return data;
        }

        private float[] ReadDataSetToSingle(IBandProvider srcbandpro, Size srcSize, string dataSetName, int bandIndex)
        {
            Single[] data = new Single[srcSize.Width * srcSize.Height];
            using (IRasterBand rasterBand = srcbandpro.GetBands(dataSetName)[0])
            {
                unsafe
                {
                    fixed (Single* ptr = data)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        rasterBand.Read(0, 0, srcSize.Width, srcSize.Height, bufferPtr, enumDataType.Float, srcSize.Width, srcSize.Height);
                    }
                }
            }
            return data;
        }

        #endregion

        /// <summary>
        /// 亮温转换
        /// </summary>
        /// <param name="srcBandData">
        /// 输入的是转换前的通道值
        /// 输出的是转换后的通道值
        /// </param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="srcWidth"></param>
        /// <param name="srcHeight"></param>
        /// <param name="bandIndex"></param>
        /// <param name="isRadiation"></param>
        /// <param name="isSolarZenith"></param>
        public override void DoRadiation(ushort[] srcBandData, int xOffset,int yOffset,int srcWidth, int srcHeight, 
            int bandIndex, bool isRadiation, bool isSolarZenith)
        {
            throw new NotImplementedException();
        }
    }
}
