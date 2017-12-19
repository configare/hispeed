using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    [ToolboxItem(false), ComVisible(false)]
    public class SeparatorElement : LightVisualElement
    {
        #region Fields
        LinePrimitive line1;
        LinePrimitive line2;
        Orientation orientation;

        #endregion

        #region RadProperties

        public static RadProperty ShadowOffsetProperty = RadProperty.Register(
            "ShadowOffset", typeof(Point), typeof(SeparatorElement), new RadElementPropertyMetadata(
                Point.Empty, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsArrange));

        public static RadProperty ShowShadowProperty = RadProperty.Register(
            "ShowShadow", typeof(bool), typeof(SeparatorElement), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        #endregion

        #region Properties

        public LinePrimitive Line1
        {
            get
            {
                return line1;
            }
        }

        public LinePrimitive Line2
        {
            get
            {
                return line2;
            }
        }

        public Orientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                if (this.orientation != value)
                {
                    this.orientation = value;
                    this.InvalidateMeasure();
                    OnNotifyPropertyChanged(new PropertyChangedEventArgs("Orientation"));
                }
            }
        }

        public Point ShadowOffset
        {
            get
            {
                return (Point)GetValue(ShadowOffsetProperty);
            }
            set
            {
                SetValue(ShadowOffsetProperty, value);
            }
        }

        public bool ShowShadow
        {
            get
            {
                return (bool)GetValue(ShowShadowProperty);
            }
            set
            {
                SetValue(ShowShadowProperty, value);
            }
        }

        #endregion

        #region Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.DrawFill = false;
            this.DrawBorder = false;
        }

        protected override void CreateChildElements()
        {            
            line1 = new LinePrimitive();
            line1.Class = "Line1";
            line1.BackColor = Color.Black;
            line1.GradientStyle = GradientStyles.Solid;
            line1.LineWidth = 1;
            line1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            this.Children.Add(line1);

            line2 = new LinePrimitive();
            line2.Class = "Line2";
            line2.BackColor = Color.LightGray;
            line2.GradientStyle = GradientStyles.Solid;
            line2.LineWidth = 1;
            line2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            this.Children.Add(line2);
        }

        static SeparatorElement()
        {
            new Themes.ControlDefault.RadSeparator().DeserializeTheme();
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(SeparatorElement));
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            if (ShowShadow)
            {
                line2.Visibility = ElementVisibility.Visible;
            }
            else
            {
                line2.Visibility = ElementVisibility.Collapsed;
            }

            SizeF desiredSize = SizeF.Empty;

            if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                foreach (RadElement element in this.Children)
                {
                    element.Measure(availableSize);
                    desiredSize.Height += element.DesiredSize.Height;
                    desiredSize.Width = Math.Max(desiredSize.Width, element.DesiredSize.Width);
                }
            }
            else
            {
                foreach (RadElement element in this.Children)
                {
                    element.Measure(availableSize);
                    desiredSize.Width += element.DesiredSize.Width;
                    desiredSize.Height = Math.Max(desiredSize.Height, element.DesiredSize.Height);
                }
            }

            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);

            if (this.orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                ArrangeHorizontalSeparatorLines(clientRect);
            }
            else
            {
                ArrangeVerticalSeparatorLines(clientRect);
            }
            return finalSize;
        }

        private void ArrangeVerticalSeparatorLines(RectangleF clientRect)
        {
            line1.SeparatorOrientation = line2.SeparatorOrientation = SepOrientation.Vertical;
            float contentWidth = line1.DesiredSize.Height;
            float positiveOffsetY = Math.Abs(ShadowOffset.Y);

            if (ShowShadow)
            {
                contentWidth += (line2.DesiredSize.Height + ShadowOffset.X);
            }

            float y = 0f;
            if (ShadowOffset.Y < 0)
            {
                y = positiveOffsetY;
            }

            float x = (clientRect.Width - contentWidth) / 2f;
            float height = clientRect.Height - positiveOffsetY;

            if (float.IsPositiveInfinity(height))
            {
                height = 100;
            }

            float arrangeX = clientRect.X + x;
            float arrangeY = clientRect.Y + y;
            float arrangeWidth = line1.LineWidth;
            float arrangeHeight = height;
            RectangleF arrangeRectangle = new RectangleF(arrangeX, arrangeY, arrangeWidth, arrangeHeight);

            line1.Arrange(arrangeRectangle);

            if (ShowShadow)
            {
                arrangeX = line1.BoundingRectangle.X + line1.LineWidth + ShadowOffset.X;
                arrangeY = line1.BoundingRectangle.Y + ShadowOffset.Y;
                arrangeWidth = line2.LineWidth;
                arrangeHeight = height;
                arrangeRectangle = new RectangleF(arrangeX, arrangeY, arrangeWidth, arrangeHeight);

                line2.Arrange(arrangeRectangle);
            }
        }

        private void ArrangeHorizontalSeparatorLines(RectangleF clientRect)
        {
            line1.SeparatorOrientation = line2.SeparatorOrientation = SepOrientation.Horizontal;
            float contentHeight = line1.DesiredSize.Height;
            float positiveOffsetX = Math.Abs(ShadowOffset.X);

            if (ShowShadow)
            {
                contentHeight += (line2.DesiredSize.Height + ShadowOffset.Y);
            }

            float x = 0f;
            if (ShadowOffset.X < 0)
            {
                x = positiveOffsetX;
            }

            float y = (clientRect.Height - contentHeight) / 2f;
            float width = clientRect.Width - positiveOffsetX;

            if (float.IsPositiveInfinity(width))
            {
                width = 100;
            }
            float arrangeX = clientRect.X + x;
            float arrangeY = clientRect.Y + y;
            float arrangeWidth = width;
            float arrangeHeight = line1.LineWidth;
            RectangleF arrangeRectangle = new RectangleF(arrangeX, arrangeY, arrangeWidth, arrangeHeight);

            line1.Arrange(arrangeRectangle);

            if (ShowShadow)
            {
                arrangeX = line1.BoundingRectangle.X + ShadowOffset.X;
                arrangeY = line1.BoundingRectangle.Y + line1.LineWidth + ShadowOffset.Y;
                arrangeWidth = width;
                arrangeHeight = line2.LineWidth;
                arrangeRectangle = new RectangleF(arrangeX, arrangeY, arrangeWidth, arrangeHeight);

                line2.Arrange(arrangeRectangle);
            }
        }

        #endregion
    }
}
