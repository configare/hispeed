using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.XmlSerialization
{
    public abstract class InstanceFactory
    {
        public abstract object CreateInstance(Type instanceType);
    }
}
