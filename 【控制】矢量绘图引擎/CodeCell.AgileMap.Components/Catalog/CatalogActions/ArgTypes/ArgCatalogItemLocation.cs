using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Components
{
    public class ArgCatalogItemLocation:ArgCatalogItem
    {
        protected override enumCatalogItemType GetCatalogItemType()
        {
            return enumCatalogItemType.Location;
        }
    }
}
