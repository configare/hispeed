using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand))]
    public class CommandProjectCacheClear : GeoDo.RSS.Core.UI.Command
    {
        private IProgressMonitor _progressMonitor = null;

        public CommandProjectCacheClear()
        {
            _id = 4004;
            _name = "投影缓存文件清理";
            _text = "投影缓存文件清理";
            _toolTip = "Projection Cache Clear";
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
                string cachePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, ".prjChche");
                string[] dirs = Directory.GetDirectories(cachePath);
                OnProgressStart();
                for (int i = 0; i < dirs.Length; i++)
                {
                    try
                    {
                        Directory.Delete(dirs[i], true);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        int progress = (int)((i + 1.0) / dirs.Length);
                        OnProgress(progress, dirs[i]);
                    }
                }
                MsgBox.ShowInfo("清理完毕");
            }
            catch (Exception ex)
            {
                MsgBox.ShowInfo(ex.Message);
            }
            finally
            {
                OnProgressEnd();
            }
        }

        private void OnProgress(int progerss, string text)
        {
            if (_progressMonitor != null)
                _progressMonitor.Boost(progerss, text);
        }

        private void OnProgressStart()
        {
            if (_progressMonitor != null)
            {
                _progressMonitor.Reset("", 100);
                _progressMonitor.Start(false);
            }
        }

        private void OnProgressEnd()
        {
            if (_progressMonitor != null)
                _progressMonitor.Finish();
        }
    }
}
