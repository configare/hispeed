using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Windows.Toolbar.Controls;

namespace CodeCell.AgileMap.WebComponent
{
    public partial class FeatureSelectGroup : UserControl
    {
        public static readonly DependencyProperty MapControlProperty = DependencyProperty.RegisterAttached("MapControl", typeof(MapControl), typeof(FeatureSelectGroup), null);

        public FeatureSelectGroup()
        {
            InitializeComponent();
        }

        public MapControl MapControl
        {
            get { return GetValue(MapControlProperty) as MapControl; }
            set
            {
                SetValue(MapControlProperty, value);
                InitToolButtons();
            }
        }

        private void InitToolButtons()
        {
            btnSelectByCirlce.DataContext = MapControl.FindSystemMapTool(enumMapTools.SelectToolCirle);
            btnSelectByRectangle.DataContext = MapControl.FindSystemMapTool(enumMapTools.SelectToolRect);
            btnSelectByPolygon.DataContext = MapControl.FindSystemMapTool(enumMapTools.SelectToolPolygon);
            btnSelectByIdentify.DataContext = MapControl.FindSystemMapTool(enumMapTools.Identify);
        }
    }
}
