using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public interface ICarouselPath : INotifyPropertyChanged
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        object InitialPathPoint
        {
            get;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        object FinalPathPoint
        {
            get;
        }

        double ZScale
        {
            get;
            set;
        }
        
        //CarouselAnimationTypes AnimationType(IPathCalculatorAnimationData data, double value);

        void ApplyToElement(VisualElement element, CarouselPathAnimationData data, double value, object evaluated, Animations flags, Size controlSize);

        void CreateAnimation(VisualElement element, CarouselPathAnimationData data, int frames, int delay);

        object Evaluate(VisualElement element, CarouselPathAnimationData data, object value);

        void InitializeItem(VisualElement element, object flags);

        //double CurrentPosition(VisualElement element);

        double PositionsCount(int itemsCount);

        double Step(int itemsCount);

        bool ZindexFromPath();
        
        bool EnableRelativePath
        {
            get; set;
        }

        Point3D ConvertFromRelativePath(Point3D point, Size ownerSize);
        Point3D ConvertToRelativePath(Point3D point, Size ownerSize);
    }
}
