using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
	[Flags]
	public enum DropDownButtonArrowPosition 
	{ 
		Right = 0, 
		Bottom, 
		Left, 
		Up
	}

    /// <summary>
    /// Rec editors. It is used in 
    /// RadComboboxElement, DropDownButton, etc.
    /// </summary>
    public class DropDownEditorLayoutPanel : DockLayoutPanel
    {
        private RadElement content;
		private RadElement arrowButton;
        private RadElement buttonSeparator;
		//private bool expandArrow = false;		

		public static RadProperty IsContentProperty = RadProperty.Register("IsContent", typeof(bool), typeof(DropDownEditorLayoutPanel),
			new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty IsArrowButtonProperty = RadProperty.Register("IsArrowButton", typeof(bool), typeof(DropDownEditorLayoutPanel),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty IsButtonSeparatorProperty = RadProperty.Register("IsButtonSeparator", typeof(bool), typeof(DropDownEditorLayoutPanel),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ExpandArrowProperty = RadProperty.Register("ExpandArrow", typeof(bool), typeof(DropDownEditorLayoutPanel),
			new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ArrowPositionProperty = RadProperty.Register("ArrowPosition", typeof(DropDownButtonArrowPosition), typeof(DropDownEditorLayoutPanel),
			new RadElementPropertyMetadata(DropDownButtonArrowPosition.Right, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		private bool EnsureContentAndArrowElements()
		{
			if (this.content == null || this.arrowButton == null)
			{
				foreach (RadElement child in this.GetChildren(ChildrenListOptions.IncludeCollapsed))
				{
					if ((bool)child.GetValue(IsContentProperty))
					{
						this.content = child;
					}
					else if ((bool)child.GetValue(IsArrowButtonProperty))
					{
						this.arrowButton = child;
                        this.arrowButton.StretchHorizontally = true;
                        this.arrowButton.StretchVertically = true;
					}
                    else if ((bool)child.GetValue(IsButtonSeparatorProperty))
                    {
                        this.buttonSeparator = child;
                    }
				}
				return this.content != null && this.arrowButton != null;
			}
			else
			{
				return true;
			}
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.NotifyParentOnMouseInput = true;
        }

		public bool ExpandArrow
		{
			get { return (bool)this.GetValue(ExpandArrowProperty); }
			set { this.SetValue(ExpandArrowProperty, value); }
		}

		/// <summary>
		/// Note: this property is supposed to be used only when this.Parent.AutoSizeMode==WrapAroundChildren
		/// </summary>
		public DropDownButtonArrowPosition ArrowPosition
		{
			get
			{
				return (DropDownButtonArrowPosition)this.GetValue(ArrowPositionProperty);
			}
			set
			{
				this.SetValue(ArrowPositionProperty, value);
			}
		}
		
        /// <summary>
        /// Performs layout changes based on the element given as a paramater. 
        /// Sizes and places are determined by the concrete layout panel that is used. 
        /// Since all layout panels update their layout automatically through events, 
        /// this function is rarely used directly.
        ///</summary>

        public override void PerformLayoutCore(RadElement affectedElement)
        {
			Size defaultSize;
			Size availableSize = this.Parent.Size - this.GetParentBorderSize(); 
			if (!EnsureContentAndArrowElements())
				return;		
			//defaultSize = this.arrowButton.MinSize;
			if (this.arrowButton.Visibility == ElementVisibility.Collapsed)
			{
				defaultSize = Size.Empty;
			}
			else if (this.arrowButton is RadArrowButtonElement)
			{
				defaultSize = this.arrowButton.MinSize;
			}
			else
				defaultSize = this.arrowButton.GetPreferredSize(availableSize);
			/*if (this.arrowButton is RadArrowButtonElement)
				defaultSize = ((RadArrowButtonElement)this.arrowButton).ArrowFullSize;
			else
				defaultSize = Telerik.WinControls.UI.RadArrowButtonElement.DefaultSize;*/	

			Size contentSize = this.content.GetPreferredSize(availableSize);
			// KOSEV: TEMP FIX!
			//if (this.Parent.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
			//{
			//    if (this.expandArrow)
			//        defaultSize.Width = availableSize.Width - contentSize.Width;
			//    else
			//        contentSize.Width = availableSize.Width - defaultSize.Width;
			//}					

			if (this.ArrowPosition == DropDownButtonArrowPosition.Right || this.ArrowPosition == DropDownButtonArrowPosition.Left)
			{
				if (this.ExpandArrow)
					//defaultSize.Width = availableSize.Width - contentSize.Width - GetParentBorderSize().Width;
					defaultSize.Width = availableSize.Width - contentSize.Width;
				else if (this.Parent.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize || this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
					contentSize.Width = availableSize.Width - defaultSize.Width;

				if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
					contentSize.Height = availableSize.Height;


				//This is a fix - arrowButton.GetPreferredSize must be called or you'll be unpleasantly surprised !
				Size arrowBtnSize = this.arrowButton.GetPreferredSize(new Size(defaultSize.Width, contentSize.Height));
				arrowBtnSize = defaultSize;

				arrowBtnSize.Height = Math.Max(Math.Max(arrowBtnSize.Height, contentSize.Height), this.Parent.MinSize.Height);

				if (this.GetHorizontalArrowPosition() == DropDownButtonArrowPosition.Left)
				{
					this.content.Bounds = new Rectangle(new Point(defaultSize.Width, 0), contentSize);
					this.arrowButton.Bounds = new Rectangle(Point.Empty, arrowBtnSize);
				}
				else 
				{
					this.content.Bounds = new Rectangle(Point.Empty, contentSize);
					this.arrowButton.Bounds = new Rectangle(new Point(content.Size.Width, 0), arrowBtnSize);
				}
			}
			else
			{
				if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
					contentSize.Width = availableSize.Width;

				if (this.ExpandArrow)
					defaultSize.Height = availableSize.Height - contentSize.Height - GetParentBorderSize().Height;
				else if (this.Parent.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize || this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
					contentSize.Height = availableSize.Height - defaultSize.Height;
				
				//This is a fix - arrowButton.GetPreferredSize must be called or you'll be unpleasantly surprised !
				Size arrowBtnSize = this.arrowButton.GetPreferredSize(new Size(contentSize.Width, defaultSize.Height));
				arrowBtnSize = defaultSize;

				arrowBtnSize.Width = Math.Max(Math.Max(arrowBtnSize.Width, contentSize.Width), this.Parent.MinSize.Width);

				if (this.ArrowPosition == DropDownButtonArrowPosition.Bottom)
				{
					this.content.Bounds = new Rectangle(Point.Empty, contentSize);
					this.arrowButton.Bounds = new Rectangle(new Point(0, content.Size.Height), arrowBtnSize);
				}
				else
				{
					this.content.Bounds = new Rectangle(new Point(0, defaultSize.Height), contentSize);
					this.arrowButton.Bounds = new Rectangle(Point.Empty, arrowBtnSize);
				}
			}
        }

        protected override SizeF ArrangeOverride(SizeF arrangeSize)
        {
            base.ArrangeOverride(arrangeSize);

            if (!EnsureContentAndArrowElements())
                return arrangeSize;
            
            SizeF defaultSize = this.arrowButton.DesiredSize;
            SizeF contentSize = this.content.DesiredSize;
            SizeF separatorSize = SizeF.Empty;
             
            if (this.ArrowPosition == DropDownButtonArrowPosition.Right || this.ArrowPosition == DropDownButtonArrowPosition.Left)
            {
                if (this.buttonSeparator != null)
                {
                    separatorSize = new SizeF(this.buttonSeparator.DesiredSize.Width, arrangeSize.Height);
                }

                if (this.ExpandArrow)
                {
                    defaultSize.Width = arrangeSize.Width - contentSize.Width - separatorSize.Width;
                }
                else
                {
                    contentSize.Width = arrangeSize.Width - defaultSize.Width - separatorSize.Width;
                }
                contentSize.Height = arrangeSize.Height;

                SizeF arrowBtnSize = this.arrowButton.DesiredSize;
                arrowBtnSize = defaultSize;
                arrowBtnSize.Height = Math.Max(Math.Max(arrowBtnSize.Height, contentSize.Height), arrangeSize.Height);
                
                if (this.GetHorizontalArrowPosition() == DropDownButtonArrowPosition.Left)
                {
                    this.content.Arrange(new RectangleF(new PointF(arrowBtnSize.Width + separatorSize.Width, 0f), contentSize));
                    if (this.buttonSeparator != null)
                    {
                        this.buttonSeparator.Arrange(new RectangleF(new PointF(arrowBtnSize.Width, 0f), separatorSize));
                    }
                    this.arrowButton.Arrange(new RectangleF(PointF.Empty, arrowBtnSize));
                }
                else
                {
                    this.content.Arrange(new RectangleF(PointF.Empty, contentSize));
                    if (this.buttonSeparator != null)
                    {
                        this.buttonSeparator.Arrange(new RectangleF(new PointF(contentSize.Width, 0f), separatorSize));
                    }
                    this.arrowButton.Arrange(new RectangleF(new PointF(contentSize.Width + separatorSize.Width, 0f), arrowBtnSize));
                }
            }
            else
            {
                if (this.buttonSeparator != null)
                {
                    separatorSize = new SizeF(arrangeSize.Width, this.buttonSeparator.DesiredSize.Height);
                }

                contentSize.Width = arrangeSize.Width;
                contentSize.Height = arrangeSize.Height - this.arrowButton.DesiredSize.Height;
                if (this.ExpandArrow)
                {
                    defaultSize.Height = arrangeSize.Height - contentSize.Height - GetParentBorderSize().Height;
                }
                else
                {
                    contentSize.Height = arrangeSize.Height - defaultSize.Height - separatorSize.Height + 1;
                }
                
                SizeF arrowBtnSize = this.arrowButton.DesiredSize;
                arrowBtnSize = defaultSize;
                arrowBtnSize.Width = Math.Max(Math.Max(arrowBtnSize.Width, contentSize.Width), arrangeSize.Width);
                
                if (this.ArrowPosition == DropDownButtonArrowPosition.Bottom)
                {
                    this.content.Arrange(new RectangleF(PointF.Empty, contentSize));
                    if (this.buttonSeparator != null)
                    {
                        this.buttonSeparator.Arrange(new RectangleF(new PointF(0f, contentSize.Height - 1), separatorSize));
                    }
                    this.arrowButton.Arrange(new RectangleF(new PointF(0f, contentSize.Height + separatorSize.Height - 1), arrowBtnSize));
                }
                else
                {
                    this.content.Arrange(new RectangleF(new PointF(0f, arrowBtnSize.Height + separatorSize.Height - 1), contentSize));
                    if (this.buttonSeparator != null)
                    {
                        this.buttonSeparator.Arrange(new RectangleF(new PointF(0f, arrowBtnSize.Height), separatorSize));
                    }
                    this.arrowButton.Arrange(new RectangleF(Point.Empty, arrowBtnSize));
                }
            }
            return arrangeSize;
        }

		private DropDownButtonArrowPosition GetHorizontalArrowPosition()
		{
			if (this.ArrowPosition == DropDownButtonArrowPosition.Left)
			{
				if (this.RightToLeft)
					return DropDownButtonArrowPosition.Right;
				else
					return DropDownButtonArrowPosition.Left;
			}
			else if (this.ArrowPosition == DropDownButtonArrowPosition.Right)
			{
				if (this.RightToLeft)
					return DropDownButtonArrowPosition.Left;
				else
					return DropDownButtonArrowPosition.Right;
			}
			return DropDownButtonArrowPosition.Right;
		}

        /// <summary>
        /// Retrieves the preferred size of the layout panel. If the proposed size is 
        /// smaller than the minimal one, the minimal one is retrieved. Since all layout 
        /// panels update their layout automatically through events, this function is 
        /// rarely used directly.
        /// </summary>
        /// <param name="proposedSize"></param>
        /// <returns></returns>
        public override Size GetPreferredSizeCore(Size proposedSize)
        {            
			if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
			{
				//return this.AvailableSize;
				return this.Parent.Size;
			}
			if (this.ArrowPosition == DropDownButtonArrowPosition.Right || this.ArrowPosition == DropDownButtonArrowPosition.Left)
			{
				int maxHeight = 0, width = 0;
				foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
				{					
					Size childSize = child.FullSize;
					width += childSize.Width;
					maxHeight = Math.Max(maxHeight, childSize.Height);				
				}
				if (this.ExpandArrow)
					return new Size(Math.Max(width, this.Parent.Size.Width - GetParentBorderSize().Width), maxHeight);
				else
					return new Size(width, maxHeight);
			}
			else
			{
				int maxWidth = 0, height = 0;
				foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
				{
					Size childSize = child.FullSize;
					maxWidth = Math.Max(maxWidth, childSize.Width);
					height += childSize.Height;					
				}
				if (this.ExpandArrow)
					return new Size(maxWidth, Math.Max(height, this.Parent.Size.Height - GetParentBorderSize().Height));
				else
					return new Size(maxWidth, height);
			}			
        }

        protected override SizeF MeasureOverride(SizeF constraint)
        {
            //if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
            //{
            //    //return this.AvailableSize;
            //    return this.Parent.Size;
            //}
            base.MeasureOverride(constraint);
            constraint -= GetParentBorderSize();
            if (this.ArrowPosition == DropDownButtonArrowPosition.Right || this.ArrowPosition == DropDownButtonArrowPosition.Left)
            {
                float maxHeight = 0f;
                float width = 0f;

                foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
                {
                    child.Measure(constraint);
                    SizeF childSize = child.DesiredSize;
                    width += childSize.Width;
                    maxHeight = Math.Max(maxHeight, childSize.Height);
                }
                if (this.ExpandArrow)
                    return new SizeF(width, maxHeight);
                else
                    return new SizeF(width,maxHeight);//,constraint.Height));
            }
            else
            {
                float maxWidth = 0f;
                float height = 0f;

                foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
                {
                    SizeF childSize = child.DesiredSize;
                    maxWidth = Math.Max(maxWidth, childSize.Width);
                    height += childSize.Height;
                }
                if (this.ExpandArrow)
                    return new SizeF(maxWidth, Math.Max(height, constraint.Height ));
                else
                    return new SizeF(maxWidth, height);
            }			

        }

		private Size GetParentBorderSize()
		{
			BorderPrimitive border = null;
            if (this.Parent != null)
            {
                foreach (RadElement child in this.Parent.GetChildren(ChildrenListOptions.Normal))
                {
                    if (child is BorderPrimitive)
                    {
                        border = (BorderPrimitive)child;
                        break;
                    }
                }
            }
			if (border != null)
				return border.BorderSize.ToSize();
			else
				return Size.Empty;
		}
    }
}