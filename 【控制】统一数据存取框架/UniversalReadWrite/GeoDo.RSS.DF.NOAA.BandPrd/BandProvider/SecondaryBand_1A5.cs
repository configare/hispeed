using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using GeoDo.RSS.Core.DF;


namespace GeoDo.RSS.DF.NOAA.BandPrd
{
    public class SecondaryBand_1A5:SecondaryBand
    {
        private string _fileName = null;
        private string _bandName = null;
        //private FileStream _fs = null;
        private int _offset = 21980;
        private int _sizeOfLine = 21980;

        public SecondaryBand_1A5(IRasterDataProvider rasterDataProvider)
            : base(rasterDataProvider)
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
    
        protected override void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            base.DirectRead(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
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
        private void DirectReadAngle(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, Core.DF.enumDataType dataType, int xBufferSize, int yBufferSize, int mark)
        {
            if (xSize == xBufferSize && ySize == yBufferSize)
            {                
                float[] bandBuffer = new float[xSize * ySize];
                List<float[]> angles = new List<float[]>();
                using (FileStream _fs = new FileStream(_fileName, FileMode.Open))
                {
                    if (mark == 0)
                    {
                        angles = ReadSolarZenith(_fs, yOffset, ySize);
                    }
                    else if (mark == 1)
                    {
                        angles = ReadSatZenith(_fs, yOffset, ySize);
                    }
                    else
                    {
                        angles = ReadRelAzimuth(_fs, yOffset, ySize);
                    }
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
        }

        /// <summary>
        /// 读取指定位置大小纬度或者经度
        /// </summary>
        /// <param name="isLat"></param>
        public void DirectReadGeo(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, Core.DF.enumDataType dataType, int xBufferSize, int yBufferSize, bool isLat)
        {
            if (xSize == xBufferSize && ySize == yBufferSize)
            {
                double[] bandBuffer = new double[xSize * ySize];
                List<double[]> lons = null;
                using (FileStream _fs = new FileStream(_fileName, FileMode.Open))
                {
                    lons = ReadPositionInfo(_fs, yOffset, ySize, isLat);
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
        private List<double[]> ReadPositionInfo(FileStream _fs,int yOffset, int ySize, bool isLat)
        {
            List<double[]> datas = new List<double[]>();
            using (BinaryReader br = new BinaryReader(_fs))
            {
                int index = 1;
                _fs.Seek(_offset + (yOffset * _sizeOfLine) + 260, SeekOrigin.Begin);
                if (isLat)
                {
                    while (index <= ySize)
                    {
                        datas.Add(ExtractLatInfo(br.ReadBytes(408)));
                        _fs.Seek(_sizeOfLine - 408, SeekOrigin.Current);
                        index++;
                    }
                }
                else
                {
                    while (index <= ySize)
                    {
                        datas.Add(ExtractLonInfo(br.ReadBytes(408)));
                        _fs.Seek(_sizeOfLine - 408, SeekOrigin.Current);
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
            for (int j = 0; j < 51; j++)
            {
                lons[j] = BitConverter.ToSingle(geoInfo, j * 8 + 4);
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
            for (int j = 0; j < 51; j++)
            {
                lats[j] = BitConverter.ToSingle(geoInfo, j * 8);
            }
            return lats;
        }

        /// <summary>
        /// 获取多行的太阳天顶角信息
        /// </summary>
        /// <param name="yOffset">列偏移</param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        private List<float[]> ReadSolarZenith(FileStream _fs, int yOffset, int ySize)
        {
            List<float[]> datas = new List<float[]>();
            using (BinaryReader br = new BinaryReader(_fs))
            {
                int index = 1;
                _fs.Seek(_offset + (yOffset * _sizeOfLine) + 56, SeekOrigin.Begin);
                while (index <= ySize)
                {
                    datas.Add(ExtractAngleInfo(br.ReadBytes(204)));
                    _fs.Seek(_sizeOfLine - 204, SeekOrigin.Current);
                    index++;
                }
            }
            return datas;
        }

        private List<float[]> ReadSatZenith(FileStream _fs, int yOffset, int ySize)
        {
            List<float[]> datas = new List<float[]>();
            using (BinaryReader br = new BinaryReader(_fs))
            {
                int index = 1;
                _fs.Seek(_offset + (yOffset * _sizeOfLine) + 874, SeekOrigin.Begin);
                while (index <= ySize)
                {
                    datas.Add(ExtractAngleInfo(br.ReadBytes(204)));
                    _fs.Seek(_sizeOfLine - 204, SeekOrigin.Current);
                    index++;
                }
            }
            return datas;
        }

        private List<float[]> ReadRelAzimuth(FileStream _fs, int yOffset, int ySize)
        {
            List<float[]> datas = new List<float[]>();
            using (BinaryReader br = new BinaryReader(_fs))
            {
                int index = 1;
                _fs.Seek(_offset + (yOffset * _sizeOfLine) + 1078, SeekOrigin.Begin);
                while (index <= ySize)
                {
                    datas.Add(ExtractAngleInfo(br.ReadBytes(204)));
                    _fs.Seek(_sizeOfLine - 204, SeekOrigin.Current);
                    index++;
                }
            }
            return datas;
        }

        private float[] ExtractAngleInfo(byte[] angleInfo)
        {
            byte[] angle = new byte[4];
            float[] angles = new float[51];
            for (int j = 0; j < 51; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    angles[j] = BitConverter.ToSingle(angleInfo, i * 4);
                }               
            }
            return angles;
        }

        /// <summary>
        /// 拉格朗日插值法计算指定点属性
        /// </summary>
        /// <param name="origData">未经插值数据</param>
        /// <param name="index">插值点序号</param>
        /// <returns>插值结果</returns>
        public double Lagrange(double[] origData, int index)
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
            return targetData;
        }

        public float Lagrange(float[] origData, int index)
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

        protected override void CheckArgumentsisValid(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            if (xOffset < 0 || yOffset < 0 || xOffset >= _width || yOffset >= _height)
                throw new RequestBlockOutOfRasterException(xOffset, yOffset, xSize, ySize);
            if ((xOffset + xSize) > Width || (yOffset + ySize) > Height)
                throw new Exception();
            if (dataType != enumDataType.Double)
                throw new Exception();
            if (buffer == IntPtr.Zero || xBufferSize == 0 || yBufferSize == 0)
                throw new BufferIsEmptyException();
        }
    }
}
