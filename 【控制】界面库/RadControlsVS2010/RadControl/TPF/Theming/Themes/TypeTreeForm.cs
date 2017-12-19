using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

namespace Telerik.WinControls
{
    public partial class TypeTreeForm : Form
    {
        private class TreeNodesComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return string.Compare(((TreeNode)x).Text.ToString(), ((TreeNode)y).Text.ToString());
            }
        }

        object selectedTag = null;
      
        public object SelectedTag
        {
            get { return selectedTag; }
        }

        object parentSelectedTag = null;
        public object ParentSelectedTag
        {
            get { return parentSelectedTag; }
        }

        object value;
        bool includeProperties;

        public TypeTreeForm(bool includeProperties, object value)
        {
            InitializeComponent();
            this.value = value;
            this.includeProperties = includeProperties;

			try
			{
				FillReflectionTree(typeof(RadObject));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
        }

        void FillReflectionTree(Type assignableFrom) 
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            ArrayList AssemblyNodes = new ArrayList();
            foreach (Assembly assembly in assemblies)
            {
                SortedList namespaces = new SortedList();
				Type[] assemblyTypes;
				try
				{
					assemblyTypes = assembly.GetTypes();
                    
				}
				catch
				{
					continue;
				}

                foreach (Type type in assemblyTypes)
                {
                    if (type.IsPublic && type.IsClass && assignableFrom.IsAssignableFrom(type))
                    {
                        ArrayList list;// = null;

                        string namespaceName = type.Namespace;
                        if (namespaceName == null)
                        {
                            namespaceName = string.Empty;
                        }

                        if (namespaces.Contains(namespaceName))
                        {
                            list = (ArrayList)namespaces[namespaceName];
                        }
                        else
                        {
                            list = new ArrayList();
                            if (list.Count > 0)
                            {

                            }
                            namespaces.Add(namespaceName, list);
                        }

                        list.Add(type);
                       
                    }
                }
                
				try
				{
                    
					if (namespaces.Count > 0)
					{
						TreeNode assemblyNode = new TreeNode(assembly.GetName().Name);
                        AssemblyNodes.Add(assemblyNode);
                        
						foreach (DictionaryEntry de in namespaces)
						{
                            ArrayList list = (ArrayList)de.Value;
							TreeNode namespaceNode = new TreeNode((string)de.Key);
							assemblyNode.Nodes.Add(namespaceNode);

                            ArrayList TypeNodes = new ArrayList();

							foreach (Type type in list)
							{
                                
								TreeNode typeNode = new TreeNode(type.Name);
								typeNode.Tag = type;

								if (includeProperties)
								{
                                    ArrayList PropertyNodes = new ArrayList();
									RadObjectType radObjectType = RadObjectType.FromSystemType(type);
                                   
                                    foreach (RadProperty property in radObjectType.GetRadProperties())
									{
										TreeNode node = new TreeNode(property.Name);
										node.Tag = property;
                                        PropertyNodes.Add(node);
										
										if (value != null && value.Equals(type.FullName + "." + property.Name))
										{
											treeView1.SelectedNode = node;
											node.EnsureVisible();
										}
									}
                                    
                                    PropertyNodes.Sort(new TreeNodesComparer());
                                    foreach (TreeNode t in PropertyNodes)
                                    {
                                        typeNode.Nodes.Add(t);
                                    }
                                    
								}
								else if (value != null && value.Equals(type.FullName))
								{
									treeView1.SelectedNode = typeNode;
									typeNode.EnsureVisible();
								}

                                if (!includeProperties || typeNode.Nodes.Count > 0)
                                {
                                    TypeNodes.Add(typeNode);
                                }
                            }
                            
                            TypeNodes.Sort(new TreeNodesComparer());
                            foreach (TreeNode t in TypeNodes)
                            {
                                namespaceNode.Nodes.Add(t);
                            }
                            
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
            }
            
            AssemblyNodes.Sort(new TreeNodesComparer());
            foreach (TreeNode t in AssemblyNodes)
            {
                treeView1.Nodes.Add(t);
            }
            
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null && treeView1.SelectedNode.Nodes.Count == 0)
            {
                if (e.Node.Parent != null)
                    parentSelectedTag = e.Node.Parent.Tag;
                else
                    parentSelectedTag = null;
                selectedTag = e.Node.Tag;
            }
            else
            {
                parentSelectedTag = null;
                selectedTag = null;
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Tag != null && treeView1.SelectedNode.Nodes.Count == 0)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}