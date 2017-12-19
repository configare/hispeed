using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.Layouts
{
	/// <summary>
	/// Layout panel which docks its children to the sides of the area it contains
	/// </summary>
	public class DockLayoutPanel : LayoutPanel
	{
		// Fields
		public static RadProperty DockProperty = RadProperty.RegisterAttached(
            "Dock", typeof(Dock), typeof(DockLayoutPanel),
	        new RadElementPropertyMetadata(
                Dock.Left,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout,
                new PropertyChangedCallback(OnDockChanged)), new ValidateValueCallback(IsValidDock));

		public static RadProperty LastChildFillProperty = RadProperty.Register("LastChildFill", typeof(bool), typeof(DockLayoutPanel), 
			new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }

		// Methods
		protected override SizeF ArrangeOverride(SizeF arrangeSize)
		{
			RadElementCollection children = base.Children;
			int count = children.Count;
			int effectiveChildrenCount = count - (this.LastChildFill ? 1 : 0);
			float x = 0;
			float y = 0;
			float elementWidth = 0;
			float elementHeight = 0;
			for (int i = 0; i < count; i++)
			{
				RadElement element = children[i];
				if (element != null)
				{
					SizeF desiredSize = element.DesiredSize;
					RectangleF finalRect = new RectangleF(x, y, 
						Math.Max((float) 0, (float) (arrangeSize.Width - (x + elementWidth))), 
						Math.Max((float) 0, (float) (arrangeSize.Height - (y + elementHeight))));
					if (i < effectiveChildrenCount)
					{
						switch (GetDock(element))
						{
							case Dock.Left:
								x += desiredSize.Width;
								finalRect.Width = desiredSize.Width;
								break;
							case Dock.Right:
								elementWidth += desiredSize.Width;
								finalRect.X = Math.Max((float)0, (float)(arrangeSize.Width - elementWidth));
								finalRect.Width = desiredSize.Width;
								break;
							case Dock.Top:
								y += desiredSize.Height;
								finalRect.Height = desiredSize.Height;
								break;
							case Dock.Bottom:
								elementHeight += desiredSize.Height;
								finalRect.Y = Math.Max((float)0, (float)(arrangeSize.Height - elementHeight));
								finalRect.Height = desiredSize.Height;
								break;
						}
					}
					element.Arrange(finalRect);
				}
			}
			return arrangeSize;
		}
		/// <summary>
		/// Gets the dock property of an element
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static Dock GetDock(RadElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return (Dock) element.GetValue(DockProperty);
		}

        protected override SizeF MeasureOverride(SizeF constraint)
		{
			RadElementCollection children = base.Children;
			float horizontalElementWidth = 0;
			float verticalElementWidth = 0;
			float usedWidth = 0;
			float usedHeight = 0;
			int childIndex = 0;
			int childCount = children.Count;
			while (childIndex < childCount)
			{
				RadElement element = children[childIndex];
				if (element != null)
				{
					SizeF availableSize = new SizeF(
						Math.Max((float)0, (float)(constraint.Width - usedWidth)), 
						Math.Max((float)0, (float)(constraint.Height - usedHeight))
						);

					element.Measure(availableSize);
					SizeF desiredSize = element.DesiredSize;
					switch (GetDock(element))
					{
						case Dock.Left:
						case Dock.Right:
							verticalElementWidth = Math.Max(verticalElementWidth, usedHeight + desiredSize.Height);
							usedWidth += desiredSize.Width;
							break;

						case Dock.Top:
						case Dock.Bottom:
							horizontalElementWidth = Math.Max(horizontalElementWidth, usedWidth + desiredSize.Width);
							usedHeight += desiredSize.Height;
							break;
					}
				}
				childIndex++;
			}
			horizontalElementWidth = Math.Max(horizontalElementWidth, usedWidth);
			return new SizeF(horizontalElementWidth, Math.Max(verticalElementWidth, usedHeight));
		}

        private static bool IsValidDock(object value, RadObject obj)
        {
            Dock dock = (Dock)value;
            if (((dock != Dock.Left) && (dock != Dock.Top)) && (dock != Dock.Right))
            {
                return (dock == Dock.Bottom);
            }
            return true;
        }

        private static void OnDockChanged(RadObject obj, RadPropertyChangedEventArgs e)
        {
            RadElement reference = obj as RadElement;
            if (reference != null)
            {
                DockLayoutPanel panel = reference.Parent as DockLayoutPanel;
                if (panel != null)
                {
                    panel.InvalidateMeasure();
                }
            }
        }

		/// <summary>
		/// Sets the docking position of an element
		/// </summary>
		/// <param name="element"></param>
		/// <param name="dock"></param>
		public static void SetDock(RadElement element, Dock dock)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			element.SetValue(DockProperty, dock);
		}

		/// <summary>
		/// Gets or sets a property indicating whether the last child will fill the remaining area
		/// </summary>
		public bool LastChildFill
		{
			get
			{
				return (bool) base.GetValue(LastChildFillProperty);
			}
			set
			{
				base.SetValue(LastChildFillProperty, value);
			}
		}
	}
}