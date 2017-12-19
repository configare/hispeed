using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    public interface ITraverser<T> : IEnumerator<T>, IEnumerable
    {
        object Position { get; set; }
        bool MovePrevious();
        bool MoveToEnd();
    }
}
