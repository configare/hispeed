using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    interface IListen
    {
        void StartListen();
        void StopListen();
    }
}
