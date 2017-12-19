using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.AppFramework
{
    public interface ITimerHandler
    {
        bool Enabled { get; set; }
        void Tick();
    }
}
