using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using GeoDo.RSS.DF.GDAL;
using HDF5DotNet;
using GeoDo.HDF5;

namespace GeoDo.FileProject
{
    [Export(typeof(IFileProjector)), ExportMetadata("VERSION", "1")]
    public class ProjectedFileTransform : FileProjector
    {
        private IRasterBand[] _rasterBands = null;
        private IProjectionTransform _projectionTransform;
        private RasterProjector _rasterProjector;

        public ProjectedFileTransform()
        {
            _name = "ProjectedTransform";
            _fullname = "自定义文件投影转换";
            _rasterProjector = new RasterProjector();
        }

        /// <summary>
        /// 
        /// 修改记录
        /// --------------------------------------------
        /// 修改日期：2013-80-29 
        /// 修改人：罗战克
        /// 修改内容：
        /// 计算最大输出范围，原先为计算原始数据所有点的坐标，
        /// 现修改为使用采样读取原数据地理坐标的方式（高度和宽度最多采样1000个点），以提高速度
        /// --------------------------------------------
        /// 
        /// </summary>
        /// <param name="srcRaster"></param>
        /// <param name="prjSettings"></param>
        /// <param name="dstSpatialRef"></param>
        /// <param name="progressCallback"></param>
        /// 
        public override void Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            Project(srcRaster, prjSettings, dstSpatialRef, progressCallback, 1, 1);
        }

