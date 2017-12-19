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
    public partial class UCFeatureRendererComposite : UCFeatureRendererControlBase
    {
        private class SymbolNamePair
        {
            public ISymbol Symbol = null;
            public string Name = null;

            public SymbolNamePair(ISymbol symbol, string name)
            {
                Symbol = symbol;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private int _idx = 0;

        public UCFeatureRendererComposite()
        {
            InitializeComponent();
            Load += new EventHandler(UCFeatureRendererComposite_Load);
        }

        void UCFeatureRendererComposite_Load(object sender, EventArgs e)
        {
            if (Application.StartupPath.Contains("IDE"))
                return;
            IFeatureClass fetclass = _layer.Class as IFeatureClass;
            comboBox1.DataSource = SymbolTypeItemProvider.GetSymbolTypeItemsByShapeType(fetclass.ShapeType);
            comboBox1.SelectedIndex = 0;
        }

        protected override void SetLayerAfter()
        {
            if (_layer == null)
                return ;
            ISymbol _symbol = null;
            if(_layer.Renderer is SimpleFeatureRenderer)
                _symbol = (_layer.Renderer as SimpleFeatureRenderer).Symbol;
            if (_symbol == null || !(_symbol is ICompositeSymbol))
            {
                _symbol = new CompositeSymbol();
            }
            if (_layer.Renderer != null)
                _layer.Renderer.Dispose();
            _layer.Renderer = new SimpleFeatureRenderer(_symbol);
            LoadOldSymbols(_symbol as ICompositeSymbol);
        }

        private void LoadOldSymbols(ICompositeSymbol symbol)
        {
            if (symbol == null || symbol.Symbols == null || symbol.Symbols.Count == 0)
                return;
            foreach (ISymbol sym in symbol.Symbols)
            {
                listBox1.Items.Add(new SymbolNamePair(sym, "符号(" + _idx.ToString() + ")"));
                _idx++;
            }
        }

        protected override IFeatureRenderer Apply()
        {
            ICompositeSymbol sym = (_layer.Renderer as SimpleFeatureRenderer).Symbol as ICompositeSymbol;
            if (listBox1.Items.Count > 0)
            {
                if (sym == null)
                    sym = new CompositeSymbol();
                else
                    sym.Symbols.Clear();
                foreach (SymbolNamePair p in listBox1.Items)
                {
                    sym.Symbols.Add(p.Symbol);
                }
            }
            return _layer.Renderer;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
                return;
            object sym = Activator.CreateInstance((comboBox1.SelectedItem as SymbolTypeItem).SymbolType);
            listBox1.Items.Add(new SymbolNamePair(sym as ISymbol,"符号("+_idx.ToString()+")"));
            _idx++;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndices.Count == 0)
                return;
            for (int i = listBox1.SelectedIndices.Count - 1; i >= 0; i--)
                listBox1.Items.Remove(listBox1.SelectedItems[i]);
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndices.Count == 0)
                return;
            propertyGrid1.SelectedObject = (listBox1.SelectedItem as SymbolNamePair).Symbol;
        }
    }
}
