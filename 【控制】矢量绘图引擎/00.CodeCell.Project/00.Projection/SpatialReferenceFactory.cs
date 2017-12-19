using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Core
{
    public static class SpatialReferenceFactory
    {
        public static ISpatialReference GetSpatialReferenceByPrjFile(string prjFile)
        {
            try
            {
                if (string.IsNullOrEmpty(prjFile) || !File.Exists(prjFile))
                    return null;
                return GetSpatialReferenceByWKT(File.ReadAllText(prjFile), enumWKTSource.EsriPrjFile);
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
                return null;
            }
        }

        public static ISpatialReference GetSpatialReferenceByWKT(string wkt,enumWKTSource source)
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
                try
                {
                    sref = p.Parse(wkt);
                    if (sref != null)
                    {
                        TrySetCoordinateDomain(sref);
                        return sref;
                    }
                }
                catch (Exception ex)
                {
                    Log.WriterException(ex);
                    throw;
                }
            }
            return null;
        }

        private static void TrySetCoordinateDomain(ISpatialReference sref)
        {
            if (sref.ProjectionCoordSystem == null )
                return;
            string prjName = null;
            if(sref.ProjectionCoordSystem.Name != null && sref.ProjectionCoordSystem.Name.EsriName != null)
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
