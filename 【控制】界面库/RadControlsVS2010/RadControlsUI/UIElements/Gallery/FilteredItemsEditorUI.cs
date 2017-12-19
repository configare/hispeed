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
	internal partial class FilteredItemsEditorUI : Form
	{
		public FilteredItemsEditorUI()
		{
			InitializeComponent();
		}

		private RadGalleryElement defaultElement;
		private RadGalleryGroupFilter defaultFilter;
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
			RadItemOwnerCollection collection, RadGalleryGroupFilter filter, RadGalleryElement owner)
		{
			this.host = host;
			this.edSvc = edSvc;
			this.typeSvc = typeSvc;
			this.currentValue = collection;
			this.originalValue = collection;
			this.defaultFilter = filter;
			this.defaultElement = owner;
		}

		public void End()
		{
			this.host = null;
			this.edSvc = null;
			this.typeSvc = null;
			this.defaultFilter = null;
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

		private static void RemoveItemsContainedInFilter(ArrayList items, RadItemOwnerCollection groupItems)
		{
			for (int num1 = 0; num1 < groupItems.Count; num1++)
			{
				RadGalleryGroupItem item1 = groupItems[num1] as RadGalleryGroupItem;
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

			//load unassingned items
			foreach (RadGalleryGroupItem item1 in defaultElement.Groups)
			{
				list1.Add(item1);
			}
			foreach (RadGalleryGroupFilter filter1 in defaultElement.Filters)
			{
				FilteredItemsEditorUI.RemoveItemsContainedInFilter(list1, filter1.Items);
			}
			for (int i = 0; i < list1.Count; i++)
			{
				list1[i] = new InstanceItem((list1[i] as RadGalleryGroupItem), null, defaultElement);
			}

			//load assingned to other groups items
			ArrayList list2 = new ArrayList();
			foreach (RadGalleryGroupFilter filter1 in defaultElement.Filters)
			{
				if (filter1 != defaultFilter)
				{					
					foreach (RadGalleryGroupItem item1 in filter1.Items)
					{
						list2.Add(new InstanceItem(item1, filter1, defaultElement));
					}
				}				
			}			

			// set the unassigned items box
			this.AvailableItemsBox.BeginUpdate();
			this.AvailableItemsBox.Items.Clear();
			this.AvailableItemsBox.Items.AddRange(list1.ToArray());
			this.AvailableItemsBox.EndUpdate();

			// set the assigned to other groups items box
			this.AvailableItemsBox2.BeginUpdate();
			this.AvailableItemsBox2.Items.Clear();
			this.AvailableItemsBox2.Items.AddRange(list2.ToArray());
			this.AvailableItemsBox2.EndUpdate();

			this.SetButtonsEnabledState();
		}

		private void PopulateAssignedItems()
		{
			this.AssignedItemsBox.BeginUpdate();
			this.AssignedItemsBox.Items.Clear();
			foreach (RadGalleryGroupItem item1 in this.defaultFilter.Items)
			{
				this.AssignedItemsBox.Items.Add(new InstanceItem(item1, defaultFilter, defaultElement));
			}
			this.AssignedItemsBox.EndUpdate();
			this.SetButtonsEnabledState();
		}

		private void SetButtonsEnabledState()
		{
			if (tabControl1.SelectedIndex == 0)
				this.AddBtn.Enabled = this.AvailableItemsBox.SelectedItems.Count > 0;
			else
				this.AddBtn.Enabled = this.AvailableItemsBox2.SelectedItems.Count > 0;
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
			int num1 = 0;

			if (tabControl1.SelectedIndex == 0)
			{
				foreach (InstanceItem item1 in this.AvailableItemsBox.SelectedItems)
				{
					item1.Filter = this.defaultFilter;
					this.defaultFilter.Items.Add(item1.Instance);
				}
				num1 = this.AvailableItemsBox.SelectedIndices[0];
			}
			else
			{
				foreach (InstanceItem item1 in this.AvailableItemsBox2.SelectedItems)
				{				
					item1.Filter = this.defaultFilter;
					foreach (InstanceItem item2 in this.AssignedItemsBox.Items)
					{
						if (item1.Instance.Equals(item2.Instance))
						{
							MessageBox.Show("This item is already added");
							return;
						}
					}
					this.defaultFilter.Items.Add(item1.Instance);
				}
				num1 = this.AvailableItemsBox2.SelectedIndices[0];
			}
			
			this.PopulateAvailableItems();
			this.PopulateAssignedItems();
			if (tabControl1.SelectedIndex == 0)
			{
				if (this.AvailableItemsBox.Items.Count > 0)
				{
					this.AvailableItemsBox.SelectedIndex = Math.Min(this.AvailableItemsBox.Items.Count - 1, num1);
				}
			}
			else
			{
				if (this.AvailableItemsBox.Items.Count > 0)
				{
					this.AvailableItemsBox2.SelectedIndex = Math.Min(this.AvailableItemsBox2.Items.Count - 1, num1);
				}
			}
			this.SetButtonsEnabledState();
		}

		private void RemoveAction(object sender, EventArgs e)
		{
			foreach (InstanceItem item1 in this.AssignedItemsBox.SelectedItems)
			{
				item1.Filter = null;
				this.defaultFilter.Items.Remove(item1.Instance);
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
				RadGalleryGroupItem item1 = ((InstanceItem)this.AssignedItemsBox.SelectedItems[num1]).Instance;
				int num2 = this.defaultFilter.Items.IndexOf(item1);
				numArray1[num1] = num2 + 1;
				this.defaultFilter.Items.RemoveAt(num2);
				this.defaultFilter.Items.Insert(num2 + 1, item1);
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
				RadGalleryGroupItem item1 = ((InstanceItem)this.AssignedItemsBox.SelectedItems[num1]).Instance;
				int num2 = this.defaultFilter.Items.IndexOf(item1);
				numArray1[num1] = num2 - 1;
				this.defaultFilter.Items.RemoveAt(num2);
				this.defaultFilter.Items.Insert(num2 - 1, item1);
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
			this.PopulateAvailableItems();			
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

			public InstanceItem(RadGalleryGroupItem instance, RadGalleryGroupFilter filter, RadGalleryElement owner)
			{
				this.instance = instance;
				this.filter = filter;
				this.owner = owner;
			}

			// Fields
			private RadGalleryGroupItem instance;

			public RadGalleryGroupItem Instance
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

			private RadGalleryGroupFilter filter;

			public RadGalleryGroupFilter Filter
			{
				get
				{
					return filter;
				}
				set
				{
					filter = value;
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
				if (filter != null)
					return (instance.ToString() + " (" + filter.ToString() + ")");
				else
					return instance.ToString();
			}
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.SetButtonsEnabledState();
		}
	}
}