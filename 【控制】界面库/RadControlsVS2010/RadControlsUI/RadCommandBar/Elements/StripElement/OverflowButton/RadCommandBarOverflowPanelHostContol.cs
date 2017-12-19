using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Styles;
using System;
using System.ComponentModel;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    [ToolboxItem(false)]
    public class RadCommandBarOverflowPanelHostContol : RadControl
    {
        RadCommandBarOverflowPanelElement element;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [Description("Represent RadCommandBarOverflowPanelElement")]
        public RadCommandBarOverflowPanelElement Element
        {
            get
            {
                return element;
            }
            set
            {
                element = value;
            }
        }

        public override string ThemeClassName
        {
            get
            {
                return typeof(RadCommandBar).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            this.element = new RadCommandBarOverflowPanelElement();
            this.RootElement.Children.Add(this.element);
            this.AutoSize = true;
            this.RootElement.StretchVertically = false;
            this.RootElement.StretchHorizontally = true;
        }

        public LayoutPanel LayoutPanel
        {
            get
            {
                return this.element.Layout;
            }
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            return (
                element is RadTextBoxElement ||
                element is RadDropDownListArrowButtonElement ||
                element is RadDropDownListEditableAreaElement ||
                element is RadDropDownListElement ||
                element is RadArrowButtonElement);
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }
    }
}
