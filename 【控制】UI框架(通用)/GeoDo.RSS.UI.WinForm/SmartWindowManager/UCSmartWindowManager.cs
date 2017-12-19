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
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.WinForm
{
    public partial class UCSmartWindowManager : UserControl, ISmartWindowManager
    {
        private RadDock _radDock;
        private OnActiveWindowChangedHandler _activeWindowChangedHandler;
        private Dictionary<DockPosition, DockWindow> _edgeDockwindows = new Dictionary<DockPosition, DockWindow>();
        private ILinkableViewerManager _linkableViewerManager;
        private ISmartViewer _activeViewer;
        private ICanvasViewer _activeCanvasViewer;
        private ICanvasViewer _newestCreatedCanvasViewer = null;
        private ISmartToolWindowFactory _smartToolWindowFactory;
        private ISmartSession _session;

        public UCSmartWindowManager()
        {
            InitializeComponent();
            CreateRadDockControl();
            AttachEvents();
            CreateLinkableViewerManager();
        }

        internal void SetSession(ISmartSession session)
        {
            _session = session;
            CreateToolWindowFactory();
        }

        private void CreateToolWindowFactory()
        {
            _smartToolWindowFactory = new SmartToolWindowFactory(_session);
        }

        private void CreateLinkableViewerManager()
        {
            _linkableViewerManager = new LinkableViewerManager();
        }

        private void AttachEvents()
        {
            _radDock.DockStateChanged += new DockWindowEventHandler(_radDock_DockStateChanged);
            _radDock.DockWindowAdded += new DockWindowEventHandler(_radDock_DockWindowAdded);
            _radDock.ActiveWindowChanged += new DockWindowEventHandler(_radDock_ActiveWindowChanged);
            _radDock.SelectedTabChanged += new SelectedTabChangedEventHandler(_radDock_SelectedTabChanged);
            _radDock.DockWindowClosed += new DockWindowEventHandler(_radDock_DockWindowClosed);
            _radDock.DockWindowClosing += new DockWindowCancelEventHandler(_radDock_DockWindowClosing);
            _radDock.FloatingWindowCreated += new FloatingWindowEventHandler(_radDock_FloatingWindowCreated);
            _radDock.ShowItemToolTips = false;
            SizeChanged += new EventHandler(UCSmartWindowManager_SizeChanged);
            Disposed += new EventHandler(UCSmartWindowManager_Disposed);
        }

        void _radDock_FloatingWindowCreated(object sender, FloatingWindowEventArgs e)
        {
            RadDock radDock = (sender as RadDock);
            if (radDock != null && radDock.ActiveWindow is ISmartViewer)
            {
                e.Window.MinimizeBox = true;
                e.Window.MaximizeBox = true;
                e.Window.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        }

        void _radDock_DockWindowClosing(object sender, DockWindowCancelEventArgs e)
        {
            _linkableViewerManager.Unlink(e.NewWindow as ILinkableViewer);
            if (e.NewWindow is IStatResultDisplayWindow)
                (e.NewWindow as Telerik.WinControls.IGeoDoFree).Free();
            else if (e.NewWindow is ICanvasViewer)
            {
                ICanvasViewer canViewer = e.NewWindow as ICanvasViewer;
                IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
                IGeoPanAdjust adjust = rd as IGeoPanAdjust;
                if (adjust != null && adjust.IsHasUnsavedGeoAdjusted)
                {
                    DialogResult ret = MsgBox.ShowQuestionYesNoCancel("当前影像的平移校正结果未保存,请确认是否保存？\n按【是】保存。\n按【否】不保存。\n按【取消】返回。");
                    if (ret == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else if (ret == DialogResult.Yes)
                        adjust.Save();
                    else
                        adjust.Cancel();
                }
                //退出平移校正状态
                ICommand cmd = _session.CommandEnvironment.Get(30006);
                if (cmd != null)
                    cmd.Execute();
                //退出平移校正2状态
                ICommand cmd2 = _session.CommandEnvironment.Get(30106);
                if (cmd2 != null)
                    cmd2.Execute();
            }
        }

        void _radDock_DockWindowClosed(object sender, DockWindowEventArgs e)
        {
            //触发注册在视窗上的关闭事件
            ISmartWindow smartWindow = e.DockWindow as ISmartWindow;
            if (smartWindow != null)
                if (smartWindow.OnWindowClosed != null)
                    smartWindow.OnWindowClosed(smartWindow, e);
            //
            if (e.DockWindow.Equals(_activeViewer))
                _activeViewer = null;
            //
            if (e.DockWindow.Equals(_newestCreatedCanvasViewer))
                _newestCreatedCanvasViewer = null;
            //
            _linkableViewerManager.Unlink(e.DockWindow as ILinkableViewer);
            e.DockWindow.Dispose();
            /*
             * 以下1行释放Telerik不能释放的资源
             */
            if (e.DockWindow is Telerik.WinControls.IGeoDoFree)
                (e.DockWindow as Telerik.WinControls.IGeoDoFree).Free();
            if (e.DockWindow is ISmartWindow)
                (e.DockWindow as ISmartWindow).Free();
            /*
             * 以下三行释放视图资源(CanvasViewer,LayoutViewer)
             */
            ISmartViewer smartViewer = e.DockWindow as ISmartViewer;
            if (smartViewer != null)
            {
                smartViewer.DisposeViewer();
                //
                SetActiveViewer(null, smartViewer is ICanvasViewer);
            }
            /*
             * 以下一句解决LayoutEventArgs引用DocumentTabStrip对象导致对象不释放的问题
             */
            _radDock.MainDocumentContainer.PerformLayout();
            //
            GC.Collect();
        }

        void UCSmartWindowManager_Disposed(object sender, EventArgs e)
        {
            ReleaseEvents();
            if (_linkableViewerManager != null)
            {
                _linkableViewerManager.Reset();
                _linkableViewerManager = null;
            }
            _activeWindowChangedHandler = null;
            _smartToolWindowFactory = null;
        }

        private void ReleaseEvents()
        {
            if (_radDock != null)
            {
                _radDock.DockStateChanged -= new DockWindowEventHandler(_radDock_DockStateChanged);
                _radDock.DockWindowAdded -= new DockWindowEventHandler(_radDock_DockWindowAdded);
                _radDock.ActiveWindowChanged -= new DockWindowEventHandler(_radDock_ActiveWindowChanged);
                _radDock.SelectedTabChanged -= new SelectedTabChangedEventHandler(_radDock_SelectedTabChanged);
                _radDock.DockWindowClosed -= new DockWindowEventHandler(_radDock_DockWindowClosed);
                _radDock.DockWindowClosing -= new DockWindowCancelEventHandler(_radDock_DockWindowClosing);
                _radDock.FloatingWindowCreated -= new FloatingWindowEventHandler(_radDock_FloatingWindowCreated);
                foreach (DockWindow wnd in _radDock.DockWindows)
                {
                    if (wnd is ICanvasViewer)
                        (wnd as IDisposable).Dispose();
                }
                _radDock.Dispose();
            }
            SizeChanged -= new EventHandler(UCSmartWindowManager_SizeChanged);
            Disposed -= new EventHandler(UCSmartWindowManager_Disposed);
            _edgeDockwindows.Clear();
        }

        void UCSmartWindowManager_SizeChanged(object sender, EventArgs e)
        {
            _radDock.Width = Width + 14;
            _radDock.Height = Height + 16;
        }

        void _radDock_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            ISmartViewer viewer = e.NewWindow as ISmartViewer;
            if (viewer != null)
                SetActiveViewer(viewer, viewer is ICanvasViewer);
        }

        void _radDock_ActiveWindowChanged(object sender, DockWindowEventArgs e)
        {
            ISmartViewer viewer = e.DockWindow as ISmartViewer;
            if (viewer != null)
                SetActiveViewer(viewer, viewer is ICanvasViewer);
        }

        void _radDock_DockWindowAdded(object sender, DockWindowEventArgs e)
        {
            ISmartViewer viewer = e.DockWindow as ISmartViewer;
            if (viewer != null)
                SetActiveViewer(viewer, viewer is ICanvasViewer);       //radDock的DockWindowAdded事件执行完毕，会调用ActiveWindowChanged
            if (viewer is ICanvasViewer)
                _newestCreatedCanvasViewer = viewer as ICanvasViewer;
        }

        private void SetActiveViewer(ISmartViewer viewer, bool isCanvasViewer)
        {
            if (viewer == null && isCanvasViewer)
                _activeCanvasViewer = null;
            if (viewer != null && viewer.Equals(_activeViewer) && !viewer.Equals(_newestCreatedCanvasViewer))
                return;
            ISmartViewer oldViewer = _activeViewer;
            _activeViewer = viewer;
            if (viewer is ICanvasViewer)
            {
                _activeCanvasViewer = viewer as ICanvasViewer;
            }
            if (_activeWindowChangedHandler != null)
            {
                _activeWindowChangedHandler(this, oldViewer, viewer);
            }
            UpdatePrimaryLinkWindow();
        }

        public void UpdatePrimaryLinkWindow()
        {
            if (_activeViewer is ILinkableViewer)
            {
                ILinkableViewer linkViewer = _activeViewer as ILinkableViewer;
                if (!linkViewer.IsPrimaryLinkWnd && _linkableViewerManager.IsLinking(linkViewer))
                {
                    _linkableViewerManager.ChangePrimaryLinkViewer(linkViewer);
                }
            }
        }

        public void NewHorizontalGroup(ISmartViewer viewer)
        {
            _radDock.AddDocument(viewer as DockWindow, (viewer as DockWindow).TabStrip as DocumentTabStrip, DockPosition.Right);
        }

        public void NewVerticalGroup(ISmartViewer viewer)
        {
            _radDock.AddDocument(viewer as DockWindow, (viewer as DockWindow).TabStrip as DocumentTabStrip, DockPosition.Bottom);
        }

        void _radDock_DockStateChanged(object sender, DockWindowEventArgs e)
        {
            foreach (DockPosition pos in _edgeDockwindows.Keys)
            {
                if (_edgeDockwindows[pos].Equals(e.DockWindow) && e.DockWindow.DockState != DockState.Docked)
                {
                    _edgeDockwindows.Remove(pos);
                    return;
                }
            }
        }

        private void CreateRadDockControl()
        {
            _radDock = new RadDock();
            _radDock.Left = -6;
            _radDock.Top = -8;
            _radDock.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            _radDock.ShowItemToolTips = false;
            Controls.Add(_radDock);
            //DocumentContainer dc = new DocumentContainer();
            //_radDock.MainDocumentContainer = dc;
            //_radDock.Controls.Add(dc);
            //dc.MouseDoubleClick += new MouseEventHandler(dc_MouseDoubleClick);
        }

        void dc_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        public object MainForm
        {
            get { return this.FindForm(); }
        }

        public Point ViewLeftUpperCorner
        {
            get
            {
                if (this.IsDisposed)
                    return Point.Empty;
                return this.PointToScreen(new Point(0, 0));
            }
        }

        public OnActiveWindowChangedHandler OnActiveWindowChanged
        {
            get { return _activeWindowChangedHandler; }
            set { _activeWindowChangedHandler = value; }
        }

        public ISmartWindow ActiveWindow
        {
            get { return _radDock.ActiveWindow as ISmartWindow; }
        }

        public ISmartViewer ActiveViewer
        {
            get { return _activeViewer; }
        }

        public ICanvasViewer ActiveCanvasViewer
        {
            get { return _activeCanvasViewer; }
        }

        public void AddDocument(ISmartWindow window)
        {
            HandleRepeatedName(window);
            ISmartWindow wnd = GetSmartWindow((w) => { return w.Equals(window); });
            if (wnd == null)
            {
                _radDock.AddDocument(window as DockWindow, DockPosition.Fill);
            }
        }

        public void DisplayWindow(ISmartWindow window)
        {
            HandleRepeatedName(window);
            ISmartWindow wnd = GetSmartWindow((w) => { return w.Equals(window); });
            if (wnd == null)
            {
                _radDock.AddDocument(window as DockWindow, DockPosition.Fill);
                //_radDock.AddDocument(window as DockWindow, _radDock.GetDefaultDocumentTabStrip(false), DockPosition.Fill);
                _radDock.ActivateWindow(window as DockWindow);
            }
            else
            {
                if (wnd != null)
                    _radDock.ActivateWindow(wnd as DockWindow);
            }
        }

        public void ActivateWindow(ISmartWindow wnd)
        {
            if (wnd != null)
                _radDock.ActivateWindow(wnd as DockWindow);
        }

        private void HandleRepeatedName(ISmartWindow window)
        {
            var wnds = _radDock.GetWindows<DockWindow>();
            if (wnds == null || wnds.Count() == 0)
                return;
            DockWindow dockWnd = window as DockWindow;
            if (dockWnd == null)
                return;
            foreach (DockWindow wnd in wnds)
            {
                if (wnd.Name == dockWnd.Name)
                {
                    dockWnd.Name = Guid.NewGuid().ToString();
                    break;
                }
            }
        }

        public void DisplayWindow(ISmartWindow window, WindowPosition position, params object[] options)
        {
            if (position == null || position.DockStyle == DockStyle.Fill)
            {
                DisplayWindow(window);
                return;
            }
            if ((window as DockWindow).DockState == DockState.Hidden)
            {
                (window as DockWindow).DockState = DockState.Docked;
                return;
            }
            if (position.DockStyle == DockStyle.None)
            {
                if (options != null && options.Length > 0)
                {
                    Rectangle rect = (Rectangle)options[0];
                    _radDock.FloatWindow(window as DockWindow, rect);
                }
                else
                {
                    _radDock.FloatWindow(window as DockWindow);
                }
                return;
            }
            DockPosition dockPosition = GetDockPosition(position);
            if (position.IsSharePanel)
            {
                DockWindow edgeDockwindow = GetEdgeDockwindow(dockPosition);
                if (edgeDockwindow == null)
                {
                    _radDock.DockWindow(window as DockWindow, dockPosition);
                    _edgeDockwindows.Add((DockPosition)dockPosition, window as DockWindow);
                }
                else
                {
                    _edgeDockwindows.Remove(dockPosition);
                    _edgeDockwindows.Add(dockPosition, window as DockWindow);
                    _radDock.DockWindow(window as DockWindow, edgeDockwindow.DockTabStrip, DockPosition.Top);
                }
            }
            else
            {
                _radDock.DockWindow(window as DockWindow, dockPosition);
            }
        }

        private DockPosition GetDockPosition(WindowPosition position)
        {
            switch (position.DockStyle)
            {
                case DockStyle.Fill:
                    return DockPosition.Fill;
                case DockStyle.Left:
                    return DockPosition.Left;
                case DockStyle.Right:
                    return DockPosition.Right;
                case DockStyle.Top:
                    return DockPosition.Top;
                case DockStyle.Bottom:
                    return DockPosition.Bottom;
            }
            return DockPosition.Fill;
        }

        private DockWindow GetEdgeDockwindow(DockPosition dockPosition)
        {
            if (_edgeDockwindows.ContainsKey(dockPosition))
                return _edgeDockwindows[dockPosition];
            return null;
        }

        public bool IsExist(ISmartWindow window)
        {
            var result = _radDock.GetWindows<DockWindow>();
            if (result == null || result.Count() == 0)
                return false;
            foreach (DockWindow wnd in result)
                if (wnd.Equals(window))
                    return true;
            return false;
        }

        public ISmartWindow GetSmartWindow(Func<ISmartWindow, bool> where)
        {
            var result = _radDock.GetWindows<DockWindow>();
            if (result == null || result.Count() == 0)
                return null;
            foreach (ISmartWindow wnd in result)
                if (wnd != null && where(wnd))
                    return wnd;
            return null;
        }

        public ISmartWindow[] GetSmartWindows(Func<ISmartWindow, bool> where)
        {
            var result = _radDock.GetWindows<DockWindow>();
            if (result == null || result.Count() == 0)
                return null;
            List<ISmartWindow> wnds = new List<ISmartWindow>();
            foreach (DockWindow wnd in result)
            {
                ISmartWindow swnd = wnd as ISmartWindow;
                if (swnd == null)
                    continue;
                if (swnd != null && where(swnd))
                    wnds.Add(swnd);
            }
            return wnds.Count > 0 ? wnds.ToArray() : null;
        }

        public void ToHorizontalLayout(ISmartWindow[] windows)
        {
            if (windows == null || windows.Length == 0)
                return;
            foreach (DockWindow wnd in windows)
                _radDock.AddDocument(wnd, wnd.TabStrip as DocumentTabStrip, DockPosition.Right);
        }

        public void ToVerticalLayout(ISmartWindow[] windows)
        {
            if (windows == null || windows.Length == 0)
                return;
            foreach (DockWindow wnd in windows)
                _radDock.AddDocument(wnd, wnd.TabStrip as DocumentTabStrip, DockPosition.Bottom);
        }

        public void Hide(ISmartWindow window)
        {
            (window as DockWindow).DockState = DockState.Hidden;
        }

        public void Show(ISmartWindow window)
        {
            (window as DockWindow).DockState = DockState.Docked;
        }

        public ILinkableViewerManager LinkableViewerManager
        {
            get { return _linkableViewerManager; }
        }

        public ISmartToolWindowFactory SmartToolWindowFactory
        {
            get { return _smartToolWindowFactory; }
        }

        public ICursorInfoDisplayer CursorInfoDisplayer
        {
            get
            {
                if (_smartToolWindowFactory == null)
                    return null;
                ISmartToolWindow wnd = _smartToolWindowFactory.GetSmartToolWindow(9000);
                if (wnd == null)
                    return null;
                return wnd as ICursorInfoDisplayer;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //GC.Collect();
            //DockWindow wnd = new ToolWindow();
            //wnd.CloseAction = DockWindowCloseAction.CloseAndDispose;
            //wnd.Text = Environment.TickCount.ToString();
            //_radDock.AddDocument(wnd);
            GC.Collect();
        }
    }
}
