using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class ZoomSetting
    {
        private double _zoomfactor;
        private double _zoomminpercent;
        private double _zoommaxpercent;
        private List<ZoomStepByScale> _zoomStepByScales;

        public ZoomSetting(List<ZoomStepByScale> list)
        {
            _zoomStepByScales = list;
        }

        public double Zoomfactor
        {
            get { return _zoomfactor; }
            set { _zoomfactor = value; }
        }

        public double Zoomminpercent
        {
            get { return _zoomminpercent; }
            set { _zoomminpercent = value; }
        }

        public double Zoommaxpercent
        {
            get { return _zoommaxpercent; }
            set { _zoommaxpercent = value; }
        }

        public ZoomStepByScale[] ZoomStepByScales
        {
            get
            {
                return _zoomStepByScales != null && _zoomStepByScales.Count > 0 ? _zoomStepByScales.ToArray() : null;
            }
        }

        public int GetZoomStepByScale(float scale)
        {
            foreach (ZoomStepByScale item in _zoomStepByScales)
            {
                if (scale >= item.Minscale && scale < item.Maxscale)
                {
                    return item.Steps;
                }
            }
            return 0;
        }
    }
}
