using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeCell.AgileMap.Core
{
    public interface IPersistToBinary
    {
        Stream ToStream();
    }
}
