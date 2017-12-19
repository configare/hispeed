using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace GeoDo.RSS.Layout
{
    public abstract class Element : IElement, IRenderable, IDisposable, IPersitable
    {
        protected string _name;
        protected bool _visible = true;
        protected bool _isLocked = false;
        protected bool _isSelected = false;
        protected Image _icon = null;
        protected ILayoutRuntime _runtime = null;

        public Element()
        {
        }

        [PersistAttribute(), DisplayName("名字"),Category("设计")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [PersistAttribute(), DisplayName("是否可见"), Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (!IsSystemElements(this))
                    _visible = value;
            }
        }

        [PersistAttribute(), DisplayName("是否锁定"), Category("设计")]
        public bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                if (!IsSystemElements(this))
                    _isLocked = value;
            }
        }

        [Browsable(false)]
        public Image Icon
        {
            get { return _icon; }
        }

        [DisplayName("是否选中"), Category("状态")]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (!IsSystemElements(this))
                {
                    if (_isSelected != value)
                    {
                        _isSelected = value;
                    }
                }
            }
        }

        private static bool IsSystemElements(IElement ele)
        {
            return ele is ILayout || ele is IBorder;
        }

        public virtual void Render(object sender, IDrawArgs drawArgs)
        {
            TryAttachSelectedEditBox(sender);
        }

        private void TryAttachSelectedEditBox(object sender)
        {
             _runtime = sender as ILayoutRuntime;
            if (_isSelected && this is ISizableElement)
               _runtime.SelectedEditBoxManager.Attach(this as ISizableElement);
        }

        public virtual bool IsHited(float layoutX, float layoutY)
        {
            return false;
        }

        public virtual bool IsHited(RectangleF layoutRect)
        {
            return false;
        }

        public virtual void Dispose()
        {
        }

        public virtual void InitByXml(XElement xml)
        {
            string att = null;
            if (xml.Attribute("visible") != null)
            {
                att = xml.Attribute("visible").Value;
                if (att != null)
                    _visible = bool.Parse(att);
            }
            if (xml.Attribute("name") != null)
                _name = xml.Attribute("name").Value;
            if (xml.Attribute("islocked") != null)
            {
                att = xml.Attribute("islocked").Value;
                if (att != null)
                    _isLocked = bool.Parse(att);
            }
        }

        public static Font AjustFontByScale(Font font, float scale)
        {
            float fontSize = font.Size * scale;
            string fontName = font.Name;
            FontStyle fontStyle = font.Style;
            GraphicsUnit fontUnit = font.Unit;
            byte gdiCharSet = font.GdiCharSet;
            bool gdiVerticalFont = font.GdiVerticalFont;
            int height = font.Height; //字体的行距
            return new Font(fontName, fontSize, fontStyle, fontUnit, gdiCharSet, gdiVerticalFont);
        }
    }
}
