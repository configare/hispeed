using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.Core.UI
{
    public class LinkableViewerManager : ILinkableViewerManager
    {
        protected enumViewerLayoutStyle _layout = enumViewerLayoutStyle.HorizontalLayout;
        protected List<ILinkableViewer> _viewers = new List<ILinkableViewer>();
        private ILinkableViewer _primaryViewer = null;

        public LinkableViewerManager()
        {

        }

        public IEnumerable<ILinkableViewer> LinkViewers
        {
            get { return _viewers; }
        }

        public enumViewerLayoutStyle Layout
        {
            get { return _layout; }
            set { _layout = value; }
        }

        public bool IsLinking(ILinkableViewer viewer)
        {
            return _viewers.Contains(viewer);
        }

        public bool Link(ILinkableViewer viewer)
        {
            if (viewer != null && !_viewers.Contains(viewer))
            {
                if (viewer.IsPrimaryLinkWnd)
                {
                    viewer.OnCoordEnvelopeChanged += new EventHandler((sender, arg) =>
                    {
                        To(viewer, ((viewer as ICanvasHost).Canvas).CurrentEnvelope.Clone());
                    });
                    _primaryViewer = viewer;
                }
                if (viewer is ICanvasViewer)
                    (viewer as ICanvasViewer).Canvas.IsLinking = true;
                _viewers.Add(viewer);
                return true;
            }
            return false;
        }

        public void Unlink(ILinkableViewer viewer)
        {
            if (viewer != null && _viewers.Contains(viewer))
            {
                if (viewer is ICanvasViewer && (viewer as ICanvasViewer).Canvas != null)
                    (viewer as ICanvasViewer).Canvas.IsLinking = false;
                if (viewer.IsPrimaryLinkWnd)
                {
                    viewer.OnCoordEnvelopeChanged = null;
                    viewer.IsPrimaryLinkWnd = false;
                }
                _viewers.Remove(viewer);
                if (_viewers.Count == 1)
                {
                    _viewers[0].OnCoordEnvelopeChanged = null;
                    _viewers.Clear();
                }
            }
        }

        public void ChangePrimaryLinkViewer(ILinkableViewer viewer)
        {
            if (viewer == null)
                return;
            if (_primaryViewer != null)
            {
                _primaryViewer.IsPrimaryLinkWnd = false;
                _primaryViewer.OnCoordEnvelopeChanged = null;
            }
            viewer.IsPrimaryLinkWnd = true;
            _primaryViewer = viewer;
            viewer.OnCoordEnvelopeChanged += new EventHandler((sender, arg) =>
            {
                To(viewer, ((viewer as ICanvasHost).Canvas).CurrentEnvelope);
            });
        }

        public void To(ILinkableViewer fromViewer, GeoDo.RSS.Core.DrawEngine.CoordEnvelope viewport)
        {
            foreach (ILinkableViewer v in _viewers)
                if (!v.Equals(fromViewer))
                {
                    v.To(viewport.Clone());
                }
        }

        public void Reset()
        {
            foreach (ILinkableViewer v in _viewers)
            {
                ICanvasViewer view = v as ICanvasViewer;
                if (view != null)
                    view.Canvas.IsLinking = false;
                v.IsPrimaryLinkWnd = false;
                v.OnCoordEnvelopeChanged = null;
            }
            _viewers.Clear();
        }
    }
}
