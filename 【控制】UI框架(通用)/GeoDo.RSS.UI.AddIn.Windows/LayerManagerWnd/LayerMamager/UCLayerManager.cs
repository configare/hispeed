using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    public partial class UCLayerManager: UserControl, ILayerManager
    {
        public delegate void OnLayerItemClickHandler(ILayerItem layerItem);
        [NonSerialized]
        private TextBox _textBox = new TextBox();
        [NonSerialized]
        private ILayersProvider _provider;
        [NonSerialized]
        private ILayerItem _focusedItem;
        [NonSerialized]
        private ILayerItem _currentLayerItem;
        [NonSerialized]
        private ILayerItem _dragLayerItem;
        [NonSerialized]
        private ILayerItem _editLayerItem;
        protected bool _isDraging = false;
        private bool _allowDrag = false;
        [NonSerialized]
        private Dictionary<ILayerItem, ItemDrawHelper> _helpers;
        private const int cstLayerItemHeight = 26;
        protected const int cstGroupImageTopRemainBank = 4;
        private const int cstGroupLeftRemainBank = 24;
        private const int cstFistItemLeftBank = 8;
        protected const int cstGroupBarHeight = 24;
        protected const int cstRemainBank = 8;
        protected const int cstVisibleBarWidth = 40;
        protected static int cstDownIco = 1;//IcoIndex
        protected static int cstUpIco = 2;
        protected static int cstEyeIcoOpen = 3;
        protected static int cstEyeIcoClose = 4;
        protected static int cstUnknowIco = 5;
        protected static int cstEditIco = 8;
        private Point beginDrapPoint = Point.Empty;
        protected static Image _groupImage = null;
        protected static Brush groupFillBrush = new SolidBrush(Color.FromArgb(225, 230, 232));
        protected static Cursor dragCursor = null;
        //Event
        public event OnLayerItemClickHandler OnLayerItemClick = null;

        public UCLayerManager()
        {
            InitializeComponent();
            DoubleBuffered = true;
            _textBox.Visible = false;
            _textBox.KeyDown += new KeyEventHandler(_textBox_KeyDown);
            btmRename.Click += new EventHandler(btmRename_Click);
            TspMenuSave.Click += new EventHandler(TspMenuSave_Click);
            Controls.Add(_textBox);
            _groupImage = imageList.Images[0];
            SizeChanged += new EventHandler(UCLayerManager_SizeChanged);
            _helpers = new Dictionary<ILayerItem, ItemDrawHelper>();
        }

        public ILayerItem CurrentLayerItem
        {
            get { return _currentLayerItem; }
            set { _currentLayerItem = value; }
        }

        public bool AllowDrag
        {
            get { return _allowDrag; }
            set { _allowDrag = value; }
        }

        public ILayersProvider Provider
        {
            get { return _provider; }
        }

        private void UCLayerManager_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        #region ILayerManager Member

        public void Apply(ILayersProvider provider)
        {
            _provider = provider;
        }

        public void Update()
        {
            if (_provider == null)
                return;
            _provider.Update();
            Invalidate();
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_provider == null || _provider.Items == null || _provider.Items.Count == 0)
                return;
            int x = 0, y = 0, maxWidth = 0;
            foreach (ILayerItem it in _provider.Items)
                DrawLayerItem(e.Graphics, it, ref x, ref y, ref maxWidth);
            maxWidth = maxWidth > (Parent.Width - 20) ? maxWidth : (Parent.Width - 20);
            if (y > Size.Height||maxWidth > Size.Width)
                Size = new Size(maxWidth > Size.Width ? maxWidth :Size.Width ,y > Size.Height?(y + 6):Size.Height);
            //Invalidate();
            e.Graphics.DrawLine(Pens.Gray, cstVisibleBarWidth, 0, cstVisibleBarWidth, y);
            e.Graphics.DrawLine(Pens.Gray, Width - 1, 0, Width - 1, y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_helpers == null)
                return;
            foreach (ILayerItem it in _helpers.Keys)
            {
                if (_helpers[it].Bounds.Contains(e.Location))
                {
                    _focusedItem = it;
                    return;
                }
            }
            _focusedItem = null;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            try
            {
                Cursor = Cursors.Default;
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip.Show(this, e.Location);
                    return;
                }
                ILayerItem targetLayerItem = null;
                if (_isDraging && _dragLayerItem != null && Math.Abs(e.Location.Y - beginDrapPoint.Y) > 6)
                {
                    FindFocusedLayer(e.Location, _provider);
                    targetLayerItem = _focusedItem;
                    if (targetLayerItem != null)
                    {
                        ILayerItem parent = FindLayerParent(targetLayerItem);
                        ILayerItem dragParent = FindLayerParent(_dragLayerItem);
                        if (parent == null)
                            return;
                        if (targetLayerItem.Equals(_dragLayerItem) || !parent.Equals(dragParent))
                        {
                            _provider.Group(_dragLayerItem, parent as ILayerItemGroup);
                            Update();
                            return;
                        }
                        int idx = -1;
                        ILayerItem desLayerItem = null;
                        idx = (parent as ILayerItemGroup).Items.IndexOf(targetLayerItem);
                        desLayerItem = parent;
                        _provider.AdjustOrder(idx, _dragLayerItem);
                        Update();
                    }
                    return;
                }
                _dragLayerItem = null;
            }
            finally
            {
                beginDrapPoint = Point.Empty;
                _dragLayerItem = null;
                _isDraging = false;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _focusedItem = null;
            if (_provider == null || _provider.Items == null || _provider.Items.Count == 0)
                return;
            FindFocusedLayer(e.Location, _provider);
            if (_focusedItem != null)
            {
                if (_helpers[_focusedItem].VisibleBounds.Contains(e.Location))
                {
                    _focusedItem.IsVisible = !_focusedItem.IsVisible;
                    SetAllChildrenVisible(_focusedItem, _focusedItem.IsVisible);
                    _provider.RefreshViewer();
                }
                else if (_helpers[_focusedItem].EditBounds.Contains(e.Location) && _focusedItem.IsSelected)
                {
                    _editLayerItem = _focusedItem;
                    Invalidate();
                    return;
                }
                else
                {
                    if (_helpers[_focusedItem].GroupCollpaseBounds.Contains(e.Location))
                        _helpers[_focusedItem].IsCollpased = !_helpers[_focusedItem].IsCollpased;
                }
                if (_allowDrag && _focusedItem != null && !_helpers[_focusedItem].IsFixed)
                {
                    Cursor = dragCursor;
                    _isDraging = true;
                    _dragLayerItem = _focusedItem;
                    beginDrapPoint = e.Location;
                }
            }
            if (_textBox.Visible)
                _textBox.Visible = false;
            _currentLayerItem = _focusedItem;
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (_focusedItem != null)
            {
                if (OnLayerItemClick != null)
                {
                    OnLayerItemClick(_focusedItem);
                    _currentLayerItem = _focusedItem;
                    return;
                }
            }
            _currentLayerItem = null;
        }

        private void DrawLayerItem(Graphics g, ILayerItem item, ref int x, ref int y, ref int maxWidth)
        {
            if (item == null)
                return;
            int offsetLeft = 0;
            if (item is ILayerItemGroup)
            {
                if (!_helpers.ContainsKey(item))
                    _helpers.Add(item, new ItemDrawHelper());
                ItemDrawHelper helper = _helpers[item];
                helper.Bounds = new Rectangle(0, y, Width, cstGroupBarHeight);
                g.FillRectangle(groupFillBrush, helper.Bounds);
                g.DrawRectangle(Pens.Gray, helper.Bounds);
                g.DrawLine(Pens.Gray, x + cstVisibleBarWidth, y, x + cstVisibleBarWidth, y + cstGroupBarHeight);
                ILayerItem itemParent = FindLayerParent(item);
                if (itemParent != null && !_provider.Equals(itemParent))
                    offsetLeft = GetOffsetLeft(item);
                else if (_provider.Equals(itemParent))
                    offsetLeft = cstFistItemLeftBank;
                helper.GroupCollpaseBounds = new Rectangle();
                helper.GroupCollpaseBounds.X = offsetLeft + cstVisibleBarWidth;
                helper.GroupCollpaseBounds.Y = y + cstGroupImageTopRemainBank;
                helper.GroupCollpaseBounds.Width = imageList.Images[cstUpIco].Width;
                helper.GroupCollpaseBounds.Height = imageList.Images[cstUpIco].Height;
                if (helper.IsCollpased)
                    g.DrawImage(imageList.Images[cstUpIco], helper.GroupCollpaseBounds.X, helper.GroupCollpaseBounds.Y);
                else
                    g.DrawImage(imageList.Images[cstDownIco], helper.GroupCollpaseBounds.X, helper.GroupCollpaseBounds.Y);
                g.DrawImage(_groupImage, offsetLeft + cstVisibleBarWidth + 22, y + cstGroupImageTopRemainBank);
                helper.VisibleBounds = new Rectangle();
                helper.VisibleBounds.X = 4;
                helper.VisibleBounds.Y = y + 6;
                helper.VisibleBounds.Width = imageList.Images[cstEyeIcoOpen].Width;
                helper.VisibleBounds.Height = imageList.Images[cstEyeIcoOpen].Height;
                if (item.IsVisible)
                    g.DrawImage(imageList.Images[cstEyeIcoOpen], helper.VisibleBounds.X, helper.VisibleBounds.Y);
                else
                    g.DrawImage(imageList.Images[cstEyeIcoClose], helper.VisibleBounds.X, helper.VisibleBounds.Y);
                int beginX = offsetLeft + cstVisibleBarWidth + 42;
                if (item.Equals(_currentLayerItem))
                {
                    g.DrawString(item.Name, Font, Brushes.Gray, offsetLeft + cstVisibleBarWidth + 42, y + cstRemainBank);
                    if (maxWidth < g.MeasureString(item.Name, Font).Width)
                        maxWidth = (int)g.MeasureString(item.Name, Font).Width + beginX;
                }
                else
                {
                    g.DrawString(item.Name, Font, Brushes.Black, offsetLeft + cstVisibleBarWidth + 42, y + cstRemainBank);
                    if (maxWidth < g.MeasureString(item.Name, Font).Width)
                        maxWidth = (int)g.MeasureString(item.Name, Font).Width + beginX;
                }
                y += cstGroupBarHeight;
                if (!helper.IsCollpased)
                    foreach (ILayerItem child in (item as ILayerItemGroup).Items)
                        DrawLayerItem(g, child, ref x, ref y,ref maxWidth);
            }
            else
            {
                ILayerItem itemParent = FindLayerParent(item);
                if (itemParent != null && !_provider.Equals(itemParent))
                    offsetLeft = GetOffsetLeft(item);
                else if (_provider.Equals(itemParent))
                    offsetLeft = cstFistItemLeftBank;
                if (!_helpers.ContainsKey(item))
                    _helpers.Add(item, new ItemDrawHelper());
                ItemDrawHelper helper = _helpers[item];
                helper.Bounds = new Rectangle();
                helper.Bounds.X = 0;
                helper.Bounds.Y = y;
                helper.Bounds.Width = Width;
                helper.Bounds.Height = cstLayerItemHeight;
                g.DrawRectangle(Pens.Gray, helper.Bounds);
                g.DrawLine(Pens.Gray, x + cstVisibleBarWidth, y, x + cstVisibleBarWidth, y + cstGroupBarHeight);
                g.DrawImage(GetImage(item), offsetLeft + cstVisibleBarWidth, y + cstRemainBank,16,16);
                helper.VisibleBounds = new Rectangle();
                helper.VisibleBounds.X = 4;
                helper.VisibleBounds.Y = y + cstRemainBank;
                helper.VisibleBounds.Width = imageList.Images[cstEyeIcoOpen].Width;
                helper.VisibleBounds.Height = imageList.Images[cstEyeIcoOpen].Height;
                helper.EditBounds = new Rectangle(helper.VisibleBounds.Right + 2, helper.VisibleBounds.Y, helper.VisibleBounds.Width, helper.VisibleBounds.Height);
                if (item.IsVisible)
                {
                    g.DrawImage(imageList.Images[cstEyeIcoOpen], helper.VisibleBounds.X, helper.VisibleBounds.Y);
                }
                else
                {
                    g.DrawImage(imageList.Images[cstEyeIcoClose], helper.VisibleBounds.X, helper.VisibleBounds.Y);
                }
                //if (_editLayerItem != null && item.Equals(_editLayerItem))
                //{
                //    g.DrawImage(imageList.Images[cstEditIco], helper.EditBounds.X, helper.EditBounds.Y);
                //}
                if (item.IsSelected)
                {
                    g.DrawImage(imageList.Images[cstEditIco], helper.EditBounds.X, helper.EditBounds.Y);
                }
                int beginX = offsetLeft + cstVisibleBarWidth + imageList.ImageSize.Width + 8;
                if (item.Equals(_currentLayerItem))
                {
                    g.DrawString(item.Name, Font, Brushes.Gray, offsetLeft + cstVisibleBarWidth + imageList.ImageSize.Width + 8, y + cstRemainBank + 2);
                    if (maxWidth < g.MeasureString(item.Name, Font).Width)
                        maxWidth = (int)g.MeasureString(item.Name, Font).Width + beginX;
                }
                else
                {
                    g.DrawString(item.Name, Font, Brushes.Black, offsetLeft + cstVisibleBarWidth + imageList.ImageSize.Width + 8, y + cstRemainBank + 2);
                    if (maxWidth < g.MeasureString(item.Name, Font).Width)
                        maxWidth = (int)g.MeasureString(item.Name, Font).Width + beginX;
                }
                y += cstLayerItemHeight;
                offsetLeft = 0;
            }
        }

        private void _textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (_textBox.Tag != null)
                {
                    if (_textBox.Tag is ILayerItemGroup)
                        (_textBox.Tag as LayerItemGroup).Name = _textBox.Text;
                    else if (_textBox.Tag is ILayerItem)
                        (_textBox.Tag as ILayerItem).Name = _textBox.Text;
                    _provider.SetLayerName(_textBox.Text, _textBox.Tag as ILayerItem);
                }
                _textBox.Visible = false;
                Invalidate();
            }
        }

        private void FindFocusedLayer(Point pt, ILayersProvider provider)
        {
            foreach (ILayerItem it in provider.Items)
            {
                FindFocusedLayerItem(pt, it);
            }
        }

        private void FindFocusedLayerItem(Point pt, ILayerItem layerItem)
        {
            if (_helpers[layerItem].Bounds.Contains(pt))
            {
                _focusedItem = layerItem;
                return;
            }
            if ((layerItem is ILayerItemGroup) && !_helpers[layerItem].IsCollpased && (layerItem as ILayerItemGroup).Items.Count > 0)
            {
                foreach (ILayerItem it in (layerItem as ILayerItemGroup).Items)
                    FindFocusedLayerItem(pt, it);
            }
        }

        public ILayerItem FindLayerParent(ILayerItem layer)
        {
            if (_provider == null || _provider.Items == null || _provider.Items.Count == 0)
                return null;
            foreach (ILayerItem item in _provider.Items)
            {
                if (layer.Equals(item))
                    return _provider;
                if (item is ILayerItemGroup)
                {
                    ILayerItem parent = (item as ILayerItemGroup).FindParent(layer);
                    if (parent != null)
                        return parent;
                }
            }
            return null;
        }

        private Image GetImage(ILayerItem it)
        {
            if (it.Image != null)
                return it.Image;
            switch (it.LayerType)
            {
                case enumLayerTypes.OrbitData:
                    return imageList.Images[6];
                case enumLayerTypes.BaseVector:
                    return imageList.Images[7];
                default:
                    return imageList.Images[cstUnknowIco];
            }
        }

        private void btmRename_Click(object sender, EventArgs e)
        {
            if (_currentLayerItem != null && !_helpers[_currentLayerItem].IsFixed)
            {
                _textBox.Visible = true;
                _textBox.Bounds = _helpers[_currentLayerItem].Bounds;
                _textBox.Text = _currentLayerItem.Name;
                _textBox.SelectAll();
                _textBox.Height = cstLayerItemHeight;
                _textBox.Tag = _currentLayerItem;
                _textBox.Focus();
            }
        }

        void TspMenuSave_Click(object sender, EventArgs e)
        {
            if (_currentLayerItem == null || _currentLayerItem.LayerType != enumLayerTypes.BaseVector)
                return;
            _provider.SaveVectorItemShowMethod(_currentLayerItem);
        }

        private int GetOffsetLeft(ILayerItem it)
        {
            int n = 0;
            ILayerItem parent = FindLayerParent(it);
            while (parent != null)
            {
                n++;
                it = parent;
                parent = FindLayerParent(it);
            }
            return (n-1) * cstGroupLeftRemainBank;
        }

        private void SetAllChildrenVisible(ILayerItem parent, bool isVisible)
        {
            if (parent is ILayerItemGroup)
            {
                foreach (ILayerItem item in (parent as ILayerItemGroup).Items)
                {
                    item.IsVisible = isVisible;
                    SetAllChildrenVisible(item, isVisible);
                }
            }
        }

        public void SetLayerItemDown()
        {
            if (_currentLayerItem != null)
            {
                ILayerItem parent = FindLayerParent(_currentLayerItem);
                if (parent == null)
                    return;
                int idx = (parent as ILayerItemGroup).Items.IndexOf(_currentLayerItem);
                int count = (parent as ILayerItemGroup).Items.Count;
                idx = (idx + 1) >= count ? (count - 1) : ++idx;
                if (parent is ILayersProvider)
                    _provider.AdjustOrder(idx, _currentLayerItem);
                else
                {
                    _provider.AdjustOrder(idx,_currentLayerItem,parent);
                }
                Update();
            }
        }

        public void SetLayerItemUp()
        {
            if (_currentLayerItem != null)
            {
                ILayerItem parent = FindLayerParent(_currentLayerItem);
                if (parent == null)
                    return;
                int idx = (parent as ILayerItemGroup).Items.IndexOf(_currentLayerItem);
                idx = (idx - 1) < 0 ? 0 : --idx;
                _provider.AdjustOrder(idx, _currentLayerItem);
                Update();
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (_currentLayerItem == null)
            {
                e.Cancel = true;
                return;
            }
            if (_currentLayerItem.LayerType == enumLayerTypes.BaseVector)
            {
                TspMenuSave.Visible = true;
            }
            else
            {
                TspMenuSave.Visible = false;
            }
        }
    }
}
