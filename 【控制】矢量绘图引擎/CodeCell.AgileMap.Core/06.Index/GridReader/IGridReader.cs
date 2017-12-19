using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IGridReader:IDisposable
    {
        bool IsReady { get; }
        void BeginRead();
        IGrid ReadGrid(int gridNo);
        void EndRead();
    }
}
