#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-11-01 14:38:06
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.DataPro;

namespace GeoDo.RSS.UI.WinForm
{
    /// <summary>
    /// 类名：frmConfig
    /// 属性描述：
    /// 创建者：罗战克   创建日期：2013-11-01 14:38:06
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：集成配置
    /// </summary>
    public partial class frmConfig : Form
    {
        private List<IConfig> _configs = new List<IConfig>();

        public frmConfig()
        {
            InitializeComponent();
            LoadConfigs();
        }

        private void LoadConfigs()
        {
            _configs.Add(new MifConfig());//
            _configs.Add(new ProjectConfig());
            _configs.Add(new AdjustConfig());
            this.tabControl1.TabPages.Clear();
            for (int i = 0; i < _configs.Count; i++)
            {
                IConfig config = _configs[i];
                TabPage page = CreateConfigTab(config);
                this.tabControl1.TabPages.Add(page);
            }
        }

        private TabPage CreateConfigTab(IConfig config)
        {
            TabPage tab = new TabPage(config.ConfigName);
            config.EditControl.Dock = DockStyle.Top;
            tab.Controls.Add(config.EditControl);
            return tab;
        }

        private void btnCalcel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!TrySaveConfigs())
                return;
            this.Close();
        }

        private bool TrySaveConfigs()
        {
            string message = "";
            for (int i = 0; i < _configs.Count; i++)
            {
                IConfig config = _configs[i];
                if (!(config.EditControl as IConfigEdit).TrySaveConfig(out message))
                {
                    MsgBox.ShowInfo(message);
                    return false;
                }
            }
            return true;
        }
    }
}
