using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents parameters of the scroll panel such as values for the small and
    /// large changes while scrolling.
    /// </summary>
    public struct ScrollPanelParameters
    {
        /// <summary>
        /// Represents horizonatal scroll parameters data: horizontal minimum and maximum
        /// positions, and horizontal small and large change.
        /// </summary>
        public ScrollBarParameters HorizontalScrollParameters;

        /// <summary>
        /// Represents vertical scroll parameters data: vertical minimum and maximum
        /// positions, and vertical small and large change.
        /// </summary>
        public ScrollBarParameters VerticalScrollParameters;

        /// <summary>
        /// Initializes a new ScrollPanelParameters struct.
        /// </summary>
        /// <overloads>
        /// 	<see cref="ScrollPanelParameters">ScrollPanelParameters(int,int,int,int,int,int,int,int)</see>
        /// </overloads>
        /// <param name="horizontalScrollParameters">
        /// Initializes the parameters pertaining to the horizontal scrolling - small and
        /// large horizontal changes, and minimum and maximum scrolling positions.
        /// </param>
        /// <param name="verticalScrollParameters">
        /// Initializes the parameters pertaining to the vertical scrolling - small and large
        /// vertical changes, and minimum and maximum scrolling positions.
        /// </param>
        public ScrollPanelParameters(ScrollBarParameters horizontalScrollParameters,
            ScrollBarParameters verticalScrollParameters)
        {
            this.HorizontalScrollParameters = horizontalScrollParameters;
            this.VerticalScrollParameters = verticalScrollParameters;
        }

        /// <summary>
        /// Initializes a new ScrollPanelParameters structure.
        /// </summary>
        /// <overloads>
        /// 	<see cref="ScrollPanelParameters">ScrollPanelParameters(ScrollBarParameters,ScrollBarParameters)</see>
        /// </overloads>
        /// <param name="horizMinimum">Initializes the minimum horizontal scrolling position.</param>
        /// <param name="horizMaximum">Initializes the maximum horizontal scrolling position.</param>
        /// <param name="horizSmallChange">
        /// Initializes the small horizontal change value; the value added or substracted
        /// from the current position when small horizontal change is initiated.
        /// </param>
        /// <param name="horizLargeChange">
        /// Initializes the large horizontal change value; the value added or substracted
        /// from the current position when large horizontal change is initiated.
        /// </param>
        /// <param name="vertMinimum">Initializes the vertical minimum scrolling position.</param>
        /// <param name="vertMaximum">Initializes the vertical maximum scrolling position.</param>
        /// <param name="vertSmallChange">
        /// Initializes the small change vertical value; the value added or substracted from
        /// the current position when small vertical change is initiated.
        /// </param>
        /// <param name="vertLargeChange">
        /// Initializes the large vertical change value; the value added or substracted from
        /// the current position when large vertical change is initiated.
        /// </param>
        public ScrollPanelParameters(
            int horizMinimum, int horizMaximum, int horizSmallChange, int horizLargeChange,
            int vertMinimum, int vertMaximum, int vertSmallChange, int vertLargeChange)
        {
            this.HorizontalScrollParameters = new ScrollBarParameters(
                horizMinimum, horizMaximum, horizSmallChange, horizLargeChange);
            this.VerticalScrollParameters = new ScrollBarParameters(
                vertMinimum, vertMaximum, vertSmallChange, vertLargeChange);
        }
    }
}
