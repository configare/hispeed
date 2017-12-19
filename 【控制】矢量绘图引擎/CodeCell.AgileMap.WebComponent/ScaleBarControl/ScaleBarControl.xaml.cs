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

namespace CodeCell.AgileMap.WebComponent
{
    public partial class ScaleBarControl : UserControl
    {
    
        public ScaleBarControl()
        {
            InitializeComponent();
        }

        public void SetBuddy(IMapControl mapcontrol)
        {
            mapcontrol.OnMapControlViewportChanged += new OnMapControlViewportChangedHandler(OnMapControlViewportChanged);
        }

        void OnMapControlViewportChanged(object sender, PrjRectangleF oldviewport, PrjRectangleF newviewport)
        {
            IMapControl mapcontrol = sender as IMapControl;
            txtDistance.Text = (ActualWidth * mapcontrol.Resolution / 1000).ToString("0.###") + "公里";
            txtScale.Text = "1:" + mapcontrol.Scale.ToString();
            txtResolution.Text = mapcontrol.Resolution.ToString("0.00") + "米";
        }
    }
}
