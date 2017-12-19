using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    public class PreviewDragHintEventArgs : RadDragEventArgs
    {
        #region Fields

        private Image dragHint;
        private bool useDefaultHint;

        #endregion

        #region Constructor

        public PreviewDragHintEventArgs(ISupportDrag dragInstance)
            : base(dragInstance)
        {
            this.useDefaultHint = true;
        }

        #endregion

        #region Properties

        public Image DragHint
        {
            get
            {
                return this.dragHint;
            }
            set
            {
                this.dragHint = value;
            }
        }

        /// <summary>
        /// Determines whether a default hint will be generated. Usually this is a snapshot of the dragged item.
        /// </summary>
        public bool UseDefaultHint
        {
            get
            {
                return this.useDefaultHint;
            }
            set
            {
                this.useDefaultHint = value;
            }
        }

        #endregion
    }
}
