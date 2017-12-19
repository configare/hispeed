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
using System.Text.RegularExpressions;

namespace CodeCell.AgileMap.WebComponent
{
    public partial class FeatureQueryGroup : UserControl
    {
        public static readonly DependencyProperty MapControlProperty = DependencyProperty.RegisterAttached("MapControl", typeof(MapControl), typeof(FeatureQueryGroup), null);

        public FeatureQueryGroup()
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
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            if (MapControl.QueryResultContainer == null)
            {
                MessageBox.Show("没有设置查询结果容器，无法进行查询。", "系统消息", MessageBoxButton.OK);
                return;
            }
            string keywords = txtKeyWords.Text;
            if (keywords.Trim() == string.Empty)
                return;
            Point geoPt = new Point();
            Match m = Regex.Match(txtKeyWords.Text, @"r:(?<radius>\d+(\.\d+)?)");
            double radius = 0;
            if (m.Success)
            {
                radius = double.Parse(m.Groups["radius"].Value);
                keywords = Regex.Replace(keywords, @"r:(?<radius>\d+(\.\d+)?)", string.Empty);
            }
            if (GeoHelper.ExtractGeoCoord(keywords, out geoPt))
            {
                GeoTo(geoPt, radius);
            }
            else
            {
                MapControl.QueryResultContainer.Clear();
                MapControl.Map.Query(MapControl.QueryResultContainer, null, keywords);
            }
        }

        private void GeoTo(Point geoPt, double radius)
        {
            if (radius > 0)
                MapControl.PanTo(geoPt, radius);
            else
                MapControl.Goto(geoPt);
        }

        private void txtKeyWords_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnGo_Click(null, null);
        }
    }
}
