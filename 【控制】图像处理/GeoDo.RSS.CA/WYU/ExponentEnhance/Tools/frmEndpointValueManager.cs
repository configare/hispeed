using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.CA
{
    internal partial class frmEndpointValueManager : Form
    {
        private List<EndpointValueItem> _items = null;

        public frmEndpointValueManager(List<EndpointValueItem> items)
        {
            InitializeComponent();
            _items = items;
            foreach (EndpointValueItem it in _items)
                listBox1.Items.Add(it);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndices.Count == 0)
                return;
            for (int i = listBox1.SelectedItems.Count-1; i >= 0; i--)
            {
                object it = listBox1.SelectedItems[i];
                listBox1.Items.Remove(it);
                _items.Remove(it as EndpointValueItem);
            }
        }
    }
}
