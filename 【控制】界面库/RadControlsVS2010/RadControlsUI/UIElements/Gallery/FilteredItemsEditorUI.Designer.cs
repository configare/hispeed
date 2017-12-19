using System.Windows.Forms;
namespace Telerik.WinControls.UI
{
    internal partial class FilteredItemsEditorUI
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilteredItemsEditorUI));
			this.AssignedItemsBox = new System.Windows.Forms.ListBox();
			this.AddBtn = new System.Windows.Forms.Button();
			this.RemoveBtn = new System.Windows.Forms.Button();
			this.OkBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.lbItemsInCollection = new System.Windows.Forms.Label();
			this.MoveUpBtn = new System.Windows.Forms.Button();
			this.MoveDownBtn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.AvailableItemsBox = new System.Windows.Forms.ListBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.AvailableItemsBox2 = new System.Windows.Forms.ListBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// AssignedItemsBox
			// 
			this.AssignedItemsBox.IntegralHeight = false;
			this.AssignedItemsBox.Location = new System.Drawing.Point(364, 33);
			this.AssignedItemsBox.Name = "AssignedItemsBox";
			this.AssignedItemsBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.AssignedItemsBox.Size = new System.Drawing.Size(256, 331);
			this.AssignedItemsBox.TabIndex = 2;
			this.AssignedItemsBox.SelectedIndexChanged += new System.EventHandler(this.AssignedItemsBoxSelectedIndexChanged);
			// 
			// AddBtn
			// 
			this.AddBtn.Enabled = false;
			this.AddBtn.Location = new System.Drawing.Point(283, 177);
			this.AddBtn.Name = "AddBtn";
			this.AddBtn.Size = new System.Drawing.Size(75, 23);
			this.AddBtn.TabIndex = 3;
			this.AddBtn.Text = "Add >>";
			this.AddBtn.Click += new System.EventHandler(this.AddAction);
			// 
			// RemoveBtn
			// 
			this.RemoveBtn.Enabled = false;
			this.RemoveBtn.Location = new System.Drawing.Point(283, 217);
			this.RemoveBtn.Name = "RemoveBtn";
			this.RemoveBtn.Size = new System.Drawing.Size(75, 23);
			this.RemoveBtn.TabIndex = 4;
			this.RemoveBtn.Text = "<< Remove";
			this.RemoveBtn.Click += new System.EventHandler(this.RemoveAction);
			// 
			// OkBtn
			// 
			this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkBtn.Location = new System.Drawing.Point(464, 374);
			this.OkBtn.Name = "OkBtn";
			this.OkBtn.Size = new System.Drawing.Size(75, 23);
			this.OkBtn.TabIndex = 5;
			this.OkBtn.Text = "OK";
			// 
			// CancelBtn
			// 
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(545, 374);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(75, 23);
			this.CancelBtn.TabIndex = 6;
			this.CancelBtn.Text = "Cancel";
			// 
			// lbItemsInCollection
			// 
			this.lbItemsInCollection.Location = new System.Drawing.Point(361, 17);
			this.lbItemsInCollection.Name = "lbItemsInCollection";
			this.lbItemsInCollection.Size = new System.Drawing.Size(97, 17);
			this.lbItemsInCollection.TabIndex = 7;
			this.lbItemsInCollection.Text = "Assigned to group:";
			// 
			// MoveUpBtn
			// 
			this.MoveUpBtn.BackColor = System.Drawing.Color.White;
			this.MoveUpBtn.Enabled = false;
			this.MoveUpBtn.Image = ((System.Drawing.Image)(resources.GetObject("MoveUpBtn.Image")));
			this.MoveUpBtn.Location = new System.Drawing.Point(334, 35);
			this.MoveUpBtn.Name = "MoveUpBtn";
			this.MoveUpBtn.Size = new System.Drawing.Size(24, 23);
			this.MoveUpBtn.TabIndex = 8;
			this.MoveUpBtn.UseVisualStyleBackColor = false;
			this.MoveUpBtn.Click += new System.EventHandler(this.MoveUpAction);
			// 
			// MoveDownBtn
			// 
			this.MoveDownBtn.BackColor = System.Drawing.Color.White;
			this.MoveDownBtn.Enabled = false;
			this.MoveDownBtn.Image = ((System.Drawing.Image)(resources.GetObject("MoveDownBtn.Image")));
			this.MoveDownBtn.Location = new System.Drawing.Point(334, 70);
			this.MoveDownBtn.Name = "MoveDownBtn";
			this.MoveDownBtn.Size = new System.Drawing.Size(24, 23);
			this.MoveDownBtn.TabIndex = 8;
			this.MoveDownBtn.UseVisualStyleBackColor = false;
			this.MoveDownBtn.Click += new System.EventHandler(this.MoveDownAction);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 15);
			this.label1.TabIndex = 7;
			this.label1.Text = "Available Items:";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.ItemSize = new System.Drawing.Size(134, 18);
			this.tabControl1.Location = new System.Drawing.Point(5, 35);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(272, 333);
			this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabControl1.TabIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.AvailableItemsBox);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(264, 307);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Unassingned";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// AvailableItemsBox
			// 
			this.AvailableItemsBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AvailableItemsBox.IntegralHeight = false;
			this.AvailableItemsBox.Location = new System.Drawing.Point(3, 3);
			this.AvailableItemsBox.Name = "AvailableItemsBox";
			this.AvailableItemsBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.AvailableItemsBox.Size = new System.Drawing.Size(258, 301);
			this.AvailableItemsBox.TabIndex = 2;
			this.AvailableItemsBox.SelectedIndexChanged += new System.EventHandler(this.AvailableItemsBoxSelectedIndexChanged);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.AvailableItemsBox2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(264, 307);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Assinged to other groups";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// AvailableItemsBox2
			// 
			this.AvailableItemsBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AvailableItemsBox2.IntegralHeight = false;
			this.AvailableItemsBox2.Location = new System.Drawing.Point(3, 3);
			this.AvailableItemsBox2.Name = "AvailableItemsBox2";
			this.AvailableItemsBox2.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.AvailableItemsBox2.Size = new System.Drawing.Size(258, 301);
			this.AvailableItemsBox2.TabIndex = 11;
			this.AvailableItemsBox2.SelectedIndexChanged += new System.EventHandler(this.AvailableItemsBoxSelectedIndexChanged);
			// 
			// FilteredItemsEditorUI
			// 
			this.AcceptButton = this.OkBtn;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.ClientSize = new System.Drawing.Size(628, 404);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.MoveUpBtn);
			this.Controls.Add(this.lbItemsInCollection);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.OkBtn);
			this.Controls.Add(this.RemoveBtn);
			this.Controls.Add(this.AddBtn);
			this.Controls.Add(this.AssignedItemsBox);
			this.Controls.Add(this.MoveDownBtn);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FilteredItemsEditorUI";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Items Collection Editor";
			this.Load += new System.EventHandler(this.EditorForm_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private Button AddBtn;
		private Button CancelBtn;
		private Button MoveDownBtn;
		private Button MoveUpBtn;
		private Button OkBtn;
		private Button RemoveBtn;
        //private RadItemCollection collection;
		private Label label1;
		private Label lbItemsInCollection;
		private ListBox AssignedItemsBox;

		#endregion
		private TabControl tabControl1;
		private TabPage tabPage1;
		private ListBox AvailableItemsBox;
		private TabPage tabPage2;
		private ListBox AvailableItemsBox2;

	}
}