using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public class CarouselItemsLocationBehavior : PropertyChangeBehavior
    {
        private CarouselItemsContainer owner;

        private ICarouselPath Calculator
        {
            get { return this.owner.CarouselPath; }
        }

        public CarouselItemsLocationBehavior(CarouselItemsContainer owner)
            : base(CarouselItemsContainer.CarouselLocationProperty)
        {
            this.owner = owner;
        }

        public override void OnPropertyChange(RadElement element, RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChange(element, e);

            if (e.Property != CarouselItemsContainer.CarouselLocationProperty) 
                return;

            if (double.IsNaN((double)e.NewValue))
                return;

            VisualElement visualElement = (VisualElement)element;

            CarouselPathAnimationData data = (CarouselPathAnimationData)element.GetValue(CarouselItemsContainer.CarouselAnimationData);

            object newValue = this.Calculator.Evaluate(visualElement, data, (double)e.NewValue);
            
            this.Calculator.ApplyToElement(visualElement, data, (double)e.NewValue, newValue, this.owner.AnimationsApplied, this.owner.Size);
        }        
    }
}
