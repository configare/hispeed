using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class RadScrollablePanelControlCollection : Control.ControlCollection
    {
        #region Fields

        private RadScrollablePanel owner;

        #endregion

        #region Ctor

        public RadScrollablePanelControlCollection(RadScrollablePanel owner)
            : base(owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Methods

        public override void Add(Control value)
        {
            if (object.ReferenceEquals(value, this.owner.VerticalScrollbar)
                || object.ReferenceEquals(value, this.owner.HorizontalScrollbar)
                || object.ReferenceEquals(value, this.owner.container))
            {
                throw new InvalidOperationException("Control already added!");
            }

            this.owner.container.Controls.Add(value);
        }

        public override void Remove(Control value)
        {
            if (object.ReferenceEquals(value, this.owner.VerticalScrollbar)
                || object.ReferenceEquals(value, this.owner.HorizontalScrollbar)
                || object.ReferenceEquals(value, this.owner.container))
            {
                return;
            }

            this.owner.container.Controls.Remove(value);
        }

        public override void Clear()
        {
            this.owner.container.Controls.Clear();
        }

      
        internal void AddControlInternal(Control value)
        {
            base.Add(value);
        }

        #endregion

    }
}
