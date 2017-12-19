using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace GeoDo.RSS.UI.AddIn.MaxCsr
{
    public partial class frmMaxCsr : Form
    {
        string _txtFileName = AppDomain.CurrentDomain.BaseDirectory + "MaxCsrSetting.txt";

        public frmMaxCsr()
        {
            InitializeComponent();
            Load += new EventHandler(frmMaxCsr_Load);
        }

        void frmMaxCsr_Load(object sender, EventArgs e)
        {
            
            if (File.Exists(_txtFileName))
            {
                string[] lines = File.ReadAllLines(_txtFileName,Encoding.Default);
                if (lines != null && lines.Length > 2)
                {
                    txtFileDir.Text = lines[0];
                    txtBandNos.Text = lines[1];
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //save dir to txt file
            if (string.IsNullOrEmpty(txtFileDir.Text))
            {
                MessageBox.Show("未选择待计算文件路径！");
                return;
            }
            if (string.IsNullOrEmpty(txtBandNos.Text))
            {
                MessageBox.Show("未选择计算波段！");
                return;
            }
            using (StreamWriter sw = new StreamWriter(_txtFileName, false, Encoding.Default))
            {
                sw.WriteLine(txtFileDir.Text);
                sw.WriteLine(txtBandNos.Text);
            }
            //Excute exe 
            string exeFileName = AppDomain.CurrentDomain.BaseDirectory + "GeoDo.Smart.MaxCsr.exe";
            Process myprocess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(exeFileName, "\"" + txtFileDir.Text + ";" + txtBandNos.Text + "\"");
            myprocess.StartInfo = startInfo;
            myprocess.StartInfo.UseShellExecute = false;
            myprocess.Start();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnOpenDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    txtFileDir.Text = dialog.SelectedPath;
            }
        }

    }
}
