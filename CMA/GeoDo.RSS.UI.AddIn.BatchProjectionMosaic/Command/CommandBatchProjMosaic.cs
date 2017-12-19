using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using System.Diagnostics;

namespace GeoDo.RSS.UI.AddIn.BatchProjectionMosaic
{
    [Export(typeof(ICommand))]
    public class CommandBatchProjMosaic:Command
    {
        public CommandBatchProjMosaic()
        {
            _id = 4005;
            _name = "BatchProjectionMosaic";
            _text = "批量拼接投影";
            _toolTip = "批量拼接投影";
        }

        public override void Execute()
        {
            try
            {
                base.Execute();
                using (frmArgumentSetting frm = new frmArgumentSetting())
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        string filename = AppDomain.CurrentDomain.BaseDirectory + "GeoDo.Smart.BatchProjectionMosaic.exe";
                        //调用exe
                        try
                        {
                            string s = frm._path;
                            Process myprocess = new Process();
                            ProcessStartInfo startInfo = new ProcessStartInfo(filename, "\""+ s + "\"");
                            myprocess.StartInfo = startInfo;
                            myprocess.StartInfo.UseShellExecute = false;
                            myprocess.Start();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("启动应用程序时出错！原因：" + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
