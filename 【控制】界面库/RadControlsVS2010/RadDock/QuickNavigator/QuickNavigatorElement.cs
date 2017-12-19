using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Layout;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents the RadElement structure that is hosted on a DockQuickNavigator control.
    /// The element has the following structure:
    /// 1. Header
    /// 2. Dock Layout with three elements:
    ///    - Tool Window List
    ///    - Document List
    ///    - Preview
    /// 3. Footer
    /// </summary>
    internal class QuickNavigatorElement : DockLayoutPanel
    {
        #region Constructor

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.LastChildFill = true;
        }

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            this.Padding = new Padding(1);

            //create header
            this.header = new RadLabelElement();
            this.header.TextAlignment = ContentAlignment.MiddleLeft;
            this.header.Class = HeaderClass;
            this.header.borderPrimitive.Class = "QuickNavigatorHeaderBorder";
            this.header.LabelFill.Class = "QuickNavigatorHeaderFill";
            this.header.LabelText.Class = "QuickNavigatorHeaderText";

            DockLayoutPanel.SetDock(this.header, Telerik.WinControls.Layouts.Dock.Top);
            this.Children.Add(this.header);

            //create footer
            this.footer = new RadLabelElement();
            this.footer.Class = FooterClass;
            this.footer.borderPrimitive.Class = "QuickNavigatorFooterBorder";
            this.footer.LabelFill.Class = "QuickNavigatorFooterFill";
            this.footer.LabelText.Class = "QuickNavigatorFooterText";
            this.footer.TextAlignment = ContentAlignment.MiddleLeft;
            DockLayoutPanel.SetDock(this.footer, Telerik.WinControls.Layouts.Dock.Bottom);
            this.Children.Add(this.footer);

            //a strip layout panel that will hold tool windows, documents and preview
            this.listPanel = new DockLayoutPanel();
            this.listPanel.LastChildFill = true;
            this.listPanel.Margin = new Padding(4);

            //create tool window list
            this.listPanel.Children.Add(this.CreateToolPanesList());
            //create document window list
            this.listPanel.Children.Add(this.CreateWindowPanesList());

            //create the preview - a simple label
            this.preview = new RadLabelElement();
            this.preview.Class = PreviewClass;
            DockLayoutPanel.SetDock(this.preview, Telerik.WinControls.Layouts.Dock.Left);
            this.listPanel.Children.Add(this.preview);

            this.contentFill = new FillPrimitive();
            this.contentFill.Class = "QuickNavigatorContentFill";
            this.contentFill.Children.Add(this.listPanel);

            //add the list content as the last child of the main panel
            this.Children.Add(this.contentFill);
        }

        #endregion

        #region Implementation

        private RadElement CreateToolPanesList()
        {
            this.toolWindowList = new QuickNavigatorList();
            this.toolWindowList.Header.LabelText.Class = "QuickNavigatorToolWindowListHeader";
            this.toolWindowList.Header.Class = "QuickNavigatorToolWindowListHeaderLabel";
            DockLayoutPanel.SetDock(this.toolWindowList, Telerik.WinControls.Layouts.Dock.Left);
            return this.toolWindowList;
        }

        private RadElement CreateWindowPanesList()
        {
            this.documentWindowList = new QuickNavigatorList();
            this.documentWindowList.Header.LabelText.Class = "QuickNavigatorDocumentWindowListHeader";
            this.documentWindowList.Header.Class = "QuickNavigatorDocumentWindowListHeaderLabel";
            DockLayoutPanel.SetDock(this.documentWindowList, Telerik.WinControls.Layouts.Dock.Left);
            return this.documentWindowList;
        }

        #endregion

        #region Properties

        public QuickNavigatorList ToolWindowList
        {
            get
            {
                return this.toolWindowList;
            }
        }

        public QuickNavigatorList DocumentWindowList
        {
            get
            {
                return this.documentWindowList;
            }
        }

        public FillPrimitive ContentFill
        {
            get
            {
                return this.contentFill;
            }
        }

        public RadLabelElement Header
        {
            get
            {
                return this.header;
            }
        }

        public RadLabelElement Footer
        {
            get
            {
                return this.footer;
            }
        }

        public RadLabelElement Preview
        {
            get
            {
                return this.preview;
            }
        }


        #endregion

        #region Fields

        private DockLayoutPanel listPanel;
        private RadLabelElement header;
        private RadLabelElement footer;
        private RadLabelElement preview;
        private QuickNavigatorList toolWindowList;
        private QuickNavigatorList documentWindowList;
        private FillPrimitive contentFill;

        #endregion

        #region Constants

        public const string HeaderClass = "DockQuickNavigatorHeader";
        public const string FooterClass = "DockQuickNavigatorFooter";
        public const string PreviewClass = "DockQuickNavigatorPreview";

        #endregion
    }
}
