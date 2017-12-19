using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class VectorAOITemplate : IDisposable
    {
        private string _shpFileName;
        private Func<CodeCell.AgileMap.Core.Feature, bool> _where;
        private bool _isReverse = false ;
        private static string VECTOR_TEMPLATE = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\VectorTemplate";

        public VectorAOITemplate(string shpFileName, Func<CodeCell.AgileMap.Core.Feature, bool> where)
        {
            _shpFileName = shpFileName.ToUpper();
            _where = where;
        }

        public VectorAOITemplate(string shpFileName)
        {
            _shpFileName = shpFileName.ToUpper();
            _where = (fet) => { return true; };
        }

        public VectorAOITemplate(string shpFileName, bool useFirstFeature)
        {
            _shpFileName = shpFileName.ToUpper();
        }

        public bool IsReverse
        {
            get { return _isReverse; }
            set { _isReverse = value; }
        }

        public byte[] GetRaster(Envelope dstEnvelope, Size size,string nameField,out string[] names)
        {
            names = null;
            List<string> retNames = new List<string>();
            ShapePolygon[] geometrys = GetGeometry(retNames,nameField);
            if (geometrys == null || geometrys.Length == 0)
                return null;
            names = retNames.ToArray();
            using (IVectorAOIGenerator gen = new VectorAOIGenerator())
            {
                return gen.GetRaster(geometrys, dstEnvelope, size);
            }
        }

        public int[] GetAOI(Envelope dstEnvelope, Size size)
        {
            ShapePolygon[] geometrys = GetGeometry(null,null);
            if (geometrys == null || geometrys.Length == 0)
                return null;
            using (IVectorAOIGenerator gen = new VectorAOIGenerator())
            {               
                int[] aoi = gen.GetAOI(geometrys, dstEnvelope, size);
                if (_isReverse)
                    return AOIHelper.Reverse(aoi, size);
                return aoi;
            }
        }

        public ShapePolygon[] GetGeometry(List<string> retNames,string nameField)
        {
            string fname = FindVector();
            if (fname == null)
                return null;
            IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(fname) as IVectorFeatureDataReader;
            if (dr == null)
                return null;
            try
            {
                Feature[] fets;
                if (_where != null)
                    fets = dr.FetchFeatures((f) => { return _where(f); });
                else
                {
                    Feature f = dr.FetchFirstFeature();
                    if (f != null)
                        fets = new Feature[] { f };
                    else
                        return null;
                }
                if (fets == null || fets.Length == 0)
                    return null;
                List<ShapePolygon> geometrys = new List<ShapePolygon>();
                foreach (Feature fet in fets)
                {
                    geometrys.Add(fet.Geometry as ShapePolygon);
                    if (retNames != null)
                        retNames.Add(fet.GetFieldValue(nameField)??string.Empty);
                }
                return geometrys.ToArray();
            }
            finally
            {
                dr.Dispose();
            }
        }

        public Feature[] GetFeatures()
        {
            string fname = FindVector();
            if (fname == null)
                return null;
            IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(fname) as IVectorFeatureDataReader;
            if (dr == null)
                return null;
            try
            {
                Feature[] fets;
                if (_where != null)
                    fets = dr.FetchFeatures((f) => { return _where(f); });
                else
                {
                    Feature f = dr.FetchFirstFeature();
                    if (f != null)
                        fets = new Feature[] { f };
                    else
                        return null;
                }
                return fets.Length == 0 ? null : fets.ToArray();
            }
            finally
            {
                dr.Dispose();
            }
        }

        private string FindVector()
        {
            List<string> retfNames = new List<string>();
            FindFilename(MifEnvironment.VECTOR_TEMPLATE, retfNames);
            if (retfNames.Count < 1)
                FindFilename(VECTOR_TEMPLATE, retfNames);
            return retfNames.Count > 0 ? retfNames[0] : null;
        }

        public static string FindVectorFullname(string shpFileVectorTemplate)
        {
            if (Directory.Exists(MifEnvironment.VECTOR_TEMPLATE))
            {
                string[] fnames = Directory.GetFiles(MifEnvironment.VECTOR_TEMPLATE, "*.shp", SearchOption.AllDirectories);
                if (fnames != null && fnames.Length > 0)
                {
                    foreach (string f in fnames)
                    {
                        if (Path.GetFileName(f).ToUpper() == shpFileVectorTemplate.ToUpper())
                        {
                            return Path.GetFullPath(f);
                        }
                    }
                }
            }
            if (Directory.Exists(VECTOR_TEMPLATE))
            {
                string[] fnames = Directory.GetFiles(VECTOR_TEMPLATE, "*.shp", SearchOption.AllDirectories);
                if (fnames != null && fnames.Length > 0)
                {
                    foreach (string f in fnames)
                    {
                        if (Path.GetFileName(f).ToUpper() == shpFileVectorTemplate.ToUpper())
                        {
                            return Path.GetFullPath(f);
                        }
                    }
                }
            }
            return String.Empty;
        }

        private void FindFilename(string dir, List<string> retfNames)
        {
            if (!Directory.Exists(dir))
                return;
            string[] fnames = Directory.GetFiles(dir, "*.shp");
            if (fnames != null && fnames.Length > 0)
            {
                foreach (string f in fnames)
                {
                    if (Path.GetFileName(f).ToUpper() == _shpFileName)
                    {
                        retfNames.Add(f);
                        return;
                    }
                }
            }
            string[] dirs = Directory.GetDirectories(dir);
            if (dirs != null && dirs.Length > 0)
                foreach (string d in dirs)
                    FindFilename(d, retfNames);
        }

        public void Dispose()
        {
            _where = null;
        }
    }
}
