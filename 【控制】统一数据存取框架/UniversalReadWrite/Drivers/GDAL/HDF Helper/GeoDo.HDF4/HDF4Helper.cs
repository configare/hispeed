using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace GeoDo.HDF4
{
    public static class HDF4Helper
    {
        public static bool IsHdf4(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open,FileAccess.Read,FileShare.Read))
            {
                byte[] buffer = new byte[4];
                try
                {
                    fs.Read(buffer, 0, 4);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return false;
                }
                //HDF4的文件头，共4个字节
                byte[] hdf4 = new byte[4] { 0x0E, 0x03, 0x13, 0x01 };
                for (int i = 0; i < buffer.Length; i++)
                    if (hdf4[i] != buffer[i])
                        return false;
            }
            return true;
        }

        public static bool IsHdf4(byte[] bytes1024)
        {
            if (bytes1024 == null || bytes1024.Length < 4)
                return false;
            //HDF4的文件头，共4个字节
            byte[] hdf4 = new byte[4] { 0x0E, 0x03, 0x13, 0x01 };
            for (int i = 0; i < hdf4.Length; i++)
                if (hdf4[i] != bytes1024[i])
                    return false;
            return true;
        }

        #region DLL Imports
        private const string HDF4DLL = "hm426m.dll";

        #region file / dataset access

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDcreate(int sd_id, string name, DataTypeDefinitions data_type, int rank, int[] dimsizes);

        [DllImport(HDF4DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDstart(string filename, AccessCodes access_mode);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDendaccess(int sds_id);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDend(int sd_id);

        #endregion

        #region data access

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDselect(int sd_id, int sds_index);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDreaddata(int sds_id, int[] start, int[] stride, int[] edge, IntPtr buffer);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDwritedata(int sds_id, int[] start, int[] stride, int[] edge, IntPtr buffer);

        [DllImport(HDF4DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDgetdatastrs(int sds_id, StringBuilder label, StringBuilder unit, StringBuilder format, int length);

        [DllImport(HDF4DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDsetdatastrs(int sds_id, string label, string unit, string format, string coordsys);

        #endregion

        #region dim

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDgetdimid(int sds_id, int dim_index);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDdiminfo(int dim_id, StringBuilder name, out int size, out DataTypeDefinitions datay_type, out int num_attrs);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDgetdimstrs(int dim_id, StringBuilder label, StringBuilder unit, StringBuilder format, int length);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDgetdimscale(int dim_id, IntPtr data);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDsetdimname(int dim_id, string dim_name);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDsetdimscale(int dim_id, int count, DataTypeDefinitions data_type, IntPtr data);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDsetdimstrs(int dim_id, string label, string unit, string format);

        #endregion

        #region lookups / checks

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDcheckempty(int sds_id, out bool emptySDS);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDnametoindex(int sd_id, string sds_name);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDfileinfo(int sd_id, out int num_datasets, out int num_global_attrs);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDgetinfo(int sds_id, StringBuilder sds_name, out int rank, [In, Out] int[] dimsizes, out DataTypeDefinitions data_type,
            out int num_attrs);

        [DllImport(HDF4DLL,CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SDiscoordvar(int sds_id);

        #endregion

        #region attr

        [DllImport(HDF4DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDfindattr(int obj_id, string attr_name);

        [DllImport(HDF4DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDattrinfo(int obj_id, int attr_index, StringBuilder attr_name, out DataTypeDefinitions data_type, out int count);

        /// <summary>
        /// C中定义原参见
        /// http://www.hdfgroup.org/training/HDFtraining/RefManual/RM_Section_II_SD.fm22.html
        /// IntPtr attr_buff也可以定义为void* attr_buff
        /// </summary>
        /// <param name="obj_id"></param>
        /// <param name="attr_index"></param>
        /// <param name="attr_buff">这里也可以</param>
        /// <returns></returns>
        [DllImport(HDF4DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDreadattr(int obj_id, int attr_index, IntPtr attr_buff);

        [DllImport(HDF4DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDsetattr(int obj_id, string attr_name, DataTypeDefinitions data_type, int count, StringBuilder values);

        #endregion

        #endregion

        #region Enums / Consts

        public const uint DFNT_NATIVE = 4096;
        public const int MAX_VAR_DIMS = 32;

        [Flags]
        public enum DataTypeDefinitions : uint
        {
            DFNT_CHAR8 = 4,
            DFNT_CHAR = 4,
            DFNT_UCHAR8 = 3,
            DFNT_UCHAR = 3,
            DFNT_INT8 = 20,
            DFNT_UINT8 = 21,
            DFNT_INT16 = 22,
            DFNT_UINT16 = 23,
            DFNT_INT32 = 24,
            DFNT_UINT32 = 25,
            DFNT_FLOAT32 = 5,
            DFNT_FLOAT64 = 6,
            DFNT_NINT8 = DFNT_NATIVE | DFNT_INT8,
            DFNT_NUINT8 = DFNT_NATIVE | DFNT_UINT8,
            DFNT_NINT16 = DFNT_NATIVE | DFNT_INT16,
            DFNT_NUINT16 = DFNT_NATIVE | DFNT_UINT16,
            DFNT_NINT32 = DFNT_NATIVE | DFNT_INT32,
            DFNT_NUINT32 = DFNT_NATIVE | DFNT_UINT32,
            DFNT_NFLOAT32 = DFNT_NATIVE | DFNT_FLOAT32,
            DFNT_NFLOAT64 = DFNT_NATIVE | DFNT_FLOAT64
        }
        
        public enum AccessCodes : int
        {
            DFACC_READ = 1,
            DFACC_WRITE = 2,
            DFACC_CREATE = 4,
            DFACC_ALL = 7,
            DFACC_RDONLY = 1,
            DFACC_RDWR = 3
        }
        #endregion

        #region Helper Functions

        public static double[] SDgetdimscaledouble(int dim_id, int size)
        {
            double d = 0;
            IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(d) * size);
            SDgetdimscale(dim_id, p);
            double[] dest = new double[size];
            Marshal.Copy(p, dest, 0, size);
            Marshal.FreeHGlobal(p);
            return dest;
        }

        public static void SDsetdimscaledouble(int dim_id, double[] d)
        {
            IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(d[0]) * d.Length);
            Marshal.Copy(d, 0, p, d.Length);
            int res = SDsetdimscale(dim_id, d.Length, DataTypeDefinitions.DFNT_FLOAT64, p);
            Marshal.FreeHGlobal(p);
        }

        public static double SDreaddouble(int sds_id, int[] start, int[] edge)
        {
            double d = 0;
            IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(d));
            SDreaddata(sds_id, start, null, edge, p);
            double[] dest = new double[1];
            Marshal.Copy(p, dest, 0, 1);
            d = dest[0];
            Marshal.FreeHGlobal(p);
            return d;
        }

        public static void SDwritedouble(int sds_id, int[] start, int[] edge, double d)
        {
            IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(d));
            double[] a = new double[1];
            a[0] = d;
            Marshal.Copy(a, 0, p, 1);
            int res = SDwritedata(sds_id, start, null, edge, p);
            Marshal.FreeHGlobal(p);
        }

        #endregion
    }
}
