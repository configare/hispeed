using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// 产品定义段中层定义类
    /// </summary>
    public class GribPDSLevel
    {
        /// <summary> Index number from table 3 - can be used for comparison even if the
        /// description of the level changes.
        /// </summary>
        private int _index;
        private string _name = null;// 垂直方向坐标名称
        /// <summary> Value of PDS octet10 if separate from 11, otherwise value from octet10&11.</summary>
        private float _value1;
        /// <summary> Value of PDS octet11.</summary>
        private float _value2;

        /// <summary> 
        /// Index number from table 3 - can be used for comparison even 
        /// if the description of the level changes.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// 层名称
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>获取层的第一个值</summary>
        public float Value1
        {
            get { return _value1; }
        }

        /// <summary>获取层的第二个值</summary>
        public float Value2
        {
            get { return _value2; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pds10">层的代码的第一个部分</param>
        /// <param name="pds11">层的代码的第二个部分</param>
        /// <param name="pds12">层的代码的第三个部分</param>
        public GribPDSLevel(int pds10, int pds11, int pds12)
        {
            int pds1112 = pds11 << 8 | pds12;
            _index = pds10;
            switch (_index)
            {
                case 0:
                    _name = "reserved";
                    break;

                case 1:
                    _name = "surface";
                    break;

                case 2:
                    _name = "cloud base level";
                    break;

                case 3:
                    _name = "cloud top level";
                    break;

                case 4:
                    _name = "0 degree isotherm level";
                    break;

                case 5:
                    _name = "condensation level";
                    break;

                case 6:
                    _name = "maximum wind level";
                    break;

                case 7:
                    _name = "tropopause level";
                    break;

                case 8:
                    _name = "nominal atmosphere top";
                    break;

                case 9:
                    _name = "sea bottom";
                    break;

                case 20:
                    _name = "Isothermal level";
                    _value1 = pds1112;
                    break;

                case 100:
                    _name = "isobaric";
                    _value1 = pds1112;
                    break;

                case 101:
                    _name = "layer between two isobaric levels";
                    _value1 = pds11 * 10; // 有kPa转换为hPa
                    _value2 = pds12 * 10;
                    break;

                case 102:
                    _name = "mean sea level";
                    break;

                case 103:
                    _name = "Altitude above MSL";
                    _value1 = pds1112;
                    break;

                case 104:
                    _name = "Layer between two altitudes above MSL";
                    _value1 = (pds11 * 100); // 由hm转换为m
                    _value2 = (pds12 * 100);
                    break;

                case 105:
                    _name = "fixed height above ground";
                    _value1 = pds1112;
                    break;

                case 106:
                    _name = "layer between two height levels";
                    _value1 = (pds11 * 100); // 由hm转换为m
                    _value2 = (pds12 * 100);
                    break;

                case 107:
                    _name = "Sigma level";
                    _value1 = pds1112;
                    break;

                case 108:
                    _name = "Layer between two sigma layers";
                    _value1 = pds11;
                    _value2 = pds12;
                    break;

                case 109:
                    _name = "hybrid level";
                    _value1 = pds1112;
                    break;

                case 110:
                    _name = "Layer between two hybrid levels";
                    _value1 = pds11;
                    _value2 = pds12;
                    break;

                case 111:
                    _name = "Depth below land surface";
                    _value1 = pds1112;
                    break;

                case 112:
                    _name = "Layer between two levels below land surface";
                    _value1 = pds11;
                    _value2 = pds12;
                    break;

                case 113:
                    _name = "Isentropic theta level";
                    _value1 = pds1112;
                    break;

                case 114:
                    _name = "Layer between two isentropic layers";
                    _value1 = pds11;
                    _value2 = pds12;
                    break;

                case 115:
                    _name = "level at specified pressure difference from ground to level";
                    _value1 = pds1112;
                    break;

                case 116:
                    _name = "Layer between pressure differences from ground to levels";
                    _value1 = pds11;
                    _value2 = pds12;
                    break;

                case 117:
                    _name = "potential vorticity(pv) surface";
                    _value1 = pds1112;
                    break;

                case 119:
                    _name = "Eta level";
                    _value1 = pds1112;
                    break;

                case 120:
                    _name = "layer between two Eta levels";
                    _value1 = pds11;
                    _value2 = pds12;
                    break;

                case 121:
                    _name = "layer between two isobaric surfaces";
                    _value1 = pds11;
                    _value2 = pds12;
                    break;

                case 125:
                    _name = "Height above ground (high precision)";
                    _value1 = pds1112;
                    break;

                case 126:
                    _name = "isobaric level";
                    _value1 = pds1112;
                    break;

                case 128:
                    _name = "layer between two sigma levels";
                    _value1 = pds11;
                    _value2 = pds12;
                    break;

                case 141:
                    _name = "layer between two isobaric surfaces";
                    _value1 = pds11 * 10; // 由kPa转换为hPa
                    _value2 = pds12;
                    break;

                case 160:
                    _name = "Depth below sea level";
                    _value1 = pds1112;
                    break;

                case 200:
                    _name = "entire atmosphere layer";
                    break;

                case 201:
                    _name = "entire ocean layer";
                    break;

                case 204:
                    _name = "Highest tropospheric freezing level";
                    break;

                case 206:
                    _name = "Grid scale cloud bottom level";
                    break;

                case 207:
                    _name = "Grid scale cloud top level";
                    break;

                case 209:
                    _name = "Boundary layer cloud bottom level";
                    break;

                case 210:
                    _name = "Boundary layer cloud top level";
                    break;

                case 211:
                    _name = "Boundary layer cloud layer";
                    break;

                case 212:
                    _name = "Low cloud bottom level";
                    break;

                case 213:
                    _name = "Low cloud top level";
                    break;

                case 214:
                    _name = "Low Cloud Layer";
                    break;

                case 222:
                    _name = "Middle cloud bottom level";
                    break;

                case 223:
                    _name = "Middle cloud top level";
                    break;

                case 224:
                    _name = "Middle Cloud Layer";
                    break;

                case 232:
                    _name = "High cloud bottom level";
                    break;

                case 233:
                    _name = "High cloud top level";
                    break;

                case 234:
                    _name = "High Cloud Layer";
                    break;

                case 235:
                    _name = "Ocean Isotherm Level";
                    break;

                case 236:
                    _name = "Layer between two depths below ocean surface";
                    _value1 = pds11;
                    _value2 = pds12;
                    break;

                case 237:
                    _name = "Bottom of Ocean Mixed Layer";
                    break;

                case 238:
                    _name = "Bottom of Ocean Isothermal Layer";
                    break;

                case 242:
                    _name = "Convective cloud bottom level";
                    break;

                case 243:
                    _name = "Convective cloud top level";
                    break;

                case 244:
                    _name = "Convective cloud layer";
                    break;

                case 245:
                    _name = "Lowest level of the wet bulb zero";
                    break;

                case 246:
                    _name = "Maximum equivalent potential temperature level";
                    break;

                case 247:
                    _name = "Equilibrium level";
                    break;

                case 248:
                    _name = "Shallow convective cloud bottom level";
                    break;

                case 249:
                    _name = "Shallow convective cloud top level";
                    break;

                case 251:
                    _name = "Deep convective cloud bottom level";
                    break;

                case 252:
                    _name = "Deep convective cloud top level";
                    break;

                default:
                    _name = "undefined level";
                    break;
            }
        }

        /// <summary>
        /// 垂直方向坐标类型
        /// </summary>
        /// <param name="id">序号</param>
        /// <returns>层的描述信息</returns>
        public static string GetLevelDescription(int id)
        {
            switch (id)
            {
                case 0: return "Reserved";

                case 1: return "Ground or water surface";

                case 2: return "Cloud base level";

                case 3: return "Level of cloud tops";

                case 4: return "Level of 0o C isotherm";

                case 5: return "Level of adiabatic condensation lifted from the surface";

                case 6: return "Maximum wind level";

                case 7: return "Tropopause";

                case 8: return "Nominal top of the atmosphere";

                case 9: return "Sea bottom";

                case 20: return "Isothermal level";

                case 100: return "Isobaric surface";

                case 101: return "Layer between 2 isobaric levels";

                case 102: return "Mean sea level";

                case 103: return "Altitude above mean sea level";

                case 104: return "Layer between 2 altitudes above msl";

                case 105: return "Specified height level above ground";

                case 106: return "Layer between 2 specified height level above ground";

                case 107: return "Sigma level";

                case 108: return "Layer between 2 sigma levels";

                case 109: return "Hybrid level";

                case 110: return "Layer between 2 hybrid levels";

                case 111: return "Depth below land surface";

                case 112: return "Layer between 2 depths below land surface";

                case 113: return "Isentropic theta level";

                case 114: return "Layer between 2 isentropic levels";

                case 115: return "Level at specified pressure difference from ground to level";

                case 116: return "Layer between 2 level at pressure difference from ground to level";

                case 117: return "Potential vorticity surface";

                case 119: return "Eta level";

                case 120: return "Layer between 2 Eta levels";

                case 121: return "Layer between 2 isobaric levels";

                case 125: return "Specified height level above ground";

                case 126: return "Isobaric level";

                case 128: return "Layer between 2 sigma levels (hi precision)";

                case 141: return "Layer between 2 isobaric surfaces";

                case 160: return "Depth below sea level";

                case 200: return "Entire atmosphere";

                case 201: return "Entire ocean";

                case 204: return "Highest tropospheric freezing level";

                case 206: return "Grid scale cloud bottom level";

                case 207: return "Grid scale cloud top level";

                case 209: return "Boundary layer cloud bottom level";

                case 210: return "Boundary layer cloud top level";

                case 211: return "Boundary layer cloud layer";

                case 212: return "Low cloud bottom level";

                case 213: return "Low cloud top level";

                case 214: return "Low Cloud Layer";

                case 222: return "Middle cloud bottom level";

                case 223: return "Middle cloud top level";

                case 224: return "Middle Cloud Layer";

                case 232: return "High cloud bottom level";

                case 233: return "High cloud top level";

                case 234: return "High Cloud Layer";

                case 242: return "Convective cloud bottom level";

                case 243: return "Convective cloud top level";

                case 244: return "Convective cloud layer";

                case 245: return "Lowest level of the wet bulb zero";

                case 246: return "Maximum equivalent potential temperature level";

                case 247: return "Equilibrium level";

                case 248: return "Shallow convective cloud bottom level";

                case 249: return "Shallow convective cloud top level";

                case 251: return "Deep convective cloud bottom level";

                case 252: return "Deep convective cloud top level";

                case 255: return "Missing";

                default: return "Unknown=" + id;
            }
        }

        /// <summary>层名称简称</summary>
        /// <param name="id">序号</param>
        /// <returns>层的描述信息</returns>
        public static string GetShortName(int id)
        {
            switch (id)
            {
                case 1: return "surface";

                case 2: return "cloud_base";

                case 3: return "cloud_tops";

                case 4: return "zeroDegC_isotherm";

                case 5: return "adiabatic_condensation_lifted";

                case 6: return "maximum_wind";

                case 7: return "tropopause";

                case 8: return "atmosphere_top";

                case 9: return "sea_bottom";

                case 20: return "isotherm";

                case 100: return "isobaric";

                case 101: return "layer_between_two_isobaric";

                case 102: return "msl";

                case 103: return "altitude_above_msl";

                case 104: return "layer_between_two_altitudes_above_msl";

                case 105: return "height_above_ground";

                case 106: return "layer_between_two_heights_above_ground";

                case 107: return "sigma";

                case 108: return "layer_between_two_sigmas";

                case 109: return "hybrid";

                case 110: return "layer_between_two_hybrids";

                case 111: return "depth_below_surface";

                case 112: return "layer_between_two_depths_below_surface";

                case 113: return "isentrope";

                case 114: return "layer_between_two_isentrope";

                case 115: return "pressure_difference";

                case 116: return "layer_between_two_pressure_difference_from_ground";

                case 117: return "potential_vorticity_surface";

                case 119: return "eta";

                case 120: return "layer_between_two_eta";

                case 121: return "layer_between_two_isobaric_surfaces";

                case 125: return "height_above_ground";

                case 126: return "isobaric";

                case 128: return "layer_between_two_sigmas_hi";

                case 141: return "layer_between_two_isobaric_surfaces";

                case 160: return "depth_below_sea";

                case 200: return "entire_atmosphere";

                case 201: return "entire_ocean";

                case 204: return "highest_tropospheric_freezing";

                case 206: return "grid_scale_cloud bottom";

                case 207: return "grid_scale_cloud_top";

                case 209: return "boundary_layer_cloud_bottom";

                case 210: return "boundary_layer_cloud_top";

                case 211: return "boundary_layer_cloud_layer";

                case 212: return "low_cloud_bottom";

                case 213: return "low_cloud_top";

                case 214: return "low_cloud_layer";

                case 222: return "middle_cloud_bottom";

                case 223: return "middle_cloud_top";

                case 224: return "middle_cloud_layer";

                case 232: return "high_cloud_bottom";

                case 233: return "high_cloud_top";

                case 234: return "high_cloud_layer";

                case 242: return "convective_cloud_bottom";

                case 243: return "convective_cloud_top";

                case 244: return "convective_cloud_layer";

                case 245: return "lowest_level_of_wet_bulb_zero";

                case 246: return "maximum_equivalent_potential_temperature";

                case 247: return "equilibrium";

                case 248: return "shallow_convective_cloud_bottom";

                case 249: return "shallow_convective_cloud_top";

                case 251: return "deep_convective_cloud_bottom";

                case 252: return "deep_convective_cloud_top";

                case 255: return "";

                default: return "Unknown" + id;
            }
        }

        /// <summary>
        /// 获取垂直方向坐标单位类型
        /// </summary>
        /// <param name="id">单位序号</param>
        public static string GetUnits(int id)
        {
            switch (id)
            {
                case 20:
                case 113:
                case 114:
                    return "K";
                case 100:
                case 101:
                case 115:
                case 116:
                case 121:
                case 141:
                    return "hPa";
                case 103:
                case 104:
                case 105:
                case 106:
                case 160:
                    return "m";
                case 107:
                case 108:
                case 128:
                    return "sigma";
                case 111:
                case 112:
                case 125:
                    return "cm";
                case 117:
                    return "10-6Km2/kgs";
                case 126:
                    return "Pa";
            }
            return "";
        }
    }
}
