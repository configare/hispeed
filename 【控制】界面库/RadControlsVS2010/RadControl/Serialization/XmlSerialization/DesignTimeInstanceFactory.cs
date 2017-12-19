using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Telerik.WinControls.XmlSerialization
{
    public class DesignTimeInstanceFactory: RuntimeInstanceFactory
    {
        private IDesignerHost designerHost;

        public DesignTimeInstanceFactory(IDesignerHost designerHost)
        {
            this.designerHost = designerHost;
            if (this.designerHost == null)
            {
                throw new ArgumentNullException("designerHost");
            }
        }

        #region Overrides of RuntimeInstanceFactory

        public override object CreateInstance(Type instanceType)
        {
            if (typeof(IComponent).IsAssignableFrom(instanceType))
            {
                return designerHost.CreateComponent(instanceType);
            }

            return base.CreateInstance(instanceType);
        }

        #endregion
    }
}