using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Telerik.WinControls;

namespace Telerik.WinControls.UI
{
	public partial class radNavigationPaneOptions : Form
	{
		private RadPanelBarElement panelBarElement;

		public radNavigationPaneOptions(RadPanelBarElement panelBarElement)
		{
			InitializeComponent();
			this.BackColor = Color.FromArgb(242, 242, 242);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.ShowInTaskbar = false;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.Text = "Navigation Panel Options";
			this.panelBarElement = panelBarElement;
		}

        internal void SetButtonsTheme(string themeName)
        {
            this.radButton4.ThemeName = themeName;
            this.radButton5.ThemeName = themeName;
            this.radButton3.ThemeName = themeName;
            this.radButton2.ThemeName = themeName;
            this.radButton1.ThemeName = themeName;
        }

		private void radNavigationPaneOptions_Load(object sender, EventArgs e)
		{
	
		}

		private void radNavigationPaneOptions_Load_1(object sender, EventArgs e)
		{
			if (this.panelBarElement != null)
			{
				List<int> list = new List<int>();

				this.checkedListBox1.Items.Clear();

				int i = 0;
				foreach (RadPanelBarGroupElement group in this.panelBarElement.Items)
				{
					this.checkedListBox1.Items.Add(group.Caption);
					this.checkedListBox1.SetItemChecked(i, true);
					i++;
				}

				foreach (RadPanelBarGroupElement group in this.panelBarElement.hiddenGroupsList)
				{
					this.checkedListBox1.Items.Add(group.Caption);
					this.checkedListBox1.SetItemChecked(i, false);
					i++;
					
				}
			}
		}

		private void Reset()
		{
			for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
			{
				this.checkedListBox1.SetItemChecked(i, true);
			
			}
	//		this.panelBarElement.ShowFewerButtonsMenuItem.Enabled = true;
	//		this.panelBarElement.ShowMoreButtonsMenuItem.Enabled = false;
	//		this.panelBarElement.panelBarOverFlow.PerformLayout(this.panelBarElement);
		}

		private RadPanelBarGroupElement GetGroupByCaption(object item)
		{
			string s = (string)item;
		
			foreach (RadPanelBarGroupElement group in this.panelBarElement.Items)
			{
				if (group.Caption.Equals(s))
				{
					
					if (this.visibleItems.Contains(group) || this.hiddenItems.Contains(group))
						continue;

					return group;
				}
			}

			foreach (RadPanelBarGroupElement group in this.panelBarElement.hiddenGroupsList)
			{
				if (group.Caption.Equals(s))
				{
					if (this.visibleItems.Contains(group) || this.hiddenItems.Contains(group))
						continue;

					return group;
				}
			}

			return null;
		}

		private void Save()
		{
			RearrangeGroupCollection();	
		}

		private int GetCollapsedItemsCount()
		{
			int i = 0;
			foreach (RadPanelBarGroupElement group in this.panelBarElement.Items)
			{
				if (group.Visibility == ElementVisibility.Collapsed)
					i++;
			}

			return i;
		}

        private List<RadPanelBarGroupElement> visibleItems = new List<RadPanelBarGroupElement>();
        private List<RadPanelBarGroupElement> hiddenItems = new List<RadPanelBarGroupElement>();
	
		private void RearrangeGroupCollection()
		{          
			this.panelBarElement.SuspendLayout();
			this.visibleItems.Clear();
			this.hiddenItems.Clear();

			for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
			{
				RadPanelBarGroupElement group = GetGroupByCaption(this.checkedListBox1.Items[i]);
				
				  CheckState isChecked = this.checkedListBox1.GetItemCheckState(i);
				  if (isChecked == CheckState.Checked)
				  {
					  visibleItems.Add(group);
				  }
				  else
				  {
					  hiddenItems.Add(group);
				  }
			}

			this.panelBarElement.Items.Clear();
			this.panelBarElement.hiddenGroupsList.Clear();
		
			foreach (RadPanelBarGroupElement group in visibleItems)
			{
                group.Items.Owner = group.verticalGroupLayout;
                this.panelBarElement.Items.Add(group);
    
				group.ShowCaptionButton(false);
			}

			foreach (RadPanelBarGroupElement group in hiddenItems)
			{
                group.Items.Owner = group.verticalGroupLayout;
                group.Selected = false;
				this.panelBarElement.hiddenGroupsList.Add(group);
				group.ShowCaptionButton(false);
			}

			(this.panelBarElement.CurrentStyle as OutlookStyle).collapsingSteps.Clear();
			this.panelBarElement.SetStyle();
			this.panelBarElement.ResumeLayout(true);

			
		}

		private void radButton4_Click(object sender, EventArgs e)
		{
			this.Hide();
            
		}


		private void radButton5_Click(object sender, EventArgs e)
		{
			this.Save();
			this.Hide();
		}

		private void radButton2_Click(object sender, EventArgs e)
		{
			if (this.checkedListBox1.SelectedIndex == -1)
				return;

			if (this.checkedListBox1.SelectedIndex < this.checkedListBox1.Items.Count - 1)
			{
				
				int i = this.checkedListBox1.SelectedIndex;
				CheckState isChecked = this.checkedListBox1.GetItemCheckState(i);
				object obj = this.checkedListBox1.SelectedItem;
				
				this.checkedListBox1.Items.RemoveAt(i);
				this.checkedListBox1.Items.Insert(i + 1, obj);
				if (isChecked == CheckState.Checked)
					this.checkedListBox1.SetItemChecked(i + 1, true);
				else
					this.checkedListBox1.SetItemChecked(i + 1, false);
				this.checkedListBox1.SelectedIndex = i + 1;
			}

		}

		private void radButton1_Click(object sender, EventArgs e)
		{
			if (this.checkedListBox1.SelectedIndex == -1)
				return;

			if (this.checkedListBox1.SelectedIndex > 0)
			{
				int i = this.checkedListBox1.SelectedIndex;
				object obj = this.checkedListBox1.SelectedItem;
				CheckState isChecked = this.checkedListBox1.GetItemCheckState(i);
				
				this.checkedListBox1.Items.RemoveAt(i);
				this.checkedListBox1.Items.Insert(i - 1, obj);
				if ( isChecked == CheckState.Checked )
					this.checkedListBox1.SetItemChecked(i - 1, true);	
				else
					this.checkedListBox1.SetItemChecked(i - 1, false);	
		
				this.checkedListBox1.SelectedIndex = i - 1;	
			}
		}


		private string closeButtonText;
		public string CloseButtonText
		{
			get
			{
				return this.closeButtonText;
			}
			set
			{
				this.closeButtonText = value;
				this.radButton4.Text = value;
			}
		}

		private string okButtonText;
		public string OkButtonText
		{
			get
			{
				return this.okButtonText;
			}
			set
			{
				this.okButtonText = value;
				this.radButton5.Text = value;
			}
		}

		private string downButtonText;
		public string DownButtonText
		{
			get
			{
				return this.downButtonText;
			}
			set
			{
				this.downButtonText = value;
				this.radButton2.Text = value;
			}
		}

		private string upButtonText;
		public string UpButtonText
		{
			get
			{
				return this.upButtonText;
			}
			set
			{
				this.upButtonText = value;
				this.radButton1.Text = value;
			}
		}

		private string resetButtonText;
		public string ResetButtonText
		{
			get
			{
				return this.resetButtonText;
			}
			set
			{
				this.resetButtonText = value;
				this.radButton3.Text = value;
			}
		}

		private void radButton3_Click(object sender, EventArgs e)
		{
			this.Reset();
		}

	}
}