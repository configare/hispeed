using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandGeoVisActive : Command
    {
        public CommandGeoVisActive()
        {
            _id = 8302;
            _name = "GeoVisActive";
            _text = "激活数字地球窗口";
            _toolTip = "激活数字地球窗口";
        }

        public override void Execute()
        {
            try
            {
                ISmartWindow[] wnds = _smartSession.SmartWindowManager.GetSmartWindows((wnd) => { return wnd.GetType().Equals(typeof(PluginGeoVisWnd)); });
                if (wnds == null || wnds.Length == 0)
                {
                    PluginGeoVisWnd wnd = new PluginGeoVisWnd();
                    wnd.Session = _smartSession;
                    _smartSession.SmartWindowManager.DisplayWindow(wnd);
                }
                else
                {
                    _smartSession.SmartWindowManager.DisplayWindow(wnds[0]);
                }
            }
            catch (Exception)
            {
                
            }
        }
    }

}
