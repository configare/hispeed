using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.DF.GDAL.HDF5Universal;
using GeoDo.RSS.UI.WinForm;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    [Export(typeof(GeoDo.RSS.Core.UI.ICommand))]
    public class CommandHdf5Open : Command
    {
        public CommandHdf5Open()
            : base()
        {
            _id = 2010;
            _name = "OpenHDF5File";
            _text = "打开hdf5文件";
            _toolTip = "打开hdf5文件";
        }

        public override void Execute()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Multiselect = false;
                dlg.Filter = SupportedFileFilters.HdfFilter;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Execute(dlg.FileName);
                }
            }
        }

        public override void Execute(string argument)
        {
            argument = argument.ToUpper();
            //if (FileHeaderIsOK(argument))
            //{
            HdfFileProcessor process = new HdfFileProcessor();
            bool memoryIsNotEnough = true;
            process.SetSession(_smartSession);
            process.Open(argument, out memoryIsNotEnough);
            //}
        }

        private bool FileHeaderIsOK(string filename)
        {
            string datasets = Selectdataset(filename);
            if (datasets == null || datasets.Length == 0)
                return false;
            return true; 
        }

        private string Selectdataset(string filename)
        {
            string datasets = null;
            using (Hdf5DatasetSelection frm = new Hdf5DatasetSelection())
            {
                frm.LoadFile(filename);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    datasets = frm.SelectedDatasets;
                }
            }
            return datasets;
        }
    }
}
