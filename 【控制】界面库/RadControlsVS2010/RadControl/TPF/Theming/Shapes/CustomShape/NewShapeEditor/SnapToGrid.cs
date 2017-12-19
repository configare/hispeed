using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    public class SnapToGrid
    {
        #region Data

        public enum SnapTypes
        {
            Relative = 0,
            Fixed,
        };

        // Width of a single box in the snap grid
        private float fieldWidth;
        // the delta to activate grid snap in pixels
        private float deltaSnap;
        // the delta to activate grid snap in (range 0 to 1)
        private float coefSnap;
        // precached value of the snap, calculated on setting snapType, coefSnap and deltaSnap
        private float cachedSnap;

        private PointF snapPoint;

        private SnapTypes snapType;

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor sets the following default values:
        ///     FieldWidth = 1.0f;
        ///     SnapRelative = 0.2f;
        ///     SnapDelta = 0.2f;
        ///     SnapType = SnapTypes.Relative;
        /// </summary>
        public SnapToGrid()
        {
            fieldWidth = 1f;
            coefSnap = 0.2f;
            deltaSnap = fieldWidth * coefSnap;
            snapType = SnapTypes.Relative;
            snapPoint = new PointF();
        }

        public SnapToGrid(ref SnapToGrid e)
        {
            fieldWidth = e.fieldWidth;
            coefSnap = e.coefSnap;
            deltaSnap = e.deltaSnap;
            snapType = e.snapType;
            snapPoint = e.snapPoint;
        }
        #endregion

        #region Accessors

        /// <summary>
        /// Set the snap type to be one of the following:
        ///     SnapTypes.Relative - snap distance is relative to the FieldWidth
        /// <see cref="SnapFixed"/>
        ///     SnapTypes.Fixed    - snap distance is fixed
        /// </summary>
        public SnapTypes SnapType
        {
            get { return snapType; }
            set
            {
                snapType = value;
                CacheSnap();
            }
        }
        /// <summary>
        /// Width of a single box in the snap grid.
        /// It's value cannot be less than or equal to zero.
        /// </summary>
        public float FieldWidth
        {
            get { return fieldWidth; }
            set
            {
                if (value <= 0) return;
                fieldWidth = value;
                CacheSnap();
            }
        }
        /// <summary>
        /// Sets/Gets the snap distance for fixed type snapping. 
        /// Does not activate fixed type snapping.
        /// <see cref="SnapType"/>
        /// </summary>
        public float SnapFixed
        {
            get { return deltaSnap; }
            set
            {
                deltaSnap = value;
                CacheSnap();
            }
        }
        /// <summary>
        /// Sets/Gets the relative snap distance.
        /// Does not activate relative type snapping.
        /// <see cref="SnapType"/>
        /// </summary>
        public float SnapRelative
        {
            get { return coefSnap; }
            set
            {
                coefSnap = value;
                CacheSnap();
            }
        }
        /// <summary>
        /// Gets the precached snap distance.
        /// Doesn't need to be equal to any of the SnapFixed or SnapRelative properties. 
        /// </summary>
        public float CachedSnap
        {
            get { return cachedSnap; }
        }

        public PointF SnappedPoint
        {
            get { return snapPoint; }
        }

        #endregion

        #region Methods

        private void CacheSnap()
        {
            switch (snapType)
            {
                case SnapTypes.Fixed:
                    cachedSnap = deltaSnap / fieldWidth;
                    if (cachedSnap > 0.5f) cachedSnap = 0.5f;
                    break;

                default:
                case SnapTypes.Relative:
                    cachedSnap = coefSnap;
                    if (cachedSnap > 0.5f)
                    {
                        coefSnap = cachedSnap = 0.5f;
                    }
                    break;

            };

        }

        // Snaps point to the grid if close enough to a grid line
        public int SnapPtToGrid(PointF pos)
        {
            int snapped = 0;

            PointF grid = new PointF((int)(pos.X / fieldWidth), (int)(pos.Y / fieldWidth));
            PointF delta = new PointF((float)(pos.X % fieldWidth) / fieldWidth, (float)(pos.Y % fieldWidth) / fieldWidth);

            if (Math.Abs(delta.X) < cachedSnap)
            {
                snapped |= 2;
                delta.X = 0;
            }
            else
                if (Math.Abs(delta.X) > 1f - cachedSnap)
                {
                    snapped |= 2;
                    delta.X = Math.Sign(delta.X);
                }

            if (Math.Abs(delta.Y) < cachedSnap)
            {
                snapped |= 1;
                delta.Y = 0;
            }
            else
                if (Math.Abs(delta.Y) > 1f - cachedSnap)
                {
                    snapped |= 1;
                    delta.Y = Math.Sign(delta.Y);
                }

            grid.X += delta.X;
            grid.Y += delta.Y;

            //snapPoint.X = (int)Math.Round(grid.X * fieldWidth);
            //snapPoint.Y = (int)Math.Round(grid.Y * fieldWidth);

            snapPoint.X = grid.X * fieldWidth;
            snapPoint.Y = grid.Y * fieldWidth;

            return snapped;// new Point((int)(grid.X * fieldWidth), (int)(grid.Y * fieldWidth));
        }

        #endregion
    }
}