        public override void Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback, double weight, float zoom)
        {
            ISpatialReference srcSpatialRef = srcRaster.SpatialRef;
            _projectionTransform = ProjectionTransformFactory.GetProjectionTransform(srcSpatialRef, dstSpatialRef);
            ArgsCheck(srcRaster, prjSettings, dstSpatialRef);
            ReadySession(srcRaster, prjSettings, dstSpatialRef, progressCallback);
            if (prjSettings.OutEnvelope == null || prjSettings.OutEnvelope == PrjEnvelope.Empty)
            {
                PrjEnvelope dstEnvelope = null;
                float srcResolutionX = srcRaster.ResolutionX;
                float srcResolutionY = srcRaster.ResolutionY;
                double srcLeftTopX = srcRaster.CoordEnvelope.MinX;
                double srcLeftTopY = srcRaster.CoordEnvelope.MaxY;
                int srcWidth = srcRaster.Width;
                int srcHeight = srcRaster.Height;
                Size srcSize = new Size(srcWidth, srcHeight);

                int wSample = 1;
                int hSample = 1;
                if (srcWidth > 1000)
                {
                    wSample = srcWidth / 1000;
                }
                if (srcHeight > 1000)
                {
                    hSample = srcHeight / 1000;
                }
                double[] xs = new double[(srcWidth / wSample) * (srcHeight / hSample)];
                double[] ys = new double[(srcWidth / wSample) * (srcHeight / hSample)];
                int index = 0;//非真实的索引号，采样后的
                for (int rowInx = 0; rowInx <= (srcHeight - hSample); rowInx += hSample)
                {
                    for (int colInx = 0; colInx <= (srcWidth - wSample); colInx += wSample)
                    {
                        xs[index] = srcLeftTopX + colInx * srcResolutionX;
                        ys[index] = srcLeftTopY - rowInx * srcResolutionY;
                        index++;
                    }
                }
                _rasterProjector.ComputeDstEnvelope(srcSpatialRef, xs, ys, srcSize, dstSpatialRef, out dstEnvelope, null);
                prjSettings.OutEnvelope = dstEnvelope;
            }
            GC.Collect();
            ProjectToLDF(srcRaster, prjSettings, dstSpatialRef, progressCallback, weight, zoom);
        }

        /// <summary>
        /// 投影主要方法
        /// </summary>
        /// <param name="srcRaster"></param>
        /// <param name="prjSettings"></param>
        /// <param name="dstSpatialRef"></param>
        /// <param name="progressCallback"></param>
        /// <param name="dataSets"></param>
        /// <param name="hdf5attrs"></param>
        /// <param name="outfilename"></param>
        public void ProjectNew(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback, Dictionary<string, H5T.H5Type> dataSets)
        {
            ISpatialReference srcSpatialRef = srcRaster.SpatialRef;
            _projectionTransform = ProjectionTransformFactory.GetProjectionTransform(srcSpatialRef, dstSpatialRef);
            ArgsCheck(srcRaster, prjSettings, dstSpatialRef);
            ReadySession(srcRaster, prjSettings, dstSpatialRef, progressCallback);
            if (prjSettings.OutEnvelope == null || prjSettings.OutEnvelope == PrjEnvelope.Empty)
            {
                PrjEnvelope dstEnvelope = null;
                float srcResolutionX = srcRaster.ResolutionX;
                float srcResolutionY = srcRaster.ResolutionY;
                double srcLeftTopX = srcRaster.CoordEnvelope.MinX;
                double srcLeftTopY = srcRaster.CoordEnvelope.MaxY;
                int srcWidth = srcRaster.Width;
                int srcHeight = srcRaster.Height;
                Size srcSize = new Size(srcWidth, srcHeight);

                int wSample = 1;
                int hSample = 1;
                if (srcWidth > 1000)
                {
                    wSample = srcWidth / 1000;
                }
                if (srcHeight > 1000)
                {
                    hSample = srcHeight / 1000;
                }
                double[] xs = new double[(srcWidth / wSample) * (srcHeight / hSample)];
                double[] ys = new double[(srcWidth / wSample) * (srcHeight / hSample)];
                int index = 0;//非真实的索引号，采样后的
                for (int rowInx = 0; rowInx <= (srcHeight - hSample); rowInx += hSample)
                {
                    for (int colInx = 0; colInx <= (srcWidth - wSample); colInx += wSample)
                    {
                        xs[index] = srcLeftTopX + colInx * srcResolutionX;
                        ys[index] = srcLeftTopY - rowInx * srcResolutionY;
                        index++;
                    }
                }
                _rasterProjector.ComputeDstEnvelope(srcSpatialRef, xs, ys, srcSize, dstSpatialRef, out dstEnvelope, null);
                prjSettings.OutEnvelope = dstEnvelope;
            }
            GC.Collect();

            ProjectRaster(srcRaster, prjSettings, progressCallback, dataSets);
        }

        private void ProjectToLDF(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback, double weight, float zoom)
        {
            //...此处计算分块大小。
            string outFilename = prjSettings.OutPathAndFileName;
            CheckAndCreateDir(Path.GetDirectoryName(outFilename));
            using (IRasterDataProvider prdWriter = CreateOutFile(srcRaster, prjSettings, dstSpatialRef, srcRaster.DataType))
            {
                switch (srcRaster.DataType)
                {
                    case enumDataType.Atypism:
                        throw new Exception("不支持混合类型数据");
                        break;
                    case enumDataType.Bits:
                        ProjectRaster<sbyte>(srcRaster, prjSettings, progressCallback, prdWriter, 0, weight, zoom);
                        break;
                    case enumDataType.Byte:
                        ProjectRaster<byte>(srcRaster, prjSettings, progressCallback, prdWriter, 0, weight, zoom);
                        break;
                    case enumDataType.Double:
                        ProjectRaster<Double>(srcRaster, prjSettings, progressCallback, prdWriter, 0, weight, zoom);
                        break;
                    case enumDataType.Float:
                        ProjectRaster<float>(srcRaster, prjSettings, progressCallback, prdWriter, 0, weight, zoom);
                        break;
                    case enumDataType.Int16:
                        ProjectRaster<Int16>(srcRaster, prjSettings, progressCallback, prdWriter, 0, weight, zoom);
                        break;
                    case enumDataType.Int32:
                        ProjectRaster<Int32>(srcRaster, prjSettings, progressCallback, prdWriter, 0, weight, zoom);
                        break;
                    case enumDataType.Int64:
                        ProjectRaster<Int64>(srcRaster, prjSettings, progressCallback, prdWriter, 0, weight, zoom);
                        break;
                    case enumDataType.UInt16:
                        ProjectRaster<UInt16>(srcRaster, prjSettings, progressCallback, prdWriter, 0, weight, zoom);
                        break;
                    case enumDataType.UInt32:
                        ProjectRaster<UInt32>(srcRaster, prjSettings, progressCallback, prdWriter, 0, weight, zoom);
                        break;
                    case enumDataType.UInt64:
                        ProjectRaster<UInt64>(srcRaster, prjSettings, progressCallback, prdWriter, 0, weight, zoom);
                        break;
                    case enumDataType.Unknow:
                    default:
                        throw new Exception("未知数据类型");
                        break;
                }
            }
            GC.Collect();
        }

        private void ProjectRaster<T>(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, Action<int, string> progressCallback, IRasterDataProvider prdWriter, T fillValue)
        {
            ProjectRaster<T>(srcRaster, prjSettings, progressCallback, prdWriter, fillValue, 1, 1);
        }

        private void ProjectRaster<T>(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, Action<int, string> progressCallback, IRasterDataProvider prdWriter, T fillValue, double dataWeight, float zoom)
        {
            enumDataType dataType = prdWriter.DataType;
            PrjEnvelope outEnvelope = prjSettings.OutEnvelope;
            float outResolutionX = prjSettings.OutResolutionX;
            float outResolutionY = prjSettings.OutResolutionY;
            Size outSize = prjSettings.OutSize;
            int outWidth = outSize.Width;
            int outHeight = outSize.Height;
            int outBandCount = prjSettings.OutBandNos.Length;
            if (progressCallback != null)
                progressCallback(_readyProgress++, "投影准备");
            int srcHeight = srcRaster.Height;
            int srcWidth = srcRaster.Width;
            PrjPoint outLeftTopPoint = outEnvelope.LeftTop;
            float srcResolutionX = srcRaster.ResolutionX;
            float srcResolutionY = srcRaster.ResolutionY;
            Size srcSize = new Size(srcWidth, srcHeight);
            double outLtPointX = outLeftTopPoint.X;
            double outLtPointY = outLeftTopPoint.Y;
            PrjEnvelope srcEnvelope = new PrjEnvelope(srcRaster.CoordEnvelope.MinX, srcRaster.CoordEnvelope.MaxX, srcRaster.CoordEnvelope.MinY, srcRaster.CoordEnvelope.MaxY);
            long mem = MemoryHelper.GetAvalidPhyMemory();
#if! WIN64
            mem = mem < 800 * 1024 * 1024 ? mem : 800 * 1024 * 1024;
#endif
            long maxLimit = mem / (6 * 8 * 2);
            int rowStep = (int)(maxLimit / outWidth);
            if (rowStep == 0)
                rowStep = 1;
            if (rowStep > outHeight)
                rowStep = outHeight;
            int stepCount = (int)(Math.Ceiling((double)outHeight / rowStep));
            int percent = 0;
            int progress = 0;
            int progressCount = outBandCount * stepCount + stepCount * 2;

            for (int oRow = 0; oRow < outHeight; oRow += rowStep)
            {
                if (progressCallback != null)
                {
                    percent++;
                    progress = percent * 100 / progressCount;
                    progressCallback(progress, string.Format("投影完成{0}%", progress));
                }
                if (oRow + rowStep > outHeight)
                    rowStep = outHeight - oRow;
                Size outStepSize = new Size(outWidth, rowStep);
                double[] xs = new double[outWidth * rowStep];
                double[] ys = new double[outWidth * rowStep];
                double oY = oRow * outResolutionY;
                Parallel.For(0, rowStep, j =>
                {
                    double x;
                    double y;
                    int index;
                    y = outLtPointY - j * outResolutionY - oY;
                    for (int i = 0; i < outWidth; i++)
                    {
                        x = outLtPointX + i * outResolutionX;
                        index = i + j * outWidth;
                        xs[index] = x;
                        ys[index] = y;
                    }
                });
                _projectionTransform.InverTransform(xs, ys);
                PrjEnvelope tEnvelope = PrjEnvelope.GetEnvelope(xs, ys, null);
                tEnvelope.Extend(srcResolutionX, srcResolutionY * 4);
                tEnvelope = PrjEnvelope.Intersect(tEnvelope, srcEnvelope);
                if (tEnvelope == null || tEnvelope.IsEmpty)
                    continue;
                Size tSize = tEnvelope.GetSize(srcResolutionX, srcResolutionY);
                int tBeginRow = -1, tEndRow = -1, tBeginCol = -1, tEndCol = -1;
                int oBeginRow = -1, oEndRow = -1, oBeginCol = -1, oEndCol = -1;
                PrjBlockHelper.ComputeBeginEndRowCol(srcEnvelope, srcSize, tEnvelope, tSize,
                    ref oBeginRow, ref oBeginCol, ref oEndRow, ref oEndCol,
                    ref tBeginRow, ref tBeginCol, ref tEndRow, ref tEndCol);
                int srcStepWidth = oEndCol - oBeginCol;
                int srcStepHeight = oEndRow - oBeginRow;
                Size srcStepSize = new Size(srcStepWidth, srcStepHeight);
                T[] srcBandData = new T[srcStepWidth * srcStepHeight];
                T[] dstBandData = new T[outWidth * rowStep];
                double srcStepLeftTopX = tEnvelope.MinX;
                double srcStepLeftTopY = tEnvelope.MaxY;
                double srcStepRightBottomX = tEnvelope.MaxX;
                double srcStepRightBottomY = tEnvelope.MinY;
                UInt16[] rows = new UInt16[outWidth * rowStep];  //正向查找表
                UInt16[] cols = new UInt16[outWidth * rowStep];  //正向查找表
                if (progressCallback != null)
                {
                    percent++;
                    progress = percent * 100 / progressCount;
                    progressCallback(progress, string.Format("投影完成{0}%", progress));
                }
                Parallel.For(0, rowStep, j =>
                {
                    double x;
                    double y;
                    int index;
                    for (int i = 0; i < outWidth; i++)
                    {
                        index = i + j * outWidth;
                        x = xs[index];
                        y = ys[index];
                        if (x >= srcStepLeftTopX && x <= srcStepRightBottomX && y <= srcStepLeftTopY && y >= srcStepRightBottomY)
                        {
                            cols[index] = (UInt16)((x - srcStepLeftTopX) / srcResolutionX + 0.5);
                            rows[index] = (UInt16)((srcStepLeftTopY - y) / srcResolutionY + 0.5);
                        }
                    }
                });
                xs = null;
                ys = null;
                for (int b = 0; b < outBandCount; b++)
                {
                    if (progressCallback != null)
                    {
                        percent++;
                        progress = percent * 100 / progressCount;
                        progressCallback(progress, string.Format("投影完成{0}%", progress));
                    }
                    ReadBandData<T>(srcBandData, b, oBeginCol, oBeginRow, srcStepWidth, srcStepHeight, dataType);

                    //用于测试全图输出结果，用于查看插值的效果：
#if DebugEaseGrid
                    GCHandle hTe = GCHandle.Alloc(srcBandData, GCHandleType.Pinned);
                    try
                    {Random random = new Random(12);
                        IntPtr bufferPtr = hTe.AddrOfPinnedObject();
                        Int16[] tmp = new Int16[srcBandData.Length];
                        for (int i = 0; i < srcBandData.Length; i++)
                        {
                            tmp[i] = (Int16)random.Next(200, 255);
                        }
                        Marshal.Copy(tmp, 0, bufferPtr, tmp.Length);
                    }
                    finally
                    {
                        hTe.Free();
                    }
#endif
                    _rasterProjector.Project<T>(srcBandData, srcStepSize, rows, cols, outStepSize, dstBandData, fillValue, 0, null);
                    if (dataWeight == 1)
                        WriteDataToLDF<T>(prdWriter, dataType, outWidth, rowStep, oRow, dstBandData, b);
                    else
                        WriteDataToLDF<T>(prdWriter, dataType, outWidth, rowStep, oRow, dstBandData, b, dataWeight, zoom);
                }
                rows = null;
                rows = null;
                srcBandData = null;
                dstBandData = null;
            }
        }

        private void WriteDataToLDF<T>(IRasterDataProvider prdWriter, enumDataType dataType, int outWidth, int rowStep, int oRow, T[] dstBandData, int b)
        {
            IRasterBand band = null;
            try
            {
                band = prdWriter.GetRasterBand(b + 1);
                GCHandle h = GCHandle.Alloc(dstBandData, GCHandleType.Pinned);
                try
                {
                    IntPtr bufferPtr = h.AddrOfPinnedObject();
                    band.Write(0, oRow, outWidth, rowStep, bufferPtr, dataType, outWidth, rowStep);
                }
                finally
                {
                    h.Free();
                }
            }
            finally
            {
                //这里不能释放，由于大部分band是记录在RasterDataProvider中的数组中的，如果释放后，下次取就会出错
                //if (band!=null&&band is IGDALRasterBand)
                //{
                //    band.Dispose();
                //}
            }
        }

        private void WriteDataToLDF<T>(IRasterDataProvider prdWriter, enumDataType dataType, int outWidth, int rowStep, int oRow, T[] dstBandData, int b, double dataweight, float zoom)
        {
            IRasterBand band = null;

            for (int i = 0; i < dstBandData.Length; i++)
                dstBandData[i] = (T)Convert.ChangeType(Convert.ToDouble(dstBandData[i]) * dataweight * zoom, typeof(T));
            try
            {
                band = prdWriter.GetRasterBand(b + 1);
                GCHandle h = GCHandle.Alloc(dstBandData, GCHandleType.Pinned);
                try
                {
                    IntPtr bufferPtr = h.AddrOfPinnedObject();
                    band.Write(0, oRow, outWidth, rowStep, bufferPtr, dataType, outWidth, rowStep);
                }
                finally
                {
                    h.Free();
                }
            }
            finally
            {
                //这里不能释放，由于大部分band是记录在RasterDataProvider中的数组中的，如果释放后，下次取就会出错
                //if (band!=null&&band is IGDALRasterBand)
                //{
                //    band.Dispose();
                //}
            }
        }

        /// <summary>
        /// 投影操作-重载方法
        /// </summary>
        /// <typeparam name="T">传递的数据集数据类型</typeparam>
        /// <param name="srcRaster">原始的数据驱动</param>
        /// <param name="prjSettings">投影设置类对象</param>
        /// <param name="progressCallback">回调</param>
        /// <param name="outhdf5name">导出的hdf5</param>
        /// <param name="fillValue">填充值？</param>
        private void ProjectRaster(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, Action<int, string> progressCallback, Dictionary<string, H5T.H5Type> dataSets)
        {
            enumDataType dataType = enumDataType.UInt16;
            PrjEnvelope outEnvelope = prjSettings.OutEnvelope;
            float outResolutionX = prjSettings.OutResolutionX;
            float outResolutionY = prjSettings.OutResolutionY;
            Size outSize = prjSettings.OutSize;
            int outWidth = outSize.Width;
            int outHeight = outSize.Height;
            int outBandCount = prjSettings.OutBandNos.Length;
            if (progressCallback != null)
                progressCallback(_readyProgress++, "投影准备");
            int srcHeight = srcRaster.Height;
            int srcWidth = srcRaster.Width;
            PrjPoint outLeftTopPoint = outEnvelope.LeftTop;
            float srcResolutionX = srcRaster.ResolutionX;
            float srcResolutionY = srcRaster.ResolutionY;
            Size srcSize = new Size(srcWidth, srcHeight);
            double outLtPointX = outLeftTopPoint.X;
            double outLtPointY = outLeftTopPoint.Y;
            PrjEnvelope srcEnvelope = new PrjEnvelope(srcRaster.CoordEnvelope.MinX, srcRaster.CoordEnvelope.MaxX, srcRaster.CoordEnvelope.MinY, srcRaster.CoordEnvelope.MaxY);
            long mem = MemoryHelper.GetAvalidPhyMemory();
#if! WIN64
            mem = mem < 800 * 1024 * 1024 ? mem : 800 * 1024 * 1024;
#endif
            long maxLimit = mem / (6 * 8 * 2);
            int rowStep = (int)(maxLimit / outWidth);
            if (rowStep == 0)
                rowStep = 1;
            if (rowStep > outHeight)
                rowStep = outHeight;
            int stepCount = (int)(Math.Ceiling((double)outHeight / rowStep));

            int progressCount = outBandCount * stepCount + stepCount * 2;
            H5FileId fileId = H5F.create(prjSettings.OutPathAndFileName, H5F.CreateMode.ACC_TRUNC);
            List<string> listdatasets = new List<string>();
            foreach (string name in dataSets.Keys)
            {
                listdatasets.Add(name);
            }
            HDF5.Hdf5Operator oper = new HDF5.Hdf5Operator(srcRaster.fileName);
            for (int bandnum = 0; bandnum < outBandCount; bandnum++)
            {
                enumDataType systype = enumDataType.Unknow;
                H5T.H5Type h5filetype = dataSets[listdatasets[bandnum]];
                systype = GetSysDataType(dataSets[listdatasets[bandnum]]);
                switch (systype)
                {
                    case enumDataType.Int16:
                        RastAndProject<Int16>(outSize, h5filetype, fileId, listdatasets[bandnum], progressCallback, outWidth, outHeight, rowStep, srcRaster, prjSettings, bandnum, 0, oper);
                        break;
                    case enumDataType.Int32:
                        RastAndProject<Int32>(outSize, h5filetype, fileId, listdatasets[bandnum], progressCallback, outWidth, outHeight, rowStep, srcRaster, prjSettings, bandnum, 0, oper);
                        break;
                    case enumDataType.Int64:
                        RastAndProject<Int64>(outSize, h5filetype, fileId, listdatasets[bandnum], progressCallback, outWidth, outHeight, rowStep, srcRaster, prjSettings, bandnum, 0, oper);
                        break;
                    case enumDataType.UInt16:
                        RastAndProject<UInt16>(outSize, h5filetype, fileId, listdatasets[bandnum], progressCallback, outWidth, outHeight, rowStep, srcRaster, prjSettings, bandnum, 0, oper);
                        break;
                    case enumDataType.UInt32:
                        RastAndProject<UInt32>(outSize, h5filetype, fileId, listdatasets[bandnum], progressCallback, outWidth, outHeight, rowStep, srcRaster, prjSettings, bandnum, 0, oper);
                        break;
                    case enumDataType.UInt64:
                        RastAndProject<UInt64>(outSize, h5filetype, fileId, listdatasets[bandnum], progressCallback, outWidth, outHeight, rowStep, srcRaster, prjSettings, bandnum, 0, oper);
                        break;
                    case enumDataType.Double:
                        RastAndProject<Double>(outSize, h5filetype, fileId, listdatasets[bandnum], progressCallback, outWidth, outHeight, rowStep, srcRaster, prjSettings, bandnum, 0, oper);
                        break;
                    case enumDataType.Float:
                        RastAndProject<Single>(outSize, h5filetype, fileId, listdatasets[bandnum], progressCallback, outWidth, outHeight, rowStep, srcRaster, prjSettings, bandnum, 0, oper);
                        break;
                    case enumDataType.Byte:
                        RastAndProject<Byte>(outSize, h5filetype, fileId, listdatasets[bandnum], progressCallback, outWidth, outHeight, rowStep, srcRaster, prjSettings, bandnum, 0, oper);
                        break;
                    default: break;
                }
            }

            Dictionary<string, string> filekeyvalue = oper.GetAttributes();
            foreach (string key in filekeyvalue.Keys)
            {
                string value = filekeyvalue[key];
                HDFAttributeDef attrdef = new HDFAttributeDef(key, typeof(string), filekeyvalue[key].Length, value);
                WriteHdfAttributes.WriteHdfAttribute(fileId, attrdef);
            }

            //写四角坐标信息
            HDFAttributeDef lefttopx = new HDFAttributeDef("Left-Top Latitude", typeof(string), prjSettings.OutEnvelope.LeftTop.Y.ToString().Length, prjSettings.OutEnvelope.LeftTop.Y.ToString());
            WriteHdfAttributes.WriteHdfAttribute(fileId, lefttopx);
            HDFAttributeDef lefttopy = new HDFAttributeDef("Left-Top Longtitude", typeof(string), prjSettings.OutEnvelope.LeftTop.X.ToString().Length, prjSettings.OutEnvelope.LeftTop.X.ToString());
            WriteHdfAttributes.WriteHdfAttribute(fileId, lefttopy);
            HDFAttributeDef boottomrightx = new HDFAttributeDef("Bottom-Right Latitude", typeof(string), prjSettings.OutEnvelope.RightBottom.Y.ToString().Length, prjSettings.OutEnvelope.RightBottom.Y.ToString());
            WriteHdfAttributes.WriteHdfAttribute(fileId, boottomrightx);
            HDFAttributeDef boottomrighty = new HDFAttributeDef("Bottom-Right Longtitude", typeof(string), prjSettings.OutEnvelope.RightBottom.X.ToString().Length, prjSettings.OutEnvelope.RightBottom.X.ToString());
            WriteHdfAttributes.WriteHdfAttribute(fileId, boottomrighty);
            //投影信息
            string projtype = "Geopraphic Latitude longtitude";
            HDFAttributeDef projecttype = new HDFAttributeDef("Projection Type", typeof(string), projtype.Length, projtype);
            WriteHdfAttributes.WriteHdfAttribute(fileId, projecttype);
            H5F.close(fileId);
        }

        private enumDataType GetSysDataType(H5T.H5Type type)
        {
            enumDataType systype = enumDataType.Atypism;
            switch (type)
            {
                case H5T.H5Type.NATIVE_SCHAR:
                case H5T.H5Type.NATIVE_UCHAR:
                    systype = enumDataType.Byte;
                    break;
                case H5T.H5Type.NATIVE_DOUBLE: systype = enumDataType.Double;
                    break;
                case H5T.H5Type.NATIVE_SHORT: systype = enumDataType.Int16;
                    break;
                case H5T.H5Type.NATIVE_INT: systype = enumDataType.Int32;
                    break;
                case H5T.H5Type.NATIVE_USHORT: systype = enumDataType.UInt16;
                    break;
                case H5T.H5Type.NATIVE_UINT: systype = enumDataType.UInt32;
                    break;
                case H5T.H5Type.NATIVE_FLOAT: systype = enumDataType.Float;
                    break;
                default: systype = enumDataType.Unknow;
                    break;
            }
            return systype;
        }

        public void RastAndProject<T>(Size outSize, H5T.H5Type ht5type, H5FileId fileId, string dataset, Action<int, string> progressCallback, int outWidth, int outHeight, int rowStep, IRasterDataProvider srcRaster, FilePrjSettings prjSettings, int bandnum, T fillValue, HDF5.Hdf5Operator ope)
        {
            T[,] data = new T[outSize.Height, outSize.Width];
            // 定义数据集操作部分
            long[] dims = new long[2];
            dims[0] = Convert.ToInt64(outSize.Height);
            dims[1] = Convert.ToInt64(outSize.Width);
            H5DataSpaceId dspaceId = H5S.create_simple(2, dims);
            // Define datatype for the data in the file.
            H5DataTypeId dtypeId = H5T.copy(ht5type);
            // Create the data set DATASETNAME.

            H5DataSetId dsetId = H5D.create(fileId, dataset, dtypeId, dspaceId);
            for (int oRow = 0; oRow < outHeight; oRow += rowStep)
            {
                #region 分块读取数据集
                if (progressCallback != null)
                {
                    percent++;
                    progress = percent * 100 / progressCount;
                    progressCallback(progress, string.Format("投影完成{0}%", progress));
                }
                if (oRow + rowStep > outHeight)
                    rowStep = outHeight - oRow;
                Size outStepSize = new Size(outWidth, rowStep);
                double[] xs = new double[outWidth * rowStep];
                double[] ys = new double[outWidth * rowStep];
                double oY = oRow * prjSettings.OutResolutionY;
                Parallel.For(0, rowStep, j =>
                {
                    double x;
                    double y;
                    int index;
                    y = prjSettings.OutEnvelope.LeftTop.Y - j * prjSettings.OutResolutionY - oY;
                    for (int i = 0; i < outWidth; i++)
                    {
                        x = prjSettings.OutEnvelope.LeftTop.Y + i * prjSettings.OutResolutionX;
                        index = i + j * outWidth;
                        xs[index] = x;
                        ys[index] = y;
                    }
                });
                _projectionTransform.InverTransform(xs, ys);
                PrjEnvelope tEnvelope = PrjEnvelope.GetEnvelope(xs, ys, null);
                tEnvelope.Extend(srcRaster.ResolutionX, srcRaster.ResolutionY * 4);
                PrjEnvelope srcEnvelope = new PrjEnvelope(srcRaster.CoordEnvelope.MinX, srcRaster.CoordEnvelope.MaxX, srcRaster.CoordEnvelope.MinY, srcRaster.CoordEnvelope.MaxY);
                tEnvelope = PrjEnvelope.Intersect(tEnvelope, srcEnvelope);
                if (tEnvelope == null || tEnvelope.IsEmpty)
                    continue;
                Size tSize = tEnvelope.GetSize(srcRaster.ResolutionX, srcRaster.ResolutionY);
                int tBeginRow = -1, tEndRow = -1, tBeginCol = -1, tEndCol = -1;
                int oBeginRow = -1, oEndRow = -1, oBeginCol = -1, oEndCol = -1;
                PrjBlockHelper.ComputeBeginEndRowCol(srcEnvelope, new Size(srcRaster.Width, srcRaster.Height), tEnvelope, tSize,
                    ref oBeginRow, ref oBeginCol, ref oEndRow, ref oEndCol,
                    ref tBeginRow, ref tBeginCol, ref tEndRow, ref tEndCol);
                int srcStepWidth = oEndCol - oBeginCol;
                int srcStepHeight = oEndRow - oBeginRow;
                Size srcStepSize = new Size(srcStepWidth, srcStepHeight);
                T[] srcBandData = new T[srcStepWidth * srcStepHeight];
                T[] dstBandData = new T[outWidth * rowStep];
                double srcStepLeftTopX = tEnvelope.MinX;
                double srcStepLeftTopY = tEnvelope.MaxY;
                double srcStepRightBottomX = tEnvelope.MaxX;
                double srcStepRightBottomY = tEnvelope.MinY;
                UInt16[] rows = new UInt16[outWidth * rowStep];  //正向查找表
                UInt16[] cols = new UInt16[outWidth * rowStep];  //正向查找表
                if (progressCallback != null)
                {
                    percent++;
                    progress = percent * 100 / progressCount;
                    progressCallback(progress, string.Format("投影完成{0}%", progress));
                }
                Parallel.For(0, rowStep, j =>
                {
                    double x;
                    double y;
                    int index;
                    for (int i = 0; i < outWidth; i++)
                    {
                        index = i + j * outWidth;
                        x = xs[index];
                        y = ys[index];
                        if (x >= srcStepLeftTopX && x <= srcStepRightBottomX && y <= srcStepLeftTopY && y >= srcStepRightBottomY)
                        {
                            cols[index] = (UInt16)((x - srcStepLeftTopX) / srcRaster.ResolutionX + 0.5);
                            rows[index] = (UInt16)((srcStepLeftTopY - y) / srcRaster.ResolutionY + 0.5);
                        }
                    }
                });
                xs = null;
                ys = null;
                #endregion
                //分块读取不同波段数据
                ReadBandData<T>(srcBandData, bandnum, oBeginCol, oBeginRow, srcStepWidth, srcStepHeight, GetSysDataType(ht5type));
                //投影不同分块的数据集
                _rasterProjector.Project<T>(srcBandData, srcStepSize, rows, cols, outStepSize, dstBandData, fillValue, 0, null);

                for (int i = 0; i < outStepSize.Width; i++)
                {
                    for (int j = 0; j < outStepSize.Height; j++)
                    {
                        int index = j * outStepSize.Width + i;
                        data[j + oRow, i] = dstBandData[index];
                    }
                }

                rows = null;
                rows = null;
                srcBandData = null;
                dstBandData = null;
            }
            H5D.write(dsetId, dtypeId, new H5Array<T>(data));
            //写数据集属性
            //Dictionary<string, string> dsetkeyvalue = ope.GetAttributes(dataset);
            //foreach (string key in dsetkeyvalue.Keys)
            //{
            //    string value = dsetkeyvalue[key];
            //    HDFAttributeDef attrdef = new HDFAttributeDef(key, typeof(string), dsetkeyvalue[key].Length, value);
            //    WriteHdfAttributes.WriteHdfAttribute(dsetId, attrdef);
            //}
            H5D.close(dsetId);
        }

        private void ReadBandData<T>(T[] bandData, int bandIndex, int xOffset, int yOffset, int blockWidth, int blockHeight, enumDataType dataType)
        {
            IRasterBand latBand = _rasterDataBands[bandIndex];//
            GCHandle h = h = GCHandle.Alloc(bandData, GCHandleType.Pinned);
            try
            {
                IntPtr bufferPtr = h.AddrOfPinnedObject();
                latBand.Read(xOffset, yOffset, blockWidth, blockHeight, bufferPtr, dataType, blockWidth, blockHeight);
            }
            finally
            {
                h.Free();
            }
        }

        private IRasterDataProvider CreateOutFile(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatial, enumDataType dataType)
        {
            float resolutionX = prjSettings.OutResolutionX;
            float resolutionY = prjSettings.OutResolutionY;
            string[] options = null;
            string outformat = string.IsNullOrWhiteSpace(prjSettings.OutFormat) ? "LDF" : prjSettings.OutFormat.ToUpper();
            string driver = "";
            if (outformat == "TIFF")
            {
                driver = "GDAL";
                options = new string[]{
                    "DRIVERNAME=GTiff",
                    "TFW=YES",
                    "WKT=" + dstSpatial.ToWKTString(),
                    "GEOTRANSFORM=" + string.Format("{0},{1},{2},{3},{4},{5}",prjSettings.OutEnvelope.MinX, resolutionX,0, prjSettings.OutEnvelope.MaxY,0, -resolutionY)
                    };
            }
            else if (outformat == "MEM")
            {
                driver = "MEM";
                options = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=MEM",
                "WITHHDR=TRUE",
                "SPATIALREF=" + dstSpatial.ToProj4String(),
                "MAPINFO={" + 1 + "," + 1 + "}:{" + prjSettings.OutEnvelope.MinX + "," + prjSettings.OutEnvelope.MaxY + "}:{" + resolutionX + "," + resolutionY + "}"
                    };
            }
            else
            {
                driver = "LDF";
                options = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "SPATIALREF=" + dstSpatial.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + prjSettings.OutEnvelope.MinX + "," + prjSettings.OutEnvelope.MaxY + "}:{" + resolutionX + "," + resolutionY + "}",
                            "BANDNAMES=" + BandNameString(srcRaster , prjSettings.OutBandNos)
                    };
            }
            Size outSize = prjSettings.OutEnvelope.GetSize(resolutionX, resolutionY);
            int bandCount = prjSettings.OutBandNos.Length;
            string filename = prjSettings.OutPathAndFileName;
            return CreateOutFile(driver, filename, bandCount, outSize, dataType, options);
        }

        internal IRasterDataProvider CreateOutFile(string driver, string outfilename, int dstBandCount, Size outSize, enumDataType dataType, string[] options)
        {
            CheckAndCreateDir(Path.GetDirectoryName(outfilename));
            IRasterDataDriver outdrv = GeoDataDriver.GetDriverByName(driver) as IRasterDataDriver;
            return outdrv.Create(outfilename, outSize.Width, outSize.Height, dstBandCount, dataType, options) as IRasterDataProvider;
        }

        private void GetBlockNumber(Size size, out int blockXNum, out int blockYNum, out int blockWidth, out int blockHeight)
        {
            uint mem = MemoryHelper.GetAvalidPhyMemory();
            int w = size.Width;
            int h = size.Height;
            blockXNum = 1;
            blockYNum = 1;
            blockWidth = w;
            blockHeight = h;
            int MaxX = 8000;
            int MaxY = 2000;
            if (mem / 1024 / 1024 > 1500)//1.5GB以上
            {
                MaxX = w;
                MaxY = 16000000 / w;
            }
            else if (mem / 1024 / 1024 > 800)
            {
                MaxX = w;
                MaxY = 8000000 / w;
            }
            else if (mem / 1024 / 1024 > 400)
            {
                MaxX = w;
                MaxY = 2000000 / w;
            }
            else if (mem / 1024 / 1024 > 200)
            {
                MaxX = w;
                MaxY = 1000000 / w;
            }
            else if (mem / 1024 / 1024 > 100)
            {
                MaxX = w;
                MaxY = 500000 / w;
            }
            else
            {
                throw new Exception("当前系统资源不足以完成该操作，请释放部分资源后再试。");
            }
            if (size.Width * size.Height <= MaxX * MaxY)
                return;
            while (blockWidth > MaxX)
            {
                blockXNum++;
                blockWidth = (int)Math.Floor((double)w / blockXNum);
            }
            while (blockHeight > MaxY)
            {
                blockYNum++;
                blockHeight = (int)Math.Floor((double)h / blockYNum);
            }
        }

        private void ArgsCheck(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster");
            if (prjSettings == null)
                throw new ArgumentNullException("prjSettings");
            if (dstSpatialRef == null)
                throw new ArgumentNullException("dstSpatialRef");
            if (srcRaster.SpatialRef == null)
                throw new ArgumentNullException("srcRaster.SpatialRef");
            if (string.IsNullOrWhiteSpace(prjSettings.OutFormat))
            {
                if (srcRaster.Driver.Name == "MEM")
                    prjSettings.OutFormat = "MEM";
                //else if (srcRaster.Driver.Name == "GDAL")
                //{
                //}
                else
                    prjSettings.OutFormat = "LDF";
            }
            if (prjSettings.OutResolutionX == 0f || prjSettings.OutResolutionY == 0f)
            {
                if ((dstSpatialRef.ProjectionCoordSystem == null && srcRaster.SpatialRef.ProjectionCoordSystem == null) ||
                    (dstSpatialRef.ProjectionCoordSystem != null && srcRaster.SpatialRef.ProjectionCoordSystem != null))
                {
                    prjSettings.OutResolutionX = srcRaster.ResolutionX;
                    prjSettings.OutResolutionY = srcRaster.ResolutionY;
                }
                else if (dstSpatialRef.ProjectionCoordSystem != null && srcRaster.SpatialRef.ProjectionCoordSystem == null)
                {
                    prjSettings.OutResolutionX = srcRaster.ResolutionX * 100000F;
                    prjSettings.OutResolutionY = srcRaster.ResolutionY * 100000F;
                }
                else if (dstSpatialRef.ProjectionCoordSystem == null && srcRaster.SpatialRef.ProjectionCoordSystem != null)
                {
                    prjSettings.OutResolutionX = srcRaster.ResolutionX / 100000F;
                    prjSettings.OutResolutionY = srcRaster.ResolutionY / 100000F;
                }
            }
            if (prjSettings.OutBandNos == null || prjSettings.OutBandNos.Length == 0)
            {
                List<int> bandNoList = new List<int>();
                for (int i = 1; i <= srcRaster.BandCount; i++)
                {
                    bandNoList.Add(i);
                }
                prjSettings.OutBandNos = bandNoList.ToArray();
            }
        }

        private void ReadySession(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            if (_curSession == null || _curSession != srcRaster || _isBeginSession)
            {
                Size srcSize = new Size(srcRaster.Width, srcRaster.Height);
                if (progressCallback != null)
                    progressCallback(_readyProgress++, "投影准备");
                _rasterDataBands = TryCreateRasterDataBands(srcRaster, prjSettings, progressCallback);
                _isBeginSession = false;
            }
        }

        private IRasterBand[] TryCreateRasterDataBands(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, Action<int, string> progressCallback)
        {
            List<IRasterBand> rasterBands = new List<IRasterBand>();
            for (int i = 0; i < prjSettings.OutBandNos.Length; i++)
            {
                if (progressCallback != null)
                    progressCallback(_readyProgress++, "投影准备");
                int bandNo = prjSettings.OutBandNos[i];
                IRasterBand band = srcRaster.GetRasterBand(bandNo);
                rasterBands.Add(band);
            }
            return rasterBands.ToArray();
        }

        public override bool IsSupport(string fileName)
        {
            throw new NotImplementedException();
        }

        public override void ComputeDstEnvelope(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, out PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override FilePrjSettings CreateDefaultPrjSettings()
        {
            throw new NotImplementedException();
        }

        protected override void DoRadiation(IRasterDataProvider srcImgRaster, int i, ushort[] srcBandData, float[] solarZenithData, Size srcBlockImgSize, Size angleSize)
        {
            return;
        }

        protected override void ReadLocations(IRasterDataProvider srcRaster, out double[] xs, out double[] ys, out Size locationSize)
        {
            throw new NotImplementedException();
        }

        public override IRasterDataProvider Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, IRasterDataProvider dstRaster, int beginBandIndex, Action<int, string> progressCallback)
        {
            return base.Project(srcRaster, prjSettings, dstRaster, beginBandIndex, progressCallback);
        }
    }
}
