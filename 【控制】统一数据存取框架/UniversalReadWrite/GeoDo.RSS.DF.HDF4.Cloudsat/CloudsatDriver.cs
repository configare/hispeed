using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class CloudsatDriver : RasterDataDriver
    {
        public CloudsatDriver()
            : base()
        {
            _name = "Cloudsat";
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new CloudsatDataProvider(fileName, header1024, this, args);
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            return CloudsatDataProvider.IsSupport(fileName, header1024);
        }

        public override void Delete(string fileName)
        {
            throw new NotImplementedException();
        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            throw new NotImplementedException();
        }

        public override IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options)
        {
            throw new NotImplementedException();
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
