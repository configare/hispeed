using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CodeCell.AgileMap.WebComponent
{
    public class Map:AgileMapEntity
    {
        private string _name = null;
        private string _spatialRef = null;
        private ObservableCollection<Layer> _layers = new ObservableCollection<Layer>();
        private Canvas _canvas = null;
        private IMapServerAgent _serverAgent = null;
        private IMapControl _mapControl = null;

        internal Map(string name, Canvas canvas,IMapServerAgent agent,IMapControl mapControl)
            : base()
        {
            _name = name;
            _canvas = canvas;
            _serverAgent = agent;
            _mapControl = mapControl;
            _layers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_layers_CollectionChanged);
        }

        void _layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems.Count > 0)
            {
                foreach (Layer lyr in _layers)
                {
                    lyr.Map = this;
                }
            }
        }

        public IMapControl MapControl
        {
            get { return _mapControl; }
        }

        /// <summary>
        /// 创建动态图层(临时图层)并添加到图层列表中
        /// </summary>
        /// <param name="name">动态图层的名称</param>
        /// <returns>创建的动态图层</returns>
        public DynamicFeatureLayer CreateDynamicFeatureLayer(string name)
        {
            DynamicFeatureLayer dLyr =  new DynamicFeatureLayer(_canvas);
            dLyr.Name = name;
            _layers.Add(dLyr);
            return dLyr;
        }

        public IMapServerAgent ServerAgent
        {
            get { return _serverAgent; }
        }

        public string Name
        {
            get { return _name; }
            set 
            {
                _name = value;
                NotifyPropertyChange("Name");
            }
        }

        public string SpatialRef
        {
            get { return _spatialRef; }
            set 
            {
                _spatialRef = value;
                NotifyPropertyChange("SpatialRef");
            }
        }

        public ObservableCollection<Layer> Layers
        {
            get { return _layers; }
        }
    }
}
