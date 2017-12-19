using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.AppFramework
{
    public interface ITimerHandlerMgr
    {
        void Register(ITimerHandler timerHandler);
        void UnRegister(ITimerHandler timeHandler);
    }
}
