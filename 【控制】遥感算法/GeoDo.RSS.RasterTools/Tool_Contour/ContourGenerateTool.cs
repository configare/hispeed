using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.RasterTools
{
    public class ContourGenerateTool : IContourGenerateTool
    {
        protected double _noData = 0;
        protected bool _isOutputUncomplete = false;
        protected int _sample = 1;
        protected double[] _contourValues;

        public double NoDataForOutsideAOI
        {
            get { return _noData; }
            set { _noData = value; }
        }

        public bool IsOutputUncompleted
        {
            get { return _isOutputUncomplete; }
            set { _isOutputUncomplete = value; }
        }

        public int Sample
        {
            get { return _sample ;}
            set { _sample = value; }
        }

        public double[] ContourValues
        {
            get { return _contourValues; }
        }

        public ContourLine[] Generate(IRasterBand raster, double[] contourValues, Action<int, string> tracker)
        {
            return Generate(raster, contourValues, null,tracker);
        }

        public ContourLine[] Generate(IRasterBand raster, double beginValue, double interleave, double endValue, Action<int, string> tracker)
        {
            return Generate(raster, beginValue, interleave, endValue, null,tracker);
        }

        public ContourLine[] Generate(IRasterBand raster, double beginValue, double interleave, double endValue, int[] aoi, Action<int, string> tracker)
        {
            List<double> contourValues = new List<double>();
            double v = beginValue;
            while (v < endValue)
            {
                contourValues.Add(v);
                v += interleave;
            }
            contourValues.Add(endValue);
            return Generate(raster, contourValues.ToArray(), aoi,tracker);
        }

        public ContourLine[] Generate(IRasterBand raster, double[] contourValues, int[] aoi, Action<int, string> tracker)
        {
            if (raster == null || contourValues == null || contourValues.Length == 0)
                return null;
            using (IContourGenerator gen = ContourGeneratorFactory.GetContourGenerator(raster.DataType))
            {
                gen.NoDataForOutsideAOI = _noData;
                gen.IsOutputUncompleted = _isOutputUncomplete;
                gen.Sample = _sample;
                ContourLine[] retLines = gen.Generate(raster, contourValues,aoi,tracker);
                _contourValues = gen.ContourValues;
                return retLines;
            }
        }

        //支持空间插值
        public ContourLine[] Generate(double resolutionX, double resolutionY, enumDataType dataType, double[] contourValues, IDW_Interpolation interpolate, Action<int, string> tracker)
        {
            if (contourValues == null || contourValues.Length == 0)
                return null;
            using (IContourGenerator gen = ContourGeneratorFactory.GetContourGenerator(dataType))
            {
                double[] rasPointValue;
                int width, height;
                if (tracker != null)
                    tracker(1, "正在进行空间插值...");
                interpolate.DoIDWinterpolation(resolutionX, resolutionY, out width, out height, out rasPointValue);
                gen.NoDataForOutsideAOI = _noData;
                gen.IsOutputUncompleted = _isOutputUncomplete;
                gen.Sample = _sample;
                if (tracker != null)
                    tracker(9, "正在准备数据...");
                gen.SetDataValue(rasPointValue, width, height);
                ContourLine[] retLines = gen.Generate(width, height, contourValues, tracker);
                _contourValues = gen.ContourValues;
                return retLines;
            }
        }
    }
}
