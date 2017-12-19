using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Contains information about the way Animation has finished
    /// </summary>
    public class AnimationStatusEventArgs : EventArgs
    {
        #region Constructors

        public AnimationStatusEventArgs(RadElement element)
        {
            this.element = element;
            this.isAnimationInterrupt = true;
        }

        public AnimationStatusEventArgs(RadElement element, bool isAnimationInterrupt) 
            : this(element)
        {
            this.isAnimationInterrupt = isAnimationInterrupt;
        }

        public AnimationStatusEventArgs(RadElement element, bool isAnimationInterrupt, bool registerValueAsLocal)
            : this(element, isAnimationInterrupt)
        {
            this.registerValueAsLocal = registerValueAsLocal;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets value indicating whether the animation has been interrupted by another one.
        /// </summary>
        public bool IsInterrupt
        {
            get
            {
                return this.isAnimationInterrupt;
            }
        }

        /// <summary>
        /// Gets value indicating whether the animation has been interrupted by another one.
        /// </summary>
        public bool RegisterValueAsLocal
        {
            get
            {
                return this.registerValueAsLocal;
            }
        }

        /// <summary>
        /// Gets the element associated with the specified animation.
        /// </summary>
        public RadElement Element
        {
            get
            {
                return element;
            }
        }

        #endregion

        #region Fields

        private RadElement element;
        private bool isAnimationInterrupt;
        private bool registerValueAsLocal;

        #endregion
    }

    /// <summary>
    /// AnimationStartedEventHandler delegate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AnimationStartedEventHandler(object sender, AnimationStatusEventArgs e);

    /// <summary>
    /// AnimationFinishedEventHandler delegate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AnimationFinishedEventHandler(object sender, AnimationStatusEventArgs e);
}
