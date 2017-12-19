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
using System.Windows.Data;
using System.Globalization;

namespace Windows.Toolbar.Controls.Specialized
{
    /// <summary>
    /// Used for line style (hatch / dash array)
    /// </summary>
    public class CollapsedWhenNoStyleConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;

            try
            {
                DoubleCollection dashArray = 
                    (DoubleCollection)System.Convert.ChangeType(
                        value, typeof(DoubleCollection), culture);

                visibility = (dashArray != null && dashArray.Count == 1 && dashArray[0] == 0) 
                    ? Visibility.Collapsed 
                    : Visibility.Visible;
            }
            catch
            {
            }

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
