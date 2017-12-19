using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Interface provides methods for registering and accessing <see cref="RadService"/>.
    /// </summary>
    public interface IRadServiceProvider
    {
        /// <summary>
        /// Retrieves currently registered <see cref="RadService">Service</see> by the specified type.
        /// </summary>
        /// <typeparam name="T">A type derived from <see cref="RadService"/></typeparam>
        /// <returns></returns>
        T GetService<T>() where T : RadService;
        /// <summary>
        /// Registers the specified service with ourselves.
        /// </summary>
        /// <param name="service">An instance of type derived from <see cref="RadService"/>.</param>
        void RegisterService(RadService service);
    }
}
