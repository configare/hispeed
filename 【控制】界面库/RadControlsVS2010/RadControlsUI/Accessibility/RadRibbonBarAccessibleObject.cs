using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadRibbonBarAccessibleObject : Control.ControlAccessibleObject
    {
        #region Fields

        private RadRibbonBar ribbonBar;

        #endregion

        #region Constructor

        public RadRibbonBarAccessibleObject(RadRibbonBar owner)
            : base(owner)
        {
            this.ribbonBar = owner;
        }

        #endregion

        #region Properties

        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = base.State;
                RadRibbonBar owner = (RadRibbonBar)base.Owner;
                if (owner.RibbonBarElement.IsMouseDown)
                {
                    state |= AccessibleStates.Pressed;
                }

                return state;
            }
        }

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.MenuBar;
            }
        }

        public override string Description
        {
            get
            {
                return Owner.Text;
            }
        }

        public override string Name
        {
            get
            {
                return Owner.Name;
            }
            set
            {
                Owner.Name = value;
            }
        }

        #endregion

        #region Methods

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override void DoDefaultAction()
        {
            this.ribbonBar.RibbonBarElement.ApplicationButtonElement.CallDoClick(EventArgs.Empty);
        }

        public override int GetChildCount()
        {
            int count = this.ribbonBar.QuickAccessToolBarItems.Count + 2;

            for (int i = 0; i < this.ribbonBar.CommandTabs.Count; i++)
            {
                count += this.GetRibbonTabChildrenCount(this.ribbonBar.CommandTabs[i] as RibbonTab);
            }

            return count;
        }

        private int GetRibbonTabChildrenCount(RibbonTab ribbonTab)
        {
            int count = 0;

            for (int i = 0; i < ribbonTab.Items.Count; i++)
            {
                count += this.GetRibbonBarGroupChildrenCount(ribbonTab.Items[i] as RadRibbonBarGroup);
            }

            return count;
        }

        private int GetRibbonBarGroupChildrenCount(RadRibbonBarGroup group)
        {
            int count = 0;

            for (int i = 0; i < group.Items.Count; i++)
            {
                count += this.GetRibbonBarGroupChildrenCount(group.Items[i]);
            }

            return count;
        }

        private int GetRibbonBarGroupChildrenCount(RadItem item)
        {
            RadRibbonBarButtonGroup buttonGroup = item as RadRibbonBarButtonGroup;

            if (buttonGroup == null)
            {
                return 1;
            }

            int count = 0;

            for (int i = 0; i < buttonGroup.Items.Count; ++i)
            {
                count += this.GetRibbonBarGroupChildrenCount(buttonGroup.Items[i]);
            }

            return count;
        }

        //Gets the Accessibility object of the child CurveLegend idetified by index.
        public override AccessibleObject GetChild(int index)
        {
            if (index == 0)
            {
                return new RadApplicationMenuButtonElementAccessibleObject(this.ribbonBar.RibbonBarElement.ApplicationButtonElement);
            }

            index--;

            AccessibleObject quickAccessAccessible = this.GetQuickAccessBarAccesibleObject(index);

            if (quickAccessAccessible != null)
            {
                return quickAccessAccessible;
            }

            index = index - this.ribbonBar.QuickAccessToolBarItems.Count;
            int count = 0;

            for (int i = 0; i < this.ribbonBar.CommandTabs.Count; i++)
            {
                RibbonTab ribbonTab = this.ribbonBar.CommandTabs[i] as RibbonTab;

                for (int j = 0; j < ribbonTab.Items.Count; j++)
                {
                    RadRibbonBarGroup ribbonBarGroup = ribbonTab.Items[j] as RadRibbonBarGroup;

                    for (int k = 0; k < ribbonBarGroup.Items.Count; k++)
                    {
                        RadItem currentItem = this.GetRibbonBarGroupChildren(ribbonBarGroup.Items[k], ref count, index);

                        if (currentItem != null)
                        {
                            return new RadRibbonBarItemAccessibleObject(currentItem, this);
                        }
                    }
                }
            }

            return null;
        }

        private AccessibleObject GetQuickAccessBarAccesibleObject(int index)
        {
            if (index < this.ribbonBar.QuickAccessToolBarItems.Count)
            {
                RadItem item = this.ribbonBar.QuickAccessToolBarItems[index];

                if (item is RadDropDownButtonElement)
                {
                    return new RadDropDownButtonElementAccessibleObject(item as RadDropDownButtonElement);
                }
                else if (item is RadButtonElement)
                {
                    return new RadButtonElementAccessibleObject(item as RadButtonElement);
                }

                return new AccessibleObject();
            }
            else if (index == this.ribbonBar.QuickAccessToolBarItems.Count)
            {
                RadToolStripOverFlowButtonElement overFlowButton = this.ribbonBar.RibbonBarElement.QuickAccessToolBar.OverflowButtonElement as RadToolStripOverFlowButtonElement;

                return new RadDropDownButtonElementAccessibleObject(overFlowButton);
            }

            return null;
        }

        private RadItem GetRibbonBarGroupChildren(RadItem item, ref int count, int index)
        {
            RadRibbonBarButtonGroup buttonGroup = item as RadRibbonBarButtonGroup;

            if (buttonGroup == null)
            {
                if (count == index)
                {
                    return item;
                }
                else
                {
                    return null;
                }
            }

            for (int i = 0; i < buttonGroup.Items.Count; i++)
            {
                count++;
                RadItem currentItem = this.GetRibbonBarGroupChildren(buttonGroup.Items[i], ref count, index);

                if (currentItem != null)
                {
                    return currentItem;
                }
            }

            return null;
        }

        #endregion
    }
}
