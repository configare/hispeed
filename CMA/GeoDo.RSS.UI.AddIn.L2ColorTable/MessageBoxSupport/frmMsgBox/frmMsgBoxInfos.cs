using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    public partial class frmMsgBoxInfos : Form
    {
        private int height = 180;
        private int height_more = 350;

        public frmMsgBoxInfos()
        {
            InitializeComponent();
            this.Height = height;
        }

        public DialogResult Show(string title,string msg,string moreMsg)
        {
            this.Text = title;
            this.lblErrMessage.Text = msg;
            this.txtStackTrace.Text = moreMsg;
            return this.ShowDialog();
        }

        private void btnShowDetail_Click(object sender, EventArgs e)
        {
            this.Height = this.Height == height ? height_more : height;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
