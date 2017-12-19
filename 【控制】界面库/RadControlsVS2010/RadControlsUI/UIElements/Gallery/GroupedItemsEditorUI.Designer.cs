using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System;
namespace Telerik.WinControls.UI
{
    internal partial class GroupedItemsEditorUI
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupedItemsEditorUI));
			this.FilterTypeBox = new System.Windows.Forms.ComboBox();
			this.AvailableItemsBox = new System.Windows.Forms.ListBox();
			this.AssignedItemsBox = new System.Windows.Forms.ListBox();
			this.AddBtn = new System.Windows.Forms.Button();
			this.RemoveBtn = new System.Windows.Forms.Button();
			this.OkBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.lbItemsInCollection = new System.Windows.Forms.Label();
			this.MoveUpBtn = new System.Windows.Forms.Button();
			this.MoveDownBtn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// FilterTypeBox
			// 
			this.FilterTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.FilterTypeBox.Items.AddRange(new object[] {
            "Unassigned Items",
            "Items assigned to different groups"});
			this.FilterTypeBox.Location = new System.Drawing.Point(6, 37);
			this.FilterTypeBox.Name = "FilterTypeBox";
			this.FilterTypeBox.Size = new System.Drawing.Size(248, 21);
			this.FilterTypeBox.TabIndex = 0;
			this.FilterTypeBox.SelectedIndexChanged += new System.EventHandler(this.FilterBoxSelectedIndexChanged);
			// 
			// AvailableItemsBox
			// 
			this.AvailableItemsBox.IntegralHeight = false;
			this.AvailableItemsBox.Location = new System.Drawing.Point(6, 96);
			this.AvailableItemsBox.Name = "AvailableItemsBox";
			this.AvailableItemsBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.AvailableItemsBox.Size = new System.Drawing.Size(248, 272);
			this.AvailableItemsBox.TabIndex = 1;
			this.AvailableItemsBox.SelectedIndexChanged += new System.EventHandler(this.AvailableItemsBoxSelectedIndexChanged);
			// 
			// AssignedItemsBox
			// 
			this.AssignedItemsBox.IntegralHeight = false;
			this.AssignedItemsBox.Location = new System.Drawing.Point(350, 37);
			this.AssignedItemsBox.Name = "AssignedItemsBox";
			this.AssignedItemsBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.AssignedItemsBox.Size = new System.Drawing.Size(256, 331);
			this.AssignedItemsBox.TabIndex = 2;
			this.AssignedItemsBox.SelectedIndexChanged += new System.EventHandler(this.AssignedItemsBoxSelectedIndexChanged);
			// 
			// AddBtn
			// 
			this.AddBtn.Enabled = false;
			this.AddBtn.Location = new System.Drawing.Point(262, 177);
			this.AddBtn.Name = "AddBtn";
			this.AddBtn.Size = new System.Drawing.Size(75, 23);
			this.AddBtn.TabIndex = 3;
			this.AddBtn.Text = "Add >>";
			this.AddBtn.Click += new System.EventHandler(this.AddAction);
			// 
			// RemoveBtn
			// 
			this.RemoveBtn.Enabled = false;
			this.RemoveBtn.Location = new System.Drawing.Point(262, 217);
			this.RemoveBtn.Name = "RemoveBtn";
			this.RemoveBtn.Size = new System.Drawing.Size(75, 23);
			this.RemoveBtn.TabIndex = 4;
			this.RemoveBtn.Text = "<< Remove";
			this.RemoveBtn.Click += new System.EventHandler(this.RemoveAction);
			// 
			// OkBtn
			// 
			this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkBtn.Location = new System.Drawing.Point(446, 377);
			this.OkBtn.Name = "OkBtn";
			this.OkBtn.Size = new System.Drawing.Size(75, 23);
			this.OkBtn.TabIndex = 5;
			this.OkBtn.Text = "OK";
			// 
			// CancelBtn
			// 
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(531, 377);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(75, 23);
			this.CancelBtn.TabIndex = 6;
			this.CancelBtn.Text = "Cancel";
			// 
			// lbItemsInCollection
			// 
			this.lbItemsInCollection.Location = new System.Drawing.Point(347, 18);
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
			this.MoveUpBtn.Location = new System.Drawing.Point(320, 38);
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
			this.MoveDownBtn.Location = new System.Drawing.Point(320, 70);
			this.MoveDownBtn.Name = "MoveDownBtn";
			this.MoveDownBtn.Size = new System.Drawing.Size(24, 23);
			this.MoveDownBtn.TabIndex = 8;
			this.MoveDownBtn.UseVisualStyleBackColor = false;
			this.MoveDownBtn.Click += new System.EventHandler(this.MoveDownAction);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 78);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 15);
			this.label1.TabIndex = 7;
			this.label1.Text = "Available Items:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 16);
			this.label2.TabIndex = 9;
			this.label2.Text = "Filter by:";
			// 
			// GroupedItemsEditorUI
			// 
			this.AcceptButton = this.OkBtn;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.ClientSize = new System.Drawing.Size(618, 404);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.MoveUpBtn);
			this.Controls.Add(this.lbItemsInCollection);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.OkBtn);
			this.Controls.Add(this.RemoveBtn);
			this.Controls.Add(this.AddBtn);
			this.Controls.Add(this.AssignedItemsBox);
			this.Controls.Add(this.AvailableItemsBox);
			this.Controls.Add(this.FilterTypeBox);
			this.Controls.Add(this.MoveDownBtn);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GroupedItemsEditorUI";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Items Collection Editor";
			this.Load += new System.EventHandler(this.EditorForm_Load);
			this.ResumeLayout(false);

		}


		private Button AddBtn;
		private Button CancelBtn;
		private Button MoveDownBtn;
		private Button MoveUpBtn;
		private Button OkBtn;
		private Button RemoveBtn;
		private System.Windows.Forms.ComboBox FilterTypeBox;
        //private RadItemCollection collection;
		private Label label1;
		private ListBox AvailableItemsBox;
		private Label lbItemsInCollection;
		private ListBox AssignedItemsBox;
		private Label label2;

		#endregion
	}
}