using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.WindowAnimation
{
	public class AnimationEventArgs : EventArgs
	{
		public AnimationEventArgs(object animationValue) : this(animationValue, false)
		{
		}

		public AnimationEventArgs(object animationValue, bool finished)
		{
			this.animationValue = animationValue;
			this.finished = finished;
		}

		public object AnimationValue
		{
			get
			{
				return this.animationValue;
			}
		}

		public object AnimationFinished
		{
			get
			{
				return this.finished;
			}
		}

		// Fields
		private object animationValue;
		private bool finished = false;
	}
}
