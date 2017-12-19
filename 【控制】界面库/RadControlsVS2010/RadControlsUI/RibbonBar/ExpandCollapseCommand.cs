using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Commands;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
	public class ExpandCollapseCommand : CommandBase
	{
		private bool expanded = false;

		public override object Execute(params object[] settings)
		{
			if (settings.Length > 0 &&
				this.CanExecute(settings[0]))
			{
				object parameter = settings[0];
				if (typeof(RadRibbonBar).IsAssignableFrom(parameter.GetType()))
				{
					(parameter as RadRibbonBar).Expanded = this.expanded;
					this.expanded = !this.expanded;
				}
				return base.Execute(settings);
			}
            return null;
		}

		public override bool CanExecute(object parameter)
		{
			if (typeof(Control).IsAssignableFrom(parameter.GetType()))
			{
				return true;
			}
			return base.CanExecute(parameter);
		}
	}
}