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
    public class CommandHDFProductProject : GeoDo.RSS.Core.UI.Command
    {
        private IProgressMonitor _progressMonitor = null;

        public CommandHDFProductProject()
        {
            _id = 4011;
            _name = "风三轨道产品投影";
            _text = "风三轨道产品投影";
            _toolTip = "风三轨道产品投影";
        }

        public override void Execute()
        {
            Execute("");
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Execute(string argument)
        {
            try
            {
                frmFY3OrbitProductProjection frm = new frmFY3OrbitProductProjection();
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnProgress(int progerss, string text)
        {
            if (_progressMonitor != null)
                _progressMonitor.Boost(progerss, text);
        }
    }
}
