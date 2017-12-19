using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.DF.EC_GRIB
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class ECGribDataDriver : GeoDataDriver
    {
        public ECGribDataDriver()
            : base()
        {
            _fullName = _name = "GRIB";
        }

        public ECGribDataDriver(string name, string fullName)
            : base(name, fullName)
        {
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new ECGribDataProvider(fileName, this);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new ECGribDataProvider(fileName, this);
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            using (MemoryStream ms = new MemoryStream(header1024))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    char[] chars = reader.ReadChars(4); 
                    string flag = string.Join("", chars);
                    return string.Equals(flag.ToUpper(), "GRIB");
                }
            }
        }

        public override void Delete(string fileName)
        {
            File.Delete(fileName);
        }
    }
}
