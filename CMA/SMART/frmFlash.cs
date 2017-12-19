using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using GeoDo.RSS.Core.UI;
using System.IO;

namespace SMART
{
    public partial class frmFlash : Form, IStartProgress
    {
        private string _processString = "正在初始化......";
        private Font _font = new Font("微软雅黑", 12, FontStyle.Regular);
        private Image _flash = null;

        public frmFlash()
        {
            InitializeComponent();
            TryGetFlash();
            this.Paint+=new PaintEventHandler(frmFlash_Paint);
            pictureBox1.Visible = false;
            Disposed += new EventHandler(FlashForm_Disposed);
            this.TransparencyKey = this.BackColor;
            DoubleBuffered = true;
            //TopMost = true;
        }

        private void TryGetFlash()
        {
            try
            {
                string flashFile = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\flash.dat";
                if (File.Exists(flashFile))
                {
                    _flash = Bitmap.FromFile(flashFile);
                    pictureBox1.Image = _flash;
                }
                if (_flash == null)
                    _flash = pictureBox1.Image;
            }
            catch
            {
                if (_flash == null)
                    _flash = pictureBox1.Image;
            }
        }

        void frmFlash_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_flash, new RectangleF(0, 0, Width, Height));
            //e.Graphics.DrawString(_processString, _font, Brushes.DimGray, 24, Height - 88);
            e.Graphics.DrawString(_processString, _font, Brushes.DimGray, 24, Height - 88);
        }

        void FlashForm_Disposed(object sender, EventArgs e)
        {
            if(_flash!=null)
                _flash.Dispose();
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
