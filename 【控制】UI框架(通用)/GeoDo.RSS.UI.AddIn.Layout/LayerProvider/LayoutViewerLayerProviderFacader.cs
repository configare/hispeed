using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.DataFrm;
using System.Xml;
using CodeCell.AgileMap.Core;
using System.IO;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    public class LayoutViewerLayerProviderFacader : ILayerProviderFacader
    {
        private ILayoutViewer _viewer = null;
        private string _name = null;
        private string _text = null;
        private Image _image = null;
        private List<ILayerItem> _layerItems = null;
        private bool _isSelected = false;
        private bool _isAllowSelectable = false;
        private bool _isAllowVisiable = false;
        private bool _isVisiable = false;
        private object _tag = null;

        public LayoutViewerLayerProviderFacader(ILayoutViewer viewer)
        {
            _viewer = viewer;
            _layerItems = new List<ILayerItem>();
            GetItems(viewer);
        }

        public List<ILayerItem> Items
        {
            get { return _layerItems; }
        }

        public void Update()
        {
            this.Items.Clear();
            GetItems(_viewer);
        }

        public void Remove(ILayerItem item)
        {
            this.Items.Remove(item);
            _viewer.LayoutHost.LayoutRuntime.Layout.Elements.Remove(item.Tag as IElement);
            RefreshViewer();
        }

        //调整一级子节点顺序
        public void AdjustOrder(int insertIndex, ILayerItem item)
        {
            ILayerItem parent = FindLayerParent(item);
            if (parent is ILayersProvider)
            {
                this.Items.Remove(item);
                this.Items.Insert(insertIndex, item);
                _viewer.LayoutHost.LayoutRuntime.Layout.Elements.Remove(item.Tag as IElement);
                _viewer.LayoutHost.LayoutRuntime.Layout.Elements.Insert(insertIndex, item.Tag as IElement);
                RefreshViewer();
            }
            else
            {
                AdjustOrder(insertIndex, item, parent);
            }
        }

        public ILayerItem FindLayerParent(ILayerItem layer)
        {
            if (Items == null || Items.Count == 0)
                return null;
            foreach (ILayerItem item in Items)
            {
                if (layer.Equals(item))
                    return this;
                if (item is ILayerItemGroup)
                {
                    ILayerItem parent = (item as ILayerItemGroup).FindParent(layer);
                    if (parent != null)
                        return parent;
                }
            }
            return null;
        }

        //调整二级及以上节点顺序
        public void AdjustOrder(int insertIndex, ILayerItem item, ILayerItem parentItem)
        {
            if (parentItem == null)
                return;
            (parentItem as ILayerItemGroup).Items.Remove(item);
            (parentItem as ILayerItemGroup).Items.Insert(insertIndex, item);
            foreach (IElement ele in _viewer.LayoutHost.LayoutRuntime.Layout.Elements)
            {
                if (ele is ILayerObjectContainer)
                {
                    ILayerObjectContainer container = ele as ILayerObjectContainer;
                    //ILayerObjectBase insertObj = item.Tag as ILayerObjectBase;
                    foreach (ILayerObjectBase obj in container.LayerObjects)
                    {
                        if (obj is ILayerObject)
                        {
                            ILayerObject layerObj = obj as ILayerObject;
                            if (layerObj.Tag.Equals(item.Tag))
                            {
                                container.LayerObjects.Remove(layerObj);
                                container.LayerObjects.Insert(insertIndex, layerObj);
                                RefreshViewer();
                                return;
                            }
                        }
                        if (obj is ILayerObjecGroup)
                        {
                            ILayerObjecGroup layerGrp = obj as ILayerObjecGroup;
                            foreach (ILayerObjectBase it in layerGrp.Children)
                            {
                                if ((it as ILayerObject).Tag.Equals(item.Tag))
                                {
                                    layerGrp.Children.Remove(it);
                                    layerGrp.Children.Insert(insertIndex, it);
                                    RefreshViewer();
                                    return;
                                }
                            }
                        }
                    }
                }
                if (ele.Equals(parentItem) && ele is IElementGroup)
                {
                    IElementGroup group = ele as IElementGroup;
                    group.Elements.Remove(item.Tag as IElement);
                    group.Elements.Insert(insertIndex, item.Tag as IElement);
                    RefreshViewer();
                    return;
                }
            }
            
        }

        public void Group(ILayerItem item, ILayerItemGroup group)
        {
        }

        public string SupportedFilters
        {
            get { return "*"; }
        }

        public ILayerItem AddLayer(string fname)
        {
            ILayerItem newLayer = new LayerItem(fname, fname);
            this.Items.Add(newLayer);
            return newLayer;
        }

        public ILayerItem FindParent(ILayerItem item)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Text
        {
            get { return _text; }
        }

        public Image Image
        {
            get { return _image; }
        }

        public enumLayerTypes LayerType
        {
            get { return enumLayerTypes.Unknow; }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return _isVisiable;
            }
            set
            {
                _isVisiable = value;
            }
        }

        public bool IsAllowSelectable
        {
            get { return _isAllowSelectable; }
        }

        public bool IsAllowVisiable
        {
            get { return _isAllowVisiable; }
        }

        public object Tag
        {
            get { return _tag; }
        }

        private void GetItems(ILayoutViewer viewer)
        {
            foreach (IElement it in viewer.LayoutHost.LayoutRuntime.Layout.Elements)
            {
                ILayerItem item = GetLayerItemLayout(it);                  
                Items.Add(item);
            }
        }

        private ILayerItem GetLayerItemLayout(IElement it)
        {
            if (it is ILayerObjectContainer)
            {
                return GetLayerItemLayout(it as ILayerObjectContainer);
            }
            else if (it is IElementGroup)
            {
                return GetGroupLayerItems(it as IElementGroup);
            }
            else
            {
                LayerItemLayout item = new LayerItemLayout();
                item.Name = it.Name;
                item.IsVisible = it.Visible;
                item.IsSelected = it.IsSelected;
                item.Tag = it;
                if (it.Icon != null)
                    item.Image = it.Icon;
                return item;
            }
        }

        private ILayerItem GetLayerItemLayout(ILayerObjectContainer layerObjContainer)
        {
            ILayerItemGroup g = new LayerItemGroup("数据层", "数据层");
            (g as LayerItemGroup).Tag = layerObjContainer;
            if (layerObjContainer.LayerObjects != null && layerObjContainer.LayerObjects.Count != 0)
                GetLayerItemLayouts(g, layerObjContainer);
            return g;
        }

        private void GetLayerItemLayouts(ILayerItemGroup g, ILayerObjectContainer layerObjContainer)
        {
            ILayerObjectBase[] layers = layerObjContainer.LayerObjects.ToArray();
            foreach (ILayerObjectBase obj in layers)
            {
                if (obj is ILayerObject)
                {
                    ILayerObject layerObj=obj as ILayerObject;
                    ILayerItem it = new LayerObjectItem(layerObj);
                    g.Items.Add(it);
                }
                else
                {
                    LayerItemGroup group = new LayerItemGroup(obj.Text ?? string.Empty, obj.Text ?? string.Empty);
                    IDataFrame df = layerObjContainer as IDataFrame;
                    if (df != null)
                    {
                        IDataFrameDataProvider prd = df.Provider as IDataFrameDataProvider;
                        if (prd != null)
                        {
                            ICanvas canvas = prd.Canvas;
                            if (canvas != null)
                            {
                                GeoDo.RSS.Core.DrawEngine.ILayerContainer lc = canvas.LayerContainer as GeoDo.RSS.Core.DrawEngine.ILayerContainer;
                                if (lc != null)
                                {
                                    IVectorHostLayer hostLayer = canvas.LayerContainer.VectorHost;
                                    if (hostLayer != null)
                                    {
                                        group.Tag = hostLayer.Map;
                                    }
                                }
                            }
                        }
                    }
                    g.Items.Add(group);
                    GetLayerItemLayouts(group, (obj as ILayerObjecGroup).Children.ToArray());
                }
            }
        }

        private void GetLayerItemLayouts(ILayerItemGroup g, ILayerObjectBase[] objs)
        {
            foreach (ILayerObjectBase obj in objs)
            {
                if (obj is ILayerObject)
                {
                    ILayerObject layerObj = obj as ILayerObject;
                    ILayerItem it = new LayerObjectItem(layerObj);
                    g.Items.Add(it);
                }
                else
                {
                    ILayerItemGroup group = new LayerItemGroup(obj.Text ?? string.Empty, obj.Text ?? string.Empty);
                    g.Items.Add(group);
                    GetLayerItemLayouts(group, (obj as ILayerObjecGroup).Children.ToArray());
                }
            }
        }

        private ILayerItem GetGroupLayerItems(IElementGroup group)
        {
            ILayerItemGroup itemGroup = new LayerItemGroup(group.Name ?? string.Empty, group.Name ?? string.Empty);
            (itemGroup as LayerItemGroup).Tag = group;
            itemGroup.IsSelected = group.IsSelected;
            itemGroup.IsVisible = group.Visible;
            foreach (IElement it in group.Elements)
            {
                if (it is IElementGroup)
                {
                    ILayerItem layer = GetGroupLayerItems(it as IElementGroup);
                    itemGroup.Items.Add(layer);
                }
                else
                {
                    LayerItemLayout item = new LayerItemLayout();
                    item.Name = it.Name;
                    item.IsVisible = it.Visible;
                    item.IsSelected = it.IsSelected;
                    item.Tag = it;
                    if (it.Icon != null)
                        item.Image = it.Icon;
                    itemGroup.Items.Add(item);
                }
            }
            return itemGroup;
        }

        public void RefreshViewer()
        {
            this._viewer.LayoutHost.Render(true);
        }

        public void SetLayerName(string newName, ILayerItem layer)
        {
            if(layer.Tag is IElement)
                (layer.Tag as IElement).Name = newName;
            layer.Name = newName;
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
                    XmlDocument doc = new XmlDocument();
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
