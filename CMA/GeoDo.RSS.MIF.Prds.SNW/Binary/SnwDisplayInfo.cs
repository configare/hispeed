using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    /// <summary>
    /// 积雪辅助信息显示
    /// </summary>
    public class SnwDisplayInfo
    {
        protected IArgumentProvider _argumentProvider;

        private static IArgumentProvider _perArgumentProvider = null;
        private static SnwDisplayInfo _sinfo = null;
        private static int pervisiBandNo, persIBandNo, perfIBandNo;

        public static SnwFeatureCollection GetDisplayInfo(IArgumentProvider argumentProvider, int visiBandNo, int sIBandNo, int fIBandNo)
        {
            if (argumentProvider == null || argumentProvider.DataProvider == null)
            {
                return null;
            }
            if (_sinfo == null || _perArgumentProvider != argumentProvider || pervisiBandNo != visiBandNo || persIBandNo != sIBandNo || perfIBandNo != fIBandNo)
            {
                pervisiBandNo = visiBandNo;
                persIBandNo = sIBandNo;
                perfIBandNo = fIBandNo;
                _perArgumentProvider = argumentProvider;
                _sinfo = new SnwDisplayInfo(argumentProvider);
                _sinfo.UpdateDisplayInfo(visiBandNo, sIBandNo, fIBandNo);
                return _sinfo.DisplayInfo;
            }
            else
                return _sinfo.DisplayInfo;
        }

        public SnwDisplayInfo(IArgumentProvider argumentProvider)
        {
            _argumentProvider = argumentProvider;
        }
        private SnwFeatureCollection _snwFeatureCollection = null;

        public SnwFeatureCollection DisplayInfo
        {
            get
            {
                return _snwFeatureCollection;
            }
        }

        public void UpdateDisplayInfo(int VisibleCH, int ShortInfraredCH, int FarInfraredCH)
        {
            if (_argumentProvider.DataProvider == null)
                _snwFeatureCollection = null;
            Dictionary<int, SnwFeature> features = new Dictionary<int, SnwFeature>();
            SnwFeature tempSnw = null;
            RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(_argumentProvider);

            Rectangle rect = new Rectangle(0, 0, _argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height);
            int[] aoi = null;//积雪信息提示要求不使用AOI。
            rpVisitor.VisitPixel(rect, aoi,
            new int[] { VisibleCH, ShortInfraredCH, FarInfraredCH },
                (index, values) =>
                {
                    tempSnw = new SnwFeature();
                    tempSnw.Ndsi = (Int16)((values[0] - values[1]) * 1000f / (values[0] + values[1]));
                    tempSnw.Visible = values[0];
                    tempSnw.ShortInfrared = values[1];
                    tempSnw.FarInfrared = values[2];
                    features.Add(index, tempSnw);
                }
            );
            _snwFeatureCollection = new SnwFeatureCollection("积雪辅助信息", features); ;
        }
    }
}
