using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using System.Drawing;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.UI.AddIn.DataPro;
using GeoDo.FileProject;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public class MIFCommAnalysis
    {
        public static IExtractResult StatAreaNotBaseStatSubprd(ISmartSession session, string outFileIdentify, string subProductIdentify, string AlgorithmName, string aoiTemplate, bool isCustom, bool multiSelect)
        {
            return StatAnaylsisBase(session, outFileIdentify, aoiTemplate, subProductIdentify, AlgorithmName, isCustom, multiSelect);
        }

        public static IExtractResult AreaStat(ISmartSession session, string outFileIdentify, string aoiTemplate, bool isCustom, bool multiSelect)
        {
            return StatAnaylsisBase(session, outFileIdentify, aoiTemplate, "STAT", "STATAlgorithm", isCustom, multiSelect);
        }

        public static IExtractResult AreaStat(ISmartSession session, string outFileIdentify, string aoiTemplate, bool isCustom, bool multiSelect,Dictionary<string,string> fileNameList)
        {
            return StatAnaylsisBase(session, outFileIdentify, aoiTemplate, "STAT", "STATAlgorithm", isCustom, multiSelect,fileNameList);
        }

        public static IExtractResult TimeStat(ISmartSession session, string outFileIdentify, string colorTableName, string dataIdentify, string templateName, bool isCustom, bool multiSelect)
        {
            (session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FREQ");
            GetCommandAndExecute(session, 6602);
            return CreateThemeGraphyBase(session, "FREQAlgorithm", outFileIdentify, colorTableName, dataIdentify, templateName, isCustom, multiSelect);
        }

        public static IExtractResult CycleTimeStat(ISmartSession session, string outFileIdentify, string colorTableName, string dataIdentify, string templateName, bool isCustom, bool multiSelect)
        {
            (session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("CYCI");
            GetCommandAndExecute(session, 6602);
            return CreateThemeGraphyBase(session, "CYCIAlgorithm", outFileIdentify, colorTableName, dataIdentify, templateName, isCustom, multiSelect);
        }

        /// <summary>
        /// 二值图
        /// </summary>
        /// <param name="session"></param>
        /// <param name="outFileIdentify"></param>
        /// <param name="colorTableName"></param>
        /// <param name="dataIdentify"></param>
        /// <param name="templateName"></param>
        /// <param name="isCustom"></param>
        /// <param name="multiSelect"></param>
        /// <returns></returns>
        public static IExtractResult CreateThemeGraphy(ISmartSession session, string outFileIdentify, string colorTableName, string dataIdentify, string templateName, bool isCustom, bool multiSelect)
        {
            (session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0IMG");
            GetCommandAndExecute(session, 6602);
            return CreateThemeGraphyBase(session, "0IMGAlgorithm", outFileIdentify, colorTableName, dataIdentify, templateName, isCustom, multiSelect);
        }
        /// <summary>
        /// 二值图
        /// </summary>
        /// <param name="session"></param>
        /// <param name="outFileIdentify"></param>
        /// <param name="colorTableName"></param>
        /// <param name="dataIdentify"></param>
        /// <param name="templateName"></param>
        /// <param name="isCustom"></param>
        /// <param name="multiSelect"></param>
        /// <param name="isUseRegion">
        /// 应用分区范围，
        /// 如积雪业务中，根据所选文件提取分区编号，自动匹配输出范围
        /// </param>
        /// <returns></returns>
        public static IExtractResult CreateThemeGraphy(ISmartSession session, string outFileIdentify, string colorTableName, string dataIdentify, string templateName, bool isCustom, bool multiSelect, bool isUseRegion)
        {
            (session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0IMG");
            GetCommandAndExecute(session, 6602);
            return CreateThemeGraphyBase(session, "0IMGAlgorithm", outFileIdentify, colorTableName, dataIdentify, templateName, isCustom, multiSelect, isUseRegion);
        }

        public static IExtractResult CreateThemeGraphy(ISmartSession session, string outFileIdentify, string colorTableName, string subProductId, string algorithmName, string dataIdentify, string templateName, bool isCustom, bool multiSelect)
        {
            (session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct(subProductId);
            GetCommandAndExecute(session, 6602);
            return CreateThemeGraphyBase(session, algorithmName, outFileIdentify, colorTableName, dataIdentify, templateName, isCustom, multiSelect);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="outFileIdentify"></param>
        /// <param name="colorTableName"></param>
        /// <param name="subProductId"></param>
        /// <param name="algorithmName"></param>
        /// <param name="dataIdentify"></param>
        /// <param name="templateName"></param>
        /// <param name="isCustom"></param>
        /// <param name="multiSelect"></param>
        /// <param name="isUseRegion">
        /// 应用分区范围，
        /// 如积雪业务中，根据所选文件提取分区编号，自动匹配输出范围
        /// </param>
        /// <returns></returns>
        public static IExtractResult CreateThemeGraphy(ISmartSession session, string outFileIdentify, string colorTableName, string subProductId, string algorithmName, string dataIdentify, string templateName, bool isCustom, bool multiSelect,bool isUseRegion)
        {
            (session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct(subProductId);
            GetCommandAndExecute(session, 6602);
            return CreateThemeGraphyBase(session, algorithmName, outFileIdentify, colorTableName, dataIdentify, templateName, isCustom, multiSelect,isUseRegion);
        }

        /// <summary>
        /// 根据所选的文件提取产品标识和子产品标识，自动匹配应该生成的专题图类型，如ndvi
        /// </summary>
        /// <param name="session"></param>
        /// <param name="isCustom"></param>
        /// <param name="multiSelect"></param>
        /// <returns></returns>
        public static IExtractResult CreateThemeGraphy(ISmartSession session, bool isCustom, bool multiSelect)
        {
            (session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0IMG");
            GetCommandAndExecute(session, 6602);
            return CreateThemeGraphyBase(session, "0IMGAlgorithm", isCustom, multiSelect);
        }

        /// <summary>
        /// 根据所选的文件提取产品标识和子产品标识，自动匹配应该生成的专题图类型，如ndvi
        /// </summary>
        /// <param name="session"></param>
        /// <param name="isCustom"></param>
        /// <param name="multiSelect"></param>
        /// <returns></returns>
        private static IExtractResult CreateThemeGraphyBase(ISmartSession session, string algorithmName, bool isCustom, bool multiSelect)
        {
            IMonitoringSubProduct msp = (session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            if (msp == null)
                return null;
            object obj = msp.ArgumentProvider.GetArg("SelectedPrimaryFiles");
            //从工作空间中获取选择的文件名
            if (!SetSelectedPrimaryFiles(session, ref obj, msp.ArgumentProvider, algorithmName))
                return null;
            string[] files = obj as string[];
            if (files == null || files.Length == 0)
            {
                if (File.Exists(obj.ToString()))
                    files = new string[] { obj.ToString() };
                else
                    return null;
            }
            //添加对生成专题图文件类型的判断，排除生成的专题图和统计结果文件,add by wangyu,20120923
            foreach (string fname in files)
                CanCreatThemegraph(fname);
            string outFileIdentify = null;
            string templateName = null;
            GetTemplateName(msp.Definition.SubProductInstanceDefs, files, multiSelect, out outFileIdentify, out templateName);
            if (string.IsNullOrEmpty(outFileIdentify) || string.IsNullOrEmpty(templateName))
                return null;

            if (isCustom && !SetAOIArugment(files, msp, multiSelect))
                return null;
            else if (!isCustom)
                msp.ArgumentProvider.SetArg("AOI", null);
            IThemeGraphGenerator tgg = (session.MonitoringSession as IMonitoringSession).ThemeGraphGenerator;
            msp.ArgumentProvider.SetArg("ThemeGraphyGenerator", tgg);
            msp.ArgumentProvider.SetArg("OutFileIdentify", outFileIdentify);
            msp.ArgumentProvider.SetArg("ThemeGraphTemplateName", templateName);
            IExtractResult result = msp.Make(null);
            DisplayResultClass.DisplayResult(session, msp, result, false);
            return result;
        }

        //根据所选的二值文件的文件名去查找匹配的专题图模版
        private static void GetTemplateName(SubProductInstanceDef[] instanceDefs, string[] files, bool multiSelect, out string outFileIdentify, out string templateName)
        {
            outFileIdentify = string.Empty;
            templateName = string.Empty;
            RasterIdentify rid = new RasterIdentify(files[0]);
            string identify = rid.SubProductIdentify;
            if (multiSelect)
            {
                for (int i = 1; i < files.Length; i++)
                {
                    RasterIdentify ri = new RasterIdentify(files[i]);
                    if (ri.SubProductIdentify == identify)
                        continue;
                    else //如果用户选择了多个文件并且文件标识不相等，则弹出对话框让用户自己选择专题图类型
                        GetTemplateNameByUserControl(instanceDefs, out outFileIdentify, out templateName);
                }
            }
            if (string.IsNullOrEmpty(identify)) // 如果文件名中无法提取子产品标识，则弹出对话框让用户自己选择专题图类型
                GetTemplateNameByUserControl(instanceDefs, out outFileIdentify, out templateName);

            //从配置文件中查找专题图模版名以及输出文件标识
            MatchTemplateNameByFileName(instanceDefs, identify, out outFileIdentify, out templateName);
            if (string.IsNullOrEmpty(outFileIdentify) || string.IsNullOrEmpty(templateName))
                GetTemplateNameByUserControl(instanceDefs, out outFileIdentify, out templateName);
        }

        private static void MatchTemplateNameByFileName(SubProductInstanceDef[] instanceDefs, string identify, out string outFileIdentify, out string templateName)
        {
            outFileIdentify = string.Empty;
            templateName = string.Empty;

            foreach (SubProductInstanceDef instance in instanceDefs)
            {
                if (instance == null || string.IsNullOrEmpty(instance.FileProvider))
                    continue;
                //step1:parse FileProvider,eg:ContextEnvironment:NDVI
                string fileProviderIdentify = ParseFileProvider(instance.FileProvider);
                if (string.IsNullOrEmpty(fileProviderIdentify))
                    continue;
                //step2:match one in FileProviderIdentifies[]
                if (!string.Equals(identify, fileProviderIdentify))
                    continue;
                outFileIdentify = instance.OutFileIdentify;
                templateName = instance.LayoutName;
                break;
            }
        }

        //fileProvider:ContextEnvironment:NDVI
        private static string ParseFileProvider(string fileProvider)
        {
            if (string.IsNullOrEmpty(fileProvider))
                return string.Empty;
            string[] parts = fileProvider.Split(':');
            if (parts == null || parts.Length < 2)
                return string.Empty;
            return parts[1];
        }

        //弹出对话框让用户选择生成的专题图类型
        private static void GetTemplateNameByUserControl(SubProductInstanceDef[] instanceDefs, out string outFileIdentify, out string templateName)
        {
            outFileIdentify = string.Empty;
            templateName = string.Empty;
            if (instanceDefs == null || instanceDefs.Length == 0)
                return;
            using (TemplateTypeSelector tempalteTypeSelector = new TemplateTypeSelector(instanceDefs))
            {
                if (tempalteTypeSelector.ShowDialog() == DialogResult.OK)
                {
                    outFileIdentify = tempalteTypeSelector.OutFileIdentify;
                    templateName = tempalteTypeSelector.TemplateName;
                    tempalteTypeSelector.Close();
                }
            }
        }

        public static IExtractResult StatAnaylsisBase(ISmartSession session, string outFileIdentify, string aoiTemplate, string subProductIdentify, string algorithmName, bool isCustom, bool multiSelect)
        {
            (session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct(subProductIdentify);
            GetCommandAndExecute(session, 6602);
            IMonitoringSubProduct msp = (session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            if (msp == null)
                return null;
            object obj = msp.ArgumentProvider.GetArg("SelectedPrimaryFiles");
            //if (obj == null || !File.Exists(obj.ToString()))
            //{
            if (!SetSelectedPrimaryFiles(session, ref obj, msp.ArgumentProvider, algorithmName))
                return null;
            //}
            string[] files = obj as string[];
            if (files == null || files.Length == 0)
            {
                if (File.Exists(obj.ToString()))
                    files = new string[] { obj.ToString() };
                else
                    return null;
            }
            if (isCustom && !SetAOIArugment(files, msp, multiSelect))
                return null;
            else if (!isCustom)
                msp.ArgumentProvider.SetArg("AOI", string.IsNullOrEmpty(aoiTemplate) ? null : aoiTemplate);
            try
            {
                msp.ArgumentProvider.SetArg("FileNameGenerator", (session.MonitoringSession as IMonitoringSession).FileNameGenerator);
                msp.ArgumentProvider.SetArg("OutFileIdentify", outFileIdentify);
                IExtractResult result = msp.Make(null);
                DisplayResultClass.DisplayResult(session, msp, result, false);
                return result;
            }
            catch
            {
                return null;
            }
        }

        private static void GetCommandAndExecute(ISmartSession session, int cmdID)
        {
            ICommand cmd = session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute((session.MonitoringSession as MonitoringSession).ActiveMonitoringProduct.Identify);
        }

        public static IExtractResult CreateThemeGraphyBase(ISmartSession session, string algorithmName, string outFileIdentify, string colorTableName, string dataIdentify, string templateName, bool isCustom, bool multiSelect)
        {
            return CreateThemeGraphyBase(session, algorithmName, outFileIdentify, colorTableName, dataIdentify, templateName, isCustom, multiSelect, false);
        }

        //根据文件名提取区域标识，限定专题图输出数据范围。一般用于制作网络图。
        public static IExtractResult CreateThemeGraphyBase(ISmartSession session, string algorithmName, string outFileIdentify, string colorTableName, string dataIdentify, string templateName, bool isCustom, bool multiSelect, bool isUseRegion)
        {
            IMonitoringSubProduct msp = (session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            if (msp == null)
                return null;
            object obj = msp.ArgumentProvider.GetArg("SelectedPrimaryFiles");
            //if (obj == null)
            //{
            if (!SetSelectedPrimaryFiles(session, ref obj, msp.ArgumentProvider, algorithmName))
                return null;
            //}
            string[] files = obj as string[];
            if (files == null || files.Length == 0)
            {
                if (File.Exists(obj.ToString()))
                    files = new string[] { obj.ToString() };
                else
                    return null;
            }
            //添加对生成专题图文件类型的判断，排除生成的专题图和统计结果文件,add by wangyu,20120923
            foreach (string fname in files)
                CanCreatThemegraph(fname);
            if (isCustom && !SetAOIArugment(files, msp, multiSelect))
                return null;
            else if (!isCustom)
                msp.ArgumentProvider.SetArg("AOI", null);
            IThemeGraphGenerator tgg = (session.MonitoringSession as IMonitoringSession).ThemeGraphGenerator;
            msp.ArgumentProvider.SetArg("ThemeGraphyGenerator", tgg);
            msp.ArgumentProvider.SetArg("OutFileIdentify", outFileIdentify);
            msp.ArgumentProvider.SetArg("ThemeGraphTemplateName", templateName);
            if (isUseRegion)// 换为字符串，为区域定义分组名称regionGroupName，为空代表不指定区域。
            {
                RasterIdentify rstId = new RasterIdentify(files[0]);
                if (!string.IsNullOrWhiteSpace(rstId.RegionIdentify))
                {
                    DefinedRegionParse reg = new DefinedRegionParse();
                    BlockItemGroup blockGroup = reg.BlockDefined.FindGroup("积雪");
                    PrjEnvelopeItem envItem = blockGroup.GetPrjEnvelopeItem(rstId.RegionIdentify);
                    RasterProject.PrjEnvelope prjEnvelope = RasterProject.PrjEnvelope.CreateByCenter(envItem.PrjEnvelope.CenterX, envItem.PrjEnvelope.CenterY, 1000, 1000);
                    msp.ArgumentProvider.SetArg("UseRegion", prjEnvelope);
                }
            }
            if (!string.IsNullOrEmpty(colorTableName))
                msp.ArgumentProvider.SetArg("colortablename", "colortablename=" + colorTableName);
            IExtractResult result = msp.Make(null);
            DisplayResultClass.DisplayResult(session, msp, result, false);
            return result;
        }

        //判断文件是否能生成专题图
        private static void CanCreatThemegraph(string fname)
        {
            if (string.IsNullOrEmpty(fname))
                throw new ArgumentException("选择的文件：“" + Path.GetFileName(fname) + "”不存在。");
            if (!File.Exists(fname))
                throw new ArgumentException("选择的文件：“" + Path.GetFileName(fname) + "”不存在。");
            if (!OpenFileFactory.SupportOpenFile(fname))
                throw new ArgumentException("选择的文件：“" + Path.GetFileName(fname) + "”类型不正确。");
        }

        //public static bool SetAOIArugment(string fname, IMonitoringSubProduct msp, bool multiSelect)
        //{
        //    using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
        //    {
        //        using (frmStatSubRegionTemplates frm = new frmStatSubRegionTemplates(new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.ResolutionX))
        //        {
        //            frm.listView1.MultiSelect = multiSelect;
        //            if (frm.ShowDialog() == DialogResult.OK)
        //                msp.ArgumentProvider.SetArg("AOI", frm.GetFeatureAOIIndex());
        //            else
        //                return false;
        //        }
        //    }
        //    return true;
        //}

        public static bool SetAOIArugment(string[] fnames, IMonitoringSubProduct msp, bool multiSelect)
        {
            float maxResultion = 0f;
            IRasterDataProvider prd = null;
            CoordEnvelope envelope = new CoordEnvelope(double.MaxValue, double.MinValue, double.MaxValue, double.MinValue);
            foreach (string item in fnames)
            {
                try
                {
                    prd = GeoDataDriver.Open(item) as IRasterDataProvider;
                    maxResultion = Math.Max(maxResultion, prd.ResolutionX);
                    envelope = envelope.Union(new CoordEnvelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinX + prd.ResolutionX * prd.Width,
                                              prd.CoordEnvelope.MinY, prd.CoordEnvelope.MinY + prd.ResolutionY * prd.Height));
                }
                catch
                {
                    throw new ArgumentException("选择的文件：“" + Path.GetFileName(item) + "”无法进行面积统计。");
                }
                finally
                {
                    if (prd != null)
                    {
                        prd.Dispose();
                        prd = null;
                    }
                }
            }
            Size size = new Size((int)(envelope.Width / maxResultion), (int)(envelope.Height / maxResultion));
            using (frmStatSubRegionTemplates frm = new frmStatSubRegionTemplates(size, envelope, maxResultion))
            {
                frm.listView1.MultiSelect = multiSelect;
                if (frm.ShowDialog() == DialogResult.OK)
                    msp.ArgumentProvider.SetArg("AOI", frm.GetFeatureAOIIndex());
                else
                    return false;
            }
            return true;
        }

        //自定义矢量模版面积统计 2012.10.30 by DW
        public static IExtractResult StatAnaylsisBase(ISmartSession session, string outFileIdentify, string aoiTemplate, string subProductIdentify, string algorithmName, bool isCustom, bool multiSelect,Dictionary<string,string> fileNameList)
        {
            (session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct(subProductIdentify);
            GetCommandAndExecute(session, 6602);
            IMonitoringSubProduct msp = (session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            if (msp == null)
                return null;
            object obj = msp.ArgumentProvider.GetArg("SelectedPrimaryFiles");
            if (!SetSelectedPrimaryFiles(session, ref obj, msp.ArgumentProvider, algorithmName))
                return null;
            string[] files = obj as string[];
            if (files == null || files.Length == 0)
            {
                if (File.Exists(obj.ToString()))
                    files = new string[] { obj.ToString() };
                else
                    return null;
            }
            if (isCustom && !SetAOIArugment(files, msp, multiSelect,fileNameList))
                return null;
            else if (!isCustom)
                msp.ArgumentProvider.SetArg("AOI", string.IsNullOrEmpty(aoiTemplate) ? null : aoiTemplate);
            try
            {
                msp.ArgumentProvider.SetArg("FileNameGenerator", (session.MonitoringSession as IMonitoringSession).FileNameGenerator);
                msp.ArgumentProvider.SetArg("OutFileIdentify", outFileIdentify);
                IExtractResult result = msp.Make(null);
                DisplayResultClass.DisplayResult(session, msp, result, false);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static bool SetAOIArugment(string[] fnames, IMonitoringSubProduct msp, bool multiSelect, Dictionary<string, string> fileNameList)
        {
            float maxResultion = 0f;
            IRasterDataProvider prd = null;
            CoordEnvelope envelope = new CoordEnvelope(double.MaxValue, double.MinValue, double.MaxValue, double.MinValue);
            foreach (string item in fnames)
            {
                try
                {
                    prd = GeoDataDriver.Open(item) as IRasterDataProvider;
                    maxResultion = Math.Max(maxResultion, prd.ResolutionX);
                    envelope = envelope.Union(new CoordEnvelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinX + prd.ResolutionX * prd.Width,
                                              prd.CoordEnvelope.MinY, prd.CoordEnvelope.MinY + prd.ResolutionY * prd.Height));
                }
                catch
                {
                    throw new ArgumentException("选择的文件：“" + Path.GetFileName(item) + "”无法进行面积统计。");
                }
                finally
                {
                    if (prd != null)
                    {
                        prd.Dispose();
                        prd = null;
                    }
                }
            }
            Size size = new Size((int)(envelope.Width / maxResultion), (int)(envelope.Height / maxResultion));
            using (frmStatCustomSubRegionTemplates frm = new frmStatCustomSubRegionTemplates(size, envelope, maxResultion, fileNameList))
            {
                frm.listView1.MultiSelect = multiSelect;
                if (frm.ShowDialog() == DialogResult.OK)
                    msp.ArgumentProvider.SetArg("AOI", frm.GetFeatureAOIIndex());
                else
                    return false;
            }
            return true;
        }

        public static IExtractResult CreateThemeGraphyBase(ISmartSession session, string algorithmName, string outFileIdentify, string colorTableName, string dataIdentify, string templateName, bool isCustom, bool multiSelect,Dictionary<string,string> aoiTemplateList)
        {
            IMonitoringSubProduct msp = (session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            if (msp == null)
                return null;
            object obj = msp.ArgumentProvider.GetArg("SelectedPrimaryFiles");
            //if (obj == null)
            //{
            if (!SetSelectedPrimaryFiles(session, ref obj, msp.ArgumentProvider, algorithmName))
                return null;
            //}
            string[] files = obj as string[];
            if (files == null || files.Length == 0)
            {
                if (File.Exists(obj.ToString()))
                    files = new string[] { obj.ToString() };
                else
                    return null;
            }
            //添加对生成专题图文件类型的判断，排除生成的专题图和统计结果文件,add by wangyu,20120923
            foreach (string fname in files)
                CanCreatThemegraph(fname);
            if (isCustom && !SetAOIArugment(files, msp, multiSelect,aoiTemplateList))
                return null;
            else if (!isCustom)
                msp.ArgumentProvider.SetArg("AOI", null);
            IThemeGraphGenerator tgg = (session.MonitoringSession as IMonitoringSession).ThemeGraphGenerator;
            msp.ArgumentProvider.SetArg("ThemeGraphyGenerator", tgg);
            msp.ArgumentProvider.SetArg("OutFileIdentify", outFileIdentify);
            msp.ArgumentProvider.SetArg("ThemeGraphTemplateName", templateName);
            if (!string.IsNullOrEmpty(colorTableName))
                msp.ArgumentProvider.SetArg("colortablename", "colortablename=" + colorTableName);
            IExtractResult result = msp.Make(null);
            DisplayResultClass.DisplayResult(session, msp, result, false);
            return result;
        }

        public static IExtractResult TimeStat(ISmartSession session, string outFileIdentify, string colorTableName, string dataIdentify, string templateName, bool isCustom, bool multiSelect,Dictionary<string,string> aoiTemplateList)
        {
            (session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FREQ");
            GetCommandAndExecute(session, 6602);
            return CreateThemeGraphyBase(session, "FREQAlgorithm", outFileIdentify, colorTableName, dataIdentify, templateName, isCustom, multiSelect,aoiTemplateList);
        }
        /// <summary>
        /// 临时测试用获取文件填充 SelectedPrimaryFiles 属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="iArgumentProvider"></param>
        /// <returns></returns>
        public static bool SetSelectedPrimaryFiles(ISmartSession session, ref object obj, IArgumentProvider iArgumentProvider, string algorithmName)
        {
            IWorkspace wks = (session.MonitoringSession as IMonitoringSession).Workspace;
            if (wks == null)
                return false;
            ICatalog catalog = wks.ActiveCatalog;
            if (catalog == null)
                return false;
            string[] fnames = catalog.GetSelectedFiles();
            if (fnames == null || fnames.Length == 0)
                return false;
            obj = fnames;
            iArgumentProvider.SetArg("SelectedPrimaryFiles", fnames);
            iArgumentProvider.SetArg("AlgorithmName", algorithmName);
            return true;
        }

        /// <summary>
        /// 生成动画
        /// </summary>
        /// <param name="session"></param>
        /// <param name="argument">template:沙尘动画示意专题图,动画示意专题图,productIdentify, subProductIdentify</param>
        public static void CreatAVI(ISmartSession session, string argument)
        {
            string[] fnames = GetFilesByWorkspace(session);
            if (fnames == null || fnames.Length == 0)
                return;
            string tempArg = null;
            string wndName = null;
            string productIdentify = null;
            string subIdentify = null;
            ParserArgument(argument, ref tempArg, ref wndName, ref productIdentify, ref subIdentify);
            IAVILayerDisplay aviDis = new AVILayerDisplay();
            if (subIdentify == "CMED")
                aviDis.IsCustom = true;
            string outputFname = null;
            aviDis.DisplayAvi(session, wndName, fnames, tempArg, subIdentify, out outputFname);
            IExtractResult er = new FileExtractResult(subIdentify, outputFname) as IExtractResult;
            if (er == null)
                return;
            DisplayResultClass.DisplayResult(session, null, er, false);
        }

        private static string[] GetFilesByWorkspace(ISmartSession session)
        {
            IMonitoringSession mession = session.MonitoringSession as IMonitoringSession;
            IWorkspace wks = mession.GetWorkspace();
            if (wks == null)
                return null;
            ICatalog catalog = wks.ActiveCatalog;
            if (catalog == null)
                return null;
            return catalog.GetSelectedFiles();
        }

        private static void ParserArgument(string argument, ref string tempName, ref string wndName, ref string productIdentify, ref string subIdentify)
        {
            if (string.IsNullOrEmpty(argument))
                return;
            string[] parts = argument.Split(',');
            switch (parts.Length)
            {
                case 0:
                    return;
                case 1:
                    tempName = parts[0];
                    return;
                case 2:
                    tempName = parts[0];
                    wndName = parts[1];
                    return;
                case 3:
                    tempName = parts[0];
                    wndName = parts[1];
                    productIdentify = parts[2];
                    return;
                case 4:
                    tempName = parts[0];
                    wndName = parts[1];
                    productIdentify = parts[2];
                    subIdentify = parts[3];
                    return;
            }
        }

        /// <summary>
        /// 监测示意图
        /// </summary>
        /// <param name="session"></param>
        /// <param name="argument">"template:沙尘监测示意图,监测示意图,DST,MCSI"</param>
        public static void DisplayMonitorShow(ISmartSession session, string argument)
        {
            string subIdentify = null;
            try
            {
                //MonitorShowSettings setting = GetMonitorShowSettings();
                //if (setting == null)
                //    return ;
                //MonitorShowSettings.CurrentSettings = setting;
                MonitorShowDisplay msd = new MonitorShowDisplay();
                string fname = msd.DisplayMonitorBitmap(session, argument, out subIdentify);
                if (string.IsNullOrEmpty(fname))
                    return;
                IExtractResult er = new FileExtractResult(subIdentify, fname) as IExtractResult;
                if (er == null)
                    return;
                DisplayResultClass.DisplayResult(session, null, er, false);
            }
            finally
            {
                //if (MonitorShowSettings.CurrentSettings != null)
                //    MonitorShowSettings.CurrentSettings = null;
            }
        }

        private static MonitorShowSettings GetMonitorShowSettings()
        {
            using (frmMonitorShowSetting frm = new frmMonitorShowSetting())
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return frm.Settings;
                }
            }
            return null;
        }

        public static SubProductInstanceDef[] instanceDefs { get; set; }
    }
}
