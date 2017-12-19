using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand))]
    public class CommandFileRename : GeoDo.RSS.Core.UI.Command
    {
        private IProgressMonitor _progressMonitor = null;

        public CommandFileRename()
        {
            _id = 4007;
            _name = "FileRename";
            _text = "文件重命名";
            _toolTip = "文件重命名";
        }

        public override void Execute()
        {
            Execute("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument">
        /// </param>
        public override void Execute(string argument)
        {
            try
            {
                string exe = System.AppDomain.CurrentDomain.BaseDirectory+"GeoDo.Tools.ReNameFY.exe";
                ProcessStartInfo info = new ProcessStartInfo(exe);
                using (Process processs = Process.Start(info))
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
