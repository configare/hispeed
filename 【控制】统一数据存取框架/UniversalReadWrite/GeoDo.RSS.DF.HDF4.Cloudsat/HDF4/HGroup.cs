using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public abstract class HGroup : HObject
    {
        /// <summary>
        /// The list of members (Groups and Datasets) of this group in memory.
        /// </summary>
        private List<HObject> memberList = null;

        /**
         * The parent group where this group is located. The parent of the root
         * group is null.
         */
        protected HGroup parent;

        /**
         * Total number of (Groups and Datasets) of this group in file.
         */
        protected int nMembersInFile;

        public HGroup(HFile theFile, String name, String path, HGroup parent)
            : this(theFile, name, path, parent, null)
        {
        }

        public HGroup(HFile theFile, String name, String path, HGroup parent, long[] oid)
            : base(theFile, name, path, oid)
        {
            this.parent = parent;
        }

        public HGroup Parent
        {
            get { return parent; }
        }

        public HObject[] MemberList
        {
            get
            {
                if (memberList == null)
                    return null;
                return memberList.ToArray();
            }
        }

        public void addToMemberList(HObject obj)
        {
            if (memberList == null)
            {
                memberList = new List<HObject>();
            }
            if (obj != null && !memberList.Contains(obj))
            {
                memberList.Add(obj);
            }
        }
    }
}
