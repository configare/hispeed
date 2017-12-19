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

namespace GeoDo.RSS.UI.AddIn.Windows
{
    public partial class ToolWindowBase : ToolWindow, ISmartToolWindow
    {
        protected int _id = 0;
        protected ISmartSession _session = null;
        protected IToolWindowContent _content = null;
        protected OnActiveWindowChangedHandler _viewerChangedHandler = null;
        protected EventHandler _onWindowClosed;

        public ToolWindowBase()
        {
            InitializeComponent();
            _content = GetToolWindowContent();
            if (_content != null)
            {
                (_content as Control).Dock = DockStyle.Fill;
                this.Controls.Add(_content as Control);
                //_content.Apply(_session);                
                _viewerChangedHandler = new OnActiveWindowChangedHandler(
                    (sender, oldv, newv) =>
                    {
                        _content.Apply(_session);
                    }
                    );
            }
            this.Disposed += new EventHandler(Window_Disposed);
        }

        protected virtual IToolWindowContent GetToolWindowContent()
        {
            return null;
        }

        void Window_Disposed(object sender, EventArgs e)
        {
            if (_session != null)
            {
                _session.SmartWindowManager.OnActiveWindowChanged -= _viewerChangedHandler;
            }
            _viewerChangedHandler = null;
            _onWindowClosed = null;
            if (_content != null)
            {
                _content.Free();
                _content = null;
            }
        }

        public virtual void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            if (_session.SmartWindowManager.OnActiveWindowChanged != null)
                _session.SmartWindowManager.OnActiveWindowChanged -= _viewerChangedHandler;
            _session.SmartWindowManager.OnActiveWindowChanged += _viewerChangedHandler;
            if (_content != null)
                _content.Apply(_session);
        }

        public EventHandler OnWindowClosed
        {
            get { return _onWindowClosed; }
            set { _onWindowClosed = value; }
        }

        public int Id
        {
            get { return _id; }
        }
    }
}
