using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;

namespace Telerik.WinControls.Commands
{
    public class SetPropertyValueCommand : CommandBase
    {
        public override object Execute(params object[] settings)
		{
			if (settings.Length > 1 &&
				this.CanExecute(settings[0]))
			{
				object target = settings[0];
                object propertyValue = null;
                string propertyName = string.Empty;

                if (settings[1] is IList)
                {
                    if (((IList)settings[1]).Count < 2)
	                {
                        return null;
	                }
                    propertyValue = (((IList)settings[1])[0]);
                    propertyName = (((IList)settings[1])[1]) as string;
                }
                else
                {
                    propertyValue = settings[1];
                    propertyName = settings[2] as string;
                }
                if (target != null &&
                    !string.IsNullOrEmpty(propertyName))
                {
                    Type type = target.GetType();
                    PropertyInfo myPropertyInfo = type.GetProperty(propertyName);
                    myPropertyInfo.SetValue(target, propertyValue, null);
                }
				return base.Execute(settings);
			}
            return false;
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
