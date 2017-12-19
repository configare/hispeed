using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents per-thread static instance of special RadControl, which may be used for explicit measure of RadElement instance.
    /// This functionality is required for example in the RadComboBox, when we need to calculate the size of the drop-down before it is displayed.
    /// </summary>
    public sealed class MeasurementControl : RadControl
    {
        #region Fields

        [ThreadStatic]
        private static MeasurementControl instance;

        #endregion

        #region Constructor

        private MeasurementControl()
        {
            this.elementTree.StyleManager.SuspendStyling();
            this.LoadElementTree();
        }

        #endregion

        #region Methods

        public override void RegisterHostedControl(RadHostItem hostElement)
        {
            //No need to add hosted controls for measurement
        }

        public override void UnregisterHostedControl(RadHostItem hostElement, bool removeControl)
        {
            //No hosted controls are added
        }

        /// <summary>
        /// Gets the element's desired size, using the specified available.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        public SizeF GetDesiredSize(RadElement element, SizeF availableSize)
        {
            //suspend theme updates for the element
            element.SuspendThemeRefresh();

            RadElement parent = element.Parent;
            int indexInChildrenCollection = -1;

            if (parent != null)
            {
                indexInChildrenCollection = parent.Children.IndexOf(element);
            }

            ElementVisibility oldVisibility = element.Visibility;

            if (oldVisibility != ElementVisibility.Visible)
            {
                element.Visibility = ElementVisibility.Visible;
            }
            this.RootElement.Children.Add(element);

            element.ResetLayout(true);
            element.Measure(availableSize);

            SizeF desiredSize = element.GetDesiredSize(false);

            //add the element to its previous parent (it will be automatically removed from its current parent)
            if (parent != null)
            {
                parent.Children.Insert(indexInChildrenCollection, element);
            }
            else
            {
                this.RootElement.Children.Remove(element);
            }

            if (oldVisibility != ElementVisibility.Visible)
            {
                element.Visibility = oldVisibility;
            }

            element.ResumeThemeRefresh();

            return desiredSize;
        }

        public Bitmap GetAsBitmapEx(RadElement element, SizeF availableSize, SizeF finalSize, Brush brush, float totalAngle, SizeF totalScale)
        {
            return this.GetAsBitmapEx(true, element, availableSize, finalSize, brush, totalAngle, totalScale);
        }

        public Bitmap GetAsBitmapEx(bool doArrangeAndMeasure, RadElement element, SizeF availableSize, SizeF finalSize, Brush brush, float totalAngle, SizeF totalScale)
        {
            RadElement parent = element.Parent;
            int indexInChildrenCollection = -1;

            if (parent != null)
            {
                indexInChildrenCollection = parent.Children.IndexOf(element);
            }

            this.RootElement.Children.Add(element);

            if (doArrangeAndMeasure)
            {
                element.ResetLayout(true);
                element.Measure(availableSize);
                element.Arrange(new RectangleF(new PointF(0, 0), finalSize));
            }

            Bitmap bitmap = element.GetAsBitmapEx(brush, totalAngle, totalScale);

            //add the element to its previous parent (it will be automatically removed from its current parent)
            if (parent != null)
            {
                parent.Children.Insert(indexInChildrenCollection, element);
            }
            else
            {
                this.RootElement.Children.Remove(element);
            }

            return bitmap;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance of the measurement tree (valid per UI thread)
        /// </summary>
        public static MeasurementControl ThreadInstance
        {
            get
            {
                //no need to enter critical section here since we have one instance per UI thread
                if (instance == null)
                {
                    instance = new MeasurementControl();
                }

                return instance;
            }
        }

        #endregion
    }
}
