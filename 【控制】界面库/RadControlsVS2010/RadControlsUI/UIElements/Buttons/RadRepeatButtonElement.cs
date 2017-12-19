using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using Telerik.WinControls.Design;
namespace Telerik.WinControls.UI
{
    /// <summary>Represents a repeat button element, and like all elements can be nested 
    /// in other telerik controls. RadRepeatButton is essentially a simple wrapper for 
    /// RadRepeatButtonElement. All UI and logic functionality is implemented in the
    /// RadRepeatButtonElement class. RadRepeatButton acts to transfer events to and from 
    /// the RadRepeatButton class.</summary>
    public class RadRepeatButtonElement : RadButtonElement
    {
        static RadRepeatButtonElement()
        {
			new Themes.ControlDefault.RepeatButton().DeserializeTheme();
            RadItem.ClickModeProperty.OverrideMetadata(typeof(RadRepeatButtonElement), new RadElementPropertyMetadata(ClickMode.Press, ElementPropertyOptions.None));
        }

        // Fields
        private Timer timer;
        public static readonly RadProperty DelayProperty = RadProperty.Register("Delay", typeof(int), typeof(RadRepeatButtonElement), new RadElementPropertyMetadata(RadRepeatButtonElement.GetKeyboardDelay(), ElementPropertyOptions.None), new ValidateValueCallback(RadRepeatButtonElement.IsDelayValid));
        public static readonly RadProperty IntervalProperty = RadProperty.Register("Interval", typeof(int), typeof(RadRepeatButtonElement), new RadElementPropertyMetadata(RadRepeatButtonElement.GetKeyboardSpeed(), ElementPropertyOptions.None), new ValidateValueCallback(RadRepeatButtonElement.IsIntervalValid));

        public RadRepeatButtonElement()
        {
        }

        public RadRepeatButtonElement(string text)
            : base(text)
        {
        }

        internal static int GetKeyboardDelay()
        {
            int num1 = SystemInformation.KeyboardDelay;
            if ((num1 < 0) || (num1 > 3))
            {
                num1 = 0;
            }
            return ((num1 + 1) * 250);
        }
        internal static int GetKeyboardSpeed()
        {
            int num1 = SystemInformation.KeyboardSpeed;
            if ((num1 < 0) || (num1 > 0x1f))
            {
                num1 = 0x1f;
            }
            return ((((0x1f - num1) * 0x16f) / 0x1f) + 0x21);
        }
        private  static bool IsDelayValid(object value, RadObject obj)
        {
            return ((int)value >= 0);
        }
        private  static bool IsIntervalValid(object value, RadObject obj)
        {
            return ((int)value) > 0;
        }

        private void StartTimer()
        {
            if (this.timer == null)
            {
                this.timer = new Timer();
                this.timer.Tick += new EventHandler(this.OnTimeout);
            }
            else if (this.timer.Enabled)
            {
                return;
            }
            this.timer.Interval = this.Delay;
            this.timer.Start();
        }
        private void StopTimer()
        {
            if (this.timer != null)
            {
                this.timer.Stop();
            }
        }
        private bool HandleIsMouseOverChanged()
        {
            if (base.ClickMode != ClickMode.Hover)
            {
                return false;
            }
            if (base.IsMouseOver)
            {
                this.StartTimer();
            }
            else
            {
                this.StopTimer();
            }
            return true;
        }

        protected virtual void OnTimeout(object sender, EventArgs e)
        {
            if (this.timer.Interval != this.Interval)
            {
                this.timer.Interval = this.Interval;
            }
            if (base.IsPressed)
            {
                // HAAAAAAAAAAAACK!!!!!!!!!!!!!!!!!!!!!
                this.OnClick(e);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            this.HandleIsMouseOverChanged();

			if (base.IsPressed)
			{
				this.StartTimer();
			}
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            this.HandleIsMouseOverChanged();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && (base.ClickMode != ClickMode.Hover))
            {
                this.StartTimer();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left &&
                base.ClickMode != ClickMode.Hover)
            {
                this.StopTimer();            
            }
          
           
        }

        protected override void OnEnabledChanged(RadPropertyChangedEventArgs e)
        {
            base.OnEnabledChanged(e);
            base.SetValue(RadButtonItem.IsPressedProperty, false);
            this.StopTimer();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Telerik.WinControls.UI.RadRepeatButton button = this.ElementTree.Control as Telerik.WinControls.UI.RadRepeatButton;
            if (button != null)
	        {
                button.PerformButtonClick(); 
	        }
        }

        protected override void DisposeManagedResources()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
            }

            base.DisposeManagedResources();
        }

        #region Properties
        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, the Repeat button element waits while it is pressed before it starts repeating. The value must be non-negative. 
        /// </summary>
        [Bindable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadPropertyDefaultValue("Delay", typeof(RadRepeatButtonElement))]
        [Description("Gets or sets the amount of time, in milliseconds, the Repeat button element waits while it is pressed before it starts repeating. The value must be non-negative.")]
        public int Delay
        {
            get
            {
                return (int)base.GetValue(RadRepeatButtonElement.DelayProperty);
            }
            set
            {
                base.SetValue(RadRepeatButtonElement.DelayProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, between repeats once repeating starts. The value must be non-negative.
        /// </summary>
        [Bindable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadPropertyDefaultValue("Interval", typeof(RadRepeatButtonElement))]
        [Description("Gets or sets the amount of time, in milliseconds, between repeats once repeating starts. The value must be non-negative.")]
        public int Interval
        {
            get
            {
                return (int)base.GetValue(RadRepeatButtonElement.IntervalProperty);
            }
            set
            {
                base.SetValue(RadRepeatButtonElement.IntervalProperty, value);
            }
        } 
        #endregion
    }
}