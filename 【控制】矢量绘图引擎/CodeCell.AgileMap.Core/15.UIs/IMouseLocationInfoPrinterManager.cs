using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IMouseLocationInfoPrinterManager
    {
        void Register(IMouseLocationInfoPrinter printer);
        void Remove(IMouseLocationInfoPrinter printer);
    }
}
