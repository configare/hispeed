using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    public partial class frmCreatBookMarkGroup : Form
    {
        private CoordEnvelope _evp = null;
        private string _name = string.Empty;

        public frmCreatBookMarkGroup()
        {
            InitializeComponent();
            txtName.Focus();
            FillGeoRangeControl();
        }

        public frmCreatBookMarkGroup(CoordEnvelope envelop)
        {
            InitializeComponent();
            _evp = envelop;
            txtName.Focus();
            FillGeoRangeControl();
        }

        private void FillGeoRangeControl()
        {
            if (_evp == null)
            {
                ucRange.MinX = 0;
                ucRange.MaxX = 0;
                ucRange.MinY = 0;
                ucRange.MaxY = 0;
            }
            else
            {
                ucRange.MinX = Math.Round(_evp.MinX, 2);
                ucRange.MaxX = Math.Round(_evp.MaxX, 2);
                ucRange.MinY = Math.Round(_evp.MinY, 2);
                ucRange.MaxY = Math.Round(_evp.MaxY, 2);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string text = txtName.Text;
                if (string.IsNullOrEmpty(text))
                    return;
                _name = text;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtTypeName.Text))
                return;
            bool isOk = true;
            if (_evp == null)
                isOk = GetEvpFromUCRange();
            if (!isOk)
                return;
            _name = txtName.Text;
            string groupName = txtTypeName.Text;
            BookMarkParser parser = new BookMarkParser();
            parser.AddBookMarkElement(groupName, _name, _evp);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private bool GetEvpFromUCRange()
        {
            if (ucRange.MinX == 0 && ucRange.MinY == 0 && ucRange.MaxX == 0 && ucRange.MaxY == 0)
                return false;
            else
            {
                _evp = new CoordEnvelope(ucRange.MinX, ucRange.MaxX, ucRange.MinY, ucRange.MaxY);
                return true;
            }
        }
    }
}
