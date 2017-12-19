using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the dialog form shown to the user when they drop
    /// a RadRibbonBar control on a RadForm control in the Visual Studio designer.
    /// </summary>
    [ToolboxItem(false)]
    public partial class RadFormDesignerRibbonDialog : RadForm
    {
        #region Constructor

        /// <summary>
        /// Creates an instance of the RadFormDesignerRibbonDialog
        /// </summary>
        public RadFormDesignerRibbonDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void radBtnYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void radBtnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        #endregion
    }
}
