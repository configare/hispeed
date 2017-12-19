using System;
using System.Collections.Generic;
using System.Text;
//using Telerik.WinControls.Design;
using Telerik.WinControls.Paint;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Telerik.WinControls.Enumerations;
using System.ComponentModel;
using System.Diagnostics;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.Primitives
{
    /// <summary><para>Represents a check box primitive that is drawn on the screen.</para></summary>
    public class CheckPrimitive : RadioPrimitive
    {
        #region Dependency properties

        public static RadProperty CheckPrimitiveStyleProperty =
            RadProperty.Register("CheckPrimitiveStyle", typeof(CheckPrimitiveStyleEnum), typeof(CheckPrimitive),
               new RadElementPropertyMetadata(CheckPrimitiveStyleEnum.XP, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty DrawFillProperty =
            RadProperty.Register("DrawFill", typeof(bool), typeof(CheckPrimitive),
                new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));
       
        #endregion

        #region Properties

        ///<summary>
        /// Gets or sets a value indicating the style of the check box primitive.
        ///</summary>
        [RadPropertyDefaultValue("CheckPrimitiveStyle", typeof(CheckPrimitive))]
        [Description("Gets or sets a value indicating the style of the check box primitive.")]
        public CheckPrimitiveStyleEnum CheckPrimitiveStyle
        {
            get
            {
                return (CheckPrimitiveStyleEnum)this.GetValue(CheckPrimitiveStyleProperty);
            }
            set
            {
                this.SetValue(CheckPrimitiveStyleProperty, value);
            }
        }

        ///<summary>
        /// Gets or sets a value indicating whether to draw the background.
        ///</summary>
        [RadPropertyDefaultValue("DrawFill", typeof(CheckPrimitive))]
        [Description("Gets or sets a value indicating whether to draw the background.")]
        public bool DrawFill
        {
            get
            {
                return (bool)this.GetValue(DrawFillProperty);
            }
            set
            {
                this.SetValue(DrawFillProperty, value);
            }
        }

        #endregion              

        #region Painting

        public override void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
        {
            if (DrawFill)
            {
                base.PaintPrimitive(graphics, angle, scale);
            }

            RectangleF rect = new RectangleF(this.Padding.Left, this.Padding.Top, 
                this.Size.Width - this.Padding.Horizontal, this.Size.Height - this.Padding.Vertical);
			PointF center = new PointF(rect.Width / 2, rect.Height / 2);
            float kStyle = (float)CheckPrimitiveStyle;
            float kStyleVertical = kStyle != 0.0f ? 1f : 0f;

            PointF[] points = null;
            for (int i = 0; i < 3; i++)
            {
                using (GraphicsPath graphPath = new GraphicsPath())
                {
                    switch (CheckPrimitiveStyle)
                    {
                        case CheckPrimitiveStyleEnum.XP:
                            points =
                                new PointF[] 
                        {
					    	new PointF(center.X - 3, center.Y + 1 - i),
    						new PointF(center.X - 1, center.Y + 3 - i),
						    new PointF(center.X + 3, center.Y - 1 - i )
					    };
                            break;
                        case CheckPrimitiveStyleEnum.Vista:
                            points =
                                new PointF[] 
                        {
					    	new PointF(center.X - 4 , center.Y - i ),
    						new PointF(center.X - 1, center.Y + 3 - i),
						    new PointF(center.X + 3 + 1*0.8f, center.Y - i - (1*3f) )
					    };
                            break;
                        case CheckPrimitiveStyleEnum.Mac:
                            points =
                               new PointF[] 
                        {
					    	new PointF(center.X - 4, center.Y - i ),
    						new PointF(center.X - 1, center.Y + 3 - i),
						    new PointF(center.X + 4 + 2*0.8f, center.Y - i - (2*4f) )
					    };
                            break;
                        default:
                            return;
                    }
                    graphPath.AddLines(points);
                    graphics.DrawPath(graphPath, this.ForeColor, 0, 1);
                }                
            }
        }

        protected override Rectangle GetClientRect()
        {
            return new Rectangle(this.Padding.Left, this.Padding.Top, 
                this.Size.Width - this.Padding.Horizontal, this.Size.Height - this.Padding.Vertical);
        }

        #endregion
    }
}