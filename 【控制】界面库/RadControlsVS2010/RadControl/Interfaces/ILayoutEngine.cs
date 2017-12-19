using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.Layouts
{



    /// <summary>Defines properties and methods for the default layout engine.</summary>
	public interface ILayoutEngine
	{
        /// <summary>Gets a value indicating the available size.</summary>
		Size AvailableSize { get; }

        /// <summary>Retrieves parent's padding.</summary>
		Padding GetParentPadding();
        /// <summary>Retrieves check size structure.</summary>
		Size CheckSize(Size size);
        /// <summary>Sets coerced size taken as parameter.</summary>
		void SetCoercedSize(Size newCoercedSize);
        /// <summary>Gets the face rectangle.</summary>
		Rectangle GetFaceRectangle();

		void PerformLayout(RadElement affectedElement, bool performExplicit);

		void PerformLayoutCore(RadElement affectedElement);

		void PerformParentLayout();


		Size GetPreferredSize(Size proposedSize);

		Size GetPreferredSizeCore(Size proposedSize);

        /// <summary>Invalidates layout - needs redrawing.</summary>
		void InvalidateLayout();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
		void LayoutPropertyChanged(RadPropertyChangedEventArgs e);

        /// <summary>Retrieves a value indicating whether the element is valid wrap element.</summary>
		bool IsValidWrapElement();
		void SetLayoutInvalidated(bool layoutInvalidated);

        /// <summary>Performs registered suspended layout.</summary>
		void PerformRegisteredSuspendedLayouts();
		void RegisterChildSuspendedLayout(RadElement element, PerformLayoutType performLayoutType);
		void RegisterLayoutRunning();
		void UnregisterLayoutRunning();

        /// <summary>Retrieves transformation point. The point is a Point structure.</summary>
		Point GetTransformationPoint();
        /// <summary>Retrieves transformation by alignment point using size and inner bounds.</summary>
		Point TransformByAlignment(Size size, Rectangle withinBounds);
        /// <summary>Retrieves Border offset.</summary>
		Size GetBorderOffset();
        /// <summary>Retrieves border size.</summary>
		Size GetBorderSize();
        /// <summary>Retrieves the border size of its child.</summary>
		Size GetChildBorderSize();
        /// <summary>Invalidates the cached border.</summary>
		void InvalidateCachedBorder();
	}
}