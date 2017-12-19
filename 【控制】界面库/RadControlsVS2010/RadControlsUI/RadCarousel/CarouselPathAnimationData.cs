using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public class CarouselPathAnimationData
    {
        protected double? from = null;
        protected double? to = null;
        protected bool interrupt = false;
        protected bool specialHandling = false; // should be removed later!

        protected AnimatedPropertySetting currentAnimation;

        public CarouselPathAnimationData()
        {
        }

        internal bool SpecialHandling
        {
            get { return this.specialHandling; }
        }

        public double? From
        {
            get { return this.from; }

            set 
            {
                if (this.from != value)
                {
                    this.from = value;
                }
            }

        }

        public double? To
        {
            get { return this.to; }

            set
            {
                if (this.to != value)
                {
                    this.to = value;
                }
            }
        }

        public bool Interrupt
        {
            get { return this.interrupt; }

            set {
                if (this.interrupt != value)
                {
                    this.interrupt = value;
                }
            }
        }

        public AnimatedPropertySetting CurrentAnimation
        {
            get { return this.currentAnimation; }

            set 
            {
                if (this.currentAnimation != value)
                {
                    this.currentAnimation = value;
                }
            }
        }

        public virtual bool IsOutAnimation()
        {
            if (to == null) return false;

            return  double.IsNaN(to.Value) || double.IsInfinity(to.Value);
        }

        internal bool UpdateSpecialHandling()
        {
            this.specialHandling = this.From != null && this.To != null &&
                ((double.IsNegativeInfinity(this.From.Value) && double.IsPositiveInfinity(this.to.Value)) ||
                (double.IsPositiveInfinity(this.From.Value) && double.IsNegativeInfinity(this.to.Value)));

            return this.specialHandling;
        }

        internal void ResetSpecialHandling()
        {
            if (this.specialHandling)
            {
                this.from = null;
                this.to = null;
                this.specialHandling = false;
            }
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("from = {0}, end = {1}, interrupt = {2}, specialHandling = {3}", 
                this.from == null ? "null" : this.from.Value.ToString(),
                this.to == null ? "null" : this.to.Value.ToString(),
                this.interrupt, this.specialHandling
                );

            if (currentAnimation != null)
            {
                sb.AppendFormat("; animation: {0} -> {1}", 
                    currentAnimation.StartValue, 
                    currentAnimation.EndValue
                    );
            }

            return sb.ToString();
        }
#endif
    }
}
