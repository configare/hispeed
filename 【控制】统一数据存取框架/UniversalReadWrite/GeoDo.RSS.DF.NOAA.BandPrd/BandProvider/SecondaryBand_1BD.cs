using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using GeoDo.RSS.Core.DF;


namespace GeoDo.RSS.DF.NOAA.BandPrd
{
    public class SecondaryBand_1BD : SecondaryBand
    {
        private string _fileName = null;
        private string _bandName = null;
        //private FileStream _fs = null;
        private int _offset = 22016;
        private int _sizeOfLine = 22016;
        private bool _isBigEndian = true;

        public SecondaryBand_1BD(IRasterDataProvider rasterDataProvider)
            : base(rasterDataProvider)
        {
            _fileName = rasterDataProvider.fileName;
            _width = rasterDataProvider.Width;
            _height = rasterDataProvider.Height;
            _dataType = enumDataType.Double;
            _isBigEndian = IsBigEndian(rasterDataProvider);
        }

        private bool IsBigEndian(IRasterDataProvider rasterDataProvider)
        {
            using (FileStream fs = new FileStream(rasterDataProvider.fileName, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    byte[] buffer = new byte[2];
                    fs.Seek(10, SeekOrigin.Begin);
                    buffer = br.ReadBytes(2);
                    br.Close();
                    fs.Close();
                    if (ToLocalEndian_Core.ToInt16FromBig(buffer) == 22016)
                        return true;
                    else
                        return false;
                }
            }
        }


        public string BandName
        {
            get { return _bandName; }
            set { _bandName = value; }
        }

