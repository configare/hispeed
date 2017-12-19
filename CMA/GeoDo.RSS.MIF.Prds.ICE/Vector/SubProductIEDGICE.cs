using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Layout.Elements;
using GeoDo.RSS.Layout;
using System.Xml.Linq;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Prds.ICE;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    /// <summary>
    /// 冰缘线。
    /// </summary>
    public class SubProductIEDGICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductIEDGICE(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "IEDGAlgorithm")
            {
                return IEDGAlgorithm();
            }
            return null;
        }

        private IExtractResult IEDGAlgorithm()
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int farInfraredBandNo = TryGetBandNo(bandNameRaster, "FarInfrared");

            try
            {
                ISmartSession smartSession = _argumentProvider.GetArg("SmartSession") as ISmartSession;
                ICommand cmd = smartSession.CommandEnvironment.Get(78000);
                if (cmd != null)
                    cmd.Execute(farInfraredBandNo.ToString());
                return null;
            }
            finally
            {
            }
        }
    }
}
