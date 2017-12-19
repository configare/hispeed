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
    public partial class frmMaxCsd : Form
    {
        string _txtFileName = AppDomain.CurrentDomain.BaseDirectory + "ClearSkyArg.txt";

        public frmMaxCsd()
        {
            InitializeComponent();
            Load += new EventHandler(frmMaxCsd_Load);
        }

        void frmMaxCsd_Load(object sender, EventArgs e)
        {
            if (File.Exists(_txtFileName))
            {
                string[] lines = File.ReadAllLines(_txtFileName, Encoding.Default);
                if (lines != null && lines.Length > 0)
                    InitFrm(lines);
            }
        }

        private void InitFrm(string[] lines)
        {
            txtFileDir.Text = lines[0];
            if (lines.Length > 1)
                txtOutDir.Text = lines[1];
            if (lines.Length > 2)
                txtCalcMothod.Text = lines[2];
            if (lines.Length > 3)
                txtBands.Text = lines[3];
            if (lines.Length > 4)
                txtDataSetBands.Text = lines[4];
            if (lines.Length > 5)
                rbOther.Checked = bool.Parse(lines[5]);
            if (lines.Length > 6)//角度信息
                ckbangle.Checked = bool.Parse(lines[6]);
            if (!rbOther.Checked)
                rbNDVI_CheckedChanged(null, null);
            //文件格式信息
            if (lines.Length > 7)
               this.txtextpars.Text = lines[7];

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //save dir to txt file
            if (!CheckEnv())
                return;
            List<string> context = new List<string>();
            context.Add(txtFileDir.Text);
            context.Add(txtOutDir.Text);
            context.Add(txtCalcMothod.Text);
            context.Add(txtBands.Text);
            context.Add(string.IsNullOrEmpty(txtDataSetBands.Text) ? "ALL" : txtDataSetBands.Text);
            context.Add(rbOther.Checked.ToString());
            context.Add(ckbangle.Checked.ToString());//增加是否输出角度信息功能
            context.Add(this.txtextpars.Text.Trim());//文件格式信息
            File.WriteAllLines(_txtFileName, context.ToArray(), Encoding.Default);

            //Excute exe 
            string exeFileName = AppDomain.CurrentDomain.BaseDirectory + "GeoDo.SMART.CSDatasets.exe";
            Process myprocess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(exeFileName, "\"" + _txtFileName + "\"");
            myprocess.StartInfo = startInfo;
            myprocess.StartInfo.UseShellExecute = false;
            myprocess.Start();
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool CheckEnv()
        {
            if (string.IsNullOrEmpty(txtFileDir.Text))
            {
                MessageBox.Show("请选择输入文件路径", "晴空数据集生产", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(txtOutDir.Text))
            {
                MessageBox.Show("请选择输出文件路径", "晴空数据集生产", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(txtCalcMothod.Text))
            {
                MessageBox.Show("请填写晴空数据集提取的判断逻辑", "晴空数据集生产", MessageBoxButtons.OK);
                return false;
            }
            CreateDir(_txtFileName);
            return true;
        }

        private void CreateDir(string dir)
        {
            string path = Path.GetDirectoryName(dir);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private void btnOpenDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    txtFileDir.Text = dialog.SelectedPath;
            }
        }

        private void btnOutDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    txtOutDir.Text = dialog.SelectedPath;
            }
        }

        private void rbNDVI_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNDVI.Checked)
            {
                txtCalcMothod.Text = "(float)(band2-band1)/(band2+band1)>(float)(band4-band3)/(band4+band3)";
                txtBands.Text = "band1=3,band2=4,band3=3,band4=4";
            }
        }

        private void rbSingleBand_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSingleBand.Checked)
            {
                txtCalcMothod.Text = "band1>band2";
                txtBands.Text = "band1=2,band2=2";
            }
        }

    }
}
