using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;

namespace GeoDo.RSS.Core.DF
{
    public abstract class GeoDataProvider:IGeoDataProvider
    {
        protected string _fileName = null;
        protected IGeoDataDriver _driver = null;
        protected enumCoordType _coordType = enumCoordType.Raster;
        protected ISpatialReference _spatialRef = null;

        public GeoDataProvider(string fileName,IGeoDataDriver driver)
        {
            _fileName = fileName;
            _driver = driver;
        }

        public string fileName
        {
            get { return _fileName; }
        }

        public IGeoDataDriver Driver
        {
            get { return _driver; }
            internal set { _driver = value; }
        }

        public enumCoordType CoordType
        {
            get { return _coordType; }
        }

        public ISpatialReference SpatialRef
        {
            get { return _spatialRef; }
        }

        public virtual void Dispose()
        {
        }
    }
}
