using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public class LandRasterDictionaryTemplate : RasterDictionaryTemplate<byte>
    {
        public unsafe LandRasterDictionaryTemplate(string rasterFile, string codeFile)
        {
            using (IRasterDataProvider prd = GeoDataDriver.Open(rasterFile) as IRasterDataProvider)
            {
                _pixelValues = new byte[prd.Width * prd.Height];
                fixed (byte* buffer = _pixelValues)
                {
                    IntPtr ptr = new IntPtr(buffer);
                    prd.GetRasterBand(1).Read(0, 0, prd.Width, prd.Height, ptr, enumDataType.Byte, prd.Width, prd.Height);
                }
                _hdrInfo = HdrFile.LoadFrom(HdrFile.GetHdrFileName(rasterFile));
                _pixelNames = ParsePixelNames(codeFile);
                ExtractFieldsFromHdr();
            }
        }

        private Dictionary<byte, string> ParsePixelNames(string codeFile)
        {
            Dictionary<byte, string> names = new Dictionary<byte, string>();
            string[] lines = File.ReadAllLines(codeFile, Encoding.Default);
            string[] parts = null;
            foreach (string lne in lines)
            {
                parts = lne.Split('=');
                names.Add(byte.Parse(parts[0]), parts[1]);
            }
            return names;
        }
    }
}
