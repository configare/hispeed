using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Core
{
    public interface IQueryResultContainer
    {
        void Init(ILocationService locsrv);
        void AddFeatures(Feature[] features);
        void Clear();
        bool ResultContainerVisible { get; set; }
        Control BeginInvokeContiner { get; }
    }
}
