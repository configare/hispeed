using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class SelfReferenceRelation: ObjectRelation
    {
        private string parentName;
        private string childName;

        internal SelfReferenceRelation(object list, string parentName, string childName)
            :base(list)
        {
            this.parentName = parentName;
            this.childName = childName;
        }

        public override string[] ParentRelationNames
        {
            get
            {
                return new string[] { parentName };
            }
        }

        public override string[] ChildRelationNames
        {
            get
            {
                return new string[] { childName };
            }
        }

        protected override void Initialize()
        {
        }
    }
}
