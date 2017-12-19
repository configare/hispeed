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
    public partial class MapBrowseGroup : UserControl
    {
        public static readonly DependencyProperty MapControlProperty = DependencyProperty.RegisterAttached("MapControl", typeof(MapControl), typeof(MapBrowseGroup), null);
        public MapBrowseGroup()
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
            btnFullView.DataContext = MapControl.FindSystemMapTool(enumMapTools.FullView);
            btnPan.DataContext = MapControl.FindSystemMapTool(enumMapTools.Pan);
            btnZoomIn.DataContext = MapControl.FindSystemMapTool(enumMapTools.ZoomIn);
            btnZoomOut.DataContext = MapControl.FindSystemMapTool(enumMapTools.ZoomOut);
            btnRefresh.DataContext = MapControl.FindSystemMapTool(enumMapTools.Refresh);
        }
    }
}
