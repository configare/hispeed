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
using System.Collections.ObjectModel;

namespace CodeCell.AgileMap.WebComponent
{
    public partial class QueryResultContainerControl : UserControl,IQueryResultContainer
    {
        public static readonly DependencyProperty QueryResultProperty = DependencyProperty.RegisterAttached("QueryResult", typeof(ObservableCollection<Feature>), typeof(QueryResultContainerControl), null);
        private IMapControl _mapcontrol = null;

        public QueryResultContainerControl()
        {
            InitializeComponent();
            QueryResult = new ObservableCollection<Feature>();
        }

        public void SetMapControl(IMapControl mapcontrol)
        {
            _mapcontrol = mapcontrol;
        }

        public ObservableCollection<Feature> QueryResult
        {
            get { return GetValue(QueryResultProperty) as ObservableCollection<Feature>; }
            set { SetValue(QueryResultProperty, value); }
        }

        public void Clear()
        {
            QueryResult.Clear();
        }

        public void Add(Feature feature)
        {
            QueryResult.Add(feature);
        }

        private void lstQueryResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Feature fet = lstQueryResult.SelectedItem as Feature;
            if (fet == null)
                return;
            AgileMapServiceProxy.Envelope evp = fet.Envelope;
            if (Math.Abs(evp._minX - evp._maxX) < double.Epsilon)
                GotoPoint(evp._minX, evp._minY);
            else
                GotoEnvelope(evp._minX, evp._minY, evp._maxX, evp._maxY);
        }

        private void GotoEnvelope(double minx, double miny, double maxx, double maxy)
        {
            _mapcontrol.PanTo(new Point((minx + maxx) / 2, (miny + maxy) / 2),
                              Math.Max(Math.Abs(maxx - minx),Math.Abs(maxy - miny)) * 100000
                              );
        }

        private void GotoPoint(double x, double y)
        {
            _mapcontrol.Goto(new Point(x, y));
        }
    }
}
