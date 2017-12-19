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

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public partial class UCFileSelectBase : UserControl
    {
        public delegate void OpenFileHandler(ComboBox cmbFile);
        public OpenFileHandler OpenFile = null;

        public UCFileSelectBase()
        {
            InitializeComponent();
            Load += new EventHandler(UCFileSelectBase_Load);
        }

        void UCFileSelectBase_Load(object sender, EventArgs e)
        {
            InitControls();
        }

        private void InitControls()
        {
            lbFile.Location = new System.Drawing.Point(3, 6);
            cmbFile.Location = new System.Drawing.Point(lbFile.Right + 6, 6);
            cmbFile.Size = new System.Drawing.Size(this.Width - lbFile.Width - 36, 20);
            btOpen.Location = new System.Drawing.Point(this.Width - 24, 3);
            btOpen.Size = new System.Drawing.Size(24, 24);
            btOpen.UseVisualStyleBackColor = true;
            this.Height = 30;
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            if (OpenFile != null)
                OpenFile(cmbFile);
        }


    }
}
