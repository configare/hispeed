using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Text;

namespace CodeCell.AgileMap.Core
{
    public partial class UCTrueTypeBrowser : UserControl
    {
        private Font _font = null;
        private Font _largeFont = null;
        private CharItem[] _charItems = null;
        private CharItem _currentCharItem = null;
        internal delegate void CurrentCharItemChangedHandler(CharItem it);
        internal event CurrentCharItemChangedHandler CurrentCharItemChanged;
        private static Pen _pen = new Pen(Brushes.Black, 2);

        public UCTrueTypeBrowser()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public CharItem SelectedCharItem
        {
            get { return _currentCharItem; }
        }

        public void ChangeFontName(string fontname)
        {
            _font = new Font(fontname, 12, FontStyle.Regular);
            _largeFont = new Font(fontname, 32, FontStyle.Bold);
            _charItems= new CharItem[256];
            for (int i = 0; i <= 255; i++)
            {
                _charItems[i] = new CharItem((byte)i,_font);
            }
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_charItems == null || _charItems.Length == 0)
                return;
            foreach (CharItem it in _charItems)
            {
                if (it.Bounds.Contains(e.Location))
                {
                    _currentCharItem = it;
                    Invalidate();
                    return;
                 }
            }
            _currentCharItem = null;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_currentCharItem != null)
            {
                if (CurrentCharItemChanged != null)
                    CurrentCharItemChanged(_currentCharItem);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_charItems == null || _charItems.Length == 0)
                return;
            int x = 0;
            int y = 0;
            SizeF fontSize = e.Graphics.MeasureString("FONT",_font);
            SizeF largefontSize = e.Graphics.MeasureString("FONT", _largeFont);
            string text = null;
            foreach (CharItem it in _charItems)
            {
                text = new string(Convert.ToChar(it.Code), 1);
                e.Graphics.DrawString(text, _font, Brushes.Black, x, y);
                it.Bounds = new Rectangle(x, y, (int)fontSize.Width, (int)fontSize.Height);
                if (x > Width - (int)fontSize.Width)
                {
                    x = 0;
                    y+=((int)fontSize.Height);
                }
                x += ((int)fontSize.Width);
            }
            if (_currentCharItem != null)
            {
                text = new string(Convert.ToChar(_currentCharItem.Code), 1);
                e.Graphics.FillRectangle(Brushes.White, new Rectangle(_currentCharItem.Bounds.Left - 10, _currentCharItem.Bounds.Top - 10, 46, 46));
                e.Graphics.DrawRectangle(_pen, new Rectangle(_currentCharItem.Bounds.Left - 10, _currentCharItem.Bounds.Top - 10, 46, 46));
                e.Graphics.DrawString(text, _largeFont, Brushes.Red, _currentCharItem.Bounds.Left - 10, _currentCharItem.Bounds.Top - 10);
            }
            if (y > Height)
                Size = new Size(Width, y);
        }
    }

    [Serializable]
    public class CharItem
    {
        public byte Code = 0;
        internal Rectangle Bounds = Rectangle.Empty;
        public string Text = null;
        public Font Font = null;

        public CharItem(byte code,Font font)
        {
            Code = code;
            Text = new string(Convert.ToChar(code), 1);
            Font = font;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
