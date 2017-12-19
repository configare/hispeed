using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Manages the standard MDI functionality available in .NET.
    /// </summary>
    internal class MdiController
    {
        #region Fields

        //reference to the owning RadDock instance
        private RadDock dockManager;
        private List<Form> mdiChildren;

        private MdiClient mdiClient;
        private bool loaded;
        private bool enabled;
        private byte suspendFormClear;

        #endregion

        #region Constructor

        public MdiController(RadDock dock)
        {
            this.dockManager = dock;
            this.enabled = false;
            this.mdiChildren = new List<Form>();
        }

        #endregion

        #region Implementation

        public void Dispose()
        {
            this.UnwireEvents();
        }

        public void Load()
        {
            if (this.loaded)
            {
                return;
            }

            this.loaded = true;
            if (this.enabled)
            {
                this.WireEvents();
            }
        }

        internal void OnMdiChildClosing(DockWindowCancelEventArgs e)
        {
            HostWindow host = e.NewWindow as HostWindow;
            if (host == null)
            {
                return;
            }

            Form form = host.MdiChild;
            if (form == null)
            {
                return;
            }

            this.suspendFormClear++;
            //explicitly call Close on the Form, many users have complained about their favourite Closing/Closed events missing
            form.Close();
            if (form.IsDisposed)
            {
                this.ClearForm(form, false);
            }
            else
            {
                e.Cancel = true;
            }
            this.suspendFormClear--;
        }

        internal void WireEvents()
        {
            if (this.dockManager.ParentForm == null || !this.enabled)
            {
                return;
            }

            this.FindMdiClient();
            if (this.mdiClient == null)
            {
                this.dockManager.ParentForm.ControlAdded += new ControlEventHandler(OnMdiParent_ControlAdded);
            }
            else
            {
                this.UpdateMdiClient();
            }
        }

        internal void UnwireEvents()
        {
            foreach (Form form in this.mdiChildren)
            {
                form.FormClosed -= OnChildForm_Closed;
                form.Disposed -= OnChildForm_Disposed;
            }
            this.mdiChildren.Clear();

            if (this.dockManager.ParentForm == null)
            {
                return;
            }

            this.dockManager.ParentForm.ControlAdded -= new ControlEventHandler(OnMdiParent_ControlAdded);
            if (this.mdiClient != null)
            {
                this.mdiClient.ControlAdded -= new ControlEventHandler(OnMdiClient_ControlAdded);
            }
        }

        internal void OnFormRemoved(HostWindow sender, Form form)
        {
            if (!this.mdiChildren.Contains(form))
            {
                return;
            }

            this.ClearForm(form, false);
            sender.Close();
        }

        private void FindMdiClient()
        {
            if (this.dockManager.ParentForm == null)
            {
                return;
            }

            foreach (Control child in this.dockManager.ParentForm.Controls)
            {
                this.mdiClient = child as MdiClient;
                if (this.mdiClient != null)
                {
                    this.mdiClient.ControlAdded += new ControlEventHandler(OnMdiClient_ControlAdded);
                    break;
                }
            }
        }

        private void UpdateMdiClient()
        {
            if (this.mdiClient == null)
            {
                return;
            }

            if (this.enabled)
            {
                this.mdiClient.Dock = DockStyle.None;
                this.mdiClient.SetBounds(0, 0, 0, 0);

                while (this.mdiClient.Controls.Count > 0)
                {
                    this.RegisterForm(this.mdiClient.Controls[0] as Form);
                }
            }
            else
            {
                this.mdiClient.Dock = DockStyle.Fill;
            }
        }

        private void OnEnabledChanged()
        {
            if(!loaded)
            {
                return;
            }

            this.UnwireEvents();
            if (this.enabled)
            {
                this.WireEvents();
            }
        }

        private void RegisterForm(Form form)
        {
            form.FormClosed += OnChildForm_Closed;
            form.Disposed += OnChildForm_Disposed;
            form.TextChanged += OnChildForm_TextChanged;
            form.Shown += new EventHandler(form_Shown);

            this.mdiChildren.Add(form);
            this.dockManager.DockControl(form, this.dockManager.GetDefaultDocumentTabStrip(true), DockPosition.Fill, this.dockManager.MdiChildrenDockType);
        }

        void form_Shown(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            form.Shown -= new EventHandler(form_Shown);
            DockWindow host = this.dockManager.GetHostWindow(form);
            if (host != null)
            {
                host.Text = form.Text;
            }
        }

        private void OnChildForm_TextChanged(object sender, EventArgs e)
        {
            Form form = sender as Form;
            DockWindow host = this.dockManager.GetHostWindow(form);
            if (host != null)
            {
                host.Text = form.Text;
            }
        }

        private void ClearForm(Form form, bool callClose)
        {
            if (form == null)
            {
                return;
            }

            form.FormClosed -= OnChildForm_Closed;
            form.Disposed -= OnChildForm_Disposed;
            form.TextChanged -= OnChildForm_TextChanged;
            this.mdiChildren.Remove(form);

            if (callClose)
            {
                DockWindow dockWindow = ControlHelper.FindAncestor<DockWindow>(form);
                if (dockWindow != null && dockWindow.DockManager == this.dockManager)
                {
                    this.dockManager.CloseWindow(dockWindow);
                }
            }
        }

        #endregion

        #region Event Handlers

        private void OnMdiClient_ControlAdded(object sender, ControlEventArgs e)
        {
            Debug.Assert(enabled, "MdiClient_ControlAdded should not be received if MdiController is disabled.");
            if (!this.enabled)
            {
                return;
            }

            this.RegisterForm((Form)e.Control);
        }

        private void OnChildForm_Disposed(object sender, EventArgs e)
        {
            if (suspendFormClear > 0)
            {
                return;
            }

            Form form = sender as Form;
            this.ClearForm(form, true);
        }

        private void OnChildForm_Closed(object sender, FormClosedEventArgs e)
        {
            if (suspendFormClear > 0)
            {
                return;
            }

            Form form = sender as Form;
            this.ClearForm(form, true);
        }

        private void OnMdiParent_ControlAdded(object sender, ControlEventArgs e)
        {
            if (this.mdiClient != null || !this.enabled)
            {
                return;
            }

            this.FindMdiClient();
            this.UpdateMdiClient();
        }

        #endregion

        #region Properties

        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                if (this.enabled == value)
                {
                    return;
                }

                this.enabled = value;
                this.OnEnabledChanged();
            }
        }

        public List<Form> MdiChildren
        {
            get
            {
                return this.mdiChildren;
            }
        }

        #endregion
    }
}
