using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    public class RadApplicationMenuButtonElement : RadDropDownButtonElement
    {
        #region BitState Keys

        internal const ulong ShowTwoColumnDropDownMenuStateKey = RadDropDownButtonElementLastStateKey << 1;

        #endregion

        static RadApplicationMenuButtonElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ApplicationMenuButtonStateManagerFactory(), typeof(RadApplicationMenuButtonElement));
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadApplicationMenu().DeserializeTheme();
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadApplicationMenuDropDown().DeserializeTheme();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.BitState[ShowTwoColumnDropDownMenuStateKey] = true;
        }

        protected override RadDropDownButtonPopup CreateDropDown()
        {
            RadApplicationMenuDropDown menu = new RadApplicationMenuDropDown(this);
            menu.RightToLeft = (this.RightToLeft) ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
            menu.ThemeClassName = typeof(RadApplicationMenuButtonElement).Namespace + ".RadApplicationMenuDropDown";
            menu.Items.ItemsChanged += new ItemChangedDelegate(OnItemsChanged);
            return menu;
        }

        private void OnItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted)
            {
                RadMenuItemBase menuItem = target as RadMenuItemBase;

                if (menuItem != null)
                {
                    menuItem.DropDownOpening += new CancelEventHandler(OnMenuItem_DropDownOpening);
                }
            }
            else if (operation == ItemsChangeOperation.Removed)
            {
                RadMenuItemBase menuItem = target as RadMenuItemBase;

                if (menuItem != null)
                {
                    menuItem.DropDownOpening -= new CancelEventHandler(OnMenuItem_DropDownOpening);
                }
            }
            else if (operation == ItemsChangeOperation.Clearing)
            {
                foreach (RadMenuItemBase item in changed)
                {
                    item.DropDownOpening -= new CancelEventHandler(OnMenuItem_DropDownOpening);
                }
            }
        }

        private void OnMenuItem_DropDownOpening(object sender, CancelEventArgs e)
        {
            if (this.IsDesignMode)
                return;

            RadMenuItemBase menuItem = sender as RadMenuItemBase;

            if (menuItem != null)
            {
                RadPopupOpeningEventArgs eventArgs = e as RadPopupOpeningEventArgs;
                if (menuItem.Parent is RadDropDownMenuLayout)
                {
                    RadDropDownMenuLayout parentLayout = menuItem.Parent as RadDropDownMenuLayout;
                    eventArgs.CustomLocation = new System.Drawing.Point(eventArgs.CustomLocation.X,
                        parentLayout.ControlBoundingRectangle.Y + this.DropDownMenu.Location.Y);

                    RadDropDownMenu dropDownMenu = menuItem.DropDown;
                    System.Drawing.Size popupSize = new System.Drawing.Size(dropDownMenu.Width, parentLayout.ControlBoundingRectangle.Height);
                    dropDownMenu.MinimumSize = popupSize;
                }
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadElement.RightToLeftProperty && this.DropDownMenu != null)
            {
                this.DropDownMenu.RightToLeft = (this.RightToLeft) ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
            }
        }

        protected override void OnDropDownOpening(System.ComponentModel.CancelEventArgs e)
        {
            base.OnDropDownOpening(e);
            if (this.IsDesignMode)
            {
                RadApplicationMenuDropDownElement dropDownElement = this.DropDownMenu.PopupElement as RadApplicationMenuDropDownElement;
                if (dropDownElement != null)
                {
                    dropDownElement.MenuElement.SetValue(RadDropDownMenuElement.DropDownPositionProperty, DropDownPosition.LeftContent);
                    dropDownElement.TopRightContentElement.SetValue(RadDropDownMenuElement.DropDownPositionProperty, DropDownPosition.RightContent);

                    foreach (RadElement element in dropDownElement.TopRightContentElement.ChildrenHierarchy)
                    {
                        element.SetValue(RadDropDownMenuElement.DropDownPositionProperty, DropDownPosition.RightContent);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the whether RadApplicationMenu will have TwoColumnDropDownMenu.
        /// </summary>  
        [DefaultValue(true)]
        [Category("Behavior"), Description("Gets or sets the whether RadApplicationMenu will have TwoColumnDropDownMenu.")]
        public bool ShowTwoColumnDropDownMenu
        {
            get
            {
                return this.GetBitState(ShowTwoColumnDropDownMenuStateKey);
            }
            set
            {
                this.SetBitState(ShowTwoColumnDropDownMenuStateKey, value);
            }
        }

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);

            if (key == ShowTwoColumnDropDownMenuStateKey)
            {
                if (newValue)
                {
                    ((RadApplicationMenuDropDownElement)this.DropDownMenu.PopupElement).TopRightContentElement.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    ((RadApplicationMenuDropDownElement)this.DropDownMenu.PopupElement).TopRightContentElement.Visibility = ElementVisibility.Collapsed;
                }
                this.OnNotifyPropertyChanged("ShowTwoColumnDropDownMenu");
            }
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            if (this.DropDownMenu != null)
            {
                this.DropDownMenu.Items.ItemsChanged -= new ItemChangedDelegate(OnItemsChanged);
            }
        }
    }
}
