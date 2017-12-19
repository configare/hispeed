using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IRuntimeExchanger
    {
        bool Enabled { get; set; }
        bool AutoRefreshWhileFinishOneGrid { get; set; }
        int MaxTaskCount { get; set; }
        void RemoveLayer(IFeatureLayer lyr);
        void RaiseCheckingFromOutside();
    }
}
