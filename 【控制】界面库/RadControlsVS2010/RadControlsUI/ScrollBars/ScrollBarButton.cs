using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;
using VisualStyles = System.Windows.Forms.VisualStyles;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a scrollbar button. There are two buttons in the implementation of the
    /// RadScrollBar: FirstButton and SecondButton.
    /// </summary>
	[ToolboxItem(false), ComVisible(false)]
	public class ScrollBarButton : RadButtonItem
    {
        #region Fields

        private ArrowPrimitive arrowPrimitive;
        private FillPrimitive background;
        private BorderPrimitive borderPrimitive;
        private ScrollButtonDirection direction;

        #endregion

        #region Constructor & Initialization

        /// <summary><para>Initializes a new instance of the ScrollBarButton class.</para></summary>
        public ScrollBarButton()
        {
        }

        static ScrollBarButton()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ScrollButtonStateManagerFactory(), typeof(ScrollBarButton));
        }

        /// <summary>
        /// 	<para>Initializes a new instance of the ScrollBarButton class using
        ///     scrollButtonDirection.</para>
        /// </summary>
        public ScrollBarButton(ScrollButtonDirection direction)
        {
            this.Direction = direction;
        }

        protected override void CreateChildElements()
        {
            this.background = new FillPrimitive();
            this.background.ZIndex = 0;
            this.background.Class = "ScrollBarButtonFill";
            this.Children.Add(this.background);

            this.arrowPrimitive = new ArrowPrimitive(GetArrowDirection(this.Direction));
            this.arrowPrimitive.Alignment = ContentAlignment.MiddleCenter;
            this.arrowPrimitive.ZIndex = 1;
            this.arrowPrimitive.Class = "ScrollBarButtonArrow";
            this.Children.Add(this.arrowPrimitive);

            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "ScrollBarButtonBorder";
            this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.Children.Add(this.borderPrimitive);

            this.background.BindProperty(RadButtonItem.IsPressedProperty, this, RadButtonItem.IsPressedProperty, PropertyBindingOptions.OneWay);
            this.arrowPrimitive.BindProperty(RadButtonItem.IsPressedProperty, this, RadButtonItem.IsPressedProperty, PropertyBindingOptions.OneWay);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating the <see cref="ScrollButtonDirection">button
        ///     direction</see> defined in the ScrollButtonDirection enumeration: up, right,
        ///     buttom, and left.
        /// </summary>
		public ScrollButtonDirection Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				if (this.direction != value)
                {
                    this.direction = value;
                    if (this.arrowPrimitive != null)
                    {
                        this.arrowPrimitive.Direction = GetArrowDirection(direction);
                    }
                }
			}
		}

        /// <summary>
        /// Gets an instance of <see cref="ArrowPrimitive"/> contained in the button.
        /// </summary>
        public ArrowPrimitive ArrowPrimitive
        {
            get
            {
                return this.arrowPrimitive;
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="BorderPrimitive"/> contained in the button.
        /// </summary>
        public BorderPrimitive ButtonBorder
        {
            get
            {
                return borderPrimitive;
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="FillPrimitive"/> contained in the button.
        /// </summary>
        public FillPrimitive ButtonFill
        {
            get
            {
                return background;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the default layout is overridden.
        /// </summary>
        public override bool OverridesDefaultLayout
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Layout

        /// <commentsfrom cref="Telerik.WinControls.Layouts.IRadLayoutElement.PerformLayout" filter=""/>
        public override void PerformLayoutCore(RadElement affectedElement)
        {
            //base.PerformLayoutCore(affectedElement);
            //this.arrowPrimitive.Size = Size.Empty;

            List<PreferredSizeData> list = new List<PreferredSizeData>();
            FillList(list);

            foreach (PreferredSizeData data in list)
            {
				if (data.Element != this.arrowPrimitive)
				{
					data.Element.Size = this.Size;
					continue;
				}

                int sizeValue = Math.Min(this.Size.Width, this.Size.Height);
                int newSize = sizeValue / 3;
                //if ((sizeValue % 3) > 1) newSize++;
                //this.arrowPrimitive.SetBounds(
                //    (this.Size.Width - newSize) /2,
                //    (this.Size.Height - newSize) / 2,
                //    newSize, newSize);
                this.arrowPrimitive.Size = new Size(newSize, newSize);
            }

        }

        /// <commentsfrom cref="Telerik.WinControls.Layouts.IRadLayoutElement.GetPreferredSize" filter=""/>
		public override Size GetPreferredSizeCore(Size proposedSize)
		{
            return new Size(this.arrowPrimitive.Size.Width * 3, this.Parent.Size.Height);
		}

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF res = base.MeasureOverride(availableSize);
            
            if (this.Direction == ScrollButtonDirection.Down || this.Direction == ScrollButtonDirection.Up)
            {
                if (availableSize.Height > res.Height)
                {
                    float expandedHeight = res.Height + ( 2 * this.arrowPrimitive.DesiredSize.Height );
                    res.Height = Math.Min(expandedHeight, availableSize.Height);
                }
            }
            else
            {
                if (availableSize.Width > res.Width)
                {
                    float expandedWidth = res.Width + ( 2 * this.arrowPrimitive.DesiredSize.Width );
                    res.Width = Math.Min(expandedWidth, availableSize.Height);
                }
            }

            return res;
        }

        private void FillList(List<PreferredSizeData> list)
        {
            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                list.Add(new PreferredSizeData(child, this.Size));
            }
        }

        #endregion

        #region System skinning

        public override System.Windows.Forms.VisualStyles.VisualStyleElement GetXPVisualStyle()
        {
            RadScrollBarElement parentScroll = this.Parent as RadScrollBarElement;
            if (parentScroll.ScrollType == ScrollType.Horizontal)
            {
                if (this.Class == "ScrollBarFirstButton")
                {
                    if (!this.Enabled)
                    {
                        return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.LeftDisabled;
                    }
                    else
                    {
                        if (!this.IsPressed && !this.IsMouseOver)
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.LeftNormal;
                        }
                        else if (this.IsPressed && this.IsMouseOver)
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.LeftPressed;
                        }
                        else
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.LeftHot;
                        }
                    }
                }
                else
                {
                    if (!this.Enabled)
                    {
                        return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.RightDisabled;
                    }
                    else
                    {
                        if (!this.IsPressed && !this.IsMouseOver)
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.RightNormal;
                        }
                        else if (this.IsPressed && this.IsMouseOver)
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.RightPressed;
                        }
                        else
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.RightHot;
                        }
                    }
                }

            }
            else
            {
                if (this.Class == "ScrollBarSecondButton")
                {
                    if (!this.Enabled)
                    {
                        return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.DownDisabled;
                    }
                    else
                    {
                        if (!this.IsPressed && !this.IsMouseOver)
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.DownNormal;
                        }
                        else if (this.IsPressed && this.IsMouseOver)
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.DownPressed;
                        }
                        else
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.DownHot;
                        }
                    }
                }
                else
                {
                    if (!this.Enabled)
                    {
                        return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.UpDisabled;
                    }
                    else
                    {
                        if (!this.IsPressed && !this.IsMouseOver)
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.UpNormal;
                        }
                        else if (this.IsPressed && this.IsMouseOver)
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.UpPressed;
                        }
                        else
                        {
                            return VisualStyles.VisualStyleElement.ScrollBar.ArrowButton.UpHot;
                        }
                    }
                }
            }
        }

        public override System.Windows.Forms.VisualStyles.VisualStyleElement GetVistaVisualStyle()
        {
            return this.GetXPVisualStyle();
        }

        protected override bool ShouldPaintChild(RadElement element)
        {
            if (this.paintSystemSkin == true)
            {
                if (element is FillPrimitive || element is BorderPrimitive
                    || element is ArrowPrimitive)
                {
                    return false;
                }
            }
            return base.ShouldPaintChild(element);
        }

        #endregion

        private ArrowDirection GetArrowDirection(ScrollButtonDirection buttonDirection)
        {
            return (ArrowDirection)buttonDirection;
        }
    }
}