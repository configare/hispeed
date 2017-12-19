using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RasterProject;

namespace TestForm
{
    public  class ProjectParas
    {
        private IRasterDataProvider _srcRaster = null;
        private FY3_MERSI_PrjSettings _prjSetting = null;
        private ISpatialReference _dstSpatialRef = null;
        private PrjEnvelope _dstEnvelope = null;
        private Action<int, string> _progressCallback = null;
        private IRasterBand[] _bands = null;

        public IRasterBand[] Bands
        {
            get { return _bands; }
            set { _bands = value; }
        }

        public IRasterDataProvider SrcRaster
        {
            get { return _srcRaster; }
            set { _srcRaster = value; }
        }

        public FY3_MERSI_PrjSettings PrjSetting
        {
            get { return _prjSetting; }
            set { _prjSetting = value; }
        }

        public ISpatialReference DstSpatialRef
        {
            get { return _dstSpatialRef; }
            set { _dstSpatialRef = value; }
        }

        public PrjEnvelope DstEnvelope
        {
            get { return _dstEnvelope; }
            set { _dstEnvelope = value; }
        }

        public Action<int, string> ProgressCallback
        {
            get { return _progressCallback; }
            set { _progressCallback = value; }
        }
    }
}
