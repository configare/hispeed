using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public abstract class HFile : HObject
    {
        protected int _file_id = -1;

        public HFile(HFile theFile, String theName, String thePath, long[] oid)
            : base(theFile, theName, thePath, oid)
        {
        }

        public int File_Id
        {
            get { return _file_id; }
        }
    }
}
