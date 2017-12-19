using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public class RadDragDropFeedbackEventArgs : RadDragEventArgs
    {
        #region Fields

        private Image previewImage;
        private Cursor cursor;

        #endregion

        #region Constructor

        public RadDragDropFeedbackEventArgs(ISupportDrag dragInstance)
            : base(dragInstance)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the image to be used as a preview while dragging.
        /// </summary>
        public Image PreviewImage
        {
            get
            {
                return this.previewImage;
            }
            set
            {
                this.previewImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the cursor to be used while dragging.
        /// </summary>
        public Cursor Cursor
        {
            get
            {
                return this.cursor;
            }
            set
            {
                this.cursor = value;
            }
        }

        #endregion
    }
}
