using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Design;
using System.ComponentModel;

namespace Telerik.WinControls.Layouts
{
    /// <summary>Represents a layout</summary>
	public class ImageAndTextLayoutPanel : LayoutPanel
	{
		#region Fields
		private RadElement imageElement;
		private RadElement textElement;
		private Size imageSize = Size.Empty;
		#endregion

		#region Dependency properties
		public static RadProperty ImageAlignmentProperty = RadProperty.Register("ImageAlignment", typeof(ContentAlignment), typeof(ImageAndTextLayoutPanel),
			new RadElementPropertyMetadata(ContentAlignment.MiddleLeft, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

		public static RadProperty TextAlignmentProperty = RadProperty.Register("TextAlignment", typeof(ContentAlignment), typeof(ImageAndTextLayoutPanel),
			new RadElementPropertyMetadata(ContentAlignment.MiddleLeft, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

		public static RadProperty TextImageRelationProperty = RadProperty.Register("TextImageRelation", typeof(TextImageRelation), typeof(ImageAndTextLayoutPanel),
			new RadElementPropertyMetadata(TextImageRelation.Overlay, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure));

		public static RadProperty IsImagePrimitiveProperty = RadProperty.Register("IsImagePrimitive", typeof(bool), typeof(ImageAndTextLayoutPanel),
			new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty IsTextPrimitiveProperty = RadProperty.Register("IsTextPrimitive", typeof(bool), typeof(ImageAndTextLayoutPanel),
			new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));
				
        public static RadProperty DisplayStyleProperty = RadProperty.Register("DisplayStyle", typeof(DisplayStyle), typeof(ImageAndTextLayoutPanel),
            new RadElementPropertyMetadata(DisplayStyle.ImageAndText, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure));
		#endregion

		#region Properties
        /// <summary>Gets or sets a value indicating the image alignment.</summary>
		[RadPropertyDefaultValue("ImageAlignment", typeof(ImageAndTextLayoutPanel))]
		[Category(RadDesignCategory.BehaviorCategory)]
		public ContentAlignment ImageAlignment
		{
			get
			{
				return (ContentAlignment) this.GetValue(ImageAlignmentProperty);
			}
			set
			{
				this.SetValue(ImageAlignmentProperty, value);
			}
		}

        /// <summary>Gets or sets a value indicating text alignment.</summary>
		[RadPropertyDefaultValue("TextAlignment", typeof(ImageAndTextLayoutPanel))]
		[Category(RadDesignCategory.BehaviorCategory)]
		public ContentAlignment TextAlignment
		{
			get
			{
				return (ContentAlignment) this.GetValue(TextAlignmentProperty);
			}
			set
			{
				this.SetValue(TextAlignmentProperty, value);
			}
		}

        /// <summary>
        ///     Gets or sets a value indicating the TextImageRelation: ImageAboveText,
        ///     ImageBeforeText, Overlay, TextAboveImage, and TextBeforeImage. 
        /// </summary>
		[RadPropertyDefaultValue("TextImageRelation", typeof(ImageAndTextLayoutPanel))]
		[Category(RadDesignCategory.BehaviorCategory)]
		public TextImageRelation TextImageRelation
		{
			get
			{
				return (TextImageRelation) this.GetValue(TextImageRelationProperty);
			}
			set
			{
				this.SetValue(TextImageRelationProperty, value);
			}
		}

        /// <summary>
        /// Gets or sets a value indicating the DisplayStyle: None, Image, Text, 
        /// and ImageAndText.
        /// </summary>
		[RadPropertyDefaultValue("DisplayStyle", typeof(ImageAndTextLayoutPanel))]
		[Category(RadDesignCategory.BehaviorCategory)]
        public DisplayStyle DisplayStyle
        {
            get
            {
                return (DisplayStyle)this.GetValue(DisplayStyleProperty);
            }
            set
            {                
                this.SetValue(DisplayStyleProperty, value);                
            }
        }

		private SizeF TextFullSize
		{
			get
			{
                if (this.textElement == null)
                {
                    return SizeF.Empty;
                }
				return this.UseNewLayoutSystem ? this.textElement.DesiredSize :this.textElement.FullSize;
			}
		}

		private SizeF ImageFullSize
		{
			get
			{
                if (this.imageElement == null)
                {
                    return SizeF.Empty;
                }
                return this.UseNewLayoutSystem ? this.imageElement.DesiredSize : this.imageElement.FullSize;
			}
		}

        [Obsolete]
        public override bool SuppressChildrenAlignment
        {
            get
            {
                return true;
            }
        }

		#endregion

		#region Methods

		private bool IsPrimitiveNullOrEmpty(RadElement element)
		{
            if ((element == null) || (element.Visibility == ElementVisibility.Collapsed))
			{
				return true;
			}
            BasePrimitive primitive = element as BasePrimitive;
            if (primitive != null)
	        {
                return primitive.IsEmpty;
	        }
			return false;
		}

        private bool IsImageElement(RadElement element)
        {
            return element != null && (bool)element.GetValue(IsImagePrimitiveProperty);
        }

        private bool IsTextElement(RadElement element)
        {
            return element != null && (bool)element.GetValue(IsTextPrimitiveProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldSize"></param>
        /// <param name="layoutField"></param>
        /// <param name="newLayouts"></param>
        public void LayoutTextAndImage(Size fieldSize, Rectangle layoutField, bool newLayouts)
        {
			Rectangle imageBounds = Rectangle.Empty;
            Rectangle textBounds = Rectangle.Empty;
            Size textSize = Size.Empty;
            Size imageSize = Size.Empty;

            ContentAlignment imageAlign = ImageAlignment;
            ContentAlignment textAlign = TextAlignment;
            TextImageRelation textImageRelation = TextImageRelation;

            //DK_2006_07_28
            if (!newLayouts)
            {
                List<PreferredSizeData> prefSizelist = new List<PreferredSizeData>();
                FillList(prefSizelist, fieldSize);
            }

            if (this.RightToLeft)
            {
                imageAlign = TelerikAlignHelper.RtlTranslateContent(ImageAlignment);
                textAlign = TelerikAlignHelper.RtlTranslateContent(TextAlignment);
                textImageRelation = TelerikAlignHelper.RtlTranslateRelation(TextImageRelation);
            }

            if (textElement != null)
            {
                textSize = newLayouts ? Size.Ceiling(textElement.DesiredSize) : Size.Ceiling(textElement.GetPreferredSize(fieldSize));
                textSize = this.GetInvariantLength(textSize, textElement.Margin);
            }
            if (imageElement != null)
            {
                imageSize = newLayouts ? Size.Ceiling(imageElement.DesiredSize) : Size.Ceiling(imageElement.GetPreferredSize(fieldSize));
                imageSize = this.GetInvariantLength(imageSize, imageElement.Margin);
            }

            if ((textElement != null) && IsPrimitiveNullOrEmpty(this.imageElement))
            {
				Rectangle bounds = LayoutUtils.Align(textSize, new Rectangle(Point.Empty, fieldSize), textAlign);
				bounds.Size = Size.Subtract(textSize, textElement.Margin.Size);
                if (newLayouts)
                    textElement.Arrange(bounds);
                else
				    textElement.Bounds = bounds;
                return;
            }
            if ((imageElement != null) && IsPrimitiveNullOrEmpty(this.textElement))
            {
				Rectangle bounds = LayoutUtils.Align(imageSize, new Rectangle(Point.Empty, fieldSize), imageAlign);
                bounds.Size = Size.Subtract(imageSize, imageElement.Margin.Size);
                if (newLayouts)
                    imageElement.Arrange(bounds);
                else
				    imageElement.Bounds = bounds;
                return;
            }
            // Subtract the image size from the whole field size
            Size textSizeLeft = LayoutUtils.SubAlignedRegion(fieldSize, imageSize, textImageRelation);
            textSize = newLayouts ? Size.Ceiling(this.textElement.DesiredSize) : Size.Ceiling(this.textElement.GetPreferredSize(textSizeLeft));
            textSize = this.GetInvariantLength(textSize, textElement.Margin);

            // Create a new size based on the text size and the image size
            Size newSize = LayoutUtils.AddAlignedRegion(textSize, imageSize, textImageRelation);
            // The new field which is a union of the new sizes
            Rectangle maxFieldRectangle = Rectangle.Empty;
            maxFieldRectangle.Size = LayoutUtils.UnionSizes(fieldSize, newSize);
            // Image doesn't overlay text
            bool imageAlignNoOverlay = (TelerikAlignHelper.ImageAlignToRelation(imageAlign) & textImageRelation) !=
                TextImageRelation.Overlay;
            // Text doesn't overlay image
            bool textAlignNoOverlay = (TelerikAlignHelper.TextAlignToRelation(textAlign) & textImageRelation) !=
                TextImageRelation.Overlay;

            if (imageAlignNoOverlay)
            {
                LayoutUtils.SplitRegion(maxFieldRectangle, imageSize, (AnchorStyles)textImageRelation, out imageBounds, out textBounds);
            }
            else if (textAlignNoOverlay)
            {
                LayoutUtils.SplitRegion(maxFieldRectangle, textSize, (AnchorStyles)LayoutUtils.GetOppositeTextImageRelation(textImageRelation), out textBounds, out imageBounds);
            }
            else
            {
                // Both image overlays text and text overlays image				
                if (textImageRelation == TextImageRelation.Overlay)
                {
                    LayoutUtils.SplitRegion(maxFieldRectangle, imageSize, (AnchorStyles)textImageRelation, out imageBounds, out textBounds);
                }
                else
                {
                    Rectangle alignedField = LayoutUtils.Align(newSize, maxFieldRectangle, ContentAlignment.MiddleCenter);
                    LayoutUtils.SplitRegion(alignedField, imageSize, (AnchorStyles)textImageRelation, out imageBounds, out textBounds);
                    LayoutUtils.ExpandRegionsToFillBounds(maxFieldRectangle, (AnchorStyles)textImageRelation, ref imageBounds, ref textBounds);
                }
            }
			//set image and text bounds according the size of the field
			if ((textImageRelation == TextImageRelation.TextBeforeImage) || (textImageRelation == TextImageRelation.ImageBeforeText))
			{
				int num1 = Math.Min(textBounds.Bottom, layoutField.Bottom);
				textBounds.Y = Math.Max(Math.Min(textBounds.Y, layoutField.Y + ((layoutField.Height - textBounds.Height) / 2)), layoutField.Y);
				textBounds.Height = num1 - textBounds.Y;
			}
			if ((textImageRelation == TextImageRelation.TextAboveImage) || (textImageRelation == TextImageRelation.ImageAboveText))
			{
				int num2 = Math.Min(textBounds.Right, layoutField.Right);
				textBounds.X = Math.Max(Math.Min(textBounds.X, layoutField.X + ((layoutField.Width - textBounds.Width) / 2)), layoutField.X);
				textBounds.Width = num2 - textBounds.X;
			}
			if ((textImageRelation == TextImageRelation.ImageBeforeText) && (imageBounds.Size.Width != 0))
			{
				imageBounds.Width = Math.Max(0, Math.Min(fieldSize.Width - textBounds.Width, imageBounds.Width));				
				textBounds.X = imageBounds.X + imageBounds.Width;
			}
			if ((textImageRelation == TextImageRelation.ImageAboveText) && (imageBounds.Size.Height != 0))
			{
				imageBounds.Height = Math.Max(0, Math.Min(fieldSize.Height - textBounds.Height, imageBounds.Height));
				textBounds.Y = imageBounds.Y + imageBounds.Height;
			}
			textBounds = Rectangle.Intersect(textBounds, layoutField);

			//align image and if its size is greater than the field's size it is become offseted as much as the difference
			Rectangle newImageBounds = LayoutUtils.Align(imageSize, imageBounds, imageAlign);
			if (newImageBounds.Width > imageBounds.Width)
				newImageBounds.X = imageBounds.Width - imageSize.Width;
			if (newImageBounds.Height > imageBounds.Height)
				newImageBounds.Y = imageBounds.Height - imageSize.Height;

			textBounds = LayoutUtils.Align(textSize, textBounds, textAlign);

			textBounds.Size = Size.Subtract(textBounds.Size, this.textElement.Margin.Size);
			imageBounds.Size = Size.Subtract(newImageBounds.Size, this.imageElement.Margin.Size);

            if (newLayouts)
            {
                this.textElement.Arrange(textBounds);
                this.imageElement.Arrange(newImageBounds);
            }
            else
            {
                this.textElement.Bounds = textBounds;
                this.imageElement.Bounds = newImageBounds;
            }
        }

        ///<summary>
        /// Performs layout changes based on the element given as a paramater. 
        /// Sizes and places are determined by the concrete layout panel that is used. 
        /// Since all layout panels update their layout automatically through events, 
        /// this function is rarely used directly.
        /// </summary>
		public override void PerformLayoutCore(RadElement affectedElement)
		{
			Size workingSize = this.Size;
            Rectangle layoutField = new Rectangle(Point.Empty, this.Bounds.Size);
            LayoutTextAndImage(workingSize, layoutField, false);
		}

		private SizeF CombineSizes(SizeF textSize, SizeF imageSize)
		{
            SizeF returnSize = LayoutUtils.UnionSizes(textSize, imageSize);
            if (LayoutUtils.IsVerticalRelation(this.TextImageRelation))
            {
                returnSize.Height = (textSize.Height + imageSize.Height);
            }
            else if (LayoutUtils.IsHorizontalRelation(this.TextImageRelation))
            {
                returnSize.Width = (textSize.Width + imageSize.Width);
            }
            return returnSize;
		}
        /// <summary>
        /// Retrieves the preferred size of the layout panel. If the proposed size is 
        /// smaller than the minimal one, the minimal one is retrieved. Since all layout 
        /// panels update their layout automatically through events, this function is 
        /// rarely used directly.
        /// </summary>
		public override Size GetPreferredSizeCore(Size proposedSize)
		{
            Size preferredSize = Size.Empty;

			if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
            {
                preferredSize = this.AvailableSize;
            }
            else
            {
                bool emptyImage = this.IsPrimitiveNullOrEmpty(this.imageElement);
                bool emptyText = this.IsPrimitiveNullOrEmpty(this.textElement);

                if (!emptyImage && !emptyText)
                {
                    preferredSize = Size.Ceiling( CombineSizes(this.TextFullSize, this.ImageFullSize) );
                }
                else if (emptyImage && emptyText)
                {
                    preferredSize = Size.Empty;
                }
                else
                {
					preferredSize = emptyImage ? this.textElement.FullSize : this.imageElement.FullSize;
                }
            }           
            return preferredSize;
		}

        private void SetTextAndImageVisibility()
        {
            ElementVisibility textVisibility = ElementVisibility.Visible;
            ElementVisibility imageVisibility = ElementVisibility.Visible;

            switch (this.DisplayStyle)
            {
                case DisplayStyle.Image:
                    textVisibility = ElementVisibility.Collapsed;
                    imageVisibility = ElementVisibility.Visible;
                    break;
                case DisplayStyle.Text:
                    textVisibility = ElementVisibility.Visible;
                    imageVisibility = ElementVisibility.Collapsed;
                    break;
                case DisplayStyle.ImageAndText:
                    textVisibility = ElementVisibility.Visible;
                    imageVisibility = ElementVisibility.Visible;
                    break;
                case DisplayStyle.None:
                    textVisibility = ElementVisibility.Collapsed;
                    imageVisibility = ElementVisibility.Collapsed;
                    break;
            }

            if (this.textElement != null)
            {
                this.textElement.Visibility = textVisibility;
            }
            if (this.imageElement != null)
            {
                this.imageElement.Visibility = imageVisibility;
            }
        }

        protected override void DisposeManagedResources()
        {
            this.imageElement = null;
            this.textElement = null;

            base.DisposeManagedResources();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.SetImageAndTextAlignment();
            this.SetTextAndImageVisibility();
        }

        private void SetImageAndTextAlignment()
        {
            if (!this.UseNewLayoutSystem)
            {
                return;
            }

            if (this.textElement != null)
            {
                this.textElement.Alignment = this.TextAlignment;
                TextPrimitive textPrimitive = this.textElement as TextPrimitive;
                if( textPrimitive!=null)
                {
                    textPrimitive.TextAlignment = this.TextAlignment;
                }
            }

            if (this.imageElement != null)
            {
                this.imageElement.Alignment = this.ImageAlignment;
            }
        }
		
		protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
		{
			base.OnChildrenChanged(child, changeOperation);

            switch(changeOperation)
            {
                case ItemsChangeOperation.Inserted:
                case ItemsChangeOperation.Set:
                    if (this.IsTextElement(child))
                    {
                        this.textElement = child;
                    }
                    else if (this.IsImageElement(child))
                    {
                        this.imageElement = child;
                    }
                    break;
                case ItemsChangeOperation.Removed:
                    if (this.IsTextElement(child))
                    {
                        this.textElement = null;
                    }
                    else if (this.IsImageElement(child))
                    {
                        this.imageElement = null;
                    }
                    break;
                case ItemsChangeOperation.Cleared:
                    this.textElement = null;
                    this.imageElement = null;
                    break;
            }

            this.SetTextAndImageVisibility();
		}

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (this.ElementState != ElementState.Loaded)
            {
                return;
            }

            if (e.Property == DisplayStyleProperty)
            {
				SetTextAndImageVisibility();
            }
            else if (e.Property == ImageAlignmentProperty || e.Property == TextAlignmentProperty)
            {
                this.SetImageAndTextAlignment();
            }
        }

        private Size GetInvariantLength(Size size, Padding margin)
        {            
            size.Height += margin.Vertical;            
            size.Width += margin.Horizontal;            
            return size;
        }

        private SizeF GetInvariantLength(SizeF size, Padding margin)
        {
            size.Height += margin.Vertical;
            size.Width += margin.Horizontal;
            return size;
        }

        private void FillList(List<PreferredSizeData> list, Size availableSize)
        {
            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                list.Add(new PreferredSizeData(child, availableSize));
            }
        }
        private int GetMaxHeight(List<PreferredSizeData> list)
        {
            int maxHeight = 0;
            foreach (PreferredSizeData data in list)
            {
                maxHeight = Math.Max(maxHeight, data.PreferredSize.Height);
            }
            return maxHeight;
        }
        private int GetMaxWidth(List<PreferredSizeData> list)
        {
            int maxWidth = 0;
            foreach (PreferredSizeData data in list)
            {
                maxWidth = Math.Max(maxWidth, data.PreferredSize.Width + data.Element.Margin.Horizontal);
            }
            return maxWidth;
        }

		#endregion
		
		#region New layout

		protected override SizeF MeasureOverride(SizeF availableSize)
		{
			SizeF preferredSize = SizeF.Empty;

            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
                child.Measure(availableSize);
            }

            bool emptyImage = this.IsPrimitiveNullOrEmpty(this.imageElement);
            bool emptyText = this.IsPrimitiveNullOrEmpty(this.textElement);

			if (!emptyImage && !emptyText)
			{
				preferredSize = CombineSizes(this.TextFullSize, this.ImageFullSize);				
			}
			else if (emptyImage && emptyText)
			{
				preferredSize = Size.Empty;				
			}
			else if (emptyImage && !emptyText)
			{
				preferredSize = this.TextFullSize;
			}
			else if (!emptyImage && emptyText)
			{
				preferredSize = this.ImageFullSize;
			}
			
			return preferredSize;
		}

		protected override SizeF ArrangeOverride(SizeF finalSize)
		{			
			SizeF fieldSize = finalSize;
			RectangleF layoutField = new RectangleF(PointF.Empty,finalSize);						
			
		    RectangleF imageBounds = RectangleF.Empty;
		    RectangleF textBounds = RectangleF.Empty;
		    SizeF textSize = SizeF.Empty;
		    SizeF imageSize = SizeF.Empty;

		    ContentAlignment imageAlign = this.ImageAlignment;
		    ContentAlignment textAlign = this.TextAlignment;
		    TextImageRelation textImageRelation = this.TextImageRelation;
					    
		    if (this.RightToLeft)
		    {
		        imageAlign = TelerikAlignHelper.RtlTranslateContent(ImageAlignment);
		        textAlign = TelerikAlignHelper.RtlTranslateContent(TextAlignment);
		        textImageRelation = TelerikAlignHelper.RtlTranslateRelation(TextImageRelation);
		    }

            if ((this.textElement != null) && IsPrimitiveNullOrEmpty(this.imageElement))
            {
                this.textElement.Arrange(layoutField);
                if (this.imageElement != null)
                    this.imageElement.Arrange(layoutField);
                return finalSize;
            }
            if ((this.imageElement != null) && IsPrimitiveNullOrEmpty(this.textElement))
            {
                this.imageElement.Arrange(layoutField);
                if (this.textElement != null)
                    this.textElement.Arrange(layoutField);
                return finalSize;
            }

			textSize = this.GetInvariantLength(textElement.DesiredSize, textElement.Margin);
			imageSize = this.GetInvariantLength(imageElement.DesiredSize, imageElement.Margin);
			
		    // Subtract the image size from the whole field size
		    SizeF textSizeLeft = LayoutUtils.SubAlignedRegion(fieldSize, imageSize, textImageRelation);		    			
		    // Create a new size based on the text size and the image size
		    SizeF newSize = LayoutUtils.AddAlignedRegion(textSize, imageSize, textImageRelation);
		    // The new field which is a union of the new sizes
		    RectangleF maxFieldRectangle = RectangleF.Empty;
		    maxFieldRectangle.Size = LayoutUtils.UnionSizes(fieldSize, newSize);
		    // Image doesn't overlay text
		    bool imageAlignNoOverlay = (TelerikAlignHelper.ImageAlignToRelation(imageAlign) & textImageRelation) !=
		        TextImageRelation.Overlay;
		    // Text doesn't overlay image
		    bool textAlignNoOverlay = (TelerikAlignHelper.TextAlignToRelation(textAlign) & textImageRelation) !=
		        TextImageRelation.Overlay;

		    if (imageAlignNoOverlay)
		    {
		        LayoutUtils.SplitRegion(maxFieldRectangle, imageSize, (AnchorStyles)textImageRelation, out imageBounds, out textBounds);
		    }
		    else if (textAlignNoOverlay)
		    {
		        LayoutUtils.SplitRegion(maxFieldRectangle, textSize, (AnchorStyles)LayoutUtils.GetOppositeTextImageRelation(textImageRelation), out textBounds, out imageBounds);
		    }
		    else
		    {
		        // Both image overlays text and text overlays image				
		        if (textImageRelation == TextImageRelation.Overlay)
		        {
		            LayoutUtils.SplitRegion(maxFieldRectangle, imageSize, (AnchorStyles)textImageRelation, out imageBounds, out textBounds);
		        }
		        else
		        {
		            RectangleF alignedField = LayoutUtils.Align(newSize, maxFieldRectangle, ContentAlignment.MiddleCenter);
		            LayoutUtils.SplitRegion(alignedField, imageSize, (AnchorStyles)textImageRelation, out imageBounds, out textBounds);
		            LayoutUtils.ExpandRegionsToFillBounds(maxFieldRectangle, (AnchorStyles)textImageRelation, ref imageBounds, ref textBounds);
		        }
		    }
				    

		    textBounds.Size = SizeF.Subtract(textBounds.Size, this.textElement.Margin.Size);
			imageBounds.Size = SizeF.Subtract(imageBounds.Size, this.imageElement.Margin.Size); 

		    this.textElement.Arrange(textBounds);
			this.imageElement.Arrange(imageBounds); 
		    		    
			return finalSize;
		}

		#endregion
	}
}