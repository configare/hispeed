using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 描述两个栅格之间的坐标关系。
    /// 这里的偏移，都是统一化同一个分辨率下后的，
    /// 偏移计算都是基于栅格坐标的（即像素坐标，左上角为坐标原点）
    /// </summary>
    public class RasterRectOffset
    {
        public float ResolutionXScale;
        public float ResolutionYScale;

        /// <summary>
        /// 当前目标访问区域坐标（基准坐标）
        /// </summary>
        public Rectangle rectDst;
        /// <summary>
        /// 待读取数据相对于基准坐标的位置
        /// </summary>
        public Rectangle srcInDst;
        /// <summary>
        /// 待读取数据和目标区域的相交区域坐标（相对于基准坐标）
        /// </summary>
        public Rectangle intInDst;
        /// <summary>
        /// 待读取数据和目标区域的相交区域坐标（相对于待读取数据坐标）用于读取数据使用
        /// </summary>
        public Rectangle intInSrc;

        /// <summary>
        /// 计算进一步偏移分块后的偏移结果
        /// 传入的参数都是对目标的偏移。
        /// </summary>
        /// <param name="vOffsetX"></param>
        /// <param name="vOffsetY"></param>
        /// <param name="vSizex"></param>
        /// <param name="vSizey"></param>
        /// <returns></returns>
        internal RasterRectOffset Offset(int vOffsetX, int vOffsetY, int vSizex, int vSizey)
        {
            //新的坐标基准
            Rectangle newRectDst = new Rectangle(0, 0, vSizex, vSizey);
            int left = srcInDst.Left - vOffsetX;
            int top = srcInDst.Top - vOffsetY;
            int width = srcInDst.Width;
            int height = srcInDst.Height;
            Rectangle newsrcInDst = new Rectangle(left, top, width, height);
            Rectangle newintInDst = Rectangle.Intersect(newRectDst, newsrcInDst);
            Rectangle newintInSrc = new Rectangle(newintInDst.Left - newsrcInDst.Left, newintInDst.Top - newsrcInDst.Top, newintInDst.Width, newintInDst.Height);

            RasterRectOffset off = new RasterRectOffset();
            off.rectDst = newRectDst;
            off.srcInDst = newsrcInDst;
            off.intInDst = newintInDst;
            off.intInSrc = newintInSrc;
            off.ResolutionXScale = this.ResolutionXScale;
            off.ResolutionYScale = this.ResolutionYScale;
            return off;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oEnvelope"></param>
        /// <param name="oSize"></param>
        /// <param name="tEnvelope"></param>
        /// <param name="tSize"></param>
        /// <param name="rect">源栅格的与目标栅格相交区域的坐标位置（相对于源栅格,左上为原点）</param>
        /// <param name="rectPadding">读取后的栅格（rect）四边的间距(相对目标栅格)</param>
        /// <returns></returns>
        public static bool ComputeBeginEndRowCol(
            CoordEnvelope oEnvelope, Size oSize, PointF oReslution,
            CoordEnvelope tEnvelope, Size tSize, PointF tResolution,
            out Rectangle rectDst, out Rectangle srcInDst,
            out Rectangle intInDst, out Rectangle intInSrc)
        {
            rectDst = new Rectangle();
            srcInDst = new Rectangle();
            intInDst = new Rectangle();
            intInSrc = new Rectangle();
            if (!IsInteractived(oEnvelope, tEnvelope))
                return false;
            CoordEnvelope xj = oEnvelope.Intersect(tEnvelope);
            //double resolutionX = tEnvelope.Width / tSize.Width;
            //double resolutionY = tEnvelope.Height / tSize.Height;

            //目标坐标(标准坐标)
            rectDst = new Rectangle(0, 0, tSize.Width, tSize.Height);
            int srcLeft = GetInteger((oEnvelope.MinX - tEnvelope.MinX) / tResolution.X);
            int srcTop = GetInteger((tEnvelope.MaxY - oEnvelope.MaxY) / tResolution.Y);
            int width = GetInteger(oSize.Width * oReslution.X / tResolution.X);
            int height = GetInteger(oSize.Height * oReslution.Y / tResolution.Y);

            //源数据坐标
            srcInDst = new Rectangle(srcLeft, srcTop, width, height);
            //相交坐标(有数据区域坐标,数据在目标区域位置，用于填充数据)
            intInDst = Rectangle.Intersect(rectDst, srcInDst);
            //相交区域在源数据中的坐标，数据在所在文件区域位置,用于读取数据。
            intInSrc = new Rectangle(intInDst.Left - srcInDst.Left, intInDst.Top - srcInDst.Top, intInDst.Width, intInDst.Height);
            return true;
        }

        public static int GetInteger(double fWidth)
        {
            int v = (int)Math.Round(fWidth);
            if (fWidth - v > 0.9000001)
                v++;
            return v;
        }

        public static int GetWHInteger(double fWidth)
        {
            int v = (int)Math.Round(fWidth);
            if (fWidth - v > 0.9000001)
                v++;
            if (v == 0)
                v = 1;
            return v;
        }

        public static bool IsInteractived(CoordEnvelope envelopeA, CoordEnvelope envelopeB)
        {
            RectangleF a = new RectangleF((float)envelopeA.MinX, (float)envelopeA.MinY, (float)envelopeA.Width, (float)envelopeA.Height);
            RectangleF b = new RectangleF((float)envelopeB.MinX, (float)envelopeB.MinY, (float)envelopeB.Width, (float)envelopeB.Height);
            return a.IntersectsWith(b);
        }

        public static Size GetRasterSize(CoordEnvelope envelope, float resolutionX, float resolutionY)
        {
            int width = GetInteger(envelope.Width / resolutionX);
            int height = GetInteger(envelope.Height / resolutionY);
            return new Size(width, height);
        }
    }
}
