using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class ProjectionCoordSystem:IProjectionCoordSystem,ISpatialRefFormat
    {
        private string _namedes = null;
        private NameMapItem _name = null;
        private NameValuePair[] _parameters = null;
        private AngularUnit _unit = new AngularUnit("Meter",1d);

        public ProjectionCoordSystem() // Graphics Latitude/Longitude
        { 
            //
        }

        public ProjectionCoordSystem(NameMapItem name, NameValuePair[] parameters, AngularUnit unit)
        {
            _name = name;
            _parameters = parameters;
            _unit = unit;
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
        }

        [DisplayName("参数")]
        public NameValuePair[] Parameters
        {
            get { return _parameters; }
        }

        [DisplayName("单位")]
        public AngularUnit Unit
        {
            get { return _unit; }
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
                if (v > double.Epsilon)
                    return false;
            }
            return true;
        }

        public string ToProj4String()
        {
            string proj4 = "+proj="+_name.Proj4Name+" +";
            foreach (NameValuePair nvp in _parameters)
                proj4 += (nvp.Name.Proj4Name + "=" + nvp.Value.ToString() + " +");
            if (proj4.EndsWith("+"))
                proj4 = proj4.Substring(0, proj4.Length - 1);
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
            return "PROJECTION[\"" + _name.EsriName + "\"]" + parameters + _unit.ToString();
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
    }
}
