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
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    public partial class LayoutViewer : ToolWindow, ILayoutViewer, IDisposable, IIsLayerable
    {
        private EventHandler _onCoordEnvelopeChanged = null;
        private bool _isPrimaryLinkWnd = false;
        private ISmartSession _session = null;
        private LayoutViewerLayerProviderFacader _layerProvider = null;
        private List<EventHandler> _canvasRefreshSubscribers = new List<EventHandler>();
        private string _argument = null;
        private EventHandler _onWindowClosed;
        ILayoutHost _host;
        LayoutControl _layoutControl;

        public LayoutViewer()
        {
            InitializeComponent();
            Init();
        }

        public LayoutViewer(string argument)
            : base(argument)
        {
            _argument = argument;
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            _layoutControl = new LayoutControl();
            _host = new LayoutHost(_layoutControl);
            this.Controls.Add(_layoutControl);
            _layoutControl.Dock = DockStyle.Fill;
            //默认活动工具为选择元素工具
            _host.CurrentTool = new ArrowTool();
            //注册元素被拖入的事件(用于刷新图层管理器)
            _host.OnElementIsDragDroped += new EventHandler((eles, e) => 
            {
                TryInitFeatureLements(eles);
                ISmartWindow layerManager = _session.SmartWindowManager.GetSmartWindow((wnd) => { return wnd is ILayerManager; });
                if (layerManager != null)
                    (layerManager as ILayerManager).Update();
            });
        }

        private void TryInitFeatureLements(object eles)
        {
            if (eles == null)
                return;
            if (eles is IElement[])
            {
                foreach (IElement ele in (eles as IElement[]))
                    TryInitFeatureLement(ele);
            }
            else if (eles is IElement)
            {
                TryInitFeatureLement(eles as IElement);
            }
        }

        private void TryInitFeatureLement(IElement ele)
        {
            if (ele is IDrawingElement)
                (ele as IDrawingElement).SetDataFrame(_host.ActiveDataFrame);
        }

        public EventHandler OnWindowClosed
        {
            get { return _onWindowClosed; }
            set { _onWindowClosed = value; }
        }

        public void SetSession(ISmartSession session)
        {
            _session = session;
        }

        public ILayoutHost LayoutHost
        {
            get { return _host as LayoutHost; }
        }

        public new void Free()
        {
            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }
        }

        public void DisposeViewer()
        {
            this.Free();
        }

        public string Title
        {
            get { return Text; }
        }

        public object ActiveObject
        {
            get
            {
                return _host.LayoutRuntime.Layout.Elements != null ?
                       _host.LayoutRuntime.Layout.Elements : null;
            }
        }

        public bool IsPrimaryLinkWnd
        {
            get { return _isPrimaryLinkWnd; }
            set { _isPrimaryLinkWnd = value; }
        }

        public void To(Core.DrawEngine.CoordEnvelope viewport)
        {
        }

        public EventHandler OnCoordEnvelopeChanged
        {
            get { return _onCoordEnvelopeChanged; }
            set { _onCoordEnvelopeChanged = value; }
        }

        public object LayerProvider
        {
            get
            {
                if (_layerProvider != null)
                    return _layerProvider;
                else
                {
                    _layerProvider = new LayoutViewerLayerProviderFacader(this);
                    return _layerProvider;
                }
            }
        }

        #region ILayoutViewer 成员


        public object RgbProcessorArgEditorEnvironment
        {
            get { return this; }
        }

        #endregion
    }
}
