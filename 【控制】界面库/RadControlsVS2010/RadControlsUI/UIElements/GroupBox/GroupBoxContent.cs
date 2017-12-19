using System;
using System.Drawing;
using Telerik.WinControls;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{   
    /// <summary>
    /// Represents the groupbox content.
    /// </summary>
    public class GroupBoxContent : GroupBoxVisualElement
    {
        /// <summary>
        /// Creates child elements. Please refer to TPF documentation for more information.
        /// </summary>
        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.Class = "GroupBoxContent";
        }

        /// <summary>
        /// Returns class name.
        /// </summary>
        /// <returns>class name</returns>
        public override string ToString()
        {
            return "GroupBoxContent";
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = SizeF.Empty;
            foreach (RadElement element in this.Children)
            {
                element.Measure(availableSize);

                desiredSize.Width = Math.Max(element.DesiredSize.Width, desiredSize.Width);
                desiredSize.Height = Math.Max(element.DesiredSize.Height, desiredSize.Height);
            }

            return desiredSize;
        }

        protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);

            return finalSize;
        }
    }
}