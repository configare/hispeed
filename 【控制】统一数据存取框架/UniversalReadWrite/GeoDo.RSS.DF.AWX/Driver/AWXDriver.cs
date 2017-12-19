using System;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL;

namespace GeoDo.RSS.DF.AWX
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
	public class AWXDriver : GDALRasterDriver
	{
		public AWXDriver()
			: base()
		{
			_name = "AWX";
			_fullName = "AWX Data Driver";
		}

		public AWXDriver(string name, string fullName)
			: base(name, fullName)
		{
			
		}

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new AWXDataProvider(fileName, this, access);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new AWXDataProvider(fileName, null, this, access); ;
        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            throw new NotImplementedException();
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            string fileExtension = Path.GetExtension(fileName).ToUpper();
            return (fileExtension == ".AWX") ? true : false;
        }
	}
}