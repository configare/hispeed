using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class MosaicInfos
    {
        public string Project = null;
        public string ProjectArgs = null;
        public CoordEnvelope Envelope = null;
        public string ProjectAttrName = null;
        public string ProjectArgsAttrName = null;
        public string LeftUpAttrName = null;
        public string RightDownAttrName = null;
        public DataSetMosaicInfo[] DataSetMosaicInfos = null;
        private int _bandCount = 0;

        public int BandCount
        {
            get
            {
                if (DataSetMosaicInfos == null)
                    return 1;
                else
                {
                    foreach (DataSetMosaicInfo item in DataSetMosaicInfos)
                        _bandCount += item.BandCount;
                }
                return _bandCount;
            }
        }

        public MosaicInfos()
        { }
    }
}
