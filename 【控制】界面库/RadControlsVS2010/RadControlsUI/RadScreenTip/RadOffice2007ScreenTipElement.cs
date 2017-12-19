using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Primitives;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
	public class RadOffice2007ScreenTipElement : RadScreenTipElement
	{
		public static RadProperty CaptionVisibleProperty = RadProperty.Register(
			"CaptionVisible", typeof(bool), typeof(RadLabelElement), new RadElementPropertyMetadata(
			true, ElementPropertyOptions.AffectsLayout));

		public static RadProperty FooterVisibleProperty = RadProperty.Register(
			"FooterVisible", typeof(bool), typeof(RadLabelElement), new RadElementPropertyMetadata(
			false, ElementPropertyOptions.AffectsLayout));

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            this.Shape = new RoundRectShape(2);
        }

        /// <summary>
        /// The property is always set to true. RadElements may override the 
        /// default layout. Layout panels are RadElements and always override the 
        /// default layout.
        /// </summary>
        public override bool OverridesDefaultLayout
        {
            get
            {
                return true;
            }
        }

		static RadOffice2007ScreenTipElement()
		{
			ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Resources.ScreenTipThemes.Office2007Blue.xml");
		}
			
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CaptionVisible
		{
			get
			{
				return (bool) this.GetValue(CaptionVisibleProperty);
			}
			set
			{
				this.SetValue(CaptionVisibleProperty, value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FooterVisible
		{
			get
			{
				return (bool) this.GetValue(FooterVisibleProperty);
			}
			set
			{
				this.SetValue(FooterVisibleProperty, value);
			}
		}

        private StackLayoutPanel screenTipPanel;
		private RadLabelElement captionLabel;
		private RadLineItem footerLine;
	    private RadLabelElement mainText;
        private RadLabelElement footerText;

        private FillPrimitive fillPrimitive;
        private BorderPrimitive borderPrimitive;

		protected override void CreateChildElements()
		{
			fillPrimitive = new FillPrimitive();
			fillPrimitive.Class = "ScreenTipFill";
			this.Children.Add(fillPrimitive);           
            
			
			borderPrimitive = new BorderPrimitive();
			borderPrimitive.Class = "ScreenTipBorder";
			this.Children.Add(borderPrimitive);

			screenTipPanel = new StackLayoutPanel();
			screenTipPanel.Orientation = Orientation.Vertical;
      
			captionLabel = new RadLabelElement();
			captionLabel.Text = "ScreenTipCaptionText";
			captionLabel.Class = "ScreenTipCaptionText";
            captionLabel.TextWrap = true;
			screenTipPanel.Children.Add(captionLabel);

			mainText = new RadLabelElement();
			mainText.Text = "ScreenTip Text";
			mainText.Class = "ScreenTipText";
            mainText.TextWrap = true;
            screenTipPanel.Children.Add(mainText);

			footerLine = new RadLineItem();
			footerLine.MaxSize = new Size(0, 4);
			footerLine.Alignment = ContentAlignment.BottomCenter;
			footerLine.Class = "ScreenTipFooterLine";
			screenTipPanel.Children.Add(footerLine);

			footerText = new RadLabelElement();
			footerText.Text = "ScreenTip Footer";
			footerText.Class = "ScreenTipFooterText";
            footerText.TextWrap = true;
            screenTipPanel.Children.Add(footerText);

			this.Children.Add(screenTipPanel);
			
			this.footerLine.Visibility = ElementVisibility.Collapsed;
            this.footerText.Visibility = ElementVisibility.Collapsed;
		
			this.Items.Add(captionLabel);
			this.Items.Add(mainText);
			this.Items.Add(footerLine);
			this.Items.Add(footerText);
		}

        //protected override SizeF MeasureOverride(SizeF availableSize)
        //{
        //    SizeF desiredSize = SizeF.Empty;
        //    foreach (RadElement element in this.Children)
        //    {
        //        element.Measure(availableSize);
        //        desiredSize.Width = Math.Max(desiredSize.Width, element.DesiredSize.Width);
        //        desiredSize.Height += element.DesiredSize.Height;
        //    }
        //    return size;
        //}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if(e.Property == CaptionVisibleProperty)
			{
				bool visible = (bool)e.NewValue;
				
				this.CaptionLabel.Visibility = (visible) ? 
					ElementVisibility.Visible : ElementVisibility.Collapsed;
			}
			else if(e.Property == FooterVisibleProperty)
			{
				bool visible = (bool)e.NewValue;
				
				this.FooterLine.Visibility = visible ?
					ElementVisibility.Visible : ElementVisibility.Collapsed;

				this.footerText.Visibility = (visible) ?
					ElementVisibility.Visible : ElementVisibility.Collapsed;	
			}
			
			base.OnPropertyChanged(e);
		}

        /// <summary>
        /// Gets the element that displays the caption
        /// </summary>
        [Category("Behavior")]
        [Description("Represents the element that displays the caption")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	    public RadLabelElement CaptionLabel
	    {
	        get 
            { 
                return captionLabel; 
            }
	    }

        /// <summary>
        /// Gets the element that displays the footer line
        /// </summary>
        [Category("Behavior")]
        [Description("Represents the element that displays the footer line")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	    public RadLineItem FooterLine
	    {
	        get
	        {
                return footerLine;
	        }
	    }        

        /// <summary>
        /// Gets the element that displays the Image
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Represents the element that displays the Image")]
        [Obsolete("Please, use the Image property of MainTextLabel.")]
	    public ImagePrimitive MainImage
	    {
	        get
	        {
                return this.mainText.imagePrimitive;
	        }
	    }

        /// <summary>
        /// Gets the element that displays the Text
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Represents the element that displays the Text")]
	    public RadLabelElement MainTextLabel
	    {
	        get
	        {
                return mainText;
	        }
	    }

        /// <summary>
        /// Gets the element that displays the Footer
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Represents the element that displays the Footer")]
        public RadLabelElement FooterTextLabel
        {
            get
            {
                return footerText;
            }
        }

        /// <summary>
        /// Gets the FillPrimitive instance that represents
        /// the screen tip fill.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public FillPrimitive ScreenTipFill
        {
            get
            {
                return this.fillPrimitive;
            }
        }

        /// <summary>
        /// Gets the BorderPrimitive instance that represents
        /// the screen tip border.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BorderPrimitive ScreenTipBorder
        {
            get
            {
                return this.borderPrimitive;
            }
        }
	}
}
