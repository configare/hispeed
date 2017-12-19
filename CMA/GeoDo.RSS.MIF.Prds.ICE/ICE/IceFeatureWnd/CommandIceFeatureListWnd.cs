using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.UI.AddIn.Windows;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    [Export(typeof(ICommand))]
    public class CommandIceFeatureListWnd : CommandToolWindow
    {
        public CommandIceFeatureListWnd()
        {
            _id = 78008;
            _name = "CommandIceFeatureListWnd";
            _text = _toolTip = "海冰冰缘线列表";
        }
    }
}
