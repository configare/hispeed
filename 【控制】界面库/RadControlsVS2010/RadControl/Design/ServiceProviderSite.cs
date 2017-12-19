using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.Design
{
    /// <summary>
    /// A dummy ISite implementation, which provides support for custom services.
    /// </summary>
    public class ServiceProviderSite : ISite, IContainer
    {
        #region Fields

        private IServiceProvider serviceProvider;
        private string name;

        #endregion

        #region Constructor

        public ServiceProviderSite(IServiceProvider provider)
        {
            this.serviceProvider = provider;
        }

        #endregion

        #region IContainer Members

        public void Add(IComponent component, string name)
        {
            throw new NotImplementedException();
        }

        public void Add(IComponent component)
        {
            throw new NotImplementedException();
        }

        public ComponentCollection Components
        {
            get
            {
                return new ComponentCollection(new IComponent[] { });
            }
        }

        public void Remove(IComponent component)
        {
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISite Members

        public IComponent Component
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IContainer Container
        {
            get
            {
                return this;
            }
        }

        public bool DesignMode
        {
            get
            {
                return false;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            return this.serviceProvider.GetService(serviceType);
        }

        #endregion
    }
}
