using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using Telerik.WinControls.Design;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents a Form that hosts a RadRibbonBar control and extends the behavior
    /// of a standard form by providing Office 2007 form-like appearance.
    /// </summary>
    [ToolboxItem(false)]
    [RadThemeDesignerData(typeof(RadRibbonFormThemeDesignerData))]
    public class RadRibbonForm : RadFormControlBase
    {
        #region Fields

        #endregion

        #region Constructor

        static RadRibbonForm()
        {
            RuntimeHelpers.RunClassConstructor(typeof(RibbonFormElement).TypeHandle);
        }

        #endregion

        #region Properties

        [Browsable(false)]
        public new FormBorderStyle FormBorderStyle
        {
            get
            {
                return base.FormBorderStyle;
            }
            set
            {
            }
        }

        public RibbonFormElement FormElement
        {
            get
            {
                RadRibbonFormBehavior formBehavior = this.FormBehavior as RadRibbonFormBehavior;
                if (formBehavior != null)
                {
                    return formBehavior.FormElement as RibbonFormElement;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the RadRibbonBar control associated with this form.
        /// </summary>
        public RadRibbonBar RibbonBar
        {
            get
            {
                foreach (Control control in this.Controls)
                {
                    if (control is RadRibbonBar)
                    {
                        return control as RadRibbonBar;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that determines 
        /// whether Vista Aero effects are enabled.
        /// </summary>
        public bool AllowAero
        {
            get
            {
                return (this.FormBehavior as RadRibbonFormBehavior).AllowTheming;
            }
            set
            {
                (this.FormBehavior as RadRibbonFormBehavior).AllowTheming = value;
            }
        }


        public override string ThemeClassName
        {
            get
            {
                return "Telerik.WinControls.UI.RadRibbonForm";
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        #endregion

        #region Methods

        protected override FormControlBehavior InitializeFormBehavior()
        {
            return new RadRibbonFormBehavior(this, true);
        }

        #endregion

        #region Design time support

        private bool IsRibbonBarInForm()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is RadRibbonBar)
                    return true;
            }

            return false;
        }

        protected virtual void AddRibbonBarInForm()
        {
            IDesignerHost designerHost = this.GetService(typeof(IDesignerHost)) as IDesignerHost;

            if (designerHost == null)
            {
                return;
            }

            RadRibbonBar ribbonBar = designerHost.CreateComponent(typeof(RadRibbonBar)) as RadRibbonBar;

            if (ribbonBar == null)
            {
                return;
            }

            IComponentChangeService componentChangeService = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            if (componentChangeService == null)
            {
                return;
            }

            ribbonBar.Dock = DockStyle.Top;
            componentChangeService.OnComponentChanging(this, TypeDescriptor.GetProperties(this)["Controls"]);
            this.Controls.Add(ribbonBar);
            componentChangeService.OnComponentChanged(this, TypeDescriptor.GetProperties(this)["Controls"], null, null);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (this.IsDesignMode)
            {
                if (!this.IsRibbonBarInForm())
                {
                    this.AddRibbonBarInForm();
                }
            }
        }

        #endregion
    }
}
