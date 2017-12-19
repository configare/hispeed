using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.UIs;
using CodeCell.AgileMap.Core;


namespace CodeCell.AgileMap.Express
{
    public partial class UCQueryResultContainer : UserControl
    {
        private ILocationService _locationSrv = null;
        private ILocationIconLayer _locationIconLayer = null;
        private IMapRefresh _maprefresh = null;

        public UCQueryResultContainer()
        {
            InitializeComponent();
        }

        public void Init(IMapRuntime mapruntime)
        {
            _locationSrv = mapruntime.LocationService;
            _locationIconLayer = mapruntime.LocationIconLayer;
            _maprefresh = mapruntime.MapRefresh;
        }

        public void Clear()
        {
            ucHierarchicalListBox1.Items.Clear();
            ucHierarchicalListBox1.Invalidate();
        }

        public void AddFeatures(Feature[] features)
        {
            if (features == null || features.Length == 0)
                return;
            foreach (Feature fet in features)
            {
                HierItem it = FeatureToHierItem(fet);
                if(it != null)
                    ucHierarchicalListBox1.Items.Add(it);
            }
            ucHierarchicalListBox1.Refresh();
        }

        private HierItem FeatureToHierItem(Feature fet)
        {
            if (fet == null)
                return null;
            HierItem it = new HierItem(GetTitleByFeature(fet));
            it.Tag = fet;
            it.Properties.Add("图层", fet.FeatureClass.Name);
            if (fet.FieldNames != null && fet.FieldNames.Length > 0)
                for (int i = 0; i < fet.FieldNames.Length; i++)
                    it.Properties.Add(fet.FieldNames[i], fet.FieldValues[i]);
            return it;
        }

        private string[] FixedNameFields = new string[] 
                                           {
                                               "NAME",
                                               "名称",
                                               "地址",
                                               "编号",
                                               "编码",
                                               "CODE"
                                           };
        private string GetTitleByFeature(Feature fet)
        {
            if (fet.FieldNames != null && fet.FieldNames.Length > 0)
            {
                foreach (string fld in fet.FieldNames)
                {
                    string ufld = fld.ToUpper();
                    foreach (string f in FixedNameFields)
                        if (ufld.Contains(f))
                            return fet.GetFieldValue(fld); 
                }
            }
            return "OID:" + fet.OID.ToString();
        }

        private void ucHierarchicalListBox1_OnClickHierItem(object sender, HierItem hierItem)
        {
            Feature fet = hierItem.Tag as Feature;
            _locationSrv.GotoFeature(fet);
        }

        private void ucHierarchicalListBox1_OnAfterDrawHierItems(object sender, HierItem[] hierItems)
        {
            _locationIconLayer.Clear();
            if (hierItems == null || hierItems.Length == 0)
            {
                _maprefresh.Render();
                return;
            }
            for (int i = 0; i < hierItems.Length; i++)
            {
                _locationIconLayer.Add(new LocationIcon((i + 1).ToString(), hierItems[i].Tag as Feature));
            }
            _maprefresh.Render();
        }
    }
}
