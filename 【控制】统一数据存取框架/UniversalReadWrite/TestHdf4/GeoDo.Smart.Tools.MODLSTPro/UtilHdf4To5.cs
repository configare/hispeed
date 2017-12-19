using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GeoDo.HDF4;
using GeoDo.RSS.DF.HDF4.Cloudsat;
using HDF5DotNet;

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
        public static void ConvertHdf4To5(SHdf4To5 hdf4To5)
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

            Action<string, int, int> messageAction = hdf4To5.MessageAction;
            ConvertHdf4To5(hdf4FileAttrs, hdf4To5.Hdf5Name, messageAction);
        }

        private static void ConvertHdf4To5(Hdf4FileAttrs hdf4FileAttrs, string f5name,
            Action<string, int, int> messageAction)
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
                for (int k = 0; k < sdscount; k++)
                {
                    ConvertHdf4To5BySds(hdf4FileAttrs, messageAction, k, nyf5, nxf5, rank, fileId);
                }

                HDFAttributeDef[] attributeDef5s = hdf4FileAttrs.GetHDFAttributeDefs();
                foreach (HDFAttributeDef attributeDef5 in attributeDef5s)
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
            int rank, H5FileId fileId)
        {
            var dataField = hdf4FileAttrs.Hdf4FileAttr.DataFields[k];
            string sdName = dataField.DataFieldName;
            string dataType = dataField.DataType;
            HDF4Helper.DataTypeDefinitions dtaDefinitions = Utility.EnumParse(dataType,
                HDF4Helper.DataTypeDefinitions.DFNT_UINT16);
            Type type = Utility.GetBaseType(dtaDefinitions);

            if (type == typeof (byte))
            {
                ConvertHdf4To5BySds<byte>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction);
            }
            else if (type == typeof (char))
            {
                ConvertHdf4To5BySds<char>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction);
            }
            else if (type == typeof (double))
            {
                ConvertHdf4To5BySds<double>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction);
            }
            else if (type == typeof (short))
            {
                ConvertHdf4To5BySds<short>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction);
            }
            else if (type == typeof (ushort))
            {
                ConvertHdf4To5BySds<ushort>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction);
            }
            else if (type == typeof (long))
            {
                ConvertHdf4To5BySds<long>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction);
            }
            else if (type == typeof (float))
            {
                ConvertHdf4To5BySds<float>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction);
            }
            else //if (type == typeof (int))
            {
                ConvertHdf4To5BySds<int>(hdf4FileAttrs, nyf5, nxf5, k, rank, fileId, sdName, dtaDefinitions, messageAction);
            }
        }

        private static void ConvertHdf4To5BySds<T>(Hdf4FileAttrs hdf4FileAttrs, int nyf5, int nxf5, int k, int rank,
            H5FileId fileId, string sdName, HDF4Helper.DataTypeDefinitions dtaDefinitions,
            Action<string, int, int> messageAction)
        {
            T[,] data = new T[nyf5, nxf5];
            //foreach (Hdf4FileAttr hdf4FileAttr in hdf4FileAttrs)
            for (int i = 0; i < hdf4FileAttrs.Count; i++)
            {
                Hdf4FileAttr hdf4FileAttr = hdf4FileAttrs[i];
                if (messageAction != null)
                    messageAction(String.Format("正在转换数据集 {0}", sdName), k, i);
                H4SDS sd = hdf4FileAttr.H4File.Datasets[k];
                GetDataByHdf4(sd, rank, hdf4FileAttr, data);
            }

            // 保存数据
            // Describe the size of the array and create the data space for fixed
            // size dataset.
            long[] dims = new long[rank];
            dims[0] = Convert.ToInt64(nyf5);
            dims[1] = Convert.ToInt64(nxf5);
            H5DataSpaceId dspaceId = H5S.create_simple(rank, dims);
            H5T.H5Type h5Type = Utility.GetH5Type(dtaDefinitions);
            // Define datatype for the data in the file.
            H5DataTypeId dtypeId = H5T.copy(h5Type);

            // Create the data set DATASETNAME.
            H5DataSetId dsetId = H5D.create(fileId, sdName, dtypeId, dspaceId);

            // Write the one-dimensional data set array
            H5D.write(dsetId, new H5DataTypeId(h5Type), new H5Array<T>(data));//H5A

            HDFAttributeDef[] attributeDef5s = hdf4FileAttrs[0].DatasetsAttributeDefs[k];
            foreach (HDFAttributeDef attributeDef5 in attributeDef5s)
            {
                WriteHdfAttributes.WriteHdfAttribute(dsetId, attributeDef5);
            }

            // Close dataset and file.
            H5D.close(dsetId);
        }

        private static void GetDataByHdf4<T>(H4SDS sd, int rank, Hdf4FileAttr hdf4FileAttr, T[,] data)
        {
            int xOffset = hdf4FileAttr.XOffset;
            int yOffset = hdf4FileAttr.YOffset;

            if (rank == 2)
            {
                int nx = hdf4FileAttr.XDim;
                int ny = hdf4FileAttr.YDim;
                int buffersize = nx*ny;
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