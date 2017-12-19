using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.HDF5;
using OSGeo.GDAL;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.DF.GDAL.MWRIX.SIC
{
    /// <summary>
    /// 实现对微波湿度计的L2级产品数据的支持
    /// </summary>
    //[Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class SICDriver : RasterDataDriver
    {
        public SICDriver()
            :base()
        {
            Gdal.AllRegister();
            _name = "MWRIX.SIC";
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new SICRasterDataProvider(fileName, header1024, this, access, args);
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            string ext = Path.GetExtension(fileName).ToUpper();
            if (!HDF5Helper.IsHdf5(header1024))
                return false;
            return SICRasterDataProvider.IsSupport(fileName);
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
            return new SICRasterDataProvider(fileName, null, this, access, args);
        }
    }
}