        protected override void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, Core.DF.enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            //base.DirectRead(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            switch (_bandName)
            {
                case "Latitude": DirectReadGeo(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, true); break;
                case "Longitude": DirectReadGeo(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, false); break;
                case "SolarZenith": DirectReadAngle(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, 0); break;
                case "SatelliteZenith": DirectReadAngle(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, 1); break;
                case "RelativeAzimuth": DirectReadAngle(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, 2); break;
            }
        }
        /// <summary>
        /// 读取指定位置大小某一角度信息
        /// </summary>
        /// <param name="mark">角度信息标志位，0：太阳天顶角；1：卫星天顶角；2：相对方向角；</param>
        private void DirectReadAngle(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, Core.DF.enumDataType dataType, int xBufferSize, int yBufferSize, int mark)
        {
            if (xSize == xBufferSize && ySize == yBufferSize)
            {
                Int16[] bandBuffer = new Int16[xSize * ySize];
                List<Int16[]> angles = null;
                using (FileStream fs = new FileStream(_fileName, FileMode.Open))
                {
                   angles = ReadAngleInfo(fs, yOffset, ySize, mark);
                }
                Parallel.For(0, ySize, (j) =>
                {
                    for (int i = xOffset + 1, k = 0; i <= (xOffset + xSize); i++, k++)
                    {
                        bandBuffer[k + j * xSize] = Lagrange(angles[j], i);
                    }
                });
                Marshal.Copy(bandBuffer, 0, buffer, bandBuffer.Length);
            }
            else
            {
                double[] bandBuffer = new double[xBufferSize * yBufferSize];
                List<Int16[]> angles = null;
                int[] yIndex = ComputeTargetIndexs(ySize, yBufferSize, yOffset);
                int[] xIndex = ComputeTargetIndexs(xSize, xBufferSize, xOffset);
                using (FileStream fs = new FileStream(_fileName, FileMode.Open))
                {
                    angles = ReadAngleInfo(fs, yIndex, mark);
                }
                Parallel.For(0, yBufferSize, (j) =>
                {
                    for (int i = 0; i < xIndex.Length; i++)
                    {
                        bandBuffer[i + j * xBufferSize] = Lagrange(angles[j], xIndex[i]);
                    }
                });
                Marshal.Copy(bandBuffer, 0, buffer, bandBuffer.Length);
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
                    lons = ReadPositionInfo(fs, yOffset, ySize,isLat);
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
            else
            {
                double[] bandBuffer = new double[xBufferSize * yBufferSize];
                List<double[]> lons = null;
                //计算行索引
                int[] yIndex = ComputeTargetIndexs(ySize, yBufferSize, yOffset);
                int[] xIndex = ComputeTargetIndexs(xSize, xBufferSize, xOffset);
                using (FileStream fs = new FileStream(_fileName, FileMode.Open))
                {
                    lons = ReadPositionInfo(fs, yIndex, isLat);
                }
                Parallel.For(0, yBufferSize, (j) =>
                {
                    for (int i = 0; i < xIndex.Length; i++)
                    {
                        bandBuffer[i + j * xBufferSize] = Lagrange(lons[j], xIndex[i]);
                    }
                });
                Marshal.Copy(bandBuffer, 0, buffer, bandBuffer.Length);
            }
        }

        /// <summary>
        /// 计算采样后目标索引号
        /// </summary>
        /// <param name="originalSize"></param>
        /// <param name="targetSize"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private int[] ComputeTargetIndexs(int originalSize, int targetSize, int offset)
        {
            float scale = (float)originalSize / targetSize;
            int[] index = new int[targetSize];
            for (int i = 0; i < targetSize; i++)
            {
                index[i] = (int)(offset + i * scale);
            }
            return index;
        }

        private List<double[]> ReadPositionInfo(FileStream fs,int[] yIndex, bool isLat)
        {
            List<double[]> datas = new List<double[]>();
            using (BinaryReader br = new BinaryReader(fs))
            {
                if (isLat)
                {
                    for(int i=0;i<yIndex.Length;i++)
                    {
                        fs.Seek(_offset + (yIndex[i]-1) * _sizeOfLine + 640, SeekOrigin.Begin);
                        datas.Add(ExtractLatInfo(br.ReadBytes(408)));
                    }
                }
                else
                {
                    for (int i = 0; i < yIndex.Length; i++)
                    {
                        fs.Seek(_offset + (yIndex[i] - 1) * _sizeOfLine + 640, SeekOrigin.Begin);
                        datas.Add(ExtractLonInfo(br.ReadBytes(408)));
                    }
                }
            }
            return datas;
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
                fs.Seek(_offset + (yOffset * _sizeOfLine) + 640, SeekOrigin.Begin);
                if (isLat)
                {
                    while (index <= ySize)
                    {
                        datas.Add(ExtractLatInfo(br.ReadBytes(408)));
                        fs.Seek(_sizeOfLine - 408, SeekOrigin.Current);
                        index ++;
                    }
                }
                else
                {
                    while (index <= ySize)
                    {
                        datas.Add(ExtractLonInfo(br.ReadBytes(408)));
                        fs.Seek(_sizeOfLine - 408, SeekOrigin.Current);
                        index ++;
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
                if (_isBigEndian)
                    lons[j] = (float)(ToLocalEndian_Core.ToInt32FromBig(lon) / divisor);
                else
                    lons[j] = (float)(ToLocalEndian_Core.ToInt32FromLittle(lon) / divisor);
            }
            return lons;
        }


        double divisor = Math.Pow(10, 4);
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
                if (_isBigEndian)
                    lats[j] = (float)(ToLocalEndian_Core.ToInt32FromBig(lat) / divisor);
                else
                    lats[j] = (float)(ToLocalEndian_Core.ToInt32FromLittle(lat) / divisor);
            }
            return lats;
        }

        private List<Int16[]> ReadAngleInfo(FileStream fs, int[] yIndex, int mark)
        {
            List<Int16[]> datas = new List<Int16[]>();
            using (BinaryReader br = new BinaryReader(fs))
            {
                for (int i = 0; i < yIndex.Length; i++)
                {
                    fs.Seek(_offset + (yIndex[i] - 1) * _sizeOfLine + 328, SeekOrigin.Begin);
                    datas.Add(ExtractAngleInfo(br.ReadBytes(306), mark));
                }
            }
            return datas;
        }

