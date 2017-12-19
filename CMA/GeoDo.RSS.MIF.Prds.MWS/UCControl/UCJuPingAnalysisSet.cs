#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-4-3 10:40:38
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
using System.IO;
namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：UCJuPingAnalysisSet
    /// 属性描述：距平分析设置
    /// 创建者：Lixj   创建日期：2014-4-3 10:40:38
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public partial class UCJuPingAnalysisSet : UserControl, IArgumentEditorUI
    {
        private Action<object> _handler;
        private List<string> info = new List<string>();

        public UCJuPingAnalysisSet()
        {
            InitializeComponent();
        }
        # region IArgumentEditorUI 成员
        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }
        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }
        public object GetArgumentValue()
        {
            return info;

        }
        #endregion
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string orbitType = "";
            string periodType = "";
            if (radioButton1.Checked)
                orbitType = "Ascend";
            else
                orbitType = "Descend";
            if (radioButton3.Checked)
                periodType = "winter";
            else
                periodType = "usual";//月、旬
            info.Add(orbitType);
            info.Add(periodType);
            string dateregion = startTxt.Text + "—" + endTxt.Text;
            info.Add(dateregion);
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void endTxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
