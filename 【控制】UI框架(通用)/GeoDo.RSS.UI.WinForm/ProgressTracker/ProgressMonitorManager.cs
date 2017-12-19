using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.WinForm
{
    public class ProgressMonitorManager : IProgressMonitorManager
    {
        private frmProgressBar _ucProgressBar = null;

        public ProgressMonitorManager(Form owner)
        {
            _ucProgressBar = new frmProgressBar();
            _ucProgressBar.Owner = owner;
            _ucProgressBar.Show();
            _ucProgressBar.Hide();
        }

        public IProgressMonitor DefaultProgressMonitor
        {
            get
            {
                if (_ucProgressBar == null)
                    _ucProgressBar = new frmProgressBar();
                return _ucProgressBar;
            }
        }

        public IProgressMonitor NewProgressMonintor()
        {
            return new frmProgressBar();
        }
    }
}
