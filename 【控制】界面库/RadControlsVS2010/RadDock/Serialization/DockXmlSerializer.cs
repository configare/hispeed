using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls.XmlSerialization;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking.Serialization
{
	public class DockXmlSerializer : DockXmlSerializerBase
	{
		private bool prepareFloatingAndAutoHideWindows;

		public DockXmlSerializer(RadDock dock, ComponentXmlSerializationInfo componentSerializationInfo, bool prepareFloatingAndAutoHideWindows)
			: base(dock, componentSerializationInfo)
		{
			this.prepareFloatingAndAutoHideWindows = prepareFloatingAndAutoHideWindows;
		}

		public DockXmlSerializer(RadDock dock, ComponentXmlSerializationInfo componentSerializationInfo)
			: this(dock, componentSerializationInfo, true)
		{
		}

		protected override void InitializeRead()
		{
			this.Dock.DisposeSerializableFloatingWindows();
			this.Dock.SerializableAutoHideContainer.ClearAutoHideGroups();
		}

		protected override void InitializeWrite()
		{
			base.InitializeWrite();

			if (prepareFloatingAndAutoHideWindows)
			{
				this.Dock.SynchSerializableFloatingWindows();
				this.Dock.SerializableAutoHideContainer.SynchAutoHideGroups();
			}
			this.serializeDockWindowsWithoutRedockState = true;
			this.firstDockedTabStrip = this.GetFirstDockedTabStrip();
		}

		private DockTabStrip GetFirstDockedTabStrip()
		{
			DockWindow[] dockWindows = this.Dock.DockWindows.GetWindows(delegate(DockWindow window)
										{
											return window.DockState == DockState.Docked ||
												window.DockState == DockState.TabbedDocument;
										});
			if (dockWindows != null && dockWindows.Length > 0)
			{
				return dockWindows[0].DockTabStrip;
			}
			return null;
		}

		protected override bool ProcessListOverride(System.Xml.XmlReader reader, object listOwner, PropertyDescriptor ownerProperty, System.Collections.IList list)
		{
			Control.ControlCollection controlCollection = list as Control.ControlCollection;
			if (controlCollection != null)
			{
				base.ReadMergeCollection(reader, listOwner, ownerProperty, list, "Name", false, true);
				return true;
			}

			return base.ProcessListOverride(reader, listOwner, ownerProperty, list);
		}

		protected override void DisposeObject(IDisposable toBeDisposed)
		{
			//dispose only ToolWindows that are in designer, otherwise close the window
			if (toBeDisposed is ToolWindow && 
				!(toBeDisposed is DockWindowPlaceholder))
			{
				ToolWindow target = (ToolWindow)toBeDisposed;
				if (target.Site != null)
				{
					base.DisposeObject(target);
				}
				else
				{
					target.DockState = DockState.Hidden;
				}
			}

			//Save ToolWindowns from disposal in runtime
			Control targetControl = toBeDisposed as Control;
			if (targetControl != null)
			{
				foreach (DockWindow dockWindow in DockHelper.GetDockWindows(targetControl, true, this.Dock))
				{
					if (!(dockWindow is DockWindowPlaceholder) && dockWindow.Site == null)
					{
						dockWindow.DockState = DockState.Hidden;
						dockWindow.Parent = null;
					}
				}
			}

			base.DisposeObject(toBeDisposed);
		}

		private bool serializeDockWindowsWithoutRedockState = false;
		private DockTabStrip firstDockedTabStrip = null;

		protected override IEnumerable GetCollectionElementOverride(IEnumerable list, object owner, System.ComponentModel.PropertyDescriptor property)
		{
			RadDock dock = (RadDock)this.RootSerializationObject;

			RedockService service = dock.GetService<RedockService>(ServiceConstants.Redock);

			if (service == null)
			{
				return base.GetCollectionElementOverride(list, owner, property);
			}

			DockTabStrip tabStrip = owner as DockTabStrip;

			if (tabStrip == null)
			{
				return base.GetCollectionElementOverride(list, owner, property);
			}

			if (property == null || property.Name != "Controls")
			{
				return base.GetCollectionElementOverride(list, owner, property);
			}

			DockTabStrip.ControlCollection controls = list as DockTabStrip.ControlCollection;

			if (controls == null)
			{
				return base.GetCollectionElementOverride(list, owner, property);
			}            

			//Serialize FloatingWindows, in the same container as they where originally according to their redock state.
			//Later upon deserialization, floating windows state will be restored according to layout of (placehoder) windows
			//stored in SerializableFloatingWindows collection

			ArrayList res = new ArrayList(controls);
			
			foreach(DockWindow dockWindow in dock.DockWindows.GetWindows(delegate(DockWindow window) 
													{
														return window.DockState == DockState.Floating ||
															window.DockState == DockState.AutoHide ||
															window.DockState == DockState.Hidden; 
													}))
			{
				RedockState state = service.GetState(dockWindow, DockState.Docked, false);
				if (state != null)
				{
					if (state.TargetStrip == tabStrip)
					{
						res.Add(dockWindow);
					}	
				}
				else if (this.serializeDockWindowsWithoutRedockState)
				{
					if (this.firstDockedTabStrip == tabStrip)
					{
						res.Add(dockWindow);
					}
				}
			}
			if (this.firstDockedTabStrip == tabStrip)
			{
				this.serializeDockWindowsWithoutRedockState = false;
			}

			return res;
		}
	}
}
