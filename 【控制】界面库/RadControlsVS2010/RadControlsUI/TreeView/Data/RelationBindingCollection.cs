using Telerik.Collections.Generic;

namespace Telerik.WinControls.UI
{
    public class RelationBindingCollection : NotifyCollection<RelationBinding>
    {
        /// <summary>
        /// Adds the specified data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="displayMember">The display member.</param>
        /// <param name="parentChildMember">The parent child member.</param>
        public void Add(object dataSource, string displayMember, string parentChildMember)
        {
            this.Add(new RelationBinding(dataSource, displayMember, parentChildMember));
        }

        /// <summary>
        /// Adds the specified data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="displayMember">The display member.</param>
        /// <param name="parentMember">The parent member.</param>
        /// <param name="childMember">The child member.</param>
        public void Add(object dataSource, string displayMember, string parentMember, string childMember)
        {
            this.Add(new RelationBinding(dataSource, displayMember, parentMember, childMember));
        }

        /// <summary>
        /// Adds the specified data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="displayMember">The display member.</param>
        /// <param name="parentMember">The parent member.</param>
        /// <param name="childMember">The child member.</param>
        /// <param name="valueMember">The value member.</param>
        public void Add(object dataSource, string displayMember, string parentMember, string childMember, string valueMember)
        {
            this.Add(new RelationBinding(dataSource, displayMember, parentMember, childMember, valueMember));
        }

        /// <summary>
        /// Adds the specified data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="dataMember">The data member.</param>
        /// <param name="displayMember">The display member.</param>
        /// <param name="parentMember">The parent member.</param>
        /// <param name="childMember">The child member.</param>
        /// <param name="valueMember">The value member.</param>
        public void Add(object dataSource, string dataMember, string displayMember, string parentMember, string childMember, string valueMember)
        {
            this.Add(new RelationBinding(dataSource, dataMember, displayMember, parentMember, childMember, valueMember));
        }
    }
}
