using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.Layouts
{   
    
    /// <summary>
    /// Represents the Telerik layout engine. The telerik layout engine manages the system
    /// of layouts in telerik controls.  
    /// </summary>
	public class TelerikLayoutEngine : ILayoutEngine
	{
		#region Properties
		private RadElement element;

		private RadElement Parent
		{
			get
			{
                RadElement parent = this.element.Parent;
                if (parent != null && parent.ElementState != ElementState.Loaded)
                {
                    return null;
                }

				return parent;
			}
		}

		private RadElementTree ElementTree
		{
			get
			{
				return element.ElementTree;
			}
		}
		#endregion

        /// <summary>
        /// Initializes a new instance of the TelerikLayoutEngine class; 
        /// it is responsible fo managing layouts system.
        /// </summary>
		public TelerikLayoutEngine(RadElement element)
		{
			this.element = element;
		}

		#region ILayoutEngine Properties & Methods

		public virtual Size AvailableSize
		{
			get
			{
				RadElement parent = this.Parent;
				Size availableSize = Size.Empty;

				while (parent != null)
				{
					availableSize = parent.Size;

					if ((parent.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize) ||
						parent.OverridesDefaultLayout)
					{
						SizeF maxBorderSize = GetMaxChildBorderNumber(parent, true);
						availableSize = Size.Subtract(availableSize, maxBorderSize.ToSize());
						break;
					}

					parent = parent.Parent;
				}

				if (parent == null)
				{
					if (this.ElementTree == null)
						return Size.Empty;

					if (this.ElementTree.Control == null)
						return Size.Empty;

					return this.ElementTree.Control.Size;
				}

				return availableSize;
			}
		}
     
		public virtual Padding GetParentPadding()
		{
            Padding res = Padding.Empty;
            if (this.element.FitToSizeMode != RadFitToSizeMode.FitToParentContent)
                return res;


            RadElement parent = this.Parent;
            
            /*if (parent != null && parent.Padding != Padding.Empty)
                res = parent.Padding;*/

            while (parent != null)
            {
                if ((parent.Padding != Padding.Empty) ||
                    (parent.FitToSizeMode == RadFitToSizeMode.FitToParentContent))
                {
                    res = parent.Padding;
                    break;
                }

                parent = parent.Parent;
            }

            return res;
		}

        public static Size CheckSize(Size size, Size maxSize, Size minSize)
        {
            if ((maxSize.Width > 0) && (size.Width > maxSize.Width))
                size.Width = maxSize.Width;

            if ((maxSize.Height > 0) && (size.Height > maxSize.Height))
                size.Height = maxSize.Height;

            if ((minSize.Width > 0) && (size.Width < minSize.Width))
                size.Width = minSize.Width;

            if ((minSize.Height > 0) && (size.Height < minSize.Height))
                size.Height = minSize.Height;

            return size;
        }

        public virtual Size CheckSize(Size size)
        {
            if (!this.element.AutoSize)
                return size;

            /*if ((this.element.Parent != null) && (!this.element.Parent.AutoSize) &&
                (this.element.OverridesDefaultLayout))
                return size;*/

            return CheckSize(size, this.element.MaxSize, this.element.MinSize);
        }
    
		public virtual void SetCoercedSize(Size newCoercedSize)
		{
			this.element.SetCoercedSize(newCoercedSize);

			// KOSEV: not thoroughly tested fix!

			if (this.element.Size.Equals(newCoercedSize) &&
				!this.element.cachedSize.Equals(newCoercedSize))
			{
				this.SetLayoutInvalidated(true);
				InvalidateChildrenLayout(this.element);
				this.element.PerformLayout(this.element);
			}
		}

		public virtual Rectangle GetFaceRectangle()
		{
			int width = this.element.Size.Width - this.element.Padding.Left - this.element.Padding.Right;
			int height = this.element.Size.Height - this.element.Padding.Top - this.element.Padding.Bottom;

			int left = this.element.Padding.Left;
			int top = this.element.Padding.Top;

			return new Rectangle(left, top, width, height);
		}

		public virtual void PerformLayout(RadElement affectedElement, bool performExplicit)
		{
			if (this.ElementTree == null)
				return;

			// KOSEV: new fix! not tested thoroughly!
			if (this.element.Visibility == ElementVisibility.Collapsed)
				return;

			PerformLayoutType performType = performExplicit ? PerformLayoutType.SelfExplicit : PerformLayoutType.Self;

			if (CheckSuspendedLayout(performType))
				return;

			if (!this.element.IsLayoutInvalidated && !performExplicit)
				return;

			if (this.element.GetBitState(RadElement.IsPerformLayoutRunningStateKey))
			{
				SetPerformExplicitLayout(performType);
				return;
			}

			RegisterLayoutRunning();

			this.element.SetBitState(RadElement.IsPerformLayoutRunningStateKey, true);
			PerformInternalLayout(affectedElement, performExplicit);
            this.element.SetBitState(RadElement.IsPerformLayoutRunningStateKey, false);

			CheckPerformLayoutAfterFinishLayout();

			CheckLastSelfExplicit(this.ElementTree.RootElement);

			UnregisterLayoutRunning();
		}

		private void CheckLastSelfExplicit(RadElement element)
		{
			if (this.ElementTree.RootElement.GetBitState(RadElement.IsPerformLayoutRunningStateKey))
				return;

			if (this.ElementTree.RootElement.layoutsRunning != 1)
				return;

			PerformLastSelfExplicit(element);
		}

		private void PerformLastSelfExplicit(RadElement element)
		{
			foreach (RadElement child in element.GetChildren(ChildrenListOptions.Normal))
			{
                if (child.ElementState != ElementState.Loaded)
                {
                    continue;
                }

				if (child.PerformLayoutAfterFinishLayout == PerformLayoutType.SelfExplicit)
				{
					child.PerformLayoutAfterFinishLayout = PerformLayoutType.None;
					child.LayoutEngine.PerformLayout(child, true);
					return;
				}

				PerformLastSelfExplicit(child);
			}
		}

		public virtual void PerformLayoutCore(RadElement affectedElement)
		{
			List<PreferredSizeData> list = new List<PreferredSizeData>();
			FillList(list);
			Size maxSize = GetMaxSize(list);
			bool isValidMaxSize = !LayoutUtils.IsZeroWidthOrHeight(maxSize);

			foreach (PreferredSizeData data in list)
			{
				if (data.Element.GetBitState(RadElement.IsDelayedSizeStateKey))
					SetDelayedSize(data.Element, maxSize, isValidMaxSize);
				else
					data.Element.Size = data.PreferredSize;
			}
		}
 
		public virtual void PerformParentLayout()
		{
			if (this.Parent != null)
			{
				// KOSEV: new fix! not tested thoroughly!
				if (this.Parent.Visibility == ElementVisibility.Collapsed)
					return;

				if (CheckSuspendedLayout(PerformLayoutType.Parent))
					return;

				if (!this.element.GetBitState(RadElement.IsPerformLayoutRunningStateKey))
					this.Parent.LayoutEngine.PerformLayout(this.element, true);
				else
					SetPerformExplicitLayout(PerformLayoutType.Parent);
			}
			else
			{
				if ((this.ElementTree.ComponentTreeHandler != null) && this.IsValidWrapElement())
                    this.ElementTree.ComponentTreeHandler.ElementTree.PerformLayoutInternal(this.element);
				else
					this.element.PerformLayout(this.element);
			}
		}

		public virtual Size GetPreferredSize(Size proposedSize)
		{
			if (this.ElementTree == null)
				return proposedSize;

            this.element.PerformLayout(this.element);

            if (this.element.cachedSize != LayoutUtils.InvalidSize)
                return this.element.cachedSize;

			if (this.element.AutoSize)
			{
				Size preferredSize = this.element.GetPreferredSizeCore(proposedSize);
				this.element.cachedSize = AddBoxSizesAndCheck(preferredSize, false);
				this.element.cachedSize = RemoveBoxSizes(this.element.cachedSize);
			}
			else
			{
				this.element.cachedSize = this.element.Size;
			}

			return this.element.cachedSize;
		}
      
		public virtual Size GetPreferredSizeCore(Size proposedSize)
		{
			Size newSize = Size.Empty;
			bool isDelayedSize = false;

			if (this.IsValidWrapElement())
			{
				// KOSEV: NEW FIX! 2007-08-01
				newSize = GetMaxChildSize(proposedSize);
				//if (!LayoutUtils.IsZeroWidthOrHeight(newSize))
					return newSize;
				//else
				//	isDelayedSize = true;
			}
			else
			{
				isDelayedSize = this.CheckDelayedSize();
			}

			if (isDelayedSize)
			{
				this.element.SetBitState(RadElement.IsDelayedSizeStateKey, true);
				return Size.Empty;
			}

			if (this.element.Margin != Padding.Empty)
				proposedSize = Size.Subtract(proposedSize, this.element.Margin.Size);

			/*
			if (!(this.element is IBoxElement))
			{
				Size borderSize = this.element.LayoutEngine.GetChildBorderSize();
				if (borderSize != Size.Empty)
					proposedSize = Size.Subtract(proposedSize, borderSize);
			}
			 */
			return proposedSize;
		}
      
		public virtual void InvalidateLayout()
		{
            if (this.element.ElementState != ElementState.Loaded)
            {
                return;
            }

			this.SetLayoutInvalidated(true);

			InvalidateChildrenLayout(this.element);

            RadElement parent = this.Parent;
			if (parent == null || parent.ElementState != ElementState.Loaded)
				return;

			// KOSEV: not thoroughly tested fix!
			if (parent.CoercedSize != LayoutUtils.InvalidSize)
				return;

            if (parent.LayoutEngine.IsValidWrapElement() || parent.OverridesDefaultLayout)
                parent.LayoutEngine.SetLayoutInvalidated(true);
		}

		public virtual void LayoutPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if (this.Parent != null)
			{
				CheckCoercedElements(this.element);

				// KOSEV: not thoroughly tested fix!
				if (this.Parent.CoercedSize != LayoutUtils.InvalidSize)
					return;

				if (this.Parent.LayoutEngine.IsValidWrapElement() || this.Parent.OverridesDefaultLayout)
				{
					this.Parent.LayoutEngine.PerformParentLayout();
					this.Parent.Invalidate(true);
				}
				else
				{
					this.PerformParentLayout();
					this.element.Invalidate(true);
				}

                if (e.Property == RadElement.BoundsProperty)
                {
                    if (this.Parent != null && this.ElementTree != null)
                    {
                        this.ElementTree.ComponentTreeHandler.InvalidateElement(this.Parent);
                    }
                }
			}
            else if (this.ElementTree.ComponentTreeHandler != null)
			{
                this.ElementTree.ComponentTreeHandler.ElementTree.PerformLayoutInternal(this.element);
                this.element.Invalidate();
			}
		}
     
		public virtual bool IsValidWrapElement()
		{
			if (this.element.AutoSizeMode == RadAutoSizeMode.WrapAroundChildren)
				return true;

			if ((this.element.AutoSizeMode == RadAutoSizeMode.Auto) && (this.element.LayoutableChildrenCount > 0))
				return true;

			return false;
		}
  
		public virtual void SetLayoutInvalidated(bool layoutInvalidated)
		{
			this.element.IsLayoutInvalidated = layoutInvalidated;

			if (layoutInvalidated && this.element.GetBitState(RadElement.IsPerformLayoutRunningStateKey))
				this.element.SetBitState(RadElement.IsPendingLayoutInvalidatedStateKey, true);
		}
       
		public virtual void PerformRegisteredSuspendedLayouts()
		{
			//Debug.WriteLine("PerformRegisteredSuspendedLayouts, " + this.element.GetType().Name);
			foreach (KeyValuePair<RadElement, ElementLayoutData> keyValuePair in this.element.SuspendedChildren)
			{
				ElementLayoutData elementLayoutData = keyValuePair.Value;
				if (elementLayoutData.Performed)
					continue;

				ElementLayoutData topMostData = GetTopMostPerformingParentData(elementLayoutData.Element);

				while (topMostData != null)
				{
					PerformRegisteredSuspendedLayout(topMostData);
					topMostData = GetTopMostPerformingParentData(elementLayoutData.Element);
				}
				PerformRegisteredSuspendedLayout(elementLayoutData);

				if (this.element.SuspendedChildren.Count == 0)
					return;
			}
/*
			foreach (ElementLayoutData elementLayoutData in this.element.SuspendedChildren)
			{
				RadElement element = elementLayoutData.Element;
				if (elementLayoutData.PerformLayoutType == PerformLayoutType.Parent)
					element.LayoutEngine.PerformParentLayout();
				else
					element.LayoutEngine.PerformLayout(element, (elementLayoutData.PerformLayoutType == PerformLayoutType.SelfExplicit));
			}
 */
			//this.element.SuspendedChildren = new List<ElementLayoutData>();
			this.element.SuspendedChildren.Clear();
		}

		private void PerformRegisteredSuspendedLayout(ElementLayoutData elementLayoutData)
		{
            if (elementLayoutData.Element.ElementState != ElementState.Loaded)
            {
                return;
            }

			elementLayoutData.Performed = true;

			if (elementLayoutData.PerformLayoutType == PerformLayoutType.Parent)
				elementLayoutData.Element.LayoutEngine.PerformParentLayout();
			else
				elementLayoutData.Element.LayoutEngine.PerformLayout(elementLayoutData.Element, (elementLayoutData.PerformLayoutType == PerformLayoutType.SelfExplicit));
		}

		private ElementLayoutData GetTopMostPerformingParentData(RadElement element)
		{
			ElementLayoutData topMostData = null;
			RadElement parent = element.Parent;

			while ((parent != this.element) && (parent != null))
			{
				if (this.element.SuspendedChildren.ContainsKey(parent))
				{
					ElementLayoutData tempData = this.element.SuspendedChildren[parent];
					if (!tempData.Performed)
						topMostData = tempData;
				}

				parent = parent.Parent;
			}
			return topMostData;
		}

		public virtual void RegisterChildSuspendedLayout(RadElement element, PerformLayoutType performLayoutType)
		{
			//Debug.WriteLine("RegisterChildSuspendedLayout, " + this.element.GetType().Name);
			//this.element.SuspendedChildren.Add(new ElementLayoutData(element, performLayoutType));

			if (this.element.SuspendedChildren.ContainsKey(element))
			{
				ElementLayoutData elementLayoutData = this.element.SuspendedChildren[element];
				if (elementLayoutData.Performed)
				{
					elementLayoutData.Performed = false;
					elementLayoutData.PerformLayoutType = performLayoutType;
				}
				else
				{
					if (performLayoutType == PerformLayoutType.Parent)
						elementLayoutData.PerformLayoutType = PerformLayoutType.Parent;
					else if ((elementLayoutData.PerformLayoutType != PerformLayoutType.Parent) &&
						(performLayoutType == PerformLayoutType.SelfExplicit))
						elementLayoutData.PerformLayoutType = PerformLayoutType.SelfExplicit;
				}
			}
			else
			{
				this.element.SuspendedChildren.Add(element, new ElementLayoutData(element, performLayoutType));
			}

		}

		public virtual void RegisterLayoutRunning()
		{
            if (this.element.ElementTree == null)
            {
                return;
            }

			this.ElementTree.RootElement.layoutsRunning++;
		}

		public virtual void UnregisterLayoutRunning()
		{
            if (this.element.ElementTree == null)
            {
                return;
            }

			this.ElementTree.RootElement.layoutsRunning--;
			CheckLayoutFinished();
		}

		#region Box Model Methods

		public virtual Point GetTransformationPoint()
		{
            Point point = Point.Add(this.element.Location, new Size(this.element.Margin.Left, this.element.Margin.Top));
            if (this.Parent == null)
            {
                return point;
            }

            //SuppressChildrenAlignment is obsolete but is CRUCIAL for the old layouts :D
#pragma warning disable 0618
            if (this.element.Alignment != ContentAlignment.TopLeft && !this.Parent.SuppressChildrenAlignment)
#pragma warning restore 0618
            {
                Rectangle withinBounds = new Rectangle(Point.Empty, this.Parent.FieldSize);
                Point alignOffset = TransformByAlignment(this.element.FullSize, withinBounds);
                point.Offset(alignOffset);
            }

            Padding padding = this.GetParentPadding();
            if (padding != Padding.Empty)
                point = Point.Add(point, new Size(padding.Left, padding.Top));

            if (!this.element.UseNewLayoutSystem)
            {
                Size borderSize = this.GetBorderOffset();
                if (borderSize != Size.Empty)
                    point = Point.Add(point, borderSize);
            }

            return point;
		}

		public virtual Size GetBorderOffset()
		{
			if (this.element.cachedBorderOffset == LayoutUtils.InvalidSize)
			{
				SizeF borderSize = SizeF.Empty;
				IBoxElement parentBox = this.Parent as IBoxElement;
				if ((parentBox != null) && (this.Parent.Visibility != ElementVisibility.Collapsed))
					borderSize = parentBox.Offset;
             
				if (this.element is IBoxElement)
					return borderSize.ToSize();

                //Fix 
                if (this.element is FillPrimitive)
                    return borderSize.ToSize();

                SizeF childBorderSize = this.GetMaxChildBorderOffset();

				if (childBorderSize != SizeF.Empty)
					borderSize = SizeF.Add(borderSize, childBorderSize);

				this.element.cachedBorderOffset = borderSize.ToSize();
			}

			return this.element.cachedBorderOffset;
		}

		public virtual Point TransformByAlignment(Size size, Rectangle withinBounds)
		{
			if (this.element.Alignment == ContentAlignment.TopLeft)
				return withinBounds.Location;
			
			Rectangle alignedRectangle = LayoutUtils.Align(size, withinBounds, this.element.Alignment);
			// TODO: Test alignment!
			/*
			if (alignedRectangle.Location.X < 0)
				alignedRectangle.Location = new Point(0, alignedRectangle.Location.Y);
			if (alignedRectangle.Location.Y < 0)
				alignedRectangle.Location = new Point(alignedRectangle.Location.X, 0);
			 */

			return alignedRectangle.Location;
		}

		public virtual Size GetBorderSize()
		{
			if (this.element.cachedBorderSize == LayoutUtils.InvalidSize)
			{
				SizeF borderSize = SizeF.Empty;

				IBoxElement elementBox = this.element as IBoxElement;
				if ((elementBox != null) && (this.element.Visibility != ElementVisibility.Collapsed))
					borderSize = elementBox.BorderSize;

				SizeF childBorderSize = GetMaxChildBorder();

				if (childBorderSize != SizeF.Empty)
					borderSize = SizeF.Add(borderSize, childBorderSize);

				this.element.cachedBorderSize = borderSize.ToSize();
			}

			return this.element.cachedBorderSize;
		}
 
		public virtual void InvalidateCachedBorder()
		{
			if (!(this.element is IBoxElement))
				return;

			this.element.cachedBorderSize = LayoutUtils.InvalidSize;

			foreach (RadElement child in this.element.Children)
			{
				child.cachedBorderOffset = LayoutUtils.InvalidSize;
				// Optimization - check child Children
			}

			if (this.Parent == null)
				return;
			
			this.Parent.cachedBorderSize = LayoutUtils.InvalidSize;
			// Optimization - Parent Children?

			foreach (RadElement sibling in this.Parent.Children)
			{
				if (sibling == this.element)
					continue;

				sibling.cachedBorderOffset = LayoutUtils.InvalidSize;
				// Optimization - check child Children
			}
		}

		public virtual Size GetChildBorderSize()
		{
			// TODO: needs caching
			SizeF borderSize = GetMaxChildBorderNumber(this.Parent, true);
			return borderSize.ToSize();
		}
		#endregion
		#endregion

		private void InvalidateChildrenLayout(RadElement element)
		{
            if (this.element.ElementState != ElementState.Loaded)
            {
                return;
            }

			// KOSEV: fix!
			//if (element.CoercedSize != LayoutUtils.InvalidSize)
			//    return;

			foreach (RadElement child in element.GetChildren(ChildrenListOptions.Normal))
			{
                if (child.ElementState != ElementState.Loaded || !child.AutoSize)
                {
                    continue;
                }

				child.LayoutEngine.SetLayoutInvalidated(true);

				if (!child.LayoutEngine.IsValidWrapElement() || child.OverridesDefaultLayout ||
					child.AffectsInnerLayout)
					InvalidateChildrenLayout(child);
			}
		}

		private void CheckLayoutFinished()
		{
			if (this.ElementTree.RootElement.layoutsRunning != 0)
				return;
			/*
			foreach (RadElement child in this.ElementTree.RootElement.ChildrenHierarchy)
			{
				if ((child.PerformLayoutAfterFinishLayout != PerformLayoutType.None) || (child.IsLayoutSuspended))
					return;
			}
			 */
			InvalidateChildrenCoercedSize(this.ElementTree.RootElement);
		}

		private void InvalidateChildrenCoercedSize(RadElement element)
		{
			foreach (RadElement child in element.Children)
			{
				child.SetCoercedSize(LayoutUtils.InvalidSize);
				InvalidateChildrenCoercedSize(child);
			}
		}

		private void PerformInternalLayout(RadElement affectedElement, bool performExplicit)
		{
			if (this.element.IsLayoutInvalidated)
			{
				PerformImplicitLayout(affectedElement);
				return;
			}
			if (performExplicit)
			{
				PerformExplicitLayout(affectedElement);
			}
		}

		private void PerformImplicitLayout(RadElement affectedElement)
		{
			this.element.InvalidateCachedSize();
			this.element.PerformLayoutCore(affectedElement);

			if (this.element.PerformLayoutAfterFinishLayout != PerformLayoutType.None)
				this.element.IsLayoutInvalidated = this.element.GetBitState(RadElement.IsPendingLayoutInvalidatedStateKey);
			else
				this.element.IsLayoutInvalidated = false;

			this.element.SetBitState(RadElement.IsPendingLayoutInvalidatedStateKey, false);
		}

		private void PerformExplicitLayout(RadElement affectedElement)
		{
			this.element.PerformLayoutCore(affectedElement);
		}

		private bool CheckSuspendedLayout(PerformLayoutType performLayoutType)
		{
            if (this.element.ElementState != ElementState.Loaded)
            {
                return false;
            }

			if (this.element.IsLayoutSuspended)
			{
				this.element.SetBitState(RadElement.IsLayoutPendingStateKey, true);
				return true;
			}

			RadElement suspendedParent = this.element.SuspendedParent;
			if (suspendedParent != null && suspendedParent.ElementState == ElementState.Loaded)
			{
				suspendedParent.LayoutEngine.RegisterChildSuspendedLayout(this.element, performLayoutType);
				return true;
			}

			return false;
		}

		private void SetPerformExplicitLayout(PerformLayoutType performType)
		{
			PerformLayoutType[] types = new PerformLayoutType[] {
				PerformLayoutType.Parent,
				PerformLayoutType.SelfExplicit,
				PerformLayoutType.Self
			};
			SetByPriority(performType, types);
		}

		private void SetByPriority(PerformLayoutType performType, PerformLayoutType[] types)
		{
			foreach (PerformLayoutType type in types)
			{
				if (this.element.PerformLayoutAfterFinishLayout == type)
					return;

				if (performType == type)
				{
					this.element.PerformLayoutAfterFinishLayout = type;
					if (type != PerformLayoutType.SelfExplicit)
						RegisterLayoutRunning();
					return;
				}
			}
		}

		private void InvalidateChildrenCoercedSize()
		{
			if (!this.element.InvalidateChildrenOnChildChanged)
				return;

			foreach (RadElement child in this.element.Children)
			{
				child.CoercedSize = LayoutUtils.InvalidSize;
			}
		}

		private void CheckChildrenSelfExplicitLayout()
		{
            //if (this.ElementTree.RootElement.IsPerformLayoutRunning)
			//	return;

			foreach (RadElement child in this.element.GetChildren(ChildrenListOptions.Normal))
			{
                if (child.ElementState != ElementState.Loaded)
                {
                    continue;
                }

				if (child.PerformLayoutAfterFinishLayout == PerformLayoutType.SelfExplicit)
				{
					child.PerformLayoutAfterFinishLayout = PerformLayoutType.None;
					child.LayoutEngine.PerformLayout(this.element, true);
				}
			}
		}

		private void CheckPerformLayoutAfterFinishLayout()
		{
			CheckChildrenSelfExplicitLayout();

			if (this.element.PerformLayoutAfterFinishLayout == PerformLayoutType.None)
				return;

			PerformLayoutType performType = this.element.PerformLayoutAfterFinishLayout;

			if (performType == PerformLayoutType.SelfExplicit)
				return;

			this.element.PerformLayoutAfterFinishLayout = PerformLayoutType.None;

			if (performType == PerformLayoutType.Parent)
				this.PerformParentLayout();
			else if (performType == PerformLayoutType.Self)
				this.element.PerformLayout(this.element);

			UnregisterLayoutRunning();
		}

		private void SetLayoutInvalidated2(bool layoutInvalidated)
		{
			this.element.IsLayoutInvalidated = layoutInvalidated;

            if (layoutInvalidated && this.element.GetBitState(RadElement.IsPerformLayoutRunningStateKey))
                this.element.SetBitState(RadElement.IsPendingLayoutInvalidatedStateKey, true);
		}

		private Size GetMaxSize(List<PreferredSizeData> list)
		{
			Size maxSize = Size.Empty;
			foreach (PreferredSizeData data in list)
			{
				if (data.Element is IBoxElement)
                    continue;

                //Fix for fill primitive painting with old layouts
                if (data.Element is FillPrimitive &&
                    data.Element.Parent != null)
                {
                    Size borderSize = this.GetMaxChildBorderNumber(data.Element.Parent, true).ToSize();
                    maxSize.Width = Math.Max(maxSize.Width, data.PreferredSize.Width - borderSize.Width);
                    maxSize.Height = Math.Max(maxSize.Height, data.PreferredSize.Height - borderSize.Height);
                    continue;
                }

				maxSize.Width = Math.Max(maxSize.Width, data.PreferredSize.Width);
				maxSize.Height = Math.Max(maxSize.Height, data.PreferredSize.Height);
			}
			return maxSize;
		}

		private void FillList(List<PreferredSizeData> list)
		{
			foreach (RadElement child in this.element.GetChildren(ChildrenListOptions.Normal))
			{
                list.Add(new PreferredSizeData(child, this.element.Size));
			}
		}

		private bool CheckDelayedSize()
		{
            if (this.IsValidWrapElement())
				return false;

			if (this.element.AutoSizeMode == RadAutoSizeMode.Auto)
				return true;

			if (this.Parent != null && this.Parent.ElementState == ElementState.Loaded && this.Parent.LayoutEngine.IsValidWrapElement())
				return true;

			return false;
		}

		private void SetDelayedSize(RadElement element, Size maxSize, bool isValidMaxSize)
		{
            if (element.ElementState != ElementState.Loaded)
            {
                return;
            }

            element.SetBitState(RadElement.IsDelayedSizeStateKey, false);

			if (!isValidMaxSize)
			{
				if ((element.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize) &&
					(this.element.Children.Count == 1))
					element.Size = this.element.Size;
				else
					element.Size = maxSize;

				return;
			}

			if (this.IsValidWrapElement() && (element.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize))
			{
				// KOSEV: not thoroughly tested fix!
				Size size = this.element.Size;
				// KOSEV: NEW FIX! 2006-08-02
				if (!(element is IBoxElement) &&
                    !(element is FillPrimitive))
				{
					Size borderSize = element.LayoutEngine.GetChildBorderSize();
					if (borderSize != Size.Empty)
						size = Size.Subtract(size, borderSize);
				}

				element.Size = CheckParentAutoSize(element, size);
			}
			else
			{
				Size sizeWithPadding = maxSize;

                if (element.FitToSizeMode == RadFitToSizeMode.FitToParentPadding ||
                    element.FitToSizeMode == RadFitToSizeMode.FitToParentBounds)
                {
                    sizeWithPadding = AddBoxSizesAndCheck(sizeWithPadding, true);
                }

				element.Size = CheckParentAutoSize(element, sizeWithPadding);
			}
		}

		private Size CheckParentAutoSize(RadElement element, Size size)
		{
			if (element.Parent == null)
				return size;

			if (element.Parent.AutoSize)
				return size;

			size.Width = Math.Min(element.Parent.Size.Width, size.Width);
			size.Height = Math.Min(element.Parent.Size.Height, size.Height);

			return size;
		}

		private Size AddBoxSizesAndCheck(Size size, bool isDelayedSize)
		{
			Size preferredSize = Size.Add(size, this.element.Padding.Size);
			
			if (preferredSize.Width < 0)
				preferredSize.Width = 0;

			if (preferredSize.Height < 0)
				preferredSize.Height = 0;

			if (this.IsValidWrapElement() || isDelayedSize)
			{
				Size borderSize = GetBorderSize();
				if (borderSize != Size.Empty)
					preferredSize = Size.Add(borderSize, preferredSize);
			}

			return CheckSize(preferredSize);
		}

		private Size RemoveBoxSizes(Size size)
		{
			if (this.IsValidWrapElement())
				return size;

			if (this.element is IBoxElement)
				return size;

            //Fix for fill primitive painting with old layouts
            if (this.element is FillPrimitive)
            {
                return size;
            }

			SizeF extraSize = GetMaxChildBorderNumber(this.Parent, true);            

		    return Size.Subtract(size, extraSize.ToSize());
		}

		private SizeF GetMaxChildBorder()
		{
			SizeF maxBorderSize = GetMaxChildBorderNumber(this.element, true);
			return maxBorderSize;
		}

		private Size GetMaxChildSize(Size proposedSize)
		{
			Size maxSize = Size.Empty;
			foreach (RadElement child in this.element.GetChildren(ChildrenListOptions.Normal))
			{
				// KOSEV: newly added and not thoroughly tested!
				if (child is IBoxElement)
					continue;

                Size childProposedSize = child.GetPreferredSize(proposedSize);
                
				if (child.AngleTransform != 0f || child.ScaleTransform.Width != 1f || child.ScaleTransform.Height != 1f)
				{
					Rectangle bounds = child.Bounds;
					bounds.Size = childProposedSize;
                    Rectangle rotatedBounds = child.GetBoundingRectangle(bounds);
					childProposedSize = rotatedBounds.Size;                    
				}
				maxSize.Width = Math.Max(maxSize.Width, childProposedSize.Width) + child.Margin.Horizontal;
				maxSize.Height = Math.Max(maxSize.Height, childProposedSize.Height) + child.Margin.Vertical;
			}
			return maxSize;
		}

		private void CheckCoercedElements(RadElement element)
		{
			RadElement parent = element.Parent;

			if (parent == null || parent.ElementState != ElementState.Loaded)
				return;

			if (parent.InvalidateChildrenOnChildChanged)
			{
				InvalidateCoercedElements(parent, element);
				return;
			}

			if (!parent.LayoutEngine.IsValidWrapElement())
				return;

			CheckCoercedElements(parent);
		}

		private void InvalidateCoercedElements(RadElement parent, RadElement element)
		{
			foreach (RadElement child in parent.GetChildren(ChildrenListOptions.Normal))
			{
				if (child == element)
					continue;

				if (child.CoercedSize != LayoutUtils.InvalidSize)
					continue;

                if (child.ElementState == ElementState.Loaded)
                {
                    child.LayoutEngine.SetLayoutInvalidated(true);

                    if (!child.LayoutEngine.IsValidWrapElement() || child.OverridesDefaultLayout || (element != null))
                        InvalidateCoercedElements(child, null);
                }
			}
		}

		private void InvalidateChildren(RadElement element)
		{
            foreach (RadElement child in element.GetChildren(ChildrenListOptions.Normal))
			{
				if (child == this.element)
					continue;

                if (child.ElementState == ElementState.Loaded)
                {
                    child.LayoutEngine.SetLayoutInvalidated(true);

                    if (!child.LayoutEngine.IsValidWrapElement() || child.OverridesDefaultLayout ||
                        (child.CoercedSize != LayoutUtils.InvalidSize))
                        InvalidateChildren(child);
                }
			}
		}

		private SizeF GetMaxChildBorderNumber(RadElement parent, bool getAsSize)
		{
            if (parent == null)
                return SizeF.Empty;

			SizeF maxBorderSize = SizeF.Empty;

			foreach (RadElement child in parent.GetChildren(ChildrenListOptions.Normal))
			{
				if ((parent == this.Parent) && (child == this.element))
					continue;

				IBoxElement childBox = child as IBoxElement;

				if (childBox == null)
					continue;

				if (child.LayoutableChildrenCount != 0)
					continue;

				maxBorderSize.Width = Math.Max(maxBorderSize.Width, (getAsSize ? childBox.HorizontalWidth : childBox.Offset.Width));
				maxBorderSize.Height = Math.Max(maxBorderSize.Height, (getAsSize ? childBox.VerticalWidth : childBox.Offset.Height));
			}

			return maxBorderSize;
		}

		private SizeF GetMaxChildBorderOffset()
		{
			SizeF maxBorderSize = GetMaxChildBorderNumber(this.Parent, false);
			return maxBorderSize;
		}
	}
}