using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.WindowAnimation;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents a base class for popup controls
    /// used by editors like ComboBox, MultiColumnComboBox etc.
    /// </summary>
    [ToolboxItem(false)]
    //TODO: All common logic for popups of popup-editors should be put
    //      in this class after the refactoring in RadCombo/RadMultiColumnCombo/RadDTPicker etc.
    public class RadEditorPopupControlBase : RadSizablePopupControl
    {

        #region Constructor

        /// <summary>
        /// Creates an instance of the RadEditorPopupControlBase class.
        /// This class is used in all popup-powered controls in the
        /// RadControls for WinForms suite.
        /// </summary>
        /// <param name="owner">An instance of the RadItem class that 
        /// represents the owner of the popup.</param>
        public RadEditorPopupControlBase(RadItem owner)
            : base(owner)
        {
            this.FadeAnimationType = FadeAnimationType.FadeOut;
            this.DropShadow = true;
        }

        #endregion
    }
}
