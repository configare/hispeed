using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using GeoDo.RSS.Core.UI;

namespace SMART
{
    public partial class frmFlash : Form, IStartProgress
    {
        private string _processString = "正在初始化......";
        private Font _font = new Font("微软雅黑", 12, FontStyle.Regular);

        public frmFlash()
        {
            InitializeComponent();
            this.Paint+=new PaintEventHandler(frmFlash_Paint);
            pictureBox1.Visible = false;
            Disposed += new EventHandler(FlashForm_Disposed);
            this.TransparencyKey = this.BackColor;
            DoubleBuffered = true;
            //TopMost = true;
        }

        void frmFlash_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(pictureBox1.Image, new RectangleF(0, 0, Width, Height));
            //e.Graphics.DrawString(_processString, _font, Brushes.DimGray, 24, Height - 88);
            e.Graphics.DrawString(_processString, _font, Brushes.DimGray, 24, Height - 88);
        }

        void FlashForm_Disposed(object sender, EventArgs e)
        {
            _font.Dispose();
        }

        public string ProcessString
        {
            set
            {
                _processString = value;
                this.Refresh();
            }
        }

        #region IStartProgress Members

        public void PrintStartInfo(string sInfo)
        {
            ProcessString = sInfo != null ? sInfo : "......";
        }

        public void Stop()
        {
            this.Close();
        }
        #endregion
    }
}
