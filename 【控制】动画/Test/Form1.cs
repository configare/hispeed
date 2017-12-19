using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.VideoMark;
using System.IO;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Image> res = GetImages();
            Size size = GetSizeFromImages(res);
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "AVI文件(*.avi)|*.avi";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(dialog.FileName))
                        File.Delete(dialog.FileName);
                    Application.DoEvents();
                    ViedoMarkFactory fac = new ViedoMarkFactory();
                    if (fac.Mark(dialog.FileName, "avi", size, 2650, res.ToArray(), null))
                        MessageBox.Show("动画文件:\n\n" + dialog.FileName + "\n已成功保存!");
                }
                //ViedoMarkFactory fac = new ViedoMarkFactory();
                //bool ok = fac.Mark("e://eee", "avi", size, 265, res.ToArray(), null);
            }
        }

        private Size GetSizeFromImages(List<Image> res)
        {
            Size size = res[0].Size;
            foreach (Image img in res)
            {
                if (img.Size.Width > size.Width)
                    size.Width = img.Size.Width;
                if (img.Size.Height > size.Height)
                    size.Height = img.Size.Height;
            }
            return size;
        }

        private void gif_Click(object sender, EventArgs e)
        {
            List<Image> res = GetImages();
            Size size = GetSizeFromImages(res);
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "GIF文件(*.gif)|*.gif";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(dialog.FileName))
                        File.Delete(dialog.FileName);
                    Application.DoEvents();
                    ViedoMarkFactory fac = new ViedoMarkFactory();
                    if (fac.Mark(dialog.FileName, "gif", size, 265, res.ToArray(), null))
                        MessageBox.Show("动画文件:\n\n" + dialog.FileName + "\n已成功保存!");
                }
                //ViedoMarkFactory fac = new ViedoMarkFactory();
                //bool ok = fac.Mark("e://eee", "avi", size, 265, res.ToArray(), null);
            }
        }

        private List<Image> GetImages()
        {
            List<Image> res = new List<Image>();
            res.Add(Bitmap.FromFile(@"E:\王羽\06图相关\沙尘产品截图\沙尘按省级行政区划面积统计.jpg"));
            res.Add(Bitmap.FromFile(@"E:\王羽\06图相关\沙尘产品截图\沙尘交互判识.jpg"));
            res.Add(Bitmap.FromFile(@"E:\王羽\06图相关\沙尘产品截图\沙尘监测示意图.jpg"));
            res.Add(Bitmap.FromFile(@"E:\王羽\06图相关\沙尘产品截图\沙尘能见度专题图.jpg"));
            return res;
        }
    }
}
