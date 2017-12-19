using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.HDF5;

namespace GeoDo.RSS.DF.GDAL.HDF5GEO.FY3HDF5GEO
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class FY3HDF5GEODriver : RasterDataDriver
    {
        public FY3HDF5GEODriver()
            : base()
        {
            _name = "FY3HDF5GEO";
            _fullName = "FY-3 HDF5 Geographic Projection Prds Driver";
        }

        public FY3HDF5GEODriver(string name, string fullName)
            : base(name, fullName)
        {
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
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
            if (!HDF5Helper.IsHdf5(header1024))
                return false;
            string geoprjstr = "Projection Type".ToUpper();
            string geoprj1 = "Geographic Longitude/Latitude".ToUpper();
            string geoprj2 = "Geographic Longitude/Latitute".ToUpper();
            string geoprj3 = "GLL";
            string prjType = null;
            Hdf5Operator hdfic = new Hdf5Operator(fileName);
            Dictionary<string, string> fileAttributes = hdfic.GetAttributes();
            foreach (KeyValuePair<string, string> fileAttribute in fileAttributes)
            {
                if (fileAttribute.Key.ToUpper() == geoprjstr)
                {
                    prjType = fileAttribute.Value.ToUpper();
                    break;
                }
            }
            if (prjType == null)
            {
                return false;
            }
            if (prjType != null && (prjType != geoprj1 && prjType != geoprj2 && prjType != geoprj3))
            {
                return false;
            }
            return true;
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new FY3HDF5GEOProvider(fileName, header1024, this, args);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new FY3HDF5GEOProvider(fileName, null, this, args);
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
