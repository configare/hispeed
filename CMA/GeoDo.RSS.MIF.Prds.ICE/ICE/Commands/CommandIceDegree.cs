using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;
using GeoDo.RSS.UI.AddIn.Windows;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    [Export(typeof(ICommand))]
    public class CommandIceDegree : Command
    {
        private Dictionary<Feature, IceFreeCurveInfoLayer.InfoItem[]> _iceFeatures = new Dictionary<Feature, IceFreeCurveInfoLayer.InfoItem[]>();

        public CommandIceDegree()
            : base()
        {
            _id = 78001;
            _name = _text = _toolTip = "海冰覆盖度计算";
        }

        public override void Execute()
        {
            Execute(null);
        }

        public override void Execute(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
                return;
            //string[] vals = argument.Split(',');
            IMonitoringSession ms = _smartSession.MonitoringSession as IMonitoringSession;
            IMonitoringSubProduct prd = ms.ChangeActiveSubProduct("DEGR");
            if (argument == "DEG1")
            {
                prd.ArgumentProvider.SetArg("XInterval", 0.1);
                prd.ArgumentProvider.SetArg("YInterval", 0.1);
            }
            else if (argument == "DEG2")
            {
                prd.ArgumentProvider.SetArg("XInterval", 0.25);
                prd.ArgumentProvider.SetArg("YInterval", 0.25);
            }
            else
            {
                using (frmCustomIceDegree frm = new frmCustomIceDegree())
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        prd.ArgumentProvider.SetArg("XInterval", frm.X);
                        prd.ArgumentProvider.SetArg("YInterval", frm.Y);
                    }
                    else
                        return;
                }
            }
            prd.ArgumentProvider.SetArg("OutFileIdentify", argument);
            ms.DoAutoExtract(true);
        }
    }
}
