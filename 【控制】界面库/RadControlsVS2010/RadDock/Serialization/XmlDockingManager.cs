using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Xml.Serialization;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
	public abstract class RadDockComponentFactory
	{
		public abstract ToolWindow CreateToolPane();
		public abstract DocumentWindow CreateDocumentPane();
		public abstract DocumentTabStrip CreateDocumentTabStrip();
		public abstract ToolTabStrip CreateToolTabStrip();
		public abstract RadSplitContainer CreateSplitContainer();
        public abstract DocumentContainer CreateDocumentContainer();
	}

	public class DesignTimeRadDockComponentFactory : RadDockComponentFactory
	{
		private IDesignerHost host = null;

		public DesignTimeRadDockComponentFactory(IDesignerHost host)
		{
			this.host = host;
		}

		private T CreateComponent<T>() where T : class
		{
			T component = null;
			if (this.host != null)
			{
				component = this.host.CreateComponent(typeof(T)) as T;
			}

			return component;
		}

		public override ToolWindow CreateToolPane()
		{
			return this.CreateComponent<ToolWindow>();
		}

		public override DocumentWindow CreateDocumentPane()
		{
			return this.CreateComponent<DocumentWindow>();
		}

		public override DocumentTabStrip CreateDocumentTabStrip()
		{
			return this.CreateComponent<DocumentTabStrip>();
		}

		public override ToolTabStrip CreateToolTabStrip()
		{
			return this.CreateComponent<ToolTabStrip>();
		}

		public override RadSplitContainer CreateSplitContainer()
		{
			return this.CreateComponent<RadSplitContainer>();
		}

        public override DocumentContainer CreateDocumentContainer()
        {
            return this.CreateComponent<DocumentContainer>();
        }
	}

	public class RunTimeRadDockComponentFactory : RadDockComponentFactory
	{
		public override ToolWindow CreateToolPane()
		{
			return new ToolWindow();
		}

		public override DocumentWindow CreateDocumentPane()
		{
			return new DocumentWindow();
		}

		public override DocumentTabStrip CreateDocumentTabStrip()
		{
			return new DocumentTabStrip();
		}

		public override ToolTabStrip CreateToolTabStrip()
		{
			return new ToolTabStrip();
		}

		public override RadSplitContainer CreateSplitContainer()
		{
			return new RadSplitContainer();
		}

        public override DocumentContainer CreateDocumentContainer()
        {
            return new DocumentContainer();
        }
	}


	///<exclude/>
    public class XmlDockable
    {
        [XmlAttribute("Text")]
        public string Text = "";

        [XmlAttribute("DockState")]
        public string DockState = "";

        [XmlAttribute("DockPosition")]
        public string DockPosition = "";

        [XmlAttribute("PreferredDockSize")]
        public string PreferredDockSize = "";

        [XmlAttribute("PreferredFloatSize")]
        public string PreferredFloatSize = "";

        [XmlAttribute("Guid")]
        public string DockGuid;

        [XmlAttribute("Type")]
        public string Type;

        [XmlAttribute("Assembly")]
        public string Assembly;

        [XmlAttribute("CaptionVisible")]
        public string CaptionVisible;

        [XmlAttribute("TabStripVisible")]
        public string TabStripVisible;

        [XmlAttribute("CloseButtonVisible")]
        public string CloseButtonVisible;

        [XmlAttribute("HideButtonVisible")]
        public string HideButtonVisible;

        [XmlAttribute("DropDownButtonVisible")]
        public string DropDownButtonVisible;

        public XmlDockable()
        {
        }

		public static DockState ParseDockState(string dockStateString)
		{
			return (DockState)Enum.Parse(typeof(DockState), dockStateString);
		}

		public static Size ParseSize(string sizeString)
		{
			string[] sizeComponents = sizeString.Split(';');
			int width = int.Parse(sizeComponents[0]);
			int height = int.Parse(sizeComponents[1]);

			return new Size(width, height);
		}

		private void ApplyDockWindowSettings(DockWindow dockWindow)
		{
			dockWindow.Text = this.Text;
			DockState dockState = XmlDockable.ParseDockState(this.DockState);
			if (dockState != Telerik.WinControls.UI.Docking.DockState.TabbedDocument)
			{
				dockWindow.DockState = dockState;
			}
			else
			{
				dockWindow.InnerDockState = dockState;
				dockWindow.DesiredDockState = dockState;
			}
			dockWindow.Size = XmlDockable.ParseSize(this.PreferredDockSize);
		}

		private const string DocumentPaneType = "Telerik.WinControls.Docking.DocumentWindow";
		private const string ToolPaneType = "Telerik.WinControls.Docking.DockPanel";

		private DockWindow CreateDockWindow(RadDockComponentFactory componentFactory)
		{
			DockWindow dockWindow = null;

			if (this.Type == XmlDockable.DocumentPaneType)
			{
				dockWindow = componentFactory.CreateDocumentPane();
			}
			else if (this.Type == XmlDockable.ToolPaneType)
			{
				dockWindow = componentFactory.CreateToolPane();
			}
			else
			{
				DockState dockState = XmlDockable.ParseDockState(this.DockState);
				switch (dockState)
				{
					case Docking.DockState.TabbedDocument:
						dockWindow = componentFactory.CreateDocumentPane();
						break;
					case Docking.DockState.Floating:
					case Docking.DockState.Docked:
					case Docking.DockState.AutoHide:
					case Docking.DockState.Hidden:
					default:
						dockWindow = componentFactory.CreateToolPane();
						break;
				}
			}

			return dockWindow;
		}

		private DockWindow GetDockWindow(RadDock dock, Guid guid, RadDockComponentFactory componentFactory)
		{
			DockWindow dockWindow = null;

			string name = dock.GuidToNameMappings.FindNameByGuid(guid);
			if (!string.IsNullOrEmpty(name))
			{
				dockWindow = dock[name];
			}

			if (dockWindow == null)
			{
				dockWindow = this.CreateDockWindow(componentFactory);
				XmlDockingManager.NewDockWindowsDictionary.Add(guid, dockWindow);
			}
			else
			{
				XmlDockingManager.OldContainers.Add(dockWindow.TabStrip);
			}

			return dockWindow;
		}

		public DockWindow Deserialize(RadDock dock, RadSplitContainer splitContainer, RadDockComponentFactory componentFactory)
        {
			Guid guid = new Guid(this.DockGuid);
			DockWindow dockWindow = this.GetDockWindow(dock, guid, componentFactory);
			if (dockWindow != null)
			{
				this.ApplyDockWindowSettings(dockWindow);
				if (dock != null)
				{
					dock.OnDockableDeserialized(new Telerik.WinControls.Interfaces.DockableDeserializedEventArgs(
						dockWindow, guid));
				}
			}
			

			return dockWindow;


			//(window as IToolWindowLayoutController).CaptionVisible = bool.Parse(CaptionVisible);
			//(window as IToolWindowLayoutController).TabStripVisible = bool.Parse(TabStripVisible);
			//(window as IToolWindowLayoutController).CloseButtonVisible = bool.Parse(CloseButtonVisible);
			//(window as IToolWindowLayoutController).HideButtonVisible = bool.Parse(HideButtonVisible);
			//(window as IToolWindowLayoutController).DropDownButtonVisible = bool.Parse(DropDownButtonVisible);

			//((ISupportDockingInternal)window).SetDockManager(manager);
			////host.CreateComponent

			//window.PreferredFloatSize = new Size(int.Parse(this.PreferredFloatSize.Split(';')[0]), int.Parse(this.PreferredFloatSize.Split(';')[1]));
			//window.DockPosition = (DockPosition)Enum.Parse(typeof(DockPosition), DockPosition);
        }
    }

	///<exclude/>
    public class XmlDockNode
    {
        [XmlAttribute("Orientation")]
        public string Orientation = "";

        [XmlAttribute("Size")]
        public string Size = "";

        [XmlAttribute("HidePosition")]
        public string HidePosition = "";

        [XmlArray()]
        public List<XmlDockable> Dockables = new List<XmlDockable>();

        [XmlElement("Left")]
        public XmlDockNode Left;

        [XmlElement("Rigth")]
        public XmlDockNode Right;

        public XmlDockNode()
        {
        }

		private DockState? GetFirstDockableState(List<XmlDockable> dockables)
		{
			DockState? dockState = null;
			if (dockables != null && dockables.Count > 0)
			{
				XmlDockable firstDockable = dockables[0];
				dockState = XmlDockable.ParseDockState(firstDockable.DockState);
			}

			return dockState;
		}

		private TabStripPanel CreateDockableContainer(List<XmlDockable> dockables, RadDockComponentFactory componentFactory)
		{
			DockState? dockState = this.GetFirstDockableState(dockables);
			if (dockState == null)
			{
				return null;
			}

			switch (dockState)
			{
				case DockState.TabbedDocument:
					return componentFactory.CreateDocumentTabStrip(); ;
				case DockState.Floating:
				case DockState.Docked:
				case DockState.AutoHide:
				case DockState.Hidden:
				default:
					return componentFactory.CreateToolTabStrip();
			}
		}

		public static bool HasBothChildren(XmlDockNode node)
		{
			return node.Left != null && node.Right != null;
		}

		public static System.Windows.Forms.Orientation GetReversedOrientation(XmlDockNode node)
		{
			Orientation orientation = (Orientation)Enum.Parse(typeof(Orientation), node.Orientation);
			if (orientation == System.Windows.Forms.Orientation.Horizontal)
			{
				return System.Windows.Forms.Orientation.Vertical;
			}
			return System.Windows.Forms.Orientation.Horizontal;
		}

        private void ProcessDockableContainer(TabStripPanel dockableContainer, RadDock dock, List<SplitPanel> splitPanelList, RadDockComponentFactory componentFactory)
		{
            DocumentTabStrip documentTabStrip = dockableContainer as DocumentTabStrip;
            if (documentTabStrip == null)
            {
                splitPanelList.Add(dockableContainer);
                return;
            }

			DocumentContainer documentContainer = this.CreateMainDocumentContainer(dock, componentFactory);
			if (documentContainer != null)
			{
				splitPanelList.Add(documentContainer);
			}

            dock.MainDocumentContainer.SplitPanels.Add(documentTabStrip);
		}

		private DocumentContainer CreateMainDocumentContainer(RadDock dock, RadDockComponentFactory componentFactory)
		{
			if (XmlDockingManager.MainDocumentTabStripAdded)
			{
				return null;
			}
			DocumentContainer documentContainer = componentFactory.CreateDocumentContainer();
			dock.MainDocumentContainer = documentContainer;
			XmlDockingManager.MainDocumentTabStripAdded = true;

			return documentContainer;
		}

		private RadSplitContainer CreateSplitContainer(RadDock dock, RadDockComponentFactory componentFactory)
		{
			bool bothChildrenAreDocumentTabStrips = 
				this.Left.IsDocumentTabStrip() && this.Right.IsDocumentTabStrip();
			if (bothChildrenAreDocumentTabStrips)
			{
				return this.CreateMainDocumentContainer(dock, componentFactory);
			}
			return componentFactory.CreateSplitContainer();
		}

		private bool IsDocumentTabStrip()
		{
			DockState? leftDockState = this.GetFirstDockableState(this.Dockables);
			if (leftDockState == null || leftDockState != DockState.TabbedDocument)
			{
				return false;
			}

			return true;
		}

		public List<SplitPanel> DeserializeNode(RadDock dock, RadSplitContainer splitContainer, RadDockComponentFactory componentFactory)
        {
			List<SplitPanel> splitPanelList = new List<SplitPanel>();

			TabStripPanel dockableContainer = this.CreateDockableContainer(this.Dockables, componentFactory);
			if (dockableContainer != null)
			{
				foreach (XmlDockable dockable in this.Dockables)
				{
					TabPanel panel = dockable.Deserialize(dock, splitContainer, componentFactory);
					dockableContainer.TabPanels.Add(panel);
				}
				this.ProcessDockableContainer(dockableContainer, dock, splitPanelList, componentFactory);
			}

			RadSplitContainer childSplitContainer = null;
			IList<SplitPanel> tempList = splitPanelList;

			if (XmlDockNode.HasBothChildren(this))
			{
				Orientation orientation = XmlDockNode.GetReversedOrientation(this);
				if (orientation != splitContainer.Orientation)
				{
					childSplitContainer = this.CreateSplitContainer(dock, componentFactory);
					if (childSplitContainer != null)
					{
						childSplitContainer.Orientation = orientation;
						splitPanelList.Add(childSplitContainer);
						tempList = childSplitContainer.SplitPanels; 
					}
				}
			}

			this.DeserializeChildNode(this.Left, splitContainer, childSplitContainer, tempList, componentFactory, dock);
			this.DeserializeChildNode(this.Right, splitContainer, childSplitContainer, tempList, componentFactory, dock);

			return splitPanelList;
        }

		private void DeserializeChildNode(XmlDockNode node, RadSplitContainer splitContainer, RadSplitContainer childSplitContainer,
			IList<SplitPanel> panelList, RadDockComponentFactory componentFactory, RadDock dock)
		{
			if (node != null)
			{
				RadSplitContainer container = (childSplitContainer != null) ? childSplitContainer : splitContainer;
				List<SplitPanel> splitPanels = node.DeserializeNode(dock, container, componentFactory);
				foreach (SplitPanel panel in splitPanels)
				{
					panelList.Add(panel);
				}
			}
		}
    }

	///<exclude/>
    public class XmlDockSite
    {
        [XmlElement("Sites")]
        public XmlDockNode RootNode;

        [XmlAttribute("Type")]
        public string Type;

        [XmlAttribute("Assembly")]
        public string Assembly;

        [XmlAttribute("FloatingLocation")]
        public string FloatingLocation;

        [XmlAttribute("Visible")]
        public string Visible;

        public XmlDockSite()
        {
        }

		public List<SplitPanel> Deserialize(RadDock dock, RadDockComponentFactory componentFactory)
        {
			List<SplitPanel> splitPanelList = this.RootNode.DeserializeNode(dock, dock, componentFactory);
			
			//TODO: take into account the floating position (if serialized in the XML)
			//if (FloatingLocation != null && site is DockSite)
			//{
			//    ((DockSite)site).Location = new Point(int.Parse(FloatingLocation.Split(';')[0]), int.Parse(FloatingLocation.Split(';')[1]));
			//}

			if (this.Visible != null)
			{
				bool visible = true;
				bool.TryParse(this.Visible, out visible);
				foreach (SplitPanel splitPanel in splitPanelList)
				{
					splitPanel.Collapsed = visible;
				}
			}

			return splitPanelList;
        }
    }

	///<exclude/>
    [XmlRoot("DockingTree")]
    public class XmlDockingManager
    {
		internal static bool MainDocumentTabStripAdded = false;
		internal static Dictionary<Guid, DockWindow> NewDockWindowsDictionary = null;
		internal static List<TabStripPanel> OldContainers = null;

        [XmlElement("Sites")]
        public List<XmlDockSite> Sites = new List<XmlDockSite>();

        public XmlDockingManager()
        {
        }

        public List<SplitPanel> Deserialize(RadDock dock, RadDockComponentFactory componentFactory)
        {
			MainDocumentTabStripAdded = false;
			NewDockWindowsDictionary = new Dictionary<Guid, DockWindow>();
			OldContainers = new List<TabStripPanel>();

			List<SplitPanel> list = new List<SplitPanel>();
			if (dock == null)
			{
				return list;
			}

			if (this.Sites != null)
			{
				int siteCount = this.Sites.Count;
				if (siteCount > 0)
				{
					XmlDockNode dockNode = this.Sites[0].RootNode;
					while (dockNode != null)
					{
						if (XmlDockNode.HasBothChildren(dockNode))
						{
							dock.Orientation = XmlDockNode.GetReversedOrientation(dockNode);
							break;
						}
						dockNode = dockNode.Left;
					}
				}

				for (int i = 0; i < siteCount; i++)
				{
					list.AddRange(this.Sites[i].Deserialize(dock, componentFactory));
				}
			}

			dock.SplitPanels.AddRange(list);

			this.ProcessNewDockWindowDictionary(dock);
			this.RemoveOldContainers(dock);

            return list;
        }

		private void RemoveOldContainers(RadDock dock)
		{
			foreach (TabStripPanel tabPanel in XmlDockingManager.OldContainers)
			{
				if (tabPanel != null && tabPanel.TabPanels.Count == 0)
				{
					RadSplitContainer splitContainer = tabPanel.SplitContainer;
					if(splitContainer != null)
					{
						splitContainer.SplitPanels.Remove(tabPanel);
						this.ClearEmptySplitContainer(splitContainer);
					}
				}
			}
			XmlDockingManager.OldContainers = null;
		}

		private void ClearEmptySplitContainer(RadSplitContainer splitContainer)
		{
			if (splitContainer.SplitPanels.Count == 0)
			{
			    RadSplitContainer parentSplitContainer = splitContainer.SplitContainer;
				if (parentSplitContainer != null)
				{
					parentSplitContainer.SplitPanels.Remove(splitContainer);
					this.ClearEmptySplitContainer(parentSplitContainer);
				}
			}
		}

		private void ProcessNewDockWindowDictionary(RadDock dock)
		{
			foreach (KeyValuePair<Guid, DockWindow> item in XmlDockingManager.NewDockWindowsDictionary)
			{
				dock.GuidToNameMappings[item.Key] = item.Value.Name;
			}
			XmlDockingManager.NewDockWindowsDictionary = null;
		}
	}
}
