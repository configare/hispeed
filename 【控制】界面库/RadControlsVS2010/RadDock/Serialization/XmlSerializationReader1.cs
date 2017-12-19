namespace Microsoft.Xml.Serialization.GeneratedAssembly
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;
	using Telerik.WinControls.UI.Docking;

    internal class XmlSerializationReader1 : XmlSerializationReader
    {
        private Hashtable _DockPositionValues;
        private Hashtable _DockStateValues;
        private string id1_DockingHintsType;
        private string id10_XmlDockable;
        private string id11_XmlDockNode;
        private string id12_XmlDockSite;
        private string id13_DockingTree;
        private string id14_TdiContextMenuMode;
        private string id15_DockingChangingEventArgs;
        private string id16_DockingGuidesPosition;
        private string id17_LayoutListChangeType;
        private string id18_DocumentLayoutStyle;
        private string id19_ButtonType;
        private string id2_Item;
        private string id20_Item;
        private string id21_DockingSiteType;
        private string id22_DockTabChangedEventArgs;
        private string id23_DockTabChangingEventArgs;
        private string id24_DockingChangedEventArgs;
        private string id25_Cancel;
        private string id26_FloatingText;
        private string id27_DockableText;
        private string id28_TabbedDocumentText;
        private string id29_AutoHideText;
        private string id3_Item;
        private string id30_HideText;
        private string id31_XmlDockingManager;
        private string id32_Sites;
        private string id33_Type;
        private string id34_Assembly;
        private string id35_FloatingLocation;
        private string id36_Visible;
        private string id37_Orientation;
        private string id38_Size;
        private string id39_HidePosition;
        private string id4_DockState;
        private string id40_Dockables;
        private string id41_Left;
        private string id42_Rigth;
        private string id43_Text;
        private string id44_PreferredDockSize;
        private string id45_PreferredFloatSize;
        private string id46_Guid;
        private string id47_CaptionVisible;
        private string id48_TabStripVisible;
        private string id49_CloseButtonVisible;
        private string id5_Item;
        private string id50_HideButtonVisible;
        private string id51_DropDownButtonVisible;
        private string id52_ControlName;
        private string id6_DockPosition;
        private string id7_DockPresenterDesignTimeData;
        private string id8_DockType;
        private string id9_DragManager;

        protected override void InitCallbacks()
        {
        }

        protected override void InitIDs()
        {
            this.id44_PreferredDockSize = base.Reader.NameTable.Add("PreferredDockSize");
            this.id7_DockPresenterDesignTimeData = base.Reader.NameTable.Add("DockPresenterDesignTimeData");
            this.id2_Item = base.Reader.NameTable.Add("");
            this.id45_PreferredFloatSize = base.Reader.NameTable.Add("PreferredFloatSize");
            this.id36_Visible = base.Reader.NameTable.Add("Visible");
            this.id37_Orientation = base.Reader.NameTable.Add("Orientation");
            this.id50_HideButtonVisible = base.Reader.NameTable.Add("HideButtonVisible");
            this.id51_DropDownButtonVisible = base.Reader.NameTable.Add("DropDownButtonVisible");
            this.id1_DockingHintsType = base.Reader.NameTable.Add("DockingHintsType");
            this.id42_Rigth = base.Reader.NameTable.Add("Rigth");
            this.id48_TabStripVisible = base.Reader.NameTable.Add("TabStripVisible");
            this.id3_Item = base.Reader.NameTable.Add("RadDockableSelectorDesignTimeData");
            this.id16_DockingGuidesPosition = base.Reader.NameTable.Add("DockingGuidesPosition");
            this.id10_XmlDockable = base.Reader.NameTable.Add("XmlDockable");
            this.id4_DockState = base.Reader.NameTable.Add("DockState");
            this.id43_Text = base.Reader.NameTable.Add("Text");
            this.id47_CaptionVisible = base.Reader.NameTable.Add("CaptionVisible");
            this.id19_ButtonType = base.Reader.NameTable.Add("ButtonType");
            this.id26_FloatingText = base.Reader.NameTable.Add("FloatingText");
            this.id34_Assembly = base.Reader.NameTable.Add("Assembly");
            this.id27_DockableText = base.Reader.NameTable.Add("DockableText");
            this.id8_DockType = base.Reader.NameTable.Add("DockType");
            this.id22_DockTabChangedEventArgs = base.Reader.NameTable.Add("DockTabChangedEventArgs");
            this.id14_TdiContextMenuMode = base.Reader.NameTable.Add("TdiContextMenuMode");
            this.id9_DragManager = base.Reader.NameTable.Add("DragManager");
            this.id30_HideText = base.Reader.NameTable.Add("HideText");
            this.id52_ControlName = base.Reader.NameTable.Add("ControlName");
            this.id28_TabbedDocumentText = base.Reader.NameTable.Add("TabbedDocumentText");
            this.id17_LayoutListChangeType = base.Reader.NameTable.Add("LayoutListChangeType");
            this.id41_Left = base.Reader.NameTable.Add("Left");
            this.id29_AutoHideText = base.Reader.NameTable.Add("AutoHideText");
            this.id25_Cancel = base.Reader.NameTable.Add("Cancel");
            this.id18_DocumentLayoutStyle = base.Reader.NameTable.Add("DocumentLayoutStyle");
            this.id46_Guid = base.Reader.NameTable.Add("Guid");
            this.id33_Type = base.Reader.NameTable.Add("Type");
            this.id39_HidePosition = base.Reader.NameTable.Add("HidePosition");
            this.id20_Item = base.Reader.NameTable.Add("ContextMenuLocalizationSettings");
            this.id49_CloseButtonVisible = base.Reader.NameTable.Add("CloseButtonVisible");
            this.id24_DockingChangedEventArgs = base.Reader.NameTable.Add("DockingChangedEventArgs");
            this.id12_XmlDockSite = base.Reader.NameTable.Add("XmlDockSite");
            this.id15_DockingChangingEventArgs = base.Reader.NameTable.Add("DockingChangingEventArgs");
            this.id13_DockingTree = base.Reader.NameTable.Add("DockingTree");
            this.id21_DockingSiteType = base.Reader.NameTable.Add("DockingSiteType");
            this.id5_Item = base.Reader.NameTable.Add("DocumentPresenterDesignTimeData");
            this.id31_XmlDockingManager = base.Reader.NameTable.Add("XmlDockingManager");
            this.id6_DockPosition = base.Reader.NameTable.Add("DockPosition");
            this.id11_XmlDockNode = base.Reader.NameTable.Add("XmlDockNode");
            this.id32_Sites = base.Reader.NameTable.Add("Sites");
            this.id40_Dockables = base.Reader.NameTable.Add("Dockables");
            this.id35_FloatingLocation = base.Reader.NameTable.Add("FloatingLocation");
            this.id38_Size = base.Reader.NameTable.Add("Size");
            this.id23_DockTabChangingEventArgs = base.Reader.NameTable.Add("DockTabChangingEventArgs");
        }

		//private DockingHintsType Read1_DockingHintsType(string s)
		//{
		//    switch (s)
		//    {
		//        case "HalftoneFrame":
		//            return DockingHintsType.HalftoneFrame;

		//        case "ReversibleFrame":
		//            return DockingHintsType.ReversibleFrame;

		//        case "ReversibleRectangle":
		//            return DockingHintsType.ReversibleRectangle;

		//        case "TranslucentColorRectangle":
		//            return DockingHintsType.TranslucentColorRectangle;

		//        case "TranslucentGhostRectangle":
		//            return DockingHintsType.TranslucentGhostRectangle;
		//    }
		//    throw base.CreateUnknownConstantException(s, typeof(DockingHintsType));
		//}

		//private DragManager Read10_DragManager(bool isNullable, bool checkType)
		//{
		//    XmlQualifiedName type = checkType ? base.GetXsiType() : null;
		//    bool flag = false;
		//    if (isNullable)
		//    {
		//        flag = base.ReadNull();
		//    }
		//    if ((checkType && (type != null)) && ((type.Name != this.id9_DragManager) || (type.Namespace != this.id2_Item)))
		//    {
		//        throw base.CreateUnknownTypeException(type);
		//    }
		//    if (flag)
		//    {
		//        return null;
		//    }
		//    DragManager o = new DragManager();
		//    while (base.Reader.MoveToNextAttribute())
		//    {
		//        if (!base.IsXmlnsAttribute(base.Reader.Name))
		//        {
		//            base.UnknownNode(o);
		//        }
		//    }
		//    base.Reader.MoveToElement();
		//    if (base.Reader.IsEmptyElement)
		//    {
		//        base.Reader.Skip();
		//        return o;
		//    }
		//    base.Reader.ReadStartElement();
		//    base.Reader.MoveToContent();
		//    int whileIterations = 0;
		//    int readerCount = base.ReaderCount;
		//    while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
		//    {
		//        if (base.Reader.NodeType == XmlNodeType.Element)
		//        {
		//            base.UnknownNode(o, "");
		//        }
		//        else
		//        {
		//            base.UnknownNode(o, "");
		//        }
		//        base.Reader.MoveToContent();
		//        base.CheckReaderCount(ref whileIterations, ref readerCount);
		//    }
		//    base.ReadEndElement();
		//    return o;
		//}

        private XmlDockable Read11_XmlDockable(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id10_XmlDockable) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            XmlDockable o = new XmlDockable();
            bool[] flagArray = new bool[13];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[0] && (base.Reader.LocalName == this.id43_Text)) && (base.Reader.NamespaceURI == this.id2_Item))
                {
                    o.Text = base.Reader.Value;
                    flagArray[0] = true;
                }
                else
                {
                    if ((!flagArray[1] && (base.Reader.LocalName == this.id4_DockState)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.DockState = base.Reader.Value;
                        flagArray[1] = true;
                        continue;
                    }
                    if ((!flagArray[2] && (base.Reader.LocalName == this.id6_DockPosition)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.DockPosition = base.Reader.Value;
                        flagArray[2] = true;
                        continue;
                    }
                    if ((!flagArray[3] && (base.Reader.LocalName == this.id44_PreferredDockSize)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.PreferredDockSize = base.Reader.Value;
                        flagArray[3] = true;
                        continue;
                    }
                    if ((!flagArray[4] && (base.Reader.LocalName == this.id45_PreferredFloatSize)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.PreferredFloatSize = base.Reader.Value;
                        flagArray[4] = true;
                        continue;
                    }
                    if ((!flagArray[5] && (base.Reader.LocalName == this.id46_Guid)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.DockGuid = base.Reader.Value;
                        flagArray[5] = true;
                        continue;
                    }
                    if ((!flagArray[6] && (base.Reader.LocalName == this.id33_Type)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.Type = base.Reader.Value;
                        flagArray[6] = true;
                        continue;
                    }
                    if ((!flagArray[7] && (base.Reader.LocalName == this.id34_Assembly)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.Assembly = base.Reader.Value;
                        flagArray[7] = true;
                        continue;
                    }
                    if ((!flagArray[8] && (base.Reader.LocalName == this.id47_CaptionVisible)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.CaptionVisible = base.Reader.Value;
                        flagArray[8] = true;
                        continue;
                    }
                    if ((!flagArray[9] && (base.Reader.LocalName == this.id48_TabStripVisible)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.TabStripVisible = base.Reader.Value;
                        flagArray[9] = true;
                        continue;
                    }
                    if ((!flagArray[10] && (base.Reader.LocalName == this.id49_CloseButtonVisible)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.CloseButtonVisible = base.Reader.Value;
                        flagArray[10] = true;
                        continue;
                    }
                    if ((!flagArray[11] && (base.Reader.LocalName == this.id50_HideButtonVisible)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.HideButtonVisible = base.Reader.Value;
                        flagArray[11] = true;
                        continue;
                    }
                    if ((!flagArray[12] && (base.Reader.LocalName == this.id51_DropDownButtonVisible)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.DropDownButtonVisible = base.Reader.Value;
                        flagArray[12] = true;
                        continue;
                    }
                    if (!base.IsXmlnsAttribute(base.Reader.Name))
                    {
                        base.UnknownNode(o, ":Text, :DockState, :DockPosition, :PreferredDockSize, :PreferredFloatSize, :Guid, :Type, :Assembly, :CaptionVisible, :TabStripVisible, :CloseButtonVisible, :HideButtonVisible, :DropDownButtonVisible");
                    }
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    base.UnknownNode(o, "");
                }
                else
                {
                    base.UnknownNode(o, "");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        private XmlDockNode Read12_XmlDockNode(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id11_XmlDockNode) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            XmlDockNode o = new XmlDockNode();
            if (o.Dockables == null)
            {
                o.Dockables = new List<XmlDockable>();
            }
            List<XmlDockable> dockables = o.Dockables;
            bool[] flagArray = new bool[6];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[0] && (base.Reader.LocalName == this.id37_Orientation)) && (base.Reader.NamespaceURI == this.id2_Item))
                {
                    o.Orientation = base.Reader.Value;
                    flagArray[0] = true;
                }
                else
                {
                    if ((!flagArray[1] && (base.Reader.LocalName == this.id38_Size)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.Size = base.Reader.Value;
                        flagArray[1] = true;
                        continue;
                    }
                    if ((!flagArray[2] && (base.Reader.LocalName == this.id39_HidePosition)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.HidePosition = base.Reader.Value;
                        flagArray[2] = true;
                        continue;
                    }
                    if (!base.IsXmlnsAttribute(base.Reader.Name))
                    {
                        base.UnknownNode(o, ":Orientation, :Size, :HidePosition");
                    }
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    if ((base.Reader.LocalName == this.id40_Dockables) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        if (!base.ReadNull())
                        {
                            if (o.Dockables == null)
                            {
                                o.Dockables = new List<XmlDockable>();
                            }
                            List<XmlDockable> list = o.Dockables;
                            if (base.Reader.IsEmptyElement)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                base.Reader.ReadStartElement();
                                base.Reader.MoveToContent();
                                int num3 = 0;
                                int num4 = base.ReaderCount;
                                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                                {
                                    if (base.Reader.NodeType == XmlNodeType.Element)
                                    {
                                        if ((base.Reader.LocalName == this.id10_XmlDockable) && (base.Reader.NamespaceURI == this.id2_Item))
                                        {
                                            if (list == null)
                                            {
                                                base.Reader.Skip();
                                            }
                                            else
                                            {
                                                list.Add(this.Read11_XmlDockable(true, true));
                                            }
                                        }
                                        else
                                        {
                                            base.UnknownNode(null, ":XmlDockable");
                                        }
                                    }
                                    else
                                    {
                                        base.UnknownNode(null, ":XmlDockable");
                                    }
                                    base.Reader.MoveToContent();
                                    base.CheckReaderCount(ref num3, ref num4);
                                }
                                base.ReadEndElement();
                            }
                        }
                    }
                    else if ((!flagArray[4] && (base.Reader.LocalName == this.id41_Left)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.Left = this.Read12_XmlDockNode(false, true);
                        flagArray[4] = true;
                    }
                    else if ((!flagArray[5] && (base.Reader.LocalName == this.id42_Rigth)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.Right = this.Read12_XmlDockNode(false, true);
                        flagArray[5] = true;
                    }
                    else
                    {
                        base.UnknownNode(o, ":Dockables, :Left, :Rigth");
                    }
                }
                else
                {
                    base.UnknownNode(o, ":Dockables, :Left, :Rigth");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        private XmlDockSite Read13_XmlDockSite(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id12_XmlDockSite) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            XmlDockSite o = new XmlDockSite();
            bool[] flagArray = new bool[5];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[1] && (base.Reader.LocalName == this.id33_Type)) && (base.Reader.NamespaceURI == this.id2_Item))
                {
                    o.Type = base.Reader.Value;
                    flagArray[1] = true;
                }
                else
                {
                    if ((!flagArray[2] && (base.Reader.LocalName == this.id34_Assembly)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.Assembly = base.Reader.Value;
                        flagArray[2] = true;
                        continue;
                    }
                    if ((!flagArray[3] && (base.Reader.LocalName == this.id35_FloatingLocation)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.FloatingLocation = base.Reader.Value;
                        flagArray[3] = true;
                        continue;
                    }
                    if ((!flagArray[4] && (base.Reader.LocalName == this.id36_Visible)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.Visible = base.Reader.Value;
                        flagArray[4] = true;
                        continue;
                    }
                    if (!base.IsXmlnsAttribute(base.Reader.Name))
                    {
                        base.UnknownNode(o, ":Type, :Assembly, :FloatingLocation, :Visible");
                    }
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    if ((!flagArray[0] && (base.Reader.LocalName == this.id32_Sites)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.RootNode = this.Read12_XmlDockNode(false, true);
                        flagArray[0] = true;
                    }
                    else
                    {
                        base.UnknownNode(o, ":Sites");
                    }
                }
                else
                {
                    base.UnknownNode(o, ":Sites");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        private XmlDockingManager Read14_XmlDockingManager(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id31_XmlDockingManager) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            XmlDockingManager o = new XmlDockingManager();
            if (o.Sites == null)
            {
                o.Sites = new List<XmlDockSite>();
            }
            List<XmlDockSite> sites = o.Sites;
            while (base.Reader.MoveToNextAttribute())
            {
                if (!base.IsXmlnsAttribute(base.Reader.Name))
                {
                    base.UnknownNode(o);
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    if ((base.Reader.LocalName == this.id32_Sites) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        if (sites == null)
                        {
                            base.Reader.Skip();
                        }
                        else
                        {
                            sites.Add(this.Read13_XmlDockSite(false, true));
                        }
                    }
                    else
                    {
                        base.UnknownNode(o, ":Sites");
                    }
                }
                else
                {
                    base.UnknownNode(o, ":Sites");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        //private TdiContextMenuMode Read15_TdiContextMenuMode(string s)
        //{
        //    switch (s)
        //    {
        //        case "TabContext":
        //            return TdiContextMenuMode.TabContext;

        //        case "DropContext":
        //            return TdiContextMenuMode.DropContext;
        //    }
        //    throw base.CreateUnknownConstantException(s, typeof(TdiContextMenuMode));
        //}

		//private DockingChangingEventArgs Read18_DockingChangingEventArgs(bool isNullable, bool checkType)
		//{
		//    XmlQualifiedName type = checkType ? base.GetXsiType() : null;
		//    bool flag = false;
		//    if (isNullable)
		//    {
		//        flag = base.ReadNull();
		//    }
		//    if ((checkType && (type != null)) && ((type.Name != this.id15_DockingChangingEventArgs) || (type.Namespace != this.id2_Item)))
		//    {
		//        throw base.CreateUnknownTypeException(type);
		//    }
		//    if (flag)
		//    {
		//        return null;
		//    }
		//    DockingChangingEventArgs o = new DockingChangingEventArgs();
		//    bool[] flagArray = new bool[1];
		//    while (base.Reader.MoveToNextAttribute())
		//    {
		//        if (!base.IsXmlnsAttribute(base.Reader.Name))
		//        {
		//            base.UnknownNode(o);
		//        }
		//    }
		//    base.Reader.MoveToElement();
		//    if (base.Reader.IsEmptyElement)
		//    {
		//        base.Reader.Skip();
		//        return o;
		//    }
		//    base.Reader.ReadStartElement();
		//    base.Reader.MoveToContent();
		//    int whileIterations = 0;
		//    int readerCount = base.ReaderCount;
		//    while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
		//    {
		//        if (base.Reader.NodeType == XmlNodeType.Element)
		//        {
		//            if ((!flagArray[0] && (base.Reader.LocalName == this.id25_Cancel)) && (base.Reader.NamespaceURI == this.id2_Item))
		//            {
		//                o.Cancel = XmlConvert.ToBoolean(base.Reader.ReadElementString());
		//                flagArray[0] = true;
		//            }
		//            else
		//            {
		//                base.UnknownNode(o, ":Cancel");
		//            }
		//        }
		//        else
		//        {
		//            base.UnknownNode(o, ":Cancel");
		//        }
		//        base.Reader.MoveToContent();
		//        base.CheckReaderCount(ref whileIterations, ref readerCount);
		//    }
		//    base.ReadEndElement();
		//    return o;
		//}

		//private DockingGuidesPosition Read19_DockingGuidesPosition(string s)
		//{
		//    switch (s)
		//    {
		//        case "Left":
		//            return DockingGuidesPosition.Left;

		//        case "Top":
		//            return DockingGuidesPosition.Top;

		//        case "Right":
		//            return DockingGuidesPosition.Right;

		//        case "Bottom":
		//            return DockingGuidesPosition.Bottom;

		//        case "Center":
		//            return DockingGuidesPosition.Center;

		//        case "Default":
		//            return DockingGuidesPosition.Default;
		//    }
		//    throw base.CreateUnknownConstantException(s, typeof(DockingGuidesPosition));
		//}

		//private LayoutListChangeType Read20_LayoutListChangeType(string s)
		//{
		//    switch (s)
		//    {
		//        case "Add":
		//            return LayoutListChangeType.Add;

		//        case "AddRange":
		//            return LayoutListChangeType.AddRange;

		//        case "Insert":
		//            return LayoutListChangeType.Insert;

		//        case "Remove":
		//            return LayoutListChangeType.Remove;

		//        case "RemoveAt":
		//            return LayoutListChangeType.RemoveAt;

		//        case "ActiveElement":
		//            return LayoutListChangeType.ActiveElement;

		//        case "Clear":
		//            return LayoutListChangeType.Clear;
		//    }
		//    throw base.CreateUnknownConstantException(s, typeof(LayoutListChangeType));
		//}

		//private DocumentLayoutStyle Read21_DocumentLayoutStyle(string s)
		//{
		//    switch (s)
		//    {
		//        case "TabbedDocument":
		//            return DocumentLayoutStyle.TabbedDocument;

		//        case "MuiltipleDocument":
		//            return DocumentLayoutStyle.MuiltipleDocument;
		//    }
		//    throw base.CreateUnknownConstantException(s, typeof(DocumentLayoutStyle));
		//}

		//private CaptionButton.ButtonType Read22_ButtonType(string s)
		//{
		//    switch (s)
		//    {
		//        case "Close":
		//            return CaptionButton.ButtonType.Close;

		//        case "AutoHide":
		//            return CaptionButton.ButtonType.AutoHide;
		//    }
		//    throw base.CreateUnknownConstantException(s, typeof(CaptionButton.ButtonType));
		//}

		//private ContextMenuLocalizationSettings Read23_Item(bool isNullable, bool checkType)
		//{
		//    XmlQualifiedName type = checkType ? base.GetXsiType() : null;
		//    bool flag = false;
		//    if (isNullable)
		//    {
		//        flag = base.ReadNull();
		//    }
		//    if ((checkType && (type != null)) && ((type.Name != this.id20_Item) || (type.Namespace != this.id2_Item)))
		//    {
		//        throw base.CreateUnknownTypeException(type);
		//    }
		//    if (flag)
		//    {
		//        return null;
		//    }
		//    ContextMenuLocalizationSettings o = new ContextMenuLocalizationSettings();
		//    bool[] flagArray = new bool[5];
		//    while (base.Reader.MoveToNextAttribute())
		//    {
		//        if (!base.IsXmlnsAttribute(base.Reader.Name))
		//        {
		//            base.UnknownNode(o);
		//        }
		//    }
		//    base.Reader.MoveToElement();
		//    if (base.Reader.IsEmptyElement)
		//    {
		//        base.Reader.Skip();
		//        return o;
		//    }
		//    base.Reader.ReadStartElement();
		//    base.Reader.MoveToContent();
		//    int whileIterations = 0;
		//    int readerCount = base.ReaderCount;
		//    while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
		//    {
		//        if (base.Reader.NodeType == XmlNodeType.Element)
		//        {
		//            if ((!flagArray[0] && (base.Reader.LocalName == this.id26_FloatingText)) && (base.Reader.NamespaceURI == this.id2_Item))
		//            {
		//                o.FloatingText = base.Reader.ReadElementString();
		//                flagArray[0] = true;
		//            }
		//            else if ((!flagArray[1] && (base.Reader.LocalName == this.id27_DockableText)) && (base.Reader.NamespaceURI == this.id2_Item))
		//            {
		//                o.DockableText = base.Reader.ReadElementString();
		//                flagArray[1] = true;
		//            }
		//            else if ((!flagArray[2] && (base.Reader.LocalName == this.id28_TabbedDocumentText)) && (base.Reader.NamespaceURI == this.id2_Item))
		//            {
		//                o.TabbedDocumentText = base.Reader.ReadElementString();
		//                flagArray[2] = true;
		//            }
		//            else if ((!flagArray[3] && (base.Reader.LocalName == this.id29_AutoHideText)) && (base.Reader.NamespaceURI == this.id2_Item))
		//            {
		//                o.AutoHideText = base.Reader.ReadElementString();
		//                flagArray[3] = true;
		//            }
		//            else if ((!flagArray[4] && (base.Reader.LocalName == this.id30_HideText)) && (base.Reader.NamespaceURI == this.id2_Item))
		//            {
		//                o.HideText = base.Reader.ReadElementString();
		//                flagArray[4] = true;
		//            }
		//            else
		//            {
		//                base.UnknownNode(o, ":FloatingText, :DockableText, :TabbedDocumentText, :AutoHideText, :HideText");
		//            }
		//        }
		//        else
		//        {
		//            base.UnknownNode(o, ":FloatingText, :DockableText, :TabbedDocumentText, :AutoHideText, :HideText");
		//        }
		//        base.Reader.MoveToContent();
		//        base.CheckReaderCount(ref whileIterations, ref readerCount);
		//    }
		//    base.ReadEndElement();
		//    return o;
		//}

		//private DockingSiteType Read24_DockingSiteType(string s)
		//{
		//    switch (s)
		//    {
		//        case "HostContainerSite":
		//            return DockingSiteType.HostContainerSite;

		//        case "FloatingContainerSite":
		//            return DockingSiteType.FloatingContainerSite;

		//        case "MDIRootSite":
		//            return DockingSiteType.MDIRootSite;

		//        case "Default":
		//            return DockingSiteType.Default;
		//    }
		//    throw base.CreateUnknownConstantException(s, typeof(DockingSiteType));
		//}

		//private DockTabChangedEventArgs Read25_DockTabChangedEventArgs(bool isNullable, bool checkType)
		//{
		//    XmlQualifiedName type = checkType ? base.GetXsiType() : null;
		//    bool flag = false;
		//    if (isNullable)
		//    {
		//        flag = base.ReadNull();
		//    }
		//    if ((checkType && (type != null)) && ((type.Name != this.id22_DockTabChangedEventArgs) || (type.Namespace != this.id2_Item)))
		//    {
		//        throw base.CreateUnknownTypeException(type);
		//    }
		//    if (flag)
		//    {
		//        return null;
		//    }
		//    DockTabChangedEventArgs o = new DockTabChangedEventArgs();
		//    while (base.Reader.MoveToNextAttribute())
		//    {
		//        if (!base.IsXmlnsAttribute(base.Reader.Name))
		//        {
		//            base.UnknownNode(o);
		//        }
		//    }
		//    base.Reader.MoveToElement();
		//    if (base.Reader.IsEmptyElement)
		//    {
		//        base.Reader.Skip();
		//        return o;
		//    }
		//    base.Reader.ReadStartElement();
		//    base.Reader.MoveToContent();
		//    int whileIterations = 0;
		//    int readerCount = base.ReaderCount;
		//    while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
		//    {
		//        if (base.Reader.NodeType == XmlNodeType.Element)
		//        {
		//            base.UnknownNode(o, "");
		//        }
		//        else
		//        {
		//            base.UnknownNode(o, "");
		//        }
		//        base.Reader.MoveToContent();
		//        base.CheckReaderCount(ref whileIterations, ref readerCount);
		//    }
		//    base.ReadEndElement();
		//    return o;
		//}

		//private DockTabChangingEventArgs Read26_DockTabChangingEventArgs(bool isNullable, bool checkType)
		//{
		//    XmlQualifiedName type = checkType ? base.GetXsiType() : null;
		//    bool flag = false;
		//    if (isNullable)
		//    {
		//        flag = base.ReadNull();
		//    }
		//    if ((checkType && (type != null)) && ((type.Name != this.id23_DockTabChangingEventArgs) || (type.Namespace != this.id2_Item)))
		//    {
		//        throw base.CreateUnknownTypeException(type);
		//    }
		//    if (flag)
		//    {
		//        return null;
		//    }
		//    DockTabChangingEventArgs o = new DockTabChangingEventArgs();
		//    bool[] flagArray = new bool[1];
		//    while (base.Reader.MoveToNextAttribute())
		//    {
		//        if (!base.IsXmlnsAttribute(base.Reader.Name))
		//        {
		//            base.UnknownNode(o);
		//        }
		//    }
		//    base.Reader.MoveToElement();
		//    if (base.Reader.IsEmptyElement)
		//    {
		//        base.Reader.Skip();
		//        return o;
		//    }
		//    base.Reader.ReadStartElement();
		//    base.Reader.MoveToContent();
		//    int whileIterations = 0;
		//    int readerCount = base.ReaderCount;
		//    while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
		//    {
		//        if (base.Reader.NodeType == XmlNodeType.Element)
		//        {
		//            if ((!flagArray[0] && (base.Reader.LocalName == this.id25_Cancel)) && (base.Reader.NamespaceURI == this.id2_Item))
		//            {
		//                o.Cancel = XmlConvert.ToBoolean(base.Reader.ReadElementString());
		//                flagArray[0] = true;
		//            }
		//            else
		//            {
		//                base.UnknownNode(o, ":Cancel");
		//            }
		//        }
		//        else
		//        {
		//            base.UnknownNode(o, ":Cancel");
		//        }
		//        base.Reader.MoveToContent();
		//        base.CheckReaderCount(ref whileIterations, ref readerCount);
		//    }
		//    base.ReadEndElement();
		//    return o;
		//}

		//private DockingChangedEventArgs Read27_DockingChangedEventArgs(bool isNullable, bool checkType)
		//{
		//    XmlQualifiedName type = checkType ? base.GetXsiType() : null;
		//    bool flag = false;
		//    if (isNullable)
		//    {
		//        flag = base.ReadNull();
		//    }
		//    if ((checkType && (type != null)) && ((type.Name != this.id24_DockingChangedEventArgs) || (type.Namespace != this.id2_Item)))
		//    {
		//        throw base.CreateUnknownTypeException(type);
		//    }
		//    if (flag)
		//    {
		//        return null;
		//    }
		//    DockingChangedEventArgs o = new DockingChangedEventArgs();
		//    while (base.Reader.MoveToNextAttribute())
		//    {
		//        if (!base.IsXmlnsAttribute(base.Reader.Name))
		//        {
		//            base.UnknownNode(o);
		//        }
		//    }
		//    base.Reader.MoveToElement();
		//    if (base.Reader.IsEmptyElement)
		//    {
		//        base.Reader.Skip();
		//        return o;
		//    }
		//    base.Reader.ReadStartElement();
		//    base.Reader.MoveToContent();
		//    int whileIterations = 0;
		//    int readerCount = base.ReaderCount;
		//    while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
		//    {
		//        if (base.Reader.NodeType == XmlNodeType.Element)
		//        {
		//            base.UnknownNode(o, "");
		//        }
		//        else
		//        {
		//            base.UnknownNode(o, "");
		//        }
		//        base.Reader.MoveToContent();
		//        base.CheckReaderCount(ref whileIterations, ref readerCount);
		//    }
		//    base.ReadEndElement();
		//    return o;
		//}

		//public object Read28_DockingHintsType()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id1_DockingHintsType) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read1_DockingHintsType(base.Reader.ReadElementString());
		//    }
		//    base.UnknownNode(null, ":DockingHintsType");
		//    return null;
		//}

		//public object Read29_Item()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id3_Item) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read4_Item(true, true);
		//    }
		//    base.UnknownNode(null, ":RadDockableSelectorDesignTimeData");
		//    return null;
		//}

		//public object Read30_DockState()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id4_DockState) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read5_DockState(base.Reader.ReadElementString());
		//    }
		//    base.UnknownNode(null, ":DockState");
		//    return null;
		//}

		//public object Read31_Item()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id5_Item) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read6_Item(true, true);
		//    }
		//    base.UnknownNode(null, ":DocumentPresenterDesignTimeData");
		//    return null;
		//}

		//public object Read32_DockPosition()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id6_DockPosition) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read7_DockPosition(base.Reader.ReadElementString());
		//    }
		//    base.UnknownNode(null, ":DockPosition");
		//    return null;
		//}

		//public object Read33_DockPresenterDesignTimeData()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id7_DockPresenterDesignTimeData) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read8_DockPresenterDesignTimeData(true, true);
		//    }
		//    base.UnknownNode(null, ":DockPresenterDesignTimeData");
		//    return null;
		//}

        public object Read34_DockType()
        {
            base.Reader.MoveToContent();
            if (base.Reader.NodeType == XmlNodeType.Element)
            {
                if ((base.Reader.LocalName != this.id8_DockType) || (base.Reader.NamespaceURI != this.id2_Item))
                {
                    throw base.CreateUnknownNodeException();
                }
                return this.Read9_DockType(base.Reader.ReadElementString());
            }
            base.UnknownNode(null, ":DockType");
            return null;
        }

		//public object Read35_DragManager()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id9_DragManager) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read10_DragManager(true, true);
		//    }
		//    base.UnknownNode(null, ":DragManager");
		//    return null;
		//}

        public object Read36_XmlDockable()
        {
            base.Reader.MoveToContent();
            if (base.Reader.NodeType == XmlNodeType.Element)
            {
                if ((base.Reader.LocalName != this.id10_XmlDockable) || (base.Reader.NamespaceURI != this.id2_Item))
                {
                    throw base.CreateUnknownNodeException();
                }
                return this.Read11_XmlDockable(true, true);
            }
            base.UnknownNode(null, ":XmlDockable");
            return null;
        }

        public object Read37_XmlDockNode()
        {
            base.Reader.MoveToContent();
            if (base.Reader.NodeType == XmlNodeType.Element)
            {
                if ((base.Reader.LocalName != this.id11_XmlDockNode) || (base.Reader.NamespaceURI != this.id2_Item))
                {
                    throw base.CreateUnknownNodeException();
                }
                return this.Read12_XmlDockNode(true, true);
            }
            base.UnknownNode(null, ":XmlDockNode");
            return null;
        }

        public object Read38_XmlDockSite()
        {
            base.Reader.MoveToContent();
            if (base.Reader.NodeType == XmlNodeType.Element)
            {
                if ((base.Reader.LocalName != this.id12_XmlDockSite) || (base.Reader.NamespaceURI != this.id2_Item))
                {
                    throw base.CreateUnknownNodeException();
                }
                return this.Read13_XmlDockSite(true, true);
            }
            base.UnknownNode(null, ":XmlDockSite");
            return null;
        }

        public object Read39_DockingTree()
        {
            base.Reader.MoveToContent();
            if (base.Reader.NodeType == XmlNodeType.Element)
            {
                if ((base.Reader.LocalName != this.id13_DockingTree) || (base.Reader.NamespaceURI != this.id2_Item))
                {
                    throw base.CreateUnknownNodeException();
                }
                return this.Read14_XmlDockingManager(true, true);
            }
            base.UnknownNode(null, ":DockingTree");
            return null;
        }

		//private RadDockableSelectorDesignTimeData Read4_Item(bool isNullable, bool checkType)
		//{
		//    XmlQualifiedName type = checkType ? base.GetXsiType() : null;
		//    bool flag = false;
		//    if (isNullable)
		//    {
		//        flag = base.ReadNull();
		//    }
		//    if ((checkType && (type != null)) && ((type.Name != this.id3_Item) || (type.Namespace != this.id2_Item)))
		//    {
		//        throw base.CreateUnknownTypeException(type);
		//    }
		//    if (flag)
		//    {
		//        return null;
		//    }
		//    RadDockableSelectorDesignTimeData o = new RadDockableSelectorDesignTimeData();
		//    bool[] flagArray = new bool[1];
		//    while (base.Reader.MoveToNextAttribute())
		//    {
		//        if (!base.IsXmlnsAttribute(base.Reader.Name))
		//        {
		//            base.UnknownNode(o);
		//        }
		//    }
		//    base.Reader.MoveToElement();
		//    if (base.Reader.IsEmptyElement)
		//    {
		//        base.Reader.Skip();
		//        return o;
		//    }
		//    base.Reader.ReadStartElement();
		//    base.Reader.MoveToContent();
		//    int whileIterations = 0;
		//    int readerCount = base.ReaderCount;
		//    while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
		//    {
		//        if (base.Reader.NodeType == XmlNodeType.Element)
		//        {
		//            if ((!flagArray[0] && (base.Reader.LocalName == this.id52_ControlName)) && (base.Reader.NamespaceURI == this.id2_Item))
		//            {
		//                o.ControlName = base.Reader.ReadElementString();
		//                flagArray[0] = true;
		//            }
		//            else
		//            {
		//                base.UnknownNode(o, ":ControlName");
		//            }
		//        }
		//        else
		//        {
		//            base.UnknownNode(o, ":ControlName");
		//        }
		//        base.Reader.MoveToContent();
		//        base.CheckReaderCount(ref whileIterations, ref readerCount);
		//    }
		//    base.ReadEndElement();
		//    return o;
		//}

        //public object Read40_TdiContextMenuMode()
        //{
        //    base.Reader.MoveToContent();
        //    if (base.Reader.NodeType == XmlNodeType.Element)
        //    {
        //        if ((base.Reader.LocalName != this.id14_TdiContextMenuMode) || (base.Reader.NamespaceURI != this.id2_Item))
        //        {
        //            throw base.CreateUnknownNodeException();
        //        }
        //        return this.Read15_TdiContextMenuMode(base.Reader.ReadElementString());
        //    }
        //    base.UnknownNode(null, ":TdiContextMenuMode");
        //    return null;
        //}

		//public object Read41_DockingChangingEventArgs()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id15_DockingChangingEventArgs) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read18_DockingChangingEventArgs(true, true);
		//    }
		//    base.UnknownNode(null, ":DockingChangingEventArgs");
		//    return null;
		//}

		//public object Read42_DockingGuidesPosition()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id16_DockingGuidesPosition) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read19_DockingGuidesPosition(base.Reader.ReadElementString());
		//    }
		//    base.UnknownNode(null, ":DockingGuidesPosition");
		//    return null;
		//}

		//public object Read43_LayoutListChangeType()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id17_LayoutListChangeType) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read20_LayoutListChangeType(base.Reader.ReadElementString());
		//    }
		//    base.UnknownNode(null, ":LayoutListChangeType");
		//    return null;
		//}

		//public object Read44_DocumentLayoutStyle()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id18_DocumentLayoutStyle) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read21_DocumentLayoutStyle(base.Reader.ReadElementString());
		//    }
		//    base.UnknownNode(null, ":DocumentLayoutStyle");
		//    return null;
		//}

		//public object Read45_ButtonType()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id19_ButtonType) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read22_ButtonType(base.Reader.ReadElementString());
		//    }
		//    base.UnknownNode(null, ":ButtonType");
		//    return null;
		//}

		//public object Read46_Item()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id20_Item) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read23_Item(true, true);
		//    }
		//    base.UnknownNode(null, ":ContextMenuLocalizationSettings");
		//    return null;
		//}

		//public object Read47_DockingSiteType()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id21_DockingSiteType) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read24_DockingSiteType(base.Reader.ReadElementString());
		//    }
		//    base.UnknownNode(null, ":DockingSiteType");
		//    return null;
		//}

		//public object Read48_DockTabChangedEventArgs()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id22_DockTabChangedEventArgs) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read25_DockTabChangedEventArgs(true, true);
		//    }
		//    base.UnknownNode(null, ":DockTabChangedEventArgs");
		//    return null;
		//}

		//public object Read49_DockTabChangingEventArgs()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id23_DockTabChangingEventArgs) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read26_DockTabChangingEventArgs(true, true);
		//    }
		//    base.UnknownNode(null, ":DockTabChangingEventArgs");
		//    return null;
		//}

		//private DockState Read5_DockState(string s)
		//{
		//    return (DockState)((int)XmlSerializationReader.ToEnum(s, this.DockStateValues, "global::Telerik.WinControls.Docking.DockState"));
		//}

		//public object Read50_DockingChangedEventArgs()
		//{
		//    base.Reader.MoveToContent();
		//    if (base.Reader.NodeType == XmlNodeType.Element)
		//    {
		//        if ((base.Reader.LocalName != this.id24_DockingChangedEventArgs) || (base.Reader.NamespaceURI != this.id2_Item))
		//        {
		//            throw base.CreateUnknownNodeException();
		//        }
		//        return this.Read27_DockingChangedEventArgs(true, true);
		//    }
		//    base.UnknownNode(null, ":DockingChangedEventArgs");
		//    return null;
		//}

		//private DocumentPresenterDesignTimeData Read6_Item(bool isNullable, bool checkType)
		//{
		//    XmlQualifiedName type = checkType ? base.GetXsiType() : null;
		//    bool flag = false;
		//    if (isNullable)
		//    {
		//        flag = base.ReadNull();
		//    }
		//    if ((checkType && (type != null)) && ((type.Name != this.id5_Item) || (type.Namespace != this.id2_Item)))
		//    {
		//        throw base.CreateUnknownTypeException(type);
		//    }
		//    if (flag)
		//    {
		//        return null;
		//    }
		//    DocumentPresenterDesignTimeData o = new DocumentPresenterDesignTimeData();
		//    bool[] flagArray = new bool[1];
		//    while (base.Reader.MoveToNextAttribute())
		//    {
		//        if (!base.IsXmlnsAttribute(base.Reader.Name))
		//        {
		//            base.UnknownNode(o);
		//        }
		//    }
		//    base.Reader.MoveToElement();
		//    if (base.Reader.IsEmptyElement)
		//    {
		//        base.Reader.Skip();
		//        return o;
		//    }
		//    base.Reader.ReadStartElement();
		//    base.Reader.MoveToContent();
		//    int whileIterations = 0;
		//    int readerCount = base.ReaderCount;
		//    while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
		//    {
		//        if (base.Reader.NodeType == XmlNodeType.Element)
		//        {
		//            if ((!flagArray[0] && (base.Reader.LocalName == this.id52_ControlName)) && (base.Reader.NamespaceURI == this.id2_Item))
		//            {
		//                o.ControlName = base.Reader.ReadElementString();
		//                flagArray[0] = true;
		//            }
		//            else
		//            {
		//                base.UnknownNode(o, ":ControlName");
		//            }
		//        }
		//        else
		//        {
		//            base.UnknownNode(o, ":ControlName");
		//        }
		//        base.Reader.MoveToContent();
		//        base.CheckReaderCount(ref whileIterations, ref readerCount);
		//    }
		//    base.ReadEndElement();
		//    return o;
		//}

		//private DockPosition Read7_DockPosition(string s)
		//{
		//    return (DockPosition)((int)XmlSerializationReader.ToEnum(s, this.DockPositionValues, "global::Telerik.WinControls.Docking.DockPosition"));
		//}

		//private DockPresenterDesignTimeData Read8_DockPresenterDesignTimeData(bool isNullable, bool checkType)
		//{
		//    XmlQualifiedName type = checkType ? base.GetXsiType() : null;
		//    bool flag = false;
		//    if (isNullable)
		//    {
		//        flag = base.ReadNull();
		//    }
		//    if ((checkType && (type != null)) && ((type.Name != this.id7_DockPresenterDesignTimeData) || (type.Namespace != this.id2_Item)))
		//    {
		//        throw base.CreateUnknownTypeException(type);
		//    }
		//    if (flag)
		//    {
		//        return null;
		//    }
		//    DockPresenterDesignTimeData o = new DockPresenterDesignTimeData();
		//    bool[] flagArray = new bool[1];
		//    while (base.Reader.MoveToNextAttribute())
		//    {
		//        if (!base.IsXmlnsAttribute(base.Reader.Name))
		//        {
		//            base.UnknownNode(o);
		//        }
		//    }
		//    base.Reader.MoveToElement();
		//    if (base.Reader.IsEmptyElement)
		//    {
		//        base.Reader.Skip();
		//        return o;
		//    }
		//    base.Reader.ReadStartElement();
		//    base.Reader.MoveToContent();
		//    int whileIterations = 0;
		//    int readerCount = base.ReaderCount;
		//    while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
		//    {
		//        if (base.Reader.NodeType == XmlNodeType.Element)
		//        {
		//            if ((!flagArray[0] && (base.Reader.LocalName == this.id52_ControlName)) && (base.Reader.NamespaceURI == this.id2_Item))
		//            {
		//                o.ControlName = base.Reader.ReadElementString();
		//                flagArray[0] = true;
		//            }
		//            else
		//            {
		//                base.UnknownNode(o, ":ControlName");
		//            }
		//        }
		//        else
		//        {
		//            base.UnknownNode(o, ":ControlName");
		//        }
		//        base.Reader.MoveToContent();
		//        base.CheckReaderCount(ref whileIterations, ref readerCount);
		//    }
		//    base.ReadEndElement();
		//    return o;
		//}

        private DockType Read9_DockType(string s)
        {
            switch (s)
            {
                case "ToolWindow":
                    return DockType.ToolWindow;

                case "Document":
                    return DockType.Document;
            }
            throw base.CreateUnknownConstantException(s, typeof(DockType));
        }

        internal Hashtable DockPositionValues
        {
            get
            {
                if (this._DockPositionValues == null)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("Left", 1L);
                    hashtable.Add("Top", 2L);
                    hashtable.Add("Right", 4L);
                    hashtable.Add("Bottom", 8L);
                    hashtable.Add("Fill", 0x10L);
                    hashtable.Add("Default", 0x20L);
                    this._DockPositionValues = hashtable;
                }
                return this._DockPositionValues;
            }
        }

        internal Hashtable DockStateValues
        {
            get
            {
                if (this._DockStateValues == null)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("Floating", 1L);
                    hashtable.Add("Docked", 2L);
                    hashtable.Add("TabbedDocument", 4L);
                    hashtable.Add("AutoHide", 8L);
                    hashtable.Add("Hidden", 0x10L);
                    hashtable.Add("Default", 0L);
                    this._DockStateValues = hashtable;
                }
                return this._DockStateValues;
            }
        }
    }
}

