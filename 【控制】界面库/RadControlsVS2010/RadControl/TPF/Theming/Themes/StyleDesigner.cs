using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Telerik.WinControls;
using Telerik.WinControls.Primitives;
using System.Reflection;

namespace Telerik.WinControls
{
    public partial class StyleDesigner : Form
    {
        private XmlStyleSheet currentStyle = null;

        private string currentFileName = string.Empty;
        private XmlTheme currentTheme;

        public StyleDesigner()
        {
            InitializeComponent();
        }

        public XmlStyleSheet CurrentStyle
        {
            get { return currentStyle; }
            set { currentStyle = value; }
        }

        public string CurrentFileName
        {
            get { return currentFileName; }
            set { currentFileName = value; }
        }

        public XmlTheme CurrentTheme
        {
            get { return currentTheme; }
            set { currentTheme = value; }
        }

        public void LoadTheme(XmlTheme theme)
        {
            this.tbThemeName.Text = theme.ThemeName;

            this.tvBuilders.Nodes.Clear();
            this.tvPropertySettingGroups.Nodes.Clear();
            this.CurrentTheme = theme;

            foreach (XmlStyleBuilderRegistration reg in theme.BuilderRegistrations)
            {
                TreeNode node = new TreeNode(GetBuilderString(reg));
                node.Tag = reg;
                this.tvBuilders.Nodes.Add(node);
                //MessageBox.Show("" + ((XmlStyleSheet)reg.BuilderData).PropertySettingGroups.Count);
            }

            if ( this.tvBuilders.Nodes.Count > 0 )
            {
                this.tvBuilders.SelectedNode = this.tvBuilders.Nodes[0];
            }
        }

        private string GetBuilderString(XmlStyleBuilderRegistration builder)
        {
            string res = string.Empty;

			if (builder.StylesheetRelations.Count != 1)
			{
				throw new InvalidOperationException("Stylesheet registration contais relations for more than one control");
			}

			RadStylesheetRelation relation = builder.StylesheetRelations[0];

			if (relation.RegistrationType == BuilderRegistrationType.ElementTypeControlType)
            {
				res = "Style for controls of Type " + relation.ControlType;
				if (relation.ElementType != typeof(RootRadElement).FullName)
                {
					res += " and elements of Type " + relation.ElementType;
                }
            }
            else
				if (relation.RegistrationType == BuilderRegistrationType.ElementTypeDefault)
                {
					res = "Style for elements of Type " + relation.ElementType;
                }
                else
					if (relation.RegistrationType == BuilderRegistrationType.ElementTypeDefault)
                    {
						res = "Style for elements of Type " + relation.ElementType;
                    }

            return res;
        }

        TreeNode AddPropertySettingGroup(XmlPropertySettingGroup group)
        {
            TreeNode node = new TreeNode(group.ToString(), 1, 1);
            node.Tag = group;

            TreeNode selectorsNode = new TreeNode("Selectors", 2, 2);
            selectorsNode.Name = "Selectors";
            selectorsNode.Tag = group.Selectors;
            node.Nodes.Add(selectorsNode);

            TreeNode settingsNode = new TreeNode("Settings", 4, 4);
            settingsNode.Name = "Settings";
            settingsNode.Tag = group.PropertySettings;
            node.Nodes.Add(settingsNode);

            tvPropertySettingGroups.Nodes.Add(node);

            return node;
        }

        void AddSelector(TreeNode node, XmlElementSelector selector)
        {
            if (node != null && selector != null)
            {
                TreeNode newNode = new TreeNode(selector.ToString(), 3, 3);
                newNode.Tag = selector;
                node.Nodes.Add(newNode);
            }
        }

        void AddSetting(TreeNode node, XmlPropertySetting setting)
        {
            if (node != null && setting != null)
            {
                TreeNode newNode = new TreeNode(setting.ToString(), 5, 5);
                newNode.Tag = setting;
                node.Nodes.Add(newNode);
            }
        }

        private void StyleDesigner_Load(object sender, EventArgs e)
        {
            if (CurrentStyle == null)
            {
                CurrentStyle = new XmlStyleSheet();
            }
        }

        private void itemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlPropertySettingGroup group = new XmlPropertySettingGroup();
            CurrentStyle.PropertySettingGroups.Add(group);
            AddPropertySettingGroup(group);
        }

        private void addSelectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlElementSelector selector = null;

