using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.HDF4;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{

    public class HDFAttribute
    {
        private int _sd_id = 0;
        private int _attr_index = 0;
        private string _name = "";
        private HDF4Helper.DataTypeDefinitions _data_type;
        private int _count = 0;
        private dynamic _value = null;

        public HDFAttribute()
        { }

        public static HDFAttribute Load(int sd_id, int attr_index)
        {
            int status = 0;
            StringBuilder attr_name = new StringBuilder();
            GeoDo.HDF4.HDF4Helper.DataTypeDefinitions data_type;
            int count;
            status = HDF4Helper.SDattrinfo(sd_id, attr_index, attr_name, out data_type, out count);
            HDFAttribute ds = new HDFAttribute();
            ds._sd_id = sd_id;
            ds._attr_index = attr_index;
            ds._name = attr_name.ToString();
            ds._data_type = data_type;
            ds._count = count;
            return ds;
        }

        public string Name
        {
            get { return _name; }
        }

        public HDF4Helper.DataTypeDefinitions DataType
        {
            get { return _data_type; }
        }

        public int Count
        {
            get { return _count; }
        }

        public dynamic Value
        {
            get
            {
                if (_value == null)
                    GetValue();
                return _value;
            }
        }

        private void GetValue()
        {
            int status = 0;
            switch (_data_type)
            {
                case HDF4Helper.DataTypeDefinitions.DFNT_CHAR8://string
                //case HDF4Helper.DataTypeDefinitions.DFNT_CHAR:
                case HDF4Helper.DataTypeDefinitions.DFNT_UCHAR8:
                    //case HDF4Helper.DataTypeDefinitions.DFNT_UCHAR:
                    {
                        IntPtr ptr = Marshal.AllocHGlobal(_count * 8);
                        status = HDF4Helper.SDreadattr(_sd_id, _attr_index, ptr);
                        var buffer = new byte[_count];
                        Marshal.Copy(ptr, buffer, 0, _count);
                        Marshal.FreeHGlobal(ptr);
                        _value = Encoding.ASCII.GetString(buffer);//new ASCIIEncoding()
                    }
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_INT8:
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT8:
                    {
                        IntPtr ptr = Marshal.AllocHGlobal(_count * 8);
                        status = HDF4Helper.SDreadattr(_sd_id, _attr_index, ptr);
                        var buffer = new byte[_count];
                        Marshal.Copy(ptr, buffer, 0, _count);
                        Marshal.FreeHGlobal(ptr);
                        _value = buffer;
                    }
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_INT16:
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT16:
                    {
                        IntPtr ptr = Marshal.AllocHGlobal(_count * 16);
                        status = HDF4Helper.SDreadattr(_sd_id, _attr_index, ptr);
                        var buffer = new short[_count];
                        Marshal.Copy(ptr, buffer, 0, _count);
                        Marshal.FreeHGlobal(ptr);
                        _value = buffer;
                    }
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_INT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT32:
                    {
                        IntPtr ptr = Marshal.AllocHGlobal(_count * 32);
                        status = HDF4Helper.SDreadattr(_sd_id, _attr_index, ptr);
                        var buffer = new int[_count];
                        Marshal.Copy(ptr, buffer, 0, _count);
                        Marshal.FreeHGlobal(ptr);
                        _value = buffer;
                    }
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT32:
                    {
                        IntPtr ptr = Marshal.AllocHGlobal(_count * 32);
                        status = HDF4Helper.SDreadattr(_sd_id, _attr_index, ptr);
                        var buffer = new float[_count];
                        Marshal.Copy(ptr, buffer, 0, _count);
                        Marshal.FreeHGlobal(ptr);
                        _value = buffer;
                    }
                    break;
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT64:
                    {
                        IntPtr ptr = Marshal.AllocHGlobal(_count * 64);
                        status = HDF4Helper.SDreadattr(_sd_id, _attr_index, ptr);
                        var buffer = new double[_count];
                        Marshal.Copy(ptr, buffer, 0, _count);
                        Marshal.FreeHGlobal(ptr);
                        _value = buffer;
                    }
                    break;
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
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", _name, _data_type, _count);
        }
    }
}
