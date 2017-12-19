using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    public class MemoryDataSource : FeatureDataSourceBase
    {
        private IGrid _grid = null;

        public MemoryDataSource(string name,
                               enumShapeType shapeType
                               )
            : base(name)
        {
            _shapeType = shapeType;
            _coordType = enumCoordinateType.Geographic;
            _gridDefinition = new GridDefinition(360, 180);
            _fullEnvelope = new Envelope(-180, -90, 180, 90);
            _gridStateIndicator = new GridStateIndicator(_fullEnvelope.Clone() as Envelope, _gridDefinition);
            _fullGridCount = _gridStateIndicator.Width * _gridStateIndicator.Height;
            _grid = new Grid(0, _fullEnvelope.Clone() as Envelope, new Feature[] { });
        }

        public MemoryDataSource(string name,
                                enumShapeType shapeType,
                                enumCoordinateType coordType,
                                Envelope fullEnvelope
                                )
            : base(name)
        {
            _shapeType = shapeType;
            _coordType = enumCoordinateType.Geographic;
            _gridDefinition = new GridDefinition((float)(fullEnvelope.Width + float.Epsilon),
                                                 (float)(fullEnvelope.Height + float.Epsilon));
            _fullEnvelope = fullEnvelope;
            _gridStateIndicator = new GridStateIndicator(_fullEnvelope.Clone() as Envelope, _gridDefinition);
            _fullGridCount = _gridStateIndicator.Width * _gridStateIndicator.Height;
            _grid = new Grid(0, _fullEnvelope.Clone() as Envelope, new Feature[] { });
        }

        public MemoryDataSource(string name,
                                enumShapeType shapeType,
                                string[] filedNames
                               )
            : base(name)
        {
            _shapeType = shapeType;
            _coordType = enumCoordinateType.Geographic;
            _gridDefinition = new GridDefinition(360, 180);
            _fullEnvelope = new Envelope(-180, -90, 180, 90);
            _gridStateIndicator = new GridStateIndicator(_fullEnvelope.Clone() as Envelope, _gridDefinition);
            _fullGridCount = _gridStateIndicator.Width * _gridStateIndicator.Height;
            _grid = new Grid(0, _fullEnvelope.Clone() as Envelope, new Feature[] { });
            _fields = filedNames;
        }

        public MemoryDataSource(string name,
                        enumShapeType shapeType,
                        enumCoordinateType coordType,
                        Envelope fullEnvelope,
                        string[] filedNames
                        )
            : base(name)
        {
            _shapeType = shapeType;
            _coordType = enumCoordinateType.Geographic;
            _gridDefinition = new GridDefinition((float)(fullEnvelope.Width + float.Epsilon),
                                                 (float)(fullEnvelope.Height + float.Epsilon));
            _fullEnvelope = fullEnvelope;
            _gridStateIndicator = new GridStateIndicator(_fullEnvelope.Clone() as Envelope, _gridDefinition);
            _fullGridCount = _gridStateIndicator.Width * _gridStateIndicator.Height;
            _grid = new Grid(0, _fullEnvelope.Clone() as Envelope, new Feature[] { });
            _fields = filedNames;
        }

        public void Refresh()
        {
            _readIsFinished = false;
        }

        protected override void Init()
        {
            _isReady = true;
            _isInited = true;
            _gridStateIndicator = new GridStateIndicator(_fullEnvelope.Clone() as Envelope, _gridDefinition);
            _fullGridCount = _gridStateIndicator.Width * _gridStateIndicator.Height;
        }

        public void ClearFeatures()
        {
            _featureClass.Grids[0].VectorFeatures.Clear();
        }

        public void AddFeatures(Feature[] features)
        {
            if (features == null || features.Length == 0)
                return;
            _grid.VectorFeatures.AddRange(features);
            _featureClass.TryProject(_grid);
        }

        public bool IsExist(Feature feature)
        {
            return _grid.VectorFeatures.Contains(feature);
        }

        public void Remove(Feature feature)
        {
            if (_grid.VectorFeatures != null && _grid.VectorFeatures.Contains(feature))
                _grid.VectorFeatures.Remove(feature);
        }

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("DataSource");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            obj.AddAttribute("name", _name != null ? _name : string.Empty);
            obj.AddAttribute("shapetype", _shapeType != null ? _shapeType.ToString() : string.Empty);
            if (_argOfLeveling != null)
            {
                obj.AddSubNode(_argOfLeveling.ToPersistObject());
            }
            return obj;
        }

        public static MemoryDataSource FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            string name = null;
            if (ele.Attribute("name") != null)
                name = ele.Attribute("name").Value;
            string shapetype = ele.Attribute("shapetype").Value;
            enumShapeType stype = enumShapeType.Point;
            foreach (enumShapeType st in Enum.GetValues(typeof(enumShapeType)))
            {
                if (st.ToString() == shapetype)
                {
                    stype = st;
                    break;
                }
            }
            MemoryDataSource ds = new MemoryDataSource(name, stype);
            if (ele.Element("ArgsOfLeveling") != null)
                ds.SetArgOfLevel(ArgOfLeveling.FromXElement(ele.Element("ArgsOfLeveling")));
            return ds;
        }

        public override void EndRead()
        {
            base.EndRead();
        }

        public override IGrid ReadGrid(int gridNo)
        {
            return _grid;
        }

        public override Feature[] Query(QueryFilter filter)
        {
            //未实现接口
            return new Feature[] { };
        }
    }
}
