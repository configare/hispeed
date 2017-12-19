using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Components
{
    public interface ICatalogEntity
    {
        string Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        void Store();
        void Refresh();
        bool IsEmpty();
    }
}
