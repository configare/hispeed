using System;
using System.Collections;
using Telerik.WinControls.Commands;

namespace Telerik.WinControls.UI
{
    public class FindByTextCommand : CommandBase
    {
        public override object Execute(params object[] settings)
        {
            if (settings.Length > 1 &&
                this.CanExecute(settings[0]))
            {
                RadTreeNode node = settings[0] as RadTreeNode;
                IList parameters = settings[1] as IList;

                ValidateArguments(parameters);

                string text = parameters[0] as string;

                if (node != null && !string.IsNullOrEmpty(text))
                {
                    if (node.Text == text)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        private void ValidateArguments(IList args)
        {
            if (args == null)
            {
                throw new ArgumentException("Command arguments can not be null. FindByTextCommand accepts a single string parameter.");
            }

            if (args.Count == 0)
            {
                throw new ArgumentException("Command arguments can not be empty. FindByTextCommand accepts a single string parameter.");
            }

            if (!(args[0] is string))
            {
                throw new ArgumentException("Command argument must be of type string.");
            }
        }

        public override bool CanExecute(object parameter)
        {
            if (typeof(RadTreeNode).IsAssignableFrom(parameter.GetType()))
            {
                return true;
            }
            return base.CanExecute(parameter);
        }
    }

    public class NodeExpandCollapseCommand : CommandBase
    {
        public override object Execute(params object[] settings)
        {
            if (settings.Length > 0 &&
                this.CanExecute(settings[0]))
            {
                RadTreeNode node = settings[0] as RadTreeNode;
                bool expanded = false;
                try
                {
                    if (settings.Length > 1)
                    {
                        if (settings[1] is IList)
                        {
                            expanded = !(bool)((IList)settings[1])[0];
                        }
                        else
                        {
                            expanded = !(bool)settings[1];
                        }
                    }
                    else
                    {
                        expanded = node.Expanded;
                    }
                }
                catch (Exception)
                {
                    expanded = node.Expanded;
                }
                finally
                {
                    if (expanded)
                    {
                        node.Collapse();
                    }
                    else
                    {
                        node.Expand();
                    }
                }
                return node.Expanded;
            }
            return null;
        }

        public override bool CanExecute(object parameter)
        {
            if (typeof(RadTreeNode).IsAssignableFrom(parameter.GetType()))
            {
                return true;
            }
            return base.CanExecute(parameter);
        }
    }

    public class DeselectChildNodesCommand : CommandBase
    {
        public override object Execute(params object[] settings)
        {
            if (settings.Length > 0 && CanExecute(settings[0]))
            {
                RadTreeNode node = settings[0] as RadTreeNode;
                RadTreeView treeView = node.TreeView;
                if (treeView != null)
                {
                    if (treeView.SelectedNodes.Contains(node))
                    {
                        node.Selected = false;
                        return node;
                    }
                }
            }

            return base.Execute(settings);
        }

        public override bool CanExecute(object parameter)
        {
            if (parameter is RadTreeNode)
            {
                return true;
            }

            return base.CanExecute(parameter);
        }
    }

    public class CompareNodesTagCommand : CommandBase
    {
        public override object Execute(params object[] settings)
        {
            if (settings.Length > 1 &&
                this.CanExecute(settings[0]))
            {
                RadTreeNode node = settings[0] as RadTreeNode;
                object key = (settings[1] as IList)[0];

                if (node != null && node.Tag != null)
                {
                    IComparable comparer = node.Tag as IComparable;
                    if ((comparer != null && comparer.CompareTo(key) == 0) ||
                        node.Tag.Equals(key))
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        public override bool CanExecute(object parameter)
        {
            if (typeof(RadTreeNode).IsAssignableFrom(parameter.GetType()))
            {
                return true;
            }
            return base.CanExecute(parameter);
        }
    }

    public class SetCheckBoxCommand : CommandBase
    {
        public override object Execute(params object[] settings)
        {
            if (settings.Length > 1 &&
                this.CanExecute(settings[0]))
            {
                RadTreeNode node = settings[0] as RadTreeNode;
                bool isChecked;
                if (node == null)
                    return false;
                try
                {
                    if (settings.Length > 1)
                    {
                        if (settings[1] is IList)
                        {
                            isChecked = (bool)((IList)settings[1])[0];
                        }
                        else
                        {
                            isChecked = !(bool)settings[1];
                        }
                    }
                    else
                    {
                        isChecked = !node.Checked;
                    }
                    if (node.Checked != isChecked)
                    {
                        node.Checked = isChecked;
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public override bool CanExecute(object parameter)
        {
            if (typeof(RadTreeNode).IsAssignableFrom(parameter.GetType()))
            {
                return true;
            }
            return base.CanExecute(parameter);
        }
    }
}
