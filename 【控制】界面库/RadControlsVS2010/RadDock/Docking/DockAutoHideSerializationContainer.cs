using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.UI.Docking;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a logical container of TabStrip instances that contain RadDock windows with "auto-hide" state
    /// </summary>
    public class DockAutoHideSerializationContainer
    {
        RadDock owner;

        private AutoHideGroupCollection leftAutoHideGroups;
        private AutoHideGroupCollection topAutoHideGroups;
        private AutoHideGroupCollection rightAutoHideGroups;
        private AutoHideGroupCollection bottomAutoHideGroups;

        #region constructor

        internal DockAutoHideSerializationContainer(RadDock owner)
        {
            this.owner = owner;
        }

        #endregion

        #region serializable properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AutoHideGroupCollection LeftAutoHideGroups
        {
            get
            {
                if (leftAutoHideGroups == null)
                {
                    leftAutoHideGroups = new AutoHideGroupCollection();
                }

                return this.leftAutoHideGroups;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AutoHideGroupCollection TopAutoHideGroups
        {
            get
            {
                if (topAutoHideGroups == null)
                {
                    this.topAutoHideGroups = new AutoHideGroupCollection();
                }

                return this.topAutoHideGroups;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AutoHideGroupCollection RightAutoHideGroups
        {
            get
            {
                if (rightAutoHideGroups == null)
                {
                    this.rightAutoHideGroups = new AutoHideGroupCollection();
                }
                return this.rightAutoHideGroups;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AutoHideGroupCollection BottomAutoHideGroups
        {
            get
            {
                if (bottomAutoHideGroups == null)
                {
                    this.bottomAutoHideGroups = new AutoHideGroupCollection();    
                }
                return this.bottomAutoHideGroups;
            }
        }

        internal void SynchAutoHideGroups()
        {
            this.ClearAutoHideGroups();

            this.PrepareAutoHideGroups(ref this.leftAutoHideGroups, this.owner.GetAutoHideGroups(AutoHidePosition.Left));
            this.PrepareAutoHideGroups(ref this.topAutoHideGroups, this.owner.GetAutoHideGroups(AutoHidePosition.Top));
            this.PrepareAutoHideGroups(ref this.rightAutoHideGroups, this.owner.GetAutoHideGroups(AutoHidePosition.Right));
            this.PrepareAutoHideGroups(ref this.bottomAutoHideGroups, this.owner.GetAutoHideGroups(AutoHidePosition.Bottom));
        }

        #endregion

        #region functionality used by RadDock

        internal void ClearAutoHideGroups()
        {
            DisposeGroupCollection(this.leftAutoHideGroups);
            DisposeGroupCollection(this.topAutoHideGroups);
            DisposeGroupCollection(this.rightAutoHideGroups);
            DisposeGroupCollection(this.bottomAutoHideGroups);

            this.leftAutoHideGroups = null;
            this.topAutoHideGroups = null;
            this.rightAutoHideGroups = null;
            this.bottomAutoHideGroups = null;
        }

		private void DisposeGroupCollection(AutoHideGroupCollection groups)
		{
            if (groups == null)
            {
                return;
            }

			foreach (AutoHideGroup group in groups)
			{
				foreach (DockWindow window in group.Windows)
				{
					window.Dispose();
				}
			}
		}

        private void PrepareAutoHideGroups(ref AutoHideGroupCollection autoHideGroupCollection, List<AutoHideGroup> groups)
        {
            if (autoHideGroupCollection == null)
            {
                autoHideGroupCollection = new AutoHideGroupCollection();
            }

            autoHideGroupCollection.Clear();
            foreach (AutoHideGroup group in groups)
            {
                AutoHideGroup serializableGroup = new AutoHideGroup();
                foreach (DockWindow window in group.Windows)
                {
                    DockWindowPlaceholder placeholder = new DockWindowPlaceholder(window);
                    serializableGroup.Windows.Add(placeholder);
                }
                autoHideGroupCollection.Add(serializableGroup);
            }
        }

        internal void LoadDeserializedWindows()
        {
            foreach (AutoHideGroupCollection autoHideGroupCollection in new AutoHideGroupCollection[] 
                {
                    this.leftAutoHideGroups,
                    this.topAutoHideGroups,
                    this.rightAutoHideGroups,
                    this.bottomAutoHideGroups
                })
            {
                RestoreAutoHideState(autoHideGroupCollection);
            }
        }

        private void RestoreAutoHideState(AutoHideGroupCollection autoHideGroupCollection)
        {
            if (autoHideGroupCollection == null)
            {
                return;
            }

            foreach (AutoHideGroup group in autoHideGroupCollection)
            {
                foreach (DockWindow window in group.Windows)
                {
                    DockWindowPlaceholder placeholder = window as DockWindowPlaceholder;
                    if (placeholder == null)
                    {
                        continue;
                    }

                    DockWindow actualWindow = owner[placeholder.DockWindowName];
                    if (actualWindow == null)
                    {
                        continue;
                    }

                    owner.AutoHideWindow(actualWindow);
                }
            }
        }

        #endregion
    }
}
