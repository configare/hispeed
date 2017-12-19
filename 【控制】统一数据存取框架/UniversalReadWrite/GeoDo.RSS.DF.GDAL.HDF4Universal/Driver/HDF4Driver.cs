using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using OSGeo.GDAL;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.HDF4;

namespace GeoDo.RSS.DF.GDAL.HDF4Universal
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class HDF4Driver : RasterDataDriver
    {
        public HDF4Driver()
            : base()
        {
            _name = "HDF4";
            Gdal.AllRegister();
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;
            string ext = Path.GetExtension(fileName).ToUpper();
            if (ext != ".HDF" || !HDF4Helper.IsHdf4(header1024))
                return false;
            return true;
        }
        public override void Delete(string fileName)
        {
            File.Delete(fileName);
        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            throw new NotImplementedException();
        }

        public override IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options)
        {
            throw new NotImplementedException();
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new Hdf4RasterDataProvider(fileName, header1024, this, args);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new Hdf4RasterDataProvider(fileName, null, this, args);
        }
    }
}
