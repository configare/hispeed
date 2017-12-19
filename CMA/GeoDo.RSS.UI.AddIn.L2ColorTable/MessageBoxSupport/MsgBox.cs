using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    public static class MsgBox
    {
        public const string cstInfo = "系统消息";
        public const string cstError = "系统错误";
        public const string cstQuestion = "请确认...";

        public static DialogResult ShowError(string sError)
        {
            return MessageBox.Show(sError,cstError,MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        public static DialogResult ShowInfo(string sInfo)
        {
            return MessageBox.Show(sInfo, cstInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult ShowQuestionYesNo(string sQuestion)
        {
            return MessageBox.Show(sQuestion, cstQuestion, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        //20100608添加代码开始
        public static DialogResult ShowMoreInfos(string sInfo, string moreInfos)
        {
            return (new frmMsgBoxInfos()).Show(cstInfo, sInfo, moreInfos);
        }
        //20100608添加代码结束
    }
}
