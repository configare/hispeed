using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace GeoDo.Project
{
    public static class SpatialReferenceFactory
    {
        private static ISpatialReferenceStringParser PrjFileParser = null;

        static SpatialReferenceFactory()
        {
            PrjFileParser = new WktProjectionCommonParser(enumWKTSource.EsriPrjFile);
        }

        public static ISpatialReference GetSpatialReferenceByProj4String(string proj4)
        {
            Prj4StringParser p = new Prj4StringParser();
            return p.Parse(proj4);
        }

        public static ISpatialReference GetSpatialReferenceByPrjFile(string prjFile)
        {
            if (string.IsNullOrEmpty(prjFile) || !File.Exists(prjFile))
                return null;
            return PrjFileParser.Parse(File.ReadAllText(prjFile, Encoding.Default));
        }

        public static ISpatialReference GetSpatialReferenceByWKT(string wkt, enumWKTSource source)
        {
            if (string.IsNullOrEmpty(wkt))
                return null;
            ISpatialReferenceStringParser[] ps = new ISpatialReferenceStringParser[] 
                                                                  {
                                                                      new WktProjectionCommonParser(source)
                                                                  };
            foreach (ISpatialReferenceStringParser p in ps)
            {
                ISpatialReference sref = null;
                sref = p.Parse(wkt);
                if (sref != null)
                {
                    TrySetCoordinateDomain(sref);
                    return sref;
                }
            }
            return null;
        }

        private static void TrySetCoordinateDomain(ISpatialReference sref)
        {
            if (sref.ProjectionCoordSystem == null)
                return;
            string prjName = null;
            if (sref.ProjectionCoordSystem.Name != null && sref.ProjectionCoordSystem.Name.EsriName != null)
                prjName = sref.ProjectionCoordSystem.Name.EsriName.ToUpper();
            if (prjName == null)
                return;
            switch (prjName)
            {
                case "MERCATOR":
                    sref.CoordinateDomain = new CoordinateDomain(-85.05d, 85.05d, null);
                    break;
            }
        }
    }
}
