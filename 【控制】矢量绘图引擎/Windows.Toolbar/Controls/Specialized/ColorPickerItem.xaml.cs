using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Windows.Toolbar.Controls.Specialized
{
    public partial class ColorPickerItem : UserControl
    {
        #region Fields

        private BorderType _originalBorderType;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the user clicks the control.
        /// </summary>
        public event EventHandler Click;
        /// <summary>
        /// Raises the <see cref="E:Click"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnClick(EventArgs e)
        {
            EventHandler eventHandler = Click;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Defines the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorPickerItem), new PropertyMetadata(Colors.Transparent, new PropertyChangedCallback(OnColorChanged)));
        /// <summary>
        /// Gets or sets the color to display.
        /// </summary>
        /// <value>The color to display.</value>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        private static void OnColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as ColorPickerItem;
            if (owner != null)
            {
                owner.OnColorChanged(e);
            }
        }

        private void OnColorChanged(DependencyPropertyChangedEventArgs e)
        {
            Color newValue = (Color)e.NewValue;

            if (RootElement != null)
            {
                RootElement.Background = new SolidColorBrush(newValue);
            }
        }

        /// <summary>
        /// Defines the <see cref="BorderType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderTypeProperty =
            DependencyProperty.Register("BorderType", typeof(BorderType), typeof(ColorPickerItem), new PropertyMetadata(BorderType.Independent, new PropertyChangedCallback(OnBorderTypeChanged)));
        /// <summary>
        /// Gets or sets [Add custom comment here].
        /// </summary>
        /// <value>[Add custom comment here].</value>
        public BorderType BorderType
        {
            get { return (BorderType)GetValue(BorderTypeProperty); }
            set { SetValue(BorderTypeProperty, value); }
        }

        private static void OnBorderTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as ColorPickerItem;
            if (owner != null)
            {
                owner.OnBorderTypeChanged(e);
            }
        }

        private void OnBorderTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            BorderType newValue = (BorderType)e.NewValue;

            Thickness thickness = new Thickness(1);

            switch (newValue)
            {
                case BorderType.Independent:
                    thickness = new Thickness(1, 1, 1, 1);
                    break;
                case BorderType.Top:
                    thickness = new Thickness(1, 1, 1, 0);
                    break;
                case BorderType.Body:
                    thickness = new Thickness(1, 0, 1, 0);
                    break;
                case BorderType.Bottom:
                    thickness = new Thickness(1, 0, 1, 1);
                    break;
            }

            RootElement.BorderThickness = thickness;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPickerItem"/> class.
        /// </summary>
        public ColorPickerItem()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            RootElement.Style = Resources["Selected"] as Style;

            _originalBorderType = BorderType;

            BorderType = BorderType.Independent;
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            ResetColorBorderToNormal();
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // We need here to restore as well the border (user clicks, focus no lost... restore border)
            ResetColorBorderToNormal();

            OnClick(EventArgs.Empty);
        }

        private void ResetColorBorderToNormal()
        {
            RootElement.Style = Resources["Normal"] as Style;

            BorderType = _originalBorderType;
        }

        #endregion
    }
}
