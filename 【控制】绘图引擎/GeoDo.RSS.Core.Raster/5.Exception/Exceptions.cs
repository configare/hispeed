using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class CreateDataProviderFailedByFileName : Exception
    {
        protected string _fname = null;

        public CreateDataProviderFailedByFileName(string fname)
        {
            _fname = fname;
        }

        public CreateDataProviderFailedByFileName(string fname,Exception ex)
            :base(fname,ex)
        {
            _fname = fname;
        }

        public override string Message
        {
            get
            {
                return  _fname ?? string.Empty;
            }
        }
    }
}
