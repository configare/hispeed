using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IMapServer
    {
        string Url { get; }
        bool TestConnect();
        IMapService Services { get; }
    }
}
