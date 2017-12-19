using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    internal class GradientAngleBehavior : PropertyChangeBehavior
    {
        private RadElement toRotate;
        private RadScrollBarElement scrollBarElement;
        private float? initialAngle;
        private bool rotateCounterDirection;

        public GradientAngleBehavior(RadElement toRotate)
            : base(RadScrollBarElement.ScrollTypeProperty)
        {
            this.toRotate = toRotate;
        }

        public GradientAngleBehavior(RadElement toRotate, bool rotateCounterDirection)
            : base(RadScrollBarElement.ScrollTypeProperty)
        {
            this.toRotate = toRotate;
            this.rotateCounterDirection = rotateCounterDirection;
        }

        private RadScrollBarElement ScrollBarElement
        {
            get
            {
                if (this.toRotate.ElementState != ElementState.Loaded)
                {
                    return null;
                }

                if (scrollBarElement == null)
                {
                    for (RadElement element = this.toRotate;
                        element != null;
                        element = element.Parent)
                    {
                        if (element is RadScrollBarElement)
                        {
                            this.scrollBarElement = (RadScrollBarElement)element;
                            break;
                        }
                    }
                }

                return scrollBarElement;
            }
        }

        private bool ShouldRotateAccordingToScrollType(ScrollType scrollType)
        {
            if (scrollType == ScrollType.Horizontal &&
                ScrollBarElement.GradientAngleCorrection == 90 ||
                scrollType == ScrollType.Vertical &&
                ScrollBarElement.GradientAngleCorrection == -90)
            {
                return true;
            }

            return false;
        }

        public override void OnPropertyChange(RadElement element, RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChange(element, e);

            if (e.Property == RadScrollBarElement.ScrollTypeProperty)
            {
                this.RecalculateOnGradientAngleCorrectionChanged();
            }
        }

        public void RecalculateOnGradientAngleCorrectionChanged()
        {
            RadScrollBarElement scrollBar = this.ScrollBarElement;
            if (scrollBar == null)
            {
                return;
            }

            if (toRotate.UseNewLayoutSystem)
            {
                if (initialAngle == null)
                {
                    initialAngle = toRotate.AngleTransform;
                }
                if (this.ShouldRotateAccordingToScrollType(scrollBar.ScrollType))
                {
                    int sign = rotateCounterDirection ? -1 : 1;
                    toRotate.AngleTransform = initialAngle.Value + (sign * scrollBar.GradientAngleCorrection);
                }
            }
            else
            {
                ScrollType scrollType = scrollBar.ScrollType;
                if (scrollType != RadScrollBarElement.DefaultScrollType)
                {
                    if (toRotate is FillPrimitive)
                        ((FillPrimitive)toRotate).GradientAngle += scrollBar.GradientAngleCorrection;
                    if (toRotate is BorderPrimitive)
                        ((BorderPrimitive)toRotate).GradientAngle += scrollBar.GradientAngleCorrection;
                }
                else
                {
                    if (toRotate is FillPrimitive)
                        ((FillPrimitive)toRotate).GradientAngle -= scrollBar.GradientAngleCorrection;
                    if (toRotate is BorderPrimitive)
                        ((BorderPrimitive)toRotate).GradientAngle -= scrollBar.GradientAngleCorrection;
                }
            }
        }
    }

}
