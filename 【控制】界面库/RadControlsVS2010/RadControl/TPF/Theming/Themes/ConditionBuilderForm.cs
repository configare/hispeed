using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public partial class ConditionBuilderForm : Form
    {
        public ConditionBuilderForm(XmlCondition condition)
        {
            InitializeComponent();

            propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);

            if (condition == null)
            {
                TreeNode node = new TreeNode("condition");
                node.Tag = new XmlSimpleCondition();
                treeView1.Nodes.Add(node);
            }
            else
                AddCondition(null, condition);

            label1.Text = BuildConditionString(treeView1.Nodes[0]);

			ExpandTreeToLevel(treeView1.Nodes, 3);
        }

		internal static void ExpandTreeToLevel(TreeNodeCollection fromNodes, int maxLevel)
		{
			foreach (TreeNode currNode in fromNodes)
			{
				if (currNode.Level > maxLevel)
					return;

				currNode.Expand();
				ExpandTreeToLevel(currNode.Nodes, maxLevel);
			}
		}

        public XmlCondition GetConditionTree()
        {
            return GetConditionTree(treeView1.Nodes[0]);
        }

        XmlCondition GetConditionTree(TreeNode root)
        {
            if (root.Tag is XmlSimpleCondition)
                return root.Tag as XmlSimpleCondition;
            else
            {
                XmlComplexCondition condition = root.Tag as XmlComplexCondition;
                if (root.Nodes.Count > 0) condition.Condition1 = GetConditionTree(root.Nodes[0]);
                if (root.Nodes.Count > 1) condition.Condition2 = GetConditionTree(root.Nodes[1]);
                return condition;
            }
        }

        void AddCondition(TreeNode root, XmlCondition condition)
        {
            if (condition != null)
            {
                TreeNode node = null;

                if (condition is XmlSimpleCondition)
                    node = new TreeNode("condition");
                else
                {
                    node = new TreeNode((condition as XmlComplexCondition).BinaryOperator.ToString().Replace("Operator", ""));
                    AddCondition(node, (condition as XmlComplexCondition).Condition1);
                    AddCondition(node, (condition as XmlComplexCondition).Condition2);
                }
                node.Tag = condition;
                if (root == null)
                    treeView1.Nodes.Add(node);
                else
                    root.Nodes.Add(node);
            }
        }

        public string BuildConditionString()
        {
            return BuildConditionString(treeView1.Nodes[0]);
        }

        string BuildConditionString(TreeNode root)
        {
            string s = "";

            if (root.Tag is XmlComplexCondition)
            {
                XmlComplexCondition condition = root.Tag as XmlComplexCondition;
                s += "(";
                if (root.Nodes.Count > 0) s += BuildConditionString(root.Nodes[0]);
                s += " " + condition.BinaryOperator.ToString().Replace("Operator", "").ToLower() + " ";
                if(root.Nodes.Count > 1) s += BuildConditionString(root.Nodes[1]);
                s += ")";
            } 
            else if (root.Tag is XmlSimpleCondition)
            {
                XmlSimpleCondition condition = root.Tag as XmlSimpleCondition;
                if(condition.UnaryOperator == UnaryOperator.NotOperator) s += "!";
                if (condition.Setting != null)
                    s += condition.Setting.GetPropertyName();
                else
                    s += "unknown";
            }
            
            return s;
        }		

        private void buttonAddSimple_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null &&
                treeView1.SelectedNode.Tag is XmlComplexCondition &&
                treeView1.SelectedNode.Nodes.Count < 2)
            {
                XmlSimpleCondition condition = new XmlSimpleCondition();
                TreeNode node = new TreeNode("unknown");
                node.Tag = condition;
                treeView1.SelectedNode.Nodes.Add(node);
                treeView1.SelectedNode.Expand();
            }
            if(treeView1.SelectedNode != null && treeView1.SelectedNode.Nodes.Count > 1)
            {
                buttonAddSimple.Enabled = false;
                buttonAddComplex.Enabled = false;
            }
            label1.Text = BuildConditionString(treeView1.Nodes[0]);
        }

        private void buttonAddComplex_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null &&
                treeView1.SelectedNode.Tag is XmlComplexCondition &&
                treeView1.SelectedNode.Nodes.Count < 2)
            {
                XmlComplexCondition condition = new XmlComplexCondition();
                TreeNode node = new TreeNode(condition.BinaryOperator.ToString().Replace("Operator", ""));
                node.Tag = condition;
                treeView1.SelectedNode.Nodes.Add(node);
                treeView1.SelectedNode.Expand();
            }
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Nodes.Count > 1)
            {
                buttonAddSimple.Enabled = false;
                buttonAddComplex.Enabled = false;
            }
            label1.Text = BuildConditionString(treeView1.Nodes[0]);
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode == treeView1.Nodes[0])
                    return;
                treeView1.SelectedNode.Remove();
                
            }
            label1.Text = BuildConditionString(treeView1.Nodes[0]);
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Tag is XmlComplexCondition)
                {
                    XmlSimpleCondition condition = new XmlSimpleCondition();
                    treeView1.SelectedNode.Tag = condition;
                    treeView1.SelectedNode.Nodes.Clear();
                    treeView1_AfterSelect(null, null);
                }
                else if (treeView1.SelectedNode.Tag is XmlSimpleCondition)
                {
					XmlSimpleCondition oldCondition = treeView1.SelectedNode.Tag as XmlSimpleCondition;

                    XmlComplexCondition condition = new XmlComplexCondition();
                    treeView1.SelectedNode.Tag = condition;

					TreeNode node1 = new TreeNode(treeView1.SelectedNode.Text);
					node1.Tag = oldCondition;
                    treeView1.SelectedNode.Nodes.Add(node1);

                    TreeNode node2 = new TreeNode("unknown");
                    node2.Tag = new XmlSimpleCondition();
                    treeView1.SelectedNode.Nodes.Add(node2);
                    
                    treeView1_AfterSelect(null, null);

					treeView1.SelectedNode.Text = condition.BinaryOperator.ToString().Replace("Operator", "");
					treeView1.SelectedNode.ExpandAll();
                }
            }
            label1.Text = BuildConditionString(treeView1.Nodes[0]);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                propertyGrid1.SelectedObject = treeView1.SelectedNode.Tag as XmlCondition;
				propertyGrid1.ExpandAllGridItems();

                if (treeView1.SelectedNode.Tag is XmlComplexCondition)
                {
                    if (treeView1.SelectedNode.Nodes.Count > 1)
                    {
                        buttonAddSimple.Enabled = false;
                        buttonAddComplex.Enabled = false;
                    }
                    else
                    {
                        buttonAddSimple.Enabled = true;
                        buttonAddComplex.Enabled = true;
                    }
                    buttonConvert.Enabled = true;
                    buttonRemove.Enabled = true;
                }
                else if (treeView1.SelectedNode.Tag is XmlSimpleCondition)
                {
                    buttonAddSimple.Enabled = false;
                    buttonAddComplex.Enabled = false;
                    buttonConvert.Enabled = true;
                    buttonRemove.Enabled = true;
                }
            }
            else
            {
                buttonAddSimple.Enabled = false;
                buttonAddComplex.Enabled = false;
                buttonConvert.Enabled = false;
                buttonRemove.Enabled = false;
            }

            if (treeView1.SelectedNode == treeView1.Nodes[0])
                buttonRemove.Enabled = false;
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (treeView1.SelectedNode.Tag is XmlComplexCondition)
                treeView1.SelectedNode.Text = (treeView1.SelectedNode.Tag as XmlComplexCondition).BinaryOperator.ToString().Replace("Operator", "");
            else
            {
                XmlSimpleCondition condition = treeView1.SelectedNode.Tag as XmlSimpleCondition;
                if (e.ChangedItem.Label == "Property")
                {
                    condition.Setting.Value = null;
                    propertyGrid1.Refresh();
                }
                else if (condition.Setting != null)
                    treeView1.SelectedNode.Text = (treeView1.SelectedNode.Tag as XmlSimpleCondition).Setting.Property;
            }
            label1.Text = BuildConditionString(treeView1.Nodes[0]);
        }
    }
}