using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

namespace GeoDo.RSS.UI.AddIn.RemoveLines
{
    [Export(typeof(GeoDo.RSS.Core.UI.ICommand))]
    class CommandHDF5RemoveLine : Command
    {
        public CommandHDF5RemoveLine()
            :base()
        {
            _id = 78002;
            _name = "HDF5RemoveLine";
            _text = "HDF5数据去条带";
            _toolTip = "HDF5数据去条带";

        }
        public override void Execute()
        {
            using (frmHDF5DataRemoveLines frm = new frmHDF5DataRemoveLines())
            {
                frm._smartSession = _smartSession;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
            }
        }

        public override void Execute(string argument)
        {
            Execute();
        }
    }
}
