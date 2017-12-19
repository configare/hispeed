using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadPageViewStackItem : RadPageViewItem
    {
        #region Ctor

        public RadPageViewStackItem()
        {
        }

        public RadPageViewStackItem(string text)
            : this()
        {
            this.Text = text;
        }

        public RadPageViewStackItem(string text, Image image)
            : this(text)
        {
            this.Image = image;
        }

        #endregion
    }
}

