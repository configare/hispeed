using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCell.Bricks.AppFramework
{
    public interface IShortcutFilterCollection
    {
        int Count { get;}
        IEnumerable<IShortcutFilter> ShortcutFilters { get;}
    }
}
