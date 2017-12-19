using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a simple dialog that provides customization options for the <c ref="RadCommandBar"/> element.
    /// </summary>
    public partial class CommandBarCustomizeDialog : RadForm
    { 
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                this.UnwireEvents();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.radPageView = new Telerik.WinControls.UI.RadPageView();
            this.toolstripsPage = new Telerik.WinControls.UI.RadPageViewPage();
            this.stripsListControl = new Telerik.WinControls.UI.RadListControl();
            this.toolstripItemsPage = new Telerik.WinControls.UI.RadPageViewPage();
            this.resetItemsButton = new Telerik.WinControls.UI.RadButton();
            this.chooseToolstripLabel = new Telerik.WinControls.UI.RadLabel();
            this.moveDownButton = new Telerik.WinControls.UI.RadButton();
            this.moveUpButton = new Telerik.WinControls.UI.RadButton();
            this.stripItemsListControl = new Telerik.WinControls.UI.RadListControl();
            this.stripsDropDownList = new Telerik.WinControls.UI.RadDropDownList();
            this.closeButton = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView)).BeginInit();
            this.radPageView.SuspendLayout();
            this.toolstripsPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stripsListControl)).BeginInit();
            this.toolstripItemsPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resetItemsButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chooseToolstripLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.moveDownButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.moveUpButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stripItemsListControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stripsDropDownList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radPageView
            // 
            this.radPageView.Controls.Add(this.toolstripsPage);
            this.radPageView.Controls.Add(this.toolstripItemsPage);
            this.radPageView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPageView.Location = new System.Drawing.Point(0, 0);
            this.radPageView.Name = "radPageView";
            this.radPageView.Padding = new System.Windows.Forms.Padding(3, 3, 3, 26);
            // 
            // 
            // 
            this.radPageView.RootElement.Padding = new System.Windows.Forms.Padding(3, 3, 3, 26);
            this.radPageView.SelectedPage = this.toolstripItemsPage;
            this.radPageView.Size = new System.Drawing.Size(392, 366);
            this.radPageView.TabIndex = 0;
            this.radPageView.Text = "f";
            // 
            // toolstripsPage
            // 
            this.toolstripsPage.Controls.Add(this.stripsListControl);
            this.toolstripsPage.Location = new System.Drawing.Point(8, 35);
            this.toolstripsPage.Name = "toolstripsPage";
            this.toolstripsPage.Size = new System.Drawing.Size(376, 300);
            this.toolstripsPage.Text = "Toolstrips";
            // 
            // stripsListControl
            // 
            this.stripsListControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stripsListControl.CaseSensitiveSort = true;
            this.stripsListControl.Location = new System.Drawing.Point(3, 3);
            this.stripsListControl.Name = "stripsListControl";
            this.stripsListControl.Size = new System.Drawing.Size(370, 294);
            this.stripsListControl.TabIndex = 0;
            this.stripsListControl.Text = "radListControl1";
            // 
            // toolstripItemsPage
            // 
            this.toolstripItemsPage.Controls.Add(this.resetItemsButton);
            this.toolstripItemsPage.Controls.Add(this.chooseToolstripLabel);
            this.toolstripItemsPage.Controls.Add(this.moveDownButton);
            this.toolstripItemsPage.Controls.Add(this.moveUpButton);
            this.toolstripItemsPage.Controls.Add(this.stripItemsListControl);
            this.toolstripItemsPage.Controls.Add(this.stripsDropDownList);
            this.toolstripItemsPage.Location = new System.Drawing.Point(8, 35);
            this.toolstripItemsPage.Name = "toolstripItemsPage";
            this.toolstripItemsPage.Size = new System.Drawing.Size(376, 300);
            this.toolstripItemsPage.Text = "Toolstrip Items";
            // 
            // resetItemsButton
            // 
            this.resetItemsButton.Location = new System.Drawing.Point(284, 114);
            this.resetItemsButton.Name = "resetItemsButton";
            this.resetItemsButton.Size = new System.Drawing.Size(89, 24);
            this.resetItemsButton.TabIndex = 8;
            this.resetItemsButton.Text = "Reset";
            this.resetItemsButton.Visible = false;
            // 
            // chooseToolstripLabel
            // 
            this.chooseToolstripLabel.BackColor = System.Drawing.Color.Transparent;
            this.chooseToolstripLabel.Location = new System.Drawing.Point(1, 3);
            this.chooseToolstripLabel.Name = "chooseToolstripLabel";
            this.chooseToolstripLabel.Size = new System.Drawing.Size(165, 18);
            this.chooseToolstripLabel.TabIndex = 7;
            this.chooseToolstripLabel.Text = "Choose a toolstrip to rearrange:";
            // 
            // moveDownButton
            // 
            this.moveDownButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.moveDownButton.Location = new System.Drawing.Point(284, 85);
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Size = new System.Drawing.Size(89, 22);
            this.moveDownButton.TabIndex = 6;
            this.moveDownButton.Text = "Move Down";
            this.moveDownButton.Click += new System.EventHandler(this.moveDownButton_Click);
            // 
            // moveUpButton
            // 
            this.moveUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.moveUpButton.Location = new System.Drawing.Point(284, 55);
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(89, 22);
            this.moveUpButton.TabIndex = 5;
            this.moveUpButton.Text = "Move Up";
            this.moveUpButton.Click += new System.EventHandler(this.moveUpButton_Click);
            // 
            // stripItemsListControl
            // 
            this.stripItemsListControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stripItemsListControl.CaseSensitiveSort = true;
            this.stripItemsListControl.Location = new System.Drawing.Point(3, 55);
            this.stripItemsListControl.Name = "stripItemsListControl";
            this.stripItemsListControl.Size = new System.Drawing.Size(269, 242);
            this.stripItemsListControl.TabIndex = 1;
            this.stripItemsListControl.Text = "radListControl1";
            // 
            // stripsDropDownList
            // 
            this.stripsDropDownList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stripsDropDownList.Location = new System.Drawing.Point(3, 27);
            this.stripsDropDownList.Name = "stripsDropDownList";
            this.stripsDropDownList.ShowImageInEditorArea = true;
            this.stripsDropDownList.Size = new System.Drawing.Size(370, 21);
            this.stripsDropDownList.TabIndex = 0;
            this.stripsDropDownList.SelectedValueChanged += new System.EventHandler(this.toolstripsDownList_SelectedValueChanged);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.Transparent;
            this.closeButton.Location = new System.Drawing.Point(296, 342);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(93, 24);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Close";
            // 
            // CommandBarCustomizeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 366);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.radPageView);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 220);
            this.Name = "CommandBarCustomizeDialog";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Customize";
            ((System.ComponentModel.ISupportInitialize)(this.radPageView)).EndInit();
            this.radPageView.ResumeLayout(false);
            this.toolstripsPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.stripsListControl)).EndInit();
            this.toolstripItemsPage.ResumeLayout(false);
            this.toolstripItemsPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resetItemsButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chooseToolstripLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.moveDownButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.moveUpButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stripItemsListControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stripsDropDownList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
         
        #region Fields

        public RadPageView radPageView;
        public RadPageViewPage toolstripsPage;
        public RadPageViewPage toolstripItemsPage;
        public RadListControl stripsListControl;
        public RadButton closeButton;
        public RadDropDownList stripsDropDownList;
        public RadListControl stripItemsListControl;
        public RadButton moveUpButton;
        public RadButton moveDownButton;
        public RadLabel chooseToolstripLabel;
        public RadButton resetItemsButton;
        public CommandBarStripInfoHolder stripInfoHolder;

        #endregion

        #region Ctors

        public CommandBarCustomizeDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates a customize dialog that provides customization options for the strips in the specified <c ref="CommandBarStripInfoHolder"/>.
        /// </summary>
        /// <param name="stripInfoHolder">The <c ref="CommandBarStripInfoHolder"/> from which the information for the strips will be taken.</param>
        public CommandBarCustomizeDialog(CommandBarStripInfoHolder stripInfoHolder)
        {
            InitializeComponent(); 
            chooseToolstripLabel.LabelElement.LabelFill.Visibility = ElementVisibility.Visible;
            SetLocalizedStrings();
            
            this.stripInfoHolder = stripInfoHolder;

            this.stripsListControl.CreatingVisualListItem += new CreatingVisualListItemEventHandler(stripsListControl_CreatingVisualListItem);
            this.stripsListControl.ItemDataBinding += new ListItemDataBindingEventHandler(ItemDataBinding);
            this.stripItemsListControl.CreatingVisualListItem += new CreatingVisualListItemEventHandler(toolstripItemsListControl_CreatingVisualListItem);
            this.stripItemsListControl.ItemDataBinding += new ListItemDataBindingEventHandler(toolstripItemsListControl_ItemDataBinding);
            
            this.stripsListControl.DataSource = stripInfoHolder.StripInfoList;
            
            this.stripsDropDownList.DropDownStyle = RadDropDownStyle.DropDownList;
            this.stripsDropDownList.Items.Clear();
            foreach (CommandBarStripElement strip in stripInfoHolder.StripInfoList)
            {
                RadListDataItem item = new RadListDataItem();
                item.Text = strip.DisplayName;
                item.Value = strip;
                this.stripsDropDownList.Items.Add(item);                
            }
            
            if (this.stripsDropDownList.Items.Count > 0)
            {
                this.stripsDropDownList.SelectedIndex = 0;
            }

            toolstripsDownList_SelectedValueChanged(this, EventArgs.Empty);

            this.moveUpButton.Enabled = false;

            this.WireEvents();
        }

        /// <summary>
        /// Sets the strings values depending on the current localization provider.
        /// </summary>
        protected virtual void SetLocalizedStrings()
        {
            (this.radPageView.ViewElement as RadPageViewStripElement).ItemContainer.ButtonsPanel.Visibility = ElementVisibility.Collapsed;
            this.Text = this.moveDownButton.Text = CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.CustomizeDialogTitle);
            this.closeButton.Text = CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.CustomizeDialogCloseButtonText);
            this.resetItemsButton.Text = CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.CustomizeDialogResetButtonText);
            this.moveDownButton.Text = CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.CustomizeDialogMoveDownButtonText);
            this.moveUpButton.Text = CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.CustomizeDialogMoveUpButtonText);
            this.chooseToolstripLabel.Text = CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.CustomizeDialogChooseToolstripLabelText);
            this.toolstripsPage.Text = CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.CustomizeDialogToolstripsPageTitle);
            this.toolstripItemsPage.Text = CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.CustomizeDialogItemsPageTitle);
        }

        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();
            foreach (Control control in this.Controls)
            {
                RadControl radControl = (control as RadControl);

                if (radControl != null)
                {
                    radControl.ThemeName = this.ThemeName;
                }
            }
            if (toolstripItemsPage != null)
            {
                foreach (Control control in this.toolstripItemsPage.Controls)
                {
                    RadControl radControl = (control as RadControl);

                    if (radControl != null)
                    {
                        radControl.ThemeName = this.ThemeName;
                    }
                }
            }
            if (toolstripsPage != null)
            {
                foreach (Control control in this.toolstripsPage.Controls)
                {
                    RadControl radControl = (control as RadControl);

                    if (radControl != null)
                    {
                        radControl.ThemeName = this.ThemeName;
                    }
                }
            }
        }

        #endregion

        #region Event Handlers
         
        void CommandBarLocalizationProvider_CurrentProviderChanged(object sender, EventArgs e)
        {
            this.SetLocalizedStrings();
        }

        protected virtual void toolstripItemsListControl_SelectedIndexChanged(object sender, Data.PositionChangedEventArgs e)
        {
            if (this.stripItemsListControl.SelectedIndex <= 0)
            {
                this.moveUpButton.Enabled = false;
            }
            else
            {
                this.moveUpButton.Enabled = true;
            }

            if (this.stripItemsListControl.SelectedIndex >= this.stripItemsListControl.Items.Count-1)
            {
                this.moveDownButton.Enabled = false;
            }
            else
            {
                this.moveDownButton.Enabled = true;
            }
             
        }

        protected virtual void toolstripsDropDownList_SelectedIndexChanged(object sender, Data.PositionChangedEventArgs e)
        {
            stripsListControl.SelectedIndex = e.Position;
        }

        protected virtual void stripsListControl_SelectedIndexChanged(object sender, Data.PositionChangedEventArgs e)
        {
            this.stripsDropDownList.SelectedIndex = e.Position;
        }

        protected virtual  void toolstripItemsListControl_ItemDataBinding(object sender, ListItemDataBindingEventArgs args)
        {
            args.NewItem = new CommandBarItemsListDataItem();
        }

        protected virtual  void toolstripItemsListControl_CreatingVisualListItem(object sender, CreatingVisualListItemEventArgs args)
        {
            args.VisualItem = new CommandBarItemsListVisualItem();
        }

        protected virtual  void ItemDataBinding(object sender, ListItemDataBindingEventArgs args)
        {
            args.NewItem = new CommandBarStripsListDataItem();
        }

        protected virtual  void stripsListControl_CreatingVisualListItem(object sender, CreatingVisualListItemEventArgs args)
        {
            args.VisualItem = new CommandBarStripsListVisualItem();
        }

        protected virtual  void toolstripsDownList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (stripsDropDownList.SelectedItem == null)
            {
                return;
            }

            CommandBarStripElement stripElement = (stripsDropDownList.SelectedItem.Value as CommandBarStripElement);
            if(stripElement == null)
            {
                return;
            }

            List<RadCommandBarBaseItem> items = new List<RadCommandBarBaseItem>();
            foreach (RadCommandBarBaseItem item in stripElement.Items)
            {
                items.Add(item);
            }
            foreach (RadCommandBarBaseItem item in stripElement.OverflowButton.OverflowPanel.Layout.Children)
            {
                items.Add(item);
            }

            stripItemsListControl.DataSource = items;
        }

        protected virtual  void moveUpButton_Click(object sender, EventArgs e)
        {
            int currentIndex = this.stripItemsListControl.SelectedIndex;
            int previousIndex = currentIndex - 1;

            if (previousIndex < 0)
            {
                return;
            }

            RadCommandBarBaseItem currentItem = this.stripItemsListControl.Items[currentIndex].Value as RadCommandBarBaseItem;
            RadCommandBarBaseItem previousItem = this.stripItemsListControl.Items[previousIndex].Value as RadCommandBarBaseItem;

            if (currentItem == null || previousItem == null)
            {
                return;
            }

            CommandBarStripElement stripElement = (stripsDropDownList.SelectedItem.Value as CommandBarStripElement);
            LayoutPanel overflowPanelLayout = stripElement.OverflowButton.OverflowPanel.Layout;
          
            int currentItemIndex = stripElement.Items.IndexOf(currentItem);
            int previousItemIndex = stripElement.Items.IndexOf(previousItem);

            if (currentItemIndex != -1 && previousItemIndex != -1)
            {
                RadCommandBarBaseItem temp = stripElement.Items[currentItemIndex];
                stripElement.Items.RemoveAt(currentItemIndex);
                stripElement.Items.Insert(previousItemIndex, temp); 
            }
            else if (currentItemIndex == -1 && previousItemIndex != -1)
            {
                currentItemIndex = overflowPanelLayout.Children.IndexOf(currentItem);
                if (previousItemIndex != -1)
                {
                    RadCommandBarBaseItem temp = overflowPanelLayout.Children[currentItemIndex] as RadCommandBarBaseItem;
                    if (temp != null)
                    {
                        overflowPanelLayout.Children.RemoveAt(currentItemIndex);
                        stripElement.Items.Insert(previousItemIndex, temp);
                    }
                }
            } 
            else
            {
                currentItemIndex = overflowPanelLayout.Children.IndexOf(currentItem);
                previousItemIndex = overflowPanelLayout.Children.IndexOf(previousItem);

                if (currentItemIndex != -1 && previousItemIndex != -1)
                {
                    RadElement temp = overflowPanelLayout.Children[currentItemIndex];
                    overflowPanelLayout.Children.RemoveAt(currentItemIndex);
                    overflowPanelLayout.Children.Insert(previousItemIndex, temp);
                }
            }

            toolstripsDownList_SelectedValueChanged(this, EventArgs.Empty);
            this.stripItemsListControl.SelectedIndex = previousIndex;
        }

        protected virtual  void moveDownButton_Click(object sender, EventArgs e)
        {
            int currentIndex = this.stripItemsListControl.SelectedIndex;
            int nextIndex = currentIndex + 1;

            if (nextIndex >= this.stripItemsListControl.Items.Count)
            {
                return;
            }

            RadCommandBarBaseItem currentItem = this.stripItemsListControl.Items[currentIndex].Value as RadCommandBarBaseItem;
            RadCommandBarBaseItem nextItem = this.stripItemsListControl.Items[nextIndex].Value as RadCommandBarBaseItem;

            if (currentItem == null || nextItem == null)
            {
                return;
            }

            CommandBarStripElement stripElement = (stripsDropDownList.SelectedItem.Value as CommandBarStripElement);
            LayoutPanel overflowPanelLayout = stripElement.OverflowButton.OverflowPanel.Layout;
            
            int currentItemIndex = stripElement.Items.IndexOf(currentItem);
            int nextItemIndex = stripElement.Items.IndexOf(nextItem);

            if (currentItemIndex != -1 && nextItemIndex != -1)
            {
                RadCommandBarBaseItem temp = stripElement.Items[currentItemIndex];
                stripElement.Items.RemoveAt(currentItemIndex);
                stripElement.Items.Insert(nextItemIndex, temp);
            }
            else if (currentItemIndex != -1 && nextItemIndex == -1)
            {
                nextItemIndex = overflowPanelLayout.Children.IndexOf(nextItem);
                if (nextItemIndex != -1)
                {
                    RadCommandBarBaseItem temp = overflowPanelLayout.Children[nextItemIndex] as RadCommandBarBaseItem;
                    if (temp != null)
                    {
                        overflowPanelLayout.Children.RemoveAt(nextItemIndex);
                        stripElement.Items.Insert(currentItemIndex, temp);
                    }
                }
            }
            else
            {
                currentItemIndex = overflowPanelLayout.Children.IndexOf(currentItem);
                nextItemIndex = overflowPanelLayout.Children.IndexOf(nextItem);

                if (currentItemIndex != -1 && nextItemIndex != -1)
                {
                    RadElement temp = overflowPanelLayout.Children[nextItemIndex];
                    overflowPanelLayout.Children.RemoveAt(nextItemIndex);
                    overflowPanelLayout.Children.Insert(currentItemIndex, temp);
                }
            }

            toolstripsDownList_SelectedValueChanged(this, EventArgs.Empty);
            this.stripItemsListControl.SelectedIndex = nextIndex;
        }
        #endregion

        #region Event managers

        protected virtual void UnwireEvents()
        {
            this.closeButton.Click -= delegate(object sender, EventArgs e) { this.Close(); };
            this.stripsListControl.CreatingVisualListItem -= new CreatingVisualListItemEventHandler(stripsListControl_CreatingVisualListItem);
            this.stripsListControl.ItemDataBinding -= new ListItemDataBindingEventHandler(ItemDataBinding);
            this.stripsListControl.SelectedIndexChanged -= new Data.PositionChangedEventHandler(stripsListControl_SelectedIndexChanged);
            this.stripItemsListControl.CreatingVisualListItem -= new CreatingVisualListItemEventHandler(toolstripItemsListControl_CreatingVisualListItem);
            this.stripItemsListControl.ItemDataBinding -= new ListItemDataBindingEventHandler(toolstripItemsListControl_ItemDataBinding);
            this.stripItemsListControl.SelectedIndexChanged -= new Data.PositionChangedEventHandler(toolstripItemsListControl_SelectedIndexChanged);
            CommandBarLocalizationProvider.CurrentProviderChanged -= new EventHandler(CommandBarLocalizationProvider_CurrentProviderChanged);
        }

        protected virtual void WireEvents()
        {
            this.closeButton.Click += delegate(object sender, EventArgs e) { this.Close(); };
            this.stripsListControl.SelectedIndexChanged += new Data.PositionChangedEventHandler(stripsListControl_SelectedIndexChanged);
            this.stripsDropDownList.SelectedIndexChanged += new Data.PositionChangedEventHandler(toolstripsDropDownList_SelectedIndexChanged);
            this.stripItemsListControl.SelectedIndexChanged += new Data.PositionChangedEventHandler(toolstripItemsListControl_SelectedIndexChanged);
            CommandBarLocalizationProvider.CurrentProviderChanged += new EventHandler(CommandBarLocalizationProvider_CurrentProviderChanged);
        }

        #endregion

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            chooseToolstripLabel.Location = new Point
                (chooseToolstripLabel.Parent.ClientSize.Width - chooseToolstripLabel.Location.X - chooseToolstripLabel.Width,
                chooseToolstripLabel.Location.Y);
            chooseToolstripLabel.Anchor ^= (AnchorStyles.Left | AnchorStyles.Right);
            
            closeButton.Location = new Point
                (closeButton.Parent.ClientSize.Width - closeButton.Location.X - closeButton.Width,
                closeButton.Location.Y);
            closeButton.Anchor ^= (AnchorStyles.Left | AnchorStyles.Right);

            stripItemsListControl.Location = new Point
                (stripItemsListControl.Parent.ClientSize.Width - stripItemsListControl.Location.X - stripItemsListControl.Width,
                stripItemsListControl.Location.Y);

            moveUpButton.Location = new Point
                (moveUpButton.Parent.ClientSize.Width - moveUpButton.Location.X - moveUpButton.Width,
                moveUpButton.Location.Y);
            moveUpButton.Anchor ^= (AnchorStyles.Left | AnchorStyles.Right);

            moveDownButton.Location = new Point
                (moveDownButton.Parent.ClientSize.Width - moveDownButton.Location.X - moveDownButton.Width,
                moveDownButton.Location.Y);
            moveDownButton.Anchor ^= (AnchorStyles.Left | AnchorStyles.Right);
        }
    } 
}
