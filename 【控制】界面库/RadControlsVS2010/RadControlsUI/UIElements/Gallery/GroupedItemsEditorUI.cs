using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Collections;

namespace Telerik.WinControls.UI
{
	internal partial class GroupedItemsEditorUI : Form
	{
		public GroupedItemsEditorUI()
		{
			InitializeComponent();
		}

		private RadGalleryElement defaultElement;
		private RadGalleryGroupItem defaultGroup;
		private RadItemOwnerCollection originalValue;
		private RadItemOwnerCollection currentValue;
		private IWindowsFormsEditorService edSvc;
		private IDesignerHost host;
		private ITypeDiscoveryService typeSvc;

		public RadItemOwnerCollection Value
		{
			get
			{
		        return this.currentValue;
			}
		}

		public void Start(IWindowsFormsEditorService edSvc, ITypeDiscoveryService typeSvc, IDesignerHost host, 
			RadItemOwnerCollection collection, RadGalleryGroupItem group, RadGalleryElement owner)
		{
			this.host = host;
			this.edSvc = edSvc;
			this.typeSvc = typeSvc;
			this.currentValue = collection;
			this.originalValue = collection;
			this.defaultGroup = group;
			this.defaultElement = owner;
		}

		public void End()
		{
			this.host = null;
			this.edSvc = null;
			this.typeSvc = null;
			this.defaultGroup = null;
			this.defaultElement = null;
			this.originalValue = null;
			this.Reset();
		}

		public void Reset()
		{
			this.currentValue = null;
		}

		private void AvailableItemsBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			this.SetButtonsEnabledState();
		}

