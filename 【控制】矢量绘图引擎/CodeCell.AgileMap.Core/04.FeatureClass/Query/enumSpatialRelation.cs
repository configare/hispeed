using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public enum enumSpatialRelation
    {
        /// <summary>
        /// 输入的矩形包含要素
        /// </summary>
        Contains,
        /// <summary>
        /// 输入的矩形与要素不相交
        /// </summary>
        Disjoint,
        /// <summary>
        /// 输入的矩形与要素相交(包含包含关系)
        /// </summary>
        Intersect,
        /// <summary>
        /// 输入的矩形被要素包含
        /// </summary>
        Within
    }
}
