using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public static class AutoGenretateProvider
    {
        public delegate void CustomVarSetterHandler(string[] dblvFileNames,string beginSubprod, IContextEnvironment contextEnv);

        public static void AutoGenrate(ISmartSession session, string beginSubproduct)
        {
            AutoGenrate(session, beginSubproduct, null,null);
        }
        public static void AutoGenrate(ISmartSession session, string beginSubproduct, CustomVarSetterHandler customVarSetter,string executeGroup)
        {
            ICommand cmd = session.CommandEnvironment.Get(9006);
            if (cmd != null)
                cmd.Execute("true");
            try
            {
                //处理过程中不允许关系系统
                session.CloseActionLocker.Lock("监测产品快速生成操作");
                //
                AutoGeneratorSettings settings = GetAutoGeneratorSettings();
                if (settings == null)
                    return;
                AutoGeneratorSettings.CurrentSettings = settings;
                AutoGeneratorSettings.CurrentSettings.GeneratedFileNames = new List<string>();
                AutoGeneratorSettings.CurrentSettings.GxdFileNames = new List<string>();
                AutoGeneratorSettings.CurrentSettings.Session = session;
                //
                DoAutoGenerate(session, beginSubproduct,customVarSetter,executeGroup);
            }
            finally 
            {
                if (AutoGeneratorSettings.CurrentSettings != null)
                {
                    AutoGeneratorSettings.CurrentSettings.CopyFileToCopyFolder();
                    AutoGeneratorSettings.CurrentSettings.ExportImageOfGxdFiles();
                }
                AutoGeneratorSettings.CurrentSettings = null;
                //
                session.CloseActionLocker.Unlock();
            }
        }

        private static AutoGeneratorSettings GetAutoGeneratorSettings()
        {
            using (frmAutoGeneratorSetting frm = new frmAutoGeneratorSetting())
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return frm.Settings;
                }
            }
            return null;
        }

        private static void DoAutoGenerate(ISmartSession session, string beginSubproduct,CustomVarSetterHandler customVarSetter,string executeGroup)
        {
            IContextMessage msg = session.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(9006) as IContextMessage;
            IAutoGenerateExecutor exe = new AutoGenerateExecutor(new AutoGenerateResultHandler(session),
                (session.MonitoringSession as IMonitoringSession).ActiveMonitoringProduct,
                new GeoDo.RSS.MIF.Prds.Comm.CmaThemeGraphGenerator(session), new FileNameGeneratorDefault());
            TrySetContextEnvironment(session,beginSubproduct, exe.ContextEnvironment,customVarSetter);
            DisplayResultClass._contextEnvironment = exe.ContextEnvironment;
            try
            {
                try
                {
                    session.ProgressMonitorManager.DefaultProgressMonitor.Reset(string.Empty, 100);
                    session.ProgressMonitorManager.DefaultProgressMonitor.Start(false);
                    if (string.IsNullOrEmpty(beginSubproduct))
                        exe.Execute(msg, null, executeGroup,(p, txt) =>
                        {
                            session.ProgressMonitorManager.DefaultProgressMonitor.Boost(p, txt);
                        });
                    else
                        exe.Execute(beginSubproduct, msg, null,executeGroup, (p, txt) =>
                        {
                            session.ProgressMonitorManager.DefaultProgressMonitor.Boost(p, txt);
                        });
                }
                finally
                {
                    session.ProgressMonitorManager.DefaultProgressMonitor.Finish();
                }

            }
            finally
            {
                DisplayResultClass._contextEnvironment = null;
            }
        }

        private static void TrySetContextEnvironment(ISmartSession session, string beginSubprod, IContextEnvironment contextEnv, CustomVarSetterHandler customVarSetter)
        {
            contextEnv.Session = session;
            contextEnv.PutContextVar(ContextEnvironment.ENV_VAR_NAME_CURRENT_RASTER_FILE, GetCurrentRasterFile(session));
            IWorkspace ws = (session.MonitoringSession as IMonitoringSession).Workspace;
            if (ws == null)
                return;
            ICatalog catalog = ws.ActiveCatalog;
            if (catalog == null)
                return;
            string[] fnames = catalog.GetSelectedFiles();
            if (fnames == null || fnames.Length == 0)
                return;
            RasterIdentify rst = new RasterIdentify(fnames[0]);
            if (string.IsNullOrEmpty(rst.SubProductIdentify))
                contextEnv.PutContextVar(ContextEnvironment.ENV_VAR_NAME_BINARY_FILE, fnames[0]);
            else
                contextEnv.PutContextVar(rst.SubProductIdentify, fnames[0]);
            //
            if (customVarSetter != null)
                customVarSetter(fnames, beginSubprod, contextEnv);
        }

        private static string GetCurrentRasterFile(ISmartSession session)
        {
            ICanvasViewer cv = session.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return null;
            return (cv.ActiveObject as IRasterDrawing).FileName;
        }
    }
}
