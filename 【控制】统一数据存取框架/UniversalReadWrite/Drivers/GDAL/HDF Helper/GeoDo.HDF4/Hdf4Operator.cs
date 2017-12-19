using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.HDF;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;

namespace GeoDo.HDF4
{
    public class Hdf4Operator : IHdfOperator
    {
        private const int MAX_DIMSIZES = 10;
        private string _fname = null;
        /// <summary>
        /// 所有数据集
        /// </summary>
        protected string[] _datasetNames = null;
        /// <summary>
        /// 文件属性
        /// </summary>
        protected Dictionary<string, string> _fileAttrs = new Dictionary<string, string>();
        private bool _readFileAttrs = false;

        public Hdf4Operator(string fname)
        {
            _fname = fname;
            _datasetNames = GetALLDataset();
        }

        public string[] GetDatasetNames
        {
            get { return _datasetNames; }
        }

        private string[] GetALLDataset()
        {
            int sd_id = HDF4Helper.SDstart(_fname, HDF4Helper.AccessCodes.DFACC_READ);
            try
            {
                int num_datasets = 0;
                int num_global_attrs = 0;
                HDF4Helper.SDfileinfo(sd_id, out num_datasets, out num_global_attrs);
                string[] dsNames = ReadSDinfo(sd_id, num_datasets);
                return dsNames;
            }
            finally
            {
                HDF4Helper.SDend(sd_id);
            }
        }

        private static string[] ReadSDinfo(int sd_id, int num_datasets)
        {
            string[] dsNames = new string[num_datasets];
            for (int dsIndex = 0; dsIndex < num_datasets; dsIndex++)
            {
                int sds_id = HDF4Helper.SDselect(sd_id, dsIndex);
                try
                {
                    StringBuilder sds_name = new StringBuilder(256);
                    int rank;
                    int[] dimsizes = new int[256];
                    GeoDo.HDF4.HDF4Helper.DataTypeDefinitions data_type;
                    int num_attrs;
                    HDF4Helper.SDgetinfo(sds_id, sds_name, out rank, dimsizes, out data_type, out num_attrs);
                    dsNames[dsIndex] = sds_name.ToString();
                }
                finally
                {
                    HDF4Helper.SDend(sds_id);
                }

            }
            return dsNames;
        }

        private string ReadAttribute(int objId, int attIndex)
        {
            HDF4Helper.DataTypeDefinitions dType = HDF4Helper.DataTypeDefinitions.DFNT_CHAR;
            int count = 0;
            StringBuilder attrName = new StringBuilder(256);
            HDF4Helper.SDattrinfo(objId, attIndex, attrName, out dType, out count);
            string attValue = GetAttributevalue(dType, objId, attIndex, count);
            return attValue;
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
            int id = HDF4Helper.SDstart(_fname, HDF4Helper.AccessCodes.DFACC_READ);
            try
            {
                int dsIndex = HDF4Helper.SDnametoindex(id, datasetName);
                int dsObjId = HDF4Helper.SDselect(id, dsIndex);
                int attIndex = HDF4Helper.SDfindattr(dsObjId, attributeName);
                return ReadAttribute(dsObjId, attIndex);
            }
            finally
            {
                HDF4Helper.SDend(id);
            }
        }

        public Dictionary<string, string> GetAttributes()
        {
            if (!_readFileAttrs)
            {
                Dictionary<string, string> cacheAttrs = GlobalFileAttributeCache.GetFileAttributes(_fname);
                if (cacheAttrs == null)
                {
                    _readFileAttrs = true;
                    int id = HDF4Helper.SDstart(_fname, HDF4Helper.AccessCodes.DFACC_READ);
                    try
                    {
                        int dsCount = 0;
                        int attCount = 0;
                        HDF4Helper.SDfileinfo(id, out dsCount, out attCount);
                        if (attCount == 0)
                            return null;
                        _fileAttrs = GetAttributes(id, attCount);
                        GlobalFileAttributeCache.SetFileAttributes(_fname, _fileAttrs);
                    }
                    finally
                    {
                        HDF4Helper.SDend(id);
                    }
                }
                else
                {
                    _fileAttrs = cacheAttrs;
                }
            }
            return _fileAttrs;
        }

        public Dictionary<string, string> GetAttributes(string datasetName)
        {
            StringBuilder sds_name = new StringBuilder();
            int rank = 0;
            int[] dimsizes = new int[MAX_DIMSIZES];
            HDF4Helper.DataTypeDefinitions datatype;
            int id = HDF4Helper.SDstart(_fname, HDF4Helper.AccessCodes.DFACC_READ);
            try
            {
                int dsIndex = HDF4Helper.SDnametoindex(id, datasetName);
                int dsObjId = HDF4Helper.SDselect(id, dsIndex);
                int attCount = 0;
                HDF4Helper.SDgetinfo(dsObjId, sds_name, out rank, dimsizes, out datatype, out attCount);
                //int dsCount = 0;
                //HDF4Helper.SDfileinfo(id, out dsCount, out attCount);
                if (attCount == 0)
                    return null;
                return GetAttributes(dsObjId, attCount);
            }
            finally
            {
                HDF4Helper.SDend(id);
            }
        }

