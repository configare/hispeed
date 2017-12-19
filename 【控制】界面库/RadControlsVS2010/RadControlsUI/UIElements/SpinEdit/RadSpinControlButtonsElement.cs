using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Drawing;
using Telerik.WinControls.Design;
using System.Collections;
using System.Globalization;
using Telerik.WinControls;
using System.Diagnostics;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI.Design;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    public class RadSpinControlButtonsElement : RadItem
    {
        #region Fields
        //Children    
        private BoxLayout layout;
        private RadRepeatArrowElement arrowButtonUp;
        private RadRepeatArrowElement arrowButtonDown;
        #endregion

        #region Accessors

        public RadRepeatArrowElement ButtonUp
        {
            get
            {
                return this.arrowButtonUp;
            }
        }
        public RadRepeatArrowElement ButtonDown
        {
            get
            {
                return this.arrowButtonDown;
            }
        }
       #endregion

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.layout = new BoxLayout();
            this.layout.Orientation = Orientation.Vertical;
            this.Children.Add(layout);

            this.arrowButtonUp = new RadRepeatArrowElement();
            this.arrowButtonUp.StretchVertically = false;
            this.arrowButtonUp.Direction = ArrowDirection.Up;
            this.arrowButtonUp.Border.Visibility = ElementVisibility.Hidden;
            this.arrowButtonUp.Size = new Size(10, 6);
            this.layout.Children.Add(this.arrowButtonUp);
            this.arrowButtonUp.BorderThickness = new Padding(0);

            this.arrowButtonDown = new RadRepeatArrowElement();
            this.arrowButtonDown.StretchVertically = false;
            this.arrowButtonDown.Direction = ArrowDirection.Down;
            this.arrowButtonDown.Border.Visibility = ElementVisibility.Hidden;
            this.arrowButtonDown.Size = new Size(10, 6);
            this.layout.Children.Add(this.arrowButtonDown);
            this.arrowButtonDown.BorderThickness = new Padding(0);
        }

    }



}
