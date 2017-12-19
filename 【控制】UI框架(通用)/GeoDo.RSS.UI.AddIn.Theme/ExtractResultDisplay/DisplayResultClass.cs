using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public class DisplayResultClass
    {
        public static IContextEnvironment _contextEnvironment;

        public static void DisplayResult(ISmartSession session, IMonitoringSubProduct subProduct, IExtractResult restult, bool extract)
        {
            if (restult == null)
            {
                ClosedActiveMonitoringSubProduct(session, subProduct);
                return;
            }
            IMonitoringSession msession = session.MonitoringSession as IMonitoringSession;
            if (restult is IPixelIndexMapper)
            {
                DisplayPixelIndexMapper(restult, session);
            }
            else if (restult is IFileExtractResult)
            {
                DisplayFileResult(subProduct, session, restult);
            }
            else if (restult is IExtractResultArray)
            {
                IExtractResultArray extResultMapper = restult as ExtractResultArray;
                IExtractResultBase[] mappers = extResultMapper.PixelMappers;
                if (mappers == null || mappers.Length == 0)
                    return;
                foreach (IExtractResultBase mapper in mappers)
                {
                    if (mapper is IFileExtractResult)
                        DisplayFileResult(subProduct, session, mapper as IExtractResult);
                    else if (mapper is IPixelIndexMapper && extract)
                        DisplayPixelIndexMapper(mapper as IPixelIndexMapper, session);
                    else
                        DisplayPixelFeatureMapper(session, subProduct, mapper as IExtractResult, false);
                }
            }
            else if (restult is IValueExtractResult)
                DisplayValueResult(session, subProduct, restult);
            else
                DisplayPixelFeatureMapper(session, subProduct, restult, true);
            ClosedActiveMonitoringSubProduct(session, subProduct);
        }

        private static void ClosedActiveMonitoringSubProduct(ISmartSession session, IMonitoringSubProduct subProduct)
        {
            if (subProduct == null || subProduct.Definition == null)
                return;
            if (!subProduct.Definition.IsDisplayPanel)
            {
                MonitoringSession ms = session.MonitoringSession as MonitoringSession;
                if (ms != null)
                    ms.ClosedActiveMonitoringSubProduct();
            }

        }

        private static void DisplayValueResult(ISmartSession session, IMonitoringSubProduct subProduct, IExtractResult restult)
        {
            //
            return;
        }

        private static void DisplayPixelFeatureMapper(ISmartSession session, IMonitoringSubProduct subProduct, IExtractResult restult, bool openFile)
        {
            int extHeaderSize = 0;
            object header = null;
            GetExtHeader(subProduct, out extHeaderSize, out header);
            if (restult is IPixelFeatureMapper<float>)
            {
                IPixelFeatureMapper<float> ifm = restult as IPixelFeatureMapper<float>;
                RasterIdentify rid = GetRasterIdentifyID(session);
                if (!string.IsNullOrEmpty(ifm.Name) && ifm.Name != rid.SubProductIdentify)
                    rid.SubProductIdentify = ifm.Name;
                //文件已存在，并且使用感兴趣区域时，使用更新感兴趣区域值的方式。

                IInterestedRaster<float> iir = new InterestedRaster<float>(rid, ifm.Size, ifm.CoordEnvelope, ifm.SpatialRef, extHeaderSize);
                subProduct.SetExtHeader(iir, header);
                iir.Put(ifm);
                iir.Dispose();
                TryOrbitToWorkspace(session);
                //if (openFile && NeedOpenFile())
                //    TryOpenFile(session, iir.FileName);
                RecordFileForAfterProcess(iir.FileName);
                TrySaveFileToWorkspace(subProduct, session, iir.FileName, restult, null);
            }
            else if (restult is IPixelFeatureMapper<int>)
            {
                IPixelFeatureMapper<int> ifm = restult as IPixelFeatureMapper<int>;
                RasterIdentify rid = GetRasterIdentifyID(session);
                IInterestedRaster<int> iir = new InterestedRaster<int>(rid, ifm.Size, ifm.CoordEnvelope, ifm.SpatialRef, extHeaderSize);
                iir.Put(ifm);
                iir.Dispose();
                TryOrbitToWorkspace(session);
                if (openFile && NeedOpenFile())
                    TryOpenFile(session, iir.FileName);
                RecordFileForAfterProcess(iir.FileName);
                TrySaveFileToWorkspace(subProduct, session, iir.FileName, restult, null);
            }
            else if (restult is IPixelFeatureMapper<UInt16>)
            {
                IPixelFeatureMapper<UInt16> ifm = restult as IPixelFeatureMapper<UInt16>;
                RasterIdentify rid = GetRasterIdentifyID(session);
                IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(rid, ifm.Size, ifm.CoordEnvelope, ifm.SpatialRef, extHeaderSize);
                iir.Put(ifm);
                iir.Dispose();
                TryOrbitToWorkspace(session);
                //if (openFile && NeedOpenFile())
                //    TryOpenFile(session, iir.FileName);
                RecordFileForAfterProcess(iir.FileName);
                TrySaveFileToWorkspace(subProduct, session, iir.FileName, restult, null);
            }
            else if (restult is IPixelFeatureMapper<Int16>)
            {
                IPixelFeatureMapper<Int16> ifm = restult as IPixelFeatureMapper<Int16>;
                RasterIdentify rid = GetRasterIdentifyID(session);
                IInterestedRaster<Int16> iir = new InterestedRaster<Int16>(rid, ifm.Size, ifm.CoordEnvelope, ifm.SpatialRef, extHeaderSize);
                iir.Put(ifm);
                iir.Dispose();
                TryOrbitToWorkspace(session);
                //if (openFile && NeedOpenFile())
                //    TryOpenFile(session, iir.FileName);
                RecordFileForAfterProcess(iir.FileName);
                TrySaveFileToWorkspace(subProduct, session, iir.FileName, restult, null);
            }
            ClearExtHeader(subProduct);
        }

        private static void DisplayPixelIndexMapper(IExtractResult restult, ISmartSession session)
        {
            (session.MonitoringSession as IMonitoringSession).ExtractingSession.ApplyResult(restult as IPixelIndexMapper);
            TryOrbitToWorkspace(session);
        }

        private static void DisplayFileResult(IMonitoringSubProduct subProduct, ISmartSession session, IExtractResult restult)
        {
            string filename = (restult as IFileExtractResult).FileName;
            RecordFileForAfterProcess(filename);
            IFileExtractResult fileResult = restult as IFileExtractResult;
            if (fileResult.Add2Workspace)
                TrySaveFileToWorkspace(subProduct, session, filename, restult, null);
            if (!(restult as IExtractResultBase).Display)
                return;
            if (NeedOpenFile())
            {
                object obj = subProduct.ArgumentProvider.GetArg("fileOpenArgs");
                string[] args = null;
                if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                    args = new string[] { obj.ToString() };
                ICommand cmd = session.CommandEnvironment.Get(2000);
                if (cmd != null)
                    cmd.Execute(filename, args);
            }
        }

        private static void TrySaveFileToWorkspace(IMonitoringSubProduct subProduct, ISmartSession session, string fname, IExtractResult restult, CatalogItemInfo cii)
        {
            if (string.IsNullOrEmpty(fname))
                return;
            IMonitoringSession mession = session.MonitoringSession as IMonitoringSession;
            IWorkspace wks = mession.GetWorkspace();
            RasterIdentify rst = new RasterIdentify(fname);
            ICatalog c = wks.GetCatalog("CurrentExtracting");
            if (c != null)
            {
                if (cii == null)
                    c.AddItem(new CatalogItem(fname, c.Definition as SubProductCatalogDef));
                else
                    c.AddItem(new CatalogItem(fname, cii));
            }
            if (_contextEnvironment != null && subProduct != null)
            {
                rst.SubProductIdentify = subProduct.Identify;

                IExtractResultBase erb = restult as IExtractResultBase;
                if (erb != null && !string.IsNullOrEmpty(erb.OutIdentify))
                    rst.SubProductIdentify = erb.OutIdentify;
                GetOutFileIdentify(ref rst, subProduct);
                if (!string.IsNullOrEmpty(restult.Name) && restult.Name != rst.SubProductIdentify)
                    rst.SubProductIdentify = restult.Name;
                _contextEnvironment.PutContextVar(rst.SubProductIdentify, fname);
            }
        }

        public static void TrySaveFileToWorkspace(IMonitoringSubProduct subProduct, IMonitoringSession mession, string fname, IExtractResult restult)
        {
            if (string.IsNullOrEmpty(fname))
                return;
            IWorkspace wks = mession.GetWorkspace();
            RasterIdentify rst = new RasterIdentify(fname);
            ICatalog c = wks.GetCatalog("CurrentExtracting");
            if (c != null)
                    c.AddItem(new CatalogItem(fname, c.Definition as SubProductCatalogDef));
            if (_contextEnvironment != null && subProduct != null)
            {
                rst.SubProductIdentify = subProduct.Identify;

                IExtractResultBase erb = restult as IExtractResultBase;
                if (erb != null && !string.IsNullOrEmpty(erb.OutIdentify))
                    rst.SubProductIdentify = erb.OutIdentify;
                GetOutFileIdentify(ref rst, subProduct);
                if (!string.IsNullOrEmpty(restult.Name) && restult.Name != rst.SubProductIdentify)
                    rst.SubProductIdentify = restult.Name;
                _contextEnvironment.PutContextVar(rst.SubProductIdentify, fname);
            }
        }

        private static void ClearExtHeader(IMonitoringSubProduct subProduct)
        {
            if (subProduct.ArgumentProvider.GetArg("ExtHeaderSize") != null)
                subProduct.ArgumentProvider.SetArg("ExtHeaderSize", 0);
            if (subProduct.ArgumentProvider.GetArg("ExtHeader") != null)
                subProduct.ArgumentProvider.SetArg("ExtHeader", null);
        }

        private static void GetExtHeader(IMonitoringSubProduct subProduct, out int extHeaderSize, out object header)
        {
            extHeaderSize = 0;
            header = null;
            object obj = subProduct.ArgumentProvider.GetArg("ExtHeaderSize");
            if (obj == null)
                return;
            int.TryParse(obj.ToString(), out extHeaderSize);
            header = subProduct.ArgumentProvider.GetArg("ExtHeader");
        }

        private static void TryOpenFile(ISmartSession session, string fname)
        {
            //open file
            ICommand cmd = session.CommandEnvironment.Get(2000);
            if (cmd != null)
                cmd.Execute(fname);
        }

        private static RasterIdentify GetRasterIdentifyID(ISmartSession session)
        {
            RasterIdentify rst = null;
            IMonitoringSubProduct msp;
            IRasterDataProvider prd;
            GetPrd(session, out msp, out prd);
            DataIdentify currentIdentify = null;
            if (msp != null)
            {
                string[] files = (msp as MonitoringSubProduct).GetStringArray("SelectedPrimaryFiles");
                if (files == null || files.Length == 0)
                    files = (msp as MonitoringSubProduct).GetStringArray("CurrentRasterFile");
                if (files == null || files.Length == 0)
                    files = (msp as MonitoringSubProduct).GetStringArray("mainfiles");
                if (files == null || files.Length == 0)
                {
                    if (prd == null)
                    {
                        rst = new RasterIdentify();
                        rst.ThemeIdentify = "CMA";
                    }
                    else
                    {
                        rst = new RasterIdentify(prd.fileName);
                        currentIdentify = prd.DataIdentify;
                    }

                }
                else
                    rst = new RasterIdentify(files);
            }

            rst.ProductIdentify = msp != null ? msp.Definition.ProductDef.Identify : "";
            rst.SubProductIdentify = msp != null ? msp.Identify : "";
            if (msp != null)
            {
                GetOutFileIdentify(ref rst, msp);
                GetExtInfo(ref rst, msp);
            }
            rst.IsOutput2WorkspaceDir = true;
            return rst;
        }

        private static void GetOutFileIdentify(ref RasterIdentify rst, IMonitoringSubProduct msp)
        {
            object obj = msp.ArgumentProvider.GetArg("OutFileIdentify");
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                rst.SubProductIdentify = obj.ToString();
        }

        private static void GetExtInfo(ref RasterIdentify rst, IMonitoringSubProduct msp)
        {
            object obj = msp.ArgumentProvider.GetArg("extinfo");
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                rst.ExtInfos = obj.ToString();
        }

        private static void GetPrd(ISmartSession session, out IMonitoringSubProduct msp, out IRasterDataProvider prd)
        {
            msp = (session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            IArgumentProvider ap = null;
            if (msp != null)
                ap = msp.ArgumentProvider;
            prd = null;
            if (ap != null)
                prd = ap.DataProvider;
        }

        public static void TryOrbitToWorkspace(ISmartSession session)
        {
            IMonitoringSubProduct msp;
            IRasterDataProvider prd;
            GetPrd(session, out msp, out prd);
            string fname = string.Empty;
            if (prd == null)
            {
                if (msp == null || msp.ArgumentProvider == null)
                    return;
                object obj = msp.ArgumentProvider.GetArg("CurrentRasterFile");
                if (obj == null || !string.IsNullOrEmpty(obj.ToString()))
                    return;
                else
                    fname = obj.ToString();
            }
            else
                fname = prd.fileName;
            if (!File.Exists(fname))
                return;
            CatalogItemInfo cii = new CatalogItemInfo();
            cii.Properties.Add("ProductIdentify", "ORBIT");
            cii.Properties.Add("SubProductIdentify", "OrbitFileName");
            cii.Properties.Add("OrbitDateTime", prd.DataIdentify.OrbitDateTime);
            cii.Properties.Add("CatalogItemCN", Path.GetFileName(fname));
            cii.Properties.Add("CatalogDef", "ObritData");
            //.....
            TrySaveFileToWorkspace(null, session, fname, null, cii);
        }

        private static void RecordFileForAfterProcess(string filename)
        {
            if (string.IsNullOrEmpty(filename) || AutoGeneratorSettings.CurrentSettings == null)
                return;
            if (!string.IsNullOrEmpty(AutoGeneratorSettings.CurrentSettings.FolderOfCopyTo))
            {
                if (AutoGeneratorSettings.CurrentSettings != null)
                    AutoGeneratorSettings.CurrentSettings.GeneratedFileNames.Add(filename);
            }
            if (Path.GetExtension(filename).ToUpper() == ".GXD")
            {
                if (AutoGeneratorSettings.CurrentSettings != null)
                    AutoGeneratorSettings.CurrentSettings.GxdFileNames.Add(filename);
            }
        }

        private static bool NeedOpenFile()
        {
            if (AutoGeneratorSettings.CurrentSettings == null)
                return true;
            return AutoGeneratorSettings.CurrentSettings != null && AutoGeneratorSettings.CurrentSettings.OpenFileAfterFinished;
        }
    }
}
