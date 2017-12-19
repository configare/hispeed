using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public class CandidataPixelFilterPipe:ICandidatePixelFilterPipe
    {
        private IList<ICandidatePixelFilter> _filters = new List<ICandidatePixelFilter>();

        public CandidataPixelFilterPipe()
        { 
        }

        public IList<ICandidatePixelFilter> Filters
        {
            get { return _filters; }
        }

        public void Reset()
        {
            if (_filters == null || _filters.Count == 0)
                return;
            foreach (ICandidatePixelFilter filter in _filters)
                if (filter != null)
                    filter.Reset();
        }

        public int[] Filter(IRasterDataProvider dataProvider, int[] aoi,Action<string> contextMessage)
        {
            if (_filters == null || _filters.Count == 0)
                return null;
            Size size = new Size(dataProvider.Width, dataProvider.Height);
            Rectangle rect = AOIHelper.ComputeAOIRect(aoi, size);
            int[] rolledAOI = aoi;
            foreach (ICandidatePixelFilter filter in _filters)
            {
                if (filter != null && filter.IsEnabled && !filter.IsFiltered)
                {
                    if (contextMessage != null)
                        contextMessage("正在执行\"" + (filter.Name??filter.ToString())+"\"...");
                    rolledAOI = filter.Filter(dataProvider, rect, rolledAOI);
                    rect = AOIHelper.ComputeAOIRect(rolledAOI, size);
                }
            }
            return rolledAOI;
        }
    }
}
