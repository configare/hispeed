using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.Themes.Design
{
    /// <summary>
    /// A helper class which defines the behavior of RadDock in the Visual Style Builder.
    /// </summary>
    public class RadDockThemeDesignerData : RadControlDesignTimeData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="previewSurface"></param>
        /// <returns></returns>
        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {

            //Docking requires the following stylesheet registrations

            //<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.RadDock" 
            //                       ElementType="Telerik.WinControls.RootRadElement" />

            //<RadStylesheetRelation ControlType="Telerik.WinControls.UI.RadSplitContainer" 
            //                       ElementType="Telerik.WinControls.RootRadElement" />            

            //<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.DocumentTabStrip" 
            //                       ElementType="Telerik.WinControls.RootRadElement" />

            //<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.ToolTabStrip" 
            //                       ElementType="Telerik.WinControls.RootRadElement" />

            //<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.ToolTabStrip" 
            //                       ElementType="Telerik.WinControls.UI.RadTabStripElement"  />

            //<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.DocumentTabStrip" 
            //                       ElementType="Telerik.WinControls.UI.RadTabStripElement"  />

            //<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.ToolTabStrip" 
            //                       ElementType="Telerik.WinControls.UI.RadTitleBarElement" />

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            RadDock dock = new RadDock();
            dock.Size = new Size(400, 400);

            ToolWindow window = new ToolWindow("Tool Window 2");
            dock.DockWindow(window, DockPosition.Left);

            dock.DockWindow(new ToolWindow("Tool Window 3"), window, DockPosition.Fill);

            dock.DockWindow(new ToolWindow("Tool Window 1"), window, DockPosition.Left);            

			DocumentWindow documentWindow = new DocumentWindow("Document 1");
            dock.DockWindow(documentWindow, DockPosition.Fill);
            dock.DockWindow(new DocumentWindow("Document 2"), DockPosition.Fill);

            dock.DockWindow(window, DockPosition.Bottom);
            dock.DockWindow(new ToolWindow("Tool Window 4"), window, DockPosition.Fill);

            //Todo Add parameter to ControlStyleBuilderInfo() to specify which is the control that should be managed on the design surface 

            //ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(gridViewPreview, gridRow);
            //designed.StylePreviewRoot = previewTableBodyElement.Children[previewRowIndex];
            //designed.MainElementClassName = gridRow.GetType().FullName;
            //designed.Registration = new XmlStyleBuilderRegistration(null, typeof(RadGridView).FullName, gridRow.GetType().FullName);
            //designed.StyleDispatcher = (IStyleDispatcher)gridViewPreview.GridElement;

            //<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.RadDock" 
            //                       ElementType="Telerik.WinControls.RootRadElement" />
            ControlStyleBuilderInfo mainDockInfo = new ControlStyleBuilderInfo(dock, dock.RootElement);
            res.Add(mainDockInfo);

            //<RadStylesheetRelation ControlType="Telerik.WinControls.UI.RadSplitContainer" 
            //                       ElementType="Telerik.WinControls.RootRadElement" />    
            RadSplitContainer container = (RadSplitContainer)dock.SplitPanels[0];
            ControlStyleBuilderInfo info = new ControlStyleBuilderInfo(container, container.RootElement);
            info.StylePreviewRoot = container.RootElement;
            info.MainElementClassName = container.RootElement.GetThemeEffectiveType().FullName;
            //info.Registration = new XmlStyleBuilderRegistration(null, typeof(RadGridView).FullName, gridRow.GetType().FullName);
            //designed.StyleDispatcher = (IStyleDispatcher)gridViewPreview.GridElement;
            res.Add(info);

            //<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.DocumentTabStrip" 
            //                       ElementType="Telerik.WinControls.RootRadElement" />
            DocumentTabStrip documentStrip = (DocumentTabStrip)documentWindow.TabStrip;
            info = new ControlStyleBuilderInfo(documentStrip, documentStrip.RootElement);
            info.StylePreviewRoot = documentStrip.RootElement;
            res.Add(info);

            //<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.ToolTabStrip" 
            //ElementType="Telerik.WinControls.RootRadElement" />
            info = new ControlStyleBuilderInfo(window.TabStrip, window.TabStrip.RootElement);
            info.StylePreviewRoot = window.TabStrip.RootElement;
            res.Add(info);

            ////<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.ToolTabStrip" 
            ////                       ElementType="Telerik.WinControls.UI.RadTabStripElement"  />
            //info = new ControlStyleBuilderInfo(window.TabStrip, window.TabStrip.TabStripElement);
            //info.StylePreviewRoot = window.TabStrip.TabStripElement;
            //res.Add(info);

            ////<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.DocumentTabStrip" 
            ////                       ElementType="Telerik.WinControls.UI.RadTabStripElement"  />
            //info = new ControlStyleBuilderInfo(documentStrip, documentStrip.TabStripElement);
            //info.StylePreviewRoot = documentStrip.TabStripElement;
            //res.Add(info);

            ////<RadStylesheetRelation ControlType="Telerik.WinControls.UI.Docking.ToolTabStrip" 
            ////                       ElementType="Telerik.WinControls.UI.RadTitleBarElement" />
            //RadElement captionElement = ((ToolTabStrip)window.TabStrip).CaptionElement;
            //info = new ControlStyleBuilderInfo(window.TabStrip, captionElement);
            //info.StylePreviewRoot = captionElement;
            //res.Add(info);

            return res;
        }
    }
}
