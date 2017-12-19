#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/4 9:44:37
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

namespace GeoDo.RSS.DF.MicapsData
{
    /// <summary>
    /// 类名：frmMicapsDataTypeSelect
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/4 9:44:37
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public partial class frmMicapsDataTypeSelect : Form
    {
        public frmMicapsDataTypeSelect()
        {
            InitializeComponent();
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



    }
}
