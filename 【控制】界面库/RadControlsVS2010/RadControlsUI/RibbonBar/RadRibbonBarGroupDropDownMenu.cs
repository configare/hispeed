using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the popup of a <see cref="RadRibbonBarGroup"/>.
    /// The contents of the group are placed in this popup when the group is collapsed.
    /// </summary>
    public class RadRibbonBarGroupDropDownMenu : RadDropDownButtonPopup
    {
        #region Constructor

        public RadRibbonBarGroupDropDownMenu(RadRibbonBarGroupDropDownButtonElement element)
            : base(element)
        {
        }

        protected override void InitializeChildren()
        {
            base.InitializeChildren();

            (this.PopupElement as RadDropDownMenuElement).ScrollPanel.HorizontalScrollState = ScrollState.AlwaysHide;
            (this.PopupElement as RadDropDownMenuElement).ScrollPanel.VerticalScrollState = ScrollState.AlwaysHide;
        }

        #endregion

        #region Properties

        public override string ThemeClassName
        {
            get
            {
                return typeof(RadRibbonBarGroupDropDownMenu).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        #endregion
    }
}
