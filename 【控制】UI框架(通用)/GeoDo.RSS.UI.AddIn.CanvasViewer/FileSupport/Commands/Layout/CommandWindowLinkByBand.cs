using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DrawEngine;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public abstract class CommandWindowLinkByBand : CommandWindowLink
    {
        protected abstract override enumViewerLayoutStyle GetLayoutStyle();

        protected override ILinkableViewer[] GetLinkableViewers()
        {
            ICanvasViewer viewer1 = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (viewer1 == null)
                return null;
            IRasterDrawing drawing1 = viewer1.ActiveObject as IRasterDrawing;
            if (drawing1 == null || drawing1.DataProvider.BandCount == 1)
                return null;
            int[] leftBands, rightBands;
            GetBandSetting(drawing1, out leftBands, out rightBands);
            ChangeBandsAndRefresh(drawing1, viewer1,leftBands);
            ICanvasViewer viewer2 = CreateCanvasViewer(drawing1.FileName,rightBands,drawing1);
            _smartSession.SmartWindowManager.DisplayWindow(viewer2);
            return new ILinkableViewer[] { viewer1, viewer2 };
        }

        private ICanvasViewer CreateCanvasViewer(string fname, int[] rightBands,IRasterDrawing drawing1)
        {
            CanvasViewer cv = null;
            cv = new CanvasViewer(OpenFileFactory.GetTextByFileName(fname), _smartSession);
            _smartSession.SmartWindowManager.DisplayWindow(cv);
            RasterLayerBuilder.CreateAndLoadRasterLayer(_smartSession, cv.Canvas, fname, drawing1.RgbStretcherProvider);
            return cv;
        }

        private void ChangeBandsAndRefresh(IRasterDrawing drawing1, ICanvasViewer viewer1,int[] bands)
        {
            drawing1.SelectedBandNos = bands;
        }

        private void GetBandSetting(IRasterDrawing drawing, out int[] leftBands, out int[] rightBands)
        {
            leftBands = new int[] { 1 };
            rightBands = new int[] { 2 };
        }
    }

    [Export(typeof(ICommand))]
    public class CommandWindowLinkByBandH : CommandWindowLinkByBand
    {
        public CommandWindowLinkByBandH()
        {
            _id = 9103;
            _name = "WindowLinkByBandH";
            _text = _toolTip = "左右波段联动";
        }

        protected override enumViewerLayoutStyle GetLayoutStyle()
        {
            return enumViewerLayoutStyle.HorizontalLayout;
        }
    }

    [Export(typeof(ICommand))]
    public class CommandWindowLinkByBandV : CommandWindowLinkByBand
    {
        public CommandWindowLinkByBandV()
        {
            _id = 9104;
            _name = "WindowLinkByBandV";
            _text = _toolTip = "上下波段联动";
        }

        protected override enumViewerLayoutStyle GetLayoutStyle()
        {
            return enumViewerLayoutStyle.VerticalLayout;
        }
    }
}
