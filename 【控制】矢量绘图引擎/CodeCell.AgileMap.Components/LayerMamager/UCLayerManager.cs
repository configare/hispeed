using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.UIs;


namespace CodeCell.AgileMap.Components
{
    public partial class UCLayerManager : UserControl
    {
        private IMap _map = null;

        public UCLayerManager()
        {
            InitializeComponent();
            ucLayersControl1.OnLayerItemClick += new UCLayersControl.OnLayerItemClickHandler(ucLayersControl1_OnLayerItemClick);
            ucLayersControl1.OnLayerItemEnabledChanged += new UCLayersControl.OnLayerItemEnabledChangedHandler(ucLayersControl1_OnLayerItemEnabledChanged);
            Resize += new EventHandler(UCLayerManager_Resize);
        }

        void UCLayerManager_Resize(object sender, EventArgs e)
        {
            panelBottom.Height = (int)(Height / 3) + 60;
        }

        void ucLayersControl1_OnLayerItemEnabledChanged(LayerItem layerItem)
        {
            IFeatureLayer lyr = layerItem.Tag as IFeatureLayer;
            if(lyr != null)
                (lyr as ILayerDrawable).Visible = !(lyr as ILayerDrawable).Visible;
        }

        void ucLayersControl1_OnLayerItemClick(LayerItem layerItem)
        {
            if (layerItem != null)
                propertyGrid1.SelectedObject = layerItem.Tag;
            else
                propertyGrid1.SelectedObject = null;
        }

        public bool VisiblePropertyWnd
        {
            get { return panelBottom.Visible; }
            set { panelBottom.Visible = value; }
        }

        public bool EnabledOrderAdjust
        {
            get { return ucLayersControl1.AllowDragLayerItem; }
            set { ucLayersControl1.AllowDragLayerItem = value; }
        }

        public void RemoveSelected()
        {
            if (ucLayersControl1.SelectedLayerItem == null)
                return;
            LayerItem lyrItem = ucLayersControl1.SelectedLayerItem;
            ucLayersControl1.RootLayerItem.Remove(lyrItem);
            ucLayersControl1.Invalidate();
            _map.LayerContainer.Remove(lyrItem.Tag as ILayer);
        }

        public void RefreshLayers()
        {
            Apply(_map);
        }

        public void Apply(IMap map)
        {
            try
            {
                _map = map;
                //
                ucLayersControl1.RootLayerItem.Clear();
                propertyGrid1.SelectedObject = null;
                //
                if (_map == null || _map.LayerContainer == null || _map.LayerContainer.Layers == null || _map.LayerContainer.Layers.Length == 0)
                {
                    return;
                }
                else
                {
                    LayerItem maproot = new LayerItem(_map.Name);
                    maproot.Tag = _map;
                    ucLayersControl1.RootLayerItem.Add(maproot);
                    foreach (ILayer layer in _map.LayerContainer.Layers)
                    {
                        LayerItem lyrItem = new LayerItem(layer.Name);
                        lyrItem.Enabled = (layer as ILayerDrawable).Visible;
                        lyrItem.Tag = layer;
                        maproot.Add(lyrItem);
                    }
                }
            }
            finally
            {
                Invalidate();
            }
        }
    }
}
