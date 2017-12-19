using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.UI.AddIn.Office
{
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    public class TextFileProcessor : OpenFileProcessor
    {
        public TextFileProcessor()
            : base()
        {
            _extNames.Add(".TXT");
            _extNames.Add(".RTF");
        }

        protected override bool FileHeaderIsOK(string fname, string extName)
        {
            return true;
        }

        public override bool Open(string fname, out bool memoryIsNotEnough)
        {
            memoryIsNotEnough = false;
            ISmartToolWindow toolWindow = _session.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(9005);
            if (toolWindow != null)
            {
                (toolWindow as IStatResultDisplayWindow).Open(fname);
                _session.SmartWindowManager.DisplayWindow(toolWindow);
                return true;
            }
            return false;
        }
    }
}
