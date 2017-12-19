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
    public class ServiceFeatureLayer:FeatureLayer
    {
        public ServiceFeatureLayer()
            : base()
        { 
        }

        public ServiceFeatureLayer(string id)
            : base(id)
        { 
        }

        public override Feature[] Query(QueryFilter filter)
        {
            return base.Query(filter);
        }
    }
}
