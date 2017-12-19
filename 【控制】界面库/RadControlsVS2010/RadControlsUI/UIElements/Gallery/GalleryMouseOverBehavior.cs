using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class GalleryMouseOverBehavior : PropertyChangeBehavior
    {
        private bool popupShown;
        private RadGalleryElement gallery;

        public GalleryMouseOverBehavior(RadGalleryElement gallery)
            : base(RadItem.IsMouseOverProperty)
        {
            this.gallery = gallery;
        }

        public bool IsPopupShown
        {
            get
            {
                return this.popupShown;
            }
        }

        public void ClosePopup()
        {
            if (this.popup != null && this.popup.Visible)
            {
                this.popup.Hide();
            }
        }

        private ZoomPopup popup;
        public override void OnPropertyChange(RadElement element, RadPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                if (!this.IsPopupShown)
                {
                    this.popupShown = true;
                    this.popup = new ZoomPopup(element, new SizeF(1.5f, 1.5f), false);
                    this.popup.BeginInit();
                    this.popup.Closed += delegate(object sender, EventArgs ea)
                    {
                        this.popupShown = false;
                        this.popup.Dispose();
                        if (element.Parent != null)
                        {
                            element.Parent.UpdateLayout();
                        }
                    };
                    this.popup.Clicked += delegate(object sender, EventArgs ea) { gallery.CloseDropDown(); };
                    this.popup.EndInit();
                    this.popup.Show();
                }
            }
        }
    }
}
