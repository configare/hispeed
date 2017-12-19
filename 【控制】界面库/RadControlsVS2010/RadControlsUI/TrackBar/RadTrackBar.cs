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
using Telerik.WinControls.UI;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Design;
using Telerik.WinControls.Enumerations;
namespace Telerik.WinControls.UI
{
	/// <summary>
	///     Represents a track bar. The trackbar class is essentially a simple wrapper 
	///     for the <see cref="RadTrackBarElement">RadTrackBarElement</see>. All UI and 
	///     logic functionality is implemented in the
	///     <see cref="RadTrackBarElement">RadTrackBarElement</see> class. The RadTrackBar acts
	///     to transfer the events to and from its corresponding
	///     <see cref="RadTrackBarElement">RadTrackBarElement</see> instance. The
	///     <see cref="RadTrackBarElement">RadTrackBarElement</see> may be nested in other
	///     telerik controls.
	/// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.EditorsGroup)]
    [RadThemeDesignerData(typeof(RadTrackBarDesignTimeData))]
	[ToolboxItem(true)]
	[Description("A slider control that enables the user to select a value on a bar by moving a slider")]
	[DefaultProperty("Value"), DefaultEvent("ValueChanged")]
	public class RadTrackBar : RadControl
	{
        private RadTrackBarElement trackBarElement;

        ///// <commentsfrom cref="RadTrackBarElement.Scroll" filter=""/>
        //[Category(RadDesignCategory.BehaviorCategory)]
        //[RadDescription("Scroll", typeof(RadTrackBarElement))]
        //public event EventHandler Scroll;

		/// <commentsfrom cref="RadTrackBarElement.ValueChanged" filter=""/>
		[Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("ValueChanged", typeof(RadTrackBarElement))]
		public event EventHandler ValueChanged;

		/// <summary>Initializes a new instance of the RadTrackBar class.</summary>
		public RadTrackBar()
		{
			this.AutoSize = true;
		}

        /// <summary>
        /// Gets the instance of RadTrackBarElement wrapped by this control. RadTrackBarElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadTrackBar.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadTrackBarElement TrackBarElement
		{
			get
			{
				return this.trackBarElement;
			}
		}

		[Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
			}
		}


        /// <commentsfrom cref="RadTrackBarItem.SliderAreaGradientColor1" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadDescription("SliderAreaGradientColor1", typeof(RadTrackBarElement))]
        [RadDefaultValue("SliderAreaGradientColor1", typeof(RadTrackBarElement))]
		public Color SliderAreaColor1
		{
			get
			{
				return this.trackBarElement.SliderAreaGradientColor1;
			}
			set
			{
				this.trackBarElement.SliderAreaGradientColor1 = value;
			}
		}


		/// <commentsfrom cref="RadTrackBarItem.TickColor" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadDescription("TickColor", typeof(RadTrackBarElement))]
        [RadDefaultValue("TickColor", typeof(RadTrackBarElement))]
		public Color TicksColor
		{
			get
			{
				return this.trackBarElement.TickColor;
			}
			set
			{
				this.trackBarElement.TickColor = value;
			}
		}

