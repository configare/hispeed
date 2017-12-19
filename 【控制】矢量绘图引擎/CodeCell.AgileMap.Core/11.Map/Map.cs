using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Core
{
    public class Map:IMap,IDisposable,IPersistable
    {
        protected string _name = string.Empty;
        protected string _url = string.Empty;
        protected MapVersion _version = null;
        protected MapAuthor _author = null;
        protected MapArguments _arguments = null;
        protected ILayerContainer _layerContainer = null;
        protected ILayer[] _inputLayers = null;
        protected ConflictDefinition _conflictDefForSymbol = null;
        protected ConflictDefinition _conflictDefForLabel = null;
        private IMapRuntime _mapRuntime = null;

        public Map()
        {
            _version = new MapVersion();
            _author = new MapAuthor();
            _arguments = new MapArguments();
            _conflictDefForSymbol = new ConflictDefinition();
            _conflictDefForLabel = new ConflictDefinition(new Size(60, 16), true);
        }

        public Map(ILayer[] layers)
        {
            _version = new MapVersion();
            _author = new MapAuthor();
            _arguments = new MapArguments();
            _conflictDefForSymbol = new ConflictDefinition();
            _conflictDefForLabel = new ConflictDefinition(new Size(60, 16), true);
            _inputLayers = layers;
        }

        public Map(string name, string url)
        {
            _name = name;
            _url = url;
            _version = new MapVersion();
            _author = new MapAuthor();
            _arguments = new MapArguments();
            _conflictDefForSymbol = new ConflictDefinition();
            _conflictDefForLabel = new ConflictDefinition(new Size(60, 16), true);
        }

        public Map(string name, string url,IFeatureLayer[] layers)
            : this(name, url)
        {
            _inputLayers = layers;
            _conflictDefForSymbol = new ConflictDefinition();
            _conflictDefForLabel = new ConflictDefinition(new Size(60, 16), true);
        }
         
        public Map(string name,string url,MapVersion version, MapAuthor author,MapArguments arg)
        {
            _name = name;
            _url = url;
            _version = version;
            _author = author;
            if (arg == null)
                arg = new MapArguments();
            _arguments = arg;
            _conflictDefForSymbol = new ConflictDefinition();
            _conflictDefForLabel = new ConflictDefinition(new Size(60, 16), true);
        }

        public Map(string name, string url, MapVersion version, MapAuthor author,MapArguments arg,ILayer[] layers)
            : this(name, url, version, author,arg)
        {
            _inputLayers = layers;
            _conflictDefForSymbol = new ConflictDefinition();
            _conflictDefForLabel = new ConflictDefinition(new Size(60, 16), true);
        }

        internal void InternalInit(IProjectionTransform projectionTransform, IFeatureRenderEnvironment environment)
        {
            _mapRuntime = environment as IMapRuntime;
            _layerContainer = new LayerContainer(projectionTransform, environment);
        }

        internal void InternalApplyLayers()
        {
            if (_inputLayers != null && _inputLayers.Length > 0)
            {
                foreach (ILayer layer in _inputLayers)
                    _layerContainer.Append(layer);
            }
        }

        internal void SetConflictDefinition(ConflictDefinition sym,ConflictDefinition label)
        {
            if (sym != null)
                _conflictDefForSymbol = sym;
            if (label != null)
                _conflictDefForLabel = label;
        }
    
        #region IMap Members

        [DisplayName("地图名称"), ReadOnly(true)]
        public string Name
        {
            get { return _name != null ? _name : ToString(); }
            set { _name = value; }
        }

        [DisplayName("配置文件"), ReadOnly(true)]
        public string Url
        {
            get { return _url != null ? _url : string.Empty; }
        }

        [DisplayName("地图参数"), TypeConverter(typeof(ExpandableObjectConverter))]
        public MapArguments MapArguments
        {
            get { return _arguments; }
        }

        [DisplayName("符号冲突检测"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ConflictDefinition ConflictDefForSymbol
        {
            get { return _conflictDefForSymbol; }
        }

        [DisplayName("标注冲突检测"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ConflictDefinition ConflictDefForLabel
        {
            get { return _conflictDefForLabel; }
        }

        [DisplayName("地图版本"), TypeConverter(typeof(ExpandableObjectConverter))]
        public MapVersion Version
        {
            get { return _version; }
        }

        [DisplayName("地图作者"), TypeConverter(typeof(ExpandableObjectConverter))]
        public MapAuthor Author
        {
            get { return _author; }
        }

        [Browsable(false)]
        public ILayerContainer LayerContainer
        {
            get 
            {
                if (_layerContainer == null)
                    throw new Exception("访问LayerContainer属性之前必须将Map对象应用到VectorMapRuntime。");
                return _layerContainer; 
            }
        }

        [Browsable(false)]
        public IMapRefresh MapRefresh
        {
            get { return this as IMapRefresh; }
        }

        internal static bool UseRelativePath = false;
        internal static string CurrentFilename = null;

        internal static string GetRelativeFilename(string filename)
        {
            if (!UseRelativePath)
                return filename;
            return RelativePathHelper.GetRelativePath(CurrentFilename, filename);
        }

        public void SaveTo(string filename, bool useRelativePath)
        {
            if (_arguments != null)
            {
                _arguments.Extent = _mapRuntime.Host.ExtentGeo;
            }
            CurrentFilename = filename;
            UseRelativePath = useRelativePath;
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("要保存地图的文件名为空。");
            try
            {
                SaveMapToXML(filename);
            }
            catch(Exception ex)
            {
                Log.WriterException("Map", "SaveToFile", ex);
                throw;
            }
        }

        private void SaveMapToXML(string filename)
        {
            using (MapToXML save = new MapToXML())
            {
                save.SaveTo(this as IMap, filename);
            }
        }

        public Envelope GetFullEnvelope()
        {
            if (_layerContainer == null || _layerContainer.IsEmpty())
                return null;
            Envelope evp = null;
            for (int i = 0; i < _layerContainer.Layers.Length; i++)
            {
                IClass fetclass = _layerContainer.Layers[i].Class as IClass;
                if (fetclass.IsEmpty())
                    continue;
                if(evp == null)
                    evp = fetclass.FullEnvelope.Clone() as Envelope;
                evp.UnionWith(fetclass.FullEnvelope);
            }
            return evp;
        }


        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_layerContainer != null)
            {
                _layerContainer.Dispose();
                //_layerContainer = null;
            }
        }

        #endregion

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Map");
            PersistObject authorObj = (_author as IPersistable).ToPersistObject();
            PersistObject versionObj = (_version as IPersistable).ToPersistObject();
            PersistObject argObj = (_arguments as IPersistable).ToPersistObject();
            obj.AddSubNode(argObj);
            //
            PersistObject cobjs = new PersistObject("ConflictDefinitionSymbol");
            obj.AddSubNode(cobjs);
            cobjs.AddSubNode((_conflictDefForSymbol).ToPersistObject());
            //
            cobjs = new PersistObject("ConflictDefinitionLabel");
            obj.AddSubNode(cobjs);
            cobjs.AddSubNode((_conflictDefForLabel).ToPersistObject());
            //
            obj.AddSubNode(authorObj);
            obj.AddSubNode(versionObj);
            obj.AddAttribute("name", _name);
            //layers
            PersistObject lyrObj = new PersistObject("Layers");
            obj.AddSubNode(lyrObj);
            if (_layerContainer != null && _layerContainer.Layers != null)
            {
                foreach (ILayer lyr in _layerContainer.Layers)
                {
                    lyrObj.AddSubNode((lyr as IPersistable).ToPersistObject());
                }
            }
            return obj;
        }

        #endregion
    }
}
