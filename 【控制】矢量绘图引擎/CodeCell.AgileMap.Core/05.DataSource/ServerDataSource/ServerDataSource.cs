using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using System.ComponentModel;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Core
{
    public class ServerDataSource:FeatureDataSourceBase, IGridReader, IPersistable,IServerDataSource,IQueryFeatures
    {
        protected string _uri = null;
        protected InstanceIdentify _instanceIdentify = null;
        protected FetClassIdentify _fetclassIdentify = null;
        protected string _readGridUrl = null;

        public ServerDataSource(string name)
            : base(name)
        {
            throw new NotImplementedException();
        }

        public ServerDataSource(string name, string uri, InstanceIdentify instanceIdentify, FetClassIdentify fetclassIdentify)
            :base(name)
        {
            _uri = uri;
            _instanceIdentify = instanceIdentify;
            _fetclassIdentify = fetclassIdentify;
            GenerateUrl();
        }

        private void GenerateUrl()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add(HttpCommands.cstArgNameCommand, HttpCommands.cstArgCmdReadFeatures);
            args.Add(HttpCommands.cstArgInstanceId, _instanceIdentify.Id.ToString());
            args.Add(HttpCommands.cstArgFetClassId, _fetclassIdentify.Id);
            args.Add(HttpCommands.cstArgMinX, "{0}");
            args.Add(HttpCommands.cstArgMinY, "{1}");
            args.Add(HttpCommands.cstArgMaxX, "{2}");
            args.Add(HttpCommands.cstArgMaxY, "{3}");
            string url = HttpCommands.GetCatalogUrlPage(_uri, HttpCommands.cstCatalogPage);
            url = HttpCommands.GetCatalogUrl(url, args);
            _readGridUrl = url;
        }

        [Browsable(false)]
        public override bool IsReady
        {
            get { return true; }
        }

        protected override void Init()
        {
            FetClassProperty pro = GetFetClassProperty();
            if (pro != null)
            {
                _fields = pro.Fields;
                _fullEnvelope = pro.FullEnvelope.Clone() as Envelope;
                _shapeType = pro.ShapeType;
                _spatialRef = pro.SpatialReference;
                _coordType = pro.CoordinateType;
                _featureCount = pro.FeatureCount;
                if (_shapeType == enumShapeType.Point)
                {
                    TryAdjustGridDefinition();
                }
                _gridStateIndicator = new GridStateIndicator(_fullEnvelope.Clone() as Envelope, _gridDefinition);
                _fullGridCount = _gridStateIndicator.Width * _gridStateIndicator.Height;
                _isInited = true;
            }
        }

        private void TryAdjustGridDefinition()
        {
            Envelope evp = _fullEnvelope;
            double density = _featureCount / (evp.Width * evp.Height);
            //package size
            int packageSize = 100 * 1024;//200KB
            int fetSize = GetEstimateFeatureSize();
            double reqSize = _featureCount * fetSize;
            int gridCount = (int)Math.Ceiling(reqSize / packageSize);
            int gc = (int)Math.Ceiling(Math.Sqrt(gridCount));
            _gridDefinition = new GridDefinition((float)Math.Ceiling((_fullEnvelope.Width / gc)), (float)(Math.Ceiling(_fullEnvelope.Height / gc)));
        }

        private FetClassProperty GetFetClassProperty()
        {
            try
            {
                string instanceId = _instanceIdentify.Id.ToString();
                string fetclassId = _fetclassIdentify.Id;
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add(HttpCommands.cstArgNameCommand, HttpCommands.cstArgCmdGetFetClassProperty);
                args.Add(HttpCommands.cstArgInstanceId, instanceId);
                args.Add(HttpCommands.cstArgFetClassId, fetclassId);
                string url = HttpCommands.GetCatalogUrlPage(_uri, HttpCommands.cstCatalogPage);
                url = HttpCommands.GetCatalogUrl(url, args);
                object obj = GetObjectFromHttpStream.GetObject(url);
                FetClassProperty pro = obj as FetClassProperty;
                if (pro == null)
                    throw new Exception("获取要素类\"" + _name + "\"的属性对象失败。");
                return pro;
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
                return null;
            }
        }

        #region IServerDataSource Members

        public string Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        public InstanceIdentify InstanceIdentify
        {
            get { return _instanceIdentify; }
            set { _instanceIdentify = value; }
        }

        public FetClassIdentify FetClassIdentify
        {
            get { return _fetclassIdentify; }
            set { _fetclassIdentify = value; }
        }

        #endregion

        public override void BeginRead()
        {
            try
            {
                string url = HttpCommands.GetCatalogUrlPage(_uri, HttpCommands.cstCatalogPage);
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add(HttpCommands.cstArgNameCommand, HttpCommands.cstArgCmdBeginRead);
                args.Add(HttpCommands.cstArgInstanceId, _instanceIdentify.Id.ToString());
                args.Add(HttpCommands.cstArgFetClassId,_fetclassIdentify.Id);
                url = HttpCommands.GetCatalogUrl(url, args);
                object isOK = GetObjectFromHttpStream.GetObject(url);
                if (isOK is RequestIsFailed)
                {
                    Log.WriterError("ServerDataSource","BeginRead("+_instanceIdentify.Name+","+_fetclassIdentify.Name+")",(isOK as RequestIsFailed).Msg);
                }
            }
            finally
            {
                _readIsFinished = false;
            }
        }

        public override void EndRead()
        {
            try
            {
                string url = HttpCommands.GetCatalogUrlPage(_uri, HttpCommands.cstCatalogPage);
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add(HttpCommands.cstArgNameCommand, HttpCommands.cstArgCmdEndRead);
                args.Add(HttpCommands.cstArgInstanceId, _instanceIdentify.Id.ToString());
                args.Add(HttpCommands.cstArgFetClassId, _fetclassIdentify.Id);
                url = HttpCommands.GetCatalogUrl(url, args);
                object isOK = GetObjectFromHttpStream.GetObject(url);
                if (isOK is RequestIsFailed)
                {
                    Log.WriterError("ServerDataSource","EndRead("+_instanceIdentify.Name+","+_fetclassIdentify.Name+")", (isOK as RequestIsFailed).Msg);
                }
            }
            finally
            {
                _readIsFinished = true;
            }
        }

        public override IGrid ReadGrid(int gridNo)
        {
            try
            {
                Envelope evp = _gridStateIndicator.GetEnvelope(gridNo);
                string url = string.Format(_readGridUrl, evp.MinX, evp.MinY, evp.MaxX, evp.MaxY);
                object obj = GetObjectFromHttpStream.GetObject(url);
                if (obj is Feature[])
                {
                    Feature[] fets = obj as Feature[];
                    AutoLeveling(fets);
                    if (fets != null)
                        foreach (Feature f in fets)
                            f.SetFeatureClass(_featureClass);
                    return new Grid(gridNo, evp, fets);
                }
                else
                {
                    RequestIsFailed error = obj as RequestIsFailed;
                    if (error != null)
                    {
                        Log.WriterError("Read " + _fetclassIdentify.Name + "[" + gridNo.ToString() + "]" + " from " + _instanceIdentify.Name + " failed," + error.Msg);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
                return null;
            }
        }

        private int GetEstimateFeatureSize()
        {
            int size = 1 * 6 * 2; //averate bytes,1 field,6 chinese word,12 bytes
            switch (_shapeType)
            { 
                case enumShapeType.Point:
                    return size + 16; //x,y
                case enumShapeType.Polyline:
                    return size + 1000 * 16;//1000 points
                case enumShapeType.Polygon:
                    return size + 2 * 1000 * 16;//2 rings
            }
            return size;
        }

        private void AutoLeveling(Feature[] features)
        {
            if (_argOfLeveling != null && _argOfLeveling.Enabled && _shapeType == enumShapeType.Point)
            {
                //int bTime = Environment.TickCount;
                using (LevelAdjuster set = new LevelAdjuster())
                {
                    set.BeginLevel = _argOfLeveling.BeginLevel;
                    set.GridSize = _argOfLeveling.GridSize;
                    set.Features = features.ToArray();
                    set.Do();
                }
                //int eTime = Environment.TickCount - bTime;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("DataSource");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            obj.AddAttribute("name", _name);
            obj.AddAttribute("uri", _uri != null ? _uri : string.Empty);
            if (_instanceIdentify != null)
                obj.AddSubNode(_instanceIdentify.ToPersistObject());
            if (_fetclassIdentify != null)
                obj.AddSubNode(_fetclassIdentify.ToPersistObject());
            if (_argOfLeveling != null)
                obj.AddSubNode(_argOfLeveling.ToPersistObject());
            return obj;
        }

        public static IServerDataSource FromXElement(XElement xele)
        {
            if (xele == null)
                return null;
            string name = xele.Attribute("name").Value;
            string uri = xele.Attribute("uri").Value;
            InstanceIdentify instanceId = InstanceIdentify.FromXElement(xele.Element("InstanceIdentify"));
            FetClassIdentify fetclassId = FetClassIdentify.FromXElement(xele.Element("FetClassIdentify"));
            ServerDataSource ds = new ServerDataSource(name, uri, instanceId, fetclassId);
            if (xele.Element("ArgsOfLeveling") != null)
                ds.SetArgOfLevel(ArgOfLeveling.FromXElement(xele.Element("ArgsOfLeveling")));
            return ds;
        }
    }
}
