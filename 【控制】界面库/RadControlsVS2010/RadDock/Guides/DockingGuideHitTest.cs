using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Contains the hit-test information of a DragDropService.
    /// </summary>
    public struct DockingGuideHitTest
    {
        /// <summary>
        /// Initializes new instances of the <see cref="DockingGuideHitTest">DockingGuideHitTest</see> class, using the provided parameters.
        /// </summary>
        /// <param name="dockPos"></param>
        /// <param name="guidePos"></param>
        public DockingGuideHitTest(DockPosition? dockPos, DockingGuidesPosition? guidePos)
        {
            this.DockPosition = dockPos;
            this.GuidePosition = guidePos;
        }

        /// <summary>
        /// Determines whether the specified two <see cref="DockingGuideHitTest">DockingGuideHitTest</see> instances are equal.
        /// </summary>
        /// <param name="ht1"></param>
        /// <param name="ht2"></param>
        /// <returns></returns>
        public static bool operator ==(DockingGuideHitTest ht1, DockingGuideHitTest ht2)
        {
            return ht1.DockPosition == ht2.DockPosition &&
                    ht1.GuidePosition == ht2.GuidePosition;
        }

        /// <summary>
        /// Determines whether the specified two <see cref="DockingGuideHitTest">DockingGuideHitTest</see> instances are not equal.
        /// </summary>
        /// <param name="ht1"></param>
        /// <param name="ht2"></param>
        /// <returns></returns>
        public static bool operator !=(DockingGuideHitTest ht1, DockingGuideHitTest ht2)
        {
            return !(ht1 == ht2);
        }

        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return this.DockPosition == null && this.GuidePosition == null;
        }

        /// <summary>
        /// A nullable member that defines the current <see cref="DockPosition">DockPosition</see> of the hit-test operation.
        /// </summary>
        public DockPosition? DockPosition;
        /// <summary>
        /// A nullable member that defines the current <see cref="DockingGuidesPosition">DockingGuidesPosition</see> of the hit-test operation.
        /// </summary>
        public DockingGuidesPosition? GuidePosition;

        /// <summary>
        /// The default empty instance.
        /// </summary>
        public static readonly DockingGuideHitTest Empty = new DockingGuideHitTest(null, null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public override int GetHashCode()
		{
			return this.DockPosition.GetHashCode() ^ this.GuidePosition.GetHashCode();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public override bool Equals(object obj)
		{
			if (!(obj is DockingGuideHitTest))
			{
				return false;
			}
			DockingGuideHitTest other = (DockingGuideHitTest)obj;
			return this.GuidePosition == other.GuidePosition && this.DockPosition == other.DockPosition;
		}
    }
}
