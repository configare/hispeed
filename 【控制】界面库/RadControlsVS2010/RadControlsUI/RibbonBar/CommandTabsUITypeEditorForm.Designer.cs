namespace Telerik.WinControls.UI.RibbonBar
{
    partial class CommandTabsUITypeEditorForm
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.propGridTabProperties = new System.Windows.Forms.PropertyGrid();
            this.radBtnUp = new Telerik.WinControls.UI.RadButton();
            this.radBtnDown = new Telerik.WinControls.UI.RadButton();
            this.radBtnCancel = new Telerik.WinControls.UI.RadButton();
            this.radBtnRemoveTabs = new Telerik.WinControls.UI.RadButton();
            this.radBtnOk = new Telerik.WinControls.UI.RadButton();
            this.radBtnAddNewTab = new Telerik.WinControls.UI.RadButton();
            this.radListBoxTabItems = new Telerik.WinControls.UI.RadListBox();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnRemoveTabs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnAddNewTab)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radListBoxTabItems)).BeginInit();
            this.SuspendLayout();
            // 
            // propGridTabProperties
            // 
            this.propGridTabProperties.Location = new System.Drawing.Point(299, 15);
            this.propGridTabProperties.Name = "propGridTabProperties";
            this.propGridTabProperties.Size = new System.Drawing.Size(286, 463);
            this.propGridTabProperties.TabIndex = 1;
            // 
            // radBtnUp
            // 
            this.radBtnUp.AllowShowFocusCues = true;
            this.radBtnUp.Location = new System.Drawing.Point(246, 13);
            this.radBtnUp.Name = "radBtnUp";
            this.radBtnUp.Size = new System.Drawing.Size(40, 37);
            this.radBtnUp.TabIndex = 2;
            this.radBtnUp.Text = "Up";
            this.radBtnUp.Click += new System.EventHandler(this.radBtnUp_Click);
            // 
            // radBtnDown
            // 
            this.radBtnDown.AllowShowFocusCues = true;
            this.radBtnDown.Location = new System.Drawing.Point(246, 56);
            this.radBtnDown.Name = "radBtnDown";
            this.radBtnDown.Size = new System.Drawing.Size(40, 37);
            this.radBtnDown.TabIndex = 2;
            this.radBtnDown.Text = "Down";
            this.radBtnDown.Click += new System.EventHandler(this.radBtnDown_Click);
            // 
            // radBtnCancel
            // 
            this.radBtnCancel.AllowShowFocusCues = true;
            this.radBtnCancel.Location = new System.Drawing.Point(445, 484);
            this.radBtnCancel.Name = "radBtnCancel";
            this.radBtnCancel.Size = new System.Drawing.Size(140, 35);
            this.radBtnCancel.TabIndex = 2;
            this.radBtnCancel.Text = "Cancel";
            this.radBtnCancel.Click += new System.EventHandler(this.radBtnCancel_Click);
            // 
            // radBtnRemoveTabs
            // 
            this.radBtnRemoveTabs.AllowShowFocusCues = true;
            this.radBtnRemoveTabs.Location = new System.Drawing.Point(129, 484);
            this.radBtnRemoveTabs.Name = "radBtnRemoveTabs";
            this.radBtnRemoveTabs.Size = new System.Drawing.Size(111, 35);
            this.radBtnRemoveTabs.TabIndex = 2;
            this.radBtnRemoveTabs.Text = "Remove";
            this.radBtnRemoveTabs.Click += new System.EventHandler(this.radBtnRemoveTabs_Click);
            // 
            // radBtnOk
            // 
            this.radBtnOk.AllowShowFocusCues = true;
            this.radBtnOk.Location = new System.Drawing.Point(299, 484);
            this.radBtnOk.Name = "radBtnOk";
            this.radBtnOk.Size = new System.Drawing.Size(140, 35);
            this.radBtnOk.TabIndex = 2;
            this.radBtnOk.Text = "OK";
            this.radBtnOk.Click += new System.EventHandler(this.radBtnOK_Click);
            // 
            // radBtnAddNewTab
            // 
            this.radBtnAddNewTab.AllowShowFocusCues = true;
            this.radBtnAddNewTab.Location = new System.Drawing.Point(13, 484);
            this.radBtnAddNewTab.Name = "radBtnAddNewTab";
            this.radBtnAddNewTab.Size = new System.Drawing.Size(110, 35);
            this.radBtnAddNewTab.TabIndex = 2;
            this.radBtnAddNewTab.Text = "Add";
            this.radBtnAddNewTab.Click += new System.EventHandler(this.radBtnAddNewTab_Click);
            // 
            // radListBoxTabItems
            // 
            this.radListBoxTabItems.Location = new System.Drawing.Point(13, 13);
            this.radListBoxTabItems.Name = "radListBoxTabItems";
            this.radListBoxTabItems.Size = new System.Drawing.Size(227, 463);
            this.radListBoxTabItems.TabIndex = 0;
            // 
            // CommandTabsUITypeEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 534);
            this.Controls.Add(this.radBtnUp);
            this.Controls.Add(this.radBtnDown);
            this.Controls.Add(this.radBtnCancel);
            this.Controls.Add(this.radBtnRemoveTabs);
            this.Controls.Add(this.radBtnOk);
            this.Controls.Add(this.radBtnAddNewTab);
            this.Controls.Add(this.propGridTabProperties);
            this.Controls.Add(this.radListBoxTabItems);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommandTabsUITypeEditorForm";
            this.ShowInTaskbar = false;
            this.Text = "Command Tabs Editor";
            ((System.ComponentModel.ISupportInitialize)(this.radBtnUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnRemoveTabs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnAddNewTab)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radListBoxTabItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RadListBox radListBoxTabItems;
        private System.Windows.Forms.PropertyGrid propGridTabProperties;
        private RadButton radBtnAddNewTab;
        private RadButton radBtnRemoveTabs;
        private RadButton radBtnUp;
        private RadButton radBtnDown;
        private RadButton radBtnOk;
        private RadButton radBtnCancel;
    }
}