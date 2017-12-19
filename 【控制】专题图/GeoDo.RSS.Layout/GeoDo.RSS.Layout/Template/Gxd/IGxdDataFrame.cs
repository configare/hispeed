using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.Layout
{
    public interface IGxdDataFrame:IGxdItem
    {
        string Name { get; }
        GxdEnvelope Envelope { get; set; }
        List<IGxdRasterItem> GxdRasterItems { get; }
        IGxdVectorHost GxdVectorHost { get; }
        /// <summary>
        /// ESRI prj文件内容
        /// </summary>
        string SpatialRef { get; set; }
        XElement GeoGridXml { get; }
    }
}
