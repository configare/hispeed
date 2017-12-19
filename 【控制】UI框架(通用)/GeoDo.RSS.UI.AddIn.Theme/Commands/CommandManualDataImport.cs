using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.CMA;
using GeoDo.RSS.DI;
using System.Windows.Forms;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandManualDataImport : CommandProductBase
    {
        public CommandManualDataImport()
            : base()
        {
            _id = 6634;
            _name = "DataImport";
            _text = _toolTip = "手动导入";
        }

        public override void Execute(string argument)
        {
            ManualImportData import = new ManualImportData(_smartSession, argument);
            string error = import.Do();
            if (!string.IsNullOrEmpty(error))
                MessageBox.Show(error, "数据导入", MessageBoxButtons.OK);
        }

        public override void Execute()
        {
            Execute(string.Empty);
        }
    }
}
