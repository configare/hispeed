using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Telerik.WinControls.Commands
{
    public class GetPropertyValueCommand : CommandBase
    {
        public override object Execute(params object[] settings)
        {
            if (settings.Length > 1 &&
                this.CanExecute(settings[0]))
            {
                object target = settings[0];
                string propertyName = settings[1] as string;
                
                if (target != null &&
                    !string.IsNullOrEmpty(propertyName))
                {
                    Type type = target.GetType();
                    PropertyInfo myPropertyInfo = type.GetProperty(propertyName);
                    base.Execute(settings);
                    return myPropertyInfo.GetValue(target, null);
                }
            }
            return null;
        }

        public override bool CanExecute(object parameter)
        {
            if (typeof(object).IsAssignableFrom(parameter.GetType()))
            {
                return true;
            }
            return base.CanExecute(parameter);
        }
    }
}
