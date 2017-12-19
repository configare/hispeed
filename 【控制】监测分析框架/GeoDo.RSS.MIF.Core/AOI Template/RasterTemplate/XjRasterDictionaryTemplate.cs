using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public class XjRasterDictionaryTemplate : RasterDictionaryTemplate<int>
    {
        public unsafe XjRasterDictionaryTemplate(string rasterFile, string codeFile)
        {
            using (IRasterDataProvider prd = GeoDataDriver.Open(rasterFile) as IRasterDataProvider)
            {
                int width = prd.Width;
                int height = prd.Height;
                bool isOutOfMemory = false;
            tryAgain:
                try
                {
                    _pixelValues = new int[width * height];
                }
                catch (OutOfMemoryException)
                {
                    isOutOfMemory = true;
                }
                if (isOutOfMemory)
                {
                    isOutOfMemory = false;
                    width /= 2;
                    height /= 2;
                    goto tryAgain;
                }
                _scaleFactor = prd.Width / width; 
                fixed (int* buffer = _pixelValues)
                {
                    IntPtr ptr = new IntPtr(buffer);
                    prd.GetRasterBand(1).Read(0, 0, prd.Width, prd.Height, ptr, enumDataType.Int32, width, height);
                }
                _hdrInfo = HdrFile.LoadFrom(HdrFile.GetHdrFileName(rasterFile));
                _pixelNames = ParsePixelNames(codeFile);
                ExtractFieldsFromHdr();
            }
        }

        private Dictionary<int, string> ParsePixelNames(string codeFile)
        {
            Dictionary<int, string> names = new Dictionary<int, string>();
            string[] lines = File.ReadAllLines(codeFile, Encoding.Default);
            string[] parts = null;
            foreach (string lne in lines)
            {
                parts = lne.Split('\t');
                names.Add(int.Parse(parts[0]), parts[1]);
            }
            return names;
        }
    }
}
