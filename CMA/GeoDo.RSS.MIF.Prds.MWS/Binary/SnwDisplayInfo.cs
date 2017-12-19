using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 积雪辅助信息显示，分别包括光学判识和微波判识
    /// </summary>
    public class SnwDisplayInfo
    {
        protected IArgumentProvider _argumentProvider;

        private static IArgumentProvider _perArgumentProvider = null;
        private static SnwDisplayInfo _sinfo = null;
        private static int pervisiBandNo, persIBandNo, perfIBandNo;
        private static int per18vBandNo, per23vBandNo, per36vBandNo, per89vBandNo;

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
        public static SnwFeatureCollectionMS GetDisplayInfoMS(IArgumentProvider argumentProvider, int ch18vBandNo, int ch23vBandNo, int ch36vBandNo,int ch89vBandNo)
        {
            if (argumentProvider == null || argumentProvider.DataProvider == null)
            {
                return null;
            }
            if (_sinfo == null || _perArgumentProvider != argumentProvider || per18vBandNo != ch18vBandNo || per23vBandNo != ch23vBandNo || per36vBandNo != ch36vBandNo || per89vBandNo != ch89vBandNo)
            {
                per18vBandNo = ch18vBandNo;
                per23vBandNo = ch23vBandNo;
                per36vBandNo = ch36vBandNo;
                per89vBandNo = ch89vBandNo;
                _perArgumentProvider = argumentProvider;
                _sinfo = new SnwDisplayInfo(argumentProvider);
                _sinfo.UpdateDisplayInfoMS(ch18vBandNo, ch23vBandNo, ch36vBandNo, ch89vBandNo);
                return _sinfo.DisplayInfoMS;
            }
            else
                return _sinfo.DisplayInfoMS;
        }
        //微波积雪判识
        public void UpdateDisplayInfoMS(int ch18v, int ch23v, int ch36v, int ch89v)
        {
            if (_argumentProvider.DataProvider == null)
                _snwFeatureCollectionMS = null;
            Dictionary<int, SnwFeatureMS> features = new Dictionary<int, SnwFeatureMS>();
            SnwFeatureMS tempSnw = null;
            RasterPixelsVisitor<Int16> rpVisitor = new RasterPixelsVisitor<Int16>(_argumentProvider);

            Rectangle rect = new Rectangle(0, 0, _argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height);
            int[] aoi = null;//积雪信息提示要求不使用AOI。
            rpVisitor.VisitPixel(rect, aoi,
            new int[] { ch18v, ch23v, ch36v,ch89v },
                (index, values) =>
                {
                    tempSnw = new SnwFeatureMS();
                    tempSnw.si1 = (values[1] - values[3])*0.01;
                    tempSnw.si2 = (values[0] - values[2]) * 0.01;
                    tempSnw.ch23v =( values[1] * 0.01) + 327.68;
                    tempSnw.si22 = (values[0] - values[2]) * 0.01;
                    tempSnw.si1si2 = (values[1] - values[3] - values[0] + values[2]) * 0.01;
                    tempSnw.si1si22 = (values[1] - values[3] - values[0] + values[2]) * 0.01;
                    features.Add(index, tempSnw);
                    
                }
            );
            _snwFeatureCollectionMS = new SnwFeatureCollectionMS("微波积雪辅助信息", features); ;
        }
        public SnwDisplayInfo(IArgumentProvider argumentProvider)
        {
            _argumentProvider = argumentProvider;
        }
        private SnwFeatureCollectionMS _snwFeatureCollectionMS = null;

        private SnwFeatureCollection _snwFeatureCollection = null;

        public SnwFeatureCollectionMS DisplayInfoMS
        {
            get
            {
                return _snwFeatureCollectionMS;
            }
        }

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
