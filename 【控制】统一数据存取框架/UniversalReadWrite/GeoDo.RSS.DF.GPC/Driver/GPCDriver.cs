using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using System.IO;


namespace GeoDo.RSS.DF.GPC
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class GPCDriver : RasterDataDriver
    {
        public GPCDriver()
            : base()
        {
            _name = "GCP";
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new GPCRasterDataProvider(fileName, header1024, this, args);
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            string ext = Path.GetExtension(fileName).ToUpper();
            FileInfo finfo = new FileInfo(fileName);
            long size = finfo.Length;
            if ((ext == ".GPC") && size == 871000)//871,000
                return true;
            return false;
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

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new GPCRasterDataProvider(fileName, null, this, args);
        }
    }
}
