using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.Layout
{
    public class IntegralScrollWrapPanel : WrapLayoutPanel
    {
        #region fields

        private int lineNumber = 0;
        private int curElementMaxX = 0;
        private int curElementMaxY = 0;
        private int lineCount = -1;
        private int maxColumns = int.MaxValue;
        private int maxRows = 2;
        private SizeF longestElementSize = SizeF.Empty;
        private Padding maxPadding = Padding.Empty;
        
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ClipDrawing = true;
        }

        #endregion

        #region API
        /// <summary>
        /// scrol to line
        /// </summary>
        /// <param name="lineNumber">line index to scrool - zero besed</param>
        public void ScrollToLine(int lineNumber)
        {
            this.lineNumber = lineNumber;
            this.InvalidateMeasure();
            this.InvalidateArrange();
            this.UpdateLayout();
            if (this.ElementTree != null && this.ElementTree.Control!=null)
            {
                this.ElementTree.Control.Invalidate();//fix visual glitch
            }
        }

        /// <summary>
        /// Scroll to element
        /// </summary>
        /// <param name="scrollElement"></param>
        public void ScrollToElement(RadElement scrollElement)
        {
            int i = this.Children.IndexOf(scrollElement);
            if (i == -1 || curElementMaxX == 0)
            {
                return;//element not fount or there arent any children
            }
            int linePossition = (i / curElementMaxX);//calculate which line is the element 

            this.ScrollToLine(linePossition);
        }

        /// <summary>
        /// how many lines we have 
        /// </summary>
        public int LineCount
        {
            get
            {
                return lineCount;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of columns to be shown in the in-ribbon portion of the gallery. 
        /// </summary>
        [Description("Gets or sets the maximum number of columns to be shown in the in-ribbon part of the gallery.")]
        public int MaxColumns
        {
            get
            {
                return this.maxColumns;
            }
            set
            {
                if (this.maxColumns != value)
                {
                    this.maxColumns = value;
                    this.InvalidateMeasure();
                    this.InvalidateArrange();
                    this.UpdateLayout();
                    if (this.ElementTree != null)
                    {
                        this.ElementTree.Control.Invalidate();//fix visual glitch
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of columns to be shown in the in-ribbon portion of the gallery. 
        /// </summary>
        [Description("Gets or sets the maximum number of columns to be shown in the in-ribbon part of the gallery.")]
        public int MaxRows
        {
            get
            {
                return this.maxRows;
            }
            set
            {
                if (this.maxRows != value)
                {
                    this.maxRows = value;
                    this.InvalidateMeasure();
                    this.InvalidateArrange();
                    this.UpdateLayout();
                    if (this.ElementTree != null)
                    {
                        this.ElementTree.Control.Invalidate();//fix visual glitch
                    }
                }
            }
        }


        /// <summary>
        /// which is the current line
        /// </summary>
        public int CurrentLine
        {
            get
            {
                return this.lineNumber;
            }
        }

        #endregion

        #region overrides

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            int count = this.Children.Count;
            float xTop = 0;
            float yTop = 0;
            float xMax = 0;
            float yMax = 0;
            bool shouldResizeV = false;
            RadElement element = null;
            int currentColumnIndex = 0;

            this.InitElements();
            base.MeasureOverride(availableSize);
            this.FindMaxSizedElement(availableSize);
            //this.InitElements();
            if (this.ElementTree!=null && this.ElementTree.Control != null)
            {
                this.ElementTree.Control.Invalidate();//fix visual glitch
            }
            float currentElementWidth = this.longestElementSize.Width + this.maxPadding.Vertical;
            float currentElementHeight = this.longestElementSize.Height + this.maxPadding.Horizontal;
            int verticalLinesCount = 0;

            for (int i = 0; i < count; ++i)
            {
                element = this.Children[i];
                xMax = Math.Max(xMax, currentElementWidth);
                yMax = Math.Max(yMax, currentElementHeight);

                bool shouldBreakLine = false;
                if (this.MaxColumns > 0 && currentColumnIndex >= this.MaxColumns)
                {
                    shouldBreakLine = true;
                }

                ++currentColumnIndex;

                if (xTop + currentElementWidth <= availableSize.Width && !shouldBreakLine)
                {
                    xTop += currentElementWidth;
                    xMax = Math.Max(xMax, xTop);
                    element.Measure(availableSize);
                }
                else
                {
                    shouldResizeV = true;
                    currentColumnIndex = 0;
                }

                if (shouldResizeV)
                {
                    xTop = 0;
                    shouldResizeV = false;
                    if( verticalLinesCount >= this.MaxRows)
                    {
                        break;
                    }

                    if (yTop + currentElementHeight <= availableSize.Height )
                    {
                        ++verticalLinesCount;
                        yTop += currentElementHeight;
                        yMax = Math.Max(yMax, yTop);
                        element.Measure(availableSize);
                    }
                }
            }

            return new SizeF(xMax, yMax);
        }
     
        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            //base.ArrangeOverride(finalSize);
            int count = this.Children.Count;
            RadElement element = null;
            float curX = 0;
            float curY = 0;
            if (count == 0)
            {
                return finalSize;
            }

            //this.InitElements();
           
            this.curElementMaxX = (int)Math.Floor(finalSize.Width /this.longestElementSize.Width);
            if (this.MaxColumns < this.curElementMaxX)
            {
                this.curElementMaxX = this.MaxColumns;
            }
            this.curElementMaxY = (int)Math.Floor(finalSize.Height / this.longestElementSize.Height);
            if (this.MaxRows < this.curElementMaxY)
            {
                this.curElementMaxY = this.MaxRows;
            }

            this.lineCount = count / curElementMaxX + ((count % curElementMaxX) !=0 ? 1 : 0);

            int i = this.CurrentLine * curElementMaxX;

            for (int yCount = 0; yCount < curElementMaxY && i < count; ++yCount)
            {
                for (int xCount = 0; xCount < curElementMaxX && i < count; ++xCount, ++i)
                {
                    element = this.Children[i];
                    element.Visibility = ElementVisibility.Visible;
                    element.Arrange(new RectangleF(curX, curY, this.longestElementSize.Width, this.longestElementSize.Height));
                    curX += this.longestElementSize.Width;
                }
                curY += this.longestElementSize.Height;
                curX = 0;
            }
            return finalSize;
        }

        #endregion

        #region helper methods
        private void FindMaxSizedElement(SizeF availableSize)
        {
            int count = this.Children.Count;

            RadElement element;
            for (int i = 0; i < count; ++i)
            {
                element = this.Children[i];
                element.Measure(availableSize);

                if (element.DesiredSize.Width > this.longestElementSize.Width)
                {
                    this.longestElementSize.Width = element.DesiredSize.Width;
                }
                if (element.DesiredSize.Height > this.longestElementSize.Height)
                {
                    this.longestElementSize.Height = element.DesiredSize.Height;
                }

                if (element.Padding.Horizontal > this.maxPadding.Horizontal ||
                    element.Padding.Vertical > this.maxPadding.Vertical)
                {
                    this.maxPadding = element.Padding;
                }
            }
        }

        private void InitElements()
        {
            int count = this.Children.Count;
            RadElement element;
            for (int i = 0; i < count; ++i)
            {
                element = this.Children[i];
                element.Visibility = ElementVisibility.Hidden;
                //element.ClipDrawing = true;
                //element.Location = Point.Empty;
                //element.Measure(SizeF.Empty);
                //element.Bounds = Rectangle.Empty;
            }
        }

        #endregion
    }
}
