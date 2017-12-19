using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.XmlSerialization
{
    public class IServiceProviderHelper<ServiceType> where ServiceType : class
    {
        public static ServiceType GetService(IServiceProvider serviceProvider, string caller)
        {
            ServiceType service = serviceProvider.GetService(typeof(ServiceType)) as ServiceType;
            if (service == null)
            {
                throw new NotSupportedException(string.Format("{0} requires {1} service.", typeof(ColorBlendExtension), typeof(IProvideTargetValue)));
            }

            return service;
        }
    }
}
