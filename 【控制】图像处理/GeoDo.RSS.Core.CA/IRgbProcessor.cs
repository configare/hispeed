using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.ComponentModel.Composition;
using System.Xml;

namespace GeoDo.RSS.Core.CA
{
    public interface IRgbProcessor:IDisposable
    {
        string Name { get; }
        RgbProcessorArg Arguments { get; set; }
        int BytesPerPixel { get; set; }
        void Process(BitmapData pdata);
        void Process(int[][] indexesOfAOIs, BitmapData pdata);
        void Reset();
        void CreateDefaultArguments();
        XmlElement ToXML(XmlDocument xmldoc);
    }
}
