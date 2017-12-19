using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Telerik.WinControls.UI
{
    //public class TreeViewEnumerator : IEnumerator<RadTreeNode>
    //{
    //    private RadTreeNode current, startPosition;
    //    private Stack<int> position;

    //    public TreeViewEnumerator(RadTreeNode startPosition)
    //    {
    //        this.current = this.startPosition = startPosition;
    //        if (this.current != null)
    //        {
    //            this.position = new Stack<int>(16);
    //            this.position.Push(this.current.Index);
    //        }
    //    }

    //    public RadTreeNode Current
    //    {
    //        get 
    //        {
    //            return current;
    //        }
    //    }

    //    object System.Collections.IEnumerator.Current
    //    {
    //        get { return this.Current; }
    //    }

    //    public bool MoveNext()
    //    {
    //        if (this.current == null)
    //        {
    //            return false;
    //        }

    //        if (this.current.Expanded)
    //        {
    //            int index = 0;
    //            while (index < this.current.Nodes.Count)
    //            {
    //                if (this.current.Nodes[index].Visible)
    //                {
    //                    this.current = this.current.Nodes[index];
    //                    this.position.Push(index);
    //                    return true;
    //                }
    //                index++;
    //            }
    //        }

    //        RadTreeNode parentNode = this.current.parent;
    //        while (parentNode != null )//&& this.position.Count > 0
    //        {

    //            int pos = (this.position.Count > 1) ? this.position.Pop() + 1 : this.position.Peek() + 1;
    //            while (pos < parentNode.Nodes.Count)
    //            {
    //                if (parentNode.Nodes[pos].Visible)
    //                {
    //                    this.current = parentNode.Nodes[pos];
    //                    this.position.Push(pos);
    //                    return true;
    //                }
    //                pos++;
    //            }

    //            current = parentNode;
    //            parentNode = parentNode.parent;
    //        }

    //        return false;
    //    }

    //    public bool MovePrev()
    //    {
    //        return false;
    //    }

    //    public void Reset()
    //    {
    //        this.current = this.startPosition;
    //        this.position.Clear();
    //        this.position.Push(this.current.Index);
    //    }

    //    public void Dispose()
    //    {
    //        this.current = null;
    //        this.position.Clear();
    //    }
    //}
}
