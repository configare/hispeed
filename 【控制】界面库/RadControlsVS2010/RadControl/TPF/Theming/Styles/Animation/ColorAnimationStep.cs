using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Drawing;
using System.Collections;
using Telerik.WinControls;

namespace Telerik.WinControls
{
    [TypeConverter(typeof(ColorAnimationStepConverter))]
    public class ColorAnimationStep
    {

        public int A;
        public int R;
        public int G;
        public int B;

        public ColorAnimationStep(int A, int R, int G, int B)
        {
            this.A = A;
            this.G = G;
            this.R = R;
            this.B = B;
        }
    }    
}