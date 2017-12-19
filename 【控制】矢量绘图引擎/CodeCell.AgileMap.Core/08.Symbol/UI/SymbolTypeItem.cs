using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal class SymbolTypeItem
    {
        public Type SymbolType = null;
        public string Name = null;

        public SymbolTypeItem(string name, Type symbolType)
        {
            Name = name;
            SymbolType = symbolType;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
