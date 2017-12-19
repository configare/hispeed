using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public interface IContextMessage
    {
        void Reset();
        string GetErrorInfoString();
        void AddError(string errorInfo);
        object Tag { get; set; }
    }
}
