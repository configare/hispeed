#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/28 14:54:48
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.RSS.DF.FY1D;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.DF.FY1D.BandPrd
{
    /// <summary>
    /// 类名：SecondaryBandFor1A5
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/28 14:54:48
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SecondaryBandFor1A5:RasterBand
    {
        private string _fileName = null;
        private string _bandName = null;
        private int _offset = 9744;
        private int _sizeOfLine = 9744;

        public SecondaryBandFor1A5(IRasterDataProvider rasterDataProvider)
            :base(rasterDataProvider)
        {
            _fileName = rasterDataProvider.fileName;
            _width = rasterDataProvider.Width;
            _height = rasterDataProvider.Height;
            _dataType = enumDataType.Double;
        }

        public string BandName
        {
            get { return _bandName; }
            set { _bandName = value; }
        }

        protected override void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, Core.DF.enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            switch (_bandName)
            {
                case "Latitude": DirectReadGeo(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, true); break;
                case "Longitude": DirectReadGeo(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, false); break;
            }
        }
       
        public void DirectReadGeo(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, Core.DF.enumDataType dataType, int xBufferSize, int yBufferSize, bool isLat)
        {
            if (xSize == xBufferSize && ySize == yBufferSize)
            {
                double[] bandBuffer = new double[xSize * ySize];
                List<double[]> lons = null;
                using (FileStream fs = new FileStream(_fileName, FileMode.Open))
                {
                    lons = ReadPositionInfo(fs, yOffset, ySize, isLat);
                }
                Parallel.For(0, ySize, (j) =>
                {
                    for (int i = xOffset + 1, k = 0; i <= (xOffset + xSize); i++, k++)
                    {
                        bandBuffer[k + j * xSize] = Lagrange(lons[j], i);
                    }
                });
                Marshal.Copy(bandBuffer, 0, buffer, bandBuffer.Length);
            }
        }
        /// <summary>
        /// 读取未插值的纬度或经度数组
        /// </summary>
        private List<double[]> ReadPositionInfo(FileStream fs, int yOffset, int ySize, bool isLat)
        {
            List<double[]> datas = new List<double[]>();
            using (BinaryReader br = new BinaryReader(fs))
            {
                int index = 1;
                fs.Seek(_offset + (yOffset * _sizeOfLine) + 252, SeekOrigin.Begin);
                if (isLat)
                {
                    while (index <= ySize)
                    {
                        datas.Add(ExtractLatInfo(br.ReadBytes(408)));
                        fs.Seek(_sizeOfLine - 408, SeekOrigin.Current);
                        index++;
                    }
                }
                else
                {
                    while (index <= ySize)
                    {
                        datas.Add(ExtractLonInfo(br.ReadBytes(408)));
                        fs.Seek(_sizeOfLine - 408, SeekOrigin.Current);
                        index++;
                    }
                }
            }
            return datas;
        }

        /// <summary>
        /// 获取单条数据记录的经度信息
        /// </summary>
        private double[] ExtractLonInfo(byte[] geoInfo)
        {
            byte[] lon = new byte[4];
            double[] lons = new double[51];
            int pt = 0;
            for (int j = 0; j < 51; j++)
            {
                for (int i = 4; i < 8; i++)
                {
                    lon[pt++] = geoInfo[i + 8 * j];
                }
                pt = 0;
                lons[j] = ToLocalEndian.ToFloatFromBig(lon);
            }
            return lons;
        }


        /// <summary>
        /// 获取单条数据记录的纬度信息
        /// </summary>
        private double[] ExtractLatInfo(byte[] geoInfo)
        {
            byte[] lat = new byte[4];
            double[] lats = new double[51];
            int pt = 0;
            for (int j = 0; j < 51; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    lat[pt++] = geoInfo[i + 8 * j];
                }
                pt = 0;
                lats[j] = ToLocalEndian.ToFloatFromBig(lat);
            }
            return lats;
        }

        public double Lagrange(double[] origData, int index)
        {
            //七次插值
            double[] data = new double[7];
            double[] x = new double[7];
            double targetData = 0;
            //取临近七个插值点
            if (index < 68)
            {
                for (int i = 0; i < 7; i++)
                {
                    data[i] = origData[i];
                    x[i] = 8 + 20 * i;
                }
            }
            else if (index >= 948)
            {
                for (int i = 0; i < 7; i++)
                {
                    data[i] = origData[44 + i];
                    x[i] = 8 + 20 * (44 + i);
                }
            }
            else
            {
                int d = (index - 8) / 20 - 2;
                for (int i = 0; i < 7; i++)
                {
                    data[i] = origData[d + i];
                    x[i] = 8 + 20 * (d + i);
                }
            }
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (j != i) data[i] *= (double)(index - x[j]) / (x[i] - x[j]);
                }
                targetData += data[i];
            }
            return targetData;
        }

        protected override void CheckArgumentsisValid(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            if (xOffset < 0 || yOffset < 0 || xOffset >= _width || yOffset >= _height)
                throw new RequestBlockOutOfRasterException(xOffset, yOffset, xSize, ySize);
            if ((xOffset + xSize) > Width || (yOffset + ySize) > Height)
                throw new Exception();
            if (buffer == IntPtr.Zero || xBufferSize == 0 || yBufferSize == 0)
                throw new BufferIsEmptyException();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override void DirectWrite(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            throw new NotImplementedException();
        }

        public override void Fill(double noDataValue)
        {
            throw new NotImplementedException();
        }

        public override void ComputeMinMax(out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeMinMax(double begin, double end, out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeStatistics(out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeStatistics(double begin, double end, out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeHistogram(double begin, double end, int buckets, int[] histogram, bool isIncludeOutOfRange, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }
    }
}
