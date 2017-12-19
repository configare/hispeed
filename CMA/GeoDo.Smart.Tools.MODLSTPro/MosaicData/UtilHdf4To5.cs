using System;
using System.IO;
using System.Runtime.InteropServices;
using GeoDo.HDF4;
using GeoDo.RSS.DF.HDF4.Cloudsat;
using HDF5DotNet;
using GeoDo.HDF5;
using GeoDo.RasterProject;
using System.Drawing;
using System.Threading.Tasks;
using GeoDo.Project;
using GeoDo.FileProject;
using System.Collections.Generic;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public static class UtilHdf4To5
    {
        /****************************** Demo ******************************/
        /*   SHdf4To5 hdf4To5 = new SHdf4To5(f4names, f5name) {MessageAction = MessageAction(), SdsCountAction = null};
            try
            {
                UtilHdf4To5.ConvertHdf4To5(hdf4To5);
            }
            catch (Exception ex)
            {
                MessageBox.Show("拼接失败！" + ex.Message);
            }
            MessageBox.Show("转换完成！");
         */

        /// <summary>
        /// Hdf4拼接转换为Hdf5
        /// </summary>
        /// <param name="hdf4To5"></param>
        /// <returns></returns>
        public static void ConvertHdf4To5(SHdf4To5 hdf4To5, PrjEnvelope outenvelope,
                                          float dstResolution, ISpatialReference dstSpatialReference)
        {
            var hdf4FileAttrs = PreConvertHdf4To5(hdf4To5);
            DoConvertHdf4To5(hdf4To5, hdf4FileAttrs, outenvelope, dstResolution, dstSpatialReference);
        }

        public static void DoConvertHdf4To5(SHdf4To5 hdf4To5, Hdf4FileAttrs hdf4FileAttrs, PrjEnvelope outenvelope,
                                            float dstResolution, ISpatialReference dstSpatialReference)
        {
            SEnvelope tempse = new SEnvelope();
            tempse.XMin = outenvelope.MinX;
            tempse.XMax = outenvelope.MaxX;
            tempse.YMin = outenvelope.MinY;
            tempse.YMax = outenvelope.MaxY;
            Action<string, int, int> messageAction = hdf4To5.MessageAction;
            ConvertHdf4To5(hdf4FileAttrs, hdf4To5.Hdf5Name, messageAction, tempse, dstResolution, dstSpatialReference);
            hdf4FileAttrs.Dispose();
        }

        public static Hdf4FileAttrs PreConvertHdf4To5(SHdf4To5 hdf4To5)
        {
            var hdf4FileAttrs = new Hdf4FileAttrs();
            try
            {
                hdf4FileAttrs.AddHdf4Files(hdf4To5.Hdf4Names);
                if (!hdf4FileAttrs.Validate())
                    throw new Exception("输入的Hdf4 元数据信息不一致，不能进行拼接！");

                hdf4FileAttrs.ResetOffset();
                if (hdf4To5.SdsCountAction != null)
                {
                    hdf4To5.SdsCountAction(hdf4FileAttrs.Hdf4FileAttr.DataFields.Count);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("解析Hdf4时出错，具体信息：" + ex.Message, ex);
            }
            return hdf4FileAttrs;
        }

        private static void ConvertHdf4To5(Hdf4FileAttrs hdf4FileAttrs, string f5name,
            Action<string, int, int> messageAction, SEnvelope outenvelope, float dstResolution,
            ISpatialReference dstSpatialReference)
        {
            try
            {
                // Create a new file using H5F_ACC_TRUNC access,
                // default file creation properties, and default file
                // access properties.
                H5FileId fileId = H5F.create(f5name, H5F.CreateMode.ACC_TRUNC);

                int nxf5 = hdf4FileAttrs.Hdf4FileAttr.XDim;
                int nyf5 = hdf4FileAttrs.Hdf4FileAttr.YDim;
                int rank = 2;

                //测试读取的科学数据集及其属性
                int sdscount = hdf4FileAttrs.Hdf4FileAttr.DataFields.Count;

                SEnvelope envelopeNew = null;
                UInt16[] rows = null;//正向查找表
                UInt16[] cols = null;//正向查找表
                Size srcStepSize = Size.Empty;
                Size outSize = Size.Empty;
                try
                {
                    for (int k = 0; k < sdscount; k++)
                    {
                        ConvertHdf4To5BySds(hdf4FileAttrs, messageAction, k, nyf5, nxf5, rank, fileId, outenvelope, dstResolution, dstSpatialReference, ref rows, ref cols, ref srcStepSize, ref outSize);
                        if (envelopeNew == null && dstSpatialReference != null && outenvelope != null)
                            envelopeNew = outenvelope;
                    }
                    if (envelopeNew != null)
                        hdf4FileAttrs.RefreshPointString(envelopeNew);
                }
                finally
                {
                    rows = null;
                    cols = null;
                    srcStepSize = Size.Empty;
                    GC.Collect();
                }

                HDFAttributeDef[] attributeDef5s = hdf4FileAttrs.GetHDFAttributeDefs();
                List<HDFAttributeDef> updateFileAttrs = new List<HDFAttributeDef>();
                updateFileAttrs.AddRange(attributeDef5s);
                if (dstSpatialReference != null && dstSpatialReference.IsSame(SpatialReference.GetDefault()))
                {
                    //投影信息
                    string projtype = "Geopraphic Latitude longtitude";
                    HDFAttributeDef projecttype = new HDFAttributeDef("Projection Type", typeof(string), projtype.Length, projtype);
                    updateFileAttrs.Add(projecttype);
                    //写四角坐标信息
                    HDFAttributeDef lefttopY = new HDFAttributeDef("Left-Top Latitude", typeof(float), 1, new float[] { (float)outenvelope.YMax });
                    updateFileAttrs.Add(lefttopY);
                    HDFAttributeDef lefttopX = new HDFAttributeDef("Left-Top Longtitude", typeof(float), 1, new float[] { (float)outenvelope.XMin });
                    updateFileAttrs.Add(lefttopX);
                    HDFAttributeDef boottomrightY = new HDFAttributeDef("Bottom-Right Latitude", typeof(float), 1, new float[] { (float)outenvelope.YMin });
                    updateFileAttrs.Add(boottomrightY);
                    HDFAttributeDef boottomrightX = new HDFAttributeDef("Bottom-Right Longtitude", typeof(float), 1, new float[] { (float)outenvelope.XMax });
                    updateFileAttrs.Add(boottomrightX);
                    HDFAttributeDef xResolution = new HDFAttributeDef("Longtitude Resolution", typeof(float), 1, new float[] { (float)dstResolution });
                    updateFileAttrs.Add(xResolution);
                    HDFAttributeDef yResolution = new HDFAttributeDef("Latitude Resolution", typeof(float), 1, new float[] { (float)dstResolution });
                    updateFileAttrs.Add(yResolution);
                }
                foreach (HDFAttributeDef attributeDef5 in updateFileAttrs)
                {
                    WriteHdfAttributes.WriteHdfAttribute(fileId, attributeDef5);
                }

                H5F.close(fileId);
            }
            catch (Exception ex)
            {
                throw new Exception("拼接Hdf4时出错，具体信息：" + ex.Message, ex);
            }
        }

        private static void ConvertHdf4To5BySds(Hdf4FileAttrs hdf4FileAttrs, Action<string, int, int> messageAction, int k, int nyf5, int nxf5,
            int rank, H5FileId fileId, SEnvelope envelopeNew, float dstResolution, ISpatialReference dstSpatialReference, ref UInt16[] rows, ref UInt16[] cols, ref Size srcStepSize, ref Size outSize)
        {
            var dataField = hdf4FileAttrs.Hdf4FileAttr.DataFields[k];
            string sdName = dataField.DataFieldName;
            string dataType = dataField.DataType;
            HDF4Helper.DataTypeDefinitions dtaDefinitions = Utility.EnumParse(dataType,
                HDF4Helper.DataTypeDefinitions.DFNT_UINT16);
            Type type = Utility.GetDataType(dtaDefinitions);

            if (type == typeof(byte))
            {
                ConvertHdf4To5BySds<byte>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction, envelopeNew, 0, dstResolution, dstSpatialReference, ref rows, ref cols, ref srcStepSize, ref outSize);
            }
            else if (type == typeof(char))
            {
                ConvertHdf4To5BySds<char>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction, envelopeNew, '0', dstResolution, dstSpatialReference, ref rows, ref cols, ref srcStepSize, ref outSize);
            }
            else if (type == typeof(double))
            {
                ConvertHdf4To5BySds<double>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction, envelopeNew, 0, dstResolution, dstSpatialReference, ref rows, ref cols, ref srcStepSize, ref outSize);
            }
            else if (type == typeof(short))
            {
                ConvertHdf4To5BySds<short>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction, envelopeNew, 0, dstResolution, dstSpatialReference, ref rows, ref cols, ref srcStepSize, ref outSize);
            }
            else if (type == typeof(ushort))
            {
                ConvertHdf4To5BySds<ushort>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction, envelopeNew, 0, dstResolution, dstSpatialReference, ref rows, ref cols, ref srcStepSize, ref outSize);
            }
            else if (type == typeof(long))
            {
                ConvertHdf4To5BySds<long>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction, envelopeNew, 0, dstResolution, dstSpatialReference, ref rows, ref cols, ref srcStepSize, ref outSize);
            }
            else if (type == typeof(float))
            {
                ConvertHdf4To5BySds<float>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction, envelopeNew, 0, dstResolution, dstSpatialReference, ref rows, ref cols, ref srcStepSize, ref outSize);
            }
            else  //(type == typeof(int))
            {
                ConvertHdf4To5BySds<int>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction, envelopeNew, 0, dstResolution, dstSpatialReference, ref rows, ref cols, ref srcStepSize, ref outSize);
            }
        }

        private static void ConvertHdf4To5BySds<T>(Hdf4FileAttrs hdf4FileAttrs, int nyf5, int nxf5, int k, int rank,
            H5FileId fileId, string sdName, HDF4Helper.DataTypeDefinitions dtaDefinitions, Action<string, int, int> messageAction,
            SEnvelope envelopeNew, T fillValue, float dstResolution, ISpatialReference dstSpatialReference, ref UInt16[] rows, ref UInt16[] cols, ref Size srcStepSize, ref Size outSize)
        {
            T[,] data = new T[nyf5, nxf5];
            H5DataSetId dsetId = null;
            T[,] dataNew = null;
            SDataByProject<T> dataByProject = null;
            try
            {
                for (int i = 0; i < hdf4FileAttrs.Count; i++)
                {
                    Hdf4FileAttr hdf4FileAttr = hdf4FileAttrs[i];
                    if (messageAction != null)
                        messageAction(String.Format("正在转换数据集 {0}", sdName), k, i);
                    H4SDS sd = hdf4FileAttr.H4File.Datasets[k];
                    GetDataByHdf4(sd, rank, hdf4FileAttr, data);
                }

                dataNew = data;
                if (outSize.IsEmpty)
                    outSize = new Size(nxf5, nyf5);
                if (dstSpatialReference != null && dstSpatialReference.IsSame(SpatialReference.GetDefault()))
                {
                    if (rows == null && cols == null)
                    {
                        PrjEnvelope srcEnvelope = new PrjEnvelope(hdf4FileAttrs.Hdf4FileAttr.Envelope.XMin, hdf4FileAttrs.Hdf4FileAttr.Envelope.XMax, hdf4FileAttrs.Hdf4FileAttr.Envelope.YMin, hdf4FileAttrs.Hdf4FileAttr.Envelope.YMax);
                        PrjEnvelope outenvelpoe = new PrjEnvelope(envelopeNew.XMin, envelopeNew.XMax, envelopeNew.YMin, envelopeNew.YMax);
                        FilePrjSettings prjSetting = new FilePrjSettings();
                        prjSetting.OutEnvelope = outenvelpoe;
                        prjSetting.OutResolutionX = dstResolution;
                        prjSetting.OutResolutionY = dstResolution;
                        outSize = prjSetting.OutSize;
                        Size inSize = new Size(hdf4FileAttrs.Hdf4FileAttr.XDim, hdf4FileAttrs.Hdf4FileAttr.YDim);
                        float dstResoultion = dstResolution;
                        float srcResoultionX = (float)(hdf4FileAttrs.Hdf4FileAttr.CellWidth);
                        float srcResoultionY = (float)(hdf4FileAttrs.Hdf4FileAttr.CellHeight);
                        dataByProject = GetDataByProject<T>(dataNew, srcEnvelope, outenvelpoe, inSize, outSize, dstResoultion, srcResoultionX, srcResoultionY, fillValue, dstSpatialReference, out rows, out cols, out srcStepSize);
                    }
                    else
                    {
                        T[,] dstData = new T[outSize.Height, outSize.Width];
                        dataByProject = DoProject<T>(dataNew, fillValue, ref outSize, ref srcStepSize, dstData, rows, cols);
                    }
                    if (dataByProject != null)
                        dataNew = dataByProject.Data;
                }

                long[] dims = new long[rank];
                dims[0] = Convert.ToInt64(outSize.Height);
                dims[1] = Convert.ToInt64(outSize.Width);
                H5DataSpaceId dspaceId = H5S.create_simple(rank, dims);
                H5T.H5Type h5Type = Utility.GetH5Type(dtaDefinitions);
                // Define datatype for the data in the file.
                H5DataTypeId dtypeId = H5T.copy(h5Type);

                // Create the data set DATASETNAME.
                dsetId = H5D.create(fileId, sdName, dtypeId, dspaceId);
                // Write the one-dimensional data set array
                H5D.write(dsetId, new H5DataTypeId(h5Type), new H5Array<T>(dataNew));//H5A

                HDFAttributeDef[] attributeDef5s = hdf4FileAttrs[0].DatasetsAttributeDefs[k];
                foreach (HDFAttributeDef attributeDef5 in attributeDef5s)
                {
                    WriteHdfAttributes.WriteHdfAttribute(dsetId, attributeDef5);
                }
            }
            finally
            {
                // Close dataset and file.
                if (dsetId != null)
                    H5D.close(dsetId);
                if (data != null)
                    data = null;
                if (dataNew != null)
                    dataNew = null;
                GC.Collect();
            }
        }
        /// <summary>
        ///数据集投影
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="sourcedata">原始数据集</param>
        /// <param name="srcEnvelope">原始数据集坐标范围（正弦坐标系）</param>
        /// <param name="dstEnvelope">目标数据集坐标范围（品面坐标系）</param>
        /// <param name="outSize">输出范围</param>
        /// <param name="dstResoultion">输出分辨率</param>
        /// <param name="srcResoultionX">原始数据集分辨率X</param>
        /// <param name="srcResoultionY">原始数据集分辨率Y</param>
        /// <returns></returns>
        private static SDataByProject<T> GetDataByProject<T>(T[,] sourcedata, PrjEnvelope srcEnvelope, PrjEnvelope dstEnvelope, Size inSize,
            Size outSize, float dstResoultion, float srcResoultionX, float srcResoultionY, T fillValue, ISpatialReference dstSpatialRef, out UInt16[] rows, out UInt16[] cols, out Size srcStepSize)
        {
            rows = null;
            cols = null;
            srcStepSize = Size.Empty;
            IProjectionTransform _projectionTransform;
            //初始化原始坐标系转化对象
            //ISpatialReference dstSpatialRef = SpatialReference.GetDefault();
            string proj4str = "proj4 = +proj=sinu +lon_0=0 +x_0=0 +y_0=0 +a=6371007.181 +b=6371007.181 +units=m +no_defs";
            ISpatialReference srcSpatialRef = SpatialReference.FromProj4(proj4str);
            _projectionTransform = ProjectionTransformFactory.GetProjectionTransform(srcSpatialRef, dstSpatialRef);
            //坐标系转换
            int rowStep = outSize.Height;
            for (int oRow = 0; oRow < outSize.Height; oRow += rowStep)
            {
                if (oRow + rowStep > outSize.Height)
                    rowStep = outSize.Height - oRow;
                Size outStepSize = new Size(outSize.Width, rowStep);
                double[] xs = new double[outSize.Width * rowStep];
                double[] ys = new double[outSize.Width * rowStep];
                double oY = oRow * dstResoultion;
                Parallel.For(0, rowStep, j =>
                {
                    double x;
                    double y;
                    int index;
                    y = dstEnvelope.LeftTop.Y - j * dstResoultion - oY;
                    for (int i = 0; i < outSize.Width; i++)
                    {
                        x = dstEnvelope.LeftTop.X + i * dstResoultion;
                        index = i + j * outSize.Width;
                        xs[index] = x;
                        ys[index] = y;
                    }
                });
                _projectionTransform.InverTransform(xs, ys);

                PrjEnvelope tEnvelope = PrjEnvelope.GetEnvelope(xs, ys, null);
                tEnvelope.Extend(srcResoultionX, srcResoultionY * 4);
                tEnvelope = PrjEnvelope.Intersect(tEnvelope, srcEnvelope);
                if (tEnvelope == null || tEnvelope.IsEmpty)
                    continue;
                Size tSize = tEnvelope.GetSize(srcResoultionX, srcResoultionY);
                int tBeginRow = -1, tEndRow = -1, tBeginCol = -1, tEndCol = -1;
                int oBeginRow = -1, oEndRow = -1, oBeginCol = -1, oEndCol = -1;
                PrjBlockHelper.ComputeBeginEndRowCol(srcEnvelope, new Size(inSize.Width, inSize.Height), tEnvelope, tSize,
                    ref oBeginRow, ref oBeginCol, ref oEndRow, ref oEndCol,
                    ref tBeginRow, ref tBeginCol, ref tEndRow, ref tEndCol);
                int srcStepWidth = oEndCol - oBeginCol;
                int srcStepHeight = oEndRow - oBeginRow;
                srcStepSize = new Size(srcStepWidth, srcStepHeight);

                T[,] dstData = new T[outSize.Height, outSize.Width];
                //double srcStepLeftTopX = tEnvelope.MinX;
                //double srcStepLeftTopY = tEnvelope.MaxY;
                //double srcStepRightBottomX = tEnvelope.MaxX;
                //double srcStepRightBottomY = tEnvelope.MinY;
                double srcStepLeftTopX = srcEnvelope.MinX;
                double srcStepLeftTopY = srcEnvelope.MaxY;
                double srcStepRightBottomX = srcEnvelope.MaxX;
                double srcStepRightBottomY = srcEnvelope.MinY;
                UInt16[] rowTemps = new UInt16[outSize.Width * rowStep];  //正向查找表
                UInt16[] colTemps = new UInt16[outSize.Width * rowStep];  //正向查找表

                Parallel.For(0, rowStep, j =>
                {
                    double x;
                    double y;
                    int index;
                    for (int i = 0; i < outSize.Width; i++)
                    {
                        index = i + j * outSize.Width;
                        x = xs[index];
                        y = ys[index];
                        if (x >= srcStepLeftTopX && x <= srcStepRightBottomX && y <= srcStepLeftTopY && y >= srcStepRightBottomY)
                        {
                            colTemps[index] = (UInt16)((x - srcStepLeftTopX) / srcResoultionX + 0.5);
                            rowTemps[index] = (UInt16)((srcStepLeftTopY - y) / srcResoultionY + 0.5);
                        }
                    }
                });
                rows = rowTemps;
                cols = colTemps;
                return DoProject<T>(sourcedata, fillValue, ref outStepSize, ref srcStepSize, dstData, rows, cols);
            }
            return null;
        }

        private static SDataByProject<T> DoProject<T>(T[,] sourcedata, T fillValue, ref Size outStepSize, ref Size srcStepSize, T[,] dstData, UInt16[] rows, UInt16[] cols)
        {
            //投影不同分块的数据集
            RasterProjector _rasterProjector = new RasterProjector();
            _rasterProjector.ProjectNew<T>(sourcedata, srcStepSize, rows, cols, outStepSize, dstData, fillValue);
            SDataByProject<T> returnvalue = new SDataByProject<T>();
            returnvalue.Data = dstData;
            return returnvalue;
        }

        private static void GetDataByHdf4<T>(H4SDS sd, int rank, Hdf4FileAttr hdf4FileAttr, T[,] data)
        {
            int xOffset = hdf4FileAttr.XOffset;
            int yOffset = hdf4FileAttr.YOffset;

            if (rank == 2)
            {
                int nx = hdf4FileAttr.XDim;
                int ny = hdf4FileAttr.YDim;
                int buffersize = nx * ny;
                // int typesize = HDFDataType.GetSize(sd.Datatype);

                T[] buffer = new T[buffersize];
                GCHandle h = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    IntPtr bufferPtr = h.AddrOfPinnedObject();
                    sd.Read(new int[] { 0, 0 }, null, sd.Dimsizes, bufferPtr);
                }
                finally
                {
                    h.Free();
                }

                // Data and input buffer initialization.
                for (int j = 0; j < ny; j++)
                    for (int i = 0; i < nx; i++)
                    {
                        int index = j * nx + i;
                        int iIndex = i + xOffset;
                        int jIndex = j + yOffset;
                        data[jIndex, iIndex] = buffer[index];
                    }

            }
        }
    }
}