using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///	Represents a menu item which has a combobox placed inside.
    /// </summary>
    public class RadMenuHostItem : RadMenuItemBase
    {
        // Fields
        private Control hostedControl; 

        public RadMenuHostItem(Control control)
        {
            this.hostedControl = control;
            this.Children.Add(new RadHostItem(this.hostedControl));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "RadMenuHostItem";
            this.HandlesKeyboard = true;
        }

        #region Properties

        /// <summary>
        ///	Provides a reference to the hosted control in the menu item.
        /// </summary>
        [Browsable(false)]
        [Description("Provides a reference to the ComboBox element in the menu item.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control HostedControl
        {
            get { return this.hostedControl; }
        }
        
        #endregion

        protected override void CreateChildElements()
        {

        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {            
            SizeF size = base.MeasureOverride(availableSize);
            if (this.hostedControl != null)
            {
                //TFS:106664
                //We need to extract the following values from the width because the menu layout adds them on upper levels 
                //(RadDropDownMenuLayout, RadDropDownMenuElement). Other RadMenuItems handle the added values while
                //RadMenuHostItem fits the hosted control to the available size and the dropdown size is increased everytime. 
                RadDropDownMenuLayout menuLayout = this.Parent as RadDropDownMenuLayout;
                size = this.hostedControl.Size;
                if (menuLayout != null)
                {
                    size.Width -= 2 * menuLayout.LeftColumnWidth + menuLayout.LeftColumnMaxPadding + menuLayout.Padding.Horizontal;
                }

                return size;
            }
            return size;
        }
    }
}