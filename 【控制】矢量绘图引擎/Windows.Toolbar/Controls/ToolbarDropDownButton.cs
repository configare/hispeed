using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Linq;
using System;

namespace Windows.Toolbar.Controls
{
    [TemplatePart(Name = ButtonContainerPart, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = DropDownArrowToggleButtonPart, Type = typeof(ToggleButton))]
    [TemplateVisualState(Name = SelectingVisualState, GroupName = "Commonstates")]
    public class ToolbarDropDownButton : ToolbarButton
    {
        #region Constants

        public const string ButtonContainerPart = "ButtonContainer";
        public const string DropDownArrowToggleButtonPart = "DropDownArrowToggleButton";

        public const string SelectingVisualState = "Selecting";

        #endregion

        #region Fields

        // We need this in order to calculate the X,Y coordinates for the popup to show
        private FrameworkElement _buttonContainer;

        // We need to hook on left button down on this tack panel
        // we are using templated windowsToolbar, we cannot access directly to the stack panel 
        // and add the mouse left button down hook...
        // A bit more of work, hook the template applied event and from there 
        // get the stack panel and assign it to this content variable
        private ToggleButton _dropDownArrowToggleButton;

        private Canvas _popupRoot;
        private Canvas _popupClosingCanvas;
        private bool _skipStateChange;
        private Popup _popup;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        public event EventHandler<SelectedValueChangedEventArgs> SelectedValueChanged;
        /// <summary>
        /// Raises the <see cref="E:SelectedValueChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="SelectedValueChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedValueChanged(SelectedValueChangedEventArgs e)
        {
            EventHandler<SelectedValueChangedEventArgs> eventHandler = SelectedValueChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Defines the <see cref="SelectedValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(ToolbarDropDownButton), null);
        /// <summary>
        /// Gets or sets [Add custom comment here].
        /// </summary>
        /// <value>[Add custom comment here].</value>
        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="SelectionContainer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionContainerProperty =
            DependencyProperty.Register("SelectionContainer", typeof(SelectionContainer), typeof(ToolbarDropDownButton), new PropertyMetadata(null, new PropertyChangedCallback(OnPopupContentChanged)));
        /// <summary>
        /// Gets or sets [Add custom comment here].
        /// </summary>
        /// <value>[Add custom comment here].</value>
        public SelectionContainer SelectionContainer
        {
            get { return (SelectionContainer)GetValue(SelectionContainerProperty); }
            set { SetValue(SelectionContainerProperty, value); }
        }

        private static void OnPopupContentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as ToolbarDropDownButton;
            if (owner != null)
            {
                owner.OnPopupContentChanged(e);
            }
        }

        private void OnPopupContentChanged(DependencyPropertyChangedEventArgs e)
        {
            SelectionContainer oldItem = e.OldValue as SelectionContainer;
            if (oldItem != null)
            {
                oldItem.SelectedValueChanged -= new EventHandler<SelectedValueChangedEventArgs>(SelectionContainer_SelectedValueChanged);
            }

            SelectionContainer newItem = e.NewValue as SelectionContainer;
            if (newItem != null)
            {
                newItem.SelectedValueChanged += new EventHandler<SelectedValueChangedEventArgs>(SelectionContainer_SelectedValueChanged);
            }
        }

        private void SelectionContainer_SelectedValueChanged(object sender, SelectedValueChangedEventArgs e)
        {
            SelectedValue = e.Value;

            OnSelectedValueChanged(e);

            ClosePopup();
        }

        #endregion

        #region Constructors

        public ToolbarDropDownButton()
        {
            this.DefaultStyleKey = typeof(ToolbarDropDownButton);
        }

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            BindXamlElements();

            base.OnApplyTemplate();            
        }

        private void BindXamlElements()
        {
            _buttonContainer = GetTemplateChild(ButtonContainerPart) as FrameworkElement;

            _dropDownArrowToggleButton = GetTemplateChild(DropDownArrowToggleButtonPart) as ToggleButton;
            if (_dropDownArrowToggleButton != null)
            {
                _dropDownArrowToggleButton.Checked += new RoutedEventHandler(_dropDownArrowToggleButton_Checked);
                _dropDownArrowToggleButton.Unchecked += new RoutedEventHandler(_dropDownArrowToggleButton_Unchecked);
            }
        }

