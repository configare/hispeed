using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HDF5DotNet;

namespace FYTools
{
    /// <summary>
    /// HDF5数据操作辅助类
    /// </summary>
    public class HDF5Helper : IDisposable
    {
        private H5FileId _fileId = null;
        private List<String> _fileAttributeNames = new List<String>();
        private List<String> _datasetNames = new List<String>();

        /// <summary>
        /// 返回所有的文件属性
        /// </summary>
        public List<String> FileAttributeNames
        {
            get
            {
                return _fileAttributeNames;
            }
        }

        /// <summary>
        /// 返回所有的数据集名称
        /// </summary>
        public List<String> DatasetNames
        {
            get 
            {
                return _datasetNames;
            }
        }

        public HDF5Helper(String path, bool createFlag)
        {
            if (!createFlag)
            {
                _fileId = H5F.open(path, H5F.OpenMode.ACC_RDONLY);
            }
            else
            {
                _fileId = H5F.create(path, H5F.CreateMode.ACC_TRUNC);
            }
            getFileAttributeNames();
            getDatasetNames();
        }

        /// <summary>
        /// 得到指定文件属性值
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public String GetFileAttribute(String attributeName)
        {
            if (String.IsNullOrEmpty(attributeName) || !_fileAttributeNames.Contains(attributeName))
            {
                return null;
            }

            return getAttributeValue(_fileId, attributeName);
        }

        /// <summary>
        /// 得到所有文件属性值
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, String> GetFileAttributes()
        {
            Dictionary<String, String> attributeValues = new Dictionary<String, String>();
            foreach (String attributeName in _fileAttributeNames)
            {
                attributeValues.Add(attributeName, GetFileAttribute(attributeName));
            }
            return attributeValues;
        }

        /// <summary>
        /// 得到个顶数据集的所有属性值，未对异常进行处理
        /// </summary>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetDatasetAttributes(String datasetName)
        {
            Dictionary<String, String> attributeValues = new Dictionary<String, String>();
            if (!String.IsNullOrEmpty(datasetName) && _datasetNames.Contains(datasetName))
            {
                H5DataSetId datasetId = H5D.open(_fileId, datasetName);
                if (datasetId != null)
                {
                    int attributeCount = H5A.getNumberOfAttributes(datasetId);
                    for (int ii = 0; ii < attributeCount; ++ii)
                    {
                        String attributeName = H5A.getNameByIndex(datasetId, "/" + datasetName, H5IndexType.NAME,
                            H5IterationOrder.NATIVE, (ulong)ii);
                        attributeValues.Add(attributeName, getAttributeValue(datasetId, attributeName));
                    }

                    H5D.close(datasetId);
                }            
            }

            return attributeValues;           
        }

        /// <summary>
        /// 写文件属性
        /// </summary>
        public void WriteFileAttribute(string attrName, string value)
        {
            H5DataTypeId typeId = H5T.copy(H5T.H5Type.C_S1);
            H5DataSpaceId spaceId = H5S.create(H5S.H5SClass.SCALAR);
            H5T.setSize(typeId, value.Length);
            H5AttributeId attrId = H5A.create(_fileId, attrName, typeId, spaceId);
            
            if (value != "")
            {
                H5Array<byte> buffer = new H5Array<byte>(Encoding.Default.GetBytes(value));
                H5A.write(attrId, typeId, buffer);
            }

            if (typeId != null)
                H5T.close(typeId);
            if (spaceId != null)
                H5S.close(spaceId);
            if (attrId != null)
                H5A.close(attrId);
        }

