using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.IO;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.MEF;

namespace GeoDo.RSS.Core.UI
{
    public static class OpenFileFactory
    {
        private static List<IOpenFileProcessor> _fileOpenOperators;
        private static ISmartSession _session;
        public static int ALLOW_MAX_RASTER_COUNT = 0;

        public static void RegisterAll(ISmartSession session)
        {
            _session = session;
            _fileOpenOperators = new List<IOpenFileProcessor>();
            string[] dlls = MefConfigParser.GetAssemblysByCatalog("文件支持");
            using (IComponentLoader<IOpenFileProcessor> loader = new ComponentLoader<IOpenFileProcessor>())
            {
                IOpenFileProcessor[] processors = loader.LoadComponents(dlls);
                if (processors != null)
                {
                    foreach (IOpenFileProcessor pro in processors)
                    {
                        pro.SetSession(session);
                        _fileOpenOperators.Add(pro);
                    }
                }
            }
        }

        public static void Open(string fname, params string[] options)
        {
            if (_fileOpenOperators == null || _fileOpenOperators.Count == 0 || string.IsNullOrEmpty(fname))
                return;
            if (!File.Exists(fname))
                return;
            string extName = Path.GetExtension(fname).ToUpper();
            foreach (IOpenFileProcessor opr in _fileOpenOperators)
            {
                if (opr.IsSupport(fname, extName))
                {
                    if (!CountIsAllowed(opr))
                    {
                        MsgBox.ShowInfo("已打开的影像文件个数达到最大限制数量\"" + ALLOW_MAX_RASTER_COUNT.ToString() + "\"。");
                        return;
                    }
                    bool isOK = false;
                    try
                    {
                        bool memoryIsNotEnough = false;
                        isOK = opr.Open(fname, out memoryIsNotEnough);
                        if (memoryIsNotEnough)
                            return;
                        TryFireEvents(fname);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.ShowExceptionWnd(ex, "打开文件\"" + fname + "\"时发生错误！");
                        return;
                    }
                    if (isOK)
                    {
                        AddToRecentFiles(fname);
                        //TryOpenOvervierWnd(opr);
                        WndWithOpenFile(opr);
                        return;
                    }
                    goto tryLine;
                }
            }
        tryLine:
            try
            {
                if (!TryOpenFile(fname, options))
                    ExceptionHandler.ShowExceptionWnd(null, "文件\"" + fname + "\"为不支持的格式！");
            }
            catch (Exception ex)
            {
                ExceptionHandler.ShowExceptionWnd(ex, "打开文件\"" + fname + "\"时发生错误！");
            }
        }

        private static void WndWithOpenFile(IOpenFileProcessor opr)
        {
            //by chennan 窗口上下文
            if (opr is IRasterFileProcessor)
            {
                //CmdExecute(9000);
                //CmdExecute(1004);
            }
        }

        private static bool CmdExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd != null)
            {
                cmd.Execute();
                return true;
            }
            return false;
        }

        private static bool CountIsAllowed(IOpenFileProcessor opr)
        {
            if (!(opr is IRasterFileOpenedCountable) || ALLOW_MAX_RASTER_COUNT < 1)
                return true;
            var v = _session.SmartWindowManager.GetSmartWindows((wnd) => { return wnd is ICanvasViewer; });
            if (v == null || v.Count() < ALLOW_MAX_RASTER_COUNT)
                return true;
            return false;
        }

        private static void TryFireEvents(string fname)
        {
            try
            {
                ISmartSessionEvents evts = _session as ISmartSessionEvents;
                if (evts.OnFileOpended != null)
                    evts.OnFileOpended(_session, fname);
            }
            catch
            {
            }
        }

        private static void TryOpenOvervierWnd(IOpenFileProcessor opr)
        {
            try
            {
                if (opr is IRasterFileProcessor)
                {
                    //CommandOverviewWnd
                    ICommand cmd = _session.CommandEnvironment.Get(9007);
                    if (cmd != null)
                        cmd.Execute();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("TryOpenOvervierWnd:" + ex.Message);
            }
        }

        private static void AddToRecentFiles(string fname)
        {
            _session.RecentFilesManager.AddFile(fname);
        }

        private static bool TryOpenFile(string fname, string[] options)
        {
            string hdrFile = Path.Combine(Path.GetDirectoryName(fname), Path.GetFileNameWithoutExtension(fname) + ".hdr");
            if (File.Exists(hdrFile))
            {
                IOpenFileProcessor pro = GetRasterFileProcessor();
                if (pro != null)
                {
                    bool memoryIsNotEnough = false;
                    return pro.Open(fname, out memoryIsNotEnough);
                }
            }
            return false;
        }

        private static IOpenFileProcessor GetRasterFileProcessor()
        {
            foreach (IOpenFileProcessor pro in _fileOpenOperators)
                if (pro.GetType().ToString().Contains("RasterFileProcessor"))
                    return pro;
            return null;
        }

        public static string GetTextByFileName(string fname)
        {
            if (string.IsNullOrEmpty(fname))
                return "未命名";
            try
            {
                string txt = Path.GetFileName(fname);
                return "...\\" + txt;
            }
            catch
            {
                return "未命名";
            }
        }

        public static bool SupportOpenFile(string fname)
        {
            string hdrFile = Path.Combine(Path.GetDirectoryName(fname), Path.GetFileNameWithoutExtension(fname) + ".hdr");
            if (File.Exists(hdrFile))
            {
                IOpenFileProcessor pro = GetRasterFileProcessor();
                if (pro != null)
                    return true;
            }
            return false;
        }
    }
}
