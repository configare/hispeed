using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;

namespace GeoDo.RSS.Core.DF
{
    public class SpatialReferenceBuilder : IDisposable
    {
        private IPrjStdsMapTableParser _stdsMapTableParser = null;

        public SpatialReferenceBuilder()
        {
            _stdsMapTableParser = new PrjStdsMapTableParser();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prjType">气象局定义的投影类型</param>
        /// <param name="standard_parallel_1"></param>
        /// <param name="standard_parallel_2"></param>
        /// <param name="central_meridian"></param>
        /// <returns></returns>
        public ISpatialReference GetSpatialRef(int prjType, double standard_parallel_1, double standard_parallel_2, double central_meridian)
        {
            if (prjType == 0 || prjType == 1 || prjType == -1)
                return new SpatialReference(new GeographicCoordSystem());
            IGeographicCoordSystem geoCoordSystem = new GeographicCoordSystem();
            NameValuePair[] paras = new NameValuePair[3];
            paras[0] = new NameValuePair(GetParaName("sp1"), standard_parallel_1);
            paras[1] = new NameValuePair(GetParaName("sp2"), standard_parallel_2);
            paras[2] = new NameValuePair(GetParaName("lon0"), central_meridian);
            NameMapItem prjName = GetProjectionName(prjType);
            IProjectionCoordSystem prjCoordSystem = new ProjectionCoordSystem(prjName, paras, new AngularUnit("Degree", 0d));
            return new SpatialReference(geoCoordSystem, prjCoordSystem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enviPrjId">MAS二期九度公司扩展的投影类型，采用ENVI的投影ID</param>
        /// <param name="standard_parallel_1"></param>
        /// <param name="standard_parallel_2"></param>
        /// <param name="latitude_of_origin"></param>
        /// <param name="central_meridian"></param>
        /// <param name="false_easting"></param>
        /// <param name="false_northing"></param>
        /// <param name="scale_factor"></param>
        /// <returns></returns>
        public ISpatialReference GetSpatialRef(int enviPrjId, double standard_parallel_1, double standard_parallel_2,
            double latitude_of_origin, double central_meridian,
            double false_easting, double false_northing, double scale_factor)
        {
            IGeographicCoordSystem geoCoordSystem = new GeographicCoordSystem();
            if (enviPrjId == 0 || enviPrjId == 1)
                return new SpatialReference(geoCoordSystem, null);
            NameValuePair[] paras = new NameValuePair[7];
            paras[0] = new NameValuePair(GetParaName("sp1"), standard_parallel_1);
            paras[1] = new NameValuePair(GetParaName("sp2"), standard_parallel_2);
            paras[2] = new NameValuePair(GetParaName("lat0"), latitude_of_origin);
            paras[3] = new NameValuePair(GetParaName("lon0"), central_meridian);
            paras[4] = new NameValuePair(GetParaName("x0"), false_easting);
            paras[5] = new NameValuePair(GetParaName("y0"), false_northing);
            paras[6] = new NameValuePair(GetParaName("k0"), scale_factor);
            NameMapItem prjName = _stdsMapTableParser.GetPrjNameItemByEnviName(enviPrjId.ToString());
            IProjectionCoordSystem prjCoordSystem = new ProjectionCoordSystem(prjName, paras, new AngularUnit("Degree", 0d));
            return new SpatialReference(geoCoordSystem, prjCoordSystem);
        }

        private NameMapItem GetParaName(string enviParaName)
        {
            List<NameMapItem> items = _stdsMapTableParser.GetPrjParameterItems();
            foreach (NameMapItem it in items)
                if (it.ENVIName == enviParaName)
                    return it;
            return null;
        }

        /*
         * LDF Projection Type:
         * 0: 不投影, 1: 等角投影 2: 麦卡托投影, 3: 兰布托投影  4: 极射赤面投影, 5: 艾尔伯斯投影 
         */
        private NameMapItem GetProjectionName(int prjType)
        {
            switch (prjType)
            {
                case 0:
                case 1:
                    return null;
                case 2:
                    return _stdsMapTableParser.GetPrjNameItemByWktName("Mercator");
                case 3:
                    return _stdsMapTableParser.GetPrjNameItemByWktName("Lambert_Conformal_Conic_2SP");
                case 4:
                    return _stdsMapTableParser.GetPrjNameItemByWktName("Polar_Stereographic");
                case 5:
                    return _stdsMapTableParser.GetPrjNameItemByWktName("Albers");
                default:
                    return null;//throw new NotSupportedException("未知的LDF投影标识\"" + prjType.ToString() + "\"");
            }
        }

        public void Dispose()
        {
            if (_stdsMapTableParser != null)
                _stdsMapTableParser.Dispose();
        }
    }
}
