using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.MIF.Core;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public partial class WorkspaceWindowContent : UserControl
    {
        protected ISmartSession _session;
        internal UCWorkspace _wksUI;

        public WorkspaceWindowContent()
        {
            InitializeComponent();
            _wksUI = new UCWorkspace();
            _wksUI.Dock = DockStyle.Fill;
            this.Controls.Add(_wksUI);
            _wksUI.SetDoubleClickHandler((obj) =>
            {
                if (obj != null)
                {
                    ICommand cmd = _session.CommandEnvironment.Get(2000);
                    if (cmd != null)
                        cmd.Execute(obj.ToString());
                }
            });
        }


        public void Apply(Core.UI.ISmartSession session)
        {
            _session = session;
            IMonitoringSession msession = _session.MonitoringSession as IMonitoringSession;
            if (msession.ActiveMonitoringProduct != null)
                _wksUI.Apply(msession.ActiveMonitoringProduct.Identify);
        }

        public void Free()
        {
            _session = null;
            if (_wksUI != null)
            {
                _wksUI.Dispose();
                _wksUI = null;
            }
        }
    }
}
