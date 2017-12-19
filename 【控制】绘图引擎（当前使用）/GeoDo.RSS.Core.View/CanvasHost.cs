using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;

namespace GeoDo.RSS.Core.View
{
    public partial class CanvasHost : UserControl, ICanvasHost,
        IDisposable
    {
        private ICanvas _canvas = null;

        public CanvasHost()
        {
            InitializeComponent();
            Load += new EventHandler(CanvasHost_Load);
            DoubleBuffered = true;
        }

        void CanvasHost_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;
            CreateCanvas();
            this.Select();
        }

        public void CreateCanvas()
        {
            if (_canvas == null)
            {
                _canvas = new Canvas(this);
                _canvas.CurrentViewControl = new DefaultControlLayer();
            }
        }

        public ICanvas Canvas
        {
            get { return _canvas; }
        }

        public new void DrawToBitmap(Bitmap buffer, Rectangle rect)
        {
            (this as UserControl).DrawToBitmap(buffer, rect);
        }

        public void DisposeView()
        {
            (this as IDisposable).Dispose();
        }

        void IDisposable.Dispose()
        {
            Load -= new EventHandler(CanvasHost_Load);
            if (_canvas != null)
            {
                _canvas.Dispose();
                _canvas = null;
            }
            BufferedGraphicsManager.Current.Invalidate();
            BufferedGraphicsManager.Current.Dispose();
            (this as UserControl).Dispose();
        }

    }
}
