using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadPageViewStripButtonElement : RadPageViewButtonElement
    {
        #region Overrides

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ImageAlignment = ContentAlignment.MiddleCenter;
        }

        protected internal override void SetContentOrientation(PageViewContentOrientation orientation, bool recursive)
        {
            StripViewButtons buttons = (StripViewButtons)this.Tag;
            switch(buttons)
            {
                //do not rotate scroll buttons by 270 degrees
                case StripViewButtons.LeftScroll:
                case StripViewButtons.RightScroll:
                    if (orientation == PageViewContentOrientation.Vertical270)
                    {
                        orientation = PageViewContentOrientation.Vertical90;
                    }
                    break;
                //do not rotate close button
                case StripViewButtons.Close:
                    orientation = PageViewContentOrientation.Horizontal;
                    break;
            }

            base.SetContentOrientation(orientation, recursive);
        }

        #endregion
    }
}
