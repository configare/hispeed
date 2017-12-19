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

namespace Windows.Toolbar.Controls
{
    public class ToolbarButton : Button
    {
        /// <summary>
        /// Defines the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ToolbarButton), null);
        public static readonly DependencyProperty IsActivedProperty =
            DependencyProperty.Register("IsActived", typeof(bool), typeof(ToolbarButton), null);
        /// <summary>
        /// Gets or sets [Add custom comment here].
        /// </summary>
        /// <value>[Add custom comment here].</value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public ToolbarButton()
        {
            this.DefaultStyleKey = typeof(ToolbarButton);
            this.LostFocus += new RoutedEventHandler(ToolbarButton_LostFocus);
            this.MouseLeave += new MouseEventHandler(ToolbarButton_MouseLeave);
        }

        void ToolbarButton_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsActived = this.IsActived;
        }

        void ToolbarButton_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsActived = this.IsActived;
        }

        public bool IsActived
        {
            get { return (bool)GetValue(IsActivedProperty); }
            set
            {
                SetValue(IsActivedProperty, value);
                if (value)
                    VisualStateManager.GoToState(this, "Focused", true);
                else
                    VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Dispatcher.BeginInvoke(delegate()
            {

            }
            );
        }
    }
}
