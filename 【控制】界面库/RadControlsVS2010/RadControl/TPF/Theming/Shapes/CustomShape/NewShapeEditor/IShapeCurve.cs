using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    public interface IShapeCurve
    {
        ShapePoint[] Points
        {
            get;
        }

        ShapePoint FirstPoint
        {
            get;
            set;
        }

        ShapePoint LastPoint
        {
            get;
            set;
        }
        
        ShapePoint[] Extensions
        {
            get;
        }

        PointF SnappedPoint
        {
            get;
        }

        ShapePoint SnappedCtrlPoint
        {
            get;
        }

        bool IsModified
        {
            get;
        }

        ShapePoint[] ControlPoints
        {
            get;
        }

        List<PointF> TestPoints
        {
            get;
        }

        /// <summary>
        /// Should snap to the line or curve
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="snapDistance"></param>
        /// <returns></returns>
        bool SnapToCurve(PointF pt, float snapDistance);
        bool SnapToCurveExtension(PointF pt, float snapDistance);
        bool SnapToCtrlPoints(PointF pt, float snapDistance);
        bool SnapToHorizontal(PointF pt, float xVal, float snapDistance);
        bool SnapToVertical(PointF pt, float yVal, float snapDistance);
        bool Update();

        IShapeCurve Create();
    }
}
