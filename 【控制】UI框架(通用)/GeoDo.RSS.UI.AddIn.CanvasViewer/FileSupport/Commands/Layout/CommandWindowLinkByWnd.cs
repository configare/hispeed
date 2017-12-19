using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public abstract class CommandWindowLinkByWnd:CommandWindowLink
    {
        public CommandWindowLinkByWnd()
        { 
        }

        protected abstract override enumViewerLayoutStyle GetLayoutStyle();

        protected override ILinkableViewer[] GetLinkableViewers()
        {
            ISmartWindow[] viewers = _smartSession.SmartWindowManager.GetSmartWindows((wnd) => { return true; });
            if (viewers == null || viewers.Length == 0)
                return null;
            List<ILinkableViewer> retViewers = new List<ILinkableViewer>();
            foreach (ISmartWindow v in viewers)
            {
                if (v != null && v is ILinkableViewer)
                    retViewers.Add(v as ILinkableViewer);
            }
            return retViewers.Count > 0 ? retViewers.ToArray() : null;
        }
    }

    [Export(typeof(ICommand))]
    public class CommandWindowLinkByWndH : CommandWindowLinkByWnd
    {
        public CommandWindowLinkByWndH()
        {
            _id = 9101;
            _name = "WindowLinkByWndH";
            _text = _toolTip = "左右窗口联动";
        }

        protected override enumViewerLayoutStyle GetLayoutStyle()
        {
            return enumViewerLayoutStyle.HorizontalLayout;
        }
    }

    [Export(typeof(ICommand))]
    public class CommandWindowLinkByWndV : CommandWindowLinkByWnd
    {
        public CommandWindowLinkByWndV()
        {
            _id = 9102;
            _name = "WindowLinkByWndV";
            _text = _toolTip = "上下窗口联动";
        }

        protected override enumViewerLayoutStyle GetLayoutStyle()
        {
            return enumViewerLayoutStyle.VerticalLayout;
        }
    }
}
