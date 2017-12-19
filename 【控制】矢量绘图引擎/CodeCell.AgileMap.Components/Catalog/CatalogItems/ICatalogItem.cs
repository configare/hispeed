using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Components
{
    public interface ICatalogItem
    {
        string Name { get; }
        string Description { get; }
        Image Image { get; }
        int ChildCount { get; }
        ICatalogItem[] Children { get; }
        ICatalogItem Parent { get; }
        object Tag { get; }
        void AddChild(ICatalogItem catalogEntity);
        void Remove(ICatalogItem catalogEntity);
        ContextOprItem[] ContextOprItems { get; }
        enumContextKeys DefaultKey { get; }
        void Click(enumContextKeys key);
        void Refresh();
    }
}
