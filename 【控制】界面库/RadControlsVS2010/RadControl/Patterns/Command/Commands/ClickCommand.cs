using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Commands;

namespace Telerik.WinControls.Elements
{
	/// <summary>
	/// Represents a click command.
	/// </summary>
	public class ClickCommand : CommandBase
	{
        public override object Execute(params object[] settings)
		{
			if (settings.Length > 0 &&
				this.CanExecute(settings[0]))
			{
				object parameter = settings[0];
				if (typeof(RadItem).IsAssignableFrom(parameter.GetType()))
				{
					(parameter as RadItem).CallDoClick(EventArgs.Empty);
				}
                if (typeof(RadControl).IsAssignableFrom(parameter.GetType()))
                {
                    if (parameter is RadControl)
                    {
                        (parameter as RadControl).Behavior.OnClick(EventArgs.Empty);
                    }
                }
				return base.Execute(settings);
			}
            return false;
		}
	}
}
