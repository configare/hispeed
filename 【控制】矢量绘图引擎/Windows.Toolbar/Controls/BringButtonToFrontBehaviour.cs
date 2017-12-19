using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Windows.Toolbar.Controls
{
    public class BringButtonToFrontBehaviour : Behavior<ButtonBase>
    {
        #region Fields

        private int _lastZIndex;

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseEnter += new MouseEventHandler(AssociatedObject_MouseEnter);
            AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseLeave);
        }

        private void AssociatedObject_MouseEnter(object sender, MouseEventArgs e)
        {
            BringToFront();
        }

        private void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            BringToBack();
        }

        private void BringToFront()
        {
            Canvas canvas = AssociatedObject.Parent as Canvas;
            if (canvas != null)
            {
                _lastZIndex = Canvas.GetZIndex(AssociatedObject);

                Canvas.SetZIndex(AssociatedObject, 1000);
            }
        }

        private void BringToBack()
        {

            Canvas canvas = AssociatedObject.Parent as Canvas;
            if (canvas != null)
            {
                Canvas.SetZIndex(AssociatedObject, _lastZIndex);
            }
        }
    }
}
