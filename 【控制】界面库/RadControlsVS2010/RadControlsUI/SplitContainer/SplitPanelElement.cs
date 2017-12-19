using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    /// <summary>The main element of the <strong>RadPanel</strong> control.</summary>
    public class SplitPanelElement : RadItem
    {
        private BorderPrimitive borderPrimitive;
        private FillPrimitive fillPrimitive;

        static SplitPanelElement()
        {
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_SplitPanel().DeserializeTheme();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ShouldHandleMouseInput = false;
        }
        
        [Browsable(false)]
        public BorderPrimitive Border
        {
            get
            {
                return this.borderPrimitive;
            }
        }

        [Browsable(false)]
        public FillPrimitive Fill
        {
            get
            {
                return this.fillPrimitive;
            }
        }

        /// <summary>Create the elements in the hierarchy.</summary>
        protected override void CreateChildElements()
        {
            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "SplitContainerBorder";

            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.Class = "SplitContainerFill";

            this.Children.Add(this.fillPrimitive);
            this.Children.Add(this.borderPrimitive);
        }
    }
}
