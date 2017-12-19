using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a RadRepeatButton. If the button is continuously held pressed, it
    ///     generates clicks. The RadRepeatButton class is a simple wrapper for the
    ///     <see cref="RadRepeatButtonElement">RadRepeatButtonElement</see> class. The
    ///     RadRepeatButton acts to transfer events to and from its corresponding
    ///     <see cref="RadRepeatButtonElement">RadRepeatButtonElement</see> instance. The
    ///     <see cref="RadRepeatButtonElement">RadRepeatButtonElement</see> which is
    ///     essentially the <see cref="RadRepeatButtonElement">RadRepeatButtonElement</see>
    ///     control may be nested in other telerik controls. All graphical and logical
    ///     functionality is implemented in
    ///     <see cref="RadRepeatButtonElement">RadRepeatButtonElement</see> class.
    /// </summary>
	[RadThemeDesignerData(typeof(RadRepeatButtonDesignTimeData))]
	#if RIBBONBAR
	[ToolboxItem(false)]
    internal partial class RadRepeatButton : RadButton
	#else
    [ToolboxItem(true)]
	[Description("Offers click-and-hold functionality built directly within the ButtonClick event")]
	[DefaultProperty("Text"), DefaultEvent("Click")]
	public partial class RadRepeatButton : RadButton
	#endif
    {
        #region Events
        /// <summary>
        /// Propagates internal element click.  
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Propagates internal element click."),
        EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler ButtonClick
        {
            add
            {
                this.Events.AddHandler(RadRepeatButton.ButtonClickEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadRepeatButton.ButtonClickEventKey, value);
            }
        }
        /// <summary>
        /// Raises the ButtonClick event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnButtonClick(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)base.Events[RadRepeatButton.ButtonClickEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }
        internal virtual void PerformButtonClick() 
        {
            this.OnButtonClick(EventArgs.Empty);
        }
        #endregion

        // Fields        
		private static readonly object ButtonClickEventKey;

		static RadRepeatButton()
		{
			ButtonClickEventKey = new object();
		}

        /// <summary>Initializes a new instance of the RadRepeatButton class.</summary>
        public RadRepeatButton()
        {
            
        }

        #region Properties

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 23);
            }
        }

        /// <summary>
        /// Gets the instance of RadRepeatButtonElement wrapped by this control. RadRepeatButtonElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadRepeatButton.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new RadRepeatButtonElement ButtonElement
		{
			get { return (RadRepeatButtonElement)base.ButtonElement; }
		}

        /// <summary>
        /// Determines whether the button can be clicked by using mnemonic characters.
        /// </summary>
        [Category("Appearance"),
        Description("Determines whether the button can be clicked by using mnemonic characters."),
        DefaultValue(true)]
        new public bool UseMnemonic
        {
            get
            {
                return this.ButtonElement.TextElement.UseMnemonic;
            }
            set
            {
                this.ButtonElement.TextElement.UseMnemonic = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, the Repeat button element waits while it is pressed before it starts repeating. The value must be non-negative. 
        /// </summary>
        [Bindable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("Delay", typeof(RadRepeatButtonElement))]
        [Description("Gets or sets the amount of time, in milliseconds, the Repeat button element waits while it is pressed before it starts repeating. The value must be non-negative.")]
        public int Delay
        {
            get
            {
                return this.ButtonElement.Delay;
            }
            set
            {
                this.ButtonElement.Delay = value;
            }
        }
        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, between repeats once repeating starts. The value must be non-negative.
        /// </summary>
        [Bindable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("Interval", typeof(RadRepeatButtonElement))]
        [Description("Gets or sets the amount of time, in milliseconds, between repeats once repeating starts. The value must be non-negative.")]
        public int Interval
        {
            get
            {
                return this.ButtonElement.Interval;
            }
            set
            {
                this.ButtonElement.Interval = value;
            }
        }
        #endregion

        /// <summary>
        /// Create main button element that is specific for RadRepeatButton.
        /// </summary>
        /// <returns>The element that encapsulates the funtionality of RadRepeatButton</returns>
        protected override RadButtonElement CreateButtonElement()
        {
            RadRepeatButtonElement res = new RadRepeatButtonElement();
            res.UseNewLayoutSystem = true;
            return res;
        }
    }

    /// <summary>Represents a helper class used by the Visual Style Builder.</summary>
    ///<exclude/>
	public class RadRepeatButtonDesignTimeData : RadControlDesignTimeData
	{
		public RadRepeatButtonDesignTimeData()
		{ }

		public RadRepeatButtonDesignTimeData(string name)
			: base(name)
		{ }

		public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
		{
			RadRepeatButton button = new RadRepeatButton();
            button.AutoSize = false;
            button.Size = new Size(120, 65);

			button.Text = "RadRepeatButton";

			RadRepeatButton buttonStructure = new RadRepeatButton();
			button.AutoSize = true;

			buttonStructure.Text = "RadRepeatButton";

			ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(button, buttonStructure.RootElement);
			designed.MainElementClassName = typeof(RadRepeatButtonElement).FullName;
			ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

			res.Add(designed);

			return res;
		}
	}
}
