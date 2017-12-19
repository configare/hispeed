using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls
{
    public interface IPathCalculatorAnimationData
    {
        CarouselAnimationTypes AnimationType
        {
            get;
        }

         AnimatedPropertySetting CurrentAnimation
        {
            get;
        }

        //bool ValidFor(Type calculatorType);
    }
}
