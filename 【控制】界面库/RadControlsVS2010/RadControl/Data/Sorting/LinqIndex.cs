using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Data
{
    internal class LinqIndex<T> : Index<T> where T : IDataItem
    {
        public LinqIndex(RadCollectionView<T> collectionView)
            : base(collectionView)
        {

        }

        protected internal override IList<T> Items
        {
            get { throw new NotImplementedException(); }
        }

        protected override void Perform()
        {
            throw new NotImplementedException();
        }
    }
}