        public bool GetDataSizeInfos(string datasetName, out int rank, out int[] dimsizes, out  Type dataType, out int dataTypeSize)
        {
            HDF4Helper.DataTypeDefinitions hdf4Type = HDF4Helper.DataTypeDefinitions.DFNT_NUINT16;
            if (!GetDataSizeInfos(datasetName, out rank, out  dimsizes, out hdf4Type, out  dataType, out dataTypeSize))
                return false;
            return true;
        }

        public bool GetDataSizeInfos(string datasetName, out int rank, out int[] dimsizes, out HDF4Helper.DataTypeDefinitions hdf4Type, out  Type dataType, out int dataTypeSize)
        {
            StringBuilder sds_name = new StringBuilder();
            rank = 0;
            dimsizes = new int[MAX_DIMSIZES];
            dataType = typeof(UInt16);
            hdf4Type = HDF4Helper.DataTypeDefinitions.DFNT_NUINT16;
            dataTypeSize = 0;
            int id = HDF4Helper.SDstart(_fname, HDF4Helper.AccessCodes.DFACC_READ);
            try
            {
                int dsIndex = HDF4Helper.SDnametoindex(id, datasetName);
                int dsObjId = HDF4Helper.SDselect(id, dsIndex);
                int attCount = 0;
                HDF4Helper.SDgetinfo(dsObjId, sds_name, out rank, dimsizes, out hdf4Type, out attCount);
                if (rank != 0)
                {
                    dataType = GetTypeFromHDF4DataType(hdf4Type, out dataTypeSize);
                    return true;
                }
            }
            finally
            {
                HDF4Helper.SDend(id);
            }
            return false;

        }

        public static Type GetTypeFromHDF4DataType(HDF4Helper.DataTypeDefinitions data_type, out int dataTypeSize)
        {
            dataTypeSize = 0;
            switch (data_type)
            {
                case HDF4Helper.DataTypeDefinitions.DFNT_CHAR:
                case HDF4Helper.DataTypeDefinitions.DFNT_UCHAR:
                    dataTypeSize = Marshal.SizeOf(typeof(byte));
                    return typeof(byte);
                case HDF4Helper.DataTypeDefinitions.DFNT_INT8:
                    dataTypeSize = Marshal.SizeOf(typeof(byte));
                    return typeof(byte);
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT8:
                    dataTypeSize = Marshal.SizeOf(typeof(byte));
                    return typeof(byte);
                case HDF4Helper.DataTypeDefinitions.DFNT_INT16:
                    dataTypeSize = Marshal.SizeOf(typeof(Int16));
                    return typeof(Int16);
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT16:
                    dataTypeSize = Marshal.SizeOf(typeof(UInt16));
                    return typeof(UInt16);
                case HDF4Helper.DataTypeDefinitions.DFNT_INT32:
                    dataTypeSize = Marshal.SizeOf(typeof(int));
                    return typeof(int);
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT32:
                    dataTypeSize = Marshal.SizeOf(typeof(uint));
                    return typeof(uint);
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT32:
                    dataTypeSize = Marshal.SizeOf(typeof(float));
                    return typeof(float);
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT64:
                    dataTypeSize = Marshal.SizeOf(typeof(double));
                    return typeof(double);
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT8:
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT8:
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT16:
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT16:
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT32:
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT32:
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_NFLOAT32:
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_NFLOAT64:
                    break;
                default:
                    break;
            }
            return null;
        }

        private Dictionary<string, string> GetAttributes(int objId, int attCount)
        {
            Dictionary<string, string> atts = new Dictionary<string, string>();
            for (int i = 0; i < attCount; i++)
            {
                string attName, attValue;
                GetAttribute(objId, i, out attName, out attValue);
                if (!string.IsNullOrWhiteSpace(attValue))
                    atts.Add(attName, attValue);
            }
            return atts;
        }

        private void GetAttribute(int objId, int attIndex, out string attName, out string attValue)
        {
            attName = "";
            attValue = "";
            StringBuilder attr_name = new StringBuilder(256);
            HDF4Helper.DataTypeDefinitions data_type;
            int data_count = 0;
            HDF4Helper.SDattrinfo(objId, attIndex, attr_name, out data_type, out data_count);
            attName = attr_name.ToString();
            attValue = GetAttributevalue(data_type, objId, attIndex, data_count);
        }

