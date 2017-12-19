using System.Collections.Generic;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DI
{
    public abstract class DataImport : IDataImport
    {
        public IDataImportDriver GetDriver(string productIdentify, string subProductIdentify, string filename)
        {
            return GetDriver(productIdentify, subProductIdentify, filename);
        }

        public static IDataImportDriver GetDriver(string productIdentify, string subProductIdentify, string filename, params string[] args)
        {
            string error = string.Empty;
            IDataImportDriver[] drivers = DriverFinder.RegisteredDrivers;
            if (drivers == null || drivers.Length == 0)
                return null;
            foreach (IDataImportDriver item in DriverFinder.RegisteredDrivers)
            {
                if (item.CanDo(productIdentify, subProductIdentify, filename, out error))
                    return item;
            }
            return null;
        }


        public ImportFilesObj[] AutoFindFiles(string productIdentify, string subProIdentify, IRasterDataProvider dataProvider, string dir)
        {
            return AutoFindFiles(productIdentify,subProIdentify,dataProvider, dir);
        }

        public static ImportFilesObj[] AutoFindFiles(string productIdentify, string subProIdentify, IRasterDataProvider dataProvider, string dir, params string[] args)
        {
            string error = string.Empty;
            IDataImportDriver[] drivers = DriverFinder.RegisteredDrivers;
            if (drivers == null || drivers.Length == 0)
                return null;
            List<ImportFilesObj> importFiles = new List<ImportFilesObj>();
            ImportFilesObj[] files = null;
            foreach (IDataImportDriver item in DriverFinder.RegisteredDrivers)
            {
                files = item.AutoFindFilesByDirver(productIdentify,subProIdentify,dataProvider, dir);
                if (files != null)
                    importFiles.AddRange(files);
            }
            return importFiles.Count == 0 ? null : importFiles.ToArray();
        }
    }
}
