using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public static class ShapeTypeToString
    {
        public static string GetStringByShapeType(enumShapeType shapeType)
        {
            switch (shapeType)
            { 
                case enumShapeType.Point:
                    return "点";
                case enumShapeType.Polyline:
                    return "线";
                case enumShapeType.Polygon:
                    return "多边形";
                case enumShapeType.NullShape:
                    return "空";
                case enumShapeType.MixGeomtry:
                    return "混合(点、线、多边形)";
                default:
                    throw new NotImplementedException("几何形状\"" + shapeType.ToString()+"\"到字符串的转换未实现。");
            }
        }
    }
}
