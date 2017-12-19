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
    public partial class StandardToolbar : UserControl,IToolBar
    {
        public static readonly DependencyProperty MapControlProperty = DependencyProperty.RegisterAttached("MapControl", typeof(MapControl), typeof(StandardToolbar), null);
        private ToolbarButton _previousToolbarButton = null;

        public StandardToolbar()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(StandardToolbar_Loaded);
        }

        void StandardToolbar_Loaded(object sender, RoutedEventArgs e)
        {
            AttachClickEventToToolbarButtons();
        }

        public void SetBuddy(IMapControl mapControl)
        {
            MapControl = mapControl as MapControl;
        }

        public MapControl MapControl
        {
            get { return GetValue(MapControlProperty) as MapControl; }
            set 
            {
                SetValue(MapControlProperty, value);
                mapBrowseGroup.MapControl = value;
                featureSelectGroup.MapControl = value;
                featureQueryGroup.MapControl = value;
                measureToolGroup.MapControl = value;
            }
        }

        public IMapTool CurrentMapTool
        {
            get { return MapControl.CurrentMapTool; }
            set { MapControl.CurrentMapTool = value; }
        }

        private void ToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            IMapCommand cmd = (sender as ToolbarButton).DataContext as IMapCommand ;
            if (cmd != null)
            {
                cmd.Click();
                if (cmd is IMapTool)
                {
                    ResetPreviousToolButton();
                    _previousToolbarButton = sender as ToolbarButton;
                    _previousToolbarButton.IsActived = true;
                }
            }
        }

        private void ResetPreviousToolButton()
        {
            if(_previousToolbarButton != null)
                _previousToolbarButton.IsActived = false;
        }

        private void AttachClickEventToToolbarButtons()
        {
            List<ToolbarButton> allToolbarButtons = new List<ToolbarButton>();
            FindAllToolbarButtons(this, allToolbarButtons);
            foreach (ToolbarButton btn in allToolbarButtons)
                btn.Click += new RoutedEventHandler(ToolbarButton_Click);
        }

        private void FindAllToolbarButtons(UIElement parentElement, List<ToolbarButton> btns)
        {
            if (parentElement is ToolbarButton)
                btns.Add(parentElement as ToolbarButton);
            int childCount =  VisualTreeHelper.GetChildrenCount(parentElement);
            for (int i = 0; i < childCount; i++)
            {
                UIElement child = VisualTreeHelper.GetChild(parentElement, i) as UIElement;
                if (child != null)
                    FindAllToolbarButtons(child, btns);
            }
        }
    }
}
