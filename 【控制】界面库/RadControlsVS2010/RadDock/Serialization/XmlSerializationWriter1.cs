namespace Microsoft.Xml.Serialization.GeneratedAssembly
{
	//using System.Collections.Generic;
	//using System.Globalization;
	//using System.Xml;
	//using System.Xml.Serialization;
	//using Telerik.WinControls.Docking;
	//using Telerik.WinControls.Docking.Assistance;

	//internal class XmlSerializationWriter1 : XmlSerializationWriter
	//{
	//    protected override void InitCallbacks()
	//    {
	//    }

	//    private string Write1_DockingHintsType(DockingHintsType v)
	//    {
	//        switch (v)
	//        {
	//            case DockingHintsType.HalftoneFrame:
	//                return "HalftoneFrame";

	//            case DockingHintsType.ReversibleFrame:
	//                return "ReversibleFrame";

	//            case DockingHintsType.ReversibleRectangle:
	//                return "ReversibleRectangle";

	//            case DockingHintsType.TranslucentColorRectangle:
	//                return "TranslucentColorRectangle";

	//            case DockingHintsType.TranslucentGhostRectangle:
	//                return "TranslucentGhostRectangle";
	//        }
	//        long num = (long)v;
	//        throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "Telerik.WinControls.Docking.DockingHintsType");
	//    }

	//    private void Write10_DragManager(string n, string ns, DragManager o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(DragManager)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("DragManager", "");
	//            }
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    private void Write11_XmlDockable(string n, string ns, XmlDockable o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(XmlDockable)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("XmlDockable", "");
	//            }
	//            base.WriteAttribute("Text", "", o.Text);
	//            base.WriteAttribute("DockState", "", o.DockState);
	//            base.WriteAttribute("DockPosition", "", o.DockPosition);
	//            base.WriteAttribute("PreferredDockSize", "", o.PreferredDockSize);
	//            base.WriteAttribute("PreferredFloatSize", "", o.PreferredFloatSize);
	//            base.WriteAttribute("Guid", "", o.DockGuid);
	//            base.WriteAttribute("Type", "", o.Type);
	//            base.WriteAttribute("Assembly", "", o.Assembly);
	//            base.WriteAttribute("CaptionVisible", "", o.CaptionVisible);
	//            base.WriteAttribute("TabStripVisible", "", o.TabStripVisible);
	//            base.WriteAttribute("CloseButtonVisible", "", o.CloseButtonVisible);
	//            base.WriteAttribute("HideButtonVisible", "", o.HideButtonVisible);
	//            base.WriteAttribute("DropDownButtonVisible", "", o.DropDownButtonVisible);
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    private void Write12_XmlDockNode(string n, string ns, XmlDockNode o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(XmlDockNode)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("XmlDockNode", "");
	//            }
	//            base.WriteAttribute("Orientation", "", o.Orientation);
	//            base.WriteAttribute("Size", "", o.Size);
	//            base.WriteAttribute("HidePosition", "", o.HidePosition);
	//            List<XmlDockable> dockables = o.Dockables;
	//            if (dockables != null)
	//            {
	//                base.WriteStartElement("Dockables", "", null, false);
	//                for (int i = 0; i < dockables.Count; i++)
	//                {
	//                    this.Write11_XmlDockable("XmlDockable", "", dockables[i], true, false);
	//                }
	//                base.WriteEndElement();
	//            }
	//            this.Write12_XmlDockNode("Left", "", o.Left, false, false);
	//            this.Write12_XmlDockNode("Rigth", "", o.Right, false, false);
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    private void Write13_XmlDockSite(string n, string ns, XmlDockSite o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(XmlDockSite)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("XmlDockSite", "");
	//            }
	//            base.WriteAttribute("Type", "", o.Type);
	//            base.WriteAttribute("Assembly", "", o.Assembly);
	//            base.WriteAttribute("FloatingLocation", "", o.FloatingLocation);
	//            base.WriteAttribute("Visible", "", o.Visible);
	//            this.Write12_XmlDockNode("Sites", "", o.RootNode, false, false);
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    private void Write14_XmlDockingManager(string n, string ns, XmlDockingManager o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(XmlDockingManager)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("XmlDockingManager", "");
	//            }
	//            List<XmlDockSite> sites = o.Sites;
	//            if (sites != null)
	//            {
	//                for (int i = 0; i < sites.Count; i++)
	//                {
	//                    this.Write13_XmlDockSite("Sites", "", sites[i], false, false);
	//                }
	//            }
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    private string Write15_TdiContextMenuMode(TdiContextMenuMode v)
	//    {
	//        switch (v)
	//        {
	//            case TdiContextMenuMode.TabContext:
	//                return "TabContext";

	//            case TdiContextMenuMode.DropContext:
	//                return "DropContext";
	//        }
	//        long num = (long)v;
	//        throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "Telerik.WinControls.Docking.TdiContextMenuMode");
	//    }

	//    private void Write18_DockingChangingEventArgs(string n, string ns, DockingChangingEventArgs o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(DockingChangingEventArgs)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("DockingChangingEventArgs", "");
	//            }
	//            base.WriteElementStringRaw("Cancel", "", XmlConvert.ToString(o.Cancel));
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    private string Write19_DockingGuidesPosition(DockingGuidesPosition v)
	//    {
	//        switch (v)
	//        {
	//            case DockingGuidesPosition.Center:
	//                return "Center";

	//            case DockingGuidesPosition.Default:
	//                return "Default";

	//            case DockingGuidesPosition.Left:
	//                return "Left";

	//            case DockingGuidesPosition.Top:
	//                return "Top";

	//            case DockingGuidesPosition.Right:
	//                return "Right";

	//            case DockingGuidesPosition.Bottom:
	//                return "Bottom";
	//        }
	//        long num = (long)v;
	//        throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "Telerik.WinControls.Docking.DockingGuidesPosition");
	//    }

	//    private string Write20_LayoutListChangeType(LayoutListChangeType v)
	//    {
	//        switch (v)
	//        {
	//            case LayoutListChangeType.Add:
	//                return "Add";

	//            case LayoutListChangeType.AddRange:
	//                return "AddRange";

	//            case LayoutListChangeType.Insert:
	//                return "Insert";

	//            case LayoutListChangeType.Remove:
	//                return "Remove";

	//            case LayoutListChangeType.RemoveAt:
	//                return "RemoveAt";

	//            case LayoutListChangeType.ActiveElement:
	//                return "ActiveElement";

	//            case LayoutListChangeType.Clear:
	//                return "Clear";
	//        }
	//        long num = (long)v;
	//        throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "Telerik.WinControls.Docking.LayoutListChangeType");
	//    }

	//    private string Write21_DocumentLayoutStyle(DocumentLayoutStyle v)
	//    {
	//        switch (v)
	//        {
	//            case DocumentLayoutStyle.TabbedDocument:
	//                return "TabbedDocument";

	//            case DocumentLayoutStyle.MuiltipleDocument:
	//                return "MuiltipleDocument";
	//        }
	//        long num = (long)v;
	//        throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "Telerik.WinControls.Docking.DocumentLayoutStyle");
	//    }

	//    private string Write22_ButtonType(CaptionButton.ButtonType v)
	//    {
	//        switch (v)
	//        {
	//            case CaptionButton.ButtonType.Close:
	//                return "Close";

	//            case CaptionButton.ButtonType.AutoHide:
	//                return "AutoHide";
	//        }
	//        long num = (long)v;
	//        throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "Telerik.WinControls.Docking.CaptionButton.ButtonType");
	//    }

	//    private void Write23_Item(string n, string ns, ContextMenuLocalizationSettings o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(ContextMenuLocalizationSettings)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("ContextMenuLocalizationSettings", "");
	//            }
	//            if (o.FloatingText != "Floating")
	//            {
	//                base.WriteElementString("FloatingText", "", o.FloatingText);
	//            }
	//            if (o.DockableText != "Dockable")
	//            {
	//                base.WriteElementString("DockableText", "", o.DockableText);
	//            }
	//            if (o.TabbedDocumentText != "Tabbed Document")
	//            {
	//                base.WriteElementString("TabbedDocumentText", "", o.TabbedDocumentText);
	//            }
	//            if (o.AutoHideText != "Auto Hide")
	//            {
	//                base.WriteElementString("AutoHideText", "", o.AutoHideText);
	//            }
	//            if (o.HideText != "Hide")
	//            {
	//                base.WriteElementString("HideText", "", o.HideText);
	//            }
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    private string Write24_DockingSiteType(DockingSiteType v)
	//    {
	//        switch (v)
	//        {
	//            case DockingSiteType.HostContainerSite:
	//                return "HostContainerSite";

	//            case DockingSiteType.FloatingContainerSite:
	//                return "FloatingContainerSite";

	//            case DockingSiteType.MDIRootSite:
	//                return "MDIRootSite";

	//            case DockingSiteType.Default:
	//                return "Default";
	//        }
	//        long num = (long)v;
	//        throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "Telerik.WinControls.Docking.DockingSiteType");
	//    }

	//    private void Write25_DockTabChangedEventArgs(string n, string ns, DockTabChangedEventArgs o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(DockTabChangedEventArgs)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("DockTabChangedEventArgs", "");
	//            }
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    private void Write26_DockTabChangingEventArgs(string n, string ns, DockTabChangingEventArgs o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(DockTabChangingEventArgs)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("DockTabChangingEventArgs", "");
	//            }
	//            base.WriteElementStringRaw("Cancel", "", XmlConvert.ToString(o.Cancel));
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    private void Write27_DockingChangedEventArgs(string n, string ns, DockingChangedEventArgs o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(DockingChangedEventArgs)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("DockingChangedEventArgs", "");
	//            }
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    public void Write28_DockingHintsType(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteEmptyTag("DockingHintsType", "");
	//        }
	//        else
	//        {
	//            base.WriteElementString("DockingHintsType", "", this.Write1_DockingHintsType((DockingHintsType)o));
	//        }
	//    }

	//    public void Write29_Item(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("RadDockableSelectorDesignTimeData", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write4_Item("RadDockableSelectorDesignTimeData", "", (RadDockableSelectorDesignTimeData)o, true, false);
	//        }
	//    }

	//    public void Write30_DockState(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteEmptyTag("DockState", "");
	//        }
	//        else
	//        {
	//            base.WriteElementString("DockState", "", this.Write5_DockState((DockState)o));
	//        }
	//    }

	//    public void Write31_Item(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("DocumentPresenterDesignTimeData", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write6_Item("DocumentPresenterDesignTimeData", "", (DocumentPresenterDesignTimeData)o, true, false);
	//        }
	//    }

	//    public void Write32_DockPosition(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteEmptyTag("DockPosition", "");
	//        }
	//        else
	//        {
	//            base.WriteElementString("DockPosition", "", this.Write7_DockPosition((DockPosition)o));
	//        }
	//    }

	//    public void Write33_DockPresenterDesignTimeData(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("DockPresenterDesignTimeData", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write8_DockPresenterDesignTimeData("DockPresenterDesignTimeData", "", (DockPresenterDesignTimeData)o, true, false);
	//        }
	//    }

	//    public void Write34_DockType(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteEmptyTag("DockType", "");
	//        }
	//        else
	//        {
	//            base.WriteElementString("DockType", "", this.Write9_DockType((DockType)o));
	//        }
	//    }

	//    public void Write35_DragManager(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("DragManager", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write10_DragManager("DragManager", "", (DragManager)o, true, false);
	//        }
	//    }

	//    public void Write36_XmlDockable(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("XmlDockable", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write11_XmlDockable("XmlDockable", "", (XmlDockable)o, true, false);
	//        }
	//    }

	//    public void Write37_XmlDockNode(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("XmlDockNode", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write12_XmlDockNode("XmlDockNode", "", (XmlDockNode)o, true, false);
	//        }
	//    }

	//    public void Write38_XmlDockSite(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("XmlDockSite", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write13_XmlDockSite("XmlDockSite", "", (XmlDockSite)o, true, false);
	//        }
	//    }

	//    public void Write39_DockingTree(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("DockingTree", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write14_XmlDockingManager("DockingTree", "", (XmlDockingManager)o, true, false);
	//        }
	//    }

	//    private void Write4_Item(string n, string ns, RadDockableSelectorDesignTimeData o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(RadDockableSelectorDesignTimeData)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("RadDockableSelectorDesignTimeData", "");
	//            }
	//            base.WriteElementString("ControlName", "", o.ControlName);
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    public void Write40_TdiContextMenuMode(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteEmptyTag("TdiContextMenuMode", "");
	//        }
	//        else
	//        {
	//            base.WriteElementString("TdiContextMenuMode", "", this.Write15_TdiContextMenuMode((TdiContextMenuMode)o));
	//        }
	//    }

	//    public void Write41_DockingChangingEventArgs(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("DockingChangingEventArgs", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write18_DockingChangingEventArgs("DockingChangingEventArgs", "", (DockingChangingEventArgs)o, true, false);
	//        }
	//    }

	//    public void Write42_DockingGuidesPosition(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteEmptyTag("DockingGuidesPosition", "");
	//        }
	//        else
	//        {
	//            base.WriteElementString("DockingGuidesPosition", "", this.Write19_DockingGuidesPosition((DockingGuidesPosition)o));
	//        }
	//    }

	//    public void Write43_LayoutListChangeType(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteEmptyTag("LayoutListChangeType", "");
	//        }
	//        else
	//        {
	//            base.WriteElementString("LayoutListChangeType", "", this.Write20_LayoutListChangeType((LayoutListChangeType)o));
	//        }
	//    }

	//    public void Write44_DocumentLayoutStyle(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteEmptyTag("DocumentLayoutStyle", "");
	//        }
	//        else
	//        {
	//            base.WriteElementString("DocumentLayoutStyle", "", this.Write21_DocumentLayoutStyle((DocumentLayoutStyle)o));
	//        }
	//    }

	//    public void Write45_ButtonType(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteEmptyTag("ButtonType", "");
	//        }
	//        else
	//        {
	//            base.WriteElementString("ButtonType", "", this.Write22_ButtonType((CaptionButton.ButtonType)o));
	//        }
	//    }

	//    public void Write46_Item(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("ContextMenuLocalizationSettings", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write23_Item("ContextMenuLocalizationSettings", "", (ContextMenuLocalizationSettings)o, true, false);
	//        }
	//    }

	//    public void Write47_DockingSiteType(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteEmptyTag("DockingSiteType", "");
	//        }
	//        else
	//        {
	//            base.WriteElementString("DockingSiteType", "", this.Write24_DockingSiteType((DockingSiteType)o));
	//        }
	//    }

	//    public void Write48_DockTabChangedEventArgs(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("DockTabChangedEventArgs", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write25_DockTabChangedEventArgs("DockTabChangedEventArgs", "", (DockTabChangedEventArgs)o, true, false);
	//        }
	//    }

	//    public void Write49_DockTabChangingEventArgs(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("DockTabChangingEventArgs", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write26_DockTabChangingEventArgs("DockTabChangingEventArgs", "", (DockTabChangingEventArgs)o, true, false);
	//        }
	//    }

	//    private string Write5_DockState(DockState v)
	//    {
	//        switch (v)
	//        {
	//            case DockState.Default:
	//                return "Default";

	//            case DockState.Floating:
	//                return "Floating";

	//            case DockState.Docked:
	//                return "Docked";

	//            case DockState.TabbedDocument:
	//                return "TabbedDocument";

	//            case DockState.AutoHide:
	//                return "AutoHide";

	//            case DockState.Hidden:
	//                return "Hidden";
	//        }
	//        return XmlSerializationWriter.FromEnum((long)v, new string[] { "Floating", "Docked", "TabbedDocument", "AutoHide", "Hidden", "Default" }, new long[] { 1L, 2L, 4L, 8L, 0x10L, 0L }, "Telerik.WinControls.Docking.DockState");
	//    }

	//    public void Write50_DockingChangedEventArgs(object o)
	//    {
	//        base.WriteStartDocument();
	//        if (o == null)
	//        {
	//            base.WriteNullTagLiteral("DockingChangedEventArgs", "");
	//        }
	//        else
	//        {
	//            base.TopLevelElement();
	//            this.Write27_DockingChangedEventArgs("DockingChangedEventArgs", "", (DockingChangedEventArgs)o, true, false);
	//        }
	//    }

	//    private void Write6_Item(string n, string ns, DocumentPresenterDesignTimeData o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(DocumentPresenterDesignTimeData)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("DocumentPresenterDesignTimeData", "");
	//            }
	//            base.WriteElementString("ControlName", "", o.ControlName);
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    private string Write7_DockPosition(DockPosition v)
	//    {
	//        switch (v)
	//        {
	//            case DockPosition.Fill:
	//                return "Fill";

	//            case DockPosition.Default:
	//                return "Default";

	//            case DockPosition.Left:
	//                return "Left";

	//            case DockPosition.Top:
	//                return "Top";

	//            case DockPosition.Right:
	//                return "Right";

	//            case DockPosition.Bottom:
	//                return "Bottom";
	//        }
	//        return XmlSerializationWriter.FromEnum((long)v, new string[] { "Left", "Top", "Right", "Bottom", "Fill", "Default" }, new long[] { 1L, 2L, 4L, 8L, 0x10L, 0x20L }, "Telerik.WinControls.Docking.DockPosition");
	//    }

	//    private void Write8_DockPresenterDesignTimeData(string n, string ns, DockPresenterDesignTimeData o, bool isNullable, bool needType)
	//    {
	//        if (o == null)
	//        {
	//            if (isNullable)
	//            {
	//                base.WriteNullTagLiteral(n, ns);
	//            }
	//        }
	//        else
	//        {
	//            if (!needType && (o.GetType() != typeof(DockPresenterDesignTimeData)))
	//            {
	//                throw base.CreateUnknownTypeException(o);
	//            }
	//            base.WriteStartElement(n, ns, o, false, null);
	//            if (needType)
	//            {
	//                base.WriteXsiType("DockPresenterDesignTimeData", "");
	//            }
	//            base.WriteElementString("ControlName", "", o.ControlName);
	//            base.WriteEndElement(o);
	//        }
	//    }

	//    private string Write9_DockType(DockType v)
	//    {
	//        switch (v)
	//        {
	//            case DockType.ToolWindow:
	//                return "ToolWindow";

	//            case DockType.Document:
	//                return "Document";
	//        }
	//        long num = (long)v;
	//        throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "Telerik.WinControls.Docking.DockType");
	//    }
	//}
}

