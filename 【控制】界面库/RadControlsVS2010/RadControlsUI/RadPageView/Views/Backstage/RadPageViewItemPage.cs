using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public enum PageViewItemType
    {
        GroupHeaderItem
    }

    public class RadPageViewItemPage : RadPageViewPage
    {
        private PageViewItemType itemType;
                 
        public PageViewItemType ItemType
        {
            get
            {
                return itemType;
            }
            set
            {
                itemType = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Visible
        {
            get
            {
                return false;
            }
            set
            {
                base.Visible = value;
            }
        }
    }
}
