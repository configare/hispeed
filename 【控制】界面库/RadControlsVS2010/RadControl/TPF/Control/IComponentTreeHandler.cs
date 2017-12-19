using System;
using System.Drawing;
//using System.Web.UI.Design;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls
{
    public interface IComponentTreeHandler : ILayoutHandler
    {
        /// <summary>
        /// Returns the value for some ambient properties like BackColor, ForelColor, Font, etc.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        object GetAmbientPropertyValue(RadProperty property);

        /// <summary>
        /// Updates after a change in an ambient property like BackColor, ForeColor, Font, etc.
        /// </summary>
        /// <param name="property"></param>
        void OnAmbientPropertyChanged(RadProperty property);

        void InitializeRootElement(RootRadElement rootElement);

        void LoadElementTree(Size size);

        void LoadElementTree();

        RootRadElement CreateRootElement();

        void CreateChildItems(RadElement parent);

        event ThemeNameChangedEventHandler ThemeNameChanged;

        event ToolTipTextNeededEventHandler ToolTipTextNeeded;

        void CallOnThemeNameChanged(ThemeNameChangedEventArgs e);

        void CallSetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified);

        Size CallGetPreferredSize(Size proposedSize);

        void CallOnLayout(LayoutEventArgs e);

        void InvalidateIfNotSuspended();

        bool GetShowFocusCues();

        void OnDisplayPropertyChanged(RadPropertyChangedEventArgs e);

        bool IsDesignMode { get; }

        void CallOnMouseCaptureChanged(EventArgs e);

        void CallOnToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e);

        //bool ShouldShowFocusCues { get;}

        ComponentThemableElementTree ElementTree { get; }

        ComponentInputBehavior Behavior { get; }

        string ThemeClassName { get; set; }

        string ThemeName { get; set; }

        string Name { get; set; }

        RootRadElement RootElement { get; }

        void InvalidateElement(RadElement element);

        void InvalidateElement(RadElement element, Rectangle bounds);

        bool OnFocusRequested(RadElement element);
        bool OnCaptureChangeRequested(RadElement element, bool capture);

        void SuspendUpdate();

        void ResumeUpdate();

        ImageList SmallImageList { get; set; }

        ImageList ImageList { get; set; }

        Size SmallImageScalingSize { get; set; }

        Size ImageScalingSize { get; set; }

        RadControlDesignTimeData DesignTimeData { get; }
        bool Initializing { get; }

        void RegisterHostedControl(RadHostItem hostElement);

        void UnregisterHostedControl(RadHostItem hostElement, bool removeControl);

        bool ControlDefinesThemeForElement(RadElement element);

        void CallOnScreenTipNeeded(object sender, ScreenTipNeededEventArgs e);
        void ControlThemeChangedCallback();
    }
}
