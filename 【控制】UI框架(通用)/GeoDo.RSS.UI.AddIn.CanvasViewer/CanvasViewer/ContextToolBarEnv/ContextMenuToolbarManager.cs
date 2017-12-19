using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    internal class ContextMenuToolbarManager : IContextMenuToolbarManager
    {
        private ISmartSession _session = null;
        private Form _hostForm = null;
        public delegate void UCDisposeDeleg();
        public UCDisposeDeleg ucDispose = null;

        public ContextMenuToolbarManager(ISmartSession session)
        {
            _session = session;
        }

        public IContextMenuArgProvider Show(string argProviderUI)
        {
            if (argProviderUI == null)
                return null;
            string[] parts = argProviderUI.Split(':');
            Assembly assemlby = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + parts[0]);
            IContextMenuArgProvider argProvider = assemlby.CreateInstance(parts[1]) as IContextMenuArgProvider;
            _hostForm = new Form();
            _hostForm.StartPosition = FormStartPosition.Manual;
            (argProvider as UserControl).BorderStyle = BorderStyle.None;
            _hostForm.BackColor = Color.FromArgb(218, 231, 247);
            (argProvider as Control).BackColor = Color.FromArgb(218, 231, 247);
            _hostForm.Controls.Add(argProvider as Control);
            _hostForm.Text = "参数设置...";
            _hostForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            _hostForm.Width = (argProvider as Control).Width;
            _hostForm.Height = (argProvider as Control).Height + 32;
            Point position = new Point(_session.SmartWindowManager.ViewLeftUpperCorner.X + 62,
                                       _session.SmartWindowManager.ViewLeftUpperCorner.Y + 29);
            _hostForm.Location = position;
            _hostForm.Show(_session.SmartWindowManager.MainForm as Form);
            _hostForm.FormClosed += new FormClosedEventHandler(_hostForm_FormClosed);
            return argProvider;
        }

        public void _hostForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ucDispose != null)
                ucDispose();
        }

        public void Close()
        {
            if (_hostForm != null)
            {
                _hostForm.Close();
                _hostForm = null;
            }
        }
    }
}
