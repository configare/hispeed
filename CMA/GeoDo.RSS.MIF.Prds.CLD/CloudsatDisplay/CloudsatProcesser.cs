using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.UI.AddIn.CanvasViewer;
using GeoDo.RSS.DF.HDF4.Cloudsat;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    public class CloudsatProcesser : OpenFileProcessor, IRasterFileProcessor
    {
        public CloudsatProcesser()
            : base()
        {
            _extNames.Add(".HDF");
        }

        protected override bool FileHeaderIsOK(string fname, string extName)
        {
            return CloudsatDataProvider.IsSupport(fname, null);
        }

        public override bool Open(string fname, out bool memoryIsNotEnough)
        {
            memoryIsNotEnough = true;
            CreateCanvasViewer(fname);
            return true;
        }

        private void CreateCanvasViewer(string fname)
        {
            ICommand comm = _session.CommandEnvironment.Get(29001);
            comm.Execute(fname);
        }
    }
}
