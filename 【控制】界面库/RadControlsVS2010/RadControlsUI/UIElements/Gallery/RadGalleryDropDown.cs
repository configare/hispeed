using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the drop-down
    /// of the RadGalleryElement.
    /// </summary>
    [ToolboxItem(false)]
    public class RadGalleryDropDown : RadDropDownMenu
    {
        #region Fields

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the <see cref="Telerik.WinControls.UI.RadGalleryElement"/>
        /// class.
        /// </summary>
        /// <param name="ownerElement">An instance of the <see cref="Telerik.WinControls.UI.RadGalleryElement"/>
        /// class that represents the gallery that owns this drop-down.</param>
        public RadGalleryDropDown(RadGalleryElement ownerElement)
            : base(ownerElement)
        {
            this.AutoSize = false;
            this.FadeAnimationType = FadeAnimationType.FadeOut;
        }

        #endregion

        #region Properties

        #endregion

        #region Overrides

        public override bool CanClosePopup(RadPopupCloseReason reason)
        {
            if (this.PopupElement is RadGalleryPopupElement)
            {
                if (this.Bounds.Contains(MousePosition))
                {
                    return false;
                }
            }

            return base.CanClosePopup(reason);
        }

        public override string ThemeClassName
        {
            get
            {
                return typeof(RadGalleryDropDown).Namespace + ".RadGalleryMenu";
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        #endregion
    }
}
