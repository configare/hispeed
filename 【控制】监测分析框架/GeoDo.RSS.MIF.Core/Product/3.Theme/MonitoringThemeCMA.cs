using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Core
{
    public class MonitoringThemeCMA:MonitoringTheme
    {
        public MonitoringThemeCMA(ThemeDef themeDef)
            :base(themeDef)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
