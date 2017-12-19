using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.UIs
{
    public partial class UCHierarchicalListBox : UserControl
    {
        private List<HierItem> _hierItems = new List<HierItem>();
        private int _indent = 24;
        private int _countPerPage = 20;
        private int _currentPage = 0;
        private int _pageCount = 0;
        private const int cstSpanImgAndTitle = 1;
        private const int cstSpanBetweenItem = 6;
        private const int cstOffsetProperty = 2;
        private const int cstSpanBetweenProperty = 2;
        private ImageList _imageList = null;
        public delegate void OnClickHierItemHandler(object sender, HierItem hierItem);
        public event OnClickHierItemHandler OnClickHierItem;
        public delegate void OnAfterDrawHierItemsHandler(object sender, HierItem[] hierItems);
        public event OnAfterDrawHierItemsHandler OnAfterDrawHierItems = null;
        private RectangleF _prePageBounds1 = new RectangleF();
        private RectangleF _nxtPageBounds1 = new RectangleF();
        private RectangleF _prePageBounds2 = new RectangleF();
        private RectangleF _nxtPageBounds2 = new RectangleF();
        private RectangleF _firstPageBounds1 = new RectangleF();
        private RectangleF _firstPageBounds2 = new RectangleF();
        private RectangleF _lastPageBounds1 = new RectangleF();
        private RectangleF _lastPageBounds2 = new RectangleF();
        private RectangleF _displayKeyBounds1 = new RectangleF();
        private RectangleF _displayDatailBounds1 = new RectangleF();
        private RectangleF _displayKeyBounds2 = new RectangleF();
        private RectangleF _displayDatailBounds2 = new RectangleF();
        private RectangleF _clearBounds1 = new RectangleF();
        private RectangleF _clearBounds2 = new RectangleF();
        private int crtIndex = 0;
    
        public UCHierarchicalListBox()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        [Category("自定义属性")]
        public List<HierItem> Items
        {
            get { return _hierItems; }
        }

        [Category("自定义属性")]
        public int Indent
        {
            get { return _indent; }
            set 
            {
                if (value < 1)
                    _indent = 1;
                else if (value > 100)
                    _indent = 100;
                else
                    _indent = value;
            }
        }

        [Category("自定义属性")]
        public ImageList ImageList
        {
            get { return _imageList; }
            set { _imageList = value; }
        }

        public int CountPerPage
        {
            get { return _countPerPage; }
            set 
            {
                if (value < 1)
                    _countPerPage = 1;
                else if (value > 100)
                    _countPerPage = 100;
                _countPerPage = value;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (IsPageOperate(e))
                return;
            if (IsDisplayOperate(e))
                return;
            if (IsClearOperate(e))
                return;
            HierItem findItem = null;
            int endIdx = Math.Min(_hierItems.Count, _currentPage * _countPerPage + _countPerPage);
            for (int i = _currentPage * _countPerPage; i < endIdx; i++)
            {
                HierItem it = _hierItems[i];
                GetHieritemAt(e.Location,it, ref findItem);
                if (findItem != null)
                    break;
            }
            if (findItem != null)
            {
                if (findItem._imgBounds.Contains(e.Location))
                    findItem._isclosed = !findItem._isclosed;
                else
                    if (OnClickHierItem != null)
                        OnClickHierItem(this, findItem);
                Invalidate();
            }
        }

        private bool IsClearOperate(MouseEventArgs e)
        {
            bool isOK = false;
            if (_clearBounds1.Contains(e.Location) || _clearBounds2.Contains(e.Location))
            {
                ClearResult();
                isOK = true;
            }
            else if (_clearBounds1.Contains(e.Location) || _clearBounds2.Contains(e.Location))
            {
                ClearResult();
                isOK = true;
            }
            if (isOK)
            {
                if (OnAfterDrawHierItems != null)
                    OnAfterDrawHierItems(this, null);
                Invalidate();
            }
            return false;
        }

        private void ClearResult()
        {
            if (MsgBox.ShowQuestionYesNo("确定要清空查询结果吗？") == DialogResult.Yes)
            {
                _hierItems.Clear();
                _currentPage = 0;
            }
        }

        private bool IsDisplayOperate(MouseEventArgs e)
        {
            bool isOK = false;
            if (_displayKeyBounds1.Contains(e.Location) || _displayKeyBounds2.Contains(e.Location))
            {
                SetToClosed(true);
                isOK = true;
            }
            else if (_displayDatailBounds1.Contains(e.Location) || _displayDatailBounds2.Contains(e.Location))
            {
                SetToClosed(false);
                isOK = true;
            }
            if (isOK)
                Invalidate();
            return false;
        }

        private void SetToClosed(bool isclosed)
        {
            if (_hierItems == null || _hierItems.Count == 0)
                return;
            foreach (HierItem it in _hierItems)
                it._isclosed = isclosed;
        }

        private bool IsPageOperate(MouseEventArgs e)
        {
            bool isOK = false;
            if (_prePageBounds1.Contains(e.Location) || _prePageBounds2.Contains(e.Location))
            {
                if (_currentPage > 0)
                {
                    _currentPage--;
                    isOK = true;
                }
            }
            else if (_nxtPageBounds1.Contains(e.Location) || _nxtPageBounds2.Contains(e.Location))
            {
                if (_currentPage < _pageCount - 1)
                {
                    _currentPage++;
                    isOK = true;
                }
            }
            else if (_firstPageBounds1.Contains(e.Location) || _firstPageBounds2.Contains(e.Location))
            {
                _currentPage = 0;
                isOK = true;
            }
            else if (_lastPageBounds1.Contains(e.Location) || _lastPageBounds2.Contains(e.Location))
            {
                _currentPage = _pageCount - 1;
                isOK = true;
            }
            if(isOK)
                Invalidate();
            return isOK;
        }

        private void GetHieritemAt(Point point,HierItem parent, ref HierItem item)
        {
            if (parent._imgBounds.Contains(point) || parent._titleBounds.Contains(point))
            {
                item = parent;
                return;
            }
            if (parent.ChildCount > 0)
                foreach (HierItem it in parent.Children)
                    GetHieritemAt(point, it, ref item);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawHierItems(e);
        }

        private void DrawHierItems(PaintEventArgs e)
        {
            if (_hierItems == null || _hierItems.Count == 0)
            {
                e.Graphics.DrawString("没有查询到任何要素！", Font, Brushes.Black, 4, 10);
                return;
            }
            float y = 0;
            float maxwidth = 0;
            DrawPageInfos(e,ref y,true,ref maxwidth);
            //
            int hierCount = 0;
            int endIdx = Math.Min(_hierItems.Count, _currentPage * _countPerPage + _countPerPage);
            List<HierItem> drawedItems = null;
            if (OnAfterDrawHierItems != null)
                drawedItems = new List<HierItem>();
            crtIndex = 1;
            for(int i= _currentPage * _countPerPage;i<endIdx;i++)
            {
                HierItem it = _hierItems[i];
                if (it == null)
                    continue;
                DrawHierItem(e, it, ref hierCount, ref y,ref maxwidth);
                if (drawedItems != null)
                    drawedItems.Add(it);
                crtIndex++;
            }
            //
            if (OnAfterDrawHierItems != null)
                OnAfterDrawHierItems(this, drawedItems.ToArray());
            //
            DrawPageInfos(e, ref y,false,ref maxwidth);
            //
            Height = (int)y;
            Width = (int)Math.Max(Width, maxwidth);
        }

        private void DrawPageInfos(PaintEventArgs e, ref float y,bool isPre,ref float maxwidth)
        {
            y += 8;
            if (!isPre)
            {
                e.Graphics.DrawLine(Pens.Gray, 0, y, Width, y);
                y += 4;
            }
            float x = 2;
            _pageCount = (int)Math.Ceiling((double)(_hierItems.Count / (float)_countPerPage));
            string txt = "共查询到 {0} 条记录 每页显示 {1} 条";
            SizeF sizf = e.Graphics.MeasureString(txt, Font);
            txt = string.Format(txt, _hierItems.Count, _countPerPage);
            e.Graphics.DrawString(txt, Font, Brushes.Black, 0, y);
            maxwidth = Math.Max(maxwidth, x + sizf.Width);
            y += (sizf.Height + 2);
            //
            x = 2;
            txt = "显示第 {0}/{1} 页";
            sizf = e.Graphics.MeasureString(txt, Font);
            txt = string.Format(txt, (_currentPage + 1).ToString(),_pageCount);
            e.Graphics.DrawString(txt, Font, Brushes.Black, 0, y);
            y += (sizf.Height + 2);
            //
            txt = "第一页";
            x = 2;
            sizf = e.Graphics.MeasureString(txt, Font);
            e.Graphics.DrawString(txt, Font, Brushes.Blue, x, y);
            if (isPre)
                _firstPageBounds1 = new RectangleF(x, y, sizf.Width, sizf.Height);
            else
                _firstPageBounds2 = new RectangleF(x, y, sizf.Width, sizf.Height);
            //
            x += (sizf.Width + 1);
            txt = "前一页";
            sizf = e.Graphics.MeasureString(txt, Font);
            e.Graphics.DrawString(txt, Font, Brushes.Blue, x, y);
            if(isPre)
                _prePageBounds1 = new RectangleF(x, y, sizf.Width, sizf.Height);
            else
                _prePageBounds2 = new RectangleF(x, y, sizf.Width, sizf.Height);
            //
            txt = "后一页";
            x += (sizf.Width + 1);
            sizf = e.Graphics.MeasureString(txt, Font);
            e.Graphics.DrawString(txt, Font, Brushes.Blue, x, y);
            if(isPre)
                _nxtPageBounds1 = new RectangleF(x, y, sizf.Width, sizf.Height);
            else
                _nxtPageBounds2 = new RectangleF(x, y, sizf.Width, sizf.Height);
            //
            txt = "最后一页";
            x += (sizf.Width + 1);
            sizf = e.Graphics.MeasureString(txt, Font);
            e.Graphics.DrawString(txt, Font, Brushes.Blue, x, y);
            if (isPre)
                _lastPageBounds1 = new RectangleF(x, y, sizf.Width, sizf.Height);
            else
                _lastPageBounds2 = new RectangleF(x, y, sizf.Width, sizf.Height);
            //
            txt = "显示摘要";
            x = 2;
            y += (sizf.Height + 2 + 2);
            sizf = e.Graphics.MeasureString(txt, Font);
            e.Graphics.DrawString(txt, Font, Brushes.Blue, x, y);
            if(isPre)
                _displayKeyBounds1 = new RectangleF(x, y, sizf.Width, sizf.Height);
            else
                _displayKeyBounds2 = new RectangleF(x, y, sizf.Width, sizf.Height);
            //
            txt = "显示详细";
            x += (sizf.Width + 1);
            e.Graphics.DrawString(txt, Font, Brushes.Blue, x, y);
            if(isPre)
                _displayDatailBounds1 = new RectangleF(x, y, sizf.Width, sizf.Height);
            else
                _displayDatailBounds2 = new RectangleF(x, y, sizf.Width, sizf.Height);
            //
            txt = "清空所有";
            x += (sizf.Width + 17);
            e.Graphics.DrawString(txt, Font, Brushes.Blue, x, y);
            if (isPre)
                _clearBounds1 = new RectangleF(x, y, sizf.Width, sizf.Height);
            else
                _clearBounds2 = new RectangleF(x, y, sizf.Width, sizf.Height);
            //
            y += (sizf.Height + 2);
            if (isPre)
            {
                e.Graphics.DrawLine(Pens.Gray, 0, y, Width, y);
                y += 4;
            }
        }

        private void DrawHierItem(PaintEventArgs e, HierItem it, ref int hierCount, ref float y,ref float maxwidth)
        {
            float x = hierCount * _indent;
            float oldx = x;
            Color color = ForeColor;
            Font font = GetFont(it,out color);
            //draw image
            Image img = GetImage(it,crtIndex,e.Graphics);
            if (img != null)
            {
                e.Graphics.DrawImageUnscaled(img, (int)x, (int)y);
                x += (img.Width + cstSpanImgAndTitle);
            }
            //draw title
            SizeF fontsize = e.Graphics.MeasureString(it.Title, font);
            using (Brush brush = new SolidBrush(color))
            {
                e.Graphics.DrawString(it.Title, font, brush, x, y);
                maxwidth = Math.Max(maxwidth, x + fontsize.Width);
            }
            it._imgBounds = new RectangleF(oldx, y, img.Width, img.Height);
            it._titleBounds = new RectangleF(x, y, x + fontsize.Width, fontsize.Height);
            y += fontsize.Height;
            //draw properties
            if (!it._isclosed && it.Properties != null && it.Properties.Count > 0)
            {
                float maxLen = GetMaxLenOfPropertyName(e.Graphics,font, it.Properties.Keys);
                float px = x + cstOffsetProperty;
                y += cstSpanBetweenProperty;
                using (Brush brush = new SolidBrush(ForeColor))
                {
                    foreach (string name in it.Properties.Keys)
                    {
                        e.Graphics.DrawString(name, Font, brush, px, y);
                        e.Graphics.DrawString(":" + it.Properties[name],Font, brush, px + maxLen, y);
                        y += fontsize.Height;
                        y += cstSpanBetweenProperty;
                        //
                        fontsize = e.Graphics.MeasureString(":" + it.Properties[name], Font);
                        maxwidth = Math.Max(maxwidth, px + maxLen + fontsize.Width);
                    }
                }
            }
            y += cstSpanBetweenItem;
            //draw children
            if (!it._isclosed && it.ChildCount > 0)
            {
                hierCount++;
                foreach (HierItem child in it.Children)
                {
                    DrawHierItem(e, child, ref hierCount, ref y,ref maxwidth);
                }
                hierCount--;
            }
        }

        private float GetMaxLenOfPropertyName(Graphics g,Font font, Dictionary<string, string>.KeyCollection keyCollection)
        {
            float maxLen = 0;
            SizeF sizef = SizeF.Empty;
            foreach (string name in keyCollection)
            {
                sizef = g.MeasureString(name, font);
                if (sizef.Width > maxLen)
                    maxLen = sizef.Width;
            }
            return maxLen;
        }

        private Font GetFont(HierItem it,out Color color)
        {
            if (it.IsHotlink)
            {
                color = Color.Blue;
                return new Font(Font.FontFamily, Font.Size, FontStyle.Underline);
            }
            else
            {
                color = ForeColor;
                return Font;
            }
        }

        private Image GetImage(HierItem it,int i,Graphics g)
        {
            if (it.Image != null)
                return it.Image;
            if (_imageList != null && _imageList.Images != null && it.ImageIndex >= 0 && it.ImageIndex < _imageList.Images.Count)
                return _imageList.Images[it.ImageIndex];
            Image img = imageList1.Images[0];
            SizeF fontsize = g.MeasureString(i.ToString(), Font);
            using (Graphics tg = Graphics.FromImage(img))
            {
                tg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                tg.DrawImage(img, 0,0);
                tg.DrawString(i.ToString(), Font, Brushes.Red,  (img.Width - fontsize.Width) / 2, (img.Height - fontsize.Height) / 2 -1);
            }
            return img;
        }
    }
}
