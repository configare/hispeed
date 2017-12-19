using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Core
{
    internal abstract class EsriShapeFilesReader : IVectorFeatureDataReader,
        IFeatureFetcher,
        IDisposable
    {
        protected FileStream _fsMainFile = null;
        protected BinaryReader _brMainFile = null;
        protected FileStream _fsShxFile = null;
        protected BinaryReader _brShxFile = null;
        protected IDbfReader _dbfReader = null;
        protected IAnnotationDbfReader _annReader = null;
        protected Envelope _envelope = new Envelope();
        protected const int cstHeaderLength = 100;
        protected enumShapeType _shapeType;
        protected int _fileLength = 0;
        protected int _featureCount = -1; // -1为还没有读取要素，只读了文件头
        protected List<Feature> _features = null;
        protected ISpatialReference _spatialReference = null;
        protected string[] _fields = null;
        protected string _dbffile = null;
        private string _filename = null;
        private ArgOfLeveling _argOfLeveling = null;
        private long _fileHeaderSize = 0;

        public EsriShapeFilesReader()
        {
        }

        public bool IsOK
        {
            get
            {
                return true;
            }
        }

        public void SetArgsOfLeveling(ArgOfLeveling arg)
        {
            _argOfLeveling = arg;
        }

        public ISpatialReference SpatialReference
        {
            get { return _spatialReference; }
        }

        public bool TryOpen(string filename, byte[] bytes, params object[] args)
        {
            _filename = filename;
            if (!filename.ToUpper().EndsWith(".SHP"))
                return false;
            if (string.IsNullOrEmpty(filename))
                throw new NullReferenceException("new ShapeFilesReader(null)");
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);
            string mainFile = filename;
            string dir = Path.GetDirectoryName(filename);
            string shxFile = Path.Combine(dir, Path.GetFileNameWithoutExtension(filename) + ".shx");
            if (!File.Exists(shxFile))
                throw new FileNotFoundException(shxFile);
            _dbffile = Path.Combine(dir, Path.GetFileNameWithoutExtension(filename) + ".dbf");
            if (!File.Exists(_dbffile))
                throw new FileNotFoundException(_dbffile);
            string prjFile = Path.Combine(dir, Path.GetFileNameWithoutExtension(filename) + ".prj");
            if (File.Exists(prjFile))
                ParseProjectionInfo(prjFile);
            //
            _fields = ParseDbfToDataTable.GetFields(_dbffile);
            _fsMainFile = new FileStream(mainFile, FileMode.Open, FileAccess.Read);
            _brMainFile = new BinaryReader(_fsMainFile);
            _fsShxFile = new FileStream(shxFile, FileMode.Open, FileAccess.Read);
            _brShxFile = new BinaryReader(_fsShxFile);
            //查看对应的注记文件是否存在
            try
            {
                string anndbf = Path.Combine(Path.GetDirectoryName(_dbffile), Path.GetFileNameWithoutExtension(_dbffile) + "_注记.dbf");
                if (File.Exists(anndbf))
                {
                    _annReader = new AnnotationDbfReader(anndbf);
                }
            }
            catch (Exception ex)
            {
                Log.WriterException("EsriShapeFilesReader", "TryOpen", ex);
            }
            //读文件头
            ReadHeader();
            //
            return true;
        }

        private void ParseProjectionInfo(string prjFile)
        {
            try
            {
                _spatialReference = SpatialReferenceFactory.GetSpatialReferenceByPrjFile(prjFile);
            }
            catch (Exception ex)
            {
                Log.WriterException("ShapeFileReader", "ParseProjectionInfo", ex);
            }
        }

        private void ReadHeader()
        {
            //FileCode
            int fileCode = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));
            if (fileCode != 9994)
                throw new Exception("文件不是ESRI Shape Files文件头！");
            //Skip 20 bytes unused (5 integer)
            _brMainFile.ReadBytes(20);
            //File length
            _fileLength = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4)) * 2;
            if (_fileLength != _fsMainFile.Length)
                throw new Exception("ESRI Shape Files文件未正确结束！");
            //Version
            int Version = ToLocalEndian.ToInt32FromLittle(_brMainFile.ReadBytes(4));
            //Shape Type
            int ShapeType = ToLocalEndian.ToInt32FromLittle(_brMainFile.ReadBytes(4));
            _shapeType = (enumShapeType)ShapeType;
            _envelope.MinX = ToLocalEndian.ToDouble64FromLittle(_brMainFile.ReadBytes(8));
            _envelope.MinY = ToLocalEndian.ToDouble64FromLittle(_brMainFile.ReadBytes(8));
            _envelope.MaxX = ToLocalEndian.ToDouble64FromLittle(_brMainFile.ReadBytes(8));
            _envelope.MaxY = ToLocalEndian.ToDouble64FromLittle(_brMainFile.ReadBytes(8));
            AdjustZeroWidthEnvelope(_envelope);
            //Skip minZ,maxZ,minM,maxM
            _brMainFile.ReadBytes(8 * 4);
            _fileHeaderSize = _fsMainFile.Position;
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

        private bool _isLoaded = false;
        public void ReadRecords(IProgressTracker tracker)
        {
            lock (_fsMainFile)
            {
                if (_isLoaded)
                    return;
                try
                {
                    _dbfReader = new DbaseReader(_dbffile);
                    _featureCount = 0;
                    _features = new List<Feature>();
                    while (_fsMainFile.Position < _fileLength)
                    {
                        //Record header
                        int oid = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));
                        int contentSize = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));//16bit 字为单位
                        //if (contentSize < 0)
                        //    Console.WriteLine("");
                        byte[] contentBytes = _brMainFile.ReadBytes(contentSize * 2);
                        object obj = BytesToFeature(contentBytes, oid);
                        _features.Add(obj as Feature);
                        //
                        _featureCount++;
                    }

                    if (_argOfLeveling != null && _argOfLeveling.Enabled && _shapeType == enumShapeType.Point)
                    {
                        //int bTime = Environment.TickCount;
                        using (LevelAdjuster set = new LevelAdjuster())
                        {
                            set.BeginLevel = _argOfLeveling.BeginLevel;
                            set.GridSize = _argOfLeveling.GridSize;
                            set.Features = _features.ToArray();
                            set.Do();
                        }
                        //int eTime = Environment.TickCount - bTime;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _isLoaded = true;
                }
            }
        }

        private object BytesToFeature(byte[] contentBytes, int oid)
        {
            using (MemoryStream ms = new MemoryStream(contentBytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    return ConstructFeature(br, oid);
                }
            }
        }

        protected abstract object ConstructFeature(BinaryReader br, int oid);

        #region IShapeFilesReader 成员

        public enumShapeType ShapeType
        {
            get { return _shapeType; }
        }

        public Envelope Envelope
        {
            get { return _envelope; }
        }

        public int FeatureCount
        {
            get
            {
                if (_featureCount == -1 && _features == null)
                    ReadRecords(null);
                return _featureCount != -1 ? _featureCount : 0;
            }
        }

        public Feature[] Features
        {
            get
            {
                if (_features == null && _featureCount == -1)
                    ReadRecords(null);
                return _features != null ? _features.ToArray() : null;
            }
        }

        public string[] Fields
        {
            get
            {
                return _fields;
            }
        }

        private object lockObject = new object();
        public Feature[] GetFeatures(Envelope envelope)
        {
            lock (lockObject)
            {
                if (envelope == null)
                    return null;
                if (_features == null && _featureCount == -1)
                    ReadRecords(null);
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
        }

        public Feature[] GetFeatures_NEW(Envelope envelope)
        {
            lock (lockObject)
            {
                if (envelope == null)
                    return null;
                if (_features == null && _featureCount == -1)
                    ReadRecords(null);
                Envelope validExtent = _envelope.IntersectWith(envelope);
                if (validExtent == null)
                    return null;
                List<Feature> retFets = new List<Feature>();
                FetchFeatures((fet) =>
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
                );
                if (_argOfLeveling != null && _argOfLeveling.Enabled && _shapeType == enumShapeType.Point)
                {
                    using (LevelAdjuster set = new LevelAdjuster())
                    {
                        set.BeginLevel = _argOfLeveling.BeginLevel;
                        set.GridSize = _argOfLeveling.GridSize;
                        set.Features = retFets.ToArray();
                        set.Do();
                    }
                }
                return retFets.Count > 0 ? retFets.ToArray() : null;
            }
        }
        #endregion

        public Feature FetchFirstFeature()
        {
            if (_features != null)
            {
                if (_features.Count > 0)
                    return _features[0];
                return null;
            }
            else
            {
                return ReadFirstFromFile();
            }
        }

        public Feature FetchFeature(Func<Feature, bool> where)
        {
            using (_dbfReader = new DbaseReader(_dbffile))
            {
                _fsMainFile.Seek(_fileHeaderSize, SeekOrigin.Begin);
                while (_fsMainFile.Position < _fileLength)
                {
                    int oid = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));
                    int contentSize = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));//16bit 字为单位
                    byte[] contentBytes = _brMainFile.ReadBytes(contentSize * 2);
                    object obj = BytesToFeature(contentBytes, oid);
                    if (where(obj as Feature))
                        return obj as Feature;
                }
                return null;
            }
        }

        public Feature[] FetchFeatures(Func<Feature, bool> where)
        {
            using (_dbfReader = new DbaseReader(_dbffile))
            {
                _fsMainFile.Seek(_fileHeaderSize, SeekOrigin.Begin);
                List<Feature> retFeatures = new List<Feature>();
                while (_fsMainFile.Position < _fileLength)
                {
                    int oid = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));
                    int contentSize = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));//16bit 字为单位
                    byte[] contentBytes = _brMainFile.ReadBytes(contentSize * 2);
                    object obj = BytesToFeature(contentBytes, oid);
                    if (where(obj as Feature))
                        retFeatures.Add(obj as Feature);
                }
                return retFeatures.Count > 0 ? retFeatures.ToArray() : null;
            }
        }

        public void FetchFeatures(Action<Feature> action)
        {
            using (_dbfReader = new DbaseReader(_dbffile))
            {
                _fsMainFile.Seek(_fileHeaderSize, SeekOrigin.Begin);
                while (_fsMainFile.Position < _fileLength)
                {
                    int oid = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));
                    int contentSize = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));//16bit 字为单位
                    byte[] contentBytes = _brMainFile.ReadBytes(contentSize * 2);
                    object obj = BytesToFeature(contentBytes, oid);
                    action(obj as Feature);
                }
            }
        }


        public Feature[] FetchFeatures()
        {
            using (_dbfReader = new DbaseReader(_dbffile))
            {
                _fsMainFile.Seek(_fileHeaderSize, SeekOrigin.Begin);
                List<Feature> retFeatures = new List<Feature>();
                while (_fsMainFile.Position < _fileLength)
                {
                    int oid = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));
                    int contentSize = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));//16bit 字为单位
                    byte[] contentBytes = _brMainFile.ReadBytes(contentSize * 2);
                    object obj = BytesToFeature(contentBytes, oid);
                    retFeatures.Add(obj as Feature);
                }
                return retFeatures.Count > 0 ? retFeatures.ToArray() : null;
            }
        }

        private Feature ReadFirstFromFile()
        {
            using (_dbfReader = new DbaseReader(_dbffile))
            {
                _fsMainFile.Seek(_fileHeaderSize, SeekOrigin.Begin);
                while (_fsMainFile.Position < _fileLength)
                {
                    int oid = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));
                    int contentSize = ToLocalEndian.ToInt32FromBig(_brMainFile.ReadBytes(4));//16bit 字为单位
                    byte[] contentBytes = _brMainFile.ReadBytes(contentSize * 2);
                    object obj = BytesToFeature(contentBytes, oid);
                    return obj as Feature;
                }
                return null;
            }
        }

        #region IDisposable 成员

        public virtual void Dispose()
        {
            if (_annReader != null)
                _annReader.Dispose();
            if (_dbfReader != null)
                _dbfReader.Dispose();
            if (_fsMainFile != null)
                _fsMainFile.Close();
            if (_brMainFile != null)
                _brMainFile.Close();
            if (_fsShxFile != null)
                _fsShxFile.Close();
            if (_brShxFile != null)
                _brShxFile.Close();
            _features = null;
            GC.Collect();
        }

        #endregion
    }
}
