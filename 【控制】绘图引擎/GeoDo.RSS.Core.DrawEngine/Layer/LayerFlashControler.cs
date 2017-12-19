using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class LayerFlashControler : ILayerFlashControler
    {
        protected ICanvas _canvas;
        protected IRenderLayer _layer;
        protected System.Windows.Forms.Timer _timer;

        public LayerFlashControler()
        {
        }

        public LayerFlashControler(ICanvas canvas, ILayer layer)
        {
            _canvas = canvas;
            _layer = layer as IRenderLayer;
        }

        private void CreateTimer()
        {

        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (_layer == null)
            {
                Stop();
                return;
            }
            _layer.Visible = !_layer.Visible;
            _canvas.Refresh(enumRefreshType.All);
        }

        public bool IsFlashing
        {
            get { throw new NotImplementedException(); }
        }

        public void AutoFlash(int interleave)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = interleave;
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        public void Flash()
        {
            _layer.Visible = !_layer.Visible;
            _canvas.Refresh(enumRefreshType.All);
        }

        public void Stop()
        {
            if (_timer != null)
                _timer.Stop();
            if (!_layer.Visible)
            {
                (_layer as IRenderLayer).Visible = true;
                _canvas.Refresh(enumRefreshType.All);
            }
        }

        public void Reset(ICanvas canvas, ILayer layer)
        {
            Stop();
            _canvas = canvas;
            _layer = layer as IRenderLayer;
        }

        public void Dispose()
        {
            _canvas = null;
            _layer = null;
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }
    }
}
