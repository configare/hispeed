using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeCell.AgileMap.Core
{
    public static class RasterReaderFactory
    {
        public static IRasterReader GetRasterReader(string filename)
        {
            string ext = Path.GetExtension(filename).ToUpper();
            switch (ext)
            { 
                case ".RST":
                    return new RstReader(filename);
            }
            throw new NotSupportedException("不支持的影像格式\""+filename+"\"!");
        }
    }
}
