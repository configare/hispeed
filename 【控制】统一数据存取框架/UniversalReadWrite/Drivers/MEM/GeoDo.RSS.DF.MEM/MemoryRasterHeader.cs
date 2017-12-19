using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Drawing;

namespace GeoDo.RSS.DF.MEM
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct MemoryRasterHeader
    {
        //new char[] { 'G', 'E', 'O', 'D', 'O', 'M', 'E', 'M', 'V', '0', '1' }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public char[] Identify;
        /*
         *      PrjId = Int16.Parse(spatialRef.ProjectionCoordSystem.Name.ENVIName);
                A = (float)datum.Spheroid.SemimajorAxis;
                B = (float)datum.Spheroid.SemiminorAxis;
                K0 = (float)datum.Spheroid.InverseFlattening;
                IProjectionCoordSystem prjCoordSystem = spatialRef.ProjectionCoordSystem;
                Sp1 = GetPrjParaValue(prjCoordSystem, "sp1");
                Sp2 = GetPrjParaValue(prjCoordSystem, "sp2");
                Lat0 = GetPrjParaValue(prjCoordSystem, "lat0");
                Lon0 = GetPrjParaValue(prjCoordSystem, "lon0");
                X0 = GetPrjParaValue(prjCoordSystem, "x0");
                Y0 = GetPrjParaValue(prjCoordSystem, "y0");
         */
        public int PrjId;
        public float A;
        public float B;
        public float K0;
        public float SP1;
        public float SP2;
        public float Lat0;
        public float Lon0;
        public float X0;
        public float Y0;
        //
        public float MinX;
        public float MinY;
        public float MaxX;
        public float MaxY;
        //
        public int BandCount;
        //
        public int Width;
        public int Height;
        //
        public int DataType;
        //产品标识
        public int ProductIdentify;
        //扩展头长度
        public int ExtendHeaderLength;

        public HdrFile ToHdrFile()
        {
            HdrFile hdr = new HdrFile();
            hdr.Bands = BandCount;
            hdr.Samples = Width;
            hdr.Lines = Height;
            hdr.DataType = HdrFile.DataTypeToIntValue(DataTypeHelper.Enum2DataType((enumDataType)DataType));
            hdr.HeaderOffset = Marshal.SizeOf(typeof(MemoryRasterHeader)) + ExtendHeaderLength;
            return hdr;
        }
    }
}
