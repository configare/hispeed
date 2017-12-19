using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    [Export(typeof(ICommand))]
    public class CommandFirstPage : GeoDo.RSS.Core.UI.Command
    {
        private IProgressMonitor _progressMonitor = null;

        public CommandFirstPage()
        {
            _id = 39001;
            _name = "firstPage";
            _text = "首页";
            _toolTip = "首页";
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
            ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(firstPage); });
            if (smartWindow == null)
            {
                smartWindow = new firstPage(_smartSession);
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            }
            else
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
        }
    }
}
