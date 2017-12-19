using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 20130409：该类已过时，请不要再使用，保留是因为之前有类用到。
    /// 请使用VirtualRasterHelper替代
    /// </summary>
    public class RasterOffsetHelper
    {
        public static bool ComputeBeginEndRowCol(CoordEnvelope oEnvelope, Size oSize, CoordEnvelope tEnvelope, Size tSize,
            ref int oBeginRow, ref int oBeginCol, ref int oEndRow, ref int oEndCol,
            ref int tBeginRow, ref int tBeginCol, ref int tEndRow, ref int tEndCol)
        {
            if (!IsInteractived(oEnvelope, tEnvelope))
                return false;
            double resolutionX = oEnvelope.Width / oSize.Width;
            double resolutionY = oEnvelope.Height / oSize.Height;

            double tResolutionX = tEnvelope.Width / tSize.Width;
            double tResolutionY = tEnvelope.Height / tSize.Height;

            if (oEnvelope.MinX >= tEnvelope.MinX)//左边界在目标图像内部
            {
                oBeginCol = 0;
                tBeginCol = GetInteger((oEnvelope.MinX - tEnvelope.MinX) / resolutionX);
            }
            else//左边界在目标图像外部
            {
                oBeginCol = GetInteger((tEnvelope.MinX - oEnvelope.MinX) / resolutionX);
                tBeginCol = 0;
            }
            if (oEnvelope.MaxX >= tEnvelope.MaxX)//右边界在目标图像外部
            {
                oEndCol = GetInteger((tEnvelope.MaxX - oEnvelope.MinX) / resolutionX);
                tEndCol = tSize.Width;
            }
            else//右边界在目标图像内部
            {
                oEndCol = GetInteger((oEnvelope.MaxX - oEnvelope.MinX) / resolutionX);

                tEndCol = GetInteger((oEnvelope.MaxX - tEnvelope.MinX) / tResolutionY);
            }
            if (oEnvelope.MaxY <= tEnvelope.MaxY)//上边界在目标图像内部
            {
                oBeginRow = 0;
                tBeginRow = GetInteger((tEnvelope.MaxY - oEnvelope.MaxY) / tResolutionY);
            }
            else//上边界在目标边界外部
            {
                oBeginRow = GetInteger((oEnvelope.MaxY - tEnvelope.MaxY) / resolutionY);
                tBeginRow = 0;
            }
            if (oEnvelope.MinY <= tEnvelope.MinY)//下边界在目标图像外部
            {
                oEndRow = GetInteger((oEnvelope.MaxY - tEnvelope.MinY) / resolutionY);
                tEndRow = tSize.Height;
            }
            else//下边界在目标图像内部
            {
                oEndRow = oSize.Height;
                tEndRow = GetInteger((tEnvelope.MaxY - oEnvelope.MinY) / tResolutionY);
            }
            return true;
        }

        public static int GetInteger(double fWidth)
        {
            int v = (int)Math.Round(fWidth);
            if (fWidth - v > 0.9000001)
                v++;
            return v;
        }

        public static bool IsInteractived(CoordEnvelope envelopeA, CoordEnvelope envelopeB)
        {
            RectangleF a = new RectangleF((float)envelopeA.MinX, (float)envelopeA.MinY, (float)envelopeA.Width, (float)envelopeA.Height);
            RectangleF b = new RectangleF((float)envelopeB.MinX, (float)envelopeB.MinY, (float)envelopeB.Width, (float)envelopeB.Height);
            return a.IntersectsWith(b);
        }
    }
}
