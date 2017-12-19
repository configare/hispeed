using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public class FloatToolItem
    {
        public string Text;
        public Image Image;
        public bool IsSelected = false;

        public FloatToolItem()
        { 
        }

        public FloatToolItem(string spliter)
        {
            Text = spliter;
        }

        public FloatToolItem(string text, Image image)
        {
            Text = text;
            Image = image;
        }
    }
}
