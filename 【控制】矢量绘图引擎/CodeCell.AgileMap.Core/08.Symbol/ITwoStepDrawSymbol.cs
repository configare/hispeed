using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// 两阶段绘制符号
    /// 例如：道路
    /// </summary>
    public interface ITwoStepDrawSymbol
    {
        void DrawOutline(Graphics g, GraphicsPath path);
        void Fill(Graphics g, GraphicsPath path);
    }
}
