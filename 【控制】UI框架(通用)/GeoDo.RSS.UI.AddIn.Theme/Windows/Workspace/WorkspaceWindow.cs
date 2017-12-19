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
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class WorkspaceWindow : ToolWindow, ISmartToolWindow
    {
        private int _id;
        private ISmartSession _session;
        private IWorkspace _workspace;
        private WorkspaceWindowContent _workspaceWindowContent;
        private EventHandler _onWindowClosed;
        
        public WorkspaceWindow()
            :base()
        {
            _id = 9020;
            Text = "工作空间";
            _workspaceWindowContent = new WorkspaceWindowContent();
            _workspaceWindowContent.Dock = DockStyle.Fill;
            this.Controls.Add(_workspaceWindowContent);
            _workspace = _workspaceWindowContent._wksUI as IWorkspace;
            _workspaceWindowContent._wksUI.SetDoubleClickHandler((item) => 
            {
                if (item != null)
                {
                    //打开文件
                    ICommand cmd = _session.CommandEnvironment.Get(2000);
                    if (cmd != null)
                        cmd.Execute(item.ToString());
                }
            });
        }

        public int Id
        {
            get { return _id; }
        }

        public EventHandler OnWindowClosed
        {
            get 
            { 
                return _onWindowClosed;
            }
            set 
            { 
                _onWindowClosed = value; 
            }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            _workspaceWindowContent.Apply(session);
        }

        public IWorkspace Workspace
        {
            get { return _workspace; }
        }
    }
}
