using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
    /// <summary>Represents round rectangle shape.</summary>
	[ToolboxItem(false), ComVisible(false)]
	public class RoundRectShape : ElementShape
	{
        #region Fields
        int radius = 5;

        private bool bottomLeftRounded = true;
        private bool topLeftRounded = true;
        private bool bottomRightRounded = true;
        private bool topRightRounded = true;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the RoundRectShape class.</summary>
        public RoundRectShape()
        {
        }

        /// <summary>Initializes a new instance of the RoundRectShape class.</summary>
        public RoundRectShape(IContainer components)
            : base(components)
        {
        }

        /// <summary>Initializes a new instance of the RoundRectShape class using a radius.</summary>
        public RoundRectShape(int radius)
        {
            this.radius = radius;
        }

        /// <summary>Initializes a new instance of the RoundRectShape class using a radius and rounded corners.</summary>
        public RoundRectShape(int radius, bool topLeftRounded, bool bottomLeftRounded, bool topRightRounded, bool bottomRightRounded)
            : this(radius)
        {
            this.bottomLeftRounded = bottomLeftRounded;
            this.topLeftRounded = topLeftRounded;
            this.bottomRightRounded = bottomRightRounded;
            this.topRightRounded = topRightRounded;
        }
        #endregion

        #region Properties
        /// <summary><para>Gets or sets the radius of the shape.</para></summary>
        [Description("Gets or sets the radius of the shape.")]
        [DefaultValue(5)]
        [Category("Appearance")]
        public int Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        /// <summary>
        /// Gets or sets value indicating whether the bottom left corner of the shape should be round
        /// </summary>
        [DefaultValue(true)]
        public bool BottomLeftRounded
        {
            get { return bottomLeftRounded; }
            set { bottomLeftRounded = value; }
        }

        /// <summary>
        /// Gets or sets value indicating whether top left corner of the shape should be round
        /// </summary>
        [DefaultValue(true)]
        public bool TopLeftRounded
        {
            get { return topLeftRounded; }
            set { topLeftRounded = value; }
        }

        /// <summary>
        /// Gets or sets value indicating whether bottom right corner of the shape should be round
        /// </summary>
        [DefaultValue(true)]
        public bool BottomRightRounded
        {
            get { return bottomRightRounded; }
            set { bottomRightRounded = value; }
        }

        /// <summary>
        /// Gets or sets value indicating whether top right corner of the shape should be round
        /// </summary>
        [DefaultValue(true)]
        public bool TopRightRounded
        {
            get { return topRightRounded; }
            set { topRightRounded = value; }
        } 
        #endregion

        #region Methods
        public override Region CreateRegion(Rectangle bounds)
        {
            return NativeMethods.CreateRoundRectRgn(bounds, this.radius);
        }

        public override GraphicsPath CreatePath(Rectangle bounds)
        {
            return this.CreatePath(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height));
        }

        /// <summary>Greates round rectangle like path.</summary>
        public override GraphicsPath CreatePath(RectangleF bounds)
        {

            GraphicsPath path = new GraphicsPath();

            if (bounds.Height <= 0 || bounds.Width <= 0)
                return path;

            if (this.Radius <= 0.0F)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();

                return path;
            }

            float diameter;
            SizeF sizeF;
            RectangleF arc;

            if (this.Radius >= (Math.Min(bounds.Width, bounds.Height)) / 2.0)
            {
                try
                {
                    if (bounds.Width > bounds.Height)
                    {
                        diameter = bounds.Height;
                        sizeF = new SizeF(diameter, diameter);
                        arc = new RectangleF(bounds.Location, sizeF);
                        //Misho: throw/cach exception can be avioded
                        if (arc.Size != SizeF.Empty)
                        {
                            path.AddArc(arc, 90f, 180f);
                            arc.X = bounds.Right - diameter;
                            path.AddArc(arc, 270f, 180f);
                        }
                        else
                        {
                            path.AddEllipse(bounds);
                        }
                    }
                    else if (bounds.Width < bounds.Height)
                    {
                        diameter = bounds.Width;
                        sizeF = new SizeF(diameter, diameter);
                        arc = new RectangleF(bounds.Location, sizeF);
                        //Misho: throw/cach exception can be avioded
                        if (arc.Size != SizeF.Empty)
                        {
                            path.AddArc(arc, 180f, 180f);
                            arc.Y = bounds.Bottom - diameter;
                            path.AddArc(arc, 0f, 180f);
                        }
                        else
                        {
                            path.AddEllipse(bounds);
                        }
                    }
                    else
                    {
                        path.AddEllipse(bounds);
                    }
                }
                catch (Exception)
                {
                    path.AddEllipse(bounds);
                }
                finally
                {
                    path.CloseFigure();
                }

                return path;
            }

            //path.FillMode = FillMode.Alternate;

            diameter = this.Radius * 2.0F;
            sizeF = new SizeF(diameter, diameter);
            arc = new RectangleF(bounds.Location, sizeF);

            float notRoundDiameter = 1f;
            SizeF notRoundSize = new SizeF(notRoundDiameter, notRoundDiameter);
            RectangleF notRoundArc = new RectangleF(bounds.Location, notRoundSize);

            //angle should be just a libble sharper tfor more accuracy with DrawPath with thicker Pen
            const float angle = 90f;
            //var offset = 0.5f;

            if (this.TopLeftRounded)
            {
                path.AddArc(arc, 180f, angle);
            }
            else
            {
                path.AddArc(notRoundArc, 180f, angle);
            }

            arc.X = bounds.Right - diameter;
            notRoundArc.X = bounds.Right - notRoundDiameter;

            //arc.X -= 2;
            //arc.Width += 2;            
            //arc.Height += 2;

            if (this.TopRightRounded)
            {
                path.AddArc(arc, 270f, angle);
            }
            else
            {
                path.AddArc(notRoundArc, 270f, angle);
            }

            arc.Y = bounds.Bottom - diameter;
            notRoundArc.Y = bounds.Bottom - notRoundDiameter;

            //arc.Y -= 2;

            if (this.BottomRightRounded)
            {
                path.AddArc(arc, 0f, angle);
            }
            else
            {
                path.AddArc(notRoundArc, 0f, angle);
            }

            arc.X = bounds.Left;
            notRoundArc.X = bounds.Left;

            if (this.BottomLeftRounded)
            {
                path.AddArc(arc, 90f, angle);
            }
            else
            {
                path.AddArc(notRoundArc, 90f, angle);
            }

            path.CloseFigure();

            return path;
        }

        /// <summary>Serializes properties. Required for telerik serialization mechanism.</summary>
        public override string SerializeProperties()
        {
            string res = radius.ToString();

            if (!this.BottomLeftRounded ||
                !this.TopLeftRounded ||
                !this.BottomRightRounded ||
                !this.TopRightRounded)
            {
                res += ", " + this.BottomLeftRounded.ToString();
                res += ", " + this.TopLeftRounded.ToString();
                res += ", " + this.BottomRightRounded.ToString();
                res += ", " + this.TopRightRounded.ToString();
            }

            return res;
        }

        /// <summary>Deserializes properties. Required for telerik deserialization mechanism.</summary>
        public override void DeserializeProperties(string propertiesString)
        {
            if (string.IsNullOrEmpty(propertiesString))
            {
                return;
            }

            string[] propParts = propertiesString.Split(',');

            if (propParts.Length > 0)
            {
                radius = int.Parse(propParts[0]);
            }

            if (propParts.Length > 1)
            {
                this.BottomLeftRounded = bool.Parse(propParts[1]);
            }

            if (propParts.Length > 2)
            {
                this.TopLeftRounded = bool.Parse(propParts[2]);
            }

            if (propParts.Length > 3)
            {
                this.BottomRightRounded = bool.Parse(propParts[3]);
            }

            if (propParts.Length > 4)
            {
                this.TopRightRounded = bool.Parse(propParts[4]);
            }
        } 
        #endregion
	}	
}