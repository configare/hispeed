using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;
using System.IO;
using CodeCell.Bricks.Runtime;
using System.Threading.Tasks;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class FileDataSource : FeatureDataSourceBase, IGridReader, IPersistable, IFileDataSource, IQueryFeatures
    {
        protected string _fileUrl = string.Empty;
        [NonSerialized]
        protected IVectorFeatureDataReader _reader = null;
        [NonSerialized]
        protected string _cacheIdentify;//标记数据是否来源于全局缓存(null==否)

        public FileDataSource(string name)
            : base(name)
        {
            _dataSourceType = "文件数据源";
            _name = name;
        }

        public FileDataSource(string name, string fileurl)
            : this(name)
        {
            if (fileurl == null || !File.Exists(fileurl))
                throw new FileNotFoundException(fileurl == null ? string.Empty : fileurl);
            _fileUrl = fileurl;
        }

        [DisplayName("文件URL"), Category("数据源")]
        public string FileUrl
        {
            get { return _fileUrl; }
            set
            {
                if (value.ToUpper() != _fileUrl.ToUpper())
                {
                    _fileUrl = value;
                    if (_dataSourceChanged != null)
                        _dataSourceChanged(this, null);
                }
            }
        }

        protected override void Init()
        {
            if (_reader == null)
                BeginRead();
            string prjFile = Path.Combine(Path.GetDirectoryName(_fileUrl),
                                                       Path.GetFileNameWithoutExtension(_fileUrl) + ".prj");
            try
            {
                _spatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile(prjFile);
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
            }
            _fullEnvelope = _reader.Envelope.Clone() as Envelope;
            AdjustZeroWidthEnvelope(_fullEnvelope);
            _shapeType = _reader.ShapeType;
            _coordType = _spatialRef != null && _spatialRef.ProjectionCoordSystem != null ? enumCoordinateType.Projection : enumCoordinateType.Geographic;
            _gridStateIndicator = new GridStateIndicator(_fullEnvelope.Clone() as Envelope, _gridDefinition);
            _fullGridCount = _gridStateIndicator.Width * _gridStateIndicator.Height;
            _fields = _reader.Fields;
            _isInited = true;
        }

        private void AdjustZeroWidthEnvelope(Envelope fullEnvelope)
        {
            double tolerance = fullEnvelope.MinX / 10;
            if (fullEnvelope.Width < double.Epsilon)
            {
                fullEnvelope.MaxX += tolerance;
                fullEnvelope.MinX -= tolerance;
            }
            if (fullEnvelope.Height < double.Epsilon)
            {
                fullEnvelope.MaxY += tolerance;
                fullEnvelope.MinY -= tolerance;
            }
        }

        #region IGridReader Members

        [Browsable(false)]
        public override bool IsReady
        {
            get { return _reader != null; }
        }

        public override void BeginRead()
        {
            try
            {
                IUniversalVectorDataReader udr = VectorDataReaderFactory.GetUniversalDataReader(_fileUrl);
                _reader = udr as IVectorFeatureDataReader;
                _reader.SetArgsOfLeveling(_argOfLeveling);
                _readIsFinished = false;
            }
            finally
            {
                if (GlobalCacher.VectorDataGlobalCacher != null &&
                    GlobalCacher.VectorDataGlobalCacher.IsEnabled &&
                    GlobalCacher.VectorDataGlobalCacher.IsGllPrjOfActiveViewer)
                {
                    GlobalCacher.VectorDataGlobalCacher.Request(_fileUrl);
                }
            }
        }

        public override IGrid ReadGrid(int gridNo)
        {
            if (GlobalCacher.VectorDataGlobalCacher != null
                && GlobalCacher.VectorDataGlobalCacher.IsEnabled &&
                GlobalCacher.VectorDataGlobalCacher.IsGllPrjOfActiveViewer)
            {
                bool isOK = false;
                IGrid grid = ReadGridFromCacher(gridNo, out isOK);
                if (isOK)
                    return grid;
            }
            return ReadGridFromFile(gridNo);
        }

        private IGrid ReadGridFromFile(int gridNo)
        {
            Envelope evp = _gridStateIndicator.GetEnvelope(gridNo);
            Feature[] fets = _reader.GetFeatures(evp);
            if (fets != null && fets.Length > 0)
            {
                fets = TrySplit(fets);
                if (fets == null)
                    return null;
                evp = fets[0].Geometry.Envelope.Clone() as Envelope;
                foreach (Feature fet in fets)
                {
                    fet.SetFeatureClass(_featureClass);
                    evp.UnionWith(fet.Geometry.Envelope);
                }
                return new Grid(gridNo, evp, fets);
            }
            return new Grid(gridNo, evp, null);
        }

        private Feature[] TrySplit(Feature[] fets)
        {
            if (_featureClass == null || _featureClass.RuntimeProjecter == null)
                return fets;
            string args = _featureClass.RuntimeProjecter.CanvasSpatialRef;
            if (args == null)
                return fets;
            IFeatureSplitterByProjection splitter = new FeatureSplitterByProjection(args);
            if (!splitter.IsNeedSplit)
                return fets;
            List<Feature> retFeats = new List<Feature>();
            foreach (Feature f in fets)
            {
                Feature[] splitedFets;
                splitter.Split(f, out splitedFets);
                if (splitedFets != null && splitedFets.Length > 0)
                    retFeats.AddRange(splitedFets);
            }
            //Parallel.For(0, fets.Length,
            //    (idx) =>
            //    {
            //        Feature[] splitedFets;
            //        splitter.Split(fets[idx], out splitedFets);
            //        if (splitedFets != null && splitedFets.Length > 0)
            //            retFeats.AddRange(splitedFets);
            //    });
            return retFeats.Count > 0 ? retFeats.ToArray() : null;
        }

        private IGrid ReadGridFromCacher(int gridNo, out bool isOK)
        {
            isOK = false;
            ICachedVectorData cache = GlobalCacher.VectorDataGlobalCacher.GetData(_fileUrl);
            if (cache == null)
                return null;
            if (_cacheIdentify == null)
                _cacheIdentify = cache.Identify;
            Envelope evp = _gridStateIndicator.GetEnvelope(gridNo);
            Feature[] features;
            if (cache.CoordType == enumCoordinateType.Geographic)
            {
                features = cache.GetFeatures(evp);
                isOK = true;
                return new Grid(gridNo, evp, features);
            }
            else if (cache.CoordType == enumCoordinateType.Projection)
            {
                _featureClass.RuntimeProjecter.Project(evp);
                features = cache.GetFeatures(evp);
                if (features != null)
                {
                    foreach (Feature fet in features)
                        fet.Projected = true;
                }
                IGrid grid = new Grid(gridNo, evp, features);
                grid.CoordIsConverted = true;
                isOK = true;
                return grid;
            }
            return null;
        }

        public override void EndRead()
        {
            try
            {
                if (_reader != null)
                {
                    _reader.Dispose();
                    _reader = null;
                }
            }
            finally
            {
                _readIsFinished = true;
                _isReady = false;
            }
        }

        #endregion

        #region IQueryFeatures Members

        public override Feature[] Query(QueryFilter filter)
        {
            if (filter == null || (filter.Geometry == null && string.IsNullOrEmpty(filter.WhereClause)))
                return null;
            if (filter.Geometry != null && string.IsNullOrEmpty(filter.WhereClause))
            {
                return QueryByGeometry(filter.Geometry, filter.Relation);
            }
            else if (filter.Geometry == null && !string.IsNullOrEmpty(filter.WhereClause))
            {
                return QueryByWhereClause(filter.WhereClause);
            }
            else
            {
                return QueryByComplex(filter.Geometry, filter.Relation, filter.WhereClause);
            }
        }

        private Feature[] QueryByGeometry(Shape shape, enumSpatialRelation rel)
        {
            if (shape == null)
                return null;
            if (!_isReady)
                BeginRead();
            if (_reader == null)
                return null;
            Feature[] allfeatures = _reader.Features;
            if (allfeatures == null || allfeatures.Length == 0)
                return null;
            List<Feature> retfets = new List<Feature>();
            foreach (Feature fet in allfeatures)
            {
                if (fet.Geometry == null)
                    continue;
                fet.SetFeatureClass(_featureClass);
                switch (rel)
                {
                    case enumSpatialRelation.Intersect:
                        if (fet.Geometry.Envelope.IsInteractived(shape.Envelope))
                            retfets.Add(fet);
                        break;
                    case enumSpatialRelation.Contains:
                        if (shape.Envelope.Contains(fet.Geometry.Envelope))
                            retfets.Add(fet);
                        break;
                    case enumSpatialRelation.Within:
                        if (fet.Geometry.Envelope.Contains(shape.Envelope))
                            retfets.Add(fet);
                        break;
                    case enumSpatialRelation.Disjoint:
                        if (!fet.Geometry.Envelope.IsInteractived(shape.Envelope))
                            retfets.Add(fet);
                        break;
                    default:
                        throw new NotSupportedException("数据源\"" + GetType().ToString() + "\"不支持类型为\"" + rel.ToString() + "\"的空间查询。");
                }
            }
            return retfets.Count > 0 ? retfets.ToArray() : null;
        }

        private Feature[] QueryByWhereClause(string whereClause)
        {
            if (_reader == null)
                BeginRead();
            Feature[] allfeatures = _reader.Features;
            if (allfeatures == null || allfeatures.Length == 0)
                return null;
            List<Feature> retfets = new List<Feature>();
            string[] values = whereClause.Split(' ');
            foreach (string v in values)
            {
                if (v.Trim() == string.Empty)
                    continue;
                var fets = from fet in allfeatures
                           where IsContainKey(fet.FieldValues, v)
                           select fet;
                if (fets != null)
                    retfets.AddRange(fets as IEnumerable<Feature>);
            }
            if (retfets.Count > 0)
                foreach (Feature f in retfets)
                    f.SetFeatureClass(_featureClass);
            return retfets.Count > 0 ? retfets.ToArray() : null;
        }

        private bool IsContainKey(string[] fieldValues, string key)
        {
            if (fieldValues == null)
                return false;
            foreach (string v in fieldValues)
                if (v != null)
                    if (v.Contains(key))
                        return true;
            return false;
        }

        private Feature[] QueryByComplex(Shape shape, enumSpatialRelation rel, string whereClause)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            try
            {
                if (_reader != null && _isReady)
                {
                    _reader.Dispose();
                    //_reader = null;
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(_cacheIdentify))
                    GlobalCacher.VectorDataGlobalCacher.Release(_cacheIdentify);
                //
                base.Dispose();
            }
        }

        #endregion

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("DataSource");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            obj.AddAttribute("name", _name != null ? _name : string.Empty);
            obj.AddAttribute("fileurl", _fileUrl != null ? Map.GetRelativeFilename(_fileUrl) : string.Empty);
            if (_argOfLeveling != null)
            {
                obj.AddSubNode(_argOfLeveling.ToPersistObject());
            }
            return obj;
        }

        public static FileDataSource FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            string name = null;
            if (ele.Attribute("name") != null)
                name = ele.Attribute("name").Value;
            string fileurl = ele.Attribute("fileurl").Value;
            string fname = MapFactory.GetFullFilename(fileurl);
            if (!File.Exists(fname))
                return null;
            FileDataSource ds = new FileDataSource(name, fname);
            if (ele.Element("ArgsOfLeveling") != null)
                ds.SetArgOfLevel(ArgOfLeveling.FromXElement(ele.Element("ArgsOfLeveling")));
            return ds;
        }

    }
}