        /// <summary>
        /// 获取多行的某个角度信息
        /// </summary>
        /// <param name="yOffset">列偏移</param>
        /// <param name="ySize"></param>
        /// <param name="mark">标志位，标志获取哪个角度，0：太阳天顶角；1：卫星天顶角；2：相对方位角</param>
        /// <returns></returns>
        private List<Int16[]> ReadAngleInfo(FileStream fs, int yOffset, int ySize, int mark)
        {
            List<Int16[]> datas = new List<Int16[]>();
            using (BinaryReader br = new BinaryReader(fs))
            {
                int index = 1;
                fs.Seek(_offset + (yOffset * _sizeOfLine) + 328, SeekOrigin.Begin);
                while (index <= ySize)
                {
                    datas.Add(ExtractAngleInfo(br.ReadBytes(306), mark));
                    fs.Seek(_sizeOfLine - 306, SeekOrigin.Current);
                    index++;
                }
            }
            return datas;
        }
        /// <summary>
        /// 获取单条数据的某一角度信息
        /// </summary>
        /// <param name="angleInfo"></param>
        /// <param name="mark">角度标志位，0：太阳天顶角；1:卫星天顶角；2：相对方位角</param>
        /// <returns></returns>
        private Int16[] ExtractAngleInfo(byte[] angleInfo, int mark)
        {
            byte[] angle = new byte[2];
            Int16[] angles = new Int16[51];
            int pt = 0;
            int maxi = 2 * (mark + 1);
            for (int j = 0; j < 51; j++)
            {
                for (int i = (mark * 2); i < maxi; i++)
                {
                    angle[pt++] = angleInfo[i + 6 * j];
                }
                pt = 0;
                if (_isBigEndian)
                    angles[j] = ToLocalEndian_Core.ToInt16FromBig(angle);
                else
                    angles[j] = ToLocalEndian_Core.ToInt16FromLittle(angle);
            }
            return angles;
        }

        public Int16 Lagrange(Int16[] origData, int index)
        {
            //七次插值
            double[] data = new double[7];
            double[] x = new double[7];
            double targetData = 0;
            //取临近七个插值点
            if (index < 145)
            {
                for (int i = 0; i < 7; i++)
                {
                    data[i] = origData[i];
                    x[i] = 25 + 40 * i;
                }
            }
            else if (index >= 1905)
            {
                for (int i = 0; i < 7; i++)
                {
                    data[i] = origData[44 + i];
                    x[i] = 25 + 40 * (44 + i);
                }
            }
            else
            {
                int d = (index - 25) / 40 - 2;
                for (int i = 0; i < 7; i++)
                {
                    data[i] = origData[d + i];
                    x[i] = 25 + 40 * (d + i);
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
            return (Int16)targetData;
        }

        public double Lagrange(double[] origData, int index)
        {
            //七次插值
            double[] data = new double[7];
            double[] x = new double[7];
            double targetData = 0;
            //取临近七个插值点
            if (index < 145)
            {
                //x[0] = 25;
                for (int i = 0; i < 7; i++)
                {
                    data[i] = origData[i];
                    x[i] = 25 + 40 * i;
                }
                //for (int i = 1; i < 7; i++)
                //{
                //    x[i] = x[i - 1] + 40;
                //}
            }
            else if (index >= 1905)
            {
                //x[0] = 25 + 40 * 44;
                for (int i = 0; i < 7; i++)
                {
                    data[i] = origData[44 + i];
                    x[i] = 25 + 40 * (44 + i);
                }
                //for (int i = 1; i < 7; i++)
                //{
                //    x[i] = x[i - 1] + 40;
                //}
            }
            else
            {
                int d = (index - 25) / 40 - 2;
                //x[0] = 25 + 40 * d;
                for (int i = 0; i < 7; i++)
                {
                    data[i] = origData[d + i];
                    x[i] = 25 + 40 * (d + i);
                }
                //for (int i = 1; i < 7; i++)
                //{
                //    x[i] = x[i - 1] + 40;
                //}
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
    }
}
