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
    public partial class frmDataSource : Form
    {
        public frmDataSource()
        {
            InitializeComponent();
            ucBudGISDataSource1.OnCatalogItemDoubleClicked += new OnCatalogItemDoubleClickedHandler(ucBudGISDataSource1_OnCatalogItemDoubleClicked);
        }

        void ucBudGISDataSource1_OnCatalogItemDoubleClicked(object sender, ICatalogItem catalogItem)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ucBudGISDataSource1.SelectedCatalogItem != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
