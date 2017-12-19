using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public static class SimpleActionExecutor
    {
        public static void Execute(IAction action)
        {
            using (frmPropertyEditorDialog frm = new frmPropertyEditorDialog())
            {
                IPropertyEditorDialog dlg = frm as IPropertyEditorDialog;
                bool isOk = dlg.ShowDialog(action, null);
                if (isOk)
                {
                    using (frmActionExecutor exefrm = new frmActionExecutor())
                    {
                        IActionExecutor exe = exefrm as IActionExecutor;
                        exe.Queue(action);
                        exefrm.ShowDialog();
                    }
                }
            }
        }
    }
}
