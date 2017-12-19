using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using GeoDo.Core;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public class CanvasViewerLayerProviderFacader : ILayerProviderFacader
    {
        private ICanvasViewer _viewer = null;
        private string _name = null;
        private string _text = null;
        private List<ILayerItem> _layerItems = null;
        private bool _isSelected = false;
        private bool _isAllowSelectable = false;
        private bool _isAllowVisiable = false;
        private bool _isVisiable = false;
        private object _tag = null;

        public CanvasViewerLayerProviderFacader(ICanvasViewer viewer)
        {
            _viewer = viewer;
            _layerItems = new List<ILayerItem>();
            GetItems(viewer.Canvas);
        }

        public List<ILayerItem> Items
        {
            get { return _layerItems; }
            set
            {
                 _layerItems = value;
            }
        }

        public string SupportedFilters
        {
            get { return "*"; }
        }

        public void Update()
        {
            this.Items.Clear();
            GetItems(_viewer.Canvas);
        }

        public ILayerItem FindParent(ILayerItem item)
        {
            throw new NotImplementedException();
        }

        public Image Image
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAllowSelectable
        {
            get { return _isAllowSelectable; }
        }

        public bool IsAllowVisiable
        {
            get { return _isAllowVisiable; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public bool IsVisible
        {
            get { return _isVisiable; }
            set { _isVisiable = value; }
        }

        public enumLayerTypes LayerType
        {
            get { return enumLayerTypes.Unknow; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public object Tag
        {
            get { return _tag; }
        }

        public string Text
        {
            get { return _text; }
        }


        public ILayerItem AddLayer(string fname)
        {
            ILayerItem newLayer = new LayerItemCanvasViewer();
            newLayer.Name = fname;
            this.Items.Add(newLayer);
            return newLayer;
        }

        public void AdjustOrder(int insertIndex, ILayerItem item)
        {
            if (item.Tag is CodeCell.AgileMap.Core.ILayer)
            {
                foreach (GeoDo.RSS.Core.DrawEngine.ILayer layer in _viewer.Canvas.LayerContainer.Layers)
                    if (layer is IVectorHostLayer)
                    {
                        IVectorHostLayer hostLayer = layer as IVectorHostLayer;
                        IMap map = hostLayer.Map as IMap;
                        CodeCell.AgileMap.Core.ILayer lyr = item.Tag as CodeCell.AgileMap.Core.ILayer;
                        map.LayerContainer.AdjustOrder(lyr, map.LayerContainer.Layers[insertIndex]);               
                       foreach (ILayerItem it in Items)
                       {
                          if (it is ILayerItemGroup && it.Tag is IVectorHostLayer)
                          {
                            (it as ILayerItemGroup).Items.Remove(item);
                            (it as ILayerItemGroup).Items.Insert(insertIndex, item);
                          }
                       }
                    break;
                   }
            }
            if (item.Tag is GeoDo.RSS.Core.DrawEngine.IRenderLayer||item.Tag is IVectorHostLayer)
            {
                this.Items.Remove(item);
                this.Items.Insert(insertIndex, item);
                _viewer.Canvas.LayerContainer.Layers.Remove(item.Tag as GeoDo.RSS.Core.DrawEngine.ILayer);
                _viewer.Canvas.LayerContainer.Layers.Insert(insertIndex, item.Tag as GeoDo.RSS.Core.DrawEngine.ILayer);
            }
            RefreshViewer();
        }

        public void Remove(ILayerItem item)
        {
            if (item is ILayerItemGroup)
            {
                foreach (ILayerItem it in (item as ILayerItemGroup).Items)
                {
                    this.Items.Remove(it);
                }
                if (item.Tag is IVectorHostLayer)
                {
                    IVectorHostLayer hostLayer = item.Tag as IVectorHostLayer;
                    IMap map = hostLayer.Map as IMap;
                    if (map.LayerContainer.Layers != null && map.LayerContainer.Layers.Count() != 0)
                    {
                        foreach (CodeCell.AgileMap.Core.ILayer it in map.LayerContainer.Layers)
                        {
                            map.LayerContainer.Remove(it);
                        }
                    }
                    Update();
                }
            }
            if (item.Tag is GeoDo.RSS.Core.DrawEngine.IRenderLayer)
            {
                if (item.Tag is GeoDo.RSS.Core.DrawEngine.IRasterLayer)
                    return;
                else if (item.Tag is GeoDo.RSS.Core.DrawEngine.IVectorLayer)
                {
                    IRenderLayer layer = item.Tag as IRenderLayer;
                    _viewer.Canvas.LayerContainer.Layers.Remove(item.Tag as IRenderLayer);
                    this.Items.Remove(item);
                    return;
                }  
            }
            if (item.Tag is CodeCell.AgileMap.Core.ILayer)
            {
                foreach (GeoDo.RSS.Core.DrawEngine.ILayer layer in _viewer.Canvas.LayerContainer.Layers)
                {
                    if (layer is IVectorHostLayer)
                    {
                        IVectorHostLayer hostLayer = layer as IVectorHostLayer;
                        IMap map = hostLayer.Map as IMap;
                        map.LayerContainer.Remove(item.Tag as CodeCell.AgileMap.Core.ILayer);
                        Update();
                    }
                }
                this.Items.Remove(item);
            }
            RefreshViewer();
        }

        public void Group(ILayerItem item, ILayerItemGroup group)
        {
            group.Items.Add(item);
        }

        private void GetItems(ICanvas canvas)
        {
            if (canvas == null || canvas.LayerContainer == null)
                return;
            foreach (GeoDo.RSS.Core.DrawEngine.ILayer layer in canvas.LayerContainer.Layers)
            {
                if (layer is IVectorHostLayer)
                {
                    IVectorHostLayer hostLayer = layer as IVectorHostLayer;
                    IMap map = hostLayer.Map as IMap;
                    if (map != null)
                    {
                        LayerItemGroup vectorGroup = new LayerItemGroup("矢量层", "矢量层");
                        vectorGroup.IsVisible = true;
                        /*
                         * 为了能够在层管理器中修改整个地图的参数将tag设置为map对象
                         */
                        //vectorGroup.Tag = layer;
                        vectorGroup.Tag = map;
                        if (map.LayerContainer.Layers != null && map.LayerContainer.Layers.Count() != 0)
                        {
                            foreach (CodeCell.AgileMap.Core.ILayer it in map.LayerContainer.Layers)
                            {
                                LayerItemCanvasViewer item = new LayerItemCanvasViewer(enumLayerTypes.BaseVector, null);
                                item.Name = it.Name;
                                item.IsVisible = it.IsRendered;
                                item.Tag = it;
                                vectorGroup.Items.Add(item);
                            }
                        }
                        this.Items.Add(vectorGroup);
                    }
                }
                else if (layer is IRenderLayer)
                {
                    LayerItemCanvasViewer item = new LayerItemCanvasViewer(enumLayerTypes.Raster, null);
                    item.Name = layer.Name;
                    item.IsVisible = (layer as IRenderLayer).Visible;
                    item.IsSelected = true;
                    item.Tag = layer;
                    this.Items.Add(item);
                }
            }
        }

        public void RefreshViewer()
        {
            this._viewer.Canvas.Refresh(enumRefreshType.All);
        }

        public void SetLayerName(string newName, ILayerItem layer)
        {
            if (layer.Tag is CodeCell.AgileMap.Core.ILayer)
            {
                (layer.Tag as CodeCell.AgileMap.Core.ILayer).Name = newName;
                layer.Name = newName;
            }
            else if (layer.Tag is IVectorHostLayer)
            {
                layer.Name = newName;
            }
            else if (layer.Tag is IRenderLayer)
            {
                layer.Name = newName;
            }
        }


        public void AdjustOrder(int insertIndex, ILayerItem item, ILayerItem parentItem)
        {
            
        }


        public void SaveVectorItemShowMethod(ILayerItem item)
        {
            if (item.Tag is CodeCell.AgileMap.Core.FeatureLayer)
            {
                FeatureLayer layer = item.Tag as FeatureLayer;
                if (layer != null)
                {
                    FileDataSource dataSource = layer.Class.DataSource as FileDataSource;
                    if (dataSource == null)
                        return;
                    string fileUrl = dataSource.FileUrl;
                    if (string.IsNullOrEmpty(fileUrl))
                        return;
                    string sourceFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\MapTemplate.mcd");
                    if (!File.Exists(sourceFileName))
                        return;
                    string shpName = Path.GetFileNameWithoutExtension(fileUrl);
                    string newFileName = Path.Combine(Path.GetDirectoryName(fileUrl), shpName + ".mcd");
                    File.Copy(sourceFileName, newFileName, true);
                    XmlDocument doc =new XmlDocument();
                    doc.Load(newFileName);
                    XmlNode layerNodes = doc.SelectSingleNode("Map").SelectSingleNode("Layers");
                    if (layerNodes == null)
                        return;
                    IPersistable persist = layer as IPersistable;
                    PersistObject layerObj = persist.ToPersistObject();
                    XmlNode layerNode = GetXmlNode(doc, layerObj);
                    layerNodes.AppendChild(layerNode);
                    if (layerObj.SubNodes != null)
                    {
                        foreach (PersistObject sub in layerObj.SubNodes)
                        {
                            PersistObjectToXmlNode(doc, layerNode, sub);
                        }
                    }
                    doc.Save(newFileName);
                }
            }
        }

        private void PersistObjectToXmlNode(XmlDocument doc, XmlNode parentNode, PersistObject obj)
        {
            if (obj == null)
                return;
            XmlNode node = GetXmlNode(doc, obj);
            parentNode.AppendChild(node);
            if (obj.SubNodes != null)
                foreach (PersistObject subobj in obj.SubNodes)
                    PersistObjectToXmlNode(doc, node, subobj);
        }

        private XmlNode GetXmlNode(XmlDocument doc, PersistObject obj)
        {
            if (obj == null)
                return null;
            XmlNode node = doc.CreateNode(XmlNodeType.Element, obj.Name, null);
            if (obj.Attributes != null)
            {
                foreach (KeyValuePair<string, string> att in obj.Attributes)
                {
                    if (string.IsNullOrEmpty(att.Key))
                        continue;
                    XmlAttribute attNode = doc.CreateAttribute(att.Key);
                    if (att.Value == null)
                        attNode.Value = string.Empty;
                    else
                        attNode.Value = att.Value;
                    node.Attributes.Append(attNode);
                }
            }
            return node;
        }
    }
}
