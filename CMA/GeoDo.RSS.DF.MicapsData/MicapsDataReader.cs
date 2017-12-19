#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/4 12:00:35
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.IO;
using System.Xml.Linq;

namespace GeoDo.RSS.DF.MicapsData
{
    /// <summary>
    /// 类名：MicapsDataReader
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/4 12:00:35
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class MicapsDataReader : IVectorFeatureDataReader, IDisposable
    {
        private static string DATA_CONFIG_DIR = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\MicapsDataConfig\MicapsDataDefine.xml";
        private XElement _dataSetting;
        private string _filename;
        private string _dataTypeId;
        private Feature[] _features;
        private Envelope _envelope;
        private ISpatialReference _spatialReference;
        private enumShapeType _shapeType;
        private string[] _fields;
        private ArgOfLeveling _arg;
        private bool _isOk = true;
        private int _lonIndex;
        private int _latIndex;
        private int _beginIndex = 1;
        private string _splitString;

        public MicapsDataReader()
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
            get { return _isOk; }
        }

        public bool TryOpen(string filename, byte[] bytes, params object[] args)
        {
            _filename = filename;
            if (!filename.ToUpper().EndsWith(".000"))
                return false;
            if (string.IsNullOrEmpty(filename))
                throw new NullReferenceException("new MicapsDataReader(null)");
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);
            if (args == null || args.Length < 1)
                return false;
            _dataTypeId = args[0].ToString();//identify="GroundObserveData"
            _dataSetting = GetDataTypeSetting(_dataTypeId);//指定_dataTypeId的所有Field
            _fields = GetFields(_dataSetting);//_dataTypeId的所有Field的identify组成的[]
            //检查Field的合法性
            if (!CheckFieldsIsOK(filename))
                return false;
            _shapeType = enumShapeType.Point;
            _spatialReference = CodeCell.AgileMap.Core.SpatialReference.GetDefault();
            //创建Features
            _features = ConstructPoint();//将000文件中的观测值读成features
            _envelope = GetEnvelope();//所有features的并集
            return true;
        }

        private Envelope GetEnvelope()
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Feature[] ConstructPoint()
        {
            string[] lines = File.ReadAllLines(_filename);
            if (lines == null || lines.Length < _beginIndex)
                return null;
            List<Feature> features = new List<Feature>();
            ShapePoint pt;
            string[] fieldValues;
            //除去经度、纬度属性
            int fieldCount = _fields.Length ;//字段数
            int pointCount = lines.Length - (_beginIndex-1);//可用字段数
            for (int oid = 0; oid < pointCount; oid++)
            {
                double[] lonlat = GetLatLon(lines[oid + _beginIndex - 1]);//从000的第3行开始读
                if (lonlat != null)
                {
                    pt = new ShapePoint(lonlat[0],lonlat[1]);//经度，纬度
                    fieldValues = GetPointFieldValues(lines[oid + _beginIndex - 1]);//每一个站点的信息
                    Feature f = new Feature(oid, pt, _fields, fieldValues, null);
                    features.Add(f);
                }
            }
            return features.ToArray();
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

        private string[] GetFields(XElement dataSetting)
        {
            if (dataSetting == null)
                return null;
            int fieldCount;
            if (dataSetting.Attribute("beginIndex") != null)
            {
                int beginIndex=1;
                if (Int32.TryParse(_dataSetting.Attribute("beginIndex").Value, out beginIndex))
                    _beginIndex = beginIndex;
            }
            _splitString = dataSetting.Attribute("splitChar").Value;
            if(Int32.TryParse(_dataSetting.Attribute("fieldcount").Value,out fieldCount))
            {
                string[] fields = new string[fieldCount];
                IEnumerable<XElement> items = _dataSetting.Elements("Field");
                if (items == null || items.Count() < 1)
                    return null;
                int index;
                foreach (XElement item in items)
                {
                    if(Int32.TryParse(item.Attribute("index").Value,out index))
                    {
                        if (index < fieldCount)
                        {
                            fields[index] = item.Attribute("identify").Value;
                            if (fields[index] == "Longitude")
                            {
                                _lonIndex = index;
                            }
                            else if (fields[index] == "Latitude")
                            {
                                _latIndex = index;
                            }
                        }
                    }
                }
                return fields;
            }
            return null;
        }

        private static XElement GetDataTypeSetting(string dataTypeId)
        {
            if (!File.Exists(DATA_CONFIG_DIR))
                return null;
            if (string.IsNullOrEmpty(dataTypeId))
                return null;
            XElement root = XElement.Load(DATA_CONFIG_DIR);
            IEnumerable<XElement> items = root.Elements("DataDefine");
            if (items == null || items.Count() == 0)
                return null;
            foreach (XElement item in items)
            {
                if (item.Attribute("identify").Value == dataTypeId)
                    return item;
            }
            return null;
        }

        private bool CheckFieldsIsOK(string fileName)
        {
            if (_fields == null || _fields.Length < 2)
                return false;
            string[] lines = File.ReadAllLines(fileName);
            if (lines == null || lines.Length < 3)
                return false;
            string[] fieldValues = lines[_beginIndex-1].Split(new string[] { _splitString }, StringSplitOptions.RemoveEmptyEntries);
            if (fieldValues == null || fieldValues.Length != _fields.Length)
                return false;
            bool hasLon = false, hasLat = false;
            for (int i = 0; i < _fields.Length; i++)
            {
                if (_fields[i] == "Longitude")
                    hasLon = true;
                if (_fields[i] == "Latitude")
                    hasLat = true;
                if (hasLat && hasLon)
                    break;
            }
            if (hasLat && hasLon)
                return true;
            return false;
        }

        /// <summary>
        /// 获取经纬度坐标
        /// </summary>
        /// <param name="textLine"></param>
        /// <returns>double[]={lon,lat}</returns>
        private double[] GetLatLon(string textLine)
        {
            if (string.IsNullOrEmpty(textLine))
                return null;
            string[] fieldValues = textLine.Split(new string[] { _splitString }, StringSplitOptions.RemoveEmptyEntries);
            if (fieldValues == null || fieldValues.Length<_lonIndex||fieldValues.Length<_latIndex)
                return null;
            double[] latlon=new double[2];
            double lonValue, latValue;
            if (double.TryParse(fieldValues[_latIndex].Trim(), out latValue)
                && double.TryParse(fieldValues[_lonIndex].Trim(), out lonValue))
            {
                if (lonValue < 180 && lonValue > -180)
                {
                    if (latValue < 90 && latValue > -90)
                    {
                        latlon[0] = lonValue;
                        latlon[1] = latValue;
                        return latlon;
                    }
                }
            }
            return null;
        }

        private string[] GetPointFieldValues(string textLine)
        {
            if (string.IsNullOrEmpty(textLine))
                return null;
            string[] fieldValues = textLine.Split(new string[] { _splitString }, StringSplitOptions.RemoveEmptyEntries);
            if (fieldValues == null || fieldValues.Length < (_fields.Length - 2))
                return null;
            else
                return fieldValues;
        }
    }
}
