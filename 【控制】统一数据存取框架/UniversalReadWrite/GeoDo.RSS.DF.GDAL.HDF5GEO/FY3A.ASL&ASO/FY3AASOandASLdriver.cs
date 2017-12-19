using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL;
using System.IO;
using GeoDo.HDF5;

namespace GeoDo.RSS.DF.GDAL.HDF5GEO.FY3A.ASL_ASO
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class FY3AASOandASLdriver : RasterDataDriver
    {
        public FY3AASOandASLdriver()
            : base()
        {
            _name = "FY3A_ASLASO";
            _fullName = "FY3A ASO and ASL";
        }

        public FY3AASOandASLdriver(string name, string fullName)
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
            Hdf5Operator hdfic = new Hdf5Operator(fileName);
            string[] HDFdatasets = hdfic.GetDatasetNames;
            bool matched = false;
            foreach (String[] sets in FY3AASOandASLProvider._datasets)
            {
                foreach (string set in sets)
                {
                    matched = false;
                    for (int i = 0; i < HDFdatasets.Length; i++)
                    {
                        if (HDFdatasets[i] == set)
                        {
                            matched = true;
                        }
                    }
                    if (!matched)
                    {
                        break;
                    }
                }
                if (matched)
                    return true;
            }
            return false;
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new FY3AASOandASLProvider(fileName, header1024, this, args);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new FY3AASOandASLProvider(fileName, null, this, args);
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
