using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class CursorInfoWindow : ToolWindowBase, 
        ISmartToolWindow,ICursorInfoDisplayer
    {
        public CursorInfoWindow()
            :base()
        {
            _id = 9000;
            Text = "像元信息";
        }

        protected override IToolWindowContent GetToolWindowContent()
        {
            return new CursorInfoWndContent();
        }

        void ICursorInfoDisplayer.RegisterProvider(ICursorInfoProvider infoProvider)
        {
            (_content as ICursorInfoDisplayer).RegisterProvider(infoProvider);
        }

        void ICursorInfoDisplayer.UnregisterProvider(ICursorInfoProvider infoProvider)
        {
            (_content as ICursorInfoDisplayer).UnregisterProvider(infoProvider);
        }
    }
}
