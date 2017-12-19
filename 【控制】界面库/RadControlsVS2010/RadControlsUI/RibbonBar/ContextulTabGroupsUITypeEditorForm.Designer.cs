namespace Telerik.WinControls.UI
{
    public partial class ContextulTabGroupsEditor
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
            this.radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radButtonRemoveOne = new Telerik.WinControls.UI.RadButton();
            this.radButtonRemoveAll = new Telerik.WinControls.UI.RadButton();
            this.radButtonAddAll = new Telerik.WinControls.UI.RadButton();
            this.radButtonAddOne = new Telerik.WinControls.UI.RadButton();
            this.radListBoxAssociatedTabs = new Telerik.WinControls.UI.RadListBox();
            this.radListBoxAvaibleTabs = new Telerik.WinControls.UI.RadListBox();
            this.radButtonOk = new Telerik.WinControls.UI.RadButton();
            this.radButtonCancel = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).BeginInit();
            this.radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonRemoveOne)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonRemoveAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonAddAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonAddOne)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radListBoxAssociatedTabs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radListBoxAvaibleTabs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonCancel)).BeginInit();
            this.SuspendLayout();
            // 
            // radGroupBox1
            // 
            this.radGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radGroupBox1.Controls.Add(this.radLabel2);
            this.radGroupBox1.Controls.Add(this.radLabel1);
            this.radGroupBox1.Controls.Add(this.radButtonRemoveOne);
            this.radGroupBox1.Controls.Add(this.radButtonRemoveAll);
            this.radGroupBox1.Controls.Add(this.radButtonAddAll);
            this.radGroupBox1.Controls.Add(this.radButtonAddOne);
            this.radGroupBox1.Controls.Add(this.radListBoxAssociatedTabs);
            this.radGroupBox1.Controls.Add(this.radListBoxAvaibleTabs);
            this.radGroupBox1.FooterImageIndex = -1;
            this.radGroupBox1.FooterImageKey = "";
            this.radGroupBox1.HeaderImageIndex = -1;
            this.radGroupBox1.HeaderImageKey = "";
            this.radGroupBox1.HeaderMargin = new System.Windows.Forms.Padding(0);
            this.radGroupBox1.HeaderText = "Tabs";
            this.radGroupBox1.Location = new System.Drawing.Point(13, 13);
            this.radGroupBox1.Name = "radGroupBox1";
            this.radGroupBox1.Size = new System.Drawing.Size(588, 412);
            this.radGroupBox1.TabIndex = 0;
            this.radGroupBox1.Text = "Tabs";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(325, 33);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(88, 14);
            this.radLabel2.TabIndex = 5;
            this.radLabel2.Text = "Associated Tabs";
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(13, 33);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(79, 14);
            this.radLabel1.TabIndex = 5;
            this.radLabel1.Text = "Available Tabs";
            // 
            // radButtonRemoveOne
            // 
            this.radButtonRemoveOne.AllowShowFocusCues = true;
            this.radButtonRemoveOne.Location = new System.Drawing.Point(246, 146);
            this.radButtonRemoveOne.Name = "radButtonRemoveOne";
            this.radButtonRemoveOne.Size = new System.Drawing.Size(75, 23);
            this.radButtonRemoveOne.TabIndex = 4;
            this.radButtonRemoveOne.Text = "<";
            this.radButtonRemoveOne.Click += new System.EventHandler(this.radButtonRemoveOne_Click);
            // 
            // radButtonRemoveAll
            // 
            this.radButtonRemoveAll.AllowShowFocusCues = true;
            this.radButtonRemoveAll.Location = new System.Drawing.Point(246, 116);
            this.radButtonRemoveAll.Name = "radButtonRemoveAll";
            this.radButtonRemoveAll.Size = new System.Drawing.Size(75, 23);
            this.radButtonRemoveAll.TabIndex = 3;
            this.radButtonRemoveAll.Text = "<<";
            this.radButtonRemoveAll.Click += new System.EventHandler(this.radButtonRemoveAll_Click);
            // 
            // radButtonAddAll
            // 
            this.radButtonAddAll.AllowShowFocusCues = true;
            this.radButtonAddAll.Location = new System.Drawing.Point(246, 86);
            this.radButtonAddAll.Name = "radButtonAddAll";
            this.radButtonAddAll.Size = new System.Drawing.Size(75, 23);
            this.radButtonAddAll.TabIndex = 2;
            this.radButtonAddAll.Text = ">>";
            this.radButtonAddAll.Click += new System.EventHandler(this.radButtonAddAll_Click);
            // 
            // radButtonAddOne
            // 
            this.radButtonAddOne.AllowShowFocusCues = true;
            this.radButtonAddOne.Location = new System.Drawing.Point(246, 57);
            this.radButtonAddOne.Name = "radButtonAddOne";
            this.radButtonAddOne.Size = new System.Drawing.Size(75, 23);
            this.radButtonAddOne.TabIndex = 1;
            this.radButtonAddOne.Text = ">";
            this.radButtonAddOne.Click += new System.EventHandler(this.radButtonAddOne_Click);
            // 
            // radListBoxAssociatedTabs
            // 
            this.radListBoxAssociatedTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radListBoxAssociatedTabs.Location = new System.Drawing.Point(325, 55);
            this.radListBoxAssociatedTabs.Name = "radListBoxAssociatedTabs";
            this.radListBoxAssociatedTabs.Size = new System.Drawing.Size(248, 340);
            this.radListBoxAssociatedTabs.TabIndex = 0;
            // 
            // radListBoxAvaibleTabs
            // 
            this.radListBoxAvaibleTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.radListBoxAvaibleTabs.Location = new System.Drawing.Point(13, 55);
            this.radListBoxAvaibleTabs.Name = "radListBoxAvaibleTabs";
            this.radListBoxAvaibleTabs.Size = new System.Drawing.Size(227, 340);
            this.radListBoxAvaibleTabs.TabIndex = 0;
            // 
            // radButtonOk
            // 
            this.radButtonOk.AllowShowFocusCues = true;
            this.radButtonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonOk.Location = new System.Drawing.Point(422, 430);
            this.radButtonOk.Name = "radButtonOk";
            this.radButtonOk.Size = new System.Drawing.Size(75, 23);
            this.radButtonOk.TabIndex = 1;
            this.radButtonOk.Text = "OK";
            this.radButtonOk.Click += new System.EventHandler(this.radButtonOk_Click);
            // 
            // radButtonCancel
            // 
            this.radButtonCancel.AllowShowFocusCues = true;
            this.radButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonCancel.Location = new System.Drawing.Point(524, 430);
            this.radButtonCancel.Name = "radButtonCancel";
            this.radButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.radButtonCancel.TabIndex = 2;
            this.radButtonCancel.Text = "Cancel";
            this.radButtonCancel.Click += new System.EventHandler(this.radButtonCancel_Click);
            // 
            // ContextulTabGroupsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 458);
            this.Controls.Add(this.radButtonCancel);
            this.Controls.Add(this.radButtonOk);
            this.Controls.Add(this.radGroupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ContextulTabGroupsEditor";
            this.Text = "Contextual Tab Groups Editor";
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).EndInit();
            this.radGroupBox1.ResumeLayout(false);
            this.radGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonRemoveOne)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonRemoveAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonAddAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonAddOne)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radListBoxAssociatedTabs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radListBoxAvaibleTabs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonCancel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RadGroupBox radGroupBox1;
        private RadListBox radListBoxAssociatedTabs;
        private RadListBox radListBoxAvaibleTabs;
        private RadButton radButtonOk;
        private RadButton radButtonCancel;
        private RadButton radButtonRemoveOne;
        private RadButton radButtonRemoveAll;
        private RadButton radButtonAddAll;
        private RadButton radButtonAddOne;
        private RadLabel radLabel2;
        private RadLabel radLabel1;
    }
}