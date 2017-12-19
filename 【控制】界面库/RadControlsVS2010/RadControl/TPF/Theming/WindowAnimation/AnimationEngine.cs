using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace Telerik.WinControls.WindowAnimation
{
    /// <summary>
    /// Event raised during animation notifying the new size for the panel
    /// </summary>
    /// <param name="sender">the object sending the notification</param>
    /// <param name="e">the new size for the window collasping/expanding </param>
    public delegate void AnimationEventHandler(object sender, AnimationEventArgs e);
	
	public delegate void NotifyAnimationCallback(AnimationEventArgs e);

	public class AnimationEngine : RadObject
	{
		/// <summary>
		/// Event raised when animation is finished
		/// </summary>
		[Browsable(true),
		Category(RadDesignCategory.ActionCategory)]
		public event AnimationEventHandler AnimationFinished
		{
			add
			{
				this.Events.AddHandler(AnimationEngine.AnimationFinishedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(AnimationEngine.AnimationFinishedEventKey, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnAnimationFinished(AnimationEventArgs e)
		{
			AnimationEventHandler handler1 =
				(AnimationEventHandler)base.Events[AnimationEngine.AnimationFinishedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}
		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnAnimating(AnimationEventArgs e)
		{
			AnimationEventHandler handler1 =
				(AnimationEventHandler)base.Events[AnimationEngine.AnimatingEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Event raised in parallel with the executing animation.
		/// </summary>
		[Browsable(true),
		Category(RadDesignCategory.ActionCategory)]
		public event AnimationEventHandler Animating
		{
			add
			{
				this.Events.AddHandler(AnimationEngine.AnimatingEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(AnimationEngine.AnimatingEventKey, value);
			}
		}

		#region Constructors
		static AnimationEngine()
		{
			AnimationEngine.AnimationFinishedEventKey = new object();
			AnimationEngine.AnimatingEventKey = new object();
		}

		#endregion

		// Fields
		protected StandardEasingCalculator calculator = new StandardEasingCalculator();
		protected int frames = 1;
		protected ManualResetEvent threadStart = new ManualResetEvent(false);
		//the worker thread
		protected Thread thread = null;
		private static readonly object AnimationFinishedEventKey;
		private static readonly object AnimatingEventKey;

		public RadEasingType EasingType
		{
			get 
			{
				if (this.calculator != null)
				{
					return this.calculator.EasingType;
				}
				return RadEasingType.Default; 
			}
			set 
			{
				if (this.calculator != null && this.calculator.EasingType != value)
				{
					this.calculator.EasingType = value;
				}
			}
		}

		public int AnimationFrames
		{
			get
			{
				return this.frames;
			}
			set
			{
				if (value == 0)
				{
					throw new InvalidOperationException("Frames can not be zero");
				}
				this.frames = value;
			}
		}

		public int AnimationStep
		{
			get
			{
				return 1000/this.frames;
			}
		}

		public void Start()
		{
			if (this.frames == 0)
			{
				throw new InvalidOperationException("Frames can not be zero");
			}
			//create the working thread
			threadStart.Reset();
			//if (thread == null)
			{
				thread = new Thread(new ThreadStart(Animate));
			}
			thread.IsBackground = true;
			//if (thread.IsAlive)
			//{
			//    thread.Resume();
			//}
			//else
			//{
				thread.Start();
			//}

			//waint until the thread has actually started
			threadStart.WaitOne();
		}
		
		public void Stop()
		{
			//create the working thread
			//threadStart.Reset();
            thread.Abort();
		}

		protected virtual void Animate()
		{
			//signal the calling thread that the worker started
			threadStart.Set();
		}

	}
}
