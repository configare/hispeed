using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Telerik.WinControls.Paint;
using Telerik.WinControls.UI;
using System.Reflection;
using System.Collections.Specialized;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the Telerik's Form control.
    /// You can create RadForm controls by inheriting from this class.
    /// </summary>
    //[Designer(typeof(RadFormDesigner), typeof(DocumentDesigner))]
    [RadThemeDesignerData(typeof(RadFormThemeDesignerData))]
    [ToolboxItem(false)]
    public partial class RadForm : RadFormControlBase
    {

        #region Fields

        #endregion

        #region Constructor

        public RadForm()
        {
            InitializeComponent();
            this.Behavior.BitmapRepository.DisableBitmapCache = true;  
        }

        #endregion

        #region Properties

        public override string ThemeClassName
        {
            get
            {
                if (this.FormBehavior is RadRibbonFormBehavior)
                {
                    return "Telerik.WinControls.UI.RadRibbonForm";
                }

                return "Telerik.WinControls.UI.RadForm";
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        /// <summary>
        /// Gets the RadFormElement instance that represents
        /// the element hierarchy which builds the RadForm appearance.
        /// </summary>
        public RadFormElement FormElement
        {
            get
            {
                return this.FormBehavior.FormElement as RadFormElement;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether the Form
        /// customizes its NC area when under Vista with Composition enabled.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool AllowTheming
        {
            get
            {
                if (this.FormBehavior is RadFormBehavior)
                {
                    return (this.FormBehavior as RadFormBehavior).AllowTheming;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (this.FormBehavior is RadFormBehavior)
                {
                    (this.FormBehavior as RadFormBehavior).AllowTheming = value;
                }
            }
        }


        #endregion

        #region Methods

        #region Overriden

        /// <summary>
        /// Prevent the Form from getting the mouse capture when the capture is requested
        /// by one of the system buttons.
        /// </summary>
        protected override bool ProcessCaptureChangeRequested(RadElement element, bool capture)
        {
            if (element.IsChildOf(this.FormElement) && !(element is ScrollBarThumb))
            {
                return false;
            }

            return base.ProcessCaptureChangeRequested(element, capture);
        }

        protected override FormControlBehavior InitializeFormBehavior()
        {
            return new RadFormBehavior(this, true);
        }

        #endregion

        #endregion

    }
}
