using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Styles;
using System.Diagnostics;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a tab on the <c ref="BackstageItemsPanelElement"/> which has a page associated with it.
    /// </summary>
    public class BackstageTabItem : BackstageVisualElement
    {
        #region State Manager

        class BackstageTabItemStateManagerFactory : BackstageItemStateManagerFactory
        {
            protected override StateNodeBase CreateSpecificStates()
            {
                StateNodeWithCondition selected = new StateNodeWithCondition("Selected", new SimpleCondition(BackstageTabItem.SelectedProperty, true));
                CompositeStateNode all = new CompositeStateNode("backstage tab item states");
                all.AddState(selected);

                return all;
            }

            protected override ItemStateManagerBase CreateStateManager()
            {
                ItemStateManagerBase sm = base.CreateStateManager();

                sm.AddDefaultVisibleState("Selected");

                return sm;
            }
        }

        #endregion

        #region RadProperties

        public static readonly RadProperty SelectedProperty = RadProperty.Register(
             "Selected", typeof(bool), typeof(BackstageTabItem), new RadElementPropertyMetadata(
                 false, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsArrange));

        #endregion

        #region Fields

        protected BackstageViewPage page;
        private bool cachedSelected;

        #endregion

        #region Contructors

        static BackstageTabItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new BackstageTabItemStateManagerFactory(), typeof(BackstageTabItem));
        }

        public BackstageTabItem()
        {

        }

        public BackstageTabItem(String text)
        {
            this.Text = text;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this tab is selected.
        /// </summary>
        [Browsable(false)]
        [Description("Indicates whether this tab is selected.")]
        public bool Selected
        {
            get
            {
                return cachedSelected;
            }
            internal set
            {
                if (cachedSelected != value)
                {
                    cachedSelected = value;
                    this.SetValue(BackstageTabItem.SelectedProperty, value);
                    this.OnSelectedChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the page that is associated with this tab item.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the page that is associated with this tab item.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BackstageViewPage Page
        {
            get
            {
                if (this.page == null)
                {
                    this.page = new BackstageViewPage();
                }
                return this.page;
            }
            set
            {
                this.SetPageCore(value);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the selected state of this item has changed.
        /// </summary>
        public event EventHandler SelectedChanged;

        /// <summary>
        /// Occurs when the page associated with this item has changed.
        /// </summary>
        public event EventHandler PageChagned;

        #endregion

        #region Event managers

        protected virtual void OnSelectedChanged()
        {
            if (this.SelectedChanged != null)
            {
                this.SelectedChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnPageChanged()
        {
            if (this.PageChagned != null)
            {
                this.PageChagned(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.cachedSelected = false;
            this.Class = "BackstageTabItem";
            this.ThemeRole = "BackstageTabItem";
            this.DrawFill = true;
            this.MinSize = new Size(0, 37);
            this.TextAlignment = ContentAlignment.MiddleLeft;
            this.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (this.ElementTree != null && this.ElementTree.Control != null)
            {
                BackstageItemsPanelElement parentElement = (this.Parent as BackstageItemsPanelElement);
                Debug.Assert(parentElement != null, "BackstageTabItem can only be placed in BackstageItemsPanelElement.");

                parentElement.Owner.OnItemClicked(this);

                parentElement.Owner.SelectedItem = this;
            }

        }

        #endregion

        #region Helpers

        private void SetPageCore(BackstageViewPage value)
        {
            if (value == this.page)
            {
                return;
            }

            this.page = value;

            if (this.page == null)
            {
                return;
            }

            this.page.Item = this;

            if (!this.Selected)
            {
                this.page.Visible = false;
            }

            if (this.ElementTree != null)
            {
                this.page.Parent = this.ElementTree.Control;
            }

            this.OnPageChanged();

            return;
        }

        #endregion
    }
}
