using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public interface IHObject
    {
        
    }

    public abstract class HObject : IHObject
    {
        /**
         * The full path of the file that contains the object.
         */
        private String             filename;

        /**
         * The file which contains the object
         */
        protected HFile fileFormat;
        /**
         * The name of the data object. The root group has its default name, a
         * slash. The name can be changed except the root group.
         */
        private String name;

        /**
         * The full path of the data object. The full path always starts with the
         * root, a slash. The path cannot be changed. Also, a path must ended with a
         * slash. For example, /arrays/ints/
         */
        private String path;

        /** The full name of the data object, i.e. "path + name" */
        private String fullName;
        /// <summary>
        /// Array of long integer storing unique identifier for the object.
        /// <c>
        /// HDF4 objects are uniquely identified by a (ref_id, tag_id) pair. i.e.oid[0]=tag, oid[1]=ref.
        /// </c>
        /// HDF5 objects are uniquely identified by an object reference.
        /// </summary>
        protected long[] oid = null;

        public HObject(HFile theFile, String theName, String thePath, long[] oid)
        {
            this.fileFormat = theFile;
            this.oid = oid;
            //...
            this.name = theName;
            this.path = thePath;
        }

    }
}
