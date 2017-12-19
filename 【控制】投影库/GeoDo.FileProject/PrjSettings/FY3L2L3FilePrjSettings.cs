using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    public class FY3L2L3FilePrjSettings : FilePrjSettings
    {
        protected IRasterDataProvider _secondaryOrbitRaster = null;      //250米分辨率的数据。

        public FY3L2L3FilePrjSettings()
            : base()
        {
        }

        /// <summary>
        /// 经纬度数据集所在Raster
        /// </summary>
        public IRasterDataProvider LocationFile
        {
            get { return _secondaryOrbitRaster; }
            set { _secondaryOrbitRaster = value; }
        }
    }
}
