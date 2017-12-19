using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Design;
using Telerik.WinControls.Styles;
namespace Telerik.WinControls.UI
{
	/// <summary>Represents a trackbar thumb in the track bar control.</summary>
	[ToolboxItem(false), ComVisible(false)]
	public class TrackBarThumb : RadItem
	{
		private FillPrimitive backFill;
		private BorderPrimitive borderPrimitive;
		private RadTrackBarElement parentTrackBarElement;

		public static RadProperty ThumbWidthProperty = RadProperty.Register("ThumbWidth", typeof(int),
		typeof(TrackBarThumb),
		new RadElementPropertyMetadata(12, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

		public static RadProperty UseDefaultThumbShapeProperty = RadProperty.Register("UseDefaultThumbShape", typeof(bool),
		typeof(TrackBarThumb),
		new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		/// <summary>Initializes a new instance of the TrackbarThumb class.</summary>
		public TrackBarThumb()
		{
			this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
		}

        static TrackBarThumb()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(TrackBarThumb));
        }

		/// <summary>
		/// gets or sets Thumb's width
		/// </summary>
        /// 
        [Description("Gets or sets TrackBar's thumb width.")]
        [RadPropertyDefaultValue("ThumbWidth", typeof(TrackBarThumb))]
		public virtual int ThumbWidth
		{
			get
			{
				return (int)this.GetValue(ThumbWidthProperty);
			}
			set
			{
				this.SetValue(ThumbWidthProperty, value);
			}
		}

		/// <summary>
		/// gets or sets whether the trackbar's thumb should use its default shape
		/// </summary>
		public virtual bool UseDefaultThumbShape
		{
			get
			{
				return (bool)this.GetValue(UseDefaultThumbShapeProperty);
			}
			set
			{
				this.SetValue(UseDefaultThumbShapeProperty, value);
			}
		}

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
		{
			base.OnBubbleEvent(sender, args);		
		}

		internal void PerformMouseUp(MouseEventArgs e)
		{
			this.DoMouseUp(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
		}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == VisualElement.IsMouseOverProperty)
			{
				Debug.Write(this.backFill.IsMouseOver);
			}
		}

		/// <summary>
		/// gets ParentTrackBarElement
		/// </summary>
		public RadTrackBarElement ParentTrackBarElement
		{
			get
			{
				if (this.parentTrackBarElement == null)
				{
					for (RadElement res = this.Parent; res != null && this.parentTrackBarElement == null; res = res.Parent)
					{
						this.parentTrackBarElement = res as RadTrackBarElement;
					}
				}
				return this.parentTrackBarElement;
			}
		}

        /// <summary>
        /// CreateChildElements
        /// </summary>
		protected override void CreateChildElements()
		{
			this.backFill = new FillPrimitive();
			this.backFill.Class = "ThumbFillBack";
			this.backFill.BackColor = Color.Black;
			this.backFill.BackColor2 = Color.White;
			this.backFill.GradientStyle = GradientStyles.Linear;

			this.Children.Add(this.backFill);

			this.backFill.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

			this.borderPrimitive = new BorderPrimitive();
			this.borderPrimitive.Class = "TrackBarThumbBorder";

			this.Children.Add(this.borderPrimitive);

			this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

		}


        //protected override SizeF MeasureOverride(SizeF availableSize)
        //{
        //    SizeF res = base.MeasureOverride(availableSize);
        //    if (this.ParentTrackBarElement.TrackBarOrientation == Orientation.Horizontal)
        //    {
        //        res.Width = this.ThumbWidth;
        //    }
        //    else
        //    {
        //        res.Height = this.ThumbWidth;
        //    }

        //    return res;
        //}


        /// <summary>
        /// GetPreferredSizeCore
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        /// 
        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF res = base.MeasureOverride(availableSize);

            if (float.IsInfinity(availableSize.Width) || float.IsInfinity(availableSize.Height))
                return res;
            

            if (this.ParentTrackBarElement.TrackBarOrientation == Orientation.Horizontal)
            {
                if (!this.ParentTrackBarElement.FitToAvailableSize)
                    res = new SizeF(this.ThumbWidth, availableSize.Height / 2);
                else
                    res = new SizeF(this.ThumbWidth, availableSize.Height - 2);
            }
            else
            {
                if (!this.ParentTrackBarElement.FitToAvailableSize)
                {
                    if (this.AngleTransform % 180 == 0)
                    {
                        res = new SizeF(availableSize.Width / 2, this.ThumbWidth);
                    }
                    else
                    {
                        res = new SizeF(availableSize.Height / 2, this.ThumbWidth);
                    }
                }
                else
                {
                    if (this.AngleTransform % 180 == 0)
                    {
                        res = new SizeF(availableSize.Width - 2, this.ThumbWidth);
                    }
                    else
                    {
                        res = new SizeF(availableSize.Height - 2, this.ThumbWidth);
                    }
                }
            }

