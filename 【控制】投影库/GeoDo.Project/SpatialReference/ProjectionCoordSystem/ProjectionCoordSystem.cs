using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace GeoDo.Project
{
    /// <summary>
    /// 修改日志
    /// 修改日期：2014.2.12
    /// 修改人：罗战克
    /// 修改内容：
    /// 1、修复Bug，修复ToString()方法输出的WKT格式字符串中PROJECTION["Mercator"]段后面缺少一个","的问题
    /// 2、修复构造实例后，_namedes为null的问题
    /// </summary>
    [Serializable]
    public class ProjectionCoordSystem : IProjectionCoordSystem, ISpatialRefFormat,ICloneable
    {
        private string _namedes = "";
        private NameMapItem _name = null;
        private NameValuePair[] _parameters = null;
        private AngularUnit _unit = new AngularUnit("Meter", 1d);
        private const double MINI_DOUBLE = 0.00000000001;   //比较时参照的最小浮点数


        public ProjectionCoordSystem() // Graphics Latitude/Longitude
        {
            //
        }

        public ProjectionCoordSystem(NameMapItem name, NameValuePair[] parameters, AngularUnit unit)
        {
            _name = name;
            //if(name!=null)
            //    _namedes = name.EsriName;
            _parameters = parameters;
            _unit = unit;
        }

        public NameValuePair GetParaByName(string enviParaName)
        {
            if (string.IsNullOrEmpty(enviParaName) || _parameters == null || _parameters.Length == 0)
                return null;
            foreach (NameValuePair p in _parameters)
                if (p.Name.ENVIName == enviParaName)
                    return p;
            return null;
        }

        public int ToOracleSpatialSRID()
        {
            return int.MinValue;
        }

        #region IProjectionCoordSystem Members

        [DisplayName("描述")]
        public string NameDes
        {
            get { return _namedes; }
            set { _namedes = value; }
        }

        [DisplayName("名称")]
        public NameMapItem Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DisplayName("参数")]
        public NameValuePair[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        [DisplayName("单位")]
        public AngularUnit Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        public bool IsSame(IProjectionCoordSystem prjCoordSystem)
        {
            if (prjCoordSystem == null)
                return false;
            if (_name.Name.ToUpper() != prjCoordSystem.Name.Name.ToUpper())
                return false;
            for (int i = 0; i < _parameters.Length; i++)
            {
                double v = Math.Abs(_parameters[i].Value - prjCoordSystem.Parameters[i].Value);
                if (v > MINI_DOUBLE)
                    return false;
            }
            return true;
        }

        public string ToProj4String()
        {
            string proj4 = "+proj=" + _name.Proj4Name + " +";
            foreach (NameValuePair nvp in _parameters)
            {
                if (nvp.Name.Proj4Name == "k_0" && nvp.Value == 0d)
                    continue;
                proj4 += (nvp.Name.Proj4Name + "=" + nvp.Value.ToString() + " +");
            }
            if (proj4.EndsWith("+"))
                proj4 = proj4.Substring(0, proj4.Length - 1);
            switch (_name.Proj4Name)
            {
                case "hammer":
                case "aea":
                case "merc":
                case "stere":
                case "lcc":
                    proj4 = Regex.Replace(proj4,@"\+k_0=\d*\.*\d*"," ");//移除+k_0=
                    break;
                default:
                    break;
            }
            return proj4;
        }

        #endregion

        /*
         * PROJECTION["Mercator"],
           PARAMETER["False_Easting",0.0],
           PARAMETER["False_Northing",0.0],
           PARAMETER["Central_Meridian",0.0],
           PARAMETER["Standard_Parallel_1",0.0],
           UNIT["Meter",1.0]
         */
        public override string ToString()
        {
            string parameters = null;
            foreach (NameValuePair p in _parameters)
            {
                parameters = parameters + "PARAMETER[\"" + p.Name.EsriName + "\"," + p.Value.ToString() + "],";
            }
            return "PROJECTION[\"" + _name.EsriName + "\"]" + "," + parameters + _unit.ToString();
        }

        #region ISpatialRefFormat Members

        public string FormatToString()
        {
            string parameters = null;
            foreach (NameValuePair p in _parameters)
            {
                parameters = parameters + "        PARAMETER[\"" + p.Name.EsriName + "\"," + p.Value.ToString() + "],\n";
            }
            return "        PROJECTION[\"" + _name.EsriName + "\"],\n" + parameters + "        " + _unit.ToString();
        }

        #endregion

        public object Clone()
        {
            ProjectionCoordSystem prj = new ProjectionCoordSystem();
            if (_name != null)
                prj._name = _name.Clone() as NameMapItem;
            if (_parameters != null && _parameters.Length > 0)
                prj._parameters = CloneParameters();
            if (_unit != null)
                prj._unit = _unit.Clone() as AngularUnit;
            return prj;
        }

        private NameValuePair[] CloneParameters()
        {
            NameValuePair[] vs= new NameValuePair[this._parameters.Length];
            for (int i = 0; i < vs.Length; i++)
                vs[i] = this._parameters[i].Clone() as NameValuePair;
            return vs;
        }
    }
}
