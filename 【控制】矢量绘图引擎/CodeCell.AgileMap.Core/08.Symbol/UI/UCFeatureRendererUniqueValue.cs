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
    public partial class UCFeatureRendererUniqueValue : UCFeatureRendererControlBase
    {
        private string _field = null;
        private ISymbol _publicSybmobl = null;


        public UCFeatureRendererUniqueValue()
        {
            InitializeComponent();
            cbFields.SelectedIndexChanged += new EventHandler(cbFields_SelectedIndexChanged);
        }

        void cbFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            _field = cbFields.Text;
        }

        protected override void SetLayerAfter()
        {
            if (_layer == null)
                return;
            AddAllFields();
            LoadCurrentRenderer();
        }

        private void LoadCurrentRenderer()
        {
            if (_layer == null || _layer.Renderer == null || !(_layer.Renderer is UniqueFeatureRenderer))
                return;
            UniqueFeatureRenderer _render = _layer.Renderer as UniqueFeatureRenderer;
            if (_render.Symbol != null)
            {
                foreach (string fieldValue in _render.Symbol.Keys)
                {
                    if (_publicSybmobl == null)
                        _publicSybmobl = _render.Symbol[fieldValue];
                    ListViewItem it = new ListViewItem(fieldValue);
                    it.SubItems.Add(fieldValue);
                    it.SubItems.Add(string.Empty);
                    it.BackColor = _render.Symbol[fieldValue].Color;
                    it.Tag = fieldValue;
                    listView1.Items.Add(it);
                }
            }
            //
            cbFields.Text = _render.FieldName;
        }

        private void AddAllFields()
        {
            IFeatureClass fetclass = _layer.Class as IFeatureClass;
            Feature[] features = fetclass.GetVectorFeatures();
            if (features == null || features.Length == 0 || features[0].FieldNames == null)
                return;
            int idx = 0;
            int i = 0;
            foreach (string fld in features[0].FieldNames)
            {
                if (string.IsNullOrEmpty(fld))
                    continue;
                cbFields.Items.Add(fld.Trim());
                if (fld.ToUpper().Contains("NAME") || fld.Contains("名称"))
                    idx = i;
                i++;
            }
            if (cbFields.Items.Count > 0)
                cbFields.SelectedIndex = idx;
        }

        protected override IFeatureRenderer Apply()
        {
            if (listView1.Items.Count == 0)
                return _layer.Renderer;
            Dictionary<string, ISymbol> _symbols = new Dictionary<string, ISymbol>();
            foreach (ListViewItem it in listView1.Items)
            {
                _symbols.Add(it.Tag.ToString(), GetSymbolByColor(it.BackColor));
            }
            if (_layer.Renderer != null)
                _layer.Renderer.Dispose();
            return new UniqueFeatureRenderer(_field, _symbols);
        }

        private ISymbol GetSymbolByColor(Color color)
        {
            ISymbol sym = null;
            color = Color.FromArgb((byte)(255 - 255 * ((float)txtTranspert.Value / 100f)), color.R, color.G, color.B);
            IFeatureClass fetclass = _layer.Class as IFeatureClass;
            switch (fetclass.ShapeType)
            {
                case enumShapeType.Point:
                    sym = new SimpleMarkerSymbol();
                    break;
                case enumShapeType.Polyline:
                    sym = new SimpleLineSymbol();
                    ISimpleLineSymbol lineSym = sym as ISimpleLineSymbol;
                    if (_publicSybmobl != null)
                    {
                        ISimpleLineSymbol publicSym = _publicSybmobl as ISimpleLineSymbol;
                        lineSym.DashStyle = publicSym.DashStyle;
                        lineSym.LineJoin = publicSym.LineJoin;
                        lineSym.StartLineCap = publicSym.StartLineCap;
                        lineSym.EndLineCap = publicSym.EndLineCap;
                        if(publicSym.DashPattern != null)
                            lineSym.DashPattern = publicSym.DashPattern.Clone() as float[];
                        lineSym.Width = publicSym.Width;
                    }
                    break;
                case enumShapeType.Polygon:
                    if (_publicSybmobl != null)
                        sym = new SimpleFillSymbol(Color.Yellow, new SimpleLineSymbol((_publicSybmobl as IFillSymbol).OutlineSymbol.Color, (_publicSybmobl as IFillSymbol).OutlineSymbol.Width));
                    else
                        sym = new SimpleFillSymbol();
                    break;
                default:
                    sym = null;
                    break;
            }
            if (sym != null)
                sym.Color = color;
            return sym;
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            if (string.IsNullOrEmpty(_field))
                return;
            IFeatureClass fetclass = _layer.Class as IFeatureClass;
            Feature[] features = fetclass.GetVectorFeatures();
            if (features == null || features.Length == 0)
                return;
            Dictionary<string, int> _fieldValues = new Dictionary<string, int>();
            foreach (Feature fet in features)
            {
                string v = fet.GetFieldValue(_field);
                if (v == null)
                    continue;
                v = v.Trim();
                if (_fieldValues.ContainsKey(v))
                {
                    int count = _fieldValues[v]++;
                    _fieldValues.Remove(v);
                    _fieldValues.Add(v, count);
                }
                else
                {
                    _fieldValues.Add(v, 1);
                }
            }
            foreach (string v in _fieldValues.Keys)
            {
                ListViewItem it = new ListViewItem(v);
                it.Tag = v;
                it.BackColor = GetColorByValue(v);
                it.SubItems.Add(v);
                it.SubItems.Add(_fieldValues[v].ToString());
                listView1.Items.Add(it);
            }
        }

        private Random _random = new Random(1);
        private Color GetColorByValue(string v)
        {
            return Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255));
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem it = listView1.GetItemAt(e.X, e.Y);
            if (it != null)
            {
                using (ColorDialog dlg = new ColorDialog())
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        it.BackColor = dlg.Color;
                    }
                }
            }
        }

        private void txtTranspert_ValueChanged(object sender, EventArgs e)
        {
            if (txtTranspert.Value > 100)
                txtTranspert.Value = 100;
            else if (txtTranspert.Value < 0)
                txtTranspert.Value = 0;
        }

        private void btnStyle_Click(object sender, EventArgs e)
        {
            if (_publicSybmobl == null)
            {
                IFeatureClass fetclass = _layer.Class as IFeatureClass;
                switch (fetclass.ShapeType)
                {
                    case enumShapeType.Point:
                        _publicSybmobl = new SimpleMarkerSymbol();
                        break;
                    case enumShapeType.Polyline:
                        _publicSybmobl = new SimpleLineSymbol();
                        break;
                    case enumShapeType.Polygon:
                        _publicSybmobl = new SimpleFillSymbol();
                        break;
                    default:
                        _publicSybmobl = null;
                        break;
                }
            }

            if (_publicSybmobl != null)
            {
                using (Form frm = new Form())
                {
                    frm.Owner = this.FindForm();
                    frm.TopMost = true;
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    frm.Text = "符号风格编辑...";
                    PropertyGrid grid = new PropertyGrid();
                    frm.Controls.Add(grid);
                    grid.Dock = DockStyle.Fill;
                    grid.SelectedObject = _publicSybmobl;
                    frm.ShowDialog();
                }
            }
        }
    }
}
