using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public abstract class CarouselParameterPath : ICarouselPath
    {
        protected bool isModified = false;
        protected double zScale = 500.0;
        protected bool zIndexFromPath = false;
        protected double zIndexScale = 1000;
        protected ValueMapper ranges = new ValueMapper();
        protected bool enableRelativePath = true;                

        public CarouselParameterPath()
        {
            ranges.Add(new ValueMapper.Value(double.NegativeInfinity), new ValueMapper.Range(true, 0.0, 1.0, false, 0.0));
            ranges.Add(new ValueMapper.Range(true, 0.0, 1.0, true), new ValueMapper.Range(true, 1.0, 6.0, true));
            ranges.Add(new ValueMapper.Value(double.PositiveInfinity), new ValueMapper.Range(false, 6.0, 7.0, true, 7.0));
            ranges.Add(new ValueMapper.Value(double.NaN), new ValueMapper.Range(false, 7.0, 8.0, true));
        }

        public double ZScale
        {
            get { return this.zScale; }
            set 
            {
                if (value != this.zScale)
                {
                    this.zScale = value;
                    this.OnPropertyChanged("ZScale");
                }
            }
        }        


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }

        #endregion

        #region IPathCalculator Members

        public abstract object InitialPathPoint
        {
            get;
        }

        public abstract object FinalPathPoint
        {
            get;
        }
        
        public virtual bool IsPathClosed
        {
            get { return object.Equals(this.InitialPathPoint, this.FinalPathPoint); }
        }

        public object Evaluate(VisualElement element, CarouselPathAnimationData data, object value)
        {
            double newValue = (double)value;
            int range = data.SpecialHandling ? 1 : this.ranges.GetIndexFromTarget(newValue);

            if (range == -1) return null; //throw new Exception("Invalid range!");

            if (range != 1) return null;

            double actualValue = this.ranges[range].target.MapToUnit(newValue);

            return EvaluateByParameter(element, data, actualValue);
        }

        public abstract object EvaluateByParameter(VisualElement element, CarouselPathAnimationData data, double value);

        public void CreateAnimation(VisualElement element, CarouselPathAnimationData data, int frames, int delay)
        {
            double fromValue = 0;
            double toValue = 0;

            if (data.SpecialHandling )
            {
                double value = (double)element.GetValue(CarouselItemsContainer.CarouselLocationProperty);

                fromValue = double.IsNaN(value) ? 0 : value;

                ValueMapper.MapperRange range = this.ranges.GetTarget(data.To.Value);
                if (data.From == null || double.IsNegativeInfinity(data.From.Value))
                    fromValue -= range.Length;
                else
                    fromValue += range.Length;

                data.From = null;
            }
            else
            {
                if (data.From == null)
                {
                    double value = (double)element.GetValue(CarouselItemsContainer.CarouselLocationProperty);
                    //ValueMapper.MapperRange range = this.ranges.GetTarget(data.To.Value);
                    fromValue = double.IsNaN(value) ? 0 : value;
                }
                else
                {
                    fromValue = this.ranges.MapInTarget(data.From.Value);

                    data.From = null;
                }
            }


            if (data.To == null)
            {
                data.To = double.PositiveInfinity;
                //throw new Exception("Wrong target value for animation!");
            }
            //else
            {
                toValue = this.ranges.MapInTarget(data.To.Value);
            }

            if (fromValue != toValue)
            {
                PropertySetting pos = new PropertySetting(CarouselItemsContainer.CarouselLocationProperty,
                    fromValue
                    );

                pos.ApplyValue(element);
            }
            
            AnimatedPropertySetting animation =
                new AnimatedPropertySetting(CarouselItemsContainer.CarouselLocationProperty,
                    fromValue, toValue,
                    frames, delay
                    );

            data.CurrentAnimation = animation;
        }

        public void ApplyToElement(VisualElement element, CarouselPathAnimationData data, double value, object evaluated, Animations flags, Size controlSize)
        {
            int rangeIndex;           
            double unitValue;

            
            if (data.SpecialHandling)
            {
                rangeIndex = 1;
                unitValue = 0;
            }
            else
            {
                rangeIndex = this.ranges.GetIndexFromTarget(value);
                unitValue = this.ranges.MapTargetToUnit(value);
            }

            switch (rangeIndex)
            {
                case 0:
                    ApplyValueIn(element, unitValue, flags, controlSize);
                    break;

                case 1:
                    ApplyValuePath(element, (Point3D)evaluated, flags, controlSize);
                    break;

                case 2:
                case 3:
                    ApplyValueOut(element, unitValue, flags, controlSize);
                    break;

            }
        }

        public double PositionsCount(int itemsCount)
        {
            return Math.Max(0d, this.IsClosedPath? itemsCount + 1d : itemsCount );
        }

        public double Step(int itemsCount)
        {
            double piecesCount = Math.Max(1d, this.PositionsCount(itemsCount));

            return 1.0 / piecesCount;
        }

        #endregion

        private SizeF ElementScale(Point3D value, double scale)
        {
            double cs = (float)((1 + (value.Z / zScale) ) * scale);

            float finalScale = (float)Math.Max(0.125, Math.Min(2, cs));
            return new SizeF(finalScale, finalScale);
        }

        private void ApplyValueIn(RadElement element, double value, Animations flags, Size controlSize)
        {
            float scale = (float)(0.5 + ( value * 0.5 ) );

            Point3D initialPathPoint3D = this.ConvertPointIfRelative(
                (Point3D)this.InitialPathPoint, controlSize);

            SetElementBounds(element, (Point)initialPathPoint3D, flags);

            SetElementOpacity(element, value, flags);

            SetElementScale(element, ElementScale(initialPathPoint3D, scale), flags);

            SetElementZIndex(element, initialPathPoint3D, flags);
        }

        private void ApplyValueOut(RadElement element, double value, Animations flags, Size controlSize)
        {
            float scale = 1f - (float)(value * 0.5);

            Point3D finalPathPoint3D = this.ConvertPointIfRelative(
                (Point3D)this.FinalPathPoint, controlSize);

            SetElementScale(element, ElementScale(finalPathPoint3D, scale), flags);

            SetElementBounds(element, (Point)finalPathPoint3D, flags);

            SetElementOpacity(element, 1 - value, flags);

            SetElementZIndex(element, finalPathPoint3D, flags);
        }

        private void ApplyValuePath(VisualElement element, Point3D newPoint, Animations flags, Size controlSize)
        {
            if (newPoint == null)
                return;

            newPoint = this.ConvertPointIfRelative(newPoint, controlSize);
            SetElementBounds(element, (Point)newPoint, flags);

            double opacityValue = 1.0;
            CarouselContentItem item = (CarouselContentItem) element;

            switch(item.Owner.ItemsContainer.OpacityChangeCondition)
            {
                case OpacityChangeConditions.None:
                    opacityValue = 1.0;
                    break;
                case OpacityChangeConditions.ZIndex:
                    if (((Point3D)this.FinalPathPoint).Z != 0.0)
                    {
                        opacityValue = 1 - (newPoint.Z/((Point3D) this.FinalPathPoint).Z);
                    }
                    break;
                case OpacityChangeConditions.SelectedIndex:
                    int i = item.Owner.ItemsContainer.Items.IndexOf((RadItem)item.HostedItem);
                    opacityValue = 1 - Math.Abs((double)(i - item.Owner.SelectedIndex) / item.Owner.ItemsContainer.VisibleItemCount);
                    break;
            }

            if (opacityValue < item.Owner.ItemsContainer.MinFadeOpacity)
            {
                opacityValue = item.Owner.ItemsContainer.MinFadeOpacity;
            }
            else if( opacityValue > 1.0)
            {
                opacityValue = 1.0;
            }

            SetElementOpacity(element, opacityValue , flags);
            SetElementScale(element, ElementScale(newPoint, 1.0), flags);
            SetElementZIndex(element, newPoint, flags);
        }

        private static void SetElementBounds(RadElement element, Point value, Animations flags)
        {
            if ((flags & Animations.Location) == 0 || element == null)
                return;

            Point position = value;
            Point itemCenter = ((CarouselContentItem)element).Center;

            position.Offset(-itemCenter.X, -itemCenter.Y);

            element.Bounds = new Rectangle(position, element.Bounds.Size);

            PropertySetting setting = new PropertySetting(
                VisualElement.BoundsProperty, 
                new Rectangle(value, element.Bounds.Size)
                );

            setting.ApplyValue(element);
        }

        private static void SetElementOpacity(RadElement element, double value, Animations flags)
        {
            if ((flags & Animations.Opacity) == 0 || element == null)
                return;

            PropertySetting setting = new PropertySetting(
                VisualElement.OpacityProperty,
                value);

            setting.ApplyValue(element);
        }

        private static void SetElementScale(RadElement element, SizeF value, Animations flags)
        {
            if ((flags & Animations.Scale) == 0) return;

            if (element == null)
                return;

            RadElement visual = ((CarouselContentItem)element).HostedItem;

            PropertySetting setting = new PropertySetting(
                VisualElement.ScaleTransformProperty,
                value
            );

            setting.ApplyValue(visual);
        }

        private void SetElementZIndex(RadElement element, Point3D pos, Animations flags)
        {
            if (!this.zIndexFromPath)
                return;

            int zIndex = (int)(pos.Z * zIndexScale);

            PropertySetting setting = new PropertySetting(
                VisualElement.ZIndexProperty,
                zIndex
            );

            setting.ApplyValue(element);
        }

        public void InitializeItem(VisualElement element, object flags)
        {
            Animations aflags = (Animations) flags;
            SetElementBounds(element, (Point)(Point3D)this.InitialPathPoint, Animations.Location);
            SetElementOpacity(element, 0, aflags);
            SetElementScale(element, ElementScale((Point3D)this.InitialPathPoint, 0.5), aflags);
            SetElementZIndex(element, (Point3D)this.InitialPathPoint, aflags);
        }

        public bool ZindexFromPath()
        {
            return this.zIndexFromPath;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool EnableRelativePath
        {
            get { return enableRelativePath; }
            set
            {
                if (enableRelativePath != value)
                {
                    enableRelativePath = value;
                }
            }
        }        

        protected virtual bool IsClosedPath
        {
            get { return true; }
        }

        public Point3D ConvertFromRelativePath(Point3D point, Size ownerSize)
        {
            return new Point3D(point.X*ownerSize.Width/100,
                               point.Y*ownerSize.Height/100,
                               point.Z);
        }

        public Point3D ConvertToRelativePath(Point3D point, Size ownerSize)
        {
            return new Point3D(point.X*100/ownerSize.Width,
                               point.Y*100/ownerSize.Height,
                               point.Z);
        }

        private Point3D ConvertPointIfRelative(Point3D point, Size ownerSize)
        {
            if (this.EnableRelativePath)
            {
                return new Point3D(point.X * ownerSize.Width / 100,
                                   point.Y * ownerSize.Height / 100,
                                   point.Z);
            }

            return point;
        }
    }
}
