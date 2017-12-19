using Telerik.WinControls.UI;
namespace Telerik.WinControls.UI
{
	partial class radNavigationPaneOptions
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
            this.label1 = new System.Windows.Forms.Label();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.radButton4 = new Telerik.WinControls.UI.RadButton();
            this.radButton5 = new Telerik.WinControls.UI.RadButton();
            this.radButton3 = new Telerik.WinControls.UI.RadButton();
            this.radButton2 = new Telerik.WinControls.UI.RadButton();
            this.radButton1 = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radButton4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Display button in this order";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 37);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(200, 94);
            this.checkedListBox1.TabIndex = 7;
            // 
            // radButton4
            // 
            this.radButton4.Location = new System.Drawing.Point(222, 146);
            this.radButton4.Name = "radButton4";
            // 
            // 
            // 
            this.radButton4.RootElement.ToolTipText = null;
            this.radButton4.Size = new System.Drawing.Size(73, 24);
            this.radButton4.TabIndex = 5;
            this.radButton4.Text = "Cancel";
            this.radButton4.Click += new System.EventHandler(this.radButton4_Click);
            // 
            // radButton5
            // 
            this.radButton5.Location = new System.Drawing.Point(143, 146);
            this.radButton5.Name = "radButton5";
            // 
            // 
            // 
            this.radButton5.RootElement.ToolTipText = null;
            this.radButton5.Size = new System.Drawing.Size(73, 24);
            this.radButton5.TabIndex = 4;
            this.radButton5.Text = "Ok";
            this.radButton5.Click += new System.EventHandler(this.radButton5_Click);
            // 
            // radButton3
            // 
            this.radButton3.Location = new System.Drawing.Point(222, 104);
            this.radButton3.Name = "radButton3";
            // 
            // 
            // 
            this.radButton3.RootElement.ToolTipText = null;
            this.radButton3.Size = new System.Drawing.Size(73, 24);
            this.radButton3.TabIndex = 3;
            this.radButton3.Text = "Reset";
            this.radButton3.Click += new System.EventHandler(this.radButton3_Click);
            // 
            // radButton2
            // 
            this.radButton2.Location = new System.Drawing.Point(222, 72);
            this.radButton2.Name = "radButton2";
            // 
            // 
            // 
            this.radButton2.RootElement.ToolTipText = null;
            this.radButton2.Size = new System.Drawing.Size(73, 24);
            this.radButton2.TabIndex = 2;
            this.radButton2.Text = "MoveDown";
            this.radButton2.Click += new System.EventHandler(this.radButton2_Click);
            // 
            // radButton1
            // 
            this.radButton1.Location = new System.Drawing.Point(222, 37);
            this.radButton1.Name = "radButton1";
            // 
            // 
            // 
            this.radButton1.RootElement.ToolTipText = null;
            this.radButton1.Size = new System.Drawing.Size(73, 24);
            this.radButton1.TabIndex = 1;
            this.radButton1.Text = "MoveUp";
            this.radButton1.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radNavigationPaneOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 174);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radButton4);
            this.Controls.Add(this.radButton5);
            this.Controls.Add(this.radButton3);
            this.Controls.Add(this.radButton2);
            this.Controls.Add(this.radButton1);
            this.Name = "radNavigationPaneOptions";
            this.Text = "radNavigationPaneOptions";
            this.Load += new System.EventHandler(this.radNavigationPaneOptions_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.radButton4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private RadButton radButton1;
		private RadButton radButton2;
		private RadButton radButton3;
		private RadButton radButton4;
		private RadButton radButton5;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckedListBox checkedListBox1;
	}
}