using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.Styles
{
    public static class ElementHierarchyEnumerator
    {
        /// <summary>
        /// Traverses element hierarchy, including the specified element, ignoring hierarchy of elements (and their children) that have already Style property assigned
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static IEnumerable<RadElement> TraverseElements(RadElement rootElement)
        {
            return TraverseElements(rootElement, true);
        }

        public static IEnumerable<RadElement> TraverseElements(RadElement rootElement, bool includeRootElement)
        {
            if (includeRootElement)
            {
                yield return rootElement;
            }

            if (rootElement.Children.Count > 0)
            {
                Stack parentIndexes = new Stack();

                RadElementCollection children = rootElement.Children;
                int depth = 1;
                int currIndex = 0;
                bool traversingChildren = true;
                do
                {
                    if (currIndex >= children.Count)
                    {
                        depth -= 1;
                        if (depth > 0)
                        {
                            RadElement parent = children.Owner.Parent;
                            if (parent != null)
                            {
                                currIndex = (int)parentIndexes.Pop();
                                children = parent.Children;
                                traversingChildren = false;
                            }
                        }
                    }
                    else
                    {
                        RadElement child = children[currIndex];
                        StyleSheet childStyle = child.Style;

                        if (traversingChildren)
                        {
                            if (child.PropagateStyleToChildren &&
                                (childStyle == null || !child.ElementThemeAffectsChildren) &&
                                child.Children.Count > 0)
                            {
                                parentIndexes.Push(currIndex);
                                children = child.Children;
                                currIndex = 0;
                                depth++;
                                continue;
                            }
                        }

                        if (childStyle == null)
                        {
                            yield return child;
                        }

                        currIndex++;
                        traversingChildren = true;
                    }

                }
                while (depth > 0);
            }
        }
    }
}
