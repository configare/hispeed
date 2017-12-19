namespace Telerik.WinControls.UI
{
    public partial class ToolStripDialogForm
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
            this.radTabStrip1 = new Telerik.WinControls.UI.RadTabStrip();
            this.toolBarsTabItem = new Telerik.WinControls.UI.TabItem();
            this.radListBox1 = new Telerik.WinControls.UI.RadListBox();
            this.closeButton = new Telerik.WinControls.UI.RadButton();
            this.resetButton = new Telerik.WinControls.UI.RadButton();
            this.radTitleBar1 = new Telerik.WinControls.UI.RadTitleBar();
            ((System.ComponentModel.ISupportInitialize)(this.radTabStrip1)).BeginInit();
            this.radTabStrip1.SuspendLayout();
            this.toolBarsTabItem.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radListBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resetButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radTitleBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // radTabStrip1
            // 
            this.radTabStrip1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.radTabStrip1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.radTabStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.radTabStrip1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.toolBarsTabItem});
            this.radTabStrip1.Location = new System.Drawing.Point(12, 35);
            this.radTabStrip1.Name = "radTabStrip1";
            // 
            // 
            // 
            this.radTabStrip1.RootElement.ToolTipText = null;
            this.radTabStrip1.ScrollOffsetStep = 5;
            this.radTabStrip1.Size = new System.Drawing.Size(462, 320);
            this.radTabStrip1.TabIndex = 0;
            this.radTabStrip1.TabScrollButtonsPosition = Telerik.WinControls.UI.TabScrollButtonsPosition.RightBottom;
            this.radTabStrip1.Text = "radTabStrip1";
            // 
            // toolBarsTabItem
            // 
            this.toolBarsTabItem.Alignment = System.Drawing.ContentAlignment.BottomLeft;
            this.toolBarsTabItem.CanFocus = true;
            this.toolBarsTabItem.Class = "TabItem";
            // 
            // toolBarsTabItem.ContentPanel
            // 
            this.toolBarsTabItem.ContentPanel.BackColor = System.Drawing.Color.Transparent;
            this.toolBarsTabItem.ContentPanel.CausesValidation = true;
            this.toolBarsTabItem.ContentPanel.Controls.Add(this.radListBox1);
            this.toolBarsTabItem.ContentPanel.Controls.Add(this.closeButton);
            this.toolBarsTabItem.ContentPanel.Controls.Add(this.resetButton);
            this.toolBarsTabItem.IsSelected = true;
            this.toolBarsTabItem.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.toolBarsTabItem.Name = "toolBarsTabItem";
            this.toolBarsTabItem.StretchHorizontally = false;
            this.toolBarsTabItem.Text = "ToolBars";
            this.toolBarsTabItem.ToolTipText = null;
            // 
            // radListBox1
            // 
            this.radListBox1.BackColor = System.Drawing.Color.White;
            this.radListBox1.Location = new System.Drawing.Point(14, 14);
            this.radListBox1.Name = "radListBox1";
            this.radListBox1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // 
            // 
            this.radListBox1.RootElement.ToolTipText = null;
            this.radListBox1.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.radListBox1.Size = new System.Drawing.Size(339, 260);
            this.radListBox1.TabIndex = 4;
            this.radListBox1.Visible = false;
            // 
            // closeButton
            // 
            this.closeButton.AllowShowFocusCues = true;
            this.closeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.closeButton.Location = new System.Drawing.Point(364, 45);
            this.closeButton.Name = "closeButton";
            // 
            // 
            // 
            this.closeButton.RootElement.ToolTipText = null;
            this.closeButton.Size = new System.Drawing.Size(75, 22);
            this.closeButton.TabIndex = 3;
            this.closeButton.Text = "Close";
            this.closeButton.Click += new System.EventHandler(this.radButton5_Click);
            // 
            // resetButton
            // 
            this.resetButton.AllowShowFocusCues = true;
            this.resetButton.BackColor = System.Drawing.Color.Transparent;
            this.resetButton.Location = new System.Drawing.Point(364, 14);
            this.resetButton.Name = "resetButton";
            // 
            // 
            // 
            this.resetButton.RootElement.ToolTipText = null;
            this.resetButton.Size = new System.Drawing.Size(75, 22);
            this.resetButton.TabIndex = 3;
            this.resetButton.Text = "Reset";
            this.resetButton.Click += new System.EventHandler(this.radButton4_Click);
            // 
            // radTitleBar1
            // 
            this.radTitleBar1.BackColor = System.Drawing.Color.Transparent;
            this.radTitleBar1.Text = "ToolStripDialogForm";
            this.radTitleBar1.Location = new System.Drawing.Point(3, 2);
            this.radTitleBar1.Name = "radTitleBar1";
            // 
            // 
            // 
            this.radTitleBar1.RootElement.ToolTipText = null;
            this.radTitleBar1.Size = new System.Drawing.Size(480, 27);
            this.radTitleBar1.TabIndex = 1;
            this.radTitleBar1.TabStop = false;
            this.radTitleBar1.Text = "radTitleBar1";
            // 
            // ToolStripDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(146)))), ((int)(((byte)(189)))));
            this.ClientSize = new System.Drawing.Size(492, 399);
            this.Controls.Add(this.radTitleBar1);
            this.Controls.Add(this.radTabStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximumSize = new System.Drawing.Size(1280, 994);
            this.Name = "ToolStripDialogForm";
            this.Text = "ToolStripDialogForm";
            this.Load += new System.EventHandler(this.ToolStripDialogForm_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.radTabStrip1)).EndInit();
            this.radTabStrip1.ResumeLayout(false);
            this.toolBarsTabItem.ContentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radListBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resetButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radTitleBar1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		public RadTabStrip radTabStrip1;
		private TabItem toolBarsTabItem;
		private RadButton resetButton;
		private RadButton closeButton;
		public RadListBox radListBox1;
		public RadTitleBar radTitleBar1;
	}
}