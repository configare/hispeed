using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using Telerik.WinControls.Layouts;
using System.IO;
using Telerik.WinControls.Enumerations;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents the tool strip dialog form
	/// </summary>
	public partial class ToolStripDialogForm : ShapedForm
	{
		private RadToolStripManager manager;
		private bool load;

		public ToolStripDialogForm(RadToolStripManager manager)
		{
			InitializeComponent();
			this.StartPosition = FormStartPosition.CenterScreen;
			this.ShowInTaskbar = false;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.manager = manager;
			this.Shape = new RoundRectShape(5);
			this.radTitleBar1.RootElement.Shape = new RoundRectShape(5);
			this.radTitleBar1.RootElement.ApplyShapeToControl = true;
			
			this.BorderWidth = 1;
			this.Load += new EventHandler(ToolStripDialogForm_Load);
		}

		/// <summary>
		/// Gets or sets the text in the Reset button
		/// </summary>
		[Browsable(false)]
		public string ResetButtonText
		{
			get
			{
				return this.resetButton.Text;
			}

			set
			{
				this.resetButton.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the text in the OK button
		/// </summary>
		[Browsable(false)]
		public string OkButtonText
		{
			get
			{
				return this.closeButton.Text;
			}
			set
			{
				this.closeButton.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the tab page text
		/// </summary>
		[Browsable(false)]
        [Obsolete]
		public string TabPageText
		{
			get
			{
				return this.toolBarsTabItem.Text;
			}

			set
			{
				this.toolBarsTabItem.Text = value;
			}
		}

		private void ToolStripDialogForm_Load(object sender, EventArgs e)
		{
			this.Size = new Size(492, 392);
			if ( this.manager.RootElement != null )
			if (this.manager.RootElement.ElementTree != null)
			{

                this.radTabStrip1.ThemeName = this.manager.RootElement.ElementTree.ComponentTreeHandler.ThemeName;
                this.resetButton.ThemeName = this.manager.RootElement.ElementTree.ComponentTreeHandler.ThemeName;
                this.closeButton.ThemeName = this.manager.RootElement.ElementTree.ComponentTreeHandler.ThemeName;
                this.radListBox1.ThemeName = this.manager.RootElement.ElementTree.ComponentTreeHandler.ThemeName;
                this.radTitleBar1.ThemeName = this.manager.RootElement.ElementTree.ComponentTreeHandler.ThemeName;
				
				this.BackColor = Color.FromArgb(191, 219, 254);
				this.BorderColor = Color.FromArgb(150, 185, 240);

                if (this.manager.RootElement.ElementTree.ComponentTreeHandler.ThemeName == "Office2007Black")
				{
					this.BackColor = Color.FromArgb(81, 81, 81);

					this.BorderColor = Color.Black;
				}
                if (this.manager.RootElement.ElementTree.ComponentTreeHandler.ThemeName == "Office2007Silver")
				{
					this.BackColor = Color.FromArgb(208, 212, 221); 
					this.BorderColor = Color.FromArgb(227, 232, 240 );
				}
                if (this.manager.RootElement.ElementTree.ComponentTreeHandler.ThemeName == "Telerik")
				{
					this.BackColor = Color.FromArgb(242,242,242);
					this.BorderColor = Color.FromArgb( 252, 255, 238 );
				}
			}

		
		    foreach (RadElement element in this.radTitleBar1.RootElement.Children[0].Children)
		    {
			    if (element as StripLayoutPanel != null)
			    {
				    foreach (RadElement stack in element.Children)
				    {
					    if (stack as StackLayoutPanel != null)
					    {
						    if (stack.Children.Count < 4)
						    {
							    RadImageButtonElement imageButton = new RadImageButtonElement();
							    imageButton.Image = Properties.Resources.ribbon_close;
							    imageButton.Click += new EventHandler(imageButton_Click);
							    stack.Children.Add(imageButton);

						    }
					    }
				    }
			    }
		    }
		}

		private void imageButton_Click(object sender, EventArgs e)
		{
			this.radTitleBar1.TitleBarElement.CloseButton.PerformClick();
		}

		

		internal bool Loaded()
		{
			return this.load;	
		}

		public void CleadDataFromPanel()
		{
			this.radListBox1.Items.Clear();
		}

		private void AddCheckToListBox(RadToolStripItem item, FloatingForm form, RadToolStripManager manager)
		{
			CheckItem checkBox = new CheckItem(item, form, manager);
            checkBox.Alignment = ContentAlignment.MiddleLeft;
			this.radListBox1.Visible = true;

			if (item != null)
			{
				if (item.Text == "")
					return;

				checkBox.Text = item.Text;
				if (item.Visibility == ElementVisibility.Visible)
                    checkBox.ToggleState = ToggleState.On;
				else
                    checkBox.ToggleState = ToggleState.Off;
			}
			else if (form != null)
			{
				checkBox.Text = form.Text;
				if (form.Visible)
					checkBox.ToggleState = ToggleState.On;
				else
                    checkBox.ToggleState = ToggleState.Off;
			}

			checkBox.MinSize = new Size(150, 20);
			checkBox.Click += new EventHandler(checkBox_Click);

            if ( ! this.radListBox1.Items.Contains(checkBox))
            {
                this.radListBox1.Items.Add(checkBox);
            }
		}

		public void LoadDataInPanel()
		{
			// Note:
			// When you finish with refactoring it pls let me know 
			// because I'd like to test the behavior of the control.
			// B.Markov
			if (this.manager != null)
			{
				if (this.manager.DockingSites.Count > 0)
				{
					for (int i = 0; i < this.manager.DockingSites.Count; i++)
					{
						RadToolStrip toolStrip = this.manager.DockingSites[i] as RadToolStrip;
						RadToolStripManager manager = toolStrip.ToolStripManager;

						foreach (RadToolStripElement element in manager.Items)
						{
                            foreach (RadToolStripItem item in element.Items)
                            {
                                AddCheckToListBox(item, null, manager);
                            }
						}

						foreach (RadToolStripElement element in manager.elementList)
						{
                            foreach (RadToolStripItem item in element.Items)
                            {
                                AddCheckToListBox(item, null, manager);
                            }
						}

                        foreach (FloatingForm form in manager.formList)
                        {
                            AddCheckToListBox(null, form, manager);
                        }
					}
				}
				else
				{
					foreach (RadToolStripElement element in this.manager.Items)
					{
                        foreach (RadToolStripItem item in element.Items)
                        {
                            AddCheckToListBox(item, null, this.manager);
                        }
					}
					foreach (RadToolStripElement element in this.manager.elementList)
					{
                        foreach (RadToolStripItem item in element.Items)
                        {
                            AddCheckToListBox(item, null, this.manager);
                        }
					}

                    foreach (FloatingForm form in this.manager.formList)
                    {
                        AddCheckToListBox(null, form, this.manager);
                    }
				}
				this.load = true;
			}
		}

		private void checkBox_Click(object sender, EventArgs e)
		{
			RadToolStripManager manager = (sender as CheckItem).AssociatedManager;
            CheckItem item = sender as CheckItem;
            RadToolStripItem strip = item.AssociatedItem;

            if (strip != null)
			{
                if (strip.Visibility == ElementVisibility.Collapsed)
				{
                    for (int i = 0; i < manager.elementList.Count; i++)
					{
                        RadToolStripElement element = manager.elementList[i] as RadToolStripElement;
                        if (element != null && element.Items.Contains(strip))
						{
                            element.Orientation = manager.Orientation;
							manager.Items.Add(element);
							manager.elementList.Remove(element);
							break;
						}
					}

                    strip.Visibility = ElementVisibility.Visible;
                    strip.Margin = new Padding(0, 0, 0, 0);
                    strip.InvalidateLayout();
				}
				else
				{
					strip.Visibility = ElementVisibility.Collapsed;
					strip.Margin = new Padding(0, 0, 0, 0);
					strip.InvalidateLayout();

					foreach (RadToolStripElement element in manager.Items)
					{
						if (element.Items.Contains(strip))
						{
							if (VisibleItemsOnRow(element) == 0)
							{
								RadToolStripElement myElement = new RadToolStripElement();
								foreach (RadToolStripItem currentItem in element.Items)
								{
									currentItem.ParentToolStripElement = myElement;
									currentItem.Grip.ParentToolStripElement = myElement;
									myElement.Items.Add(currentItem);
								}

								manager.elementList.Add(myElement);

								manager.Items.Remove(element);
								break;
					
							}
						}

					}
				}
			}
			else
			{
				if (item.AssociatedForm.Visible)
				{
					item.AssociatedForm.Visible = false;
				}
				else
				{
					item.AssociatedForm.Visible = true;
				}
			}
		}

		private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
		{
		}

		private void checkBox_MouseDown(object sender, MouseEventArgs e)
		{
		
		}

		private int VisibleItemsOnRow(RadToolStripElement element)
		{
			int i = 0;
			foreach (RadToolStripItem item in element.Items)
			{
				if (item.Visibility == ElementVisibility.Visible)
					i++;
			}
			return i;
		}

		private void radButton1_Click(object sender, EventArgs e)
		{

		}


		public class CheckItem : RadCheckBoxElement
		{
			private RadToolStripItem associatedItem;
			private FloatingForm  associatedForm;
			private RadToolStripManager associatedManager;

			public CheckItem(RadToolStripItem item, FloatingForm form, RadToolStripManager manager)
			{
				this.associatedItem = item;
                this.associatedForm = form;
				this.associatedManager = manager;
			}
			/// <summary>
			/// Gets the AssociatedItem
			/// </summary>
			public RadToolStripItem AssociatedItem
			{
				get
				{
					return this.associatedItem;
				}
			}

			/// <summary>
			/// Gets the AssociatedManager
			/// </summary>
			public RadToolStripManager AssociatedManager
			{
				get
				{
					return this.associatedManager;
				}
			}
			/// <summary>
			/// Gets the AssociatedForm
			/// </summary>
			public FloatingForm AssociatedForm
			{
				get
				{
					return this.associatedForm;
				}
			}

            protected override Type ThemeEffectiveType
            {
                get
                {
                    return typeof(RadCheckBoxElement);
                }
            }
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void radButton5_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void radButton4_Click(object sender, EventArgs e)
		{
			int count = 0;

			foreach (CheckItem item in this.radListBox1.Items)
			{
                if (!(item.ToggleState == ToggleState.On))
                {
                    count++;
                }
			}

			if (count > 0)
			{
				DialogResult d = MessageBox.Show(
                    RadToolStripLocalizationProvider.CurrentProvider.GetLocalizationString(RadToolStripLocalizationStringId.ResetToolBarsAlertMessage)
                    , "RadToolStrip", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

				if (d == DialogResult.OK)
				{
                    foreach (CheckItem item in this.radListBox1.Items)
					{
						RadToolStripManager manager = item.AssociatedManager;
                        manager.SuspendLayout();

						for (int i = 0; i < manager.elementList.Count; i++)
						{
                            RadToolStripElement element = manager.elementList[i] as RadToolStripElement;
                            if (element != null && element.Items.Contains(item.AssociatedItem))
                            {
                                element.Orientation = manager.Orientation;
							    manager.Items.Add(element);
							    manager.elementList.Remove(element);
							    break;
                            }
						}

                        manager.ResumeLayout(false);

                        if (item.AssociatedItem != null)
                        {
                            item.AssociatedItem.Visibility = ElementVisibility.Visible;
                            item.AssociatedItem.Margin = new Padding(0, 0, 0, 0);
                            item.ToggleState = ToggleState.On;
                        }
                        else if (item.AssociatedForm != null)
                        {
                            item.AssociatedForm.Show();
                            item.ToggleState = ToggleState.On;
                        }
					}
				}
			}
		}

        private void ToolStripDialogForm_Load_1(object sender, EventArgs e)
        {
            this.resetButton.Text =
                RadToolStripLocalizationProvider.CurrentProvider.GetLocalizationString(RadToolStripLocalizationStringId.ResetButton);
            this.closeButton.Text =
                RadToolStripLocalizationProvider.CurrentProvider.GetLocalizationString(RadToolStripLocalizationStringId.CloseButton);
            this.toolBarsTabItem.Text =
                RadToolStripLocalizationProvider.CurrentProvider.GetLocalizationString(RadToolStripLocalizationStringId.ToolBarsTabItem);
            this.Text = RadToolStripLocalizationProvider.CurrentProvider.GetLocalizationString(
                RadToolStripLocalizationStringId.CustomizeDialogCaption);
        }
		
	}
	
}