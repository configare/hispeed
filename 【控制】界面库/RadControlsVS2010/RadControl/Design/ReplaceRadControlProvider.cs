using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls
{
   /// <summary>
    /// This is a helper class which avoids design time error when control design time is opened directly.
    /// </summary>
    internal class ReplaceRadControlProvider : TypeDescriptionProvider
    {
        public ReplaceRadControlProvider()
            : base(TypeDescriptor.GetProvider(typeof(RadControl)))
        {
        }

        public override Type GetReflectionType(Type objectType, object instance)
        {
            if (objectType == typeof(RadControl))
                return typeof(Control);

            return base.GetReflectionType(objectType, instance);
        }

        public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
        {
            if (objectType == typeof(RadControl))
                objectType = typeof(Control);

            return base.CreateInstance(provider, objectType, argTypes, args);
        }
    }
}
