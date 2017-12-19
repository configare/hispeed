using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.UI.Data;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadListControlAccessibleObject : Control.ControlAccessibleObject
    {
        #region Constructor

        public RadListControlAccessibleObject(RadListControl owner)
            : base(owner)
        {
            this.List.SelectedIndexChanged += new PositionChangedEventHandler(Control_SelectedIndexChanged);
        }

        #endregion

        #region Event Handlers

        private void Control_SelectedIndexChanged(object sender, PositionChangedEventArgs e)
        {
            this.NotifyClients(AccessibleEvents.Selection, e.Position);
        }

        #endregion

        #region Properties

        public RadListControl List
        {
            get
            {
                return this.Owner as RadListControl;
            }
        }

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.List;
            }
        }

        public override string Value
        {
            get
            {
                if (this.List.SelectedItem != null)
                {
                    return this.List.SelectedItem.Text;
                }

                return null;
            }
            set
            {
                if (this.List.SelectedItem.Text != null)
                {
                    this.List.SelectedItem.Text = value;
                }
            }
        }

        #endregion

        #region Methods

        public override AccessibleObject GetChild(int index)
        {
            return new RadListDataItemAccessibleObject(this.List.Items[index]);
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            Point location = this.List.PointToClient(new Point(x, y));
            RadListVisualItem visualItem = this.List.ElementTree.GetElementAtPoint(location) as RadListVisualItem;

            if (visualItem != null)
            {
                return new RadListDataItemAccessibleObject(visualItem.Data);
            }

            return null;
        }

        public override int GetChildCount()
        {
            if (this.List.IsDisposed)
            {
                return 0;
            }

            return this.List.Items.Count;
        }


        public override AccessibleObject Navigate(AccessibleNavigation direction)
        {
            return null;
        }

        #endregion
    }
}
