using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents an item that may reside in a QuickNavigator DockWindow list.
    /// </summary>
    [RadToolboxItemAttribute(false)]
    public class QuickNavigatorListItem : RadButtonElement
    {
        #region Constructor

        static QuickNavigatorListItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new QuickNavigatorListItemStateManagerFactory(), typeof(QuickNavigatorListItem));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ImageAlignment = ContentAlignment.MiddleLeft;
            this.TextAlignment = ContentAlignment.MiddleLeft;
            this.TextImageRelation = TextImageRelation.ImageBeforeText;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.ButtonFillElement.Class = "QuickNavigatorListItemFill";
            this.BorderElement.Class = "QuickNavigatorListItemBorder";
            this.TextElement.Class = "QuickNavigatorListItemText";
            this.TextAlignment = (this.RightToLeft) ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            //TODO: This is for testing purposes, should be removed once Themed
            if (e.Property == RadButtonElement.IsDefaultProperty)
            {
                if ((bool)e.NewValue)
                {
                    this.Font = new Font(this.Font, FontStyle.Bold);
                }
                else
                {
                    this.Font = new Font(this.Font, FontStyle.Regular);
                }
            }
            if (e.Property == RadElement.RightToLeftProperty)
            {
                this.TextAlignment = (this.RightToLeft) ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// A property to indicate whether an item is currently active (displayed with bold text on an owning QuickNavigator).
        /// </summary>
        public static RadProperty IsActiveProperty = RadProperty.Register(
            "IsActive", typeof(bool), typeof(QuickNavigatorListItem), 
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay | 
                                                  ElementPropertyOptions.AffectsArrange | 
                                                  ElementPropertyOptions.AffectsMeasure |
                                                  ElementPropertyOptions.CanInheritValue));

        /// <summary>
        /// Determines whether the item represents an active window.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return (bool)this.GetValue(IsActiveProperty);
            }
            set
            {
                this.SetValue(IsActiveProperty, value);
            }
        }


        internal QuickNavigatorList Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        #endregion

        #region Fields

        private QuickNavigatorList owner;

        #endregion
    }
}
