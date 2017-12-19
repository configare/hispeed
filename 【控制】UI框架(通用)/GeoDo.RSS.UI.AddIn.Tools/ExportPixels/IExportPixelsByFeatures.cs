using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    /// <summary>
    /// 像素特征
    /// </summary>
    public class PixelFeatures
    {
        /// <summary>
        /// 矢量点对应的像元索引
        /// </summary>
        public int[] RasterIndexes;
        /// <summary>
        /// 矢量点特征值（例如:观测点水深）
        /// </summary>
        public double[] FeatureValues; 
    }

    /// <summary>
    /// 通过矢量特征导出栅格像元序号
    /// 导出矢量点对应的栅格像元序号和矢量点的某个属性
    /// 例如：导出水深观测点的像元序号、水深
    /// </summary>
    public interface IExportPixelsByFeatures
    {
        ExportPixelsByFeatures.PixelFeatures Export(Envelope geoEnvelope, Size rasterSize, string shpFile, string fieldName, Func<Feature,double, bool> filter);
    }
}