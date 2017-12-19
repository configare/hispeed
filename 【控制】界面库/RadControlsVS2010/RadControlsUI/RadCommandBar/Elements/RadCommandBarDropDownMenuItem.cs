using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a menu item on the context menu opened by right click on the RadCommandBar control.
    /// Has a coresponding <see cref="CommandBarStripElement"/> element and controls its VisibleInCommandBar property.
    /// </summary>
    [RadThemeDesignerData(typeof(RadMenuItem))]
    class CommandBarDropDownMenu : RadMenuItem
    {
        private CommandBarStripElement representedElement;

        public CommandBarDropDownMenu(CommandBarStripElement representedElement)
        {
            this.representedElement = representedElement;
            this.Class = "RadMenuItem";
        }

        public override bool IsChecked
        {
            get
            {
                return base.IsChecked;
            }
            set
            {
                base.IsChecked = value;
                
                this.representedElement.VisibleInCommandBar = value;
            }
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            this.IsChecked = !this.IsChecked;
            base.OnMouseDown(e);
        }

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadMenuItem);
            }
        }
    }
}
