using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class MinMaxInfo
    {
        public MinMaxInfo()
        {
        }

        public MinMaxInfo(
            Size maxTrack,
            Size minTrack,
            Size maxSize,
            Point maxPosition,
            Size sizeReserved)
        {
            this.maxTrackSize = maxTrack;
            this.minTrackSize = minTrack;
            this.maxSize = maxSize;
            this.maxPosition = maxPosition;
            this.sizeReserved = sizeReserved;
        }

        private Size maxTrackSize;

        public Size MaxTrackSize
        {
            get { return maxTrackSize; }
            set { maxTrackSize = value; }
        }
        private Size minTrackSize;

        public Size MinTrackSize
        {
            get { return minTrackSize; }
            set { minTrackSize = value; }
        }
        private Size maxSize;

        public Size MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }
        private Point maxPosition;

        public Point MaxPosition
        {
            get { return maxPosition; }
            set { maxPosition = value; }
        }
        private Size sizeReserved;

        public Size SizeReserved
        {
            get { return sizeReserved; }
            set { sizeReserved = value; }
        }

    }
}
