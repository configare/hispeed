using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    internal class TreeNodeDescriptor
    {
        LinkedList<PropertyDescriptor> parentDescriptor;
        LinkedList<PropertyDescriptor> valueDescriptor;
        LinkedList<PropertyDescriptor> displayDescriptor;
        LinkedList<PropertyDescriptor> childDescriptor;

        public TreeNodeDescriptor()
        {
            this.childDescriptor = new LinkedList<PropertyDescriptor>();
            this.parentDescriptor = new LinkedList<PropertyDescriptor>();
            this.valueDescriptor = new LinkedList<PropertyDescriptor>();
            this.displayDescriptor = new LinkedList<PropertyDescriptor>();
        }

        public TreeNodeDescriptor(PropertyDescriptor parentDescriptor, PropertyDescriptor childDescriptor, PropertyDescriptor valueDescriptor, PropertyDescriptor displayDescriptor)
        {
            this.childDescriptor = new LinkedList<PropertyDescriptor>(new PropertyDescriptor[] { childDescriptor });
            this.parentDescriptor = new LinkedList<PropertyDescriptor>(new PropertyDescriptor[] { parentDescriptor });
            this.valueDescriptor = new LinkedList<PropertyDescriptor>(new PropertyDescriptor[] { valueDescriptor });
            this.displayDescriptor = new LinkedList<PropertyDescriptor>(new PropertyDescriptor[] { displayDescriptor });
        }

        public TreeNodeDescriptor(PropertyDescriptorCollection descriptors, string parentPath, string childPath, string valuePath, string displayPath)
        {
            //this.childDescriptor = new LinkedList<PropertyDescriptor>(new PropertyDescriptor[] { childDescriptor });
            //this.parentDescriptor = new LinkedList<PropertyDescriptor>(new PropertyDescriptor[] { parentDescriptor });
            //this.valueDescriptor = new LinkedList<PropertyDescriptor>(new PropertyDescriptor[] { valueDescriptor });
            //this.displayDescriptor = new LinkedList<PropertyDescriptor>(new PropertyDescriptor[] { displayDescriptor });
        }

        public void SetDisplaytDescriptor(PropertyDescriptor descriptor, string path)
        {
            string[] names = path.Split('.');
            this.displayDescriptor.Clear();

            int index = 0;
            while (descriptor != null)
            {
                this.displayDescriptor.AddLast(descriptor);

                index++;
                if (index < names.Length)
                {
                    descriptor = descriptor.GetChildProperties().Find(names[index], true);
                }
                else
                {
                    descriptor = null;
                }
            }
        }

        public void SetChildDescriptor(PropertyDescriptor descriptor, string path)
        {
            string[] names = path.Split('.');
            this.childDescriptor.Clear();

            int index = 0;
            while (descriptor != null)
            {
                this.childDescriptor.AddLast(descriptor);

                index++;
                if (index < names.Length)
                {
                    descriptor = descriptor.GetChildProperties().Find(names[index], true);
                }
                else
                {
                    descriptor = null;
                }
            }
        }

        public void SetValueDescriptor(PropertyDescriptor descriptor, string path)
        {
            string[] names = path.Split('.');
            this.valueDescriptor.Clear();

            int index = 0;
            while (descriptor != null)
            {
                this.valueDescriptor.AddLast(descriptor);

                index++;
                if (index < names.Length)
                {
                    descriptor = descriptor.GetChildProperties().Find(names[index], true);
                }
                else
                {
                    descriptor = null;
                }
            }
        }

        public void SetParentDescriptor(PropertyDescriptor descriptor, string path)
        {
            string[] names = path.Split('.');
            this.parentDescriptor.Clear();

            int index = 0;
            while (descriptor != null)
            {
                this.parentDescriptor.AddLast(descriptor);

                index++;
                if (index < names.Length)
                {
                    descriptor = descriptor.GetChildProperties().Find(names[index], true);
                }
                else
                {
                    descriptor = null;
                }
            }
        }

        public PropertyDescriptor ParentDescriptor
        {
            get 
            {
                if (parentDescriptor.First == null)
                {
                    return null;
                }

                return parentDescriptor.First.Value; 
            }
            set 
            {
                if (parentDescriptor.First == null)
                {
                    parentDescriptor.AddFirst(value);
                    return;
                }

                parentDescriptor.First.Value = value; 
            }
        }

        public PropertyDescriptor ValueDescriptor
        {
            get 
            {
                if (valueDescriptor.First == null)
                {
                    return null;
                }

                return valueDescriptor.First.Value; 
            }
            set 
            {
                if (valueDescriptor.First == null)
                {
                    valueDescriptor.AddFirst(value);
                    return;
                }

                valueDescriptor.First.Value = value; 
            }
        }

        public PropertyDescriptor DisplayDescriptor
        {
            get 
            {
                if (displayDescriptor.First == null)
                {
                    return null;
                }

                return displayDescriptor.First.Value; 
            }
            set 
            {
                if (displayDescriptor.First == null)
                {
                    displayDescriptor.AddFirst(value);
                    return;
                }

                displayDescriptor.First.Value = value; 
            }
        }

        public PropertyDescriptor ChildDescriptor
        {
            get 
            {
                if (childDescriptor.First == null)
                {
                    return null;
                }

                return childDescriptor.First.Value; 
            }
            set 
            {
                if (childDescriptor.First == null)
                {
                    childDescriptor.AddFirst(value);
                    return;
                }

                childDescriptor.First.Value = value; 
            }
        }

        public object GetNestedValue(RadTreeNode node, LinkedList<PropertyDescriptor> nestedDescriptor)
        {
            object val = node.DataBoundItem;
            LinkedListNode<PropertyDescriptor> current = nestedDescriptor.First;

            while (current != null)
            {
                val = current.Value.GetValue(val);
                current = current.Next;
            }

            return val;
        }

        public void SetNestedValue(RadTreeNode node, LinkedList<PropertyDescriptor> nestedDescriptor, object value)
        {
            object val = node.DataBoundItem;
            LinkedListNode<PropertyDescriptor> current = nestedDescriptor.First;

            while (current != null)
            {
                if (current.Next == null)
                {
                    current.Value.SetValue(val, value);
                    return;
                }

                val = current.Value.GetValue(val);
            }
        }

        public object GetParent(RadTreeNode node)
        {
            return GetNestedValue(node, this.parentDescriptor);
        }

        public void SetParent(RadTreeNode node, object value)
        {
            SetNestedValue(node, this.parentDescriptor, value);
        }

        public object GetValue(RadTreeNode node)
        {
            return GetNestedValue(node, this.valueDescriptor);
        }

        public void SetValue(RadTreeNode node, object value)
        {
            SetNestedValue(node, this.valueDescriptor, value);
        }

        public object GetDisplay(RadTreeNode node)
        {
            return GetNestedValue(node, this.displayDescriptor);
        }

        public void SetDisplay(RadTreeNode node, object value)
        {
            SetNestedValue(node, this.displayDescriptor, value);
        }

        public object GetChild(RadTreeNode node)
        {
            return GetNestedValue(node, this.childDescriptor);
        }

        public void SetChild(RadTreeNode node, object value)
        {
            SetNestedValue(node, this.childDescriptor, value);
        }
    }
}
