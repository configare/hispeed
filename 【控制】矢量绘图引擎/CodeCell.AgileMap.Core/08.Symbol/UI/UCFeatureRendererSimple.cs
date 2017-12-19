using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Core
{
    public partial class UCFeatureRendererSimple : UCFeatureRendererControlBase
    {
        public UCFeatureRendererSimple()
        {
            InitializeComponent();
            Load += new EventHandler(UCFeatureRendererSimple_Load);
        }

        void UCFeatureRendererSimple_Load(object sender, EventArgs e)
        {
            if (Application.StartupPath.Contains("IDE"))
                return;
            IFeatureClass fetclass = _layer.Class as IFeatureClass;
            comboBox1.DataSource = SymbolTypeItemProvider.GetSymbolTypeItemsByShapeType(fetclass.ShapeType);
        }

        protected override void SetLayerAfter()
        {
            if (_layer == null)
                return ;
            ISymbol _symbol = null;
            if(_layer.Renderer is SimpleFeatureRenderer)
                _symbol = (_layer.Renderer as SimpleFeatureRenderer).Symbol;
            if (_symbol == null || _symbol is ICompositeSymbol)
            {
                IFeatureClass fetclass = _layer.Class as IFeatureClass;
                switch (fetclass.ShapeType)
                {
                    case enumShapeType.Point:
                        _symbol = new SimpleMarkerSymbol();
                        break;
                    case enumShapeType.Polyline:
                        _symbol = new SimpleLineSymbol();
                        break;
                    case enumShapeType.Polygon:
                        _symbol = new SimpleFillSymbol();
                        break;
                    default:
                        _symbol = null;
                        break;
                }
            }
            if (_layer.Renderer != null)
                _layer.Renderer.Dispose();
            _layer.Renderer = new SimpleFeatureRenderer(_symbol);
            if (_layer.Renderer != null)
                propertyGrid1.SelectedObject = _layer.Renderer;
        }

        protected override IFeatureRenderer Apply()
        {
            return _layer.Renderer;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
                return;
            ISymbol _symbol = null;
            if (_layer.Renderer is SimpleFeatureRenderer)
            {
                _symbol = Activator.CreateInstance((comboBox1.SelectedItem as SymbolTypeItem).SymbolType) as ISymbol;
                (_layer.Renderer as SimpleFeatureRenderer).Symbol = _symbol;
                propertyGrid1.SelectedObject = _symbol;
            }
        }
    }
}
