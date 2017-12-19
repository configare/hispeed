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
using CodeCell.AgileMap.WebComponent.AgileMapServiceProxy;

namespace CodeCell.AgileMap.WebComponent
{
    public partial class CurrentCoordDisplayer : UserControl, ICurrentCoordDisplayer
    {
        enum enumCoordDisplayType
        {
            DecimalDegree,
            DegreeMinuteSecond
        }

        private string cstPrjCoordFormat = "###,###.##";
        private const string cstDegreeChar = "°";
        private const string cstMinuteChar = "′";
        private const string cstSecondChar = "″";
        private IMapServerAgent _mapServerAgent = null;
        private enumCoordDisplayType _coordDisplayType = enumCoordDisplayType.DecimalDegree;

        public CurrentCoordDisplayer()
        {
            InitializeComponent();
        }

        public void SetBuddy(IMapControl mapControl)
        {
            _mapServerAgent = mapControl.MapServerAgent;
            _mapServerAgent.MapServiceClient.Prj2GeoCompleted += new EventHandler<Prj2GeoCompletedEventArgs>(_mapsrv_Prj2GeoCompleted);
            mapControl.CurrentCoordDisplayer = this;
        }

        public void DisplayCoord(Point prjPt)
        {
            currentPositionTips.Text = "投影坐标:" + prjPt.X.ToString(cstPrjCoordFormat) + "m , " +
                                                         prjPt.Y.ToString(cstPrjCoordFormat) + "m";
            ObservableCollection<PointF> requestpts = new ObservableCollection<PointF>();
            AgileMapServiceProxy.PointF reqpt = new PointF();
            reqpt.x = (float)prjPt.X;
            reqpt.y = (float)prjPt.Y;
            requestpts.Add(reqpt);
            _mapServerAgent.MapServiceClient.Prj2GeoAsync(requestpts, "MouseTip");
        }

        void _mapsrv_Prj2GeoCompleted(object sender, Prj2GeoCompletedEventArgs e)
        {
            ObservableCollection<AgileMapServiceProxy.PointF> pts = e.Result;
            if (pts == null || pts.Count == 0)
                return;
            if (e.UserState != null && e.UserState.ToString() == "MouseTip")
            {
                AgileMapServiceProxy.PointF pt = pts[0];
                switch (_coordDisplayType)
                {
                    case enumCoordDisplayType.DecimalDegree:
                        currentGeocoordTips.Text = "地理坐标:" + pt.x.ToString("####.######") + cstDegreeChar + " , " +
                                                                 pt.y.ToString("####.######") + cstDegreeChar;
                        break;
                    case enumCoordDisplayType.DegreeMinuteSecond:
                        currentGeocoordTips.Text = "地理坐标:" + GeoHelper.DegreeToString(pt.x) + " , " + GeoHelper.DegreeToString(pt.y);
                        break;
                }
            }
        }

        private void btnSetGeoDiaplyFormat_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu c = new ContextMenu();
            MenuItem degMinSceDegree = new MenuItem();
            degMinSceDegree.Header = "###°##′##″";
            degMinSceDegree.Click += new RoutedEventHandler(degMinSceDegree_Click);
            c.Items.Add(degMinSceDegree);
            MenuItem decimalDegree = new MenuItem();
            decimalDegree.Header = "###.######°";
            decimalDegree.Click += new RoutedEventHandler(decimalDegree_Click);
            c.Items.Add(decimalDegree);
            c.HorizontalOffset = e.GetPosition(null).X;
            c.VerticalOffset = e.GetPosition(null).Y + 8;
            c.IsOpen = true;
        }

        void decimalDegree_Click(object sender, RoutedEventArgs e)
        {
            _coordDisplayType = enumCoordDisplayType.DecimalDegree;
        }

        void degMinSceDegree_Click(object sender, RoutedEventArgs e)
        {
            _coordDisplayType = enumCoordDisplayType.DegreeMinuteSecond;
        }
    }
}
