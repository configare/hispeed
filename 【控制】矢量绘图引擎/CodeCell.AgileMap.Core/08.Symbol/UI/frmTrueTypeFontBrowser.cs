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
    public partial class frmTrueTypeFontBrowser : Form
    {
        public frmTrueTypeFontBrowser()
        {
            InitializeComponent();
            Load += new EventHandler(frmTrueTypeFontBrowser_Load);
            ucTrueTypeBrowser1.CurrentCharItemChanged += new UCTrueTypeBrowser.CurrentCharItemChangedHandler(ucTrueTypeBrowser1_CurrentCharItemChanged);
        }

        void ucTrueTypeBrowser1_CurrentCharItemChanged(CharItem it)
        {
            if (it == null)
            {
                textBox1.Tag = null;
                charLarge.Text = string.Empty;
                return;
            }
            string text = new string(Convert.ToChar(it.Code), 1);
            charLarge.Text = text;
            textBox1.Text = it.Code.ToString();
            textBox1.Tag = it;
        }

        void frmTrueTypeFontBrowser_Load(object sender, EventArgs e)
        {
            if (Application.StartupPath.Contains("IDE"))
                return;
            LoadFontNames();
        }

        private void LoadFontNames()
        {
            cbFonts.SelectedIndexChanged += new EventHandler(cbFonts_SelectedIndexChanged);
            InstalledFontCollection enumFonts = new InstalledFontCollection();
            FontFamily[] fonts = enumFonts.Families;
            foreach (FontFamily font in fonts)
            {
                cbFonts.Items.Add(font.Name);
            }
            int idx = cbFonts.Items.IndexOf("ESRI Business");
            if (idx > 0)
                cbFonts.SelectedIndex = idx;
            else
                cbFonts.SelectedIndex = 0;
            cbFonts.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        void cbFonts_SelectedIndexChanged(object sender, EventArgs e)
        {
            charLarge.Font = new Font(cbFonts.Text, 32,FontStyle.Bold);
            charLarge.ForeColor = Color.Red;
            ucTrueTypeBrowser1.ChangeFontName(cbFonts.Text);
        }

        public CharItem SelectedCharItem
        {
            get 
            {
                return textBox1.Tag as CharItem;
            }
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
    }
}
