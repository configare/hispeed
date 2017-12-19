using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class FileNameGeneratorDefault:IFileNameGenerator
    {
        static IFileNameGenerator _instance;

        public FileNameGeneratorDefault()
        {
            _instance = this;
        }

        public static IFileNameGenerator GetFileNameGenerator()
        {
            if (_instance == null)
                new FileNameGeneratorDefault();
            return _instance;
        }

        public string NewFileName(RasterIdentify rasterIdentify)
        {
            return rasterIdentify.ToWksFullFileName(rasterIdentify.Format);
        }
    }
}
