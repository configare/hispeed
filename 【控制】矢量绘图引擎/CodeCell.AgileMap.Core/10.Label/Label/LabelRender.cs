using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class LabelRender : ILabelRender
    {
        [NonSerialized]
        private PointF pointF = new PointF();
        private IConflictor _conflictor = null;
        private IFeatureRenderEnvironment _environment = null;
        internal static StringFormat StaticTextStringFormat = StringFormat.GenericDefault;

        static LabelRender()
        {
            StaticTextStringFormat.Alignment = StringAlignment.Center;
            StaticTextStringFormat.LineAlignment = StringAlignment.Center;
        }

        //
        #region ILabelRender Members

        public void Begin(IConflictor conflictor, object environment)
        {
            _conflictor = conflictor;
            _environment = environment as IFeatureRenderEnvironment;
        }

        public void Draw(Matrix emptyRotateMatrix, Graphics g, LabelDef labelDef, ISymbol currentSymbol, Feature fet, QuickTransformArgs tranArgs)
        {
            string text = fet.GetFieldValue(labelDef.Fieldname);
            if (string.IsNullOrEmpty(text) || labelDef.InvalidValue.Equals(text))
                return;
            Matrix oldMatrix = g.Transform;
            LabelLocation[] locs = GetLabelLocations(fet, labelDef);
            if (locs == null || locs.Length == 0)
                return;
            foreach (LabelLocation loc in locs)
            {
                //标注点在可视区域外
                if (!_environment.ExtentOfProjectionCoord.IsContains(loc.Location))
                    continue;
                //
                pointF.X = (float)(loc.Location.X * tranArgs.kLon + tranArgs.bLon);
                pointF.Y = (float)(loc.Location.Y * tranArgs.kLat + tranArgs.bLat);
                #region debug
                //符号中心点
                //(_environment.ConflictorForLabel as PixelConflictor).DrawTestRectangleF(new PointF(pointF.X - 1, pointF.Y - 1), new SizeF(2, 2), Color.Blue);
                #endregion
                PointF oldPt = pointF;
                SizeF fontSize = g.MeasureString(text, labelDef.LabelFont);
                //计算标注矩形区域
                SizeF labelRect = SizeF.Empty;
                if (labelDef.AutoToNewline && text.Length > labelDef.CharcountPerLine)
                {
                    int pos = (int)Math.Ceiling(text.Length / 2f);
                    labelRect = g.MeasureString(text.Substring(0, pos), labelDef.LabelFont);
                    labelRect.Height *= 2;
                    labelRect.Height = (float)Math.Ceiling(labelRect.Height);
                }
                else
                {
                    labelRect = g.MeasureString(text, labelDef.LabelFont);
                }
                //应用冲突检测
                masLabelPosition currentpos = labelDef.MasLabelRuler;
                if (_conflictor != null && _conflictor.Enabled)
                {
                    bool isConflicted = true;
                    PointF originalPt = pointF;
                    #region debug
                    //if (Array.IndexOf(fet.FieldValues, "东芝笔记本电脑技术服务中心") >= 0 || Array.IndexOf(fet.FieldValues, "甘托克") >= 0)
                    //    Console.WriteLine("");

                    //画出8个候选位置
                    //foreach (masLabelPosition pos in Enum.GetValues(typeof(masLabelPosition)))
                    //{
                    //    switch (pos)
                    //    {
                    //        case masLabelPosition.LeftCenter:
                    //        case masLabelPosition.RightCenter:
                    //        case masLabelPosition.UpCenter:
                    //        case masLabelPosition.BottomCenter:
                    //            pointF = originalPt;
                    //            GetLabelLocation(pos, g, ref  pointF, labelRect, currentSymbol);
                    //            (_environment.ConflictorForLabel as PixelConflictor).DrawTestRectangleF(pointF, labelRect, Color.Blue);
                    //            break;
                    //    }
                    //}
                    #endregion
                    foreach (masLabelPosition pos in Enum.GetValues(typeof(masLabelPosition)))
                    {
                        pointF = originalPt;
                        GetLabelLocation(pos, g, ref  pointF, labelRect, currentSymbol);
                        if (!_conflictor.IsConflicted(pointF, labelRect))
                        {
                            isConflicted = false;
                            currentpos = pos;
                            break;
                        }
                    }
                    if (isConflicted)
                        return;
                }
                else
                {
                    //应用对齐方式
                    GetLabelLocation(labelDef.MasLabelRuler, g, ref  pointF, fontSize, currentSymbol);
                }
                //
                if (loc.Angle != 0 && loc.Angle != 360)//一般情况下是沿线标注
                {
                    emptyRotateMatrix.Reset();
                    emptyRotateMatrix.RotateAt(loc.Angle, oldPt);
                    g.Transform = emptyRotateMatrix;

                    if (_conflictor != null)
                        _conflictor.HoldPosition(pointF.X, pointF.Y, labelRect, emptyRotateMatrix);
                }
                //背景符号
                if (labelDef.ContainerSymbol != null)
                {
                    SizeF backSize = labelDef.ContainerSymbol.Draw(g, pointF, fontSize);
                    if (backSize.Width - fontSize.Width > 0)
                        pointF.X += (backSize.Width - fontSize.Width) / 2f;
                    if (backSize.Height - fontSize.Height > 0)
                        pointF.Y += (backSize.Height - fontSize.Height) / 2f;
                }
                //写标注
                DrawString(text, g, pointF, labelRect, labelDef);
                //
                if (loc.Angle != 0)
                {
                    g.Transform = oldMatrix;
                }
            }
        }

        private void DrawString(string text, Graphics g, PointF pointF, SizeF labelRect, LabelDef labelDef)
        {
            RectangleF rect = new RectangleF(pointF.X, pointF.Y, labelRect.Width, labelRect.Height);
            if (labelDef.Angle != 0)
            {
                Matrix oldMatrix = g.Transform;
                try
                {
                    Matrix matrix = g.Transform.Clone();
                    Matrix newMatrix = new Matrix();
                    newMatrix.RotateAt(labelDef.Angle, new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2));
                    newMatrix.Multiply(matrix, MatrixOrder.Append);
                    g.Transform = newMatrix;
                    DrawString(rect, text, g, pointF, labelRect, labelDef);
                    matrix.Dispose();
                }
                finally 
                {
                    g.Transform.Dispose();
                    g.Transform = oldMatrix;
                }
            }
            else
            {
                DrawString(rect, text, g, pointF, labelRect, labelDef);
            }
        }

        private void DrawString(RectangleF rect,string text, Graphics g, PointF pointF, SizeF labelRect, LabelDef labelDef)
        {
            if (labelDef.MaskBrush != null)
            {
                rect.Location = new PointF(pointF.X + 1, pointF.Y + 1);
                g.DrawString(text, labelDef.LabelFont, labelDef.MaskBrush, rect, StaticTextStringFormat);
                rect.Location = new PointF(pointF.X + 1, pointF.Y - 1);
                g.DrawString(text, labelDef.LabelFont, labelDef.MaskBrush, rect, StaticTextStringFormat);
                rect.Location = new PointF(pointF.X + 1, pointF.Y);
                g.DrawString(text, labelDef.LabelFont, labelDef.MaskBrush, rect, StaticTextStringFormat);
                rect.Location = new PointF(pointF.X - 1, pointF.Y + 1);
                g.DrawString(text, labelDef.LabelFont, labelDef.MaskBrush, rect, StaticTextStringFormat);
                rect.Location = new PointF(pointF.X - 1, pointF.Y - 1);
                g.DrawString(text, labelDef.LabelFont, labelDef.MaskBrush, rect, StaticTextStringFormat);
                rect.Location = new PointF(pointF.X - 1, pointF.Y);
                g.DrawString(text, labelDef.LabelFont, labelDef.MaskBrush, rect, StaticTextStringFormat);
                rect.Location = new PointF(pointF.X, pointF.Y + 1);
                g.DrawString(text, labelDef.LabelFont, labelDef.MaskBrush, rect, StaticTextStringFormat);
                rect.Location = new PointF(pointF.X, pointF.Y - 1);
                g.DrawString(text, labelDef.LabelFont, labelDef.MaskBrush, rect, StaticTextStringFormat);
            }
            rect.Location = new PointF(pointF.X, pointF.Y);
            g.DrawString(text, labelDef.LabelFont, labelDef.LabelBrush, rect, StaticTextStringFormat);
        }

        internal static void DrawStringWithBorder(string text, Graphics g, PointF pointF, Font font, Brush fontBrush, Brush maskBrush)
        {
            g.DrawString(text, font, maskBrush, pointF.X, pointF.Y);
            g.DrawString(text, font, maskBrush, pointF.X + 1, pointF.Y - 1);
            g.DrawString(text, font, maskBrush, pointF.X + 1, pointF.Y);
            g.DrawString(text, font, maskBrush, pointF.X - 1, pointF.Y + 1);
            g.DrawString(text, font, maskBrush, pointF.X - 1, pointF.Y - 1);
            g.DrawString(text, font, maskBrush, pointF.X - 1, pointF.Y);
            g.DrawString(text, font, maskBrush, pointF.X, pointF.Y + 1);
            g.DrawString(text, font, maskBrush, pointF.X, pointF.Y - 1);
            //
            g.DrawString(text, font, fontBrush, pointF.X, pointF.Y);
        }

        private LabelLocation[] GetLabelLocations(Feature fet, ILabelDef labelDef)
        {
            switch (labelDef.LabelSource)
            {
                case enumLabelSource.Label:
                    if (fet.LabelLocationService == null)
                        return null;
                    return fet.LabelLocationService.LabelLocations;
                case enumLabelSource.Annotation:
                    return fet.Annotations;
                default:
                    return null;
            }
        }

        private void GetLabelLocation(masLabelPosition position, Graphics g, ref PointF pointF, SizeF fontSize, ISymbol currentSymbol)
        {
            if (currentSymbol != null && !currentSymbol.SymbolSize.IsEmpty)
            {
                pointF.X -= fontSize.Width / 2f;
                pointF.Y -= fontSize.Height / 2f;
                SizeF symSize = currentSymbol.SymbolSize;
                int diff = 2;
                switch (position)
                {
                    case masLabelPosition.RightCenter:
                        pointF.X = pointF.X + fontSize.Width / 2 + symSize.Width / 2f + diff;
                        break;
                    case masLabelPosition.LeftCenter:
                        pointF.X = pointF.X - fontSize.Width / 2 - symSize.Width / 2f - diff;
                        break;
                    case masLabelPosition.BottomCenter:
                        pointF.Y = pointF.Y + fontSize.Height / 2 + symSize.Height / 2f + diff;
                        break;
                    case masLabelPosition.UpCenter:
                        pointF.Y = pointF.Y - fontSize.Height / 2 - symSize.Height / 2f - diff;
                        break;
                    case masLabelPosition.RightUpCorner:
                        pointF.X = pointF.X + fontSize.Width / 2 + symSize.Width / 2f;
                        pointF.Y = pointF.Y - fontSize.Height / 2 - symSize.Height / 2f;
                        break;
                    case masLabelPosition.RightDownCorner:
                        pointF.X = pointF.X + fontSize.Width / 2 + symSize.Width / 2f;
                        pointF.Y = pointF.Y + fontSize.Height / 2 + symSize.Height / 2f;
                        break;
                    case masLabelPosition.LeftUpCorner:
                        pointF.X = pointF.X - fontSize.Width / 2 - symSize.Width / 2f;
                        pointF.Y = pointF.Y - fontSize.Height / 2 - symSize.Height / 2f;
                        break;
                    case masLabelPosition.LeftDownCorner:
                        pointF.X = pointF.X - fontSize.Width / 2 - symSize.Width / 2f;
                        pointF.Y = pointF.Y + fontSize.Height / 2 + symSize.Height / 2f;
                        break;
                }
            }
            else
            {
                if (position == masLabelPosition.Center)
                {
                    pointF.X = pointF.X - fontSize.Width / 2;
                    pointF.Y = pointF.Y - fontSize.Height / 2;
                }
            }
        }

        #endregion
    }
}
