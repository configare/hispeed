using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public class SubProductTHANLST : CmaMonitoringSubProduct
    {
        public SubProductTHANLST(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "THANExtract")
            {
                return THANExtract(progressTracker);
            }
            return null;
        }

        private IExtractResult THANExtract(Action<int, string> progressTracker)
        {
            object obj = _argumentProvider.GetArg("ucAnlysisTool");
            UCAnlysisTool ucAnlysisTool = null;
            if (obj != null)
            {
                ucAnlysisTool = obj as UCAnlysisTool;
                ucAnlysisTool.btnGetInfos_Click(null, null);
            }
            return null;
        }

        public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        {
            object obj = _argumentProvider.GetArg("ucAnlysisTool");
            UCAnlysisTool ucAnlysisTool = null;
            if (obj != null)
                ucAnlysisTool = obj as UCAnlysisTool;
            else
                return null;
            RasterIdentify rid = new RasterIdentify(_argumentProvider.DataProvider.fileName);
            rid.ProductIdentify = _subProductDef.ProductDef.Identify;
            rid.SubProductIdentify = _subProductDef.Identify;
            string dstfilename = rid.ToWksFullFileName(".txt");
            if (File.Exists(dstfilename))
                File.Delete(dstfilename);
            if (!string.IsNullOrEmpty(ucAnlysisTool.txtInfos.Text))
            {
                File.WriteAllLines(dstfilename, new string[] { ucAnlysisTool.txtInfos.Text }, Encoding.Unicode);
                FileExtractResult resTxt = new FileExtractResult("LST", dstfilename, true);
                resTxt.SetDispaly(false);
                return resTxt;
            }
            return null;
        }
    }
}
