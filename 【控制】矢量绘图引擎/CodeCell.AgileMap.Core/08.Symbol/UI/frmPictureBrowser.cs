using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Text;

namespace CodeCell.AgileMap.Core
{
    public partial class frmPictureBrowser : Form
    {
        class DirInfo
        {
            public string Name = null;
            public string Dir = null;

            public DirInfo(string dir)
            {
                string[] ps = dir.Split(Path.DirectorySeparatorChar);
                Name = ps[ps.Length - 1];
                Dir = dir;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public frmPictureBrowser()
        {
            InitializeComponent();
            Load += new EventHandler(frmTrueTypeFontBrowser_Load);
            txtCategories.SelectedIndexChanged += new EventHandler(txtCategories_SelectedIndexChanged);
        }

        void txtCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadImages((txtCategories.SelectedItem as DirInfo).Dir);
        }

        private void LoadImages(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return;
            string[] files = Directory.GetFiles(dir);
            if (files == null || files.Length == 0)
                return;
            List<string> fs = new List<string>();
            foreach (string f in files)
            {
                string ext = Path.GetExtension(f).ToUpper();
                switch (ext)
                { 
                    case ".PNG":
                    case ".JPE":
                    case ".JPEG":
                    case ".BMP":
                    case ".ICO":
                        fs.Add(f);
                        break;
                    default:
                        continue;
                }
            }
            if (fs.Count > 0)
                ucPictureBrowser1.ApplyImages(fs.ToArray());
        }

        void frmTrueTypeFontBrowser_Load(object sender, EventArgs e)
        {
            if (Application.StartupPath.Contains("IDE"))
                return;
            LoadImageCategories();
        }

        private void LoadImageCategories()
        {
            string dir = Constants.GetMapResourceDir();
            string[] subdirs = Directory.GetDirectories(dir);
            if (subdirs == null || subdirs.Length == 0)
                return;
            foreach (string d in subdirs)
            {
                txtCategories.Items.Add(new DirInfo(d));
            }
            if(txtCategories.Items.Count>0)
                txtCategories.SelectedIndex = 0;
        }

        internal ImageItem GetSelectedImage()
        {
            return ucPictureBrowser1.GetSelectedImage();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void frmTrueTypeFontBrowser_Load_1(object sender, EventArgs e)
        {

        }

        private void btnNewCategory_Click(object sender, EventArgs e)
        {
            //
        }

        private void btnFromFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "All Supported Picture Files(*.jpg,*.png,*.ico,*.bmp)|*.jpg;*.png;*.ico;*.bmp";
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string[] filenames = dlg.FileNames;
                    if (filenames == null || filenames.Length == 0)
                        return;
                    DirInfo info = txtCategories.SelectedItem as DirInfo ;
                    if(info == null)
                    {
                        Directory.CreateDirectory(Constants.GetMapResourceDir()+ "\\Default");
                        txtCategories.Items.Add(info);
                        txtCategories.SelectedIndex = 0;
                    }
                    foreach (string f in filenames)
                    {
                        try
                        {
                            File.Copy(f, Path.Combine(info.Dir, Path.GetFileName(f)), false);
                        }
                        catch
                        {
                        }
                    }
                    txtCategories_SelectedIndexChanged(null, null);
                }
            }
        }
    }
}
