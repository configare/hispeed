using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductRasterVgtCYCA : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductRasterVgtCYCA()
            : base()
        {
        }

        public SubProductRasterVgtCYCA(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "CYCAAlgorithm")
            {
                return CYCAAlgorithm();
            }
            return null;
        }

        private IExtractResult CYCAAlgorithm()
        {
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null)
                files = GetFiles();
            if (files == null || files.Length == 0)
                return null;
            IRasterDataProvider[] prds = null;
            IRasterDataProvider dstPrd = null;
            string dstFilename = null;
            try
            {
                prds = GetProviderFromFiles(files);
                if (prds == null || prds.Length == 0)
                    return null;                
                RasterMoasicProcesser process = new RasterMoasicProcesser();
                dstFilename = @"d:\VGT_CYCA_" + DateTime.Now.ToString("yyyy-MM-dd") + ".ldf";
                if (_argumentProvider.GetArg("CYCAIdentify") == null) //max or avg
                    return null;
                string identify = _argumentProvider.GetArg("CYCAIdentify").ToString();
                if (identify.ToLower() == "cycamax")
                    dstPrd = process.Moasic<float>(prds, "LDF", dstFilename, false, null, "MAX", null, (srcValue, dstValue) => { return srcValue > dstValue ? srcValue : dstValue; });
                else if (identify.ToLower() == "cycaavg")
                    dstPrd = process.Moasic<float>(prds, "LDF", dstFilename, false, null, "AVG", null, (srcValue, dstValue) => { return (srcValue + dstValue) / 2f; });
            }
            catch
            {
                throw new ArgumentException("请选择正确的NDVI数据进行周期合成。");
            }
            finally
            {
                foreach (IRasterDataProvider item in prds)
                    item.Dispose();
                if (dstPrd != null)
                {
                    dstPrd.Dispose();
                    dstPrd = null;
                }
            }
            return new FileExtractResult("CYCA", dstFilename);
        }

        private string[] GetFiles()
        {
            string dir = MifEnvironment.GetWorkspaceDir() + @"\VGT\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\栅格产品\\";
            if (!Directory.Exists(dir))
                return null;
            return Directory.GetFiles(dir, "*.dat", SearchOption.TopDirectoryOnly);
        }

        private IRasterDataProvider[] GetProviderFromFiles(string[] files)
        {
            List<IRasterDataProvider> prds = new List<IRasterDataProvider>();
            int length = files.Length;
            IRasterDataProvider temp = null;
            for (int i = 0; i < length; i++)
            {
                temp = GeoDataDriver.Open(files[i]) as IRasterDataProvider;
                if (temp != null)
                    prds.Add(temp);
            }
            return prds.Count == 0 ? null : prds.ToArray();
        }

    }
}
