using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.IO;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using GeoDo.RSS.UI.AddIn.Layout;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Core.VectorDrawing;
using System.Threading.Tasks;
using System.Threading;

namespace GeoDo.RSS.UI.WinForm
{
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    internal class ThemeFileProcessor : OpenFileProcessor
    {
        public ThemeFileProcessor()
            : base()
        {
            _extNames.Add(".GXT");
            _extNames.Add(".GXD");
        }

        protected override bool FileHeaderIsOK(string fname, string extName)
        {
            return true;
        }

        public override bool Open(string fname, out bool memoryIsNotEnough)
        {
            memoryIsNotEnough = false;
            string text = OpenFileFactory.GetTextByFileName(fname);
            ILayoutViewer viewer = new LayoutViewer(text);
            (viewer as LayoutViewer).Tag = fname;
            (viewer as LayoutViewer).SetSession(_session);
            _session.SmartWindowManager.DisplayWindow(viewer);
            if (viewer != null)
            {
                string extName = Path.GetExtension(fname).ToUpper();
                switch (extName)
                {
                    case ".GXT":
                        ILayoutTemplate template = LayoutTemplate.LoadTemplateFrom(fname);
                        if (template != null)
                        {
                            viewer.LayoutHost.ApplyTemplate(template);
                            viewer.LayoutHost.ToSuitedSize();
                            AnsynRefreshData(viewer);
                            _session.UIFrameworkHelper.ActiveTab("专题制图");
                        }
                        break;
                    case ".GXD":
                        IGxdDocument doc = GxdDocument.LoadFrom(fname);
                        if (doc != null)
                        {
                            viewer.LayoutHost.ApplyGxdDocument(doc);
                            viewer.LayoutHost.ToSuitedSize();
                            AnsynRefreshData(viewer);
                            //_session.UIFrameworkHelper.ActiveTab("专题制图");
                        }
                        break;
                }
            }
            return true;
        }

        private void AnsynRefreshData(ILayoutViewer viewer)
        {
            ICommand cmd = _session.CommandEnvironment.Get(6032);//refresh data
            if (cmd != null)
                cmd.Execute();
        }
    }
}
