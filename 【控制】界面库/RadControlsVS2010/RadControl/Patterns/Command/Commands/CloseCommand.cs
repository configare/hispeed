using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Commands;
using System.Windows.Forms;

namespace Telerik.WinControls.Elements
{
    public class CloseCommand : CommandBase
    {
        public override object Execute(params object[] settings)
        {
            if (settings.Length > 0 &&
                this.CanExecute(settings[0]))
            {
                object parameter = settings[0];
                if (typeof(Form).IsAssignableFrom(parameter.GetType()))
                {
                    (parameter as Form).Close();
                }
                else if (typeof(Control).IsAssignableFrom(parameter.GetType()))
                {
                    (parameter as Control).Hide();
                }
                else if (typeof(RadItem).IsAssignableFrom(parameter.GetType()))
                {
                    (parameter as RadItem).Visibility = ElementVisibility.Hidden;
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
