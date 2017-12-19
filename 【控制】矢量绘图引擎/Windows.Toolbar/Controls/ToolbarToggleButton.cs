using System.Windows;
using System.Windows.Controls.Primitives;

namespace Windows.Toolbar.Controls
{
    public class ToolbarToggleButton : ToggleButton
    {        
        /// <summary>
        /// Defines the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ToolbarToggleButton), null);
        /// <summary>
        /// Gets or sets [Add custom comment here].
        /// </summary>
        /// <value>[Add custom comment here].</value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }


        public ToolbarToggleButton()
        {
            this.DefaultStyleKey = typeof(ToolbarToggleButton);
        }
    }
}
