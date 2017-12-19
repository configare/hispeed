using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MicroWaveFYDataRetrieval;
using GeoDo.RSS.UI.AddIn.Theme;


namespace GeoDo.RSS.MIF.Prds.MWS
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    class CommandMWSNWInqurStat : CommandProductBase
    {
        public CommandMWSNWInqurStat()
             : base()
        {
            _id = 36603;
            _name = "MWSNWInqurStat";
            _text = _toolTip = "微波积雪数据查询统计";
        }

        public override void Execute()
        {
            FormMain frm = new FormMain();
            frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            frm.Show();
        }

    }
}
