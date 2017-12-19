using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.MEF;

namespace GeoDo.RSS.Core.DF
{
    internal class DriverFinder
    {
        private static IGeoDataDriver[] _registeredDrivers = null;

        public static IGeoDataDriver[] RegisteredDrivers
        {
            get 
            {
                if (_registeredDrivers == null)
                    _registeredDrivers = GetRegisteredDrivers();
                return _registeredDrivers; 
            }
        }

        internal static void PreLoading()
        {
            _registeredDrivers = GetRegisteredDrivers();
        }

        private static IGeoDataDriver[] GetRegisteredDrivers()
        {
            string[] files = MefConfigParser.GetAssemblysByCatalog("数据驱动");
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            using (IComponentLoader<IGeoDataDriver> loader = new ComponentLoader<IGeoDataDriver>())
            {
                return loader.LoadComponents(files);
            }
        }
    }
}
