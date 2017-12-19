using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.DF.HJ
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class HJDriver : RasterDataDriver
    {
        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            string[] fnames;
            if (!IsOK(fileName, args, out fnames))
                return null;
            foreach (string f in fnames)
                if (!File.Exists(f))
                    throw new FileNotFoundException(f);
            return new LogicalRasterDataProvider(fileName, fnames, null);
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            string[] fnames;
            if (IsOK(fileName, args, out fnames))
                return true;
            return false;
        }

        private bool IsOK(string fileName, object[] args, out string[] fnames)
        {
            fnames = null;
            string extName = Path.GetExtension(fileName).ToUpper();
            if (extName != ".XML")
                return false;
            if (HJXML.Read(fileName, out fnames))
                return true;
            return false;
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
            string id;
            string[] fnames;
            if (!IsOK(fileName, args, out fnames))
                return null;
            return new LogicalRasterDataProvider(fileName, fnames, null);
        }
    }
}
