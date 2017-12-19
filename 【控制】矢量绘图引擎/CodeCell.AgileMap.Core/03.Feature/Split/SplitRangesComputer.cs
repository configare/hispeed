using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal class SplitRangesComputer
    {
        internal struct Range
        {
            public float MinValue;
            public float MaxValue;

            public Range(float minValue, float maxValue)
            {
                MinValue = minValue;
                MaxValue = maxValue;
            }
        }

        private List<Range> _validLonRanges = new List<Range>();
        private Range _validLatRange = new Range();
        private ISpatialReference _spatialRef = null;

        public SplitRangesComputer(ISpatialReference spatialRef)
        {
            _spatialRef = spatialRef;
        }

        public bool ComputeValidGeoRange(out double centerLont, out double minLat, out double maxLat,out double minLon,out double maxLon)
        {
            centerLont = minLat = maxLat = minLon = maxLon = 0;
            if (_spatialRef == null || _spatialRef.ProjectionCoordSystem == null)
                return false;
            NameValuePair lat0 = _spatialRef.ProjectionCoordSystem.GetParaByName("lat0");
            NameValuePair lon0 = _spatialRef.ProjectionCoordSystem.GetParaByName("lon0");
            NameValuePair sp1 = _spatialRef.ProjectionCoordSystem.GetParaByName("sp1");
            NameValuePair sp2 = _spatialRef.ProjectionCoordSystem.GetParaByName("sp2");
            centerLont = (double)lon0.Value - 180;
            minLat = -85;
            maxLat = 85;
            minLon = -179.9999;
            maxLon = 179.9999;
            string prjName = _spatialRef.ProjectionCoordSystem.Name.EsriName;
            switch (prjName)
            {
                case "Transverse_Mercator":
                    minLat = -85;
                    maxLat = 85;
                    minLon = lon0.Value - 60;
                    maxLon = lon0.Value + 60;
                    break;
            }
            return true;
        }

        public bool ComputeValidGeoRange(out Range[] lonRanges,out Range latRange)
        {
            float minLon = -180, maxLon = 180;
            _validLonRanges.Clear();
            //Geographic Lon/Lat //GLL
            if (_spatialRef == null || _spatialRef.ProjectionCoordSystem == null)
            {
                Range lonRange = new Range(-180, 180);
                _validLatRange.MinValue = -90;
                _validLatRange.MaxValue = 90;
                _validLonRanges.Add(lonRange);
                lonRanges = _validLonRanges.ToArray();
                latRange = _validLatRange;
                return false;
            }
            NameValuePair lat0 = _spatialRef.ProjectionCoordSystem.GetParaByName("lat0");
            NameValuePair lon0 = _spatialRef.ProjectionCoordSystem.GetParaByName("lon0");
            NameValuePair sp1 = _spatialRef.ProjectionCoordSystem.GetParaByName("sp1");
            NameValuePair sp2 = _spatialRef.ProjectionCoordSystem.GetParaByName("sp2");
            string prjName = _spatialRef.ProjectionCoordSystem.Name.EsriName;
            SetLonRangesAndCorrectEndpoint(prjName, (float)lat0.Value, (float)lon0.Value, out minLon, out maxLon);
            SetLatRangesAndCorrectEndpoint(prjName, lat0, sp1, sp2);
            lonRanges = _validLonRanges.ToArray();
            latRange = _validLatRange;
            return true;
        }

        private void SetLatRangesAndCorrectEndpoint(string prjName, NameValuePair lat0, NameValuePair sp1, NameValuePair sp2)
        {
            if (prjName == "Lambert_Azimuthal_Equal_Area")
            {
                if (lat0.Value == 0)//赤道
                {
                    _validLatRange.MinValue = -90f;
                    _validLatRange.MaxValue = 90f;
                }
                else if (lat0.Value == 90)  //北极
                {
                    _validLatRange.MinValue = 0;
                    _validLatRange.MaxValue = 90f;
                }
                else if (lat0.Value == -90)//南极
                {
                    _validLatRange.MinValue = -90;
                    _validLatRange.MaxValue = 0f;
                }
            }
            else
            {
                //if (sp1 != null && sp2 != null && !(Math.Abs(sp1.Value) < float.Epsilon && Math.Abs(sp2.Value) < float.Epsilon))
                //{
                //    _validLatRange.MinValue =(float)sp1.Value;
                //    _validLatRange.MaxValue =(float)sp2.Value;
                //}
                //else
                //{
                _validLatRange.MinValue = -85f;
                _validLatRange.MaxValue = 85;
                //}
            }
            _validLatRange.MinValue = Math.Max(_validLatRange.MinValue, -90);
            _validLatRange.MaxValue = Math.Min(_validLatRange.MaxValue, 90);
        }

        private void SetLonRangesAndCorrectEndpoint(string prjName, float lat0, float lon0, out float min, out float max)
        {
            if (prjName == null)
                prjName = string.Empty;
            if (prjName == "Transverse_Mercator")
            {
                _validLonRanges.Add(new Range(lon0 - 60, lon0 + 60));
            }
            else if (prjName == "Lambert_Azimuthal_Equal_Area")
            {
                if (lat0 == 0)
                {
                    _validLonRanges.Add(new Range(lon0 - 90, lon0 + 90));
                }
            }
            else if (prjName == "Mercator_2SP" ||
                prjName == "Albers" ||
                prjName == "Lambert_Conformal_Conic" ||
                prjName == "Hammer-Aitoff (world)")
            {
                if (lon0 > 0)
                {
                    float l1 = (int)(lon0 - 180d);
                    float l1end = 180;
                    float l2 = -180;
                    float l2end = l1;
                    _validLonRanges.Add(new Range(l1, l1end));
                    _validLonRanges.Add(new Range(l2, l2end));
                }
                else if (lon0 < 0)
                {
                    float l1 = -180;
                    float l1end = 180 + lon0;
                    float l2 = l1end;
                    float l2end = 180;
                    _validLonRanges.Add(new Range(l1, l1end));
                    _validLonRanges.Add(new Range(l2, l2end));
                }
                else
                {
                    _validLonRanges.Add(new Range(-180, 180));
                }
            }
            else
            {
                _validLonRanges.Add(new Range(-180, 180));
            }
            //
            min = float.MaxValue;
            max = float.MinValue;
            for (int i = 0; i < _validLonRanges.Count; i++)
            {
                min = Math.Max(_validLonRanges[i].MinValue, -180);
                max = Math.Min(_validLonRanges[i].MaxValue, 180);
                _validLonRanges[i] = new Range(min, max);
            }
            min = float.MaxValue;
            max = float.MinValue;
            foreach (Range r in _validLonRanges)
            {
                if (r.MinValue < min)
                    min = r.MinValue;
                if (r.MaxValue > max)
                    max = r.MaxValue;
            }
        }
    }
}
