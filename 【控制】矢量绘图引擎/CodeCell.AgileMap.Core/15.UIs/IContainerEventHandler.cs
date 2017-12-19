using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IContainerEventHandler
    {
        void Handle(object sender, enumContainerEventType eventType,object eventArg, ref bool isHandled);
    }
}