        /// <commentsfrom cref="RadTrackBarItem.SliderAreaGradientColor2" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadDescription("SliderAreaGradientColor2", typeof(RadTrackBarElement))]
        [RadDefaultValue("SliderAreaGradientColor2", typeof(RadTrackBarElement))]
		public Color SliderAreaColor2
		{
			get
			{
				return this.trackBarElement.SliderAreaGradientColor2;
			}
			set
			{
				this.trackBarElement.SliderAreaGradientColor2 = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarItem.SliderAreaGradientAngle" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDescription("SliderAreaGradientAngle", typeof(RadTrackBarElement))]
		[RadDefaultValue("SliderAreaGradientAngle", typeof(RadTrackBarElement))]
		public float SliderAreaGradientAngle
		{
			get
			{
				return this.trackBarElement.SliderAreaGradientAngle;
			}
			set
			{
				this.trackBarElement.SliderAreaGradientAngle = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarItem.Minimum" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("Minimum", typeof(RadTrackBarElement))]
		[RadDefaultValue("Minimum", typeof(RadTrackBarElement))]
		public int Minimum
		{
			get
			{
				return this.trackBarElement.Minimum;
			}
			set
			{
				this.trackBarElement.Minimum = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarElement.ThumbWidth" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadDescription("ThumbWidth", typeof(TrackBarThumb))]
		[RadDefaultValue("ThumbWidth", typeof(TrackBarThumb))]
		public int ThumbWidth
		{
			get
			{
				return this.trackBarElement.ThumbWidth;
			}
			set
			{
				this.trackBarElement.ThumbWidth = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarItem.Maximum" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("Maximum", typeof(RadTrackBarElement))]
		[RadDefaultValue("Maximum", typeof(RadTrackBarElement))]
		public int Maximum
		{
			get
			{
				return this.trackBarElement.Maximum;
			}
			set
			{
				this.trackBarElement.Maximum = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarItem.ShowTicks" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDescription("ShowTicks", typeof(RadTrackBarElement))]
		[RadDefaultValue("ShowTicks", typeof(RadTrackBarElement))]
		public bool ShowTicks
		{
			get
			{
				return this.trackBarElement.ShowTicks;
			}
			set
			{
				this.trackBarElement.ShowTicks = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarItem.ShowSlideArea" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDescription("ShowSlideArea", typeof(RadTrackBarElement))]
		[RadDefaultValue("ShowSlideArea", typeof(RadTrackBarElement))]
		public bool ShowSlideArea
		{
			get
			{
				return this.trackBarElement.ShowSlideArea;
			}
			set
			{
				this.trackBarElement.ShowSlideArea = value;
			}
		}

        /// <commentsfrom cref="RadTrackBarItem.FitToAvailableSize" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadDescription("FitToAvailableSize", typeof(RadTrackBarElement))]
        [RadDefaultValue("FitToAvailableSize", typeof(RadTrackBarElement))]
		public bool FitTrackerToSize
		{
			get
			{
				return this.trackBarElement.FitToAvailableSize;
			}
			set
			{
				this.trackBarElement.FitToAvailableSize = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarItem.LargeChange" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("LargeChange", typeof(RadTrackBarElement))]
		[RadDefaultValue("LargeChange", typeof(RadTrackBarElement))]
		public int LargeChange
		{
			get
			{
				return this.trackBarElement.LargeChange;
			}
			set
			{
				this.trackBarElement.LargeChange = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarItem.Value" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("Value", typeof(RadTrackBarElement))]
		[RadDefaultValue("Value", typeof(RadTrackBarElement))]
		public int Value
		{
			get
			{
				return this.trackBarElement.Value;
			}
			set
			{
				this.trackBarElement.Value = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarItem.TickFrequency" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("TickFrequency", typeof(RadTrackBarElement))]
		[RadDefaultValue("TickFrequency", typeof(RadTrackBarElement))]
		public int TickFrequency
		{
			get
			{
				return this.trackBarElement.TickFrequency;
			}
			set
			{
				this.trackBarElement.TickFrequency = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarItem.TickStyle" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("TickStyle", typeof(RadTrackBarElement))]
		[RadDefaultValue("TickStyle", typeof(RadTrackBarElement))]
		public TickStyles TickStyle
		{
			get
			{
				return this.trackBarElement.TickStyle;
			}
			set
			{
				this.trackBarElement.TickStyle = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarItem.SlideAreaWidth" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDescription("SlideAreaWidth", typeof(RadTrackBarElement))]
		[RadDefaultValue("SlideAreaWidth", typeof(RadTrackBarElement))]
		public int SlideAreaWidth
		{
			get
			{
				return this.trackBarElement.SlideAreaWidth;
			}
			set
			{
				this.trackBarElement.SlideAreaWidth = value;
			}
		}

		/// <commentsfrom cref="RadTrackBarItem.TrackBarOrientation" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("Orientation", typeof(RadTrackBarElement))]
		[RadDefaultValue("TrackBarOrientation", typeof(RadTrackBarElement))]
		public Orientation Orientation
		{
			get
			{
				return this.trackBarElement.TrackBarOrientation;
			}
			set
			{
				if (value != this.trackBarElement.TrackBarOrientation)
                {
                    this.trackBarElement.TrackBarOrientation = value;

                    int width = this.Width;
                    this.Width = this.Height;
                    this.Height = width;

                }
			}
		}

		public virtual void OnValueChanged(EventArgs e)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, e);
			}
		}


        protected override void CreateChildItems(RadElement parent)
        {
            this.trackBarElement = new RadTrackBarElement();
            this.trackBarElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.RootElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.RootElement.Children.Add(this.trackBarElement);

            this.trackBarElement.Scroll += delegate(object sender, ScrollEventArgs args) { OnScroll(args); };
            this.trackBarElement.ValueChanged += delegate(object sender, EventArgs args) { OnValueChanged(args); };

            base.CreateChildItems(this.trackBarElement);
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(150, 30);
            }
        }
	}
}
