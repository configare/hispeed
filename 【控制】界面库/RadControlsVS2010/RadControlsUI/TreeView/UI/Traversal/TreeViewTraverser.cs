
namespace Telerik.WinControls.UI
{
    public class TreeViewTraverser : ITraverser<RadTreeNode>
    {
        #region Fields

        private RadTreeViewElement owner;
        private RadTreeNode position;
        private TreeViewTraverser enumerator;
        //private TreeViewEnumerator enum2;

        #endregion

        #region Constructor

        public TreeViewTraverser(RadTreeViewElement owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Events

        public event TreeViewTraversingEventHandler Traversing;

        protected virtual void OnTraversing(TreeViewTraversingEventArgs e)
        {
            TreeViewTraversingEventHandler handler = this.Traversing;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected bool OnTraversing()
        {
            TreeViewTraversingEventArgs e = new TreeViewTraversingEventArgs(this.position);
            this.OnTraversing(e);
            return e.Process;
        }

        #endregion

        #region ITraverser<RadTreeNode> Members

        public object Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value as RadTreeNode;
                //this.enum2 = new TreeViewEnumerator(position);
            }
        }

        public bool MovePrevious()
        {
            while (this.MovePreviousCore())
            {
                if (this.OnTraversing())
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual bool MovePreviousCore()
        {
            if (position == null)
            {
                if (this.owner.Nodes.Count > 0)
                {
                    position = this.owner.Nodes[0];
                    return true;
                }

                return false;
            }

            RadTreeNode node = position.PrevVisibleNode;
            if (node != null)
            {
                position = node;
                return true;
            }

            if (position != null)
            {
                position = null;
                return true;
            }

            return false;
        }

        public bool MoveToEnd()
        {
            if (this.owner.Nodes.Count > 0)
            {
                int index = this.owner.Nodes.Count - 1;

                while (index >= 0)
                {
                    this.position = this.owner.Nodes[index];

                    if (this.OnTraversing())
                    {
                        return true;
                    }

                    index--;
                }

                return false;
            }

            return false;
        }

        #endregion

        #region IEnumerator<RadTreeNode> Members

        public RadTreeNode Current
        {
            get
            {
                return this.position;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //this.owner = null;
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public bool MoveNext()
        {
            while (this.MoveNextCore())
            {
                if (this.OnTraversing())
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual bool MoveNextCore()
        {
            if (position == null)
            {
                if (this.owner.Nodes.Count > 0)
                {
                    position = this.owner.Nodes[0];
                    //this.enum2 = new TreeViewEnumerator(position);
                    return true;
                }

                return false;
            }

            RadTreeNode node = position.NextVisibleNode;
            //RadTreeNode node = null;
            //if (this.enum2.MoveNext())
            //{
            //    node = this.enum2.Current;
            //}

            if (node != null)
            {
                position = node;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            this.position = null;
        }

        #endregion

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            if (enumerator == null)
            {
                this.enumerator = new TreeViewTraverser(this.owner);
            }

            this.enumerator.Position = this.position;
            return enumerator;
        }

        #endregion

        #region Methods

        public bool MoveForward(RadTreeNode node)
        {
            do
            {
                if (this.Current == node)
                {
                    return true;
                }
            }
            while (this.MoveNext());

            return false;
        }

        public bool MoveBackward(RadTreeNode node)
        {
            do
            {
                if (this.Current == node)
                {
                    return true;
                }
            }
            while (this.MovePrevious());

            return false;
        }

        #endregion
    }
}
