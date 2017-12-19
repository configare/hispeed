using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// 新定义的矢量层接口
    /// </summary>
    public interface IFeatureLayer:IDisposable,IPersistable,IIdentifyFeatures,ILayer
    {
        bool EnabledDisplayLevel { get; set; }
        RotateFieldDef RotateFieldDef { get; set; }
        ILabelDef LabelDef { get; set; }
        IFeatureRenderer Renderer { get; set; }
        bool IsTwoStepDraw { get; set; }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="mapX">X</param>
        /// <param name="mapY">Y</param>
        /// <param name="Tolerance">容差</param>
        /// <returns></returns>
        string TipText(double mapX, double mapY, double Tolerance);
    }
}
