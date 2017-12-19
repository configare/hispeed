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

    [TemplateVisualState(Name = MouseOverState, GroupName = "CommonStates")]
    [TemplateVisualState(Name = NormalState, GroupName = "CommonStates")]
    public class ToolbarGroup : ContentControl
    {
        #region Constants

        public const string MouseOverState = "MouseOver";
        public const string NormalState = "Normal";

        #endregion

        /// <summary>
        /// Defines the <see cref="Footer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer", typeof(string), typeof(ToolbarGroup), null);
        /// <summary>
        /// Gets or sets [Add custom comment here].
        /// </summary>
        /// <value>[Add custom comment here].</value>
        public string Footer
        {
            get { return (string)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }

        public ToolbarGroup()
        {
            this.DefaultStyleKey = typeof(ToolbarGroup);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, MouseOverState, true);

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, NormalState, true);

            base.OnMouseMove(e);
        }
    }
}
