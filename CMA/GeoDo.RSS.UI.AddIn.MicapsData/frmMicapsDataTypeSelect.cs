#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/5 13:54:56
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
using System.IO;
using System.Xml.Linq;

namespace GeoDo.RSS.UI.AddIn.MicapsData
{
    /// <summary>
    /// 类名：frmMicapsDataTypeSelect
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/5 13:54:56
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public partial class frmMicapsDataTypeSelect : Form
    {
        private string _selectDataType;

        public frmMicapsDataTypeSelect()
        {
            InitializeComponent();
            InitDataTypeList();
        }

        public string SelectDataType
        {
            get { return _selectDataType; }
        }

        private void InitDataTypeList()
        {
           //从配置文件读取观测数据类型
            if (!File.Exists(MicapsFileProcessor.DATATYPE_CONFIG_DIR))
                return;
            XElement root = XElement.Load(MicapsFileProcessor.DATATYPE_CONFIG_DIR);
            IEnumerable<XElement> items = root.Elements("DataDefine");
            if (items == null || items.Count() == 0)
                return ;
            string typeName;
            foreach (XElement item in items)
            {
                typeName=item.Attribute("name").Value;
                lstDataTypes.Items.Add(typeName);
            }
            if (lstDataTypes.Items.Count > 1)
                lstDataTypes.SelectedIndex = 0;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(lstDataTypes.SelectedItem!=null)
               _selectDataType = lstDataTypes.SelectedItem.ToString();
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
