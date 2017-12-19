using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class AVIHelper : IAVIHelper
    {
        private ILayoutHost _host = null;
        private Timer _timer = null;
        private List<Bitmap> _bmps = null;
        private int _interval = 0;
        private int _count = 0;
        private bool _enabled = false;
        private int _index = 0;
        private EventHandler _onTimerStopped = null;

        public AVIHelper(ILayoutHost host)
        {
            _host = host;
            _bmps = new List<Bitmap>();
            InitAviLayerInfos();
            InitTimer();
        }

        public EventHandler OnTimerStopped
        {
            get { return _onTimerStopped; }
            set { _onTimerStopped = value; }
        }

        public Bitmap[] Bitmaps
        {
            get { return _bmps.Count != 0 ? _bmps.ToArray() : null; }
        }

        private void InitTimer()
        {
            _timer = new Timer();
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Interval = _interval;
            _timer.Enabled = _enabled;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (_index < _count)
            {
                GetBitmap();
                return;
            }
            else
            {
                _timer.Stop();
                if (_onTimerStopped != null)
                    _onTimerStopped(sender, e);
            }
        }

        private void GetBitmap()
        {
            Bitmap bm = _host.ExportToBitmap(PixelFormat.Format24bppRgb);
            _bmps.Add(bm);
            _index++;
        }

        private void InitAviLayerInfos()
        {
            ILayout layout = _host.LayoutRuntime.Layout;
            if (layout == null)
                return;
            if (layout.Elements == null || layout.Elements.Count == 0)
                return;
            IDataFrame df = null;
            foreach (IElement ele in layout.Elements)
                if (ele is IDataFrame)
                {
                    df = ele as IDataFrame;
                    break;
                }
            if (df == null)
                return;
            if (df.Provider == null)
                return;
            ICanvas canvas = (df.Provider as IDataFrameDataProvider).Canvas;
            if (canvas == null)
                return;
            List<GeoDo.RSS.Core.DrawEngine.ILayer> layers = canvas.LayerContainer.Layers;
            if (layers == null || layers.Count == 0)
                return;
            IAVILayer aviLry = null;
            foreach (GeoDo.RSS.Core.DrawEngine.ILayer lry in layers)
                if (lry is IAVILayer)
                    aviLry = lry as IAVILayer;
            if (aviLry == null)
                return;
            if (aviLry.BitmapCount == 0 )
                return;
            _count = aviLry.BitmapCount;
            _interval = aviLry.Interval;
            _enabled = aviLry.IsRunning;
            return;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (_bmps != null)
            {
                foreach (Bitmap bmp in _bmps)
                {
                    if (bmp != null)
                    {
                        bmp.Dispose();
                    }
                }
                _bmps = null;
            }
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
        #endregion
    }
}
