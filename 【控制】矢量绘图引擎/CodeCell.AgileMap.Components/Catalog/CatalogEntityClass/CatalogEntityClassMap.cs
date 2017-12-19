using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    internal class CatalogEntityClassMap:CatalogEntityClassBase
    {
        public CatalogEntityClassMap(string connString)
            : base(connString)
        { 
        }

        protected override string GetTableName()
        {
            return BudGISMetadata.cstMapTableName;
        }

        protected override ICatalogEntity NewCatalogEntity()
        {
            return new SpatialMap();
        }
    }
}
