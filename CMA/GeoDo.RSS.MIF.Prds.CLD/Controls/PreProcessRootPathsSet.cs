using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class PreProcessRootPathsSet : Form
    {
        private static string _RootPathsArgsXml = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\CLD\PreProcessRootPathsArgs.xml";
        private bool isChanged = false,isInit =true;

        public PreProcessRootPathsSet()
        {
            InitializeComponent();
            TryParseXmlArgs();
        }

        public void TryParseXmlArgs()
        {
            if (File.Exists(_RootPathsArgsXml))
            {
                DataBaseArg arg = new DataBaseArg();
                arg.ParseRootPathXml(_RootPathsArgsXml);
                if (arg.MODISRootPath != null && arg.MODISRootPath.Count > 0)
                {
                    Dictionary<int, string> modisdir = arg.MODISRootPath;
                    foreach (int year in modisdir.Keys)
                    {
                        cbxMODISYear.Items.Add(year);
                        cbxMODISdir.Items.Add(modisdir[year]);
                    }
                }
                if (arg.ISCCPRootPath!=null)
                    ISCCPRootPath.Text = arg.ISCCPRootPath;
                if (arg.AIRSRootPath != null)
                    AIRSRootPath.Text = arg.AIRSRootPath;
                if (arg.CloudSATRootPath != null)
                    ClouSATRootPath.Text = arg.CloudSATRootPath;
                isInit = false;
            }
        }
        #region 事件响应
        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RootPath_TextChanged(object sender, EventArgs e)
        {
            if (isInit == true)
                return;
            else
                isChanged = true;
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            if (!isChanged)
                this.Close();
            if (CheckArgsIsOK())
            {
                SaveArgs2Xml();
                MessageBox.Show("保存配置成功");
                this.Close();
            } 
        }


        #endregion
        private void SaveArgs2Xml()
        {
            //根据界面显示将MODIS路径添加为dictionary
            DataBaseArg arg = new DataBaseArg();
            if (cbxMODISYear.Items.Count>0)
            {
                Dictionary<int, string> modisdir = new Dictionary<int, string>();
                int year;
                for(int i=0;i<cbxMODISYear.Items.Count;i++)
                {
                    if(int.TryParse(cbxMODISYear.Items[i].ToString(),out year))
                        modisdir.Add(year,cbxMODISdir.Items[i].ToString());
                }
                if (modisdir.Count > 0)
                    arg.MODISRootPath = modisdir;
            }
            if (!string.IsNullOrEmpty(ISCCPRootPath.Text) || !string.IsNullOrWhiteSpace(ISCCPRootPath.Text))
                arg.ISCCPRootPath = ISCCPRootPath.Text;
            if (!string.IsNullOrEmpty(AIRSRootPath.Text) || !string.IsNullOrWhiteSpace(AIRSRootPath.Text))
                arg.AIRSRootPath = AIRSRootPath.Text;
            if (!string.IsNullOrEmpty(ClouSATRootPath.Text) || !string.IsNullOrWhiteSpace(ClouSATRootPath.Text))
                arg.CloudSATRootPath = ClouSATRootPath.Text;
            arg.RootPathToXml(_RootPathsArgsXml);
        }

        private bool CheckArgsIsOK()
        {
            if (string.IsNullOrEmpty(AIRSRootPath.Text) || string.IsNullOrEmpty(ISCCPRootPath.Text) || string.IsNullOrEmpty(ClouSATRootPath.Text))
            {
                MessageBox.Show("请设置数据库参数！");
                return false;
            }
            if (!Directory.Exists(AIRSRootPath.Text))
                Directory.CreateDirectory(AIRSRootPath.Text);
            if (!Directory.Exists(ISCCPRootPath.Text))
                Directory.CreateDirectory(ISCCPRootPath.Text);
            if (!Directory.Exists(ClouSATRootPath.Text))
                Directory.CreateDirectory(ClouSATRootPath.Text);
            return true;
        }

        private void cbxMODISYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxMODISYear.SelectedIndex >= 0 && cbxMODISYear.SelectedIndex<cbxMODISdir.Items.Count)
            {
                cbxMODISdir.SelectedIndex = cbxMODISYear.SelectedIndex;
            }
            else
                cbxMODISdir.Items.Add(@"D:\");
        }

   }
}
