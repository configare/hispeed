using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Windows;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using System.Drawing;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class IceFeatureListWnd : ToolWindowBase, ISmartToolWindow
    {
        public IceFeatureListWnd()
            : base()
        {
            _id = 78008;
            Text = "海冰冰缘线交互窗口";
            //_onWindowClosed = new EventHandler((obj, args) =>
            //{
            //});
        }

        protected override IToolWindowContent GetToolWindowContent()
        {
            _content = new IceFeatureListContent();
            return _content;
        }

        void IceFeatureListWnd_ExportToShapeFile(string file)
        {
            
        }

        public IToolWindowContent ToolWindowContent
        {
            get { return _content; }
        }

        internal void InitIce(string defaultBand)
        {
            (_content as IceFeatureListContent).Init(defaultBand);
        }
    }
}
