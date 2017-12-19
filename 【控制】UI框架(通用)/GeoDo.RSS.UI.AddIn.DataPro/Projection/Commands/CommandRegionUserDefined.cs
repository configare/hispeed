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
    public class CommandRegionUserDefined : GeoDo.RSS.Core.UI.Command
    {
        private IProgressMonitor _progressMonitor = null;

        public CommandRegionUserDefined()
        {
            _id = 4006;
            _name = "RegionUserDefined";
            _text = "自定义分幅范围";
            _toolTip = "自定义分幅范围";
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
                frmDefinedRegionManager frm = new frmDefinedRegionManager();
                if (frm.ShowDialog(_smartSession.SmartWindowManager.MainForm as System.Windows.Forms.Form) == System.Windows.Forms.DialogResult.OK)
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
