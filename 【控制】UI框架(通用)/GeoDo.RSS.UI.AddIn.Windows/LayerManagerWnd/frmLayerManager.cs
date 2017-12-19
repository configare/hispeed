using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI.Docking;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class frmLayerManager : ToolWindowBase, ISmartToolWindow,ILayerManager
    {
        private ILayersProvider _provider = null;
        private OnActiveWindowChangedHandler _viewIsChangedHandler;

        public frmLayerManager()
        {
            InitializeComponent();
            _id = 9002;
            Text = "层管理器";
            _viewIsChangedHandler = new OnActiveWindowChangedHandler(ViewIsChanged);
            Disposed += new EventHandler(frmLayerManager_Disposed);
           
        }

        public frmLayerManager(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }


        public ILayersProvider Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        void frmLayerManager_Disposed(object sender, EventArgs e)
        {
            _session.SmartWindowManager.OnActiveWindowChanged -= _viewIsChangedHandler;
        }

        public override void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            _session.SmartWindowManager.OnActiveWindowChanged += _viewIsChangedHandler;
        }

        private void ViewIsChanged(object sender, ISmartWindow oldV, ISmartWindow newV)
        {
            if (newV == null)
            {
                (this.Controls[0] as UCLayerManagerPanel).Apply(null);
                return;
            }
            if (newV is ICanvasViewer)
            {
                ICanvasViewer v = newV as ICanvasViewer;
                if (v == null)
                    return;
                IVectorHostLayer host = v.Canvas.LayerContainer.VectorHost;
                if (host == null)
                    return;
                (this.Controls[0] as UCLayerManagerPanel).Apply(v.LayerProvider as ILayersProvider);
            }
            else if (newV is ILayoutViewer)
            {
                ILayoutViewer v = newV as ILayoutViewer;
                if (v == null)
                    return;
                (this.Controls[0] as UCLayerManagerPanel).Apply(v.LayerProvider as ILayersProvider);
            }
        }

        public void CloseWnd()
        {
        }

        void ILayerManager.Apply(ILayersProvider provider)
        {
            (this.Controls[0] as UCLayerManagerPanel).Apply(provider);
        }

        void ILayerManager.Update()
        {
            (this.Controls[0] as UCLayerManagerPanel).UpdateLayers();
        }
    }
}