            return res;
        }
		public override Size GetPreferredSizeCore(Size proposedSize)
		{
			if (this.ParentTrackBarElement != null)
			{
				Size desiredSize = Size.Empty;

				if (this.ParentTrackBarElement.TrackBarOrientation == Orientation.Horizontal)
				{
					if (!this.ParentTrackBarElement.FitToAvailableSize)
						desiredSize = new Size(this.ThumbWidth, this.ParentTrackBarElement.Size.Height / 2);
					else
						desiredSize = new Size(this.ThumbWidth, this.ParentTrackBarElement.Size.Height - 2);

					this.backFill.Size = desiredSize;
					this.borderPrimitive.Size = new Size(desiredSize.Width + 2, desiredSize.Height + 2);
					
					return desiredSize;
				}
				else
				{
					if (!this.ParentTrackBarElement.FitToAvailableSize)
						desiredSize = new Size(this.ParentTrackBarElement.Size.Width / 2, this.ThumbWidth);
					else
						desiredSize = new Size(this.ParentTrackBarElement.Size.Width - 2, this.ThumbWidth);

					this.backFill.Size = desiredSize;
					this.borderPrimitive.Size = new Size(desiredSize.Width + 2, desiredSize.Height + 2);
					
					return desiredSize;
				}
			}
		
			return base.GetPreferredSizeCore(proposedSize);
		}

        /// <summary>
        /// PerformLayoutCore
        /// </summary>
        /// <param name="affectedElement"></param>
		public override void PerformLayoutCore(RadElement affectedElement)
		{
			if (this.ParentTrackBarElement != null)
			{
				if (this.ParentTrackBarElement.TrackBarOrientation == Orientation.Horizontal)
				{
					if ((this.ParentTrackBarElement.TickStyle == TickStyles.Both) || (this.ParentTrackBarElement.TickStyle == TickStyles.None))
					{
						ValueSource currentValueSource = this.GetValueSource(ShapeProperty);

						if (this.UseDefaultThumbShape)
						{
							if (currentValueSource != ValueSource.Style)
								this.Shape = null;
						}

						this.Location = new Point(this.Location.X, this.ParentTrackBarElement.Bounds.Height / 4);
					}
					else
						if (this.ParentTrackBarElement.TickStyle == TickStyles.BottomRight)
						{
							ValueSource currentValueSource = this.GetValueSource(ShapeProperty);
							if (this.UseDefaultThumbShape)
							{
								if (currentValueSource != ValueSource.Style)
									this.Shape = new TrackBarDThumbShape();
							}

							this.Location = new Point(this.Location.X, 0);
						}
						else
						{
							ValueSource currentValueSource = this.GetValueSource(ShapeProperty);
							if (this.UseDefaultThumbShape)
							{
								if (currentValueSource != ValueSource.Style)
									this.Shape = new TrackBarUThumbShape();
							}
	
							this.Location = new Point(this.Location.X, ( ( this.ParentTrackBarElement.Bounds.Height / 2 )- 2 ) );
						}
					if (this.ParentTrackBarElement.FitToAvailableSize)
					{
						ValueSource currentValueSource = this.GetValueSource(ShapeProperty);
						if (this.UseDefaultThumbShape)
						{

							if (currentValueSource != ValueSource.Style)
								this.Shape = new EllipseShape();
						}

						this.Location = new Point(this.Location.X, 0);
					}
				}
				else
				{
					if ((this.ParentTrackBarElement.TickStyle == TickStyles.Both) || (this.ParentTrackBarElement.TickStyle == TickStyles.None))
					{
						ValueSource currentValueSource = this.GetValueSource(ShapeProperty);
						if (this.UseDefaultThumbShape)
						{
							if (currentValueSource != ValueSource.Style)
								this.Shape = null;
						}

						this.Location = new Point(this.ParentTrackBarElement.Bounds.Width / 4, this.Location.Y);
					}
					else
						if (this.ParentTrackBarElement.TickStyle == TickStyles.BottomRight)
						{
							ValueSource currentValueSource = this.GetValueSource(ShapeProperty);
							if (this.UseDefaultThumbShape)
							{
								if (currentValueSource != ValueSource.Style)
									this.Shape = new TrackBarRThumbShape();
							}
	
							this.Location = new Point(0, this.Location.Y);
						}
						else
						{
							ValueSource currentValueSource = this.GetValueSource(ShapeProperty);
							if (this.UseDefaultThumbShape)
							{
								if (currentValueSource != ValueSource.Style)
									this.Shape = new TrackBarLThumbShape();
							}
	
							this.Location = new Point( ( this.ParentTrackBarElement.Bounds.Width / 2 ) - 2, this.Location.Y);
						}
					if (this.ParentTrackBarElement.FitToAvailableSize)
					{
						ValueSource currentValueSource = this.GetValueSource(ShapeProperty);
						if (this.UseDefaultThumbShape)
						{
							if (currentValueSource != ValueSource.Style)
								this.Shape = new EllipseShape();
						}
						 
						this.Location = new Point(0, this.Location.Y);
					}

				}
			}
			base.PerformLayoutCore(affectedElement);
		}
	}
}
