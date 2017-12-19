using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a drop down panel.
    /// </summary>
    public class DropDownPanel : RadElement
    {
        private const int BackgroundZedIndex = 0;
        private const int ContentZedIndex = 1;
        private const int BorderZedIndex = 2;
        

        private FillPrimitive background;
        /// <summary>
        /// Gets the background (fill primitive).
        /// </summary>
        public FillPrimitive Background
        {
            get { return background; }
        }

        private StripLayoutPanel content;
        /// <summary>
        /// Gets the content (strip layout panel).
        /// </summary>
        public StripLayoutPanel Content
        {
            get { return content; }
        }

        private BorderPrimitive border;

        /// <summary>
        /// Gets the border (BorderPrimitive).
        /// </summary>
        public BorderPrimitive Border
        {
            get { return border; }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            background = new FillPrimitive();
            background.ZIndex = BackgroundZedIndex;
            background.Class = "DropDownButtonBackground";
			Children.Add(background);

            content = new StripLayoutPanel();
			content.Orientation = System.Windows.Forms.Orientation.Vertical;
			content.EqualChildrenWidth = true;
            content.ZIndex = ContentZedIndex;
            Children.Add(content);

            border = new BorderPrimitive();
            border.ZIndex = BorderZedIndex;
            Children.Add(border);
        }
    }
}
