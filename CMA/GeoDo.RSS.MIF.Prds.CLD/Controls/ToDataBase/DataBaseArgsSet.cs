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
    public partial class DataBaseArgsSet : Form
    {
        private static string _dataBaseXml = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\CLD\CPDataBaseArgs.xml";

        public DataBaseArgsSet()
        {
            InitializeComponent();
            TryParseXmlArgs();
            cbxDisplayPasswords.CheckedChanged += new EventHandler(cbxDisplayPasswords_CheckedChanged);
        }

        public void TryParseXmlArgs()
        {
            if (File.Exists(_dataBaseXml))
            {
                DataBaseArg arg = DataBaseArg.ParseXml(_dataBaseXml);
                txtserver.Text = arg.ServerName;
                txtDatabase.Text = arg.DatabaseName;
                txtAccount.Text = arg.UID;
                txtpassword.Text = arg.Passwords;
                MODISRootPath.Text=arg.OutputDir ;
               AIRSRootPath.Text=arg.AIRSRootPath ;
                 ISCCPRootPath.Text=arg.ISCCPRootPath ;
                CloudSATRootPath.Text=arg.CloudSATRootPath;
            }
        }
        void cbxDisplayPasswords_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxDisplayPasswords.Checked)
            {
                txtpassword.PasswordChar = '\0';
            } 
            else
            {
                txtpassword.PasswordChar = '*';
            }
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckArgsIsOK())
                {
                    SaveArgs2Xml();
                    MessageBox.Show("保存配置成功");
                    this.Close();
                }
            }
            catch(SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveArgs2Xml()
        {
            DataBaseArg arg = new DataBaseArg();
            arg.ServerName = txtserver.Text;
            arg.DatabaseName = txtDatabase.Text;
            arg.UID = txtAccount.Text;
            arg.Passwords = txtpassword.Text;
            arg.OutputDir = MODISRootPath.Text;
            arg.AIRSRootPath = AIRSRootPath.Text;
            arg.ISCCPRootPath = ISCCPRootPath.Text;
            arg.CloudSATRootPath = CloudSATRootPath.Text;
            arg.ToXml(_dataBaseXml);
        }


        private bool CheckArgsIsOK()
        {
            if (string.IsNullOrEmpty(txtserver.Text) || string.IsNullOrEmpty(txtDatabase.Text) || string.IsNullOrEmpty(txtAccount.Text) || string.IsNullOrEmpty(txtpassword.Text))
            {
                MessageBox.Show("请设置数据库参数！");
                return false;
            }
            if (string.IsNullOrEmpty(this.MODISRootPath.Text) || string.IsNullOrEmpty(AIRSRootPath.Text) || string.IsNullOrEmpty(ISCCPRootPath.Text) || string.IsNullOrEmpty(CloudSATRootPath.Text))
            {
                MessageBox.Show("请设置归档目录参数！");
                return false;
            }
            if (!Directory.Exists(MODISRootPath.Text))
                Directory.CreateDirectory(MODISRootPath.Text);
            if (!Directory.Exists(AIRSRootPath.Text))
                Directory.CreateDirectory(AIRSRootPath.Text);
            if (!Directory.Exists(ISCCPRootPath.Text))
                Directory.CreateDirectory(ISCCPRootPath.Text);
            if (!Directory.Exists(MODISRootPath.Text))
                Directory.CreateDirectory(MODISRootPath.Text);
            return true;
        }
        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTestLink_Click(object sender, EventArgs e)
        {
            ConnectMySqlCloud con = new ConnectMySqlCloud(txtserver.Text, txtDatabase.Text, txtAccount.Text, txtpassword.Text);
            try
            {
                if (con.ConnOpend()||con.OpenConn())
                {
                    MessageBox.Show("链接数据库成功！");
                }
                else
                {
                    MessageBox.Show("链接数据库失败！请确认服务已开启并输入正确的参数！");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }
}
