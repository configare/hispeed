using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    internal enum enumEventType
    { 
        MouseDown,
        MouseUp,
        MouseMove,
        MouseDoubleClick,
        MouseWheel,
        Other
    }

    internal class EventHandleStatus
    {
        public bool Handled = false;
    }

    internal interface IEventHandler
    {
        void Handle(object sender, enumEventType eventType, object eventArg,EventHandleStatus status);
    }
}
