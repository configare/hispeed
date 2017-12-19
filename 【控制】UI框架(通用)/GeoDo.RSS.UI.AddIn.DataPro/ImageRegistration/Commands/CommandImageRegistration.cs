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
    public class CommandImageRegistration : GeoDo.RSS.Core.UI.Command
    {
        private IProgressMonitor _progressMonitor = null;

        public CommandImageRegistration()
        {
            _id = 19001;
            _name = "ImageRegistration";
            _text = "图像配准";
            _toolTip = "图像配准";
        }

        public override void Execute()
        {
            Execute("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument">
        /// 格式"*#*"冒号前面部分表示文件名称，冒号后面部分表示所选投影坐标的Wkt格式字符串
        /// </param>
        public override void Execute(string argument)
        {
            try
            {
                using (frmImageRegistration frm = new frmImageRegistration())
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        string filename = frm.Filename;
                        _smartSession.CommandEnvironment.Get(2000).Execute(filename);
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
