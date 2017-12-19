using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CodeCell.AgileMap.WebComponent
{
    public enum enumMouseButtons
    {
        LeftButton,
        RightButton
    }

    public enum enumMouseActions
    {
        MouseDown,
        MouseUp
    }

    public enum enumMouseDirections
    {
        MouseEnter,
        MouseLeave
    }

    public delegate void OnFeatureCollectionChangedHandler(object sender);
    public delegate void OnFeatureClickedHandler(object sender,Feature feature,enumMouseButtons button,enumMouseActions action,MouseButtonEventArgs e);
    public delegate void OnMouseOverFeatureHandler(object sender,Feature feature,enumMouseDirections direction,MouseEventArgs e);

    public class FeatureLayer:Layer,IQueryFeatures
    {
        public OnFeatureCollectionChangedHandler OnFeatureCollectionChanged = null;
        public OnFeatureClickedHandler OnFeatureClicked = null;
        public OnMouseOverFeatureHandler OnMouseOverFeature = null;
   
        public FeatureLayer()
            : base()
        { 
        }

        public FeatureLayer(string id)
            : base(id)
        { 
        }

        public virtual Feature[] Query(QueryFilter filter)
        {
            return null;
        }
    }
}
