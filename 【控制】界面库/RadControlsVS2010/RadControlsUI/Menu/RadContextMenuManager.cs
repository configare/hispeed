using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Adds the RadContextMenu dynamic property and enables using RadContextMenu in all controls.
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.MenuAndToolbarsGroup)]
    [ToolboxItem(true)]
	[ProvideProperty("RadContextMenu", typeof(Control))]
    [Designer(DesignerConsts.RadContextMenuManagerDesignerString)]
    [Description("Adds the RadContextMenu dynamic property and enables using RadContextMenu in all controls.")]
    [ToolboxBitmap(typeof(Telerik.WinControls.UI.RadContextMenu), "RadContextMenuManager.bmp")]
    public class RadContextMenuManager : Component, IExtenderProvider
	{
        private Hashtable contextMenus = new Hashtable();
        
		#region IExtenderProvider Members

		public bool CanExtend(object extendee)
		{
			return extendee is Control && !(extendee is RadTreeView);
		}

		#endregion

		[DisplayName("RadContextMenu")]
		[DefaultValue(null)]
		[Category("Behavior")]
        public RadContextMenu GetRadContextMenu(Control control)
		{
			return (RadContextMenu)contextMenus[control];
		}

        public void SetRadContextMenu(Control control, RadContextMenu value)
		{
			contextMenus[control] = value;
			if (value != null)
			{
				control.MouseDown += new MouseEventHandler(control_MouseDown);
			}
			else
			{
				control.MouseDown -= new MouseEventHandler(control_MouseDown);
			}
		}
		
		private void control_MouseDown(object sender, MouseEventArgs e)
		{
            RadContextMenu menu = (RadContextMenu)contextMenus[sender];
			if (menu != null && e.Button == MouseButtons.Right)
			{
				menu.Show((Control)sender, e.X, e.Y);
			}
		}        
    }
}