            switch ((sender as ToolStripMenuItem).Name)
            {
                case "TypeSelector": selector = new XmlTypeSelector(); break;
                case "NameSelector": selector = new XmlNameSelector(); break;
                //case "GeneralSelector": selector = new XmlGeneralSelector(); break;
                case "ClassSelector": selector = new XmlClassSelector(); break;
            }

            if (tvPropertySettingGroups.SelectedNode != null)
            {
                AddSelector(tvPropertySettingGroups.SelectedNode, selector);
                XmlPropertySettingGroup group = tvPropertySettingGroups.SelectedNode.Parent.Tag as XmlPropertySettingGroup;
                group.Selectors.Add(selector);
                tvPropertySettingGroups.SelectedNode.Expand();
            }
        }

        private void addPropertySettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlPropertySetting setting = null;

            switch (((ToolStripMenuItem)sender).Name)
            {
                case "AnimatedPropertySetting": setting = new XmlAnimatedPropertySetting(); break;
                case "PropertySetting": setting = new XmlPropertySetting(); break;
            }

            if (tvPropertySettingGroups.SelectedNode != null)
            {
                AddSetting(tvPropertySettingGroups.SelectedNode, setting);
                XmlPropertySettingGroup group = (XmlPropertySettingGroup)tvPropertySettingGroups.SelectedNode.Parent.Tag;
                group.PropertySettings.Add(setting);
                tvPropertySettingGroups.SelectedNode.Expand();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tvPropertySettingGroups.SelectedNode != null)
            {
                if (tvPropertySettingGroups.SelectedNode.Tag is IPropertySetting)
                {
                    XmlPropertySettingGroup group = tvPropertySettingGroups.SelectedNode.Parent.Parent.Tag as XmlPropertySettingGroup;
                    group.PropertySettings.Remove((XmlPropertySetting)tvPropertySettingGroups.SelectedNode.Tag);
                }
                else if (tvPropertySettingGroups.SelectedNode.Tag is XmlElementSelector)
                {
                    XmlPropertySettingGroup group = tvPropertySettingGroups.SelectedNode.Parent.Parent.Tag as XmlPropertySettingGroup;
                    group.Selectors.Remove((XmlElementSelector)tvPropertySettingGroups.SelectedNode.Tag);
                }
                else if (tvPropertySettingGroups.SelectedNode.Tag is XmlPropertySettingGroup)
                {
                    CurrentStyle.PropertySettingGroups.Remove((XmlPropertySettingGroup)tvPropertySettingGroups.SelectedNode.Tag);
                }
                tvPropertySettingGroups.SelectedNode.Remove();
            }
        }

        private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!(e.Node.Tag is XmlPropertySettingGroup))
                e.CancelEdit = false;
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            //if (e.Node.Tag is PropertySettingGroup)
            //    (e.Node.Tag as PropertySettingGroup).Text = e.Label;
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && tvPropertySettingGroups.SelectedNode != null)
                tvPropertySettingGroups.SelectedNode.BeginEdit();
        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pt = new Point(e.X, e.Y);
                tvPropertySettingGroups.SelectedNode = tvPropertySettingGroups.GetNodeAt(pt);
                pt = tvPropertySettingGroups.PointToScreen(pt);
                if (tvPropertySettingGroups.SelectedNode == null) contextMenuStripRoot.Show(pt);
                else if (tvPropertySettingGroups.SelectedNode.Name == "Selectors") 
                    contextMenuStripSelector.Show(pt);
                else if (tvPropertySettingGroups.SelectedNode.Name == "Settings") 
                    contextMenuStripPropertySetting.Show(pt);
                else if (tvPropertySettingGroups.SelectedNode.Tag is XmlPropertySettingGroup ||
                         tvPropertySettingGroups.SelectedNode.Tag is XmlElementSelector ||
                         tvPropertySettingGroups.SelectedNode.Tag is XmlPropertySetting)
                    contextMenuStripElement.Show(pt);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = e.Node.Tag;
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            //LoadStyle(CreateTestStyle());
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                using (XmlReader reader = XmlReader.Create(openFileDialog1.FileName))
                {
                    XmlTheme xmlTheme = XmlTheme.LoadFromReader(reader);
                    if (xmlTheme != null)
                    {
                        if (this.CurrentTheme.BuilderRegistrations.Length > 0)
                        {
                            DialogResult res = MessageBox.Show("Theme already contains styles for controls. Merge loaded with existing?", "Load theme", MessageBoxButtons.YesNoCancel);
                            if (res == DialogResult.Yes)
                            {
                                this.MergerWithTheme(xmlTheme);
                                this.CurrentFileName = openFileDialog1.FileName;
                            }
                            else
                                if (res == DialogResult.No)
                                {
                                    this.LoadTheme(xmlTheme);
                                    this.CurrentFileName = openFileDialog1.FileName;
                                }
                        }
                    }
                }
            }
        }

        private void MergerWithTheme(XmlTheme theme)
        {
            //this.tbThemeName.Text = theme.ThemeName;

            this.tvBuilders.Nodes.Clear();
            this.tvPropertySettingGroups.Nodes.Clear();

            ArrayList mergedBuildersList = new ArrayList();

            mergedBuildersList.AddRange(this.CurrentTheme.BuilderRegistrations);
            mergedBuildersList.AddRange(theme.BuilderRegistrations);
            
            this.CurrentTheme = theme;

            theme.BuilderRegistrations = new XmlStyleBuilderRegistration[mergedBuildersList.Count];
            mergedBuildersList.CopyTo(theme.BuilderRegistrations, 0);

            this.LoadTheme(this.CurrentTheme);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.currentTheme.ThemeName))
            {
                MessageBox.Show("Please, enter Theme name");
                return;
            }

            if (!string.IsNullOrEmpty(CurrentFileName))
            {
                using (XmlWriter writer = this.CreateXmlWriter(CurrentFileName))
                {
					this.CurrentTheme.SaveToWriter(writer);
                }
            }
            else
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    CurrentFileName = saveFileDialog1.FileName;
					using (XmlWriter writer = this.CreateXmlWriter(CurrentFileName))
                    {
						this.CurrentTheme.SaveToWriter(writer);
                    }
                }
            }
        }

		private XmlWriter CreateXmlWriter(string fileName)
		{
			XmlTextWriter res = new XmlTextWriter(fileName, Encoding.UTF8);
			res.Formatting = Formatting.Indented;
			return res;
		}

        private void tvBuilders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            XmlStyleBuilderRegistration reg = (XmlStyleBuilderRegistration)e.Node.Tag;

            this.tvPropertySettingGroups.Nodes.Clear();            
            this.CurrentStyle = (XmlStyleSheet)reg.BuilderData;
            if (CurrentStyle != null)
            {
                foreach (XmlPropertySettingGroup group in CurrentStyle.PropertySettingGroups)
                {
                    TreeNode node = AddPropertySettingGroup(group);

                    foreach (XmlElementSelector selector in group.Selectors)
                        AddSelector(node.Nodes[0], selector);
                    foreach (XmlPropertySetting setting in group.PropertySettings)
                        AddSetting(node.Nodes[1], setting);
                }
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            this.AddReference();
        }

        private void tbThemeName_TextChanged(object sender, EventArgs e)
        {
            if (this.CurrentTheme != null)
            {
                this.CurrentTheme.ThemeName = tbThemeName.Text;
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGrid1.SelectedObject is XmlPropertySetting && e.ChangedItem.Label == "Property")
            {
                (propertyGrid1.SelectedObject as XmlPropertySetting).Value = null;
                propertyGrid1.Refresh();
            }
                
        }

        private void AddReference()
        {
            if (this.openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                Assembly asm = Assembly.LoadFrom(this.openFileDialog2.FileName);
                this.treeView1.Nodes[0].Nodes.Add(new TreeNode(asm.GetName().Name, 1, 1));
            }
        }

        private void btnAddReference_Click(object sender, EventArgs e)
        {
            this.AddReference();
        }

        private void removeCurrentControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tvBuilders.SelectedNode != null)
            {
                ArrayList filtered = new ArrayList();
                foreach(XmlStyleBuilderRegistration reg in this.currentTheme.BuilderRegistrations)
                {
                    if ( this.tvBuilders.SelectedNode.Tag != reg )
                    {
                        filtered.Add( reg );
                    }
                }

                this.currentTheme.BuilderRegistrations = new XmlStyleBuilderRegistration[filtered.Count];
                filtered.CopyTo(this.currentTheme.BuilderRegistrations, 0);

                this.LoadTheme(this.CurrentTheme);
            }
        }

        private void addStyleForControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Feature not available in the current release");
        }

        private void changeControlTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Feature not available in the current release");
        }        
    }
}