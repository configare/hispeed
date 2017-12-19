using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CodeCell.Bricks.UIs
{
    public partial class UCLayersControl : UserControl
    {
        public delegate void OnLayerItemClickHandler(LayerItem layerItem);
        public delegate void OnLayerItemDoubleClickHandler(LayerItem layerItem);
        public delegate void OnMouseUpAtLayerItemHandler(LayerItem layerItem);
        public delegate void OnLayerItemEnabledChangedHandler(LayerItem layerItem);
        public delegate void OnLayerItemOrderChangedHandler(LayerItem resLayerItem,LayerItem desLayerItem,int orderAtDes);
        public delegate void OnLayerItemNameChangedHandler(LayerItem layerItem,string name);
        [NonSerialized]
        protected LayerItem _selectedLayerItem = null;
        [NonSerialized]
        protected LayerItem _rootLayerItem = new LayerItem();
        [NonSerialized]
        private LayerItem _activedLayerItem = null;
        private LayerItem _currentLayerItem = null;
        private LayerItem _editLayerItem = null;
        protected const int cstVisibleBarWidth = 40;
        protected const int cstLayerItemHeight = 26;//32;
        protected const int cstRemainBank = 8;
        protected const int cstGroupImageTopRemainBank = 4;
        protected const int cstGroupLeftRemainBank = 24;
        protected const int cstGroupBarHeight = 24;
        protected static Image _groupImage = null;
        protected static Brush groupFillBrush = new SolidBrush(Color.FromArgb(225, 230, 232));
        protected static Brush focusFillBrush = new SolidBrush(Color.FromArgb(193,210,238));
        protected static Cursor dragCursor = null;
        protected static int cstDownIco = 1;
        protected static int cstUpIco = 2;
        protected static int cstEyeIcoOpen = 3;
        protected static int cstEyeIcoClose = 4;
        protected static int cstUnknowIco = 5;
        protected static int cstEditIco = 11;
        protected bool _isDraging = false;
        protected bool _allowDrag = false;
        protected LayerItem _dragLayerItem = null;
        public event OnLayerItemClickHandler OnLayerItemClick = null;
        public event OnLayerItemDoubleClickHandler OnLayerItemDoubleClick = null;
        public event OnMouseUpAtLayerItemHandler OnMouseUpAtLayerItem = null;
        public event OnLayerItemEnabledChangedHandler OnLayerItemEnabledChanged = null;
        public event OnLayerItemOrderChangedHandler OnLayerItemOrderChanged = null;
        public event OnLayerItemNameChangedHandler OnLayerItemNameChanged = null;
        private TextBox _textBox = new TextBox();

        public UCLayersControl()
        {
            InitializeComponent();
            _textBox.KeyDown += new KeyEventHandler(_textBox_KeyDown);
            _textBox.Visible = false;
            Controls.Add(_textBox);
            _groupImage = imageList1.Images[0];
            MouseHover += new EventHandler(UCLayerManager_MouseHover);
            MouseMove += new MouseEventHandler(UCLayerManager_MouseMove);
            MouseDown += new MouseEventHandler(UCLayerManager_MouseDown);
            Click += new EventHandler(UCLayerManager_Click);
            MouseDoubleClick += new MouseEventHandler(UCLayerManager_MouseDoubleClick);
            MouseUp += new MouseEventHandler(UCLayerManager_MouseUp);
            DoubleBuffered = true;
            Stream st = this.GetType().Assembly.GetManifestResourceStream("AgileMap.Bricks.Controls.UCLayerManager.move.cur");
            if (st != null)
            {
                dragCursor = new Cursor(st);
                st.Dispose();
            }
            else
                dragCursor = Cursors.Default;
        }

        void UCLayerManager_MouseHover(object sender, EventArgs e)
        {
            //FindActivedLayerItem(PointToClient(Control.MousePosition), _rootLayerItem);
            //if (_activedLayerItem != null)
            //{
            //    toolTip1.Show(_activedLayerItem.Name, 
            //                         this, 
            //                         _activedLayerItem._bounds.X,
            //                         _activedLayerItem._bounds.Y,
            //                         2000);
            //}
        }

        public void EnsureVisible(LayerItem layerItem)
        {
            if (Parent == null)
                return;
            using (Control c = new Control())
            {
                c.Location = layerItem._bounds.Location;
                c.Width = Width;
                c.Height = layerItem._bounds.Height;
                Parent.Controls.Add(c);
                (Parent as Panel).ScrollControlIntoView(c);
                (Parent as Panel).Controls.Remove(c);
            }
        }

        void _textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (_textBox.Tag != null)
                {
                    (_textBox.Tag as LayerItem).Name = _textBox.Text;
                    if (OnLayerItemNameChanged != null)
                        OnLayerItemNameChanged(_textBox.Tag as LayerItem, _textBox.Text);
                }
                _textBox.Visible = false;
            }
        }

        public LayerItem CurrentLayerItem
        {
            get { return _currentLayerItem; }
            set 
            {
                _currentLayerItem = value;
                if (_currentLayerItem != null)
                    EnsureVisible(_currentLayerItem);
            }
        }

        void UCLayerManager_Click(object sender, EventArgs e)
        {
            if (_activedLayerItem != null)
            {
                if (OnLayerItemClick != null)
                {
                    OnLayerItemClick(_activedLayerItem);
                    _currentLayerItem = _activedLayerItem;
                    EnsureVisible(_currentLayerItem);
                    return;
                }
            }
            _currentLayerItem = null;
        }

        void UCLayerManager_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                Cursor = Cursors.Default;
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(this, e.Location);
                    return;
                }
                LayerItem targetLayerItem = null;
                if (_isDraging && _dragLayerItem != null && Math.Abs(e.Location.Y - beginDrapPoint.Y) > 6)
                {
                    FindActivedLayerItem(e.Location, _rootLayerItem);
                    targetLayerItem = _activedLayerItem;
                    if (targetLayerItem != null)
                    {
                        if (targetLayerItem.Equals(_dragLayerItem) || !targetLayerItem.Parent.Equals(_dragLayerItem.Parent))
                        {
                            _isDraging = false;
                            return;
                        }
                        int idx = -1;
                        LayerItem desLayerItem = null;
                        idx = targetLayerItem.Parent.IndexOf(targetLayerItem);
                        desLayerItem = targetLayerItem.Parent;
                        if (OnLayerItemOrderChanged != null)
                            OnLayerItemOrderChanged(_dragLayerItem, desLayerItem, idx);
                    }
                    return;
                }
                _dragLayerItem = null;
                if (targetLayerItem != null && !targetLayerItem.IsFixed)
                {
                    if (OnMouseUpAtLayerItem != null)
                        OnMouseUpAtLayerItem(targetLayerItem);
                }
            }
            finally 
            {
                beginDrapPoint = Point.Empty;
                _isDraging = false;
            }
        }

        void UCLayerManager_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_activedLayerItem != null)
            {
                 if (OnLayerItemDoubleClick != null)
                     OnLayerItemDoubleClick(_activedLayerItem);
            }
        }

        private Point beginDrapPoint = Point.Empty;
        void UCLayerManager_MouseDown(object sender, MouseEventArgs e)
        {
            _activedLayerItem = null;
            FindActivedLayerItem(e.Location, _rootLayerItem);
            if (_activedLayerItem != null)
            {
                if (_activedLayerItem._visibleBounds.Contains(e.Location))
                {
                    _activedLayerItem.Enabled = !_activedLayerItem.Enabled;
                    if (_activedLayerItem.ChildCount > 0)
                        foreach (LayerItem it in _activedLayerItem.Children)
                            it.Enabled = _activedLayerItem.Enabled;
                    if (OnLayerItemEnabledChanged != null)
                        OnLayerItemEnabledChanged(_activedLayerItem);
                }
                else if (_activedLayerItem._editBounds.Contains(e.Location)  &&  _activedLayerItem.Editable)
                {
                    _editLayerItem = _activedLayerItem;
                    Invalidate();
                    return;
                }
                else
                {
                    if (_activedLayerItem._groupCollpaseBounds.Contains(e.Location))
                        _activedLayerItem.IsCollpased = !_activedLayerItem.IsCollpased;
                }
                if (_allowDrag && _activedLayerItem != null &&  !_activedLayerItem.IsFixed)
                {
                    _isDraging = true;
                    _dragLayerItem = _activedLayerItem;
                    beginDrapPoint = e.Location;
                }
            }
            if (_textBox.Visible)
                _textBox.Visible = false;
            _currentLayerItem = _activedLayerItem;
            Invalidate();
        }

        void UCLayerManager_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraging  && _activedLayerItem != null && e.Button == MouseButtons.Left && Math.Abs(e.Location.Y - beginDrapPoint.Y)>6)
            {
                Cursor = dragCursor;
                return;
            }
            //_activedLayerItem = null;
            //Cursor = Cursors.Default;
            //FindActivedLayerItem(e.Location,_rootLayerItem);
            //if (_activedLayerItem != null && _activedLayerItem._bounds.Contains(e.Location))
            //    Cursor = Cursors.Hand;
            //Invalidate();
        }

        private void FindActivedLayerItem(Point pt,LayerItem layerItem)
        {
            if (layerItem._bounds.Contains(pt))
            {
                _activedLayerItem = layerItem;
                return;
            }
            if (layerItem.Children != null && !layerItem.IsCollpased)
            {
                foreach (LayerItem it in layerItem.Children)
                    FindActivedLayerItem(pt, it);
            }
        }

        public bool AllowDragLayerItem
        {
            get { return _allowDrag; }
            set { _allowDrag = value; }
        }

        public LayerItem RootLayerItem
        {
            get { return _rootLayerItem; }
        }

        public LayerItem SelectedLayerItem
        {
            get { return _selectedLayerItem; }
            set { _selectedLayerItem = value; }
        }

        public LayerItem EditLayerItem
        {
            get { return _editLayerItem; }
            set { _editLayerItem = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_rootLayerItem == null)
                return;
            int top = 0, left = 0;
            if (_rootLayerItem.Children != null)
            {
                foreach (LayerItem it in _rootLayerItem.Children)
                    DrawLayerItem(it, e.Graphics, ref top, ref left);
                if (top > Size.Height)
                    Size = new Size(Parent.Width - 24, top + 6);
                e.Graphics.DrawLine(Pens.Gray, cstVisibleBarWidth, 0, cstVisibleBarWidth, top);
                e.Graphics.DrawLine(Pens.Gray, Width - 1, 0, Width - 1, top);
            }
        }

        private void DrawLayerItem(LayerItem it, Graphics g,ref int top,ref int left)
        {
            if (it == null)
                return;
            int offsetLeft = 0;
            if (it.ChildCount == 0)
            {
                if (it.Parent != null && !_rootLayerItem.Equals(it.Parent))
                    offsetLeft = GetOffsetLeft(it);
                it._bounds.X = 0;
                it._bounds.Y = top;
                it._bounds.Width = Width;
                it._bounds.Height = cstLayerItemHeight;
                //if (_activedLayerItem != null && _activedLayerItem.Equals(it) || it.Equals(_currentLayerItem))
                //    g.FillRectangle(focusFillBrush, it._bounds);
                g.DrawRectangle(Pens.Gray, it._bounds);
                g.DrawImage(GetImage(it), offsetLeft + cstVisibleBarWidth + 4, top + cstRemainBank);
                it._visibleBounds.X = 4;
                it._visibleBounds.Y = top + cstRemainBank;
                it._visibleBounds.Width = imageList1.Images[cstEyeIcoOpen].Width;
                it._visibleBounds.Height = imageList1.Images[cstEyeIcoOpen].Height;
                //
                it._editBounds = new Rectangle(it._visibleBounds.Right + 2,it._visibleBounds.Y,it._visibleBounds.Width,it._visibleBounds.Height);
                if (it.Enabled)
                {
                    g.DrawImage(imageList1.Images[cstEyeIcoOpen], it._visibleBounds.X, it._visibleBounds.Y);
                }
                else
                {
                    g.DrawImage(imageList1.Images[cstEyeIcoClose], it._visibleBounds.X, it._visibleBounds.Y);
                }
                if (_editLayerItem != null && it.Equals(_editLayerItem))
                {
                    g.DrawImage(imageList1.Images[cstEditIco], it._editBounds.X, it._editBounds.Y);
                }
                if (it.Equals(_currentLayerItem))
                {
                    g.DrawString(it.Name, Font, Brushes.Gray, offsetLeft + cstVisibleBarWidth + imageList1.ImageSize.Width + 8, top + cstRemainBank + 2);
                }
                else
                {
                    g.DrawString(it.Name, Font, Brushes.Black, offsetLeft + cstVisibleBarWidth + imageList1.ImageSize.Width + 8, top + cstRemainBank + 2);
                }
                top += cstLayerItemHeight;
                offsetLeft = 0;
            }
            else//group
            {
                it._bounds = new Rectangle(cstVisibleBarWidth, top, Width, cstGroupBarHeight);
                it._bounds.X = 0;
                it._bounds.Y = top;
                it._bounds.Width = Width;
                it._bounds.Height = cstGroupBarHeight;
                g.FillRectangle(groupFillBrush, it._bounds);
                //if (_activedLayerItem != null && _activedLayerItem.Equals(it))
                //    g.FillRectangle(focusFillBrush, it._bounds);
                g.DrawRectangle(Pens.Gray, it._bounds);
                if (it.Parent != null && !_rootLayerItem.Equals(it.Parent))
                    offsetLeft = GetOffsetLeft(it);
                it._groupCollpaseBounds.X = offsetLeft + cstVisibleBarWidth + 4;
                it._groupCollpaseBounds.Y = top + cstGroupImageTopRemainBank;
                it._groupCollpaseBounds.Width = imageList1.Images[2].Width;
                it._groupCollpaseBounds.Height = imageList1.Images[2].Height;
                if (it.IsCollpased)
                    g.DrawImage(imageList1.Images[cstUpIco], it._groupCollpaseBounds.X, it._groupCollpaseBounds.Y);
                else
                    g.DrawImage(imageList1.Images[cstDownIco], it._groupCollpaseBounds.X, it._groupCollpaseBounds.Y);
                g.DrawImage(_groupImage, offsetLeft + cstVisibleBarWidth + 22, top + cstGroupImageTopRemainBank);
                it._visibleBounds.X = 4;
                it._visibleBounds.Y = top + 6;
                it._visibleBounds.Width = imageList1.Images[cstEyeIcoOpen].Width;
                it._visibleBounds.Height = imageList1.Images[cstEyeIcoOpen].Height;
                if (it.Enabled)
                    g.DrawImage(imageList1.Images[cstEyeIcoOpen], it._visibleBounds.X, it._visibleBounds.Y);
                else
                    g.DrawImage(imageList1.Images[cstEyeIcoClose], it._visibleBounds.X, it._visibleBounds.Y);
                if (it.Equals(_currentLayerItem))
                {
                    g.DrawString(it.Name, Font, Brushes.Gray, offsetLeft + cstVisibleBarWidth + 38, top + cstRemainBank);
                }
                else
                {
                    g.DrawString(it.Name, Font, Brushes.Black, offsetLeft + cstVisibleBarWidth + 38, top + cstRemainBank);
                }
                top += cstGroupBarHeight;
                //
                if(!it.IsCollpased)
                    foreach (LayerItem child in it.Children)
                        DrawLayerItem(child, g, ref top, ref left);
            }
        }

        private Image GetImage(LayerItem it)
        {
            if (it.Image != null)
                return it.Image;
            switch (it.LayerType)
            { 
                case LayerItem.enumLayerTypes.OrbitData:
                    return imageList1.Images[6];
                case LayerItem.enumLayerTypes.BaseVector:
                    return imageList1.Images[7];
                case LayerItem.enumLayerTypes.DigitalNumberMeasure:
                    return imageList1.Images[8];
                case LayerItem.enumLayerTypes.RoutineObser:
                    return imageList1.Images[9];
                case LayerItem.enumLayerTypes.Raster:
                    return imageList1.Images[10];
                default:
                    return imageList1.Images[cstUnknowIco];
            }
        }

        private int GetOffsetLeft(LayerItem it)
        {
            int n = 0;
            while (it.Parent != null)
            {
                n++;
                it = it.Parent;
            }
            return (n - 1) * cstGroupLeftRemainBank;
        }

        private void btmRename_Click(object sender, EventArgs e)
        {
            if (_currentLayerItem != null && !_currentLayerItem.IsFixed)
            {
                _textBox.Bounds = _currentLayerItem._bounds;
                _textBox.Text = _currentLayerItem.Name;
                _textBox.SelectAll();
                _textBox.Height = cstLayerItemHeight;
                _textBox.Tag = _currentLayerItem;
                _textBox.Visible = true;
                _textBox.Focus();
            }
        }
    }
}
