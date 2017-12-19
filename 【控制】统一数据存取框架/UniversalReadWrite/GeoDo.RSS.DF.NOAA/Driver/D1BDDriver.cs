using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel.Composition;
using OSGeo.GDAL;
using GeoDo.RSS.DF.GDAL;
using GeoDo.RSS.Core.DF;


namespace GeoDo.RSS.DF.NOAA
{
    [Export(typeof(IGeoDataDriver)),ExportMetadata("VERSION","1")]
    public class D1BDDriver:GDALRasterDriver,ID1BDDriver
    {
         public D1BDDriver()
            : base()
        {
            _name = "NOAA_1BD";
            _fullName = "NOAA 1BD Data Driver";
        }

         public D1BDDriver(string name, string fullName)
            :base(name,fullName)
        { 
        }

         protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, Core.DF.enumDataProviderAccess access, params object[] args)
         {
             return new D1BDDataProvider(fileName, header1024, this, access);
         }

         protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
         {
             return new D1BDDataProvider(fileName, this, access);
         }

         protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
         {
             return D1BDHeader.Is1BD(header1024);
         }

         public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
         {
             throw new NotImplementedException();
         }
        
    }
}
