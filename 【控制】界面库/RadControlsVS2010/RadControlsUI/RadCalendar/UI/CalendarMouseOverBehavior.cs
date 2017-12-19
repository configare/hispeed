using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class CalendarMouseOverBehavior : PropertyChangeBehavior
    {
        private bool popupShown;


        public CalendarMouseOverBehavior()
            : base(RadItem.IsMouseOverProperty)
        {
        }

        public override void OnPropertyChange(RadElement element, RadPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                if (!popupShown)
                {
                    popupShown = true;
                    ZoomPopup popup = new ZoomPopup(element, new SizeF(2f, 2f));
                    popup.AnimationFrames = 10;
                    popup.AnimationInterval = 20;
                    popup.UseNewLayoutSystem = true;
                    popup.BeginInit();
                    popup.Closed += delegate(object sender, EventArgs ea)
                    {
                        popupShown = false;
                        ThemeResolutionService.UnsubscribeFromThemeChanged(((ZoomPopup)sender).ElementTree);
                    };

                    popup.EndInit();
                    popup.Show();
                }
            }
        }
    }
}
