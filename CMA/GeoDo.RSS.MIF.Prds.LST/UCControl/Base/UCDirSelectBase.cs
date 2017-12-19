using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public partial class UCDirSelectBase : UserControl
    {
        public delegate void SelectDirHandler(TextBox txtDir);
        public SelectDirHandler SelectDir = null;

        public UCDirSelectBase()
        {
            InitializeComponent();
            Load += new EventHandler(UCDirSelectBase_Load);
        }

        void UCDirSelectBase_Load(object sender, EventArgs e)
        {
            InitControls();
        }

        private void InitControls()
        {
            lbFile.Location = new System.Drawing.Point(3, 6);
            txtDir.Location = new System.Drawing.Point(lbFile.Right + 6, 6);
            txtDir.Size = new System.Drawing.Size(this.Width - lbFile.Width - 36, 20);
            btDir.Location = new System.Drawing.Point(this.Width - 24, 3);
            btDir.Size = new System.Drawing.Size(24, 24);
            btDir.UseVisualStyleBackColor = true;
            this.Height = 30;
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            if (SelectDir != null)
                SelectDir(txtDir);
        }

        public void SetTextBoxInfo(string text)
        {
            txtDir.Text = text;
        }
    }
}
