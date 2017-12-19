using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.IO;

namespace GeoDo.RSS.DF.AeronetData
{
    /// <summary>
    /// Aeronet数据驱动，指定文件夹或文件名称，读取所有站点数据
    /// 同一文件夹中需要提供同级数据,且均为日合成或时刻数据
    /// </summary>
    public class AeronetDataReader : IVectorFeatureDataReader, IDisposable
    {
        private string[] _fileNames;
        private Envelope _envelope;
        private string[] _fields;
        private Feature[] _features;
        private ISpatialReference _spatialReference;
        private enumShapeType _shapeType;
        private ArgOfLeveling _arg;
        private string _level;

        public AeronetDataReader()
        {
        }

        public Envelope Envelope
        {
            get { return _envelope; }
        }

        public int FeatureCount
        {
            get { return 1; }
        }

        public Feature[] Features
        {
            get { return _features; }
        }

        public int Length
        {
            get { return _features.Length; }

        }

        public string[] Fields
        {
            get { return _fields; }
        }

        public enumShapeType ShapeType
        {
            get { return _shapeType; }
        }

        public ISpatialReference SpatialReference
        {
            get { return _spatialReference; }
        }

        public bool IsOK
        {
            get { throw new NotImplementedException(); }
        }

        public bool TryOpen(string fileDirName, byte[] bytes, params object[] args)
        {
            bool isDir = IsDir(fileDirName);
            //为文件夹
            if (isDir)
            {
                //若为文件夹，则仅能打开同一种Level的数据
                if (args == null || args.Length < 1)
                    return false;
                _level = args[0].ToString();
                if (_level != "LEV20" && _level != "LEV15" && _level != "LEV10")
                    return false;
                string[] files = Directory.GetFiles(fileDirName, "*." + _level);
                if (files == null || files.Length < 1)
                    return false;
                _fileNames = files;
                //检查文件合法性
                if (!CheckFilesIsOk())
                    return false;
                InitDataReader();
                return true;
            }
            //单文件
            else
            {
                string extension = Path.GetExtension(fileDirName).ToUpper();
                if (extension != ".LEV20" && extension != ".LEV15" && extension != ".LEV10")
                    return false;
                _fileNames = new string[] { fileDirName };
                switch (extension)
                {
                    case ".LEV20":
                        {
                            _level = "LEV20";
                            break;
                        }
                    case ".LEV15":
                            {
                                _level="LEV15";
                                break;
                            }
                    case ".LEV10":
                            {
                                _level = "LEV10";
                                break;
                            }
                }
                //检查是否为AeronetData
                if (!CheckFilesIsOk())
                    return false;
                InitDataReader();
                return true;
            }
        }

        private void InitDataReader()
        {
            _shapeType = enumShapeType.Point;
            _spatialReference = CodeCell.AgileMap.Core.SpatialReference.GetDefault();
            //
            string[] lines = File.ReadAllLines(_fileNames[0]);
            List<string> fields = new List<string>();
            fields.Add("Location");
            fields.AddRange(lines[4].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            _fields = fields.ToArray();
            List<string> fieldValues = new List<string>();
            int oId = 0; ShapePoint pt;
            double lon, lat;
            List<Feature> features = new List<Feature>();
            foreach (string file in _fileNames)
            {
                lines = File.ReadAllLines(file);
                string[] infos = lines[2].Split(new string[] { ",", "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (!double.TryParse(infos[3], out lon) || !double.TryParse(infos[5], out lat))
                    return;
                for (int i = 5; i < lines.Length; i++)
                {
                    pt = new ShapePoint(lon, lat);
                    fieldValues.Clear();
                    fieldValues.Add(infos[1]);
                    fieldValues.AddRange(lines[i].Split(new string[] { ","}, StringSplitOptions.RemoveEmptyEntries));
                    Feature f = new Feature(oId++, pt, _fields, fieldValues.ToArray(), null);
                    features.Add(f);
                }
            }
            _features = features.ToArray();
            _envelope = GetEnvelope();//所有features的并集
        }

        private CodeCell.AgileMap.Core.Envelope GetEnvelope()
        {
            if (_features == null)
                return null;
            Envelope env = new Envelope(180, 90, 0, 0);
            foreach (Feature item in _features)
            {
                env.UnionWith(item.Geometry.Envelope);
            }
            return env;
        }

        public static bool IsDir(string filepath)
        {
            FileInfo fi = new FileInfo(filepath);
            FileAttributes fa = fi.Attributes;
            if ((int)fa == -1)
            {
                return false;
            }
            else if ((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                return true;
            return false;
        }

        private bool CheckFilesIsOk()
        {
            foreach (string item in _fileNames)
            {
                string[] lines = File.ReadAllLines(item);
                if (lines == null || lines.Length < 5)
                    return false;
                switch (_level)
                {
                    case "LEV20":
                        {
                            if (!lines[0].Contains("Level 2.0. Quality Assured Data."))
                                return false;
                            break;
                        }
                    case "LEV15":
                        {
                            if (!lines[0].Contains("Level 1.5. Quality Assured Data."))
                                return false;
                            break;
                        }
                    case "LEV10":
                        {
                            if (!lines[0].Contains("Level 2.0. Quality Assured Data."))
                                return false;
                            break;
                        }
                    default:
                        return false;
                }
            }
            return true;
        }

        public Feature[] GetFeatures(Envelope envelope)
        {
            if (envelope == null || _features == null || _features.Length < 1)
                return null;
            Envelope validExtent = _envelope.IntersectWith(envelope);
            if (validExtent == null)
                return null;
            List<Feature> retFets = new List<Feature>();
            foreach (Feature fet in _features)
            {
                if (validExtent.Contains(fet.Geometry.Envelope))
                {
                    retFets.Add(fet);
                }
                else
                {
                    if (fet.Geometry.Envelope.IsInteractived(validExtent))
                    {
                        retFets.Add(fet);
                        fet.IsRepeatedOverGrids = true;
                    }
                }
            }
            return retFets.Count > 0 ? retFets.ToArray() : null;
        }

        public void SetArgsOfLeveling(ArgOfLeveling arg)
        {
            _arg = arg;
        }

        public Feature FetchFeature(Func<Feature, bool> where)
        {
            return null;
        }

        public Feature[] FetchFeatures()
        {
            return _features;
        }

        public Feature[] FetchFeatures(Func<Feature, bool> where)
        {
            return null;
        }

        public Feature FetchFirstFeature()
        {
            return _features.First();
        }

        public void Dispose()
        {

        }
    }
}