        private string GetAttributevalue(HDF4Helper.DataTypeDefinitions data_type, int objId, int attIndex, int data_count)
        {
            StringBuilder attr_Data = new StringBuilder(256);
            switch (data_type)
            {
                case HDF4Helper.DataTypeDefinitions.DFNT_CHAR:
                case HDF4Helper.DataTypeDefinitions.DFNT_UCHAR:
                    {
                        IntPtr attr_buff = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * data_count);
                        HDF4Helper.SDreadattr(objId, attIndex, attr_buff);
                        byte[] dest = new byte[data_count];
                        Marshal.Copy(attr_buff, dest, 0, data_count);
                        Marshal.FreeHGlobal(attr_buff);
                        foreach (char value in dest)
                        {
                            attr_Data.Append(value);
                        }
                        break;
                    }
                case HDF4Helper.DataTypeDefinitions.DFNT_INT8:
                    {
                        IntPtr attr_buff = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * data_count);
                        HDF4Helper.SDreadattr(objId, attIndex, attr_buff);
                        byte[] dest = new byte[data_count];
                        Marshal.Copy(attr_buff, dest, 0, data_count);
                        Marshal.FreeHGlobal(attr_buff);
                        foreach (sbyte value in dest)
                        {
                            attr_Data.Append(value);
                            attr_Data.Append(",");
                        }
                        break;
                    }
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT8:
                    {
                        IntPtr attr_buff = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * data_count);
                        HDF4Helper.SDreadattr(objId, attIndex, attr_buff);
                        byte[] dest = new byte[data_count];
                        Marshal.Copy(attr_buff, dest, 0, data_count);
                        Marshal.FreeHGlobal(attr_buff);
                        foreach (byte value in dest)
                        {
                            attr_Data.Append(value);
                            attr_Data.Append(",");
                        }
                        break;
                    }
                case HDF4Helper.DataTypeDefinitions.DFNT_INT16:
                    {
                        IntPtr attr_buff = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(short)) * data_count);
                        HDF4Helper.SDreadattr(objId, attIndex, attr_buff);
                        short[] dest = new short[data_count];
                        Marshal.Copy(attr_buff, dest, 0, data_count);
                        Marshal.FreeHGlobal(attr_buff);
                        foreach (short value in dest)
                        {
                            attr_Data.Append(value);
                            attr_Data.Append(",");
                        }
                    }
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT16:
                    {
                        IntPtr attr_buff = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(short)) * data_count);
                        HDF4Helper.SDreadattr(objId, attIndex, attr_buff);
                        short[] dest = new short[data_count];
                        Marshal.Copy(attr_buff, dest, 0, data_count);
                        Marshal.FreeHGlobal(attr_buff);
                        foreach (ushort value in dest)
                        {
                            attr_Data.Append(value);
                            attr_Data.Append(",");
                        }
                    }
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_INT32:
                    {
                        IntPtr attr_buff = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * data_count);
                        HDF4Helper.SDreadattr(objId, attIndex, attr_buff);
                        int[] dest = new int[data_count];
                        Marshal.Copy(attr_buff, dest, 0, data_count);
                        Marshal.FreeHGlobal(attr_buff);
                        foreach (int value in dest)
                        {
                            attr_Data.Append(value);
                            attr_Data.Append(",");
                        }
                    }
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT32:
                    {
                        IntPtr attr_buff = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * data_count);
                        HDF4Helper.SDreadattr(objId, attIndex, attr_buff);
                        int[] dest = new int[data_count];
                        Marshal.Copy(attr_buff, dest, 0, data_count);
                        Marshal.FreeHGlobal(attr_buff);
                        foreach (uint value in dest)
                        {
                            attr_Data.Append(value);
                            attr_Data.Append(",");
                        }
                    }
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT32:
                    {
                        IntPtr attr_buff = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * data_count);
                        HDF4Helper.SDreadattr(objId, attIndex, attr_buff);
                        float[] dest = new float[data_count];
                        Marshal.Copy(attr_buff, dest, 0, data_count);
                        Marshal.FreeHGlobal(attr_buff);
                        foreach (float value in dest)
                        {
                            attr_Data.Append(value);
                            attr_Data.Append(",");
                        }
                    }
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT64:
                    {
                        IntPtr attr_buff = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * data_count);
                        HDF4Helper.SDreadattr(objId, attIndex, attr_buff);
                        double[] dest = new double[data_count];
                        Marshal.Copy(attr_buff, dest, 0, data_count);
                        Marshal.FreeHGlobal(attr_buff);
                        foreach (double value in dest)
                        {
                            attr_Data.Append(value);
                            attr_Data.Append(",");
                        }
                    }
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT8:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT8:
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT16:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT16:
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NFLOAT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NFLOAT64:
                    break;
                default:
                    break;
            }
            string str = attr_Data.ToString();
            if (str != null)
            {
                if (str.EndsWith(","))
                    str = str.Substring(0, str.Length - 1);
                str = str.Replace('\0', ' ');
            }
            return str;
        }

        public void Dispose()
        {
        }
    }
}
