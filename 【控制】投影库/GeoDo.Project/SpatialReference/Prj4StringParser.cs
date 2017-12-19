using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace GeoDo.Project
{
    public class Prj4StringParser
    {
        private IPrjStdsMapTableParser _prjStdsMapTableParser = null;

        public Prj4StringParser()
        {
            _prjStdsMapTableParser = new PrjStdsMapTableParser();
        }

        public ISpatialReference Parse(string proj4)
        {
            if (string.IsNullOrEmpty(proj4))
                return null;
            proj4 = proj4.ToLower();
            string[] parts = proj4.Split('+');
            if (parts.Length < 2)
                return null;
            Dictionary<string, string> nameValuePairs = new Dictionary<string, string>();
            foreach (string part in parts)
            {
                string[] it = part.Trim().Split('=');
                if (it.Length < 2)
                    continue;
                nameValuePairs.Add(it[0], it[1]);
            }
            if (!nameValuePairs.Keys.Contains("proj"))
                return null;
            IGeographicCoordSystem geoCoordSystem = GetGeoCoordSystemByProj4(nameValuePairs);
            if (nameValuePairs.ContainsKey("a"))
                geoCoordSystem.Datum.Spheroid.SemimajorAxis = double.Parse(nameValuePairs["a"]);
            if (nameValuePairs.ContainsKey("b"))
                geoCoordSystem.Datum.Spheroid.SemiminorAxis = double.Parse(nameValuePairs["b"]);
            if (nameValuePairs.ContainsKey("f"))
                geoCoordSystem.Datum.Spheroid.InverseFlattening = 1 / double.Parse(nameValuePairs["f"]);
            IProjectionCoordSystem prjCoordSystem = GetPrjCoordSystemByProj4(nameValuePairs);
            ISpatialReference spatialReference = new SpatialReference(geoCoordSystem, prjCoordSystem);
            if(prjCoordSystem != null)
                spatialReference.Name = GetSpatialNameByPrjName(prjCoordSystem.Name.Name);
            return spatialReference;
        }

        /*Proj4字符串中没有定义空间参考的名称，不影响功能，可以不实现*/
        private string GetSpatialNameByPrjName(string name)
        {
            switch (name)
            {
                case "Mercator":
                    return "World_Mercator";
                case "Transverse Mercator":
                    return "Transverse Mercator";
                case "Lambert Conformal Conic":
                    return "Lambert_Conformal_Conic_2SP";
                case "Albers Conical Equal Area":
                    return "Albers";
                case "Lambert Azimuthal Equal Area":
                    return "Lambert Azimuthal Equal Area";
                case "Hammer":
                    return "World_Hammer_Aitoff";
                case "Polar Stereographic":
                    return "Polar_Stereographic";
                case "Lambert Azimuthal Equal Area (sphere)":
                    return "Lambert Azimuthal Equal Area (sphere)";
            }
            return null;
        }

        private IProjectionCoordSystem GetPrjCoordSystemByProj4(Dictionary<string, string> nameValuePairs)
        {
            string coordType = nameValuePairs["proj"];
            if (coordType == "latlong")
                return null;
            List<NameMapItem> args = _prjStdsMapTableParser.GetPrjParameterItems();
            if (args == null)
                return null;
            ProjectionCoordSystem prj = new ProjectionCoordSystem();
            prj.Name = _prjStdsMapTableParser.GetPrjNameItemByPrj4(coordType);
            List<NameValuePair> paras = new List<NameValuePair>();
            foreach (NameMapItem arg in args)
            {
                if (nameValuePairs.ContainsKey(arg.Proj4Name))
                    paras.Add(new NameValuePair(_prjStdsMapTableParser.GetPrjParamterItemByPrj4(arg.Proj4Name), double.Parse(nameValuePairs[arg.Proj4Name])));
            }
            prj.Parameters = paras.Count > 0 ? paras.ToArray() : null;
            return prj;
        }

        /*
            string Name { get;  }
            AngularUnit AngularUnit { get; }
            PrimeMeridian PrimeMeridian { get; }
            Datum Datum { get; }
            string proj4 = "+datum={0} +a={1} +b={2} +f={3} +nodefs";
         */
        private IGeographicCoordSystem GetGeoCoordSystemByProj4(Dictionary<string, string> nameValuePairs)
        {
            Datum datum = new Datum();
            string name = "wgs84";
            if (nameValuePairs.ContainsKey("datum"))
            {
                name = GetGeocoordsystemNameByProj4Name(nameValuePairs["datum"]);
                datum = GetDatum(nameValuePairs["datum"], nameValuePairs);
                datum.Name = GetDatumNameByProj4DatumName(nameValuePairs["datum"]);
                datum.Spheroid.Name = GetSpheroidNameByProj4Name(nameValuePairs["datum"]);// nameValuePairs["datum"].ToUpper();
            }

            string unitname = "Degree";
            if (nameValuePairs.ContainsKey("units"))
                unitname = nameValuePairs["units"];
            AngularUnit unit = new AngularUnit(unitname, 0.0174532925199433d);
            string primeName = "Greenwich";
            PrimeMeridian primeMeridian = new PrimeMeridian(primeName, 0d);
            return new GeographicCoordSystem(name, unit, primeMeridian, datum);
        }

        /*Proj4字符串中没有定义椭球体的名称，不影响功能，可以不实现*/
        private string GetSpheroidNameByProj4Name(string proj4DatumName)
        {
            switch (proj4DatumName)
            {
                case "wgs84":
                    return "WGS_1984";
            }
            return null;
        }

        /*Proj4字符串中没有定义地理坐标系统的名称，不影响功能，可以不实现*/
        private string GetGeocoordsystemNameByProj4Name(string proj4DatumName)
        {
            switch (proj4DatumName)
            {
                case "wgs84":
                    return "GCS_WGS_1984";
            }
            return null;
        }

        private Datum GetDatum(string name, Dictionary<string, string> nameValuePairs)
        {
            Spheroid spheroid = null;
            if (nameValuePairs.ContainsKey("a") && nameValuePairs.ContainsKey("b") && nameValuePairs.ContainsKey("f"))
            {
                spheroid = new Spheroid();
                spheroid.SemimajorAxis = double.Parse(nameValuePairs["a"]);
                spheroid.SemiminorAxis = double.Parse(nameValuePairs["b"]);
                spheroid.InverseFlattening = 1 / double.Parse(nameValuePairs["f"]);
            }
            else
            {
                spheroid = GetSheroidByDatum(name);
            }
            return new Datum(name, spheroid);
        }

        private Spheroid GetSheroidByDatum(string name)
        {
            if (name == "wgs84")
                return new Spheroid();
            throw new NotImplementedException();
        }

        private string GetDatumNameByProj4DatumName(string proj4DatumName)
        {
            Dictionary<string, string> datumNames = _prjStdsMapTableParser.GetDatumsItems();
            foreach (string name in datumNames.Keys)
                if (datumNames[name].ToLower() == proj4DatumName.ToLower())
                    return name;
            return null;
        }
    }
}
