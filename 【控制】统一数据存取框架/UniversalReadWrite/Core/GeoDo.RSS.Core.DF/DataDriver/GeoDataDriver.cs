using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.Core.DF
{
    public abstract class GeoDataDriver:IGeoDataDriver
    {
        protected string _name = null;
        protected string _fullName = null;

        public GeoDataDriver()
        { 
        }

        public GeoDataDriver(string name, string fullName)
        {
            _name = name;
            _fullName = fullName;
        }

        public string Name
        {
            get { return _name ?? string.Empty; }
        }

        public string FullName
        {
            get { return _fullName ?? string.Empty; }
        }

        public IGeoDataProvider Open(string fileName, byte[] header1024,enumDataProviderAccess access, params object[] args)
        {
            if (!IsCompatible(fileName, header1024, args))
                return null;
            return BuildDataProvider(fileName, header1024,access, args);
        }

        public IGeoDataProvider Open(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return BuildDataProvider(fileName, access, args);
        }

        protected abstract IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024,enumDataProviderAccess access, params object[] args);
        protected abstract IGeoDataProvider BuildDataProvider(string fileName,  enumDataProviderAccess access, params object[] args);

        protected abstract bool IsCompatible(string fileName, byte[] header1024, params object[] args);

        public abstract void Delete(string fileName);

        public virtual void Dispose()
        {
        }

        public static IGeoDataDriver GetDriverByName(string name,params string[] args)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            IGeoDataDriver[] drivers = DriverFinder.RegisteredDrivers;
            if (drivers == null || drivers.Length == 0)
                throw new DriverListIsEmptyException();
            foreach (IGeoDataDriver drv in drivers)
                if (drv.Name == name)
                    return drv;
            throw new NoMatchedDirverException();
        }

        public static IGeoDataProvider Open(string fileName, params string[] args)
        {
            return Open(fileName, enumDataProviderAccess.ReadOnly, args);
        }

        public static void PreLoading()
        {
            DriverFinder.PreLoading();
        }

        public static IGeoDataProvider Open(string fileName, enumDataProviderAccess access,params string[] args)
        {
            IGeoDataDriver[] drivers = DriverFinder.RegisteredDrivers;
            if (drivers == null || drivers.Length == 0)
                throw new DriverListIsEmptyException();
            byte[] head1024 = GetHeader1024Bytes(fileName);
            foreach (IGeoDataDriver driver in drivers)
            {
                IGeoDataProvider prd = driver.Open(fileName, head1024, access, args);
                if (prd != null)
                    return prd;
            }
            throw new NoMatchedDirverException();
        }

        private static byte[] GetHeader1024Bytes(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open,FileAccess.Read))
            {
                byte[] buffer = new byte[1024];
                fs.Read(buffer, 0, 1024);
                fs.Close();
                return buffer;
            }
        }
    }
}
