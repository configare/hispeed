using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace GeoDo.RSS.MIF.Core
{
    public class AutoGenerateExecutor : IAutoGenerateExecutor
    {
        protected IContextEnvironment _contextEnvironment;
        protected IMonitoringProduct _monitoringProduct;
        protected IResultHandler _resultHandler;
        protected IThemeGraphGenerator _layoutGenerator;
        protected IFileNameGenerator _fileNameGenerator;

        public AutoGenerateExecutor(IResultHandler resultHandler,
            IMonitoringProduct monitoringProduct,
            IThemeGraphGenerator layoutGenerator,
            IFileNameGenerator fileNameGenerator)
        {
            _resultHandler = resultHandler;
            _monitoringProduct = monitoringProduct;
            _layoutGenerator = layoutGenerator;
            _fileNameGenerator = fileNameGenerator;
            _contextEnvironment = new ContextEnvironment();
        }

        public IContextEnvironment ContextEnvironment
        {
            get { return _contextEnvironment; }
        }

        public string[] GetNeedFirstSettedArguments()
        {
            string firstSubProduct = GetFirstSubProduct();
            return GetNeedFirstSettedArguments(firstSubProduct);
        }

        public string[] GetNeedFirstSettedArguments(string beginSubProduct)
        {
            List<string> needFirstSettedArgs = new List<string>();
            List<string> initedArgs = new List<string>();
            SubProductDef firstSubProduct = _monitoringProduct.Definition.GetSubProductDefByIdentify(beginSubProduct);
            if (firstSubProduct != null && firstSubProduct.IsNeedCurrentRaster)
                needFirstSettedArgs.Add(GeoDo.RSS.MIF.Core.ContextEnvironment.ENV_VAR_NAME_CURRENT_RASTER_FILE);
            if (_monitoringProduct.SubProducts != null)
            {
                string rstFileName = _contextEnvironment.GetContextVar(GeoDo.RSS.MIF.Core.ContextEnvironment.ENV_VAR_NAME_CURRENT_RASTER_FILE);
                RasterIdentify rstIdentify = new RasterIdentify();
                if (rstFileName != null)
                {
                    rstIdentify = new RasterIdentify(rstFileName);
                    initedArgs.Add(GeoDo.RSS.MIF.Core.ContextEnvironment.ENV_VAR_NAME_CURRENT_RASTER_FILE);
                }
                foreach (IMonitoringSubProduct subprd in _monitoringProduct.SubProducts)
                {
                    AlgorithmDef alg = subprd.Definition.Algorithms[0];
                    if (subprd.Definition.IsNeedCurrentRaster && subprd.Definition.Algorithms.Count() > 0)
                    {
                        ExtractAlgorithmIdentify id = new ExtractAlgorithmIdentify();
                        id.Satellite = rstIdentify.Satellite;
                        id.Sensor = rstIdentify.Sensor;
                        alg = subprd.Definition.GetAlgorithmDefByAlgorithmIdentify(id);
                        if (alg == null)
                            alg = subprd.Definition.Algorithms[0];
                    }
                    //子产品不存在实例
                    if (subprd.Definition.SubProductInstanceDefs == null)
                    {
                        foreach (ArgumentDef arg in alg.Arguments.Where((a) => { return a is ArgumentDef; }))
                        {
                            if (arg.IsOptional)
                                continue;
                            if (arg.RefType == "file" && arg.FileProvider == null)
                            {
                                if (!needFirstSettedArgs.Contains(arg.Name))
                                    needFirstSettedArgs.Add(arg.Name);
                            }
                            else if (arg.RefType == "file")
                            {
                                if (arg.FileProvider.Contains("ContextEnvironment:"))
                                {
                                    string[] parts = arg.FileProvider.Split(':');
                                    if (!initedArgs.Contains(parts[1]) && !needFirstSettedArgs.Contains(parts[1]))
                                        needFirstSettedArgs.Add(parts[1]);
                                }
                            }
                        }
                    }
                    //子产品有实例
                    else
                    {
                        foreach (SubProductInstanceDef ist in subprd.Definition.SubProductInstanceDefs)
                        {
                            if (ist.FileProvider.Contains("ContextEnvironment:"))
                            {
                                string[] parts = ist.FileProvider.Split(':');
                                if (!initedArgs.Contains(parts[1]) && !needFirstSettedArgs.Contains(parts[1]))
                                    needFirstSettedArgs.Add(parts[1]);
                            }
                        }
                    }
                    //
                    initedArgs.Add(subprd.Identify);
                }
            }
            return needFirstSettedArgs.Count > 0 ? needFirstSettedArgs.ToArray() : null;
        }

        public void Execute(IContextMessage contextMessage, IArgumentMissProcessor argumentMissProcessor,string executeGroup,Action<int, string> processTracker)
        {
            string firstSubProduct = GetFirstSubProduct();
            Execute(firstSubProduct, contextMessage, argumentMissProcessor,executeGroup, processTracker);
        }

        public void Execute(string beginSubProduct, IContextMessage contextMessage, IArgumentMissProcessor argumentMissProcessor,string executeGroup, Action<int, string> processTracker)
        {
            try
            {
                SubProductDef firstSubProduct = _monitoringProduct.Definition.GetSubProductDefByIdentify(beginSubProduct);
                List<IMonitoringSubProduct> waitingSubProducts = new List<IMonitoringSubProduct>();
                bool isBefore = true;
                if (string.IsNullOrEmpty(executeGroup))
                {
                    foreach (IMonitoringSubProduct subprd in _monitoringProduct.SubProducts)
                    {
                        if (firstSubProduct.Identify != subprd.Definition.Identify && isBefore)
                            continue;
                        if (!subprd.Definition.IsAutoGenerate)
                            continue;
                        isBefore = false;
                        waitingSubProducts.Add(subprd);
                    }
                    Execute(waitingSubProducts, contextMessage, argumentMissProcessor,null, processTracker);
                }
                else
                {
                    foreach (IMonitoringSubProduct subprd in _monitoringProduct.SubProducts)
                    {
                        if (firstSubProduct.Identify != subprd.Definition.Identify && isBefore)
                            continue;
                        if (!subprd.Definition.IsAutoGenerate)
                            continue;
                        if (subprd.Definition.AutoGenerateGroup != null)
                        {
                            if (!subprd.Definition.AutoGenerateGroup.Contains(executeGroup))
                                continue;
                        }
                        isBefore = false;
                        waitingSubProducts.Add(subprd);
                    }
                    Execute(waitingSubProducts, contextMessage, argumentMissProcessor,executeGroup, processTracker);
                }
            }
            catch (Exception ex)
            {
                if (contextMessage != null)
                    contextMessage.PrintMessage(ex.Message);
            }
        }

        private void Execute(List<IMonitoringSubProduct> waitingSubProducts, IContextMessage contextMessage, IArgumentMissProcessor argumentMissProcessor,string executeGroup, Action<int, string> processTracker)
        {
            if (waitingSubProducts == null || waitingSubProducts.Count == 0)
            {
                if (contextMessage != null)
                    contextMessage.PrintMessage("待批量生成的子产品(定量产品\\专题图产品\\统计分析表格等)为空,批量生成进程退出。");
                return;
            }
            int step = 100 / waitingSubProducts.Count;
            int i = 0;
            foreach (IMonitoringSubProduct subprd in waitingSubProducts)
            {
                if (processTracker != null)
                    processTracker((i++) * step, "正在生成\"" + subprd.Name + "\"......");
                if (contextMessage != null)
                    contextMessage.PrintMessage("开始生成\"" + subprd.Name + "\"......");
                MakeSubProduct("    ", subprd, contextMessage, argumentMissProcessor,executeGroup, processTracker);
                if (contextMessage != null)
                    contextMessage.PrintMessage("\"" + subprd.Name + "\"生成完成。");
            }
        }

        private void MakeSubProduct(string preBanks, IMonitoringSubProduct subprd, IContextMessage contextMessage, IArgumentMissProcessor argumentMissProcessor, string executeGroup, Action<int, string> processTracker)
        {
            bool isCreatDataProvider = false;
            IRasterDataProvider raster = null;
            try
            {
                subprd.ArgumentProvider.Reset();
                subprd.ArgumentProvider.AOI = null;
                AlgorithmDef alg = subprd.AlgorithmDefs[0];
                RasterIdentify rstIdentify = null;
                rstIdentify = GetRasterIdentifOfCurrentFile();
                if (subprd.Definition.IsNeedCurrentRaster)
                {
                    string rstFileName = _contextEnvironment.GetContextVar(GeoDo.RSS.MIF.Core.ContextEnvironment.ENV_VAR_NAME_CURRENT_RASTER_FILE);
                    raster = GeoDataDriver.Open(rstFileName) as IRasterDataProvider;
                    subprd.ArgumentProvider.DataProvider = raster;
                    isCreatDataProvider = true;
                    //
                    TryApplyAOITemplates(subprd);
                    //
                    if (rstIdentify == null)
                    {
                        if (contextMessage != null)
                            contextMessage.PrintMessage("参数\"" + GeoDo.RSS.MIF.Core.ContextEnvironment.ENV_VAR_NAME_CURRENT_RASTER_FILE + "\"为空！");
                        if (argumentMissProcessor == null)
                            return;
                        string crtFileName = argumentMissProcessor.DoGettingArgument(subprd.Definition, subprd.UseDefaultAlgorithm(subprd.Definition.ProductDef.Identify), GeoDo.RSS.MIF.Core.ContextEnvironment.ENV_VAR_NAME_CURRENT_RASTER_FILE);
                        if (crtFileName == null)
                            return;
                        rstIdentify = new RasterIdentify(crtFileName);
                        _contextEnvironment.PutContextVar(GeoDo.RSS.MIF.Core.ContextEnvironment.ENV_VAR_NAME_CURRENT_RASTER_FILE, crtFileName);
                    }
                    ExtractAlgorithmIdentify id = new ExtractAlgorithmIdentify();
                    id.Satellite = rstIdentify.Satellite;
                    id.Sensor = rstIdentify.Sensor;
                    alg = subprd.Definition.GetAlgorithmDefByAlgorithmIdentify(id);
                    if (alg == null)
                        alg = subprd.Definition.Algorithms[0];
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
                            if (contextMessage != null)
                                contextMessage.PrintMessage(preBanks + "从波段映射表获取\"" + band.Identify + "\"的波段序号失败,生成过程终止！");
                            return;
                        }
                    }
                }
                //
                TryHandleCustomArguments(subprd.ArgumentProvider, alg);
                //
                if (subprd.Definition.SubProductInstanceDefs != null)
                    MakeSubProductUseInstances(preBanks + "  ", subprd, alg, contextMessage, argumentMissProcessor, executeGroup, processTracker);
                else
                    DirectMakeSubProduct(preBanks + "  ", subprd, alg, contextMessage, argumentMissProcessor, processTracker);
            }
            finally
            {
                if (raster != null)
                {
                    raster.Dispose();
                    raster = null;
                }
                if (subprd.ArgumentProvider.DataProvider != null && isCreatDataProvider)
                {
                    subprd.ArgumentProvider.DataProvider.Dispose();
                }
            }
        }

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
                object v = null;
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
                IArgumentEditorUI editor = v as IArgumentEditorUI;
                IArgumentEditorUI2 editor2 = v as IArgumentEditorUI2;
                if (editor == null&&editor2==null)
                    return null;
                if (editor != null)
                    return editor.ParseArgumentValue(xElement);
                else
                    return editor2.ParseArgumentValue(xElement);
            }
            catch
            {
                return null;
            }
        }

        private void TryApplyAOITemplateBySeconaryArg(IMonitoringSubProduct subprd)
        {
            if (string.IsNullOrEmpty(subprd.Definition.AOISecondaryInfoFromArg))
                return;
            string seconaryFile = subprd.ArgumentProvider.GetArg(subprd.Definition.AOISecondaryInfoFromArg).ToString();
            using (IRasterDataProvider prd = GeoDataDriver.Open(seconaryFile) as IRasterDataProvider)
            {
                if (prd != null)
                    ApplyAOI(prd, subprd);
            }
        }

        private void TryApplyAOITemplates(IMonitoringSubProduct subprd)
        {
            ApplyAOI(subprd.ArgumentProvider.DataProvider, subprd);
        }

        private void ApplyAOI(IRasterDataProvider prd, IMonitoringSubProduct subprd)
        {
            if (!subprd.Definition.IsUseAoiTemplate || prd == null)
                return;
            string aoistring = subprd.Definition.AoiTemplates;
            if (string.IsNullOrEmpty(aoistring))
                return;
            string[] aoiNames = aoistring.Split(',');
            CoordEnvelope evp = prd.CoordEnvelope;
            Size size = new Size(prd.Width, prd.Height);

            int[] retAOI = null;
            foreach (string aoiName in aoiNames)
            {
                int[] aoi = AOITemplateFactory.MakeAOI(aoiName, evp.MinX, evp.MaxX, evp.MinY, evp.MaxY, size);
                if (retAOI == null)
                    retAOI = aoi;
                else
                    retAOI = AOIHelper.Merge(new int[][] { retAOI, aoi });
            }
            subprd.ArgumentProvider.AOI = retAOI;
        }

        private void DirectMakeSubProduct(string preBanks, IMonitoringSubProduct subprd, AlgorithmDef alg, IContextMessage contextMessage, IArgumentMissProcessor argumentMissProcessor, Action<int, string> processTracker)
        {
            IArgumentProvider prd = subprd.ArgumentProvider;
            prd.SetArg("ThemeGraphyGenerator", _layoutGenerator);
            prd.SetArg("FileNameGenerator", _fileNameGenerator);
            foreach (ArgumentDef arg in alg.Arguments.Where((a) => { return a is ArgumentDef; }))
            {
                if (arg.IsOptional)
                    continue;
                if (arg.RefType == "file" && arg.FileProvider == null)
                {
                    if (contextMessage != null)
                        contextMessage.PrintMessage(preBanks + "参数\"" + arg.Name + "\"为空,子产品\"" + subprd.Name + "\"过程终止！");
                    return;
                }
                else if (arg.RefType == "file")
                {
                    GetAndSetFileVar(preBanks + " ", subprd, arg, contextMessage);
                }
            }
            //根据某个文件型参数生成AOI
            TryApplyAOITemplateBySeconaryArg(subprd);
            //
            IExtractResult result = subprd.Make(processTracker);

            if (result != null)
                if (_resultHandler != null)
                    _resultHandler.HandleResult(_contextEnvironment, _monitoringProduct, subprd, result);
        }

        private bool GetAndSetFileVar(string preBanks, IMonitoringSubProduct subprd, ArgumentDef arg, IContextMessage contextMessage)
        {
            if (arg.FileProvider.Contains("ContextEnvironment:"))
            {
                string[] parts = arg.FileProvider.Split(':');
                string v = _contextEnvironment.GetContextVar(parts[1]);
                if (v == null)
                {
                    if (contextMessage != null)
                        contextMessage.PrintMessage(preBanks + "参数\"" + arg.Name + "\"为空,子产品\"" + subprd.Name + "\"过程终止！");
                    return false;
                }
                _contextEnvironment.PutContextVar(parts[1], v);
                _contextEnvironment.PutContextVar(arg.Name, v);
                subprd.ArgumentProvider.SetArg(arg.Name, v);
            }
            return true;
        }

        private void MakeSubProductUseInstances(string preBanks, IMonitoringSubProduct subprd, AlgorithmDef alg, IContextMessage contextMessage, IArgumentMissProcessor argumentMissProcessor,string executeGroup, Action<int, string> processTracker)
        {
            IArgumentProvider prd = subprd.ArgumentProvider;
            prd.SetArg("ThemeGraphyGenerator", _layoutGenerator);
            prd.SetArg("FileNameGenerator", _fileNameGenerator);
            foreach (SubProductInstanceDef ist in subprd.Definition.SubProductInstanceDefs)
            {
                if (!ist.isautogenerate)
                    continue;
                if (!string.IsNullOrEmpty(executeGroup)&&ist.AutoGenerateGroup!=null&&!ist.AutoGenerateGroup.Contains(executeGroup))
                    continue;
                ResetFileVars(prd, alg);
                if (ist.FileProvider == null)
                {
                    if (contextMessage != null)
                        contextMessage.PrintMessage(preBanks + "参数\"FileProvider\"为空,子产品\"" + ist.Name + "\"过程终止！");
                    continue;
                }
                if (ist.AOIProvider != null)
                    SetAOI(ist.AOIProvider, prd);
                if (ist.FileProvider != null)
                    if (!SetPrimaryFiles(preBanks, ist.FileProvider, ist.Argument, ref ist.extInfo, prd, contextMessage))
                        continue;
                if (ist.OutFileIdentify != null)
                    prd.SetArg("OutFileIdentify", ist.OutFileIdentify);
                if (ist.LayoutName != null)
                    prd.SetArg("ThemeGraphTemplateName", ist.LayoutName);
                if (!string.IsNullOrEmpty(ist.ColorTableName))
                    prd.SetArg("colortablename", "colortablename=" + ist.ColorTableName);
                if (!string.IsNullOrEmpty(ist.extInfo))
                    prd.SetArg("extinfo", ist.extInfo);

                //多通道合成图需要使用Canverviewer中的数据。包括文件名，波段，图像增强方案等。
                prd.SetArg("SmartSession", _contextEnvironment.Session);

                //根据某个文件型参数生成AOI
                TryApplyAOITemplateBySeconaryArg(subprd);
                //
                IExtractResult result = subprd.Make(processTracker);
                if (result != null)
                    if (_resultHandler != null)
                        _resultHandler.HandleResult(_contextEnvironment, _monitoringProduct, subprd, result);
            }
        }

        private void ResetFileVars(IArgumentProvider prd, AlgorithmDef alg)
        {
            foreach (ArgumentDef arg in alg.Arguments.Where((a) => { return a is ArgumentDef; }))
                if (arg.RefType == "file")
                    prd.SetArg(arg.Name, null);
        }

        private bool SetPrimaryFiles(string preBanks, string fileProvider, string argument,ref string extinfo, IArgumentProvider prd, IContextMessage contextMessage)
        {
            string[] parts = fileProvider.Split(':');
            if (fileProvider.Contains("ContextEnvironment:"))
            {
                string v = _contextEnvironment.GetContextVar(parts[1]);
                if (v == null)
                {
                    if (contextMessage != null)
                        contextMessage.PrintMessage(preBanks + "参数\"SelectedPrimaryFiles\"为空,生成过程终止！");
                    return false;
                }
                _contextEnvironment.PutContextVar(parts[1], v);
                prd.SetArg("SelectedPrimaryFiles", v);
            }
            else if (fileProvider.ToUpper().Contains(".DLL,"))
            {
                parts = fileProvider.Split(',');
              
                string assembly = parts[0];
                string cls = parts[1];
                object obj = null;
                Assembly ass = TryGetExistedAssembly(assembly);
                if (ass != null)
                    obj = ass.CreateInstance(cls);
                else
                {
                    if (GetType().Assembly.Location.Contains(assembly))
                        obj = GetType().Assembly.CreateInstance(cls);
                    else
                    {
                        ass = TryGetExistedAssembly(assembly);
                        if (ass != null)
                            obj = ass.CreateInstance(cls);
                        else
                            obj = Activator.CreateInstance(assembly, cls);
                    }
                }
                if (obj != null)
                {
                    IFileFinder finder = obj as IFileFinder;
                    if (finder != null)
                    {
                        string fname = _contextEnvironment.GetContextVar(GeoDo.RSS.MIF.Core.ContextEnvironment.ENV_VAR_NAME_CURRENT_RASTER_FILE);
                        string[] files = finder.Find(fname,ref extinfo, argument);
                        if (files != null)
                            prd.SetArg("SelectedPrimaryFiles", files);
                    }
                }
            }
            return true;
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

        private void SetAOI(string aoiProvider, IArgumentProvider prd)
        {
            if (string.IsNullOrEmpty(aoiProvider))
                return;
            if (aoiProvider.Contains("SystemAOI:"))
            { }
            else
            {
                prd.SetArg("AOI", aoiProvider);
            }
        }

        private RasterIdentify GetRasterIdentifOfCurrentFile()
        {
            string rstFileName = _contextEnvironment.GetContextVar(GeoDo.RSS.MIF.Core.ContextEnvironment.ENV_VAR_NAME_CURRENT_RASTER_FILE);
            RasterIdentify rstIdentify = null;
            if (rstFileName != null)
                rstIdentify = new RasterIdentify(rstFileName);
            return rstIdentify;
        }

        private string GetFirstSubProduct()
        {
            return _monitoringProduct.SubProducts[0].Identify;
        }
    }
}
