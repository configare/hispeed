using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCell.Bricks.AppFramework
{
    public interface IUIBuilder
    {
        void Refresh();
        void DirectRefresh();
        void Building(ICommandProvider commandProvider);
        bool IntimeRefresh { get; set; }
    }
}
