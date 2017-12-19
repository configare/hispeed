using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class Grib1DataDriver:RasterDataDriver
    {
        public Grib1DataDriver()
            : base()
        {
            _fullName = _name = "GRIB1 Data Driver";
        }

        public Grib1DataDriver(string name, string fullName)
            : base(name, fullName)
        {
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            using (MemoryStream ms = new MemoryStream(header1024))
            {
                StringBuilder hdr = new StringBuilder();
                int match = 0;
                while (ms.Position < 1024)
                {
                    // 代码必须是 "G" "R" "I" "B"
                    char c = (char)ms.ReadByte();

                    hdr.Append((char)c);
                    if (c == 'G')
                    {
                        match = 1;
                    }
                    else if ((c == 'R') && (match == 1))
                    {
                        match = 2;
                    }
                    else if ((c == 'I') && (match == 2))
                    {
                        match = 3;
                    }
                    else if ((c == 'B') && (match == 3))
                    {
                        //检查是否为GRIB1
                        int edition = TryGetEdition(ms);
                        if (edition == 1)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        match = 0;
                    }
                }
                return false;
            }
        }

        private int TryGetEdition(MemoryStream ms)
        {
            int edition = 0;
            if (ms.Position < ms.Length-3)
            {
                ms.Seek(3, SeekOrigin.Current);
                edition = ms.ReadByte();
            }
            return edition;
        }

        public override void Delete(string fileName)
        {

        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            throw new NotImplementedException();
        }

        public override IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options)
        {
            throw new NotImplementedException();
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new GRIB1DataProvider(fileName, header1024, this, access);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new GRIB1DataProvider(fileName, null, this, access);
        }
    }
}
