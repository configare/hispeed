using Telerik.WinControls.Primitives;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System;
using System.Text;
using Telerik.WinControls;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// 	<para>Represents a title bar element. All logic and UI functionality
    ///     is implemented in the RadFormTitleBarElement class.</para>
    /// 	<para>You can use RadFormTitleBarElement events to substitute the title bar in a
    ///     borderless application.</para>
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    public class RadFormTitleBarElement : RadTitleBarElement, INotifyPropertyChanged
    {
        static RadFormTitleBarElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadFormTitleBarElementStateManager(), typeof(RadFormTitleBarElement));
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadTitleBar().DeserializeTheme();
        }

        #region Overrides

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadTitleBarElement);
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.layout.Children.Remove(this.titleBarIcon);
            this.layout.Children.Add(this.titleBarIcon);
            this.layout.Children.Remove(this.caption);
            this.layout.Children.Add(this.caption);
        }

        public override void HandleMouseMove(MouseEventArgs e, Form form)
        {
        }

        #endregion
    }
}