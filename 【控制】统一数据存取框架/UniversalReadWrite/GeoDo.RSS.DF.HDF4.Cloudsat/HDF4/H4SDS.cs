using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.HDF4;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    /// <summary>
    /// Multifile Scientific Data Set Interface (SD)
    /// </summary>
    public class H4SDS:IDisposable
    {
        private const int MAX_DIMSIZES = 10;
        private string _sds_name;
        private int _sds_id;//这个每次读取都不唯一，不用记录
        private int _rank;
        private int _num_attrs;
        private int[] _dimsizes = null;
        private HDF4Helper.DataTypeDefinitions _datatype;
        private HDFAttribute[] _sdAttributes = null;
        private int _sds_ref;
        private int _sd_id;

        /// <summary>
        /// SDS唯一值有_sd_id和index确定
        /// </summary>
        private H4SDS()
        { }

        public static H4SDS Load(int sd_id, int sds_index)
        {
            int status = 0;
            int sds_id = 0;
            int sds_ref = 0;
            StringBuilder sds_name = new StringBuilder();
            int rank = 0;
            int[] dimsizes = new int[MAX_DIMSIZES];
            HDF4Helper.DataTypeDefinitions datatype;
            int num_attrs = 0;
            sds_id = H4SDAPI.SDselect(sd_id, sds_index);
            sds_ref = HDF4API.SDidtoref(sds_id);
            int sds_index2 = HDF4API.SDreftoindex(sd_id, sds_ref);
            
            //HDF4Helper.SDreaddata

            status = HDF4Helper.SDgetinfo(sds_id, sds_name, out rank, dimsizes, out datatype, out num_attrs);
            status = HDF4Helper.SDendaccess(sds_id);
            int[] dims = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                dims[i] = dimsizes[i];
            }
            H4SDS ds = new H4SDS();
            ds._sd_id = sd_id;
            ds._sds_ref = sds_ref;
            ds._sds_id = sds_id;
            ds._sds_name = sds_name.ToString();
            ds._rank = rank;
            ds._dimsizes = dims;
            ds._datatype = datatype;
            ds._num_attrs = num_attrs;
            return ds;
        }

        public int Open()
        {
            int id = -1;
            try
            {
                int index = 0;
                //int tag = (int)oid[0];
                //if (tag == H4SDS.DFTAG_NDG_NETCDF)
                //{
                //    index = (int)oid[1]; //HDFLibrary.SDidtoref(id) fails for netCDF
                //}
                //else
                //{
                index = HDF4API.SDreftoindex(_sd_id, _sds_ref);//(int)oid[1]
                //}
                id = HDF4Helper.SDselect(_sd_id, index);
            }
            catch (Exception ex)
            {
                id = -1;
            }
            return id;
        }

        public void Close(int sds_id)
        {
            try
            {
                HDF4Helper.SDendaccess(sds_id);
            }
            catch
            {
                Console.WriteLine("Error,SDendaccess:" + sds_id);
            }
        }

        public string Name
        {
            get { return _sds_name; }
        }

        public int Sds_id
        {
            get { return _sds_id; }
        }

        public int Rank
        {
            get { return _rank; }
        }

        public int Num_attrs
        {
            get { return _num_attrs; }
        }

        public int[] Dimsizes
        {
            get { return _dimsizes; }
        }

        public HDF4Helper.DataTypeDefinitions Datatype
        {
            get { return _datatype; }
        }

        public HDFAttribute[] SDAttributes
        {
            get
            {
                if (_sdAttributes == null)
                    ReadAttribute();
                return _sdAttributes;
            }
        }

        private void ReadAttribute()
        {
            HDFAttribute[] sdAttributes = new HDFAttribute[_num_attrs];
            for (int i = 0; i < _num_attrs; i++)
            {
                sdAttributes[i] = HDFAttribute.Load(_sds_id, i);
            } 
            _sdAttributes = sdAttributes;
        }

        /// <summary>
        /// 读取数据集值
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stride"></param>
        /// <param name="edges"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool Read(int[] start, int[] stride, int[] edges, IntPtr ptr)
        {
            int sds_id = -1;
            try
            {
                sds_id = Open();
                if (sds_id == -1)
                    return false;
                int status = 0;
                status = HDF4Helper.SDreaddata(sds_id, start, stride, edges, ptr);
                return true;
            }
            finally
            {
                Close(sds_id);
            }
        }

        private int GetSize(HDF4Helper.DataTypeDefinitions _datatype)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6}", _sds_id, _sds_ref, _sds_name, _rank, string.Join("*", _dimsizes), _datatype, _num_attrs);
        }

        public void Dispose()
        {
            if (_sds_id >= 0)
            {
                Close(_sds_id);
            }
        }
    }
}
