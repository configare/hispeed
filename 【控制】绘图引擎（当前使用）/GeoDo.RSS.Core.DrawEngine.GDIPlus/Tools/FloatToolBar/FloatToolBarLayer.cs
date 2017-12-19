using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public class FloatToolBarLayer : Layer, IRenderLayer, ICanvasEvent, IFloatToolBarLayer, IToolboxLayer
    {
        protected bool _visible = true;
        protected bool _autoHide = true;
        protected const int BAR_WIDTH = 60;
        protected const int BAR_TOP = 28;
        protected const int BAR_LEFT = 4;
        protected const int SPLITER_HEIGHT = 1;
        protected Rectangle _bounds = new Rectangle();
        protected List<FloatToolItem> _toolItems = new List<FloatToolItem>();
        protected Action<FloatToolItem> _toolItemClicked;
        protected Dictionary<FloatToolItem, Rectangle> _itemBounds = new Dictionary<FloatToolItem, Rectangle>();
        protected bool _isFirst = true;
        protected FloatToolItem _currentToolItem;
        protected static Font _font = new Font("微软雅黑", 12, FontStyle.Bold);
        protected static Brush _brush = new SolidBrush(Color.FromArgb(160, 64, 64, 64));

        public FloatToolBarLayer()
        {
            _name = "浮动工具栏";
        }

        [DisplayName("是否可见"),Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        [DisplayName("是否可用"), Category("状态")]
        public bool IsEnabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        [DisplayName("自动隐藏"), Category("状态")]
        public bool IsAutoHide
        {
            get { return _autoHide; }
            set { _autoHide = value; }
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (!_enabled || _toolItems == null || _toolItems.Count == 0)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            int corner = 10;
            if (_isFirst)
            {
                int itemHeight = 40;
                int barHeight = /*_toolItems.Count * itemHeight +*/ 2 * corner;
                foreach (FloatToolItem item in _toolItems)
                {
                    if (item.Text == "-")
                        barHeight += SPLITER_HEIGHT;
                    else
                        barHeight += itemHeight;
                }
                _bounds = new Rectangle(BAR_LEFT, corner + BAR_TOP, BAR_WIDTH - 2 * corner, barHeight - 2 * corner);
                DrawRoundRect(g, _bounds, corner, Pens.Gray);
                int x = BAR_LEFT + 4;
                int y = corner + BAR_TOP + 6;
                foreach (FloatToolItem item in _toolItems)
                {
                    if (item.Text != "-")
                    {
                        if (item.Image != null)
                            g.DrawImage(item.Image, x, y);
                        else
                            g.FillRectangle(Brushes.Gray, x, y, 34, SPLITER_HEIGHT);
                        _itemBounds.Add(item, new Rectangle(x, y, itemHeight, itemHeight));
                        y += itemHeight;
                    }
                    else
                    {
                        g.FillRectangle(Brushes.Gray, new Rectangle(x, y - 6, 34, SPLITER_HEIGHT));
                        _itemBounds.Add(item, new Rectangle(x, y - 6, 34, SPLITER_HEIGHT));
                        y += SPLITER_HEIGHT;
                    }
                }
                _isFirst = false;
            }
            else
            {
                DrawRoundRect(g, _bounds, corner, Pens.Gray);
                foreach (FloatToolItem item in _toolItems)
                {
                    if (item == null)
                        continue;
                    Rectangle rect = _itemBounds[item];
                    if (item.Image != null)
                        g.DrawImage(item.Image, rect.X, rect.Y);
                    else
                        g.FillRectangle(Brushes.Gray, rect);
                }
                if (_currentToolItem != null)
                {
                    Rectangle rect = _itemBounds[_currentToolItem];
                    g.DrawRectangle(Pens.Yellow, rect.X - 2, rect.Y - 4, rect.Width - 4, rect.Height - 4);
                    if (!string.IsNullOrEmpty(_currentToolItem.Text))
                    {
                        g.DrawString(_currentToolItem.Text, _font, Brushes.Yellow, rect.X + 36, rect.Y + 2);
                    }
                }
            }
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            if (!_enabled)
                return;
            bool visible = _visible;
            switch (eventType)
            {
                case enumCanvasEventType.MouseMove:
                    if (_autoHide)
                    {
                        _visible = _bounds.Contains(e.ScreenX, e.ScreenY);
                        if (!_visible && e.ScreenX <= _bounds.X + _bounds.Width)
                            _visible = true;
                        if ((visible && _visible) || (visible && !_visible))
                        {
                            (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                        }
                    }
                    if (_visible)
                    {
                        FloatToolItem it = HitToolItem(e.ScreenX, e.ScreenY);
                        _currentToolItem = it;
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                    }
                    break;
                case enumCanvasEventType.MouseDown:
                    if (Control.MouseButtons == MouseButtons.Left)
                    {
                        FloatToolItem clickedItem = HitToolItem(e.ScreenX, e.ScreenY);
                        if (clickedItem != null)
                        {
                            if (_toolItemClicked != null)
                            {
                                _toolItemClicked(clickedItem);
                                e.IsHandled = true;             //激活工具栏，取消后续操作
                            } 
                        }
                    }
                    else if (Control.MouseButtons == MouseButtons.Right)
                    {
                        if (e.ScreenX <= _bounds.X + _bounds.Width)
                            _autoHide = !_autoHide;
                    }
                    break;
            }
        }

        private FloatToolItem HitToolItem(int x, int y)
        {
            foreach (FloatToolItem it in _itemBounds.Keys)
            {
                if (_itemBounds[it].Contains(x, y))
                    return it;
            }
            return null;
        }

        public void DrawRoundRect(Graphics g, Rectangle r, int d, Pen p)
        {

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddArc(r.X, r.Y, d, d, 180, 90);
                gp.AddArc(r.X + r.Width - d, r.Y, d, d, 270, 90);
                gp.AddArc(r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
                gp.AddArc(r.X, r.Y + r.Height - d, d, d, 90, 90);
                gp.AddLine(r.X, r.Y + r.Height - d, r.X, r.Y + d / 2f);
                g.FillPath(_brush, gp);
                g.DrawPath(p, gp);
            }
        }

        [Browsable(false)]
        public List<FloatToolItem> ToolItems
        {
            get { return _toolItems; }
        }

        [Browsable(false)]
        public Action<FloatToolItem> ToolItemClicked
        {
            get { return _toolItemClicked; }
            set { _toolItemClicked = value; }
        }

        [Browsable(false)]
        public FloatToolItem CurrentToolItem
        {
            get { return _currentToolItem; }
            set { _currentToolItem = value; }
        }
    }
}
