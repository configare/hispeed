using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandCreatePyramids : Command
    {
        public CommandCreatePyramids()
            : base()
        {
            _name = "CommandCreatePyramids";
            _text = _toolTip = "建立金字塔";
            _id = 7106;
        }

        public override void Execute()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "All Smart Supported Files(*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Execute(dlg.FileName);
                }
            }
        }

        public override void Execute(string argument)
        {
            if (argument == null)
                return;
            IRasterDataProvider prd = null;
            try
            {
                prd = GeoDataDriver.Open(argument) as IRasterDataProvider;
            }
            catch (Exception ex)
            {
                ExceptionHandler.ShowExceptionWnd(ex, "为文件\"" + Path.GetFileName(argument) + "\"创建金字塔失败！");
                return;
            }
            if (prd == null)
                return;
            if (prd.IsSupprtOverviews)
            {
                IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
                try
                {
                    string tip = "正在建立金字塔\"" + Path.GetFileName(argument) + "\"...";
                    progress.Reset(tip, 100);
                    progress.Start(false);
                    prd.BuildOverviews((idx, tipo) => { progress.Boost(idx, tip); });
                }
                finally
                {
                    progress.Finish();
                }
            }
        }
    }
}
