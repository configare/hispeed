using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class FontAnimationStep
    {
        private float sizeStep;

        public FontAnimationStep(float sizeStep)
        {
            this.sizeStep = sizeStep;
        }

        public float SizeStep
        {
            get { return sizeStep; }
            set { sizeStep = value; }
        }
    }
}
