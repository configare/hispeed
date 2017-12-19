using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;

namespace GeoDo.ProjectDefine
{
    public class SpatialReferenceFile
    {
        private ISpatialReference _spatialReference = null;
        private bool _isPrjFile = false;

        public SpatialReferenceFile()
        {
        }

        public ISpatialReference SpatialReference
        {
            get { return _spatialReference; }
            set { _spatialReference = value; }
        }

        public bool IsPrjFile
        {
            get { return _isPrjFile; }
            set { _isPrjFile = value; }
        }
    }
}
