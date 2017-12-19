using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HDF5DotNet;
using GeoDo.HDF;
using System.IO;

using GeoDo.RSS.Core.DF;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.UI.AddIn.RemoveLines
{
    public class HDF5RemoveLines: IHDF5RemoveLines
    {
        string fileName;
        public HDF5RemoveLines()
        { }

        #region 依据波段号写入
        /// <summary>
        /// HDF5数据去条带
        /// </summary>
        /// <param name="bandNo">指定波段号的数据集</param>
        /// <param name="inputFileName">需要操作的文件</param>
        /// <param name="writeMode">操作模式</param>
        /// <param name="outputFileName">输出文件</param>
        public void RemoveLines(int[] bandNo, string inputFileName, HDFWriteMode writeMode, string outputFileName, Action<int, string> progressCallback)
        {
            IRasterDataProvider prd = null;
            
            //获取将要处理的文件
            GetOperateFileName(inputFileName, writeMode, outputFileName);

            if (bandNo == null || bandNo.Length == 0 || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            try
            {
                List<Dictionary<string, int>> count = new List<Dictionary<string, int>>();

                for (int i = 0; i < bandNo.Length; i++)
                {
                    count.Add(GetMappingInfo(bandNo[i]));
                }
                if (count == null || count.Count == 0)
                {
                    MessageBox.Show("缺少波段对应关系！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                float percent = (float)(1 / (float)count.Count);
                int progessIndex = 0;

                //打印进度
                if (progressCallback != null)
                {
                    progressCallback(0, "开始处理数据...");
                }
                //获取波段提供者
                //progessIndex++;
                prd = GetRasDataProvide(inputFileName);
                if (prd == null)
                {
                    MessageBox.Show("错误文件!");
                    return;
                }
                for (int i = 0; i < bandNo.Length; i++)
                {
                    //获取指定波段号的波段信息
                    IRasterBand band = prd.GetRasterBand(bandNo[i]);
                    //获取波段的数据
                    object data = ReadOldBrandData(band, prd.DataType);
                    //读取映射波段对应的数据集名称
                    Dictionary<string, int> mappingDataSet = GetMappingInfo(bandNo[i]);
                    //int bandIndex = 0;
                    foreach (KeyValuePair<string, int> dataSet in mappingDataSet)
                    {
                        DoRemoveLineOperate(dataSet.Key, prd.DataType, data, dataSet.Value, band.Width, band.Height, progessIndex, percent, progressCallback);
                        progessIndex++;
                    }
                }
                if (progressCallback != null)
                    progressCallback(100, "数据处理完毕!");
                if (writeMode == HDFWriteMode.ReWrite)
                {
                    File.Copy(fileName, inputFileName, true);
                    File.Delete(fileName);
                }
                MessageBox.Show("处理成功！");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                prd.Dispose();
            }
        }

        /// <summary>
        /// 获取操作的文件
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <param name="writeMode"></param>
        /// <param name="outputFileName"></param>
        /// <returns></returns>
        private void GetOperateFileName(string inputFileName, HDFWriteMode writeMode, string outputFileName)
        {
            //如果为另存模式
            if (writeMode == HDFWriteMode.SaveAS)
            {
                if (string.IsNullOrEmpty(outputFileName))
                {
                    MessageBox.Show("输出文件不能为空", "警告!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!Directory.Exists(outputFileName.Substring(0, outputFileName.LastIndexOf(@"\"))))
                {
                    Directory.CreateDirectory(outputFileName.Substring(0, outputFileName.LastIndexOf(@"\")));
                }
                fileName = outputFileName;
                File.Copy(inputFileName, fileName, true);
            }
            else
            {
                //如果为覆盖模式
                fileName = inputFileName.Substring(0, inputFileName.LastIndexOf('.')) + "_Cpoy"
                    + inputFileName.Substring(inputFileName.LastIndexOf('.'));

                File.Copy(inputFileName, fileName, true);
            }
        }

        /// <summary>
        /// 获取数据提供者
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private IRasterDataProvider GetRasDataProvide(string fileName)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(fileName) as IRasterDataProvider;
            return prd;
        }

        /// <summary>
        /// 获取波段映射关系
        /// </summary>
        /// <param name="BandNo"></param>
        /// <returns></returns>
        private Dictionary<string, int> GetMappingInfo(int BandNo)
        {
            Dictionary<string, int> dataSetInfo = new Dictionary<string, int>();
            if (BandNo == 5)
            {
                dataSetInfo.Add("EV_250_Aggr.1KM_Emissive", 0);
            }
            else if (BandNo == 6)
            {
                dataSetInfo.Add("EV_1KM_RefSB", 0);
            }
            return dataSetInfo;
        }

        /// <summary>
        /// 获取原始波段的数据
        /// </summary>
        /// <param name="band"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private unsafe object ReadOldBrandData(IRasterBand band, enumDataType dataType)
        {
            object retObject = null;
            switch (dataType)
            {
                case enumDataType.Byte:
                    Byte[] Bytebuffer = null;
                    Bytebuffer = new Byte[band.Width * band.Height];
                    fixed (Byte* ptr = Bytebuffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(0, 0, band.Width, band.Height, bufferPtr, dataType, band.Width, band.Height);
                    }
                    retObject = Bytebuffer;
                    break;
                case enumDataType.UInt16:
                    UInt16[] UInt16buffer = null;
                    UInt16buffer = new UInt16[band.Width * band.Height];
                    fixed (UInt16* ptr = UInt16buffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(0, 0, band.Width, band.Height, bufferPtr, dataType, band.Width, band.Height);
                    }
                    retObject = UInt16buffer;
                    break;
                case enumDataType.Int16:
                    Int16[] Int16buffer = null;
                    Int16buffer = new Int16[band.Width * band.Height];
                    fixed (Int16* ptr = Int16buffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(0, 0, band.Width, band.Height, bufferPtr, dataType, band.Width, band.Height);
                    }
                    retObject = Int16buffer;
                    break;
                case enumDataType.UInt32:
                    UInt32[] UInt32buffer = null;
                    UInt32buffer = new UInt32[band.Width * band.Height];
                    fixed (UInt32* ptr = UInt32buffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(0, 0, band.Width, band.Height, bufferPtr, dataType, band.Width, band.Height);
                    }
                    retObject = UInt32buffer;
                    break;
                case enumDataType.Int32:
                    Int32[] Int32buffer = null;
                    Int32buffer = new Int32[band.Width * band.Height];
                    fixed (Int32* ptr = Int32buffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(0, 0, band.Width, band.Height, bufferPtr, dataType, band.Width, band.Height);
                    }
                    retObject = Int32buffer;
                    break;
                case enumDataType.UInt64:
                    UInt64[] UInt64buffer = null;
                    UInt64buffer = new UInt64[band.Width * band.Height];
                    fixed (UInt64* ptr = UInt64buffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(0, 0, band.Width, band.Height, bufferPtr, dataType, band.Width, band.Height);
                    }
                    retObject = UInt64buffer;
                    break;
                case enumDataType.Int64:
                    Int64[] Int64buffer = null;
                    Int64buffer = new Int64[band.Width * band.Height];
                    fixed (Int64* ptr = Int64buffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(0, 0, band.Width, band.Height, bufferPtr, dataType, band.Width, band.Height);
                    }
                    retObject = Int64buffer;
                    break;
                case enumDataType.Float:
                    float[] floatbuffer = null;
                    floatbuffer = new float[band.Width * band.Height];
                    fixed (float* ptr = floatbuffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(0, 0, band.Width, band.Height, bufferPtr, dataType, band.Width, band.Height);
                    }
                    retObject = floatbuffer;
                    break;
                case enumDataType.Double:
                    Double[] Doublebuffer = null;
                    Doublebuffer = new Double[band.Width * band.Height];
                    fixed (Double* ptr = Doublebuffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(0, 0, band.Width, band.Height, bufferPtr, dataType, band.Width, band.Height);
                    }
                    retObject = Doublebuffer;
                    break;
            }
            return retObject;
        }

        /// <summary>
        /// 去除条带
        /// </summary>
        /// <param name="dataSetName">数据集名称</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="data">原始数据</param>
        /// <param name="mappingBandNo">映射波段号</param>
        /// <param name="bandWith">宽</param>
        /// <param name="bandHeight">高</param>
        private unsafe void DoRemoveLineOperate(string dataSetName, enumDataType dataType, object data, int mappingBandNo, int bandWith, int bandHeight, int processIndex, float samplePercent, Action<int, string> progressCallback)
        {
            H5DataTypeId dataTypeId = null;
            switch (dataType)
            {
                case enumDataType.UInt16:
                    dataTypeId = H5T.copy(H5T.H5Type.NATIVE_USHORT);
                    UInt16[] uint16Values = data as UInt16[];
                    //重写数据集的值，去除条带操作
                    float[] uint16buffer = new float[uint16Values.Length];
                    for (int i = 0; i < uint16buffer.Length; i++)
                    {
                        uint16buffer[i] = (float)(uint16Values[i]);
                    }
                    int pro = 0;
                    fixed (float* ptr = uint16buffer)
                    {
                        RemoveLineAPI.ProgressCallback callback =
                        (value) =>
                        {
                            pro = (int)(value * samplePercent + processIndex * samplePercent * 100);
                            progressCallback(pro, "处理数据中...");
                        };
                        RemoveLineAPI.remove_lines(ptr, Math.Min(bandWith, bandHeight), Math.Max(bandWith, bandHeight),
                            callback);
                    }
                    
                    for (int i = 0; i < uint16Values.Length; i++)
                    {
                        uint16Values[i] = Convert.ToUInt16(Math.Abs(uint16buffer[i]));
                    }
                    //波段对应数据集的数据层
                    ReWriteDataSet(dataSetName, dataTypeId, uint16Values, mappingBandNo);
                    break;
                //case enumDataType.Int16:
                //    dataTypeId = H5T.copy(H5T.H5Type.NATIVE_SHORT);
                //    Int16[] int16Values = data as Int16[];
                //    float[] int16buffer = new float[int16Values.Length];
                //    for (int i = 0; i < int16buffer.Length; i++)
                //    {
                //        int16buffer[i] = (float)(int16Values[i]);
                //    }
                //    fixed (float* ptr = int16buffer)
                //    {
                //        fixed (int* pos = indexPos)
                //        {
                //            RemoveLineAPI.remove_lines(ptr, Math.Min(bandWith, bandHeight), Math.Max(bandWith, bandHeight), pos);
                //        }
                //    }
                //    for (int i = 0; i < int16Values.Length; i++)
                //    {
                //        int16Values[i] = Convert.ToInt16(Math.Abs(int16buffer[i]));
                //    }
                //    //波段对应数据集的数据层
                //    ReWriteDataSet(dataSetName, dataTypeId, int16Values, mappingBandNo);
                //    break;
                //case enumDataType.UInt32:
                //    dataTypeId = H5T.copy(H5T.H5Type.NATIVE_UINT);
                //    UInt32[] uint32Values = data as UInt32[];
                //    //重写数据集的值，去除条带操作
                //    float[] uint32buffer = new float[uint32Values.Length];
                //    for (int i = 0; i < uint32buffer.Length; i++)
                //    {
                //        uint32buffer[i] = (float)(uint32Values[i]);
                //    }
                //    fixed (float* ptr = uint32buffer)
                //    {
                //        fixed (int* pos = indexPos)
                //        {
                //            RemoveLineAPI.remove_lines(ptr, Math.Min(bandWith, bandHeight), Math.Max(bandWith, bandHeight), pos);
                //        }
                //    }
                //    for (int i = 0; i < uint32Values.Length; i++)
                //    {
                //        uint32Values[i] = Convert.ToUInt32(Math.Abs(uint32buffer[i]));
                //    }
                //    //波段对应数据集的数据层
                //    ReWriteDataSet(dataSetName, dataTypeId, uint32Values, mappingBandNo);
                //    break;
                //case enumDataType.Int32:
                //    dataTypeId = H5T.copy(H5T.H5Type.NATIVE_INT);
                //    Int32[] int32Values = data as Int32[];
                //    float[] int32buffer = new float[int32Values.Length];
                //    for (int i = 0; i < int32buffer.Length; i++)
                //    {
                //        int32buffer[i] = (float)(int32Values[i]);
                //    }
                //    fixed (float* ptr = int32buffer)
                //    {
                //        fixed (int* pos = indexPos)
                //        {
                //            RemoveLineAPI.remove_lines(ptr, Math.Min(bandWith, bandHeight), Math.Max(bandWith, bandHeight), pos);
                //        }
                //    }
                //    for (int i = 0; i < int32Values.Length; i++)
                //    {
                //        int32Values[i] = Convert.ToInt32(Math.Abs(int32buffer[i]));
                //    }
                //    //波段对应数据集的数据层
                //    ReWriteDataSet(dataSetName, dataTypeId, int32Values, mappingBandNo);
                //    break;
                //case enumDataType.UInt64:
                //    dataTypeId = H5T.copy(H5T.H5Type.NATIVE_ULLONG);
                //    UInt64[] uint64Values = data as UInt64[];
                //    //重写数据集的值，去除条带操作
                //    float[] uint64buffer = new float[uint64Values.Length];
                //    for (int i = 0; i < uint64buffer.Length; i++)
                //    {
                //        uint64buffer[i] = (float)(uint64Values[i]);
                //    }
                //    fixed (float* ptr = uint64buffer)
                //    {
                //        fixed (int* pos = indexPos)
                //        {
                //            RemoveLineAPI.remove_lines(ptr, Math.Min(bandWith, bandHeight), Math.Max(bandWith, bandHeight), pos);
                //        }
                //    }
                //    for (int i = 0; i < uint64Values.Length; i++)
                //    {
                //        uint64Values[i] = Convert.ToUInt64(Math.Abs(uint64buffer[i]));
                //    }
                //    //波段对应数据集的数据层
                //    ReWriteDataSet(dataSetName, dataTypeId, uint64Values, mappingBandNo);
                //    break;
                //case enumDataType.Int64:
                //    dataTypeId = H5T.copy(H5T.H5Type.NATIVE_SHORT);
                //    Int64[] int64Values = data as Int64[];
                //    float[] int64buffer = new float[int64Values.Length];
                //    for (int i = 0; i < int64buffer.Length; i++)
                //    {
                //        int64buffer[i] = (float)(int64Values[i]);
                //    }
                //    fixed (float* ptr = int64buffer)
                //    {
                //        fixed (int* pos = indexPos)
                //        {
                //            RemoveLineAPI.remove_lines(ptr, Math.Min(bandWith, bandHeight), Math.Max(bandWith, bandHeight), pos);
                //        }
                //    }
                //    for (int i = 0; i < int64Values.Length; i++)
                //    {
                //        int64Values[i] = Convert.ToInt64(Math.Abs(int64buffer[i]));
                //    }
                //    //波段对应数据集的数据层
                //    ReWriteDataSet(dataSetName, dataTypeId, int64Values, mappingBandNo);
                //    break;
                //case enumDataType.Double:
                //    dataTypeId = H5T.copy(H5T.H5Type.NATIVE_DOUBLE);
                //    double[] doubleValues = data as double[];
                //    float[] doublebuffer = new float[doubleValues.Length];
                //    for (int i = 0; i < doublebuffer.Length; i++)
                //    {
                //        doublebuffer[i] = (float)(doubleValues[i]);
                //    }
                //    fixed (float* ptr = doublebuffer)
                //    {
                //        fixed (int* pos = indexPos)
                //        {
                //            RemoveLineAPI.remove_lines(ptr, Math.Min(bandWith, bandHeight), Math.Max(bandWith, bandHeight), pos);
                //        }
                //    }
                //    for (int i = 0; i < doubleValues.Length; i++)
                //    {
                //        doubleValues[i] = Convert.ToDouble(Math.Abs(doublebuffer[i]));
                //    }
                //    //波段对应数据集的数据层
                //    ReWriteDataSet(dataSetName, dataTypeId, doubleValues, mappingBandNo);
                //    break;
                //case enumDataType.Float:
                //    dataTypeId = H5T.copy(H5T.H5Type.NATIVE_FLOAT);
                //    float[] floatValues = data as float[];
                //    fixed (float* ptr = floatValues)
                //    {
                //        fixed (int* pos = indexPos)
                //        {
                //            RemoveLineAPI.remove_lines(ptr, Math.Min(bandWith, bandHeight), Math.Max(bandWith, bandHeight), pos);
                //        }
                //    }
                //    //波段对应数据集的数据层
                //    ReWriteDataSet(dataSetName, dataTypeId, floatValues, mappingBandNo);
                //    break;
            }
        }

        /// <summary>
        /// 重写数据集的值（去条带的数据）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dataSetName">数据集的名称</param>
        /// <param name="dataTypeId">数据集的类型ID</param>
        /// <param name="values">去条带之后数据</param>
        /// <param name="BrandNo">在数据集的维度从0开始</param>
        private void ReWriteDataSet<T>(string dataSetName, H5DataTypeId dataTypeId, T[] values, int BrandNo)
        {
            H5FileId _h5FileId = null;
            H5DataSetId dataSetId = null;
            H5DataSpaceId spaceid = null;
            try
            {
                _h5FileId = H5F.open(fileName, H5F.OpenMode.ACC_RDWR);
                //先找出含有指定波段的数据集
                dataSetId = H5D.open(_h5FileId, dataSetName);
                spaceid = H5D.getSpace(dataSetId);
                long[] dims = H5S.getSimpleExtentDims(spaceid);//得到数据数组的大小，比如[3,1800,2048]
                int rank = H5S.getSimpleExtentNDims(spaceid);//得到数据数组的维数，比如3
                H5S.close(spaceid);
                //根据数据集的名字获取数据集的ID
                int size = 0;
                if (rank == 0)
                {
                    size = 1;
                }
                else if (rank == 1)
                {
                    size = Convert.ToInt32(dims[0]);
                }
                else if (rank == 2)
                {
                    size = Convert.ToInt32(dims[0] * dims[1]);
                }
                else if (rank == 3)
                {
                    size = Convert.ToInt32(dims[0] * dims[1] * dims[2]);
                }
                T[] v = new T[size];
                //从数据集中读取原始数据
                H5D.read<T>(dataSetId, dataTypeId, new H5Array<T>(v));
                //将波段校正后的数据读取赋给相应的波段
                for (int i = BrandNo; i < values.Length; i++)
                {
                    v[i] = values[i];
                }
                H5D.write<T>(dataSetId, dataTypeId, new H5Array<T>(v));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                H5D.close(dataSetId);
                H5F.close(_h5FileId);
            }
        }

        #endregion

        #region 依据数据集映射关系
        
        /// <summary>
        /// HDF5数据去条带
        /// </summary>
        /// <param name="mappingDataSet">数据集和波段号的映射关系</param>
        /// <param name="inputFileName">需要操作的文件</param>
        /// <param name="writeMode">操作模式</param>
        /// <param name="outputFileName">输出文件</param>
        public void RemoveLines(Dictionary<string, int> mappingDataSet, string inputFileName, HDFWriteMode writeMode, string outputFileName, Action<int, string> progressCallback)
        {

            if (mappingDataSet == null || mappingDataSet.Count == 0)
            {
                MessageBox.Show("缺少波段映射关系!");
                return;
            }
            try
            {
                //获取将要处理的文件
                GetOperateFileName(inputFileName, writeMode, outputFileName);
                if (string.IsNullOrEmpty(fileName))
                {
                    return;
                }

                float percent = (float)(1 / (float)mappingDataSet.Count);
                int progessIndex = 0;

                //打印进度
                if (progressCallback != null)
                {
                    progressCallback(0, "开始处理数据...");
                }
                int bandWidth, bandHeight;
                enumDataType dataType;
                object data = null;
                foreach (KeyValuePair<string, int> dataSet in mappingDataSet)
                {
                    //打印进度
                    if (progressCallback != null)
                    {
                        progressCallback(0, "正在处理数据...");
                    }
                    ReadOldDataSetData(dataSet.Key, dataSet.Value, out bandWidth, out bandHeight, out dataType, out data);

                    DoRemoveLineOperate(dataSet.Key, dataType, data, dataSet.Value, bandWidth, bandHeight, progessIndex, percent, progressCallback);
                    progessIndex++;
                }
                if (writeMode == HDFWriteMode.ReWrite)
                {
                    File.Copy(fileName, inputFileName, true);
                    File.Delete(fileName);
                }
            }
            catch(Exception e)
            {
                
            }
        }

        private void ReadOldDataSetData(string dataSetName, int bandIndex, out int bandWidth, out int bandHeight, out enumDataType dataType, out object retObject)
        {
            bandHeight = bandWidth = 0;
            dataType = enumDataType.UInt16;
            retObject = null;
            H5FileId _h5FileId = null;
            H5DataSpaceId spaceid = null;
            H5DataSetId dataSetId = null;
            try
            {
                _h5FileId = H5F.open(fileName, H5F.OpenMode.ACC_RDONLY);
                //先找出含有指定波段的数据集
                dataSetId = H5D.open(_h5FileId, dataSetName);
                spaceid = H5D.getSpace(dataSetId);
                long[] dims = H5S.getSimpleExtentDims(spaceid);//得到数据数组的大小，比如[3,1800,2048]
                int rank = H5S.getSimpleExtentNDims(spaceid);//得到数据数组的维数，比如3
                int size = 0;
                if (rank == 1)
                {
                    bandHeight = bandWidth = 1;
                    size = bandWidth * bandHeight * rank;
                }
                else if (rank == 2)
                {
                    bandWidth = Convert.ToInt32(dims[0]);
                    bandHeight = Convert.ToInt32(dims[1]);
                    size = bandWidth * bandHeight;
                }
                else if (rank == 3)
                {
                    List<long> r = dims.ToList<long>();
                    r.Sort();
                    long[] temp = r.ToArray();

                    bandWidth = Convert.ToInt32(temp[1]);
                    bandHeight = Convert.ToInt32(temp[2]);
                    size = bandWidth * bandHeight * Convert.ToInt32(temp[0]);
                }
                int outSize = bandWidth * bandHeight;
                H5DataTypeId typeId = H5D.getType(dataSetId);
                H5T.H5TClass typeClass = H5T.getClass(typeId);//得到数据集的类型
                int dataSize = H5T.getSize(typeId);
                H5DataTypeId newTypeId = null;
                switch (typeClass)
                {
                    case H5T.H5TClass.INTEGER:
                        H5T.Sign sign = H5T.Sign.TWOS_COMPLEMENT;
                        sign = H5T.getSign(typeId);
                        switch (dataSize)
                        {
                            case 1:
                                newTypeId = H5T.copy(H5T.H5Type.NATIVE_B8);
                                retObject = ReadArray<byte>(size, dataSetId, newTypeId, bandIndex, outSize);
                                dataType = enumDataType.Byte;
                                break;
                            case 2:
                                switch (sign)
                                {
                                    case H5T.Sign.TWOS_COMPLEMENT:
                                        newTypeId = H5T.copy(H5T.H5Type.NATIVE_SHORT);
                                        retObject = ReadArray<Int16>(size, dataSetId, newTypeId, bandIndex, outSize);
                                        dataType = enumDataType.Int16;
                                        break;
                                    case H5T.Sign.UNSIGNED:
                                        newTypeId = H5T.copy(H5T.H5Type.NATIVE_USHORT);
                                        retObject = ReadArray<UInt16>(size, dataSetId, newTypeId, bandIndex, outSize);
                                        dataType = enumDataType.UInt16;
                                        break;
                                }
                                break;
                            case 4:
                                switch (sign)
                                {
                                    case H5T.Sign.TWOS_COMPLEMENT:
                                        newTypeId = H5T.copy(H5T.H5Type.NATIVE_INT);
                                        retObject = ReadArray<Int32>(size, dataSetId, newTypeId, bandIndex, outSize);
                                        dataType = enumDataType.Int32;
                                        break;
                                    case H5T.Sign.UNSIGNED:
                                        newTypeId = H5T.copy(H5T.H5Type.NATIVE_UINT);
                                        retObject = ReadArray<UInt32>(size, dataSetId, newTypeId, bandIndex, outSize);
                                        dataType = enumDataType.UInt32;
                                        break;
                                }
                                break;
                            case 8:
                                switch (sign)
                                {
                                    case H5T.Sign.TWOS_COMPLEMENT:
                                        newTypeId = H5T.copy(H5T.H5Type.NATIVE_LONG);
                                        retObject = ReadArray<Int64>(size, dataSetId, newTypeId, bandIndex, outSize);
                                        dataType = enumDataType.Int64;
                                        break;
                                    case H5T.Sign.UNSIGNED:
                                        newTypeId = H5T.copy(H5T.H5Type.NATIVE_ULONG);
                                        retObject = ReadArray<UInt64>(size, dataSetId, newTypeId, bandIndex, outSize);
                                        dataType = enumDataType.UInt64;
                                        break;
                                }
                                break;
                        }
                        break;
                    case H5T.H5TClass.FLOAT:
                        switch (dataSize)
                        {
                            case 4:
                                newTypeId = H5T.copy(H5T.H5Type.NATIVE_FLOAT);
                                retObject = ReadArray<float>(size, dataSetId, newTypeId, bandIndex, outSize);
                                dataType = enumDataType.Float;
                                break;
                            case 8:
                                newTypeId = H5T.copy(H5T.H5Type.NATIVE_DOUBLE);
                                retObject = ReadArray<double>(size, dataSetId, newTypeId, bandIndex, outSize);
                                dataType = enumDataType.Double;
                                break;
                        }
                        break;
                }
            }
            finally
            {
                H5S.close(spaceid);
                H5D.close(dataSetId);
                H5F.close(_h5FileId);
            }
        }

        private T[] ReadArray<T>(int size, H5DataSetId dsId, H5DataTypeId typeId,int bandIndex, int outSize)
        {
            T[] v = new T[size];
            H5D.read<T>(dsId, typeId, new H5Array<T>(v));
            T[] outdata = new T[outSize];
            int index = bandIndex * outSize;
            for (int i = 0; i < outdata.Length; i++)
            {
                outdata[i] = v[i + index];
            }
            return outdata;
        }
        #endregion

    }

    public enum HDFWriteMode
    {
        ReWrite,
        SaveAS
    }
}