        /// <summary>
        /// 写数据集属性
        /// </summary>
        public void WriteDatasetAttribute(string datasetName, string attrName, string value)
        {
            H5DataSetId datasetId = H5D.open(_fileId, datasetName);
            H5DataTypeId typeId = H5T.copy(H5T.H5Type.C_S1);
            H5DataSpaceId spaceId = H5S.create(H5S.H5SClass.SCALAR);
            H5T.setSize(typeId, value.Length);
            H5AttributeId attrId = H5A.create(datasetId, attrName, typeId, spaceId);

            if (value != "")
            {
                H5Array<byte> buffer = new H5Array<byte>(Encoding.Default.GetBytes(value));
                H5A.write(attrId, typeId, buffer);
            }

            if (typeId != null)
                H5T.close(typeId);
            if (spaceId != null)
                H5S.close(spaceId);
            if (attrId != null)
                H5A.close(attrId);
            if (datasetId != null)
                H5D.close(datasetId);
        }

        /// <summary>
        /// 获得数据集的类型
        /// </summary>
        public string GetDatasetType(string datasetName)
        {
            H5DataSetId datasetId = H5D.open(_fileId, datasetName);
            H5DataTypeId typeId = H5D.getType(datasetId);
            H5T.H5TClass typeClass = H5T.getClass(typeId);
            return typeClass.ToString();
        }

        /// <summary>
        /// 读取指定数据集，未对异常进行处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datasetName"></param>
        /// <param name="bandN"></param>
        /// <param name="bandH"></param>
        /// <param name="bandW"></param>
        /// <returns></returns>
        public T[] ReadDataArray<T>(String datasetName, ref int bandN, ref int bandH, ref int bandW)
        {
            H5DataSetId datasetId = null;
            H5DataSpaceId spaceId = null;
            H5DataTypeId typeId = null;
            long[] dims = null;

            if (!String.IsNullOrEmpty(datasetName) && _datasetNames.Contains(datasetName))
            {
                datasetId = H5D.open(_fileId, datasetName);
                spaceId = H5D.getSpace(datasetId);
                dims = H5S.getSimpleExtentDims(spaceId);
                if (dims.Length == 2)
                {
                    bandN = 1;
                    bandH = (int)dims[0];
                    bandW = (int)dims[1];
                }
                else if (dims.Length == 3)
                {
                    bandN = (int)dims[0];
                    bandH = (int)dims[1];
                    bandW = (int)dims[2];
                }
                typeId = H5D.getType(datasetId);
                typeId = H5T.getNativeType(typeId, H5T.Direction.DEFAULT);
                T[] dv = new T[bandN * bandH * bandW];
                H5D.read<T>(datasetId, typeId, new H5Array<T>(dv));

                if (typeId != null)
                {
                    H5T.close(typeId);
                }
                if (spaceId != null)
                {
                    H5S.close(spaceId);
                }
                if (datasetId != null)
                {
                    H5D.close(datasetId);
                }
                return dv;
            }
            else
            {
                throw new Exception("未查到指定数据集！");
            }
        }

        /// <summary>
        /// 写入数据集，未对异常进行处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datasetName"></param>
        /// <param name="bandN"></param>
        /// <param name="bandH"></param>
        /// <param name="bandW"></param>
        public void WriteDataArray<T>(String datasetName, T[] data, int bandN, int bandH, int bandW)
        {
            long[] dims = null;
            H5DataSpaceId spaceId = null;
            H5DataTypeId typeId = null;
            H5DataSetId datasetId = null;

            if (data == null || data.Length == 0)
            {
                throw new System.IO.IOException("无效数据块！");
            }

            if (bandN == 1)
            {
                dims = new long[2];
                dims[0] = bandH;
                dims[1] = bandW;
                spaceId = H5S.create_simple(2, dims);
            }
            else
            {
                dims = new long[3];
                dims[0] = bandN;
                dims[1] = bandH;
                dims[2] = bandW;
                spaceId = H5S.create_simple(3, dims);
            }

            /* 设置数据类型，目前只设置了byte short、int、float相关的类型 */
            if (data[0] is Byte)
            {
                typeId = H5T.copy(H5T.H5Type.NATIVE_UCHAR);
            }
            else if (data[0] is Int16)
            {
                typeId = H5T.copy(H5T.H5Type.NATIVE_SHORT);
            }
            else if (data[0] is UInt16)
            {
                typeId = H5T.copy(H5T.H5Type.NATIVE_USHORT);
            }
            else if (data[0] is Int32)
            {
                typeId = H5T.copy(H5T.H5Type.NATIVE_INT);
            }
            else if (data[0] is UInt32)
            {
                typeId = H5T.copy(H5T.H5Type.NATIVE_UINT);
            }
            else if (data[0] is Single)
            {
                typeId = H5T.copy(H5T.H5Type.NATIVE_FLOAT);
            }
            //H5T.setOrder(typeId, H5T.Order.LE);
            typeId = H5T.getNativeType(typeId, H5T.Direction.DEFAULT);
            datasetId = H5D.create(_fileId, datasetName, typeId, spaceId);
            H5D.write<T>(datasetId, typeId, new H5Array<T>(data));

            if (typeId != null)
            {
                H5T.close(typeId);
            }
            if (spaceId != null)
            {
                H5S.close(spaceId);
            }
            if (datasetId != null)
            {
                H5D.close(datasetId);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_fileId != null)
            {
                H5F.close(_fileId);
                _fileId = null;
            }
        }

        /// <summary>
        /// 判断文件是否为HDF5格式，HDF5的文件头共8个字节
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsHDF5(string path)
        {
            byte[] buffer = null, hdf5 = new byte[8] { 137, 72, 68, 70, 13, 10, 26, 10 };
            int ii = 0;

            using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                buffer = new byte[8];
                try
                {
                    fs.Read(buffer, 0, 8);
                    for (ii = 0; ii < hdf5.Length; ++ii)
                    {
                        if (hdf5[ii] != buffer[ii])
                        {
                            return false;
                        }
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 得到所有文件属性名，未对异常进行处理
        /// </summary>
        private void getFileAttributeNames()
        {
            H5GroupId groupId = H5G.open(_fileId, "/");
            int attCount = H5A.getNumberOfAttributes(groupId);
            for (int ii = 0; ii < attCount; ++ii)
            {
                String attName = H5A.getNameByIndex(groupId, "/", H5IndexType.NAME, H5IterationOrder.NATIVE, (ulong)ii);
                if (attName != null)
                {
                    _fileAttributeNames.Add(attName);
                }
            }

            if (groupId != null)
            {
                H5G.close(groupId);
            }
        }

        /// <summary>
        /// 得到所有数据集合名，未对异常进行处理
        /// </summary>
        private void getDatasetNames()
        {
            H5GroupId groupId = H5G.open(_fileId, "/");
            long dscount = H5G.getNumObjects(groupId);
            for (int ii = 0; ii < dscount; ++ii)
            {
                String v = H5G.getObjectNameByIndex(groupId, (ulong)ii);
                _datasetNames.Add(v);
            }

            if (groupId != null)
            {
                H5G.close(groupId);
            }
        }

        /// <summary>
        /// 读取指定属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="size"></param>
        /// <param name="attId"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        private T[] readAttribute<T>(long size, H5AttributeId attId, H5DataTypeId typeId)
        {
            T[] v = new T[size];
            H5A.read<T>(attId, typeId, new H5Array<T>(v));
            return v;
        }

        /// <summary>
        /// 将array变成字符串
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private String arrayToString(object v)
        {
            if (v == null)
            {
                return string.Empty;
            }
            if (v is double[])
            {
                return arrayToString<double>(v as double[]);
            }
            else if (v is float[])
            {
                return arrayToString<float>(v as float[]);
            }
            else if (v is byte[])
            {
                return arrayToString<byte>(v as byte[]);
            }
            else if (v is UInt16[])
            {
                return arrayToString<UInt16>(v as UInt16[]);
            }
            else if (v is Int16[])
            {
                return arrayToString<Int16>(v as Int16[]);
            }
            else if (v is Int32[])
            {
                return arrayToString<Int32>(v as Int32[]);
            }
            else if (v is UInt32[])
            {
                return arrayToString<UInt32>(v as UInt32[]);
            }
            else if (v is Int64[])
            {
                return arrayToString<Int64>(v as Int64[]);
            }
            else if (v is UInt64[])
            {
                return arrayToString<UInt64>(v as UInt64[]);
            }
            return v.ToString().Replace('\0', ' ').TrimEnd();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <returns></returns>
        private String arrayToString<T>(T[] v)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T t in v)
            {
                sb.Append(t.ToString());
                sb.Append(",");
            }
            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }        

        /// <summary>
        /// 得到指定属性集合中指定属性名的属性值，未对异常进行处理
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private String getAttributeValue(H5ObjectWithAttributes obj, String attributeName)
        {
            H5AttributeId attId = null;
            attId = H5A.open(obj, attributeName);
            if (attId == null)
            {
                return null;
            }

            H5DataTypeId typeId = null;
            H5DataTypeId dtId = null;
            H5AttributeInfo attInfo = null;
            H5DataSpaceId spaceId = null;
            object attributeVal = null;

            typeId = H5A.getType(attId);
            attInfo = H5A.getInfo(attId);
            dtId = H5A.getType(attId);
            spaceId = H5A.getSpace(attId);
            int dataSize = H5T.getSize(dtId);
            typeId = H5T.getNativeType(typeId, H5T.Direction.DEFAULT);
            H5T.H5TClass typeClass = H5T.getClass(typeId);
            long[] dims = H5S.getSimpleExtentDims(spaceId);
            if (dims.Length == 0)
            {
                dims = new long[1];
                dims[0] = 1;
            }
            switch (typeClass)
            {
                case H5T.H5TClass.STRING:
                    long size = attInfo.dataSize;
                    byte[] chars = readAttribute<byte>(size, attId, typeId);
                    attributeVal = Encoding.ASCII.GetString(chars);
                    break;
                case H5T.H5TClass.INTEGER:
                    H5T.Sign sign = H5T.Sign.TWOS_COMPLEMENT;
                    sign = H5T.getSign(typeId);
                    switch (dataSize)
                    {
                        case 1:
                            attributeVal = readAttribute<byte>(dims[0], attId, typeId);
                            break;
                        case 2:
                            switch (sign)
                            {
                                case H5T.Sign.TWOS_COMPLEMENT:
                                    attributeVal = readAttribute<Int16>(dims[0], attId, typeId);
                                    break;
                                case H5T.Sign.UNSIGNED:
                                    attributeVal = readAttribute<UInt16>(dims[0], attId, typeId);
                                    break;
                            }
                            break;
                        case 4:
                            switch (sign)
                            {
                                case H5T.Sign.TWOS_COMPLEMENT:
                                    attributeVal = readAttribute<Int32>(dims[0], attId, typeId);
                                    break;
                                case H5T.Sign.UNSIGNED:
                                    attributeVal = readAttribute<UInt32>(dims[0], attId, typeId);
                                    break;
                            }
                            break;
                        case 8:
                            switch (sign)
                            {
                                case H5T.Sign.TWOS_COMPLEMENT:
                                    attributeVal = readAttribute<Int64>(dims[0], attId, typeId);
                                    break;
                                case H5T.Sign.UNSIGNED:
                                    attributeVal = readAttribute<UInt64>(dims[0], attId, typeId);
                                    break;
                            }
                            break;
                    }
                    break;
                case H5T.H5TClass.FLOAT:
                    switch (dataSize)
                    {
                        case 4:
                            attributeVal = readAttribute<float>(dims[0], attId, typeId);
                            break;
                        case 8:
                            attributeVal = readAttribute<double>(dims[0], attId, typeId);
                            break;
                    }
                    break;
            }

            if (spaceId != null)
            {
                H5S.close(spaceId);
            }
            if (attId != null)
            {
                H5A.close(attId);
            }
            if (typeId != null)
            {
                H5T.close(typeId);
            }
            if (dtId != null)
            {
                H5T.close(dtId);
            }

            return arrayToString(attributeVal);
        }
    }
}
