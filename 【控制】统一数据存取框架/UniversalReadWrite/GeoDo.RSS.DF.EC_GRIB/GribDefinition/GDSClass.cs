using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.EC_GRIB
{
    internal class GDSClass : IThirdSection
    {
        //全部默认为等经纬度投影
        internal int GDSLength { get; set; }
        internal int LatPointsNum { get; set; }//wid
        internal int LonPointsNum { get; set; }//hei
        //单位为度
        internal float LatResolution { get; set; }//纬度
        internal float LonResolution { get; set; }//经度

        internal GDSClass(FileStream fs, BinaryReader br)
        {
            if (fs == null || br == null)
                return;
            //01-03	Length in bytes of the GDS
            GDSLength = MathHelper.Bytes2Int(br.ReadBytes(3));
            //04	NV, the number of vertical coordinate parameters
            //05	PV, the location (octet number) of the list of vertical coordinate parameters, if present or PL, the location (octet number) of the list of numbers of points in each row (when no vertical parameters are present), if present or (all bits set to 1) if neither are present
            //06	Data representation type
            fs.Seek(3, SeekOrigin.Current);

            //07-32	Grid description, according to data representation type, except Lambert, Mercator or Space View.
            //07-08	Ni - No. of points along a latitude circle
            LatPointsNum = MathHelper.Bytes2Int(br.ReadBytes(2));
            //09-10	Nj - No. of points along a longitude meridian
            LonPointsNum = MathHelper.Bytes2Int(br.ReadBytes(2));
            //11-13	La1 - latitude of first grid point units: millidegrees (degrees x 1000)
            //values limited to range 0 - 90,000 bit 1 (leftmost) set to 1 for south latitude
            //14-16	Lo1 - longitude of first grid point units: millidegrees (degrees x 1000)
            //values limited to range 0 - 360,000 bit 1 (leftmost) set to 1 for west longitude
            //17	Resolution and component flags
            //18-20	La2 - Latitude of last grid point (same units, value range, and bit 1 as La1)
            //21-23	Lo2 - Longitude of last grid point (same units, value range, and bit 1 as Lo1)
            //经度方向上的增量
            //24-25	Di - Longitudinal Direction Increment (same units as Lo1) (if not given, all bits set = 1)
            fs.Seek(13, SeekOrigin.Current);
            LonResolution =(float) MathHelper.Bytes2Int(br.ReadBytes(2)) / 1000;
            //纬度方向上的增量
            //26 - 27	Regular Lat/Lon Grid:
            //Dj - Latitudinal Direction Increment (same units as La1) (if not given, all bits set = 1)
            //Gaussian Grid:
            //N - number of latitude circles between a pole and the equator. Mandatory if Gaussian Grid specified
            LatResolution = (float)MathHelper.Bytes2Int(br.ReadBytes(2)) / 1000;
            //28	Scanning mode flags            
            //29 - 32	Reserved (set to zero)
            //07-42	Grid description for Lambert or Mercator grid
            //07-44	Grid description for Space View perspective grid
            //PV	List of vertical coordinate parameters. length = NV x 4 octets; if present, then PL = 4 x NV + PV
            //PL	List of numbers of points in each row, used for quasi-regular grids. length = NROWS x 2 octets, where NROWS is the total number of rows defined within the grid description
            br.ReadBytes(GDSLength - 27);

        }
    }
}
