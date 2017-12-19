using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.HDF4;
using GeoDo.RSS.Core.DF;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.HDF5;

namespace GeoDo.RSS.DF.GDAL.HDF5GEO
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class FY3HDFL2ProductDriver : RasterDataDriver, IFY3HDFL2ProductDriver
    {
        public FY3HDFL2ProductDriver()
            : base()
        {
            _name = "FY3HDFL2Pro";
            _fullName = "FY-3 HDF L2 Product Geographic Projection Driver";
        }

        public FY3HDFL2ProductDriver(string name, string fullName)
            : base(name, fullName)
        {
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            return IsCompatible(fileName, header1024);
        }

        public static bool IsCompatible(string fileName, byte[] header1024)
        {
            if (header1024 == null)
            {
                header1024 = new byte[1024];
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(header1024, 0, 1024);
                    fs.Close();
                }
            }
            if (!HDF5Helper.IsHdf5(header1024) && !HDF4Helper.IsHdf4(header1024))
                return false;
            L2ProductDefind[] l2Pros = L2ProductDefindParser.GetL2ProductDefs(fileName);
            if (l2Pros == null || l2Pros.Length == 0)
                return false;
            return true;
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new FY3HDFL2ProductProvider(fileName, header1024, this, args);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new FY3HDFL2ProductProvider(fileName, null, this, args);
        }

        public override void Delete(string fileName)
        {
            File.Delete(fileName);
        }

        public override IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options)
        {
            throw new NotImplementedException();
        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            throw new NotImplementedException();
        }
    }

}
