using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface ICatalog
    {
        object UI { get; }
        CatalogDef Definition { get; }
        ICatalogItem[] GetSelectedItems();
        string[] GetSelectedFiles();
        string[] GetSelectedFiles(string identify);
        void AddItem(ICatalogItem item);
        bool IsExist(ICatalogItem item);
        void Update(ICatalogItem item);
        bool RemoveItem(ICatalogItem item,bool removeOther);
        void Clear();
    }
}
