using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSGeo.GDAL;

namespace GeoDo.RSS.DF.GDAL
{
    public class GDALDataset:IDisposable
    {
        protected Dataset _dataset = null;

        public GDALDataset(Dataset dataset)
        {
            _dataset = dataset;
        }

        public void Dispose()
        {
            if (_dataset != null)
            {
                _dataset.Dispose();
                _dataset = null;
            }
        }
    }
}
