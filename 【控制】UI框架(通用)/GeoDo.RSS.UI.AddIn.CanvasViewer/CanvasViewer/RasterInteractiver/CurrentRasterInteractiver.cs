using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    /// <summary>
    /// 从外部(例如:判识面板)拾取当前视窗中的影像信息(例如：波段值、AOI中波段统计值等）
    /// </summary>
    internal class CurrentRasterInteractiver : ICurrentRasterInteractiver
    {
        private ICanvasViewer _canvasViewer;
        private ICanvas _canvas;
        private IRasterDrawing _rasterDrawing;
        private int[] _bandNos;
        private Action<double[]> _bandValuesNotifier;
        private ILabelService _labelService;

        public CurrentRasterInteractiver(ICanvasViewer canvasViewer)
        {
            _canvasViewer = canvasViewer;
            _labelService = new LabelService(_canvasViewer);
            Control control = _canvasViewer.Canvas.Container;
            control.MouseUp += new MouseEventHandler(control_MouseUp);
        }

        unsafe void control_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (_bandNos == null || _bandNos.Length == 0 || _bandValuesNotifier == null)
                    return;
                _canvas = _canvasViewer.Canvas;
                _rasterDrawing = _canvas.PrimaryDrawObject as IRasterDrawing;
                double[] bandValues = new double[_rasterDrawing.BandCount];
                float row, col;
                _canvas.CoordTransform.Screen2Raster(e.X, e.Y, out row, out col);
                fixed (double* bandValuesPtr = bandValues)
                {
                    _rasterDrawing.ReadPixelValues((int)col, (int)row, bandValuesPtr);
                    double[] retValues = new double[_bandNos.Length];
                    for (int i = 0; i < _bandNos.Length; i++)
                        retValues[i] = bandValues[_bandNos[i] - 1];
                    _bandValuesNotifier(retValues);
                }
                _bandNos = null;
                _bandValuesNotifier = null;
            }
            finally
            {
                _canvasViewer.Canvas.Container.Cursor = Cursors.Default;
            }
        }

        public unsafe double[] GetBandValuesInAOI(int bandNo)
        {
            int[] aoi = _canvasViewer.AOIProvider.GetIndexes();
            if (aoi == null || aoi.Length == 0)
                return null;
            _canvas = _canvasViewer.Canvas;
            _rasterDrawing = _canvas.PrimaryDrawObject as IRasterDrawing;
            int count = aoi.Length;
            double[] retValues = new double[count];
            double[] pValue = new double[_rasterDrawing.BandCount];
            int row = 0, col = 0;
            int width = _rasterDrawing.DataProvider.Width;
            fixed (double* buffer = pValue)
            {
                for (int i = 0; i < count; i++)
                {
                    row = aoi[i] / width;
                    col = aoi[i] % width;
                    _rasterDrawing.ReadPixelValues(col, row, buffer);
                    retValues[i] = pValue[bandNo - 1];
                }
            }
            return retValues;
        }

        public double GetAvgBandValueInAOI(int bandNo)
        {
            double[] bandValues = GetBandValuesInAOI(bandNo);
            if (bandValues == null || bandValues.Length == 0)
                return 0;
            return bandValues.Sum() / bandValues.Length;
        }

        public double GetMaxAvgBandValueInAOI(int bandNo, double percent)
        {
            if (percent <= 0 || percent - 0.001 < 0)
                return GetMaxBandValueInAOI(bandNo);
            double[] bandValues = GetBandValuesInAOI(bandNo);
            if (bandValues == null || bandValues.Length == 0)
                return 0;
            bandValues = bandValues.OrderByDescending((t) => t).ToArray();
            int length = (int)Math.Round(bandValues.Length * percent + 0.5);
            double[] resultValue = new double[length];
            Array.Copy(bandValues, resultValue, length);
            return resultValue.Sum() / resultValue.Length;
        }

        public double GetMinAvgBandValueInAOI(int bandNo, double percent)
        {
            if (percent <= 0 || percent - 0.001 < 0)
                return GetMinBandValueInAOI(bandNo);
            double[] bandValues = GetBandValuesInAOI(bandNo);
            if (bandValues == null || bandValues.Length == 0)
                return 0;
            bandValues = bandValues.OrderBy((t) => t).ToArray();
            int length = (int)Math.Round(bandValues.Length * percent + 0.5);
            double[] resultValue = new double[length];
            Array.Copy(bandValues, resultValue, length);
            return resultValue.Sum() / resultValue.Length;
        }

        public double GetMaxBandValueInAOI(int bandNo)
        {
            double[] bandValues = GetBandValuesInAOI(bandNo);
            if (bandValues == null || bandValues.Length == 0)
                return 0;
            return bandValues.Max();
        }

        public double GetMinBandValueInAOI(int bandNo)
        {
            double[] bandValues = GetBandValuesInAOI(bandNo);
            if (bandValues == null || bandValues.Length == 0)
                return 0;
            return bandValues.Min();
        }

        public void StartInteractive(int[] bandNos, Action<double[]> bandValuesNotifier)
        {
            _bandNos = bandNos;
            _bandValuesNotifier = bandValuesNotifier;
            _canvasViewer.Canvas.Container.Cursor = Cursors.Cross;
        }


        public void StartAOIDrawing(Action drawFinishedNotifiter)
        {
            //该方法由CanvasViewer实现
        }

        public ILabelService LabelService
        {
            get { return _labelService; }
        }

        public void StartAOIDrawing(string shapeType, Action<int[], CodeCell.AgileMap.Core.Shape> drawFinishedNotifier)
        {
            //该方法由CanvasViewer实现
        }

        public void TryFinishPencilTool()
        {
            //该方法由CanvasViewer实现
        }

    }
}
