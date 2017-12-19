using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.UIs;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Components
{
    public class cmdQueryBox:BaseControlItem
    {
        private ToolStripComboBox _cb = null;
        private IMapControl _mapControl = null;

        public cmdQueryBox()
            : base()
        {
            Init();
        }

        public cmdQueryBox(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _control = new ToolStripComboBox();
            _cb = _control as ToolStripComboBox;
            _cb.SelectedIndexChanged += new EventHandler(_cb_SelectedIndexChanged);
            _cb.KeyDown += new KeyEventHandler(_cb_KeyDown);
            _cb.Size = new System.Drawing.Size(200, _cb.Height);
        }

        void _cb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                HanleQueryKey(_cb.Text);
            }
        }

        void _cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            HanleQueryKey(_cb.Text);
        }

        public override void Init(IHook hook)
        {
            base.Init(hook);
            _mapControl = (hook as IHookOfAgileMap).MapControl;
        }

        private void HanleQueryKey(string keys)
        {
            PointF ptf = GeoCoordHelper.ExtractGeoCoord(keys);
            ShapePoint pt = new ShapePoint(ptf.X, ptf.Y);
            if (!(pt.X < double.Epsilon && pt.Y < double.Epsilon))
                GotoPoint(pt);
            else
            {
                IQueryResultContainer resultContainer = GetQueryResultContainer();
                if (resultContainer == null)
                {
                    MsgBox.ShowError("没有设置查询结果容器,无法进行查询。");
                    return;
                }
                AsyncQueryPlacements querydel = new AsyncQueryPlacements(QueryPlacements);
                resultContainer.BeginInvokeContiner.BeginInvoke(querydel, keys);
            }
        }

        private delegate void AsyncQueryPlacements(string keys);

        private void GotoPoint(ShapePoint pt)
        {
            ShapePoint pt0 = pt.Clone() as ShapePoint;
            _mapControl.PanTo(pt);
            _mapControl.ReRender(delegate() 
                                         {
                                             _mapControl.CoordinateTransfrom.GeoCoord2PrjCoord(new ShapePoint[] { pt0 });
                                             _mapControl.MapRuntime.LocatingFocusLayer.Focus(pt, 6, 500);
                                         }
                                 );
        }

        private void QueryPlacements(string keys)
        {
            if (_mapControl.Map == null || _mapControl.Map.LayerContainer.Layers.Length == 0)
                return;
            Application.DoEvents();
            IQueryResultContainer resultContainer = GetQueryResultContainer();
            resultContainer.ResultContainerVisible = true;
            string[] layers = GetLayers(ref keys);
            foreach (string lyrname in layers)
            {
                AsyncQueryFeaturesDelegate querydel = new AsyncQueryFeaturesDelegate(QueryFeatures);
                resultContainer.BeginInvokeContiner.BeginInvoke(querydel, keys, lyrname);
            }
        }

        private delegate void AsyncQueryFeaturesDelegate(string keys, string lyrname);
        private void QueryFeatures(string keys, string lyrname)
        {
            IQueryResultContainer resultContainer = GetQueryResultContainer();
            IFeatureLayer lyr = _mapControl.Map.LayerContainer.GetLayerByName(lyrname) as IFeatureLayer;
            if (lyr == null)
                return;
            IFeatureClass fetclass = lyr.Class as IFeatureClass;
            Feature[] fs = (fetclass.DataSource as IFeatureDataSource).Query(new QueryFilter(keys));
            resultContainer.AddFeatures(fs);
        }

        private IQueryResultContainer GetQueryResultContainer()
        {
            IQueryResultContainer rstc = _mapControl.MapRuntime.QueryResultContainer;
            return rstc;
        }
  
        private string[] GetLayers(ref string keys)
        {
            string exp = @"LAYER:(?<LAYER>\S+)$";
            Match m = Regex.Match(keys, exp, RegexOptions.IgnoreCase);
            string[] layers = null;
            if (m.Success)
            {
                string strLayer = m.Groups["LAYER"].Value;
                keys = Regex.Replace(keys, exp, string.Empty, RegexOptions.IgnoreCase);
                if (strLayer.ToUpper() == "VISIBLE" || strLayer.Contains("可见"))
                {
                    return GetVisibleLayers();
                }
                else if (strLayer.ToUpper() == "ALL" || strLayer.Contains("所有"))
                {
                    return GetAllLayers();
                }
                else
                {
                    layers = m.Groups["LAYER"].Value.Split(',');
                }
            }
            else
            {
                layers = GetAllLayers();
            }
            return layers;
        }

        private string[] GetAllLayers()
        {
            List<string> lys = new List<string>();
            foreach (IFeatureLayer lyr in _mapControl.Map.LayerContainer.FeatureLayers)
            {
                lys.Add(lyr.Name);
            }
            return lys.Count > 0 ? lys.ToArray() : null;
        }

        private string[] GetVisibleLayers()
        {
            List<string> lys = new List<string>();
            foreach (IFeatureLayer lyr in _mapControl.Map.LayerContainer.FeatureLayers)
            {
                if((lyr as ILayerDrawable).Visible)
                    lys.Add(lyr.Name);
            }
            return lys.Count > 0 ? lys.ToArray() : null;
        }
    }
}
