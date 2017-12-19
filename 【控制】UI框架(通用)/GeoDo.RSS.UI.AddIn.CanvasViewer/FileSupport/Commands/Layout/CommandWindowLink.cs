using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public abstract class CommandWindowLink : Command
    {
        public CommandWindowLink()
        {
        }

        public override void Execute()
        {
            ILinkableViewer[] viewers = GetLinkableViewers();
            if (viewers == null || viewers.Length <= 1)
                return;
            ILinkableViewer pv = viewers[0];
            if (!(pv is ICanvasViewer))
                pv = viewers[1];
            pv.IsPrimaryLinkWnd = true;
            _smartSession.SmartWindowManager.LinkableViewerManager.Layout = GetLayoutStyle();
            foreach (ILinkableViewer v in viewers)
                _smartSession.SmartWindowManager.LinkableViewerManager.Link(v);
            switch (_smartSession.SmartWindowManager.LinkableViewerManager.Layout)
            {
                case enumViewerLayoutStyle.HorizontalLayout:
                    for (int i = 1; i < viewers.Length; i++)
                            _smartSession.SmartWindowManager.NewHorizontalGroup(viewers[i] as ISmartViewer);
                    break;
                case enumViewerLayoutStyle.VerticalLayout:
                    for (int i = 1; i < viewers.Length; i++)
                            _smartSession.SmartWindowManager.NewVerticalGroup(viewers[i] as ISmartViewer);
                    break;
            }
            _smartSession.SmartWindowManager.UpdatePrimaryLinkWindow();
            ICanvasViewer primaryViewer = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (primaryViewer != null)
            {
                primaryViewer.Canvas.CurrentEnvelope = (primaryViewer as ICanvasViewer).Canvas.CurrentEnvelope;
                primaryViewer.Canvas.Refresh(enumRefreshType.All);
            }
        }

        protected abstract enumViewerLayoutStyle GetLayoutStyle();
        protected abstract ILinkableViewer[] GetLinkableViewers();
    }

    [Export(typeof(ICommand))]
    public class CommandResetWindowLink : CommandWindowLink
    {
        public CommandResetWindowLink()
        {
            _id = 9100;
            _name = "ResetWindowLink";
            _text = _toolTip = "恢复窗口联动";
        }

        public override void Execute()
        {
            foreach (ILinkableViewer v in _smartSession.SmartWindowManager.LinkableViewerManager.LinkViewers)
                _smartSession.SmartWindowManager.DisplayWindow(v as ISmartViewer);
            _smartSession.SmartWindowManager.LinkableViewerManager.Reset();
        }

        protected override enumViewerLayoutStyle GetLayoutStyle()
        {
            throw new NotImplementedException();
        }

        protected override ILinkableViewer[] GetLinkableViewers()
        {
            throw new NotImplementedException();
        }
    }
}
