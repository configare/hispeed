using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.DrawEngine;
using System.Xml.Linq;
using GeoDo.Core;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public interface IContourLayer : IDisposable, ILayer, IActionAtContructAfter
    {
        bool IsFillColor { get; set; }
        bool IsLabel { get; set; }
        string LabelFormat { get; set; }
        bool IsUseCurveRender { get; set; }
        void Apply(ContourLine[] contourLines,ContourClass[] items, bool isLabel, bool isFillColor);
        void Apply(ContourLine[] contourLines,bool isProjected);
        ContourLine[] ContourLines { get; }
        /// <summary>
        /// 保存等值线（视图+数据）
        /// 视图保存为.xml文件
        /// 数据保存为.xml.contour文件
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="isWithDataFile">true:保存数据，否则只保存视图</param>
        void Save(string fname,bool isWithDataFile);
        /// <summary>
        /// 将视图生成为XElment对象(仅视图）
        /// </summary>
        /// <returns></returns>
        XElement ToXml();        
    }
}
