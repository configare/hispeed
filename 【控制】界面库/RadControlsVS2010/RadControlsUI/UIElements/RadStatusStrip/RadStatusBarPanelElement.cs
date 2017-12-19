using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{

    /// <summary>
    /// represent the RadStatusBarPanelElement
    /// </summary>
    public class RadStatusBarPanelElement : RadItem
    {
        //members
        private BorderPrimitive borderPrimitive;
        private FillPrimitive fillPrimitive;
        private TextPrimitive textPrimitive;
        private ImagePrimitive imagePrimitive;

        static RadStatusBarPanelElement()
        {
            ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Resources.RadPanelCalendarExamples.xml");
            ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Resources.RadPanelBlack.xml");
        }

        /// <summary>
        /// create child items
        /// </summary>
        protected override void CreateChildElements()
        {
            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "RadPanelBorder";
            this.borderPrimitive.BoxStyle = BorderBoxStyle.OuterInnerBorders;

            this.fillPrimitive = new FillPrimitive();
            fillPrimitive.BackColor = Color.Transparent;
            fillPrimitive.BackColor2 = Color.Transparent;
            fillPrimitive.BackColor3 = Color.Transparent;
            fillPrimitive.BackColor4 = Color.Transparent;
            fillPrimitive.Class = "RadPanelFill";

            this.textPrimitive = new TextPrimitive();
            this.textPrimitive.Alignment = ContentAlignment.MiddleLeft;
            this.textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadItem.TextProperty, PropertyBindingOptions.TwoWay);

            this.imagePrimitive = new ImagePrimitive();
            this.imagePrimitive.Alignment = ContentAlignment.MiddleLeft;
            

            this.Children.Add(this.fillPrimitive);
            this.Children.Add(this.borderPrimitive);
            this.Children.Add(this.textPrimitive);
            this.Children.Add(this.imagePrimitive);
        }
        
    }
}
