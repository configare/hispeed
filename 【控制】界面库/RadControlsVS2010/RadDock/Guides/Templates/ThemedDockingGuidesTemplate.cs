using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    public class ThemedDockingGuidesTemplate: IDockingGuidesTemplate
    {
        DockingGuidesControl control;

        #region Properties

        public string ThemeName
        {
            get { return control.ThemeName; }
            set { control.ThemeName = value; }
        }

        public DockingGuidesControl GuidesControl
        {
            get { return control; }
        }

        #endregion

        #region Initialization

        public ThemedDockingGuidesTemplate()
        {
            control = new DockingGuidesControl();
            control.LoadElementTree();            
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (control != null)
            {
                control.Dispose();
                control = null;
            }
        }

        #endregion

        #region IDockingGuidesTemplate Members

        public int DockingHintBorderWidth
        {
            get
            {
                return control.GuidesElement.DockingHintBorderWidth;
            }
        }

        public Color DockingHintBackColor
        {
            get
            {
                return control.GuidesElement.DockingHintBackColor;
            }
        }

        public Color DockingHintBorderColor
        {
            get
            {
                return control.GuidesElement.DockingHintBorderColor;
            }
        }

        public IDockingGuideImage LeftImage
        {
            get { return control.GuidesElement.LeftImage; }
        }

        public IDockingGuideImage RightImage
        {
            get { return control.GuidesElement.RightImage; }
        }

        public IDockingGuideImage TopImage
        {
            get { return control.GuidesElement.TopImage; }
        }

        public IDockingGuideImage BottomImage
        {
            get { return control.GuidesElement.BottomImage; }
        }

        public IDockingGuideImage FillImage
        {
            get { return control.GuidesElement.FillImage; }
        }

        public IDockingGuideImage CenterBackgroundImage
        {
            get { return control.GuidesElement.CenterBackgroundImage; }
        }

        #endregion
    }
}
