using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;

namespace Telerik.WinControls.WindowAnimation
{
	public class WindowAnimationEngine : AnimationEngine
	{
		public WindowAnimationEngine()
		{
		}

		// Fields
		protected Rectangle minimum = Rectangle.Empty;
		protected Rectangle maximum = Rectangle.Empty;
		protected Rectangle currentFrameValue = Rectangle.Empty;
		protected bool minmax = true;
		private bool cancel = false;

		/// <summary>
		/// Get/Set minimum value allowed for size
		/// </summary>
		public Rectangle Minimum
		{
			get
			{
				return minimum;
			}

			set
			{
                minimum = value;
			}
		}

		/// <summary>
		/// Get/Set maximum value allowed for size
		/// </summary>
		public Rectangle Maximum
		{
			get
			{
				return maximum;
			}
			set
			{
                maximum = value;
			}
		}

		public bool AnimateMinimumToMaximum
		{
			get
			{
				return minmax;
			}
			set
			{
                minmax = value;
			}
		}

		public void Cancel()
		{
            cancel = true;
		}

		protected override void Animate()
		{
			base.Animate();

            if (this.IsDisposing || this.IsDisposed)
            {
                return;
            }

			Rectangle start = this.minimum;
			Rectangle end = this.maximum;
			if (!this.AnimateMinimumToMaximum)
			{
				end = this.minimum;
				start = this.maximum;
			}
			AnimationValueRectangleCalculator threadCalculator = new AnimationValueRectangleCalculator();
			for (int i = 0; i < this.AnimationFrames; i++)
			{
                //keep in mind that at any execution line the calling thread may have disposed this instance, so always check for the state of the object
                if (this.IsDisposing || this.IsDisposed)
                {
                    return;
                }

                if (cancel)
				{
                    cancel = false;
					break;
				}
				this.currentFrameValue =
					(Rectangle) threadCalculator.CalculateAnimatedValue(start, end, null, null, i, this.frames, this.calculator);

                //keep in mind that at any execution line the calling thread may have disposed this instance, so always check for the state of the object
                if (this.IsDisposing || this.IsDisposed)
                {
                    return;
                }

				this.OnAnimating(new AnimationEventArgs(this.currentFrameValue, false));
				Thread.Sleep(20);
			}

            //keep in mind that at any execution line the calling thread may have disposed this instance, so always check for the state of the object
            if (this.IsDisposing || this.IsDisposed)
            {
                return;
            }

			this.OnAnimationFinished(new AnimationEventArgs(this.Maximum, true));
		}
	}
}
