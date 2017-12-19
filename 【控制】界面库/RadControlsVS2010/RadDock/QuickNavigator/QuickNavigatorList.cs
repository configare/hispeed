using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.Drawing;
using Telerik.WinControls.Layout;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a dock layout panel, which hosts a header plus a scrollable list with items.
    /// </summary>
    internal class QuickNavigatorList : DockLayoutPanel
    {
        #region Constructor

        public QuickNavigatorList()
        {            
        }

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            this.LastChildFill = true;
            this.BorderThickness = Padding.Empty;

            this.header = new RadLabelElement();
            this.header.Margin = new Padding(0, 0, 0, 4);
            DockLayoutPanel.SetDock(this.header, Telerik.WinControls.Layouts.Dock.Top);
            this.Children.Add(this.header);

            this.viewer = new RadScrollViewer();
            this.viewer.BorderThickness = Padding.Empty;
            this.viewer.ShowBorder = false;
            this.viewer.ShowFill = false;
            //we do not vertical scrollbar
            this.viewer.VerticalScrollState = ScrollState.AlwaysHide;

            this.itemsPanel = new WrapLayoutPanel();
            this.itemsPanel.Orientation = Orientation.Vertical;

            RadCanvasViewport viewport = new RadCanvasViewport();
            viewport.Children.Add(this.itemsPanel);
            this.viewer.Viewport = viewport;

            this.Children.Add(this.viewer);
        }

        #endregion

        #region Properties

        public RadLabelElement Header
        {
            get
            {
                return this.header;
            }
        }

        public WrapLayoutPanel ItemsPanel
        {
            get
            {
                return this.itemsPanel;
            }
        }

        #endregion

        #region Fields

        private RadScrollViewer viewer;
        private RadLabelElement header;
        private WrapLayoutPanel itemsPanel;

        #endregion
    }
}
