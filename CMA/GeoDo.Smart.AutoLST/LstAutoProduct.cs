using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.UI.WinForm;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace GeoDo.Smart.AutoLST
{
    public class LstAutoProduct
    {
        private ISmartSession _session;
        private IContextMessage _context;
        private string _rstFileName;
        private string _ndvi;
        private string _outfile;

        public LstAutoProduct()
        {
            frmMainForm mainForm = new frmMainForm(new StartProgress());
            _session = mainForm.Session;
            _context = new ContentText();
        }

        public void Make(string[] args)
        {
            PrintInfo("启动LST计算");
            try
            {
                LstArgs arg = new LstArgs(args);
                if (arg == null)
                    PrintInfo("输入参数为空，计算终止");
                _rstFileName = arg.LdfFile;
                _ndvi = arg.NDVIFile;
                _outfile = arg.OutFile;
                if (string.IsNullOrWhiteSpace(_rstFileName))
                {
                    PrintInfo("输入ldf局地文件为空，计算终止");
                    return;
                }
                if (string.IsNullOrWhiteSpace(_outfile))
                {
                    PrintInfo("输出文件设置为空，计算终止");
                    return;
                }
                IMonitoringSubProduct subprd = CreateMonitoringSubProduct(_rstFileName, "LST", "DBLV");
                if (subprd == null)
                {
                    PrintInfo("激活或生成子产品失败，计算终止");
                    return;
                }
                //下面需要设置文件来源参数(控制台程序中获得的参数)
                SetConsoleArgs(subprd);
                IExtractResult result = subprd.Make(null, _context);
                string resultFile = null;
                if (result is ExtractResultArray)
                    resultFile = ((result as ExtractResultArray).PixelMappers[0] as IFileExtractResult).FileName;
                //搬运文件，或者裁切文件。
                TryCutFile(resultFile, _outfile);
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
            }
            finally
            {
                PrintInfo("结束LST计算");
            }
        }

        private void SetConsoleArgs(IMonitoringSubProduct subprd)
        {
            subprd.ArgumentProvider.DataProvider = null;
            Dictionary<string, string[]> pathDic = new Dictionary<string, string[]>();
            pathDic.Add("FileNames", new string[] { _rstFileName });
            subprd.ArgumentProvider.SetArg("OrbitFileSelectType", pathDic);
            subprd.ArgumentProvider.SetArg("NDVIFile", _ndvi);
            subprd.ArgumentProvider.SetArg("OutFile", _outfile);
        }

        private void TryCutFile(string resultFile, string outfile)
        {
            if (resultFile != outfile)
            {
                if (File.Exists(resultFile))
                {
                    File.Copy(resultFile, outfile);
                    TryDeleteFile(resultFile);
                    string hdr = Path.ChangeExtension(resultFile, ".hdr");
                    string dsthdr = Path.ChangeExtension(outfile, ".hdr");
                    if (File.Exists(hdr))
                    {
                        File.Copy(hdr, dsthdr);
                        TryDeleteFile(hdr);
                    }
                }
            }
        }

        private void TryDeleteFile(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch(Exception ex)
            {
                PrintInfo(ex.Message);
            }
        }

        private IMonitoringSubProduct CreateMonitoringSubProduct(string rstFileName, string productIdentify, string subProductIdentify)
        {
            MonitoringSession session = new MonitoringSession(_session);
            IMonitoringProduct monitorProduct = session.ChangeActiveProduct(productIdentify);
            IMonitoringSubProduct subprd = session.ChangeActiveSubProduct(subProductIdentify);
            RasterIdentify rstIdentify = new RasterIdentify(rstFileName);
            ExtractAlgorithmIdentify id = new ExtractAlgorithmIdentify();
            id.Satellite = rstIdentify.Satellite;
            id.Sensor = rstIdentify.Sensor;
            AlgorithmDef alg = subprd.Definition.GetAlgorithmDefByAlgorithmIdentify(id);
            if (alg == null)
            {
                PrintInfo("没有匹配的算法：" + "卫星" + rstIdentify.Satellite + "，传感器" + rstIdentify.Sensor);
                return null;
            }
            subprd.ResetArgumentProvider(alg.Identify);
            subprd.ArgumentProvider.SetArg("AlgorithmName", alg.Identify);
            if (alg.Bands != null && alg.Bands.Length > 0)
            {
                MonitoringThemeFactory.SetBandArgs(subprd, rstIdentify.Satellite, rstIdentify.Sensor);
                foreach (BandDef band in alg.Bands)
                {
                    if (subprd.ArgumentProvider.GetArg(band.Identify).ToString() == "-1")
                    {
                        PrintInfo("从波段映射表获取\"" + band.Identify + "\"的波段序号失败,生成过程终止！");
                        return null;
                    }
                }
            }
            TryHandleCustomArguments(subprd.ArgumentProvider, alg);
            return subprd;
        }

        #region 用户自定义类型IArgumentEditorUI参数设置
        private void TryHandleCustomArguments(IArgumentProvider argProvider, AlgorithmDef alg)
        {
            foreach (ArgumentDef arg in alg.Arguments.Where((a) => { return a is ArgumentDef; }))
            {
                string uieditor = arg.EditorUiProvider;
                if (string.IsNullOrEmpty(uieditor))
                    continue;
                var xElement = arg.DefaultValueElement;
                if (xElement == null)
                    continue;
                if (!xElement.HasElements)
                    continue;
                object v = TryGetValueUseUIEditor(uieditor, xElement);
                if (v != null)
                    argProvider.SetArg(arg.Name, v);
            }
        }

        private object TryGetValueUseUIEditor(string uieditor, System.Xml.Linq.XElement xElement)
        {
            try
            {
                string[] parts = uieditor.Split(':');
                dynamic v = null;
                if (GetType().Assembly.Location.Contains(parts[0]))
                    v = GetType().Assembly.CreateInstance(parts[1]);
                else
                {
                    Assembly ass = TryGetExistedAssembly(parts[0]);
                    if (ass != null)
                        v = ass.CreateInstance(parts[1]);
                    else
                        v = Activator.CreateInstance(parts[0], parts[1]);
                }
                if (v == null)
                    return null;
                if (v is IArgumentEditorUI)
                {
                    return (v as IArgumentEditorUI).ParseArgumentValue(xElement);
                }
                else if (v is IArgumentEditorUI2)
                {
                    (v as IArgumentEditorUI2).ParseArgumentValue(xElement);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private Assembly TryGetExistedAssembly(string assembly)
        {
            Assembly[] asses = AppDomain.CurrentDomain.GetAssemblies();
            if (asses == null || asses.Length == 0)
                return null;
            foreach (Assembly ass in asses)
            {
                try
                {
                    if (ass.Location.Contains(assembly))
                        return ass;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
            }
            return null;
        }
        #endregion

        private void PrintInfo(string message)
        {
            if (_context != null)
                _context.PrintMessage(message);
        }

    }

    public class ContentText : IContextMessage
    {
        public ContentText()
        { }

        public void PrintMessage(string message)
        {
            //...记录处理过程.
            Console.WriteLine(message);
        }
    }

    //系统启动进度
    public class StartProgress : IStartProgress
    {
        public void PrintStartInfo(string sInfo)
        {
            Console.WriteLine(sInfo);
        }

        public void Stop()
        {
        }
    }

}
