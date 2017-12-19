using System;
using GeoDo.MEF;

namespace GeoDo.RSS.DI
{
    public class DriverFinder
    {
        private static IDataImportDriver[] _registeredDrivers = null;

        public static IDataImportDriver[] RegisteredDrivers
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

        private static IDataImportDriver[] GetRegisteredDrivers()
        {
            string[] files = MefConfigParser.GetAssemblysByCatalog("数据导入");
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            using (IComponentLoader<IDataImportDriver> loader = new ComponentLoader<IDataImportDriver>())
            {
                return loader.LoadComponents(files);
            }
        }
    }
}
