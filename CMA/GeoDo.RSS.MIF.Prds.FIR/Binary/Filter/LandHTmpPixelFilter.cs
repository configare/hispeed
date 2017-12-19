using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    /// <summary>
    /// 陆地高温点判识
    /// eg:中红外温度 > 315
    /// </summary>
    internal class LandHTmpPixelFilter : CandidatePixelFilter
    {
        private float _minTemperature;
        private int _bandNo;
        private float _bandZoom;
        private IArgumentProvider _argProvider;

        public LandHTmpPixelFilter(int bandNo, float bandZoom, float minTemperature, IArgumentProvider argProvider)
        {
            _bandNo = bandNo;
            _bandZoom = bandZoom;
            _minTemperature = minTemperature;
            _argProvider = argProvider;
        }

        protected override int[] DoFilter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi)
        {
            return DoFilter(dataProvider, aoiRect, aoi, null);
        }

        protected override int[] DoFilter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi, byte[] assistInfo)
        {
            if (aoi == null)
                return null;
            List<int> retAOI = new List<int>(aoi.Length);
            switch (dataProvider.DataType)
            {
                case enumDataType.UInt16:
                    using (IRasterPixelsVisitor<UInt16> vistor = new RasterPixelsVisitor<UInt16>(_argProvider))
                    {
                        vistor.VisitPixel(aoiRect, aoi, new int[] { _bandNo }, (idx, values) =>
                        {
                            if (values[0] / _bandZoom > _minTemperature)
                            {
                                retAOI.Add(idx);
                                if (assistInfo != null)
                                    assistInfo[idx] = 1;
                            }
                        });
                    }
                    break;
                case enumDataType.Int16:
                    using (IRasterPixelsVisitor<Int16> vistor = new RasterPixelsVisitor<Int16>(_argProvider))
                    {
                        vistor.VisitPixel(aoiRect, aoi, new int[] { _bandNo }, (idx, values) =>
                        {
                            if (values[0] / _bandZoom > _minTemperature)
                            {
                                retAOI.Add(idx);
                                if (assistInfo != null)
                                    assistInfo[idx] = 1;
                            }
                        });
                    }
                    break;
                case enumDataType.Byte:
                    using (IRasterPixelsVisitor<byte> vistor = new RasterPixelsVisitor<byte>(_argProvider))
                    {
                        vistor.VisitPixel(aoiRect, aoi, new int[] { _bandNo }, (idx, values) =>
                        {
                            if (values[0] / _bandZoom > _minTemperature)
                            {
                                retAOI.Add(idx);
                                if (assistInfo != null)
                                    assistInfo[idx] = 1;
                            }
                        });
                    }
                    break;
                case enumDataType.Int32:
                    using (IRasterPixelsVisitor<Int32> vistor = new RasterPixelsVisitor<Int32>(_argProvider))
                    {
                        vistor.VisitPixel(aoiRect, aoi, new int[] { _bandNo }, (idx, values) =>
                        {
                            if (values[0] / _bandZoom > _minTemperature)
                            {
                                retAOI.Add(idx);
                                if (assistInfo != null)
                                    assistInfo[idx] = 1;
                            }
                        });
                    }
                    break;
                case enumDataType.UInt32:
                    using (IRasterPixelsVisitor<UInt32> vistor = new RasterPixelsVisitor<UInt32>(_argProvider))
                    {
                        vistor.VisitPixel(aoiRect, aoi, new int[] { _bandNo }, (idx, values) =>
                        {
                            if (values[0] / _bandZoom > _minTemperature)
                            {
                                retAOI.Add(idx);
                                if (assistInfo != null)
                                    assistInfo[idx] = 1;
                            }
                        });
                    }
                    break;
                case enumDataType.Int64:
                    using (IRasterPixelsVisitor<Int64> vistor = new RasterPixelsVisitor<Int64>(_argProvider))
                    {
                        vistor.VisitPixel(aoiRect, aoi, new int[] { _bandNo }, (idx, values) =>
                        {
                            if (values[0] / _bandZoom > _minTemperature)
                            {
                                retAOI.Add(idx);
                                if (assistInfo != null)
                                    assistInfo[idx] = 1;
                            }
                        });
                    }
                    break;
                case enumDataType.UInt64:
                    using (IRasterPixelsVisitor<UInt64> vistor = new RasterPixelsVisitor<UInt64>(_argProvider))
                    {
                        vistor.VisitPixel(aoiRect, aoi, new int[] { _bandNo }, (idx, values) =>
                        {
                            if (values[0] / _bandZoom > _minTemperature)
                            {
                                retAOI.Add(idx);
                                if (assistInfo != null)
                                    assistInfo[idx] = 1;
                            }
                        });
                    }
                    break;
                case enumDataType.Float:
                    using (IRasterPixelsVisitor<float> vistor = new RasterPixelsVisitor<float>(_argProvider))
                    {
                        vistor.VisitPixel(aoiRect, aoi, new int[] { _bandNo }, (idx, values) =>
                        {
                            if (values[0] / _bandZoom > _minTemperature)
                            {
                                retAOI.Add(idx);
                                if (assistInfo != null)
                                    assistInfo[idx] = 1;
                            }
                        });
                    }
                    break;
                case enumDataType.Double:
                    using (IRasterPixelsVisitor<double> vistor = new RasterPixelsVisitor<double>(_argProvider))
                    {
                        vistor.VisitPixel(aoiRect, aoi, new int[] { _bandNo }, (idx, values) =>
                        {
                            if (values[0] / _bandZoom > _minTemperature)
                            {
                                retAOI.Add(idx);
                                if (assistInfo != null)
                                    assistInfo[idx] = 1;
                            }
                        });
                    }
                    break;
            }
            return retAOI.Count > 0 ? retAOI.ToArray() : null;
        }
    }
}