        private void _dropDownArrowToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void _dropDownArrowToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowPopup();
        }

        private void CreatePopup()
        {
            if (_popup == null)
            {
                _popup = new Popup();
                _popup.Opened += new System.EventHandler(Popup_Opened);
                _popup.Closed += new System.EventHandler(Popup_Closed);

                // 1. We need to create two canvas:
                //      --> One that fills the whole plugin size.
                //      --> Another that fills as well the whole plugin size, when the user clicks over it, it will close the popup            
                //
                //      What we are going to do is 
                //      --> Add to the popup content the first canvas (full size, no event).
                //      --> Add to the popup the second canvas (the one with the click and close event).
                //      --> Add to the popup the popup content (the dialog we want to show), and set the right
                //          coordinates for this popup (from the dropdown button get the right X,Y offset coordinates).
                //
                //
                // 2. By following this approach we will get the expected behaviour (ZOrder and events):
                //
                //      --> If an user clicks on the body content (the dialog we want to show) the second transparent canvas
                //          won't fire a mouse left button down event.
                //
                //      --> If an user clicks on the outer canvas (the second one), the left button down mouse event will be
                //          fired and the dialog will get closed (this look likea lostfocus functionallity... we have to use
                //          this trick because LostFocus does not work well for this scenario).

                _popupRoot = new Canvas();
                _popupRoot.Background = new SolidColorBrush(Colors.Transparent);

                _popupClosingCanvas = new Canvas();
                _popupClosingCanvas.Background = new SolidColorBrush(Colors.Transparent);
                _popupClosingCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(_outsidePopupCanvas_MouseLeftButtonDown);

                _popupRoot.Children.Add(_popupClosingCanvas);

                _popup.Child = _popupRoot;
            }
        }

        
        public void ShowPopup()
        {
            CreatePopup();

            // 1. Get the position of the button that triggers the show popup
            //    Get Button display coords and add height
            Point position = CalculateControlCoords(_buttonContainer);

            if (SelectionContainer != null)
            {
                if (!_popupRoot.Children.Contains(SelectionContainer))
                {
                    _popupRoot.Children.Add(SelectionContainer);
                }

                Content hostContent = Application.Current.Host.Content;
                double rootWidth = hostContent.ActualWidth;
                double rootHeight = hostContent.ActualHeight;

                _popupClosingCanvas.Width = rootWidth;
                _popupClosingCanvas.Height = rootHeight;

                SelectionContainer.SetValue(Canvas.LeftProperty, position.X);
                SelectionContainer.SetValue(Canvas.TopProperty, position.Y);
            }

            _popup.HorizontalOffset = 0;
            _popup.VerticalOffset = 0;
            _popup.IsOpen = true;

            VisualStateManager.GoToState(this, SelectingVisualState, true);
        }

        void Popup_Opened(object sender, System.EventArgs e)
        {
            VisualStateManager.GoToState(this, SelectingVisualState, true);
        }

        void Popup_Closed(object sender, System.EventArgs e)
        {
            if (!_skipStateChange)
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }


            _skipStateChange = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_popup != null && _popup.IsOpen)
            {
                VisualStateManager.GoToState(this, SelectingVisualState, true);
            }
            else
            {
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (_popup != null && _popup.IsOpen)
            {
                VisualStateManager.GoToState(this, SelectingVisualState, true);
            }
        }

        public void ClosePopup()
        {
            _popup.IsOpen = false;

            _dropDownArrowToggleButton.IsChecked = false;
        }

        private void _outsidePopupCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var elements = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), _buttonContainer);

            if (elements != null && elements.Contains(_buttonContainer))
            {
                VisualStateManager.GoToState(this, "MouseOver", true);

                _skipStateChange = true;
            }

            ClosePopup();
        }

        private Point CalculateControlCoords(UIElement element)
        {
            GeneralTransform absoluteTransformation = element.TransformToVisual(Application.Current.RootVisual as UIElement);
            Point position = absoluteTransformation.Transform(new Point(0, 0));

            position.Y += _buttonContainer.ActualHeight;

            return position;
        }

        #endregion
    }
}
