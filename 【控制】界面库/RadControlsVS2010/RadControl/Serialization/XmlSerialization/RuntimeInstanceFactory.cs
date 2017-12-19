using System;

namespace Telerik.WinControls.XmlSerialization
{
    public class RuntimeInstanceFactory : InstanceFactory
    {
        #region Overrides of InstanceFactory

        public override object CreateInstance(Type instanceType)
        {
            return Activator.CreateInstance(instanceType);
        }

        #endregion
    }
}