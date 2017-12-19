#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-1-16 9:53:37
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

namespace GeoDo.RSS.DF.AddIn.HDF5GEO
{
    /// <summary>
    /// 类名：frmFYSnowPrdDataSelecte
    /// 属性描述：
    /// 创建者：LiXJ   创建日期：2014-1-16 9:53:37
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public partial class frmFYSnowPrdDataSelecte : Form
    {
        public frmFYSnowPrdDataSelecte()
        {
            InitializeComponent();

        }
        public string ComponentID
        {
            get
            {
                if (lvComponents.SelectedIndices.Count == 0)
                    return null;
                string id = "";
                string pole = lvComponents.SelectedItem.ToString();
                if (lvComponents.SelectedItem == "EASE-Grid North")
                {
                    id = "SD_Flags_NorthernDaily_A,SD_Flags_NorthernDaily_D,SD_NorthernDaily_A,SD_NorthernDaily_D,SWE_Flags_NorthernDaily_A,SWE_Flags_NorthernDaily_D,SWE_NorthernDaily_A,SWE_NorthernDaily_D";
                }
                if (lvComponents.SelectedItem == "EASE-Grid South")
                {
                    id = "SD_Flags_SouthernDaily_A,SD_Flags_SouthernDaily_D,SD_SouthernDaily_A,SD_SouthernDaily_D,SWE_Flags_SouthernDaily_A,SWE_Flags_SouthernDaily_D,SWE_SouthernDaily_A,SWE_SouthernDaily_D";
                }
                return id;
            }
        }
        public void Apply(string[] alldatasets)
        {
            foreach (string ln in alldatasets)
            {
                lvComponents.Items.Add(ln);
            }
            lvComponents.SetSelected(0, true);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (lvComponents.SelectedIndices.Count == 0)
                return;

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
