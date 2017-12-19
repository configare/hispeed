using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using GeoDo.RSS.Core.RasterDrawing;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public class AutoImportData
    {
        private ISmartSession _smartSession = null;
        private IMonitoringSession _monitoringSession = null;
        private string _defaultImpotFileDir;
        private string _argsFileDir = AppDomain.CurrentDomain.BaseDirectory + "\\SystemData\\DataImportXML\\";
        private string _defaultSubPro;
        private bool _defaultManual = false;
        private string _argsFile;

        public AutoImportData(ISmartSession smartSession)
        {
            _smartSession = smartSession;
            _monitoringSession = _smartSession.MonitoringSession == null ? null : _smartSession.MonitoringSession as MonitoringSession;
        }

        public string Do()
        {
            if (_monitoringSession == null)
                return "未定义待导入哪个产品!";
            IRasterDataProvider dataProvider = GetRasterDataProvider();
            if (dataProvider == null)
                return "未选择待导入影像数据!";
            GetDefaultInfo();
            if (_monitoringSession.ActiveMonitoringSubProduct == null)
            {
                if (!string.IsNullOrEmpty(_defaultSubPro))
                    ChangedSubPro();
                else
                    return "未定义待导入哪个子产品!";
            }
            ImportFilesObj[] objs = GetImportFileObjByForm(dataProvider);
            if (objs == null)
            {
                if (_defaultManual)
                {
                    ExecuteCommd(6634, _argsFile);
                    return string.Empty;
                }
                else
                    return "无可导入数据!";
            }
            ApplayExtractResult(dataProvider, objs);
            return string.Empty;
        }

        private void ChangedSubPro()
        {
            _monitoringSession.ChangeActiveSubProduct(_defaultSubPro);
            ExecuteCommd(6602, string.Empty);
        }

        private void ExecuteCommd(int commdID, string arg)
        {
            ICommand commd = _smartSession.CommandEnvironment.Get(commdID);
            if (commd != null)
            {
                if (string.IsNullOrEmpty(arg))
                    commd.Execute();
                else
                    commd.Execute(arg);
            }

        }

        private void ApplayExtractResult(IRasterDataProvider dataProvider, ImportFilesObj[] objs)
        {
            IDataImportDriver driver = null;
            string error = null;
            foreach (ImportFilesObj obj in objs)
            {
                if (string.IsNullOrEmpty(obj.ProIdentify) || string.IsNullOrEmpty(obj.SubProIdentify))
                {
                    obj.ProIdentify = _monitoringSession.ActiveMonitoringProduct.Identify;
                    obj.SubProIdentify = _monitoringSession.ActiveMonitoringSubProduct.Identify;
                }
                driver = DataImport.GetDriver(obj.ProIdentify, obj.SubProIdentify, obj.FullFilename, null);
                if (driver == null)
                    continue;
                IExtractResult result = driver.Do(obj.ProIdentify, obj.SubProIdentify, dataProvider, obj.FullFilename, out error);
                if (result == null)
                    continue;
                DisplayResultClass.DisplayResult(_smartSession, _monitoringSession.ActiveMonitoringSubProduct, result, true);
            }
        }

        private ImportFilesObj[] GetImportFileObjByForm(IRasterDataProvider dataProvider)
        {
            ImportFilesObj[] objs = null;
            objs = AutoFindFile(dataProvider);
            return objs;
        }

        private void GetDefaultInfo()
        {
            _defaultImpotFileDir = string.Empty;
            _defaultSubPro = string.Empty;
            _defaultManual = false;
            if (!Directory.Exists(_argsFileDir))
                return;
            _argsFile = Path.Combine(_argsFileDir, _monitoringSession.ActiveMonitoringProduct.Identify + ".txt");
            if (!File.Exists(_argsFile))
                return;
            string[] contexts = File.ReadAllLines(_argsFile, Encoding.Default);
            if (contexts == null || contexts.Length == 0 || contexts.Length > 3 || !Directory.Exists(contexts[0]))
                return;
            _defaultImpotFileDir = contexts[0];
            if (contexts.Length > 1)
                _defaultSubPro = contexts[1];
            if (contexts.Length > 2)
                _defaultManual = bool.Parse(contexts[2]);
        }

        private ImportFilesObj[] AutoFindFile(IRasterDataProvider dataProvider)
        {
            return DataImport.AutoFindFiles(_monitoringSession.ActiveMonitoringProduct.Identify,
                _monitoringSession.ActiveMonitoringSubProduct.Identify, dataProvider, _defaultImpotFileDir, null);
        }

        private IRasterDataProvider GetRasterDataProvider()
        {
            if (_smartSession.SmartWindowManager == null)
                return null;
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null || drawing.DataProvider == null)
                return null;
            return drawing.DataProvider;
        }
    }
}
