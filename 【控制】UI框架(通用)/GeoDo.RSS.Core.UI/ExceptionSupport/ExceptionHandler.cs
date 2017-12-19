using System;

namespace GeoDo.RSS.Core.UI
{
    /// <summary>
    /// 异常处理类
    /// </summary>
    public static class ExceptionHandler
    {
        public static void ShowExceptionWnd(Exception Ex, string ActionDes)
        {
            frmException _frm = new frmException();
            _frm.ErrException = Ex;
            _frm.ActionDescription = ActionDes;
            _frm.ShowDialog();
        }
    }
}
