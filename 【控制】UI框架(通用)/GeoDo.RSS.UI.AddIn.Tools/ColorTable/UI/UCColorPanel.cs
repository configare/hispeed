using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public partial class UCColorPanel : UserControl
    {
        private LinearGradientBrush _brush = null;
        private List<ColorItem> _colorItems = new List<ColorItem>();
        private ColorItem _selectedColorItem = null;
        private bool _isDraging = false;
        private int barMinHeight = 8;
        private int barMaxHeight = 12;
        private int barHalfWidth = 4;
        private int barTopBank = 2;
        internal int MinValue = 0;
        internal int MaxValue = 0;
        internal event OnMoveColorBarHandler OnMoveColorBarHandler = null;
        private bool _isDrawScales = true;

        public UCColorPanel()
        {
            InitializeComponent();
            _colorItems.Add(new ColorItem(0f, Color.White));
            _colorItems.Add(new ColorItem(1f, Color.Black));
            DoubleBuffered = true;
            ReCreateBrush();
        }

        public bool IsDrawScales
        {
            get { return _isDrawScales; }
            set { _isDrawScales = value; }
        }

        public ColorItem[] ColorItems 
        {
            get 
            {
                if (_colorItems.Count < 3)
                    return null;
                List<ColorItem> its = new List<ColorItem>();
                for (int i = 1; i < _colorItems.Count - 1; i++)
                    its.Add(_colorItems[i]);
                return its.ToArray();
            }
        }

        public void ClearItems()
        {
            if (_colorItems.Count > 2)
            {
                for (int i = _colorItems.Count - 2; i >= 1; i--)
                    _colorItems.RemoveAt(i);
                Invalidate();
            }
        }

        public void AddColorItems(ColorItem[] items)
        {
            if (items == null || items.Length == 0)
                return;
            foreach (ColorItem it in items)
            {
                AddColorItem(it,false);
            }
            AdjustOrderOfColorItems();
            ReCreateBrush();
        }

        public void AddColorItem(ColorItem item,bool needRefresh)
        {
            if (needRefresh)
                item.Position = 0.5f;
            _colorItems.Insert(1,item);
            if(needRefresh)
                ReCreateBrush();
        }

        public void RemoveSelectedColorItem()
        {
            if (_selectedColorItem != null)
                _colorItems.Remove(_selectedColorItem);
            ReCreateBrush();
        }

        internal void ReCreateBrush()
        {
            _brush = new LinearGradientBrush(new Rectangle(0,0,Width,Height),Color.White, Color.Black, LinearGradientMode.Horizontal);
            if (_colorItems.Count > 0)
            {
                float[] poses = new float[_colorItems.Count];
                Color[] colors = new Color[_colorItems.Count];
                for (int i = 0; i < _colorItems.Count; i++)
                {
                    poses[i] = _colorItems[i].Position;
                    colors[i] = _colorItems[i].Color;
                }
                ColorBlend blend = new ColorBlend(_colorItems.Count);
                blend.Colors = colors;
                blend.Positions = poses;
                _brush.InterpolationColors = blend;
            }
            Invalidate();
        }

        public Color[] GetColors(int count)
        {
            if (count <= 0)
                return null;
            if (_bitmap == null)
                return null;
            float dx = (float)Width / (float)count;
            int y = _bitmap.Height / 2;
            Color[] colors = new Color[count];
            for (int i = 0; i < count; i++)
            {
                int x = (int)(i * dx);
                if (x >= _bitmap.Width)
                    break;
                colors[i] = _bitmap.GetPixel(x, y);
            }
            return colors;
        }

        private Bitmap _bitmap = null;
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_brush == null)
                return;
            _bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            using (Graphics g = Graphics.FromImage(_bitmap))
            {
                g.FillRectangle(_brush,0,barTopBank + barMaxHeight,Width,Height - 2 * barTopBank - 30);
                if (IsDrawScales)
                {
                    if (MaxValue > MinValue)
                    {
                        int span = MaxValue - MinValue;
                        float pixelPerSpan = span / (float)Width;
                        int pixelSpan = 40;
                        int beginX = 0;
                        float v = MinValue;
                        g.DrawLine(Pens.Blue, 0, Height - 6, Width, Height - 6);
                        while (beginX < Width)
                        {
                            g.DrawLine(Pens.Blue, beginX, Height - 6, beginX, Height - 12);
                            int intv = (int)v;
                            g.DrawString(intv.ToString(), Font, Brushes.Blue, beginX, Height - 17);
                            beginX += pixelSpan;
                            v += (pixelSpan * pixelPerSpan);
                        }
                    }
                }
            }
            e.Graphics.DrawImage(_bitmap, 0,0);
            if (_colorItems.Count == 0)
                return;
            if (_colorItems.Count < 3)
                return;
            for(int i=1;i<_colorItems.Count-1;i++)
            {
                ColorItem it = _colorItems[i];
                int x = (int)(Width * it.Position);
                it.X = x;
                GraphicsPath path = new GraphicsPath();
                path.AddLine(x - barHalfWidth, barTopBank, x + barHalfWidth, barTopBank);
                path.AddLine(x + barHalfWidth, barTopBank, x + barHalfWidth, barTopBank + barMinHeight);
                path.AddLine(x + barHalfWidth, barTopBank + barMinHeight, x, barTopBank + barMaxHeight);
                path.AddLine(x, barTopBank + barMaxHeight, x - barHalfWidth, barTopBank + barMinHeight);
                path.AddLine(x - barHalfWidth, barTopBank + barMinHeight, x - barHalfWidth, barTopBank);
                it.Bounds = path;
                if (_selectedColorItem != null && it.Equals(_selectedColorItem))
                {
                    e.Graphics.FillPath(Brushes.Red, path);
                    e.Graphics.DrawPath(Pens.Black, path);
                }
                else
                {
                    e.Graphics.FillPath(Brushes.LightSkyBlue, path);
                    e.Graphics.DrawPath(Pens.Black, path);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_selectedColorItem != null && _selectedColorItem.Bounds.IsVisible(e.Location))
            {
                _isDraging = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDraging && _selectedColorItem != null)
            {
                if (e.X <= 0 || e.X >= ClientSize.Width)
                    return;
                _selectedColorItem.Position = e.X / (float)Width;
                AdjustOrderOfColorItems();
                ReCreateBrush();
                if (OnMoveColorBarHandler != null)
                {
                    string vstring = string.Empty;
                    if (MaxValue > MinValue)
                    {
                        int span = MaxValue - MinValue;
                        float pixelPerSpan = span / (float)Width;
                        int v = (int)(e.X * pixelPerSpan);
                        vstring = v.ToString();
                    }
                    OnMoveColorBarHandler(this, vstring, _bitmap.GetPixel(e.X, Height / 2));
                }
                return;
            }
            if (_colorItems == null || _colorItems.Count == 0)
                return;
            foreach (ColorItem it in _colorItems)
            {
                if (it.Bounds == null)
                    continue;
                if (it.Bounds.IsVisible(e.Location))
                {
                    _selectedColorItem = it;
                    Invalidate();
                    return;
                }
            }
            _selectedColorItem = null;
            Invalidate();
        }

        internal void AdjustOrderOfColorItems()
        {
            List<ColorItem> items = new List<ColorItem>();
            for (int i = 1; i < _colorItems.Count - 1; i++)
            {
                items.Add(_colorItems[i]);
            }
            items.Sort(new Comparison<ColorItem>(delegate(ColorItem a, ColorItem b)
                                                                                         {
                                                                                             return a.X - b.X;
                                                                                         }
                                                                               )
                           );
            items.Add(_colorItems[_colorItems.Count - 1]);
            items.Insert(0, _colorItems[0]);
            _colorItems = items;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isDraging = false;
            if (e.Button == MouseButtons.Right)
            {
                if (_selectedColorItem != null && _selectedColorItem.Bounds.IsVisible(e.Location))
                {
                    contextMenuStrip1.Show(this, e.X, e.Y);
                }
            }
            if (OnMoveColorBarHandler != null)
                OnMoveColorBarHandler(this, string.Empty, Color.Gray);
        }

        private void mnuDelete_Click(object sender, EventArgs e)
        {
            RemoveSelectedColorItem();
        }
    }

    internal  delegate void OnMoveColorBarHandler(object sender,string value,Color color);
}
