using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.UI.WinForm
{
    public partial class frmComponentFileSelect : Form
    {
        public frmComponentFileSelect()
        {
            InitializeComponent();
        }

        public string ComponentID
        {
            get 
            {
                if (lvComponents.SelectedIndices.Count == 0)
                    return null;
                return lvComponents.SelectedItem.ToString().Split(':')[1];
            }
        }

        public void Apply(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            foreach (string ln in lines)
            {
                if (ln.Contains("Component ID:"))
                {
                    lvComponents.Items.Add(ln);
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (lvComponents.SelectedIndices.Count == 0)
                return;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
