using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using GeoDo.RSS.RasterTools;
using System.Threading;

namespace test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.FileName = "f:\\4_大昭寺_IMG_GE.tif";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(dlg.FileName);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bm = (Bitmap)pictureBox1.Image;
            BitmapData pdata = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
                ImageLockMode.ReadWrite, bm.PixelFormat);
            try
            {
                IBitmapMagicWand wand = new BitmapMagicWand();
                ScanLinePolygon[] polygons;
                //wand.Extract(pdata, new Point(497, 200),
                //    new byte[] { 227, 9, 0 },
                //    64,
                //    true,
                //    out bs);
                //wand.Extract(pdata, new Point(0, 0),
                //    new byte[] { 255, 212, 23 },
                //    64,
                //    false,
                //    out bs);
                //wand.Extract(pdata, new Point(6584, 5516),
                //    new byte[] { 23, 20, 15 },
                //    16,
                //    true,
                //    out bs);
                wand.FillColor(pdata, new Point(76, 98), 32, true, new byte[] { 0, 0, 255 });
            }
            finally
            {
                bm.UnlockBits(pdata);
            }
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save("f:\\1.bmp", ImageFormat.Bmp);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Bitmap bm = (Bitmap)pictureBox1.Image;
                BitmapData pdata = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, bm.PixelFormat);
                try
                {
                    using (IBitmapMagicWand wand = new BitmapMagicWand())
                    {
                        Text = e.Location.ToString();
                        wand.FillColor(pdata, new Point(e.X, e.Y), 16, checkBox1.Checked, new byte[] { 0, 0, 255 });
                        //ScanLineSegment[] segs = wand.ExtractSnappedPixels(pdata, new Point(e.X, e.Y), 16, checkBox1.Checked);
                    }
                }
                finally
                {
                    bm.UnlockBits(pdata);
                }
                pictureBox1.Refresh();
            }
        }
    }
}
