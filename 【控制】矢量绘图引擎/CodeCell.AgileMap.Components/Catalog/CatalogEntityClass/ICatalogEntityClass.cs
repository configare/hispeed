using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Components
{
    public interface ICatalogEntityClass:IDisposable
    {
        void Insert(ICatalogEntity entity);
        void Update(ICatalogEntity entity);
        bool IsExist(ICatalogEntity entity);
        ICatalogEntity[] Query(ICatalogEntity templateEntity);
        ICatalogEntity[] Query(string where);
        void Delete(ICatalogEntity entity);
    }
}
