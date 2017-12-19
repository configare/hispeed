using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Components
{
    public partial class frmEntityEditor : Form
    {
        protected bool _isNew = false;
        protected ICatalogEntity _entity = null;
        protected bool _readOnly = false;

        public frmEntityEditor()
        {
            InitializeComponent();
            Load += new EventHandler(frmEntityEditor_Load);
        }

        void frmEntityEditor_Load(object sender, EventArgs e)
        {
            FillEntityToCotrols();
            if (_readOnly)
            {
                foreach (Control c in this.Controls)
                {
                    SetEnable(c, false);
                }
            }
        }

        private void SetEnable(Control pControl, bool enabled)
        {
            bool e = (pControl.Name == "btnCancel" || pControl.GetType().Equals(typeof(TabControl)) || pControl.GetType().Equals(typeof(TabPage)));
            pControl.Enabled = e;
            if (pControl.Controls.Count > 0)
                foreach (Control c in pControl.Controls)
                    SetEnable(c, enabled);
        }

        protected virtual void FillEntityToCotrols()
        {
        }

        public virtual bool ShowDialog(ref ICatalogEntity entity,bool isNew,bool readOnly)
        {
            _isNew = isNew;
            _entity = entity;
            _readOnly = readOnly;
            return ShowDialog() == DialogResult.OK;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (IsCanSave())
            {
                CollectValuesToEntity();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        protected virtual bool IsCanSave()
        {
            return false;
        }

        protected virtual void CollectValuesToEntity()
        {
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