		private void AssignedItemsBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			this.SetButtonsEnabledState();
		}

		private static void RemoveItemsContainedInGroup(ArrayList items, RadItemOwnerCollection groupItems)
		{
			for (int num1 = 0; num1 < groupItems.Count; num1++)
			{
				RadGalleryItem item1 = groupItems[num1] as RadGalleryItem;
				int num2 = items.IndexOf(item1);
				if (num2 >= 0)
				{
					items.RemoveAt(num2);
					num1--;
				}
			}
		}

		private void PopulateAvailableItems()
		{
			ArrayList list1 = new ArrayList();
			if (this.FilterTypeBox.SelectedIndex == 0)
			{
				//load all items
				foreach (RadGalleryItem item1 in defaultElement.Items)
				{
                    if( !(bool)item1.GetValue(RadItem.IsAddNewItemProperty))
					    list1.Add(item1);
				}
				foreach (RadGalleryGroupItem group1 in defaultElement.Groups)
				{
					GroupedItemsEditorUI.RemoveItemsContainedInGroup(list1, group1.Items);
				}
				for (int i = 0; i < list1.Count; i++)
				{
					list1[i] = new InstanceItem((list1[i] as RadGalleryItem), defaultElement);
				}
			}
			else if (this.FilterTypeBox.SelectedIndex == 1)
			{
				foreach (RadGalleryGroupItem group1 in defaultElement.Groups)
				{
					if (group1 != defaultGroup)
					{
						//load all items
						foreach (RadGalleryItem item1 in group1.Items)
						{
							list1.Add(new InstanceItem(item1, defaultElement));
						}
					}
				}
			}

			// set the items box
			this.AvailableItemsBox.BeginUpdate();
			this.AvailableItemsBox.Items.Clear();
			this.AvailableItemsBox.Items.AddRange(list1.ToArray());
			this.AvailableItemsBox.EndUpdate();
			this.SetButtonsEnabledState();
		}

		private void PopulateAssignedItems()
		{
			this.AssignedItemsBox.BeginUpdate();
			this.AssignedItemsBox.Items.Clear();
			foreach (RadGalleryItem item1 in this.defaultGroup.Items)
			{
				this.AssignedItemsBox.Items.Add(new InstanceItem(item1, defaultElement));
			}
			this.AssignedItemsBox.EndUpdate();
			this.SetButtonsEnabledState();
		}

		private void SetButtonsEnabledState()
		{
			this.AddBtn.Enabled = this.AvailableItemsBox.SelectedItems.Count > 0;
			if (this.AssignedItemsBox.SelectedItems.Count > 0)
			{
				this.RemoveBtn.Enabled = true;
				this.MoveUpBtn.Enabled = this.AssignedItemsBox.SelectedIndices[0] != 0;
				this.MoveDownBtn.Enabled = this.AssignedItemsBox.SelectedIndices[this.AssignedItemsBox.SelectedIndices.Count - 1] != (this.AssignedItemsBox.Items.Count - 1);
			}
			else
			{
				this.RemoveBtn.Enabled = this.MoveDownBtn.Enabled = this.MoveUpBtn.Enabled = false;
			}
		}

		private void AddAction(object sender, EventArgs e)
		{
			foreach (InstanceItem item1 in this.AvailableItemsBox.SelectedItems)
			{
				if (this.FilterTypeBox.SelectedIndex == 1)
				{
					foreach (RadGalleryGroupItem group1 in defaultElement.Groups)
					{
						if (group1 != defaultGroup)
						{
							//load all items
							foreach (RadGalleryItem item2 in group1.Items)
							{
								if (item1.Instance == item2)
								{
									group1.Items.Remove(item2);
									break;
								}
							}
						}
					}					
				}
				this.defaultGroup.Items.Add(item1.Instance);
			}
			int num1 = this.AvailableItemsBox.SelectedIndices[0];
			this.PopulateAvailableItems();
			this.PopulateAssignedItems();
			if (this.AvailableItemsBox.Items.Count > 0)
			{
				this.AvailableItemsBox.SelectedIndex = Math.Min(this.AvailableItemsBox.Items.Count - 1, num1);
			}
			this.SetButtonsEnabledState();
		}

		private void RemoveAction(object sender, EventArgs e)
		{
			foreach (InstanceItem item1 in this.AssignedItemsBox.SelectedItems)
			{
				this.defaultGroup.Items.Remove(item1.Instance);
			}
			int num1 = this.AssignedItemsBox.SelectedIndices[0];
			this.PopulateAvailableItems();
			this.PopulateAssignedItems();
			if (this.AssignedItemsBox.Items.Count > 0)
			{
				this.AssignedItemsBox.SelectedIndex = Math.Min(this.AssignedItemsBox.Items.Count - 1, num1);
			}
			this.SetButtonsEnabledState();
		}

		private void MoveDownAction(object sender, EventArgs e)
		{
			int[] numArray1 = new int[this.AssignedItemsBox.SelectedItems.Count];

			for (int num1 = this.AssignedItemsBox.SelectedItems.Count - 1; num1 >= 0; num1--)
			{
				RadGalleryItem item1 = ((InstanceItem)this.AssignedItemsBox.SelectedItems[num1]).Instance;
				int num2 = this.defaultGroup.Items.IndexOf(item1);
				numArray1[num1] = num2 + 1;
				this.defaultGroup.Items.RemoveAt(num2);
				this.defaultGroup.Items.Insert(num2 + 1, item1);
			}

			this.AssignedItemsBox.BeginUpdate();
			this.PopulateAssignedItems();
			foreach (int num3 in numArray1)
			{
				this.AssignedItemsBox.SetSelected(num3, true);
			}
			this.AssignedItemsBox.EndUpdate();
			this.SetButtonsEnabledState();
		}

		private void MoveUpAction(object sender, EventArgs e)
		{
			int[] numArray1 = new int[this.AssignedItemsBox.SelectedItems.Count];

			for (int num1 = 0; num1 < this.AssignedItemsBox.SelectedItems.Count; num1++)
			{
				RadGalleryItem item1 = ((InstanceItem)this.AssignedItemsBox.SelectedItems[num1]).Instance;
				int num2 = this.defaultGroup.Items.IndexOf(item1);
				numArray1[num1] = num2 - 1;
				this.defaultGroup.Items.RemoveAt(num2);
				this.defaultGroup.Items.Insert(num2 - 1, item1);
			}

			this.AssignedItemsBox.BeginUpdate();
			this.PopulateAssignedItems();
			foreach (int num3 in numArray1)
			{
				this.AssignedItemsBox.SetSelected(num3, true);
			}
			this.AssignedItemsBox.EndUpdate();
			this.SetButtonsEnabledState();
		}

		private void FilterBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			this.PopulateAvailableItems();
			this.SetButtonsEnabledState();
		}

		private void EditorForm_Load(object sender, EventArgs e)
		{
			this.FilterTypeBox.SelectedIndex = 0;
			this.PopulateAssignedItems();
			if (this.AvailableItemsBox.Items.Count > 0)
			{
				this.AvailableItemsBox.SelectedIndex = 0;
			}
		}

		protected class InstanceItem
		{
			public InstanceItem()
			{
			}

			public InstanceItem(RadGalleryItem instance, RadGalleryElement owner)
			{
				this.instance = instance;
				this.owner = owner;
			}

			// Fields
			private RadGalleryItem instance;

			public RadGalleryItem Instance
			{
				get
				{
					return instance;
				}
				set
				{
					instance = value;
				}
			}

			private RadGalleryElement owner;

			public RadGalleryElement Owner
			{
				get
				{
					return owner;
				}
				set
				{
					owner = value;
				}
			}

			public override string ToString()
			{
				string group = string.Empty;
				foreach (RadGalleryGroupItem group1 in this.owner.Groups)
				{
					foreach (RadGalleryItem item1 in group1.Items)
					{
						if (instance == item1)
						{
							group = " (" + group1.ToString() + ")";
						}
					}
				}
				return instance.ToString() + group;
			}
		}
	}
}