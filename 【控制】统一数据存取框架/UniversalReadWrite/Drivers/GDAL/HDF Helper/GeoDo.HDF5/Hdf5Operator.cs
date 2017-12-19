using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HDF5DotNet;
using GeoDo.HDF;

namespace GeoDo.HDF5
{
    public class Hdf5Operator : IHdfOperator
    {
        protected H5FileId _h5FileId = null;
        protected List<string> _allFileAttributes = new List<string>();
        protected List<string> _datasetNames = new List<string>();
        protected Dictionary<string, string> _fileAttrs = new Dictionary<string, string>();
        private bool _readFileAttrs = false;
        private string _fname = null;

        public Hdf5Operator(string filename)
        {
            _fname = filename;
            _h5FileId = H5F.open(filename, HDF5DotNet.H5F.OpenMode.ACC_RDONLY);
            GetAllFileAttributes();
            GetAllDatasetNames();
        }

        public string[] GetDatasetNames
        {
            get { return _datasetNames.Count > 0 ? _datasetNames.ToArray() : null; }
        }

        public Dictionary<string, string> GetAttributes()
        {
            if (!_readFileAttrs)
            {
                Dictionary<string, string> cacheAttrs = GlobalFileAttributeCache.GetFileAttributes(_fname);
                if (cacheAttrs != null)
                {
                    _fileAttrs = cacheAttrs;
                }
                else
                {
                    Dictionary<string, string> attValues = new Dictionary<string, string>();
                    foreach (string attributeName in _allFileAttributes)
                    {
                        attValues.Add(attributeName, ReadAttributeValue(_h5FileId, attributeName));
                    }
                    _fileAttrs = attValues;
                    GlobalFileAttributeCache.SetFileAttributes(_fname, attValues);
                }
            }
            return _fileAttrs;
        }

        public Dictionary<string, string> GetAttributes(string datasetName)
        {
            if (string.IsNullOrEmpty(datasetName) || !_datasetNames.Contains(datasetName))
                return null;
            H5DataSetId datasetId = null;
            H5GroupId groupId = null;
            H5DataTypeId typeId = null;
            H5DataSpaceId spaceId = null;
            //H5PropertyListId psId = null;
            try
            {
                int groupIndex = datasetName.LastIndexOf('/');
                if (groupIndex == -1)
                    datasetId = H5D.open(_h5FileId, datasetName);
                else
                {
                    string groupName = datasetName.Substring(0, groupIndex + 1);
                    string dsName = datasetName.Substring(groupIndex + 1);
                    groupId = H5G.open(_h5FileId, groupName);
                    datasetId = H5D.open(groupId, dsName);
                }
                if (datasetId == null)
                    return null;
                Dictionary<string, string> attValues = new Dictionary<string, string>();

                typeId = H5D.getType(datasetId);
                H5T.H5TClass type = H5T.getClass(typeId);
                int tSize = H5T.getSize(typeId);

                spaceId = H5D.getSpace(datasetId);
                long[] dims = H5S.getSimpleExtentDims(spaceId);
                long storageSize = H5D.getStorageSize(datasetId);

                attValues.Add("DataSetName", datasetName);
                attValues.Add("DataType", type.ToString());
                attValues.Add("DataTypeSize", tSize.ToString() + "Byte");
                attValues.Add("Dims", String.Join("*", dims));
                attValues.Add("StorageSize", storageSize.ToString() + "Byte");

                int attrCount = H5A.getNumberOfAttributes(datasetId);
                for (int i = 0; i < attrCount; i++)
                {
                    string attName = H5A.getNameByIndex(datasetId, "/" + datasetName, H5IndexType.NAME, H5IterationOrder.NATIVE, (ulong)i);
                    attValues.Add(attName, ReadAttributeValue(datasetId, attName));
                }
                return attValues;
            }
            finally
            {
                if (spaceId != null)
                    H5S.close(spaceId);
                if (typeId != null)
                    H5T.close(typeId);
                if (datasetId != null)
                    H5D.close(datasetId);
                if (groupId != null)
                    H5G.close(groupId);
            }
        }

        /// <summary>
        /// 修改时间：20130906
        /// 修改人：罗战克
        /// 修改内容：获取数据集名字时候，加入类型判断，确定是H5GType.DATASET时候才加入。
        /// </summary>
        private void GetAllDatasetNames()
        {
            GetGroupDatasetNames("/");
        }

        private void GetGroupDatasetNames(string groupName)
        {
            H5GroupId h5GroupId = H5G.open(_h5FileId, groupName);
            try
            {
                long dscount = H5G.getNumObjects(h5GroupId);
                for (int i = 0; i < dscount; i++)
                {
                    string objname = H5G.getObjectNameByIndex(h5GroupId, (ulong)i);
                    ObjectInfo objInfo = H5G.getObjectInfo(h5GroupId, objname, false);
                    switch (objInfo.objectType)
                    {
                        case H5GType.DATASET:
                            if (objInfo.objectType == H5GType.DATASET)
                            {
                                if (groupName == "/")
                                    _datasetNames.Add(objname);
                                else
                                    _datasetNames.Add(groupName + objname);
                            }
                            break;
                        case H5GType.GROUP:
                            if (groupName == "/")
                                GetGroupDatasetNames(objname + "/");
                            else
                                GetGroupDatasetNames(groupName + objname + "/");
                            break;
                        case H5GType.LINK:
                            break;
                        case H5GType.TYPE:
                            break;
                        default:
                            break;
                    }
                }
            }
            finally
            {
                if (h5GroupId != null)
                    H5G.close(h5GroupId);
            }
        }

        private void GetAllFileAttributes()
        {
            H5GroupId h5GroupId = H5G.open(_h5FileId, "/");
            try
            {
                int attCount = H5A.getNumberOfAttributes(h5GroupId);
                for (int i = 0; i < attCount; i++)
                {
                    string attName = H5A.getNameByIndex(h5GroupId, "/", H5IndexType.NAME, H5IterationOrder.NATIVE, (ulong)i);
                    if (attName != null)
                        _allFileAttributes.Add(attName);
                }
            }
            finally
            {
                if (h5GroupId != null)
                    H5G.close(h5GroupId);
            }
        }

        public string GetAttributeValue(string attributeName)
        {
            GetAttributes();
            if (_fileAttrs == null || _fileAttrs.Count == 0)
                return null;
            else if (_fileAttrs.ContainsKey(attributeName))
                return _fileAttrs[attributeName];
            else
                return null;
        }

        public string GetAttributeValue(string datasetName, string attributeName)
        {
            if (string.IsNullOrEmpty(datasetName) || !_datasetNames.Contains(datasetName))
                return null;
            H5DataSetId datasetId = H5D.open(_h5FileId, datasetName);
            if (datasetId == null)
                return null;
            try
            {
                return ReadAttributeValue(datasetId, attributeName);
            }
            finally
            {
                H5D.close(datasetId);
            }
        }

        private string ReadAttributeValue(H5ObjectWithAttributes obj, string attributeName)
        {
            object v = GetAttributeValue(obj, attributeName);
            return TryArrayToString(v);
        }

        //private string TryArrayToString(object v)
        //{
        //    if (v == null)
        //        return string.Empty;
        //    if (v is double[])
        //        return ArrayToString<double>(v as double[]);
        //    else if (v is float[])
        //        return ArrayToString<float>(v as float[]);
        //    else if (v is byte[])
        //        return ArrayToString<byte>(v as byte[]);
        //    else if (v is UInt16[])
        //        return ArrayToString<UInt16>(v as UInt16[]);
        //    else if (v is Int16[])
        //        return ArrayToString<Int16>(v as Int16[]);
        //    else if (v is Int32[])
        //        return ArrayToString<Int32>(v as Int32[]);
        //    else if (v is UInt32[])
        //        return ArrayToString<UInt32>(v as UInt32[]);
        //    else if (v is Int64[])
        //        return ArrayToString<Int64>(v as Int64[]);
        //    else if (v is UInt64[])
        //        return ArrayToString<UInt64>(v as UInt64[]);
        //    return v.ToString().Replace('\0', ' ').TrimEnd();
        //}

        private string TryArrayToString(object v)
        {
            if (v == null)
                return string.Empty;
            if (v is double[])
                return string.Join<double>(",", v as double[]);
            else if (v is float[])
                return string.Join<float>(",", v as float[]);
            else if (v is byte[])
                return string.Join<byte>(",", v as byte[]);
            else if (v is UInt16[])
                return string.Join<UInt16>(",", v as UInt16[]);
            else if (v is Int16[])
                return string.Join<Int16>(",", v as Int16[]);
            else if (v is Int32[])
                return string.Join<Int32>(",", v as Int32[]);
            else if (v is UInt32[])
                return string.Join<UInt32>(",", v as UInt32[]);
            else if (v is Int64[])
                return string.Join<Int64>(",", v as Int64[]);
            else if (v is UInt64[])
                return string.Join<UInt64>(",", v as UInt64[]);
            return v.ToString().Replace('\0', ' ').TrimEnd();
        }

        //private string ArrayToString<T>(T[] v)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    //sb.Append("[");
        //    foreach (T t in v)
        //    {
        //        sb.Append(t.ToString());
        //        sb.Append(",");
        //    }
        //    if (sb.Length > 1)
        //        sb.Remove(sb.Length - 1, 1);
        //    //sb.Append("]");
        //    return sb.ToString();
        //}

        private object GetAttributeValue(H5ObjectWithAttributes obj, string attributeName)
        {
            H5AttributeId attId = null;
            attId = H5A.open(obj, attributeName);
            if (attId == null)
                return null;
            H5DataTypeId typeId = null;
            H5DataTypeId dtId = null;
            H5AttributeInfo attInfo = null;
            H5DataSpaceId spaceId = null;
            H5DataTypeId oldTypeId = null;
            object retObject = null;
            try
            {
                typeId = H5A.getType(attId);
                attInfo = H5A.getInfo(attId);
                dtId = H5A.getType(attId);
                spaceId = H5A.getSpace(attId);
                int dataSize = H5T.getSize(dtId);
                //
                oldTypeId = typeId;
                typeId = H5T.getNativeType(typeId, H5T.Direction.DEFAULT);
                H5T.H5TClass typeClass = H5T.getClass(typeId);
                long[] dims = H5S.getSimpleExtentDims(spaceId);
                long dimSize = 1;
                if (dims.Length == 0)
                {
                    dimSize = 1;
                }
                else
                {
                    foreach (long dim in dims)
                    {
                        dimSize *= dim;
                    }
                }
                switch (typeClass)
                {
                    case H5T.H5TClass.STRING:
                        long size = attInfo.dataSize;
                        byte[] chars = ReadArray<byte>(size, attId, typeId);
                        retObject = Encoding.ASCII.GetString(chars);
                        break;
                    case H5T.H5TClass.INTEGER:
                        H5T.Sign sign = H5T.Sign.TWOS_COMPLEMENT;
                        sign = H5T.getSign(oldTypeId);
                        switch (dataSize)
                        {
                            case 1:
                                retObject = ReadArray<byte>(dimSize, attId, typeId);
                                break;
                            case 2:
                                switch (sign)
                                {
                                    case H5T.Sign.TWOS_COMPLEMENT:
                                        retObject = ReadArray<Int16>(dimSize, attId, typeId);
                                        break;
                                    case H5T.Sign.UNSIGNED:
                                        retObject = ReadArray<UInt16>(dimSize, attId, typeId);
                                        break;
                                }
                                break;
                            case 4:
                                switch (sign)
                                {
                                    case H5T.Sign.TWOS_COMPLEMENT:
                                        retObject = ReadArray<Int32>(dimSize, attId, typeId);
                                        break;
                                    case H5T.Sign.UNSIGNED:
                                        retObject = ReadArray<UInt32>(dimSize, attId, typeId);
                                        break;
                                }
                                break;
                            case 8:
                                switch (sign)
                                {
                                    case H5T.Sign.TWOS_COMPLEMENT:
                                        retObject = ReadArray<Int64>(dimSize, attId, typeId);
                                        break;
                                    case H5T.Sign.UNSIGNED:
                                        retObject = ReadArray<UInt64>(dimSize, attId, typeId);
                                        break;
                                }
                                break;
                        }
                        break;
                    case H5T.H5TClass.FLOAT:
                        switch (dataSize)
                        {
                            case 4:
                                retObject = ReadArray<float>(dimSize, attId, typeId);
                                break;
                            case 8:
                                retObject = ReadArray<double>(dimSize, attId, typeId);
                                break;
                        }
                        break;
                }
                return retObject;
            }
            finally
            {
                if (spaceId != null)
                    H5S.close(spaceId);
                if (attId != null)
                    H5A.close(attId);
                if (oldTypeId != null)
                    H5T.close(oldTypeId);
                if (typeId != null)
                    H5T.close(typeId);
                if (dtId != null)
                    H5T.close(dtId);
            }
        }

        private T[] ReadArray<T>(long size, H5AttributeId attId, H5DataTypeId typeId)
        {
            T[] v = new T[size];
            if (size == 0)
                return v;
            H5A.read<T>(attId, typeId, new H5Array<T>(v));
            return v;
        }

        public void Dispose()
        {
            if (_h5FileId != null)
            {
                H5F.close(_h5FileId);
                _h5FileId = null;
            }
        }
    }
}
