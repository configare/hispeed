#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-09-26 18:46:13
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.FileProject
{
    /// <summary>
    /// 类名：Block
    /// 属性描述：
    /// 创建者：admin   创建日期：2013-09-26 18:46:13
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class Block : ICloneable
    {
        public int xOffset;
        public int xEnd;
        public int yBegin;
        public int yEnd;

        public Block()
        { }

        public Block(int _xOffset, int _xEnd, int _yBegin, int _yEnd)
        {
            this.xOffset = _xOffset;
            this.xEnd = _xEnd;
            this.yBegin = _yBegin;
            this.yEnd = _yEnd;
        }

        public int Width
        {
            get { return xEnd - xOffset + 1; }
        }

        public int Height
        {
            get { return yEnd - yBegin + 1; }
        }

        public Size Size
        {
            get { return new Size(Width, Height); }
        }

        public object Clone()
        {
            return new Block(xOffset, xEnd, yBegin, yEnd);
        }

        internal Block Zoom(int xZoom, int yZoom)
        {
            return new Block(xOffset * xZoom, xEnd * xZoom, yBegin * yZoom, yEnd * yZoom);
        }

        public static Block Empty
        {
            get
            {
                return new Block(0, -1, 0, -1);
            }
        }
    }
}
