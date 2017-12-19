using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    public partial class UCLayerManagerPanel : UserControl
    {
        private ILayersProvider _layersProvider;
        private ISmartSession _session;

        public UCLayerManagerPanel(ISmartSession session)
        {
            InitializeComponent();
            _session = session;
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            btnDisplayProperty.Click += new EventHandler(btnDisplayProperty_Click);
            btnDelete.Click += new EventHandler(btnDelete_Click);
            btnLockDrag.Click += new EventHandler(btnLockDrag_Click);
            btnLockDrag.CheckedChanged += new EventHandler(btnLockDrag_CheckedChanged);
            btnAdd.Click += new EventHandler(btnAdd_Click);
            btnLayerUp.Click += new EventHandler(btnLayerUp_Click);
            btnLayerDown.Click += new EventHandler(btnLayerDown_Click);
            ucLayerManager.OnLayerItemClick += new UCLayerManager.OnLayerItemClickHandler(ucLayerManager_OnLayerItemClick);
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateLayers();
        }

        internal void UpdateLayers()
        {
            ucLayerManager.Update();
        }

        void btnLockDrag_CheckedChanged(object sender, EventArgs e)
        {
            if (btnLockDrag.Checked)
                btnLockDrag.Image = GeoDo.RSS.UI.AddIn.Windows.Properties.Resources.lock_unlock;
            else
                btnLockDrag.Image = GeoDo.RSS.UI.AddIn.Windows.Properties.Resources._lock;
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(2004);
            if (cmd != null)
            {
                cmd.Execute();
                ucLayerManager.Update();
            }
        }

        public void Apply(ILayersProvider provider)
        {
            _layersProvider = provider;
            ucLayerManager.Apply(_layersProvider);
            ucLayerManager.Invalidate();
            SetNullObject();
        }

        private void SetNullObject()
        {
            propertyGrid.SelectedObject = new object();
        }

        private void btnLockDrag_Click(object sender, EventArgs e)
        {
            btnLockDrag.Checked = !btnLockDrag.Checked;
            ucLayerManager.AllowDrag = btnLockDrag.Checked;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (ucLayerManager.Provider == null || ucLayerManager.CurrentLayerItem == null)
                return;
            ucLayerManager.Provider.Remove(ucLayerManager.CurrentLayerItem);
            ucLayerManager.Invalidate();
        }

        private void btnDisplayProperty_Click(object sender, EventArgs e)
        {
            btnDisplayProperty.Checked = !btnDisplayProperty.Checked;
            propertyGrid.Visible = btnDisplayProperty.Checked;
        }

        void btnLayerDown_Click(object sender, EventArgs e)
        {
            if (btnLockDrag.Checked)
            {
                ucLayerManager.SetLayerItemDown();
            }
        }

        void btnLayerUp_Click(object sender, EventArgs e)
        {
            if (btnLockDrag.Checked)
            {
                ucLayerManager.SetLayerItemUp();
            }
        }
        private void ucLayerManager_OnLayerItemClick(ILayerItem layerItem)
        {
            if (layerItem == null)
                return;
            if (layerItem.Tag != null)
                propertyGrid.SelectedObject = layerItem.Tag;
            else
                propertyGrid.SelectedObject = new object();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _layersProvider.RefreshViewer();
        }
    }
}
