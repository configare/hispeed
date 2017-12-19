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
using System.Collections.ObjectModel;

namespace CodeCell.AgileMap.WebComponent
{
    public interface IQueryResultContainer
    {
        void Clear();
        void Add(Feature feature);
        ObservableCollection<Feature> QueryResult { get; }
        void SetMapControl(IMapControl mapcontrol);
    }
}
