using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
   public enum enumElementAligment
    {
        Left,
        Right,
        Top,
        Bottom,
       /// <summary>
       /// 左右居中
       /// </summary>
       LeftRightMid ,
       /// <summary>
       /// 上下居中
       /// </summary>
       TopBottomMid,
       /// <summary>
       /// 横向分布
       /// </summary>
       Horizontal,
       /// <summary>
       /// 纵向分布
       /// </summary>
       Vertical,
       /// <summary>
       /// 向右旋转90度
       /// </summary>
       RotateRight90,
       /// <summary>
       /// 向左旋转90度
       /// </summary>
       RotateLeft90,
       /// <summary>
       /// 垂直翻转
       /// </summary>
       RotateVertical,
       /// <summary>
       /// 水平翻转
       /// </summary>
       RotateHorizontal,
       /// <summary>
       /// 横向填充
       /// </summary>
       HorizontalStrech,
       /// <summary>
       /// 纵向填充
       /// </summary>
       VerticalStrech
    }
}
