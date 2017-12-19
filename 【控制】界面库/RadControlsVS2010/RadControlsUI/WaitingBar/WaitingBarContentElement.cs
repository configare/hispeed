using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Telerik.WinControls.Data;
using Telerik.WinControls.Enumerations;
using System.Collections;

namespace Telerik.WinControls.UI
{
    public class WaitingBarContentElement: LightVisualElement
    {
        
        #region Fields
        
        protected WaitingBarSeparatorElement separatorElement;
        protected WaitingBarTextElement textElement;
        protected ProgressOrientation waitingDirection;
        protected bool waitingFirstRun; 
        protected float offset;
        protected bool isWaiting;
        protected bool isBackwards;
        public WaitingIndicatorCollection indicators;

        #endregion

        #region RadProperties
        
        public static RadProperty WaitingStyleProperty = RadProperty.Register("WaitingStyle", typeof(WaitingBarStyles),
          typeof(WaitingBarContentElement), new RadElementPropertyMetadata(
          WaitingBarStyles.Indeterminate, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets a collection of <see cref="WaitingBarIndicatorElement"/> elements
        /// which contains all waiting indicators of RadWaitingBar
        /// </summary>
        [Browsable(false),
        Description("Gets a collection of WaitingBarIndicatorElement elements which contains all waiting indicators of RadWaitingBar"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WaitingIndicatorCollection Indicators
        {
            get { return this.indicators; }
        }

        /// <summary>
        /// Gets an instance of the <see cref="WaitingBarTextElement"/> class
        /// that represents the waiting bar text element
        /// </summary>
        [Browsable(false), 
        Description("Gets an instance of the WaitingBarTextElement class which represents the waiting bar text element"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WaitingBarTextElement TextElement
        {
            get { return this.textElement; }
        }

        /// <summary>
        /// Gets an instance of the <see cref="WaitingBarSeparatorElement"/> class
        /// that represents the waiting bar separator element
        /// </summary>
        [Browsable(false), 
        Description("Gets an instance of the WaitingBarSeparatorElement class which represents the waiting bar separator element"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WaitingBarSeparatorElement SeparatorElement
        {
            get { return this.separatorElement; }
        }

        /// <summary>
        /// Gets and sets the direction of waiting, e.g.
        /// the Right value moves the indicator from left to right
        /// Range: Bottom, Left, Right, Top
        /// </summary>
        [DefaultValue(ProgressOrientation.Right),
        Description("Gets and sets the direction of waiting")]
        public ProgressOrientation WaitingDirection
        {
            get
            {
                return this.waitingDirection;
            }
            set
            {
                this.waitingDirection = value;
                bool isVertical = IsVertical();
                if (isVertical)
                {
                    this.separatorElement.ProgressOrientation = ProgressOrientation.Top;
                }
                else
                {
                    this.separatorElement.ProgressOrientation = ProgressOrientation.Right;
                }
            }
        }

        /// <summary>
        /// Indicates whether the element is currently waiting
        /// </summary>
        [Description("Indicates whether the element is currently waiting")]
        public bool IsWaiting
        {
            get
            {
                return this.isWaiting;
            }
            set
            {
                this.isWaiting = value;
            }
        }

        /// <summary>
        /// Sets the style of the WaitingBarElement 
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Sets the style of the WaitingBarElement")]
        [DefaultValue(WaitingBarStyles.Indeterminate)]
        public WaitingBarStyles WaitingStyle
        {
            get
            {
                return (WaitingBarStyles)this.GetValue(WaitingStyleProperty);
            }
            set
            {
                this.SetValue(WaitingStyleProperty, value);
                if (this.WaitingStyle == WaitingBarStyles.Dash)
                {
                    this.SeparatorElement.Dash = true;
                }
                else
                {
                    this.SeparatorElement.Dash = false;
                }
            }
        }
        
        #endregion

        #region Initialization
        
        public WaitingBarContentElement()
        {
            offset = 0;
            this.waitingDirection = ProgressOrientation.Right;
            Indicators.CollectionChanged += new NotifyCollectionChangedEventHandler(Indicators_CollectionChanged);
        }

        protected void UpdateIndicatorStretch(WaitingBarIndicatorElement indicator)
        {
            bool isVertical = IsVertical();
            if (isVertical)
            {
                indicator.StretchHorizontally = true;
                indicator.StretchVertically = false;
            }
            else
            {
                indicator.StretchHorizontally = false;
                indicator.StretchVertically = true;
            }
        }

        protected void UpdateElementsState(WaitingBarIndicatorElement indicator)
        {
            bool isVertical = IsVertical();
            indicator.SetValue(WaitingBarIndicatorElement.IsVerticalProperty, isVertical);
            indicator.SeparatorElement.SetValue(WaitingBarSeparatorElement.IsVerticalProperty, isVertical);
        }

        void Indicators_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                IList list = e.NewItems;
                for (int i = 0; i < list.Count; i++)
                {
                    WaitingBarIndicatorElement indicator = (WaitingBarIndicatorElement)list[i];
                    this.Children.Add(indicator);
                    UpdateElementsState(indicator);
                    UpdateIndicatorStretch(indicator);
                }
            }

            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                IList list = e.NewItems;
                for (int i = 0; i < list.Count; i++)
                {
                    ((WaitingBarIndicatorElement)list[i]).Invalidate();
                    this.Children.Remove((WaitingBarIndicatorElement)list[i]);
                }
            }
            
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            waitingFirstRun = true;
            this.StretchHorizontally = true;
            this.StretchVertically = true;
            this.DrawFill = false;
            this.DrawBorder = false;
            this.ClipDrawing = true;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            separatorElement = new WaitingBarSeparatorElement();
            textElement = new WaitingBarTextElement();
            indicators = new WaitingIndicatorCollection();
            
            Indicators[0].SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
            Indicators[1].SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
            separatorElement.Class = "WaitingBarSeparator";

            this.Children.Add(separatorElement);
            this.Children.Add(textElement);
            this.Children.Add(Indicators[0]);
            this.Children.Add(Indicators[1]);
        }

        #endregion

        #region Methods

        public ProgressOrientation GetReversedDirection(ProgressOrientation direction)
        {
            if (direction == ProgressOrientation.Bottom)
            {
                return ProgressOrientation.Top;
            }
            if (direction == ProgressOrientation.Top)
            {
                return ProgressOrientation.Bottom;
            }
            if (direction == ProgressOrientation.Left)
            {
                return ProgressOrientation.Right;
            }
            return ProgressOrientation.Left;
        }

        public void IncrementOffset(int value)
        {
            offset += value;
        }
        
        public bool IsVertical()
        {
            if (this.WaitingDirection == ProgressOrientation.Right || this.WaitingDirection == ProgressOrientation.Left)
            {
                return false;
            }
            return true;
        }
       
        public virtual void ResetWaiting()
        {
            this.offset = 0;
            isBackwards = false;
            waitingFirstRun = true;
            this.separatorElement.InvalidateMeasure();
            for (int i = 0; i < Indicators.Count; i++)
            {
                this.Indicators[0].InvalidateMeasure();
            }
            this.InvalidateMeasure();
        }
       
        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(availableSize);
            
            for (int i = 0; i < Indicators.Count; i++)
            {
                Indicators[i].Measure(availableSize);
            }
            separatorElement.Measure(availableSize);
            textElement.Measure(availableSize);

            return Indicators[0].DesiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);
            RectangleF clientRect = GetClientRectangle(finalSize);
            
            if (this.WaitingStyle == WaitingBarStyles.Indeterminate)
            {
                ArrangeIndeterminateIndicatorElements(indicators, clientRect);
            }

            if (this.WaitingStyle == WaitingBarStyles.Throbber)
            {
                RectangleF indicatorFinalSize = GetThrobberIndicatorElementFinalSize(Indicators[0], clientRect);
                indicators[0].Arrange(indicatorFinalSize);
            }

            if (this.WaitingStyle == WaitingBarStyles.Dash)
            {
                RectangleF separatorFinalSize = GetDashElementFinalSize(this.SeparatorElement, clientRect);
                this.SeparatorElement.Arrange(separatorFinalSize);
            }

            this.textElement.Arrange(clientRect);
            SetElementsVisibility(this.WaitingStyle);

            return finalSize;
        }
        
        protected float AddIndicatorStep(float step, int i)
        {
            if (i == 0) return 0;

            bool isVertical = IsVertical();
            if (isVertical)
            {
                return (indicators.Count - i) * (step + indicators[0].DesiredSize.Height);
            }
            return (indicators.Count - i) * (step + indicators[0].DesiredSize.Width);
        }

        protected void ArrangeIndeterminateIndicatorElements(WaitingIndicatorCollection indicators, RectangleF clientRect)
        {
            UpdateOffset(clientRect);   
            float indicatorStep = CalculateIndicatorStep(clientRect);

            for (int i = 0; i < indicators.Count; i++)
            {
                RectangleF rect = MoveIndicatorElement(Indicators[i], clientRect, this.WaitingDirection);
                float dx = rect.X;
                float dy = rect.Y;

                if ((this.WaitingDirection == ProgressOrientation.Right && !this.RightToLeft) || (this.WaitingDirection == ProgressOrientation.Left && this.RightToLeft))
                {
                    dx += AddIndicatorStep(indicatorStep, i);
                    if (dx > clientRect.Width * 2 - indicators[0].DesiredSize.Width)
                    {
                        dx -= clientRect.Width * 2;
                    }
                    if (waitingFirstRun && i != 0 && dx > offset)
                    {
                        dx = -Indicators[0].DesiredSize.Width;
                    }
                }

                if (this.WaitingDirection == ProgressOrientation.Bottom)
                {
                    dy += AddIndicatorStep(indicatorStep, i);
                    if (dy > clientRect.Height * 2 - indicators[0].DesiredSize.Height)
                    {
                        dy -= clientRect.Height * 2;
                    }
                    if (waitingFirstRun && i != 0 && dy > offset)
                    {
                        dy = -Indicators[0].DesiredSize.Height;
                    }
                }

                if ((this.WaitingDirection == ProgressOrientation.Left && !this.RightToLeft) || (this.WaitingDirection == ProgressOrientation.Right && this.RightToLeft))
                {
                    dx -= AddIndicatorStep(indicatorStep, i);
                   if (dx < -clientRect.Width)
                    {
                        dx += clientRect.Width * 2;
                    }
                    if (waitingFirstRun && i != 0 && dx < clientRect.Width - indicators[0].DesiredSize.Width - offset)
                    {
                        dx = clientRect.Width;
                    }
                }

                if (this.WaitingDirection == ProgressOrientation.Top)
                {
                    dy -= AddIndicatorStep(indicatorStep, i);
                    if (dy < -clientRect.Height)
                    {
                        dy += clientRect.Height * 2;
                    }
                    if (waitingFirstRun && i != 0 && dy < clientRect.Height - indicators[0].DesiredSize.Height - offset)
                    {
                        dy = -Indicators[0].DesiredSize.Height;
                    }
                }

                indicators[i].Arrange(new RectangleF(new PointF(dx, dy), indicators[i].DesiredSize));
            }
        }
        
        protected float CalculateIndicatorStep(RectangleF clientRect)
        {
            bool isVertical = IsVertical();
            if (isVertical)
            {
                return (clientRect.Height * 2 - Indicators[0].DesiredSize.Height * Indicators.Count) / Indicators.Count;
            }
            return (clientRect.Width * 2 - Indicators[0].DesiredSize.Width * Indicators.Count) / Indicators.Count;
        }
        
        protected RectangleF GetThrobberIndicatorElementFinalSize(WaitingBarIndicatorElement element, RectangleF clientRect)
        {
            bool isVertical = IsVertical();
            if ((!isVertical && offset >= clientRect.Width - element.DesiredSize.Width) || (isVertical && offset >= clientRect.Height - element.DesiredSize.Height))
            {
                offset = 0;
                isBackwards = isBackwards ? false : true;
            }

            if (!isBackwards)
            {
                return MoveIndicatorElement(element, clientRect, this.WaitingDirection);
            }
            else
            {
                ProgressOrientation reverseDirection = GetReversedDirection(this.WaitingDirection);
                return MoveIndicatorElement(element, clientRect, reverseDirection);
            }
        }
        
        protected RectangleF GetDashElementFinalSize(WaitingBarSeparatorElement element, RectangleF clientRect)
        {
            if (!element.Dash)
            {
                return RectangleF.Empty;
            }

            int step = (element.StepWidth + element.SeparatorWidth) * 2;
            float dx = clientRect.X, dy = clientRect.Y;
            float width = clientRect.Width, height = clientRect.Height;

            if (offset >= step / 2)
            {
                offset = 0;
                return SetDashInitialPosition(element, clientRect);
            }

            if ((this.WaitingDirection == ProgressOrientation.Right && !this.RightToLeft) || (this.WaitingDirection == ProgressOrientation.Left && this.RightToLeft))
            {
                dx += offset - step;
                width += step;
                dy--;
                height++;
            }

            if ((this.WaitingDirection == ProgressOrientation.Left && !this.RightToLeft) || (this.WaitingDirection == ProgressOrientation.Right && this.RightToLeft))
            {
                dx -= offset + step / 2;
                width += step;
                dy--;
                height++;
            }

            if (this.WaitingDirection == ProgressOrientation.Top)
            {
                dy -= offset + step / 2;
                height += step;
                dx--;
                width++;
            }

            if (this.WaitingDirection == ProgressOrientation.Bottom)
            {
                dy += offset - step;
                height += step;
                dx--;
                width++;
            }

            return new RectangleF(new PointF(dx, dy), new SizeF(width, height));
        }
        
        protected RectangleF MoveIndicatorElement(WaitingBarIndicatorElement element, RectangleF clientRect, ProgressOrientation waitingDirection)
        {
            float dx = clientRect.X;
            float dy = clientRect.Y;

            if ((waitingDirection == ProgressOrientation.Right && !this.RightToLeft) || (waitingDirection == ProgressOrientation.Left && this.RightToLeft))
            {
                dx += offset;
                dy += (clientRect.Height - element.DesiredSize.Height) / 2;
            }

            if ((waitingDirection == ProgressOrientation.Left && !this.RightToLeft) || (waitingDirection == ProgressOrientation.Right && this.RightToLeft))
            {
                dx += clientRect.Width - element.DesiredSize.Width - offset;
                dy += (clientRect.Height - element.DesiredSize.Height) / 2;
            }

            if (waitingDirection == ProgressOrientation.Top)
            {
                dy += clientRect.Height - element.DesiredSize.Height - offset;
                dx += (clientRect.Width - element.DesiredSize.Width) / 2;
            }

            if (waitingDirection == ProgressOrientation.Bottom)
            {
                dy += offset;
                dx += (clientRect.Width - element.DesiredSize.Width) / 2;
            }

            return new RectangleF(new PointF(dx, dy), element.DesiredSize);
        }
        
        protected override void SetClipping(Graphics rawGraphics)
        {
            if (this.Shape != null)
            {
                GraphicsPath path = this.Shape.GetElementShape(this);
                rawGraphics.SetClip(path, CombineMode.Intersect);
            }
            else
            {
                base.SetClipping(rawGraphics);
            }
        }
        
        protected void SetElementsVisibility(WaitingBarStyles style)
        {
            if (style == WaitingBarStyles.Indeterminate)
            {
                SetIndicatorsVisibility(ElementVisibility.Visible);
                SeparatorElement.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
            }
            if (style == WaitingBarStyles.Throbber)
            {
                SetIndicatorsVisibility(ElementVisibility.Collapsed);
                SeparatorElement.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
            }
            if (style == WaitingBarStyles.Dash)
            {
                SetIndicatorsVisibility(ElementVisibility.Collapsed);
                SeparatorElement.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
            }
        }
        
        protected void SetIndicatorsVisibility(ElementVisibility visibility)
        {
            int i = 0;
            if (WaitingStyle == WaitingBarStyles.Throbber)
            {
                Indicators[i++].SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
            }
            for (; i < indicators.Count; i++)
            {
                Indicators[i].SetDefaultValueOverride(RadElement.VisibilityProperty, visibility);
            }
        }
        
        protected RectangleF SetDashInitialPosition(WaitingBarSeparatorElement element, RectangleF clientRect)
        {
            const int i = 1;
            int step = element.StepWidth + element.SeparatorWidth;
            bool isVertical = IsVertical();
            
            if (isVertical)
            {
                return new RectangleF(new PointF(clientRect.X - i, clientRect.Y - step), new SizeF(clientRect.Width + i, clientRect.Height + step * 2));
            }
            return new RectangleF(new PointF(clientRect.X - step, clientRect.Y - i), new SizeF(clientRect.Width + step * 2, clientRect.Height + i));
        }
        
        protected void UpdateOffset(RectangleF clientRect)
        {
            bool isVertical = IsVertical();

            if ((!isVertical && offset >= clientRect.Width * 2) || (isVertical && offset >= clientRect.Height * 2))
            {
                if (waitingFirstRun) waitingFirstRun = false;
                offset = 0;
            }
        }

        #endregion
    }
}
