using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Commands;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class ActivateMenuItemCommand: CommandBase
    {
        public override object Execute(params object[] settings)
        {
			if (settings.Length > 0 && this.CanExecute(settings[0]))
			{
                RadMenuItem parameter = settings[0] as RadMenuItem;
                if (parameter != null)
                {
                    if (!parameter.Selected)
                    {
                            parameter.Select();
                    }
                    parameter.ShowChildItems();
                    return base.Execute(settings);
                }
			}
            return null;
        }

        public override bool CanExecute(object parameter)
        {
            if (typeof(RadMenuItem).IsAssignableFrom(parameter.GetType()))
            {
                return true;
            }
            return false;
        }
    }
}
