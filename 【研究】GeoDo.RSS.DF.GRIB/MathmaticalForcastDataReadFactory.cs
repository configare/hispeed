using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
   public static class MathmaticalForcastDataReadFactory
    {
       public static IRasterDataProvider GetGrib1Data(string filename)
       {
           try
           {
               if (!File.Exists(filename))
                   return null;
               IRasterDataProvider raster = GeoDataDriver.Open(filename) as IRasterDataProvider;
               return raster;
           }
           catch
           {
               return null;
           }
       }
    }
}
