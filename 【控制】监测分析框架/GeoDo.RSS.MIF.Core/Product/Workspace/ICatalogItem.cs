using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface ICatalogItem
    {
        string FileName { get; }
        CatalogItemInfo Info {get;set; }
    }
}
