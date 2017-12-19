using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.UIs;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public partial class frmNewSpatialDbConn : Form
    {
        public frmNewSpatialDbConn()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Test();
        }

        private bool Test()
        {
            string connString = GetConnString();
            try
            {
                using (IDbConnection conn = DbConnectionFactory.CreateDbConnection(connString))
                {
                    conn.Open();
                    MsgBox.ShowInfo("测试连接成功。");
                    return true;
                }
            }
            catch (Exception ex)
            {
                MsgBox.ShowError("测试连接失败:" + ex.Message);
            }
            return false;
        }

        public string GetConnString()
        {
            enumSpatialDatabaseType dbType = enumSpatialDatabaseType.Unknow ;
            if (rdMySQL.Checked)
                dbType = enumSpatialDatabaseType.MySql;
            else if (rdOracle.Checked)
                dbType = enumSpatialDatabaseType.Oracle;
            else if (rdSQLServer.Checked)
                dbType = enumSpatialDatabaseType.MsSql;
            return dbType.ToString() + "$" + textBox1.Text;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MsgBox.ShowError("连接字符串必须输入。");
                return;
            }
            if (!Test())
                return;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
