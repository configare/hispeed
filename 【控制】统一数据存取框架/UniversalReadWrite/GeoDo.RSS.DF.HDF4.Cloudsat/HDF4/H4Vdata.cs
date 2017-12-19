using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public class H4Vdata : HObject, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theFile"></param>
        /// <param name="theName"></param>
        /// <param name="thePath"></param>
        /// <param name="oid">oid中第一个是ref，第二个是tag</param>
        public H4Vdata(HFile theFile, string theName, string thePath, long[] oid)
            : base(theFile, theName, thePath, oid)
        { }

        /// <summary>
        /// 唯一标识,初始化后，必须
        /// </summary>
        public int Vdata_ref { get; set; }

        /// <summary>
        /// 这个id每次连接上后读不一样，所以记录无意义
        /// </summary>
        public int Vdata_id { get; set; }

        /// <summary>
        /// 记录个数，即Vdata表格数据的行数
        /// </summary>
        public int N_records { get; set; }

        /// <summary>
        /// Interlace Mode
        /// </summary>
        public int Interlace { get; set; }

        /// <summary>
        /// Vdata表格字段名称
        /// </summary>
        public string[] Fields { get; set; }

        /// <summary>
        /// 4(字节数)
        /// </summary>
        public int Vdata_size { get; set; }

        public string Vdata_name { get; set; }

        public string Vdata_class { get; set; }

        public string Vdata_type { get; set; }

        public FieldList[] FieldLists
        {
            get
            {
                return _fieldLists;
            }
        }

        private FieldList[] _fieldLists = null;

        private void Init()
        {
            int vdata_id = Open();
            if (vdata_id < 0)
                return;
            string[] fields = Fields;
            FieldList[] fls = new FieldList[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                int fieldorder = HDF4API.VFfieldorder(vdata_id, 0);
                int fieldtype = HDF4API.VFfieldtype(vdata_id, 0);
                int fieldsize = HDF4API.VFfieldisize(vdata_id, 0);
                FieldList fl = new FieldList();
                fl.Fname = fields[i];
                fl.Forder = fieldorder;
                fl.Ftype = fieldtype;
                fl.Fsize = fieldsize;
                fls[i] = fl;
            }
        }

        private FieldList FindField(string field)
        {
            if (_fieldLists == null)
                return null;
            for (int i = 0; i < _fieldLists.Length; i++)
            {
                if (_fieldLists[i].Fname == field)
                    return _fieldLists[i];
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nStart">int[1]</param>
        /// <param name="selectedDims">int[1]</param>
        /// <param name="ptr"></param>
        public object Read(int nStart, int count, string field)
        {
            if (nStart < 0 || count <= 0 || nStart >= N_records || nStart + count > N_records)
            {
                string strErrInfo = "HDF4读取表格数据，读取参数异常";
                throw new Exception(strErrInfo);
            }
            int vs_id = Open();
            if (vs_id < 0)
                return null;
            try
            {
                //FieldList fl = FindField(field);
                int vdata_ref = HDF4API.VSfind(fileFormat.File_Id, Vdata_name);
                vs_id = HDF4API.VSattach(fileFormat.File_Id, vdata_ref, "r");
                int status = 0;
                string fields = null;
                if (string.IsNullOrWhiteSpace(field))
                    fields = string.Join(",", Fields);
                else
                    fields = field;
                int num_of_records;
                int records = count;
                if (nStart != 0)//偏移
                {
                    int record_indes = 0;
                    int record_pos = 0;
                    record_indes = nStart;
                    record_pos = HDF4API.VSseek(Vdata_id, record_indes);
                    int[] recordSize = { 0 };
                    HDF4API.VSQueryvsize(Vdata_id, ref recordSize);
                    int s = recordSize[0];
                }
                if (fields != string.Join(",", Fields))
                {
                    status = HDF4API.VSsetfields(vs_id, fields);
                }
                int fieldtype = HDF4API.VFfieldtype(vs_id, 0);
                int fieldsize = HDF4API.VFfieldisize(vs_id, 0);
                int memsize = fieldsize * records;
                IntPtr ptrBuffer = Marshal.AllocHGlobal(memsize);
                try
                {
                    num_of_records = HDF4API.VSread(vs_id, ptrBuffer, records, INTERLACE_MODE.FULL_INTERLACE);
                    dynamic buffer = null;
                    switch (fieldtype)
                    {
                        case HDFConstants.DFNT_FLOAT32:
                            {
                                buffer = new float[num_of_records];
                                Marshal.Copy(ptrBuffer, buffer, 0, num_of_records);
                            }
                            break;
                        case HDFConstants.DFNT_INT16:
                            {
                                buffer = new short[num_of_records];
                                Marshal.Copy(ptrBuffer, buffer, 0, num_of_records);
                            }
                            break;
                        default:
                            break;
                    }
                    return buffer;
                }
                finally
                {
                    Marshal.Release(ptrBuffer);
                }
            }
            finally
            {
                Close(vs_id);
            }
        }

        public int Open()
        {
            int vs_id = -1;
            try
            {
                vs_id = HDF4API.VSattach(base.fileFormat.File_Id, Vdata_ref, "r");
            }
            catch (Exception ex)
            {
                vs_id = -1;
            }
            return vs_id;
        }

        private void Close(int vs_id)
        {
            try
            {
                HDF4API.VSdetach(vs_id);
            }
            catch (Exception ex)
            {
                ;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6}", Vdata_id, Vdata_ref, N_records, Interlace, Vdata_name, string.Join(",", Fields), Vdata_size);
        }

        public int Vdata_tag { get; set; }

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// Vdata中的字段信息
    /// </summary>
    public class FieldList
    {
        public string Fname;
        public int Ftype;
        public int Forder;
        public int Fsize;
    }
}
