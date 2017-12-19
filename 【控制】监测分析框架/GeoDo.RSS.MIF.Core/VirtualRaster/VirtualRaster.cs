using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Core
{
    public class VirtualRaster
    {
        private IRasterDataProvider _raster;
        private VirtualRasterHeader _vHeader;
        private RasterRectOffset _inOffset;
        private int[] _bandMaps;

        public VirtualRaster(IRasterDataProvider raster, VirtualRasterHeader vHeader)
            : this(raster, vHeader, null)
        {
        }

        public VirtualRaster(IRasterDataProvider raster, VirtualRasterHeader vHeader, int[] bandNosMap)
        {
            _raster = raster;
            _vHeader = vHeader;
            if (_raster == null || _vHeader == null)
                return;
            CalcRasterOffset();
            if (bandNosMap == null || bandNosMap.Length == 0)
            {
                int bandCount = raster.BandCount;
                bandNosMap = new int[bandCount];
                for (int j = 0; j < bandCount; j++)
                {
                    bandNosMap[j] = j + 1;
                }
            }
            _bandMaps = bandNosMap;
        }

        public VirtualRasterHeader VirtualHeader
        {
            get { return _vHeader; }
        }

        /// <summary>
        /// 实体数据对于VirtualHeader的偏移参数。
        /// 计算出的偏移，可用于分块访问数据时候计算分块偏移量用
        /// </summary>
        public RasterRectOffset InOffset
        {
            get { return _inOffset; }
        }

        private void CalcRasterOffset()
        {
            CoordEnvelope oEnvelope = _raster.CoordEnvelope;
            CoordEnvelope tEnvelope = _vHeader.CoordEnvelope;
            Size oSize = new Size(_raster.Width, _raster.Height);
            Size tSize = new Size(_vHeader.Width, _vHeader.Height);
            float xScale = _raster.ResolutionX / _vHeader.ResolutionX;
            float yScale = _raster.ResolutionY / _vHeader.ResolutionY;
            Rectangle rectDst, srcInDst, intInDst, intInSrc;
            bool isInternal = RasterRectOffset.ComputeBeginEndRowCol(
                oEnvelope, oSize, new PointF(_raster.ResolutionX, _raster.ResolutionY),
                tEnvelope, tSize, new PointF(_vHeader.ResolutionX, _vHeader.ResolutionY),
                out rectDst, out srcInDst, out intInDst, out intInSrc);
            if (!isInternal)
                ;//不相交...
            _inOffset = new RasterRectOffset();
            _inOffset.rectDst = rectDst;
            _inOffset.srcInDst = srcInDst;
            _inOffset.intInDst = intInDst;
            _inOffset.intInSrc = intInSrc;
            _inOffset.ResolutionXScale = xScale;
            _inOffset.ResolutionYScale = yScale;
        }

        /// <summary>
        /// 访问数据
        /// 参数中的偏移量是相对于VirtualHeader指定的数据范围的。
        /// </summary>
        /// <param name="bandNo"></param>
        /// <param name="vOffsetX"></param>
        /// <param name="vOffsetY"></param>
        /// <param name="vSizex"></param>
        /// <param name="vSizey"></param>
        /// <returns></returns>
        public T[] ReadData<T>(int bandNo, int vOffsetX, int vOffsetY, int vSizex, int vSizey)
        {
            return ReadData<T>(bandNo, vOffsetX, vOffsetY, vSizex, vSizey, default(T));
        }

        /// <summary>
        /// 修改日期2013年1月3日
        /// 修改内容：
        /// 取相交区域，
        /// if (tWidth == intInSrc.Width && intInSrc.Height == tHeight)即分辨率一致时候，后面的读取参数偏移量应当使用前面修正后的toffsetx, toffsety
        /// 应为toffsetx, toffsety可能和intInSrc.X，intInSrc.Y 有偏差（由于浮点运算的缘故）。
        /// 读取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bandNo"></param>
        /// <param name="vOffsetX"></param>
        /// <param name="vOffsetY"></param>
        /// <param name="vSizex"></param>
        /// <param name="vSizey"></param>
        /// <param name="nullValue">非相交区域数据填充值（默认为default(T)）</param>
        /// <returns></returns>
        public T[] ReadData<T>(int bandNo, int vOffsetX, int vOffsetY, int vSizex, int vSizey, T nullValue)
        {
            if (_bandMaps != null)
                bandNo = _bandMaps[bandNo - 1];
            if (vOffsetX < 0 || vOffsetY < 0 || vSizex > _vHeader.Width || vSizey > _vHeader.Height)
                return null;
            GCHandle buffer = new GCHandle();
            IntPtr bufferPtr;
            try
            {
                T df = default(T);
                Type type = df.GetType();
                enumDataType dataType = GetDataTypeFrom(type);// _raster.DataType;
                int dataTypeSize = Marshal.SizeOf(type);
                T[] vBufferData = null;
                //计算当前分块的位置
                RasterRectOffset offset = _inOffset.Offset(vOffsetX, vOffsetY, vSizex, vSizey);
                Rectangle rectDst = offset.rectDst;
                Rectangle srcInDst = offset.srcInDst;
                Rectangle intInSrc = offset.intInSrc;
                Rectangle intInDst = offset.intInDst;
                vBufferData = new T[intInDst.Width * intInDst.Height];
                buffer = GetHandles(vBufferData);
                bufferPtr = buffer.AddrOfPinnedObject();
                //获取相交区域的数据
                int toffsetx = RasterRectOffset.GetInteger(intInSrc.X / offset.ResolutionXScale);
                int toffsety = RasterRectOffset.GetInteger(intInSrc.Y / offset.ResolutionYScale);
               
                #region by chennan 20140812 分辨率不同，切块剩余行数小于分辨率放大倍数时，误判断为不相交

                if (intInSrc.Width == 0 || intInSrc.Height == 0)//当前分块不相交，造成无数据
                    return null;
                int tWidth = RasterRectOffset.GetWHInteger(intInSrc.Width / offset.ResolutionXScale);
                int tHeight = RasterRectOffset.GetWHInteger(intInSrc.Height / offset.ResolutionYScale);

                //int tWidth = RasterRectOffset.GetInteger(intInSrc.Width / offset.ResolutionXScale);
                //int tHeight = RasterRectOffset.GetInteger(intInSrc.Height / offset.ResolutionYScale);
                //if (tWidth == 0 || tHeight == 0)//当前分块不相交，造成无数据
                //    return null;

                #endregion

                if (tWidth + toffsetx > _raster.Width)
                    tWidth = _raster.Width - toffsetx;
                if (tHeight + toffsety > _raster.Height)
                    tHeight = _raster.Height - toffsety;
                _raster.GetRasterBand(bandNo).Read(toffsetx, toffsety, tWidth, tHeight, bufferPtr, dataType, intInSrc.Width, intInSrc.Height);
                //如果相交区域即是目标区域
                if (intInDst == rectDst)
                    return vBufferData;
                else//按行列填充目标区域
                {
                    int readH = intInDst.Height;
                    int readW = intInDst.Width;
                    int padLeft = intInDst.Left;
                    int padRight = (vSizey + vOffsetY) - intInDst.Right;
                    int topOffset = intInDst.Top - rectDst.Top;

                    T[] tdata = new T[vSizex * vSizey];
                    if (!nullValue.Equals(df))
                    {
                        for (int i = 0; i < vSizex * vSizey; i++)
                            tdata[i] = nullValue;
                    }
                    for (int readLine = 0; readLine < readH; readLine++)//按行拷贝
                    {
                        int srcOffset = readLine * readW;
                        int dstOffset = (topOffset + readLine) * vSizex + padLeft;
                        Array.Copy(vBufferData, srcOffset, tdata, dstOffset, readW);
                    }
                    return tdata;
                }
            }
            finally
            {
                if (buffer.IsAllocated)
                    buffer.Free();
            }
        }

        private enumDataType GetDataTypeFrom(Type type)
        {
            TypeCode tc = Type.GetTypeCode(type);
            switch (tc)
            {
                case TypeCode.Boolean:
                    break;
                case TypeCode.Byte:
                    return enumDataType.Byte;
                    break;
                case TypeCode.Char:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.DateTime:
                    break;
                case TypeCode.Decimal:
                    break;
                case TypeCode.Double:
                    return enumDataType.Double;
                    break;
                case TypeCode.Empty:
                    break;
                case TypeCode.Int16:
                    return enumDataType.Int16;
                    break;
                case TypeCode.Int32:
                    return enumDataType.Int32;
                    break;
                case TypeCode.Int64:
                    return enumDataType.Int64;
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.SByte:
                    break;
                case TypeCode.Single:
                    return enumDataType.Float;
                    break;
                case TypeCode.String:
                    break;
                case TypeCode.UInt16:
                    return enumDataType.UInt16;
                    break;
                case TypeCode.UInt32:
                    return enumDataType.UInt32;
                    break;
                case TypeCode.UInt64:
                    return enumDataType.UInt64;
                    break;
                default:
                    break;
            }
            return enumDataType.Unknow;
        }

        public static IRasterDataProvider CreateVirtureData(VirtualRasterHeader vHeader, string filename, enumDataType enumDataType)
        {
            IRasterDataDriver dr = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            return dr.Create(filename, vHeader.Width, vHeader.Height, 1, enumDataType,
                vHeader.CoordEnvelope.ToMapInfoString(new Size(vHeader.Width, vHeader.Height)));
        }

        internal static GCHandle GetHandles<T>(T[] virtureInData)
        {
            return GCHandle.Alloc(virtureInData, GCHandleType.Pinned);
        }
    }

    public class test
    {
        public static void totest2()
        {
            string file = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\SystemData\RasterTemplate\China_XjRaster.dat";
            IRasterDataProvider inRaster = RasterDataDriver.Open(file) as IRasterDataProvider;
            VirtualRasterHeader vrh = VirtualRasterHeader.Create(new CoordEnvelope(120, 140, 10, 30), 0.005f, 0.005f);
            VirtualRaster vin = new VirtualRaster(inRaster, vrh);

            int[] data = vin.ReadData<int>(1, 10, 10, vrh.Width - 10, vrh.Height - 10);

            using (IRasterDataProvider raster = VirtualRaster.CreateVirtureData(vrh, @"d:\pppppp2.ldf", enumDataType.Int32))
            {
                GCHandle buffer = VirtualRaster.GetHandles(data);
                IntPtr bufferPtr = buffer.AddrOfPinnedObject();
                raster.GetRasterBand(1).Write(10, 10, vrh.Width - 10, vrh.Height - 10, bufferPtr, enumDataType.Int32, vrh.Width - 10, vrh.Height - 10);
                buffer.Free();
            }
        }
    }
}
