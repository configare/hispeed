using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal class WktProjectionCommonParser : WktParserBase
    {
        public WktProjectionCommonParser(enumWKTSource source)
            : base(source)
        {
        }

        /*
         * PROJECTION["Mercator"],
           PARAMETER["False_Easting",0.0],
           PARAMETER["False_Northing",0.0],
           PARAMETER["Central_Meridian",0.0],
           PARAMETER["Standard_Parallel_1",0.0],
           UNIT["Meter",1.0]
         */
        protected override IProjectionCoordSystem ParseProjectionCoordSystem(WktItem wktItem)
        {
            if (wktItem == null)//Is Graphics Coordinate System
                return null;
            string name = wktItem.Value.Split(',')[0].Replace("\"", string.Empty);
            NameMapItem prjName = GetPrjName(wktItem.GetWktItem("PROJECTION"));  //5ms
            if (prjName == null)
            {
                throw new Exception("投影坐标系统\"" + name + "\"没有在系统中预先配置。");
            }
            NameValuePair[] parameters = GetPrjParameters(wktItem.GetWktItems("PARAMETER"));
            AngularUnit unit = new AngularUnit(wktItem.GetWktItem("UNIT"));
            ProjectionCoordSystem prj = new ProjectionCoordSystem(prjName, parameters, unit);
            prj.NameDes = name;
            return prj;
        }

        private NameMapItem GetPrjName(WktItem wktItem)
        {
            using (OGCWkt2Proj4Mapper map = new OGCWkt2Proj4Mapper())
            {
                string prjName = wktItem.Value.Split(',')[0].Replace("\"", string.Empty);
                switch (_wktSource)
                {
                    case enumWKTSource.EsriPrjFile:
                        return map.GetPrjNameFromEsriName(prjName);
                    case enumWKTSource.GDAL:
                        return map.GetPrjNameFromWKTName(prjName);
                    default:
                        return map.GetPrjNameFromWKTName(prjName);
                }
            }
        }

        private NameValuePair[] GetPrjParameters(WktItem[] wktItems)
        {
            List<NameValuePair> values = new List<NameValuePair>();
            using (OGCWkt2Proj4Mapper map = new OGCWkt2Proj4Mapper())
            {
                for (int i = 0; i < wktItems.Length; i++)
                {
                    NameMapItem name = null;
                    WktItem witem = wktItems[i];
                    string[] vs = witem.Value.Split(',');
                    string pname = vs[0].Replace("\"", string.Empty);
                    double pvalue = double.Parse(vs[1]);
                    try
                    {
                        switch (_wktSource)
                        {
                            case enumWKTSource.EsriPrjFile:
                                name = map.GetParameterFromEsriName(pname);
                                break;
                            case enumWKTSource.GDAL:
                                name = map.GetParameterFromWKTName(pname);
                                break;
                            default:
                                name = map.GetParameterFromWKTName(pname);
                                break;
                        }
                        if (name != null)
                        {
                            values.Add(new NameValuePair(name, pvalue));
                        }
                    }
                    catch { continue; }
                }
            }
            return values.Count > 0 ? values.ToArray() : null;
        }
    }
}
