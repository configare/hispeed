using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public abstract class RasterDataDriver:GeoDataDriver,IRasterDataDriver
    {
        public RasterDataDriver()
            : base()
        { 
        }

        public RasterDataDriver(string name, string fullName)
            :base(name,fullName)
        { 
        }

        protected abstract override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024,enumDataProviderAccess access, params object[] args);

        protected abstract override bool IsCompatible(string fileName, byte[] header1024, params object[] args);

        public abstract override void Delete(string fileName);

        public abstract IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options);

        public abstract IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options);
    }
}
