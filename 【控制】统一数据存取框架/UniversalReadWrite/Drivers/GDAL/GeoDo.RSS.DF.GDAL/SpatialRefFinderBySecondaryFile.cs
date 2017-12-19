using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;
using System.IO;
using OSGeo.GDAL;
using GeoDo.RSS.Core.DF;
using System.Xml.Linq;

namespace GeoDo.RSS.DF.GDAL
{
    public static class SpatialRefFinderBySecondaryFile
    {
        public static ISpatialReference TryGetSpatialRef(string fname,out double[] GTs)
        {
            GTs = null;
            if (string.IsNullOrEmpty(fname))
                return null;
            try
            {
                string extName = Path.GetExtension(fname.ToLower());
                switch (extName)
                {
                    case ".til":
                        return TryGetSpatialRefForTilFile(fname,out GTs);
                    default:
                        return TryGetSpatialRefForAuxFile(fname, out GTs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        private static ISpatialReference TryGetSpatialRefForAuxFile(string fname, out double[] GTs)
        {
            ISpatialReference spatial = null;
            GTs = null;
            string wdFile = WorldFile.GetWorldFilenameByRasterFilename(fname);
            if (File.Exists(wdFile))
            {
                WorldFile wf = new WorldFile(wdFile);
                GTs = new double[] { wf.MinX, wf.XResolution, 0, wf.MaxY, 0, wf.YResolution };
            }
            string auxFile = fname + ".aux.xml";
            if (File.Exists(auxFile))
            {
                XElement ele = XElement.Load(auxFile);
                XElement srs = ele.Element("SRS");
                if (srs != null)
                {
                    string wkt = srs.Value;
                    spatial = SpatialReference.FromWkt(wkt, enumWKTSource.EsriPrjFile);
                }
            }
            return spatial;
        }

        private static ISpatialReference TryGetSpatialRefForTilFile(string fname,out double[] GTs)
        {
            GTs = new double[6];
            string[] allLines = File.ReadAllLines(fname);
            if (allLines == null || allLines.Length == 0)
                return null;
            //filename = "08MAR25030758-M2AS_R1C1-052827221030_01_P001.TIF";
            string atif = null;
            foreach (string aLine in allLines)
            {
                if (aLine.Contains("filename = \"") && aLine.EndsWith(".TIF\";"))
                {
                    atif = aLine.Replace("filename = \"",string.Empty).Replace("\";",string.Empty).Trim();
                    break;
                }
            }
            if (atif == null)
                return null;
            atif = Path.Combine(Path.GetDirectoryName(fname), atif);
            if (!File.Exists(atif))
                return null;
            using (Dataset ds = Gdal.Open(atif, Access.GA_ReadOnly))
            {
                ds.GetGeoTransform(GTs);
                string spatialRefString = ds.GetProjectionRef();
                if (string.IsNullOrWhiteSpace(spatialRefString))
                    return null;
                return SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRefString, enumWKTSource.GDAL);
            }
        }
    }
}
