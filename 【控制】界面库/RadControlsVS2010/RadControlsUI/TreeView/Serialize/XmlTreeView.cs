using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Telerik.WinControls.UI
{
    [XmlRoot("TreeView")]
    public class XmlTreeView : IXmlTreeSerializable
    {
        #region Constructor

        public XmlTreeView()
        {
        }

        public XmlTreeView(RadTreeView treeView)
        {
            this.AllowDragDrop = treeView.AllowDragDrop;
            this.AllowDrop = treeView.AllowDrop;
            this.BackColor = treeView.BackColor;
            this.CausesValidation = treeView.CausesValidation;
            this.CheckBoxes = treeView.CheckBoxes;
            this.AllowIncrementalSearch = treeView.AllowIncrementalSearch;
            this.MultiSelect = treeView.MultiSelect;
            this.FullRowSelect = treeView.FullRowSelect;
            this.ItemHeight = treeView.ItemHeight;
            this.LineColor = treeView.LineColor;
            this.LineStyle = treeView.LineStyle;
            this.PathSeparator = treeView.PathSeparator;
            this.ShowLines = treeView.ShowLines;
            this.ShowExpandCollapse = treeView.ShowExpandCollapse;
            this.ShowRootLines = treeView.ShowRootLines;
            this.ThemeClassName = treeView.ThemeClassName;
            this.ThemeName = treeView.ThemeName;
            this.TreeIndent = treeView.TreeIndent;
            this.TriStateMode = treeView.TriStateMode;
            this.LabelEdit = treeView.AllowEdit;
            this.ExpandAnimation = treeView.ExpandAnimation;
            this.AllowArbitraryItemHeight = treeView.AllowArbitraryItemHeight;
            this.AllowDragDrop = treeView.AllowDragDrop;
            this.SpacingBetweenNodes = treeView.SpacingBetweenNodes;
            this.RightToLeft = treeView.RightToLeft;
            this.SpacingBetweenNodes = treeView.SpacingBetweenNodes;

            foreach (RadTreeNode node in treeView.Nodes)
            {
                this.Nodes.Add(new XmlTreeNode(node));
            }
        }

        #endregion

        #region Properties

        [XmlAttribute("AllowIncrementalSearch"), DefaultValue(false)]
        public bool AllowIncrementalSearch = false;

        [XmlAttribute("ExpandAnimation"), DefaultValue(ExpandAnimation.Opacity)]
        public Telerik.WinControls.UI.ExpandAnimation ExpandAnimation = ExpandAnimation.Opacity;

        [XmlAttribute("AllowArbitaryItemHeight"), DefaultValue(false)]
        public bool AllowArbitraryItemHeight = false;

        [Obsolete("This field is obsolete. Use AllowArbitraryItemHeight instead.")]
        [XmlAttribute("AllowArbitaryItemHeight"), DefaultValue(false)]
        public bool AllowArbitaryItemHeight = false;

        [XmlAttribute("PlusMinusAnimationStep"), DefaultValue(0.025)]
        public double PlusMinusAnimationStep = 0.025;

        [XmlAttribute("MultiSelect"), DefaultValue(false)]
        public bool MultiSelect = false;

        [XmlAttribute("PathSeparator"), DefaultValue("\\")]
        public string PathSeparator = "\\";

        [XmlIgnore]
        public Color BackColor = SystemColors.Control;

        [XmlAttribute("BackColor"), DefaultValue("Control")]
        public string XmlBackColor
        {
            get
            {
                return XmlTreeNode.ColorConverter.ConvertToString(this.BackColor);
            }
            set
            {
                this.BackColor = (Color)XmlTreeNode.ColorConverter.ConvertFromString(value);
            }
        }

        [XmlAttribute("ShowLines"), DefaultValue(false)]
        public bool ShowLines = false;

        [XmlAttribute("FullRowSelect"), DefaultValue(true)]
        public bool FullRowSelect = true;

        [XmlAttribute("LineStyle"), DefaultValue(TreeLineStyle.Dot)]
        public TreeLineStyle LineStyle = TreeLineStyle.Dot;

        [XmlAttribute("SpacingBetweenNodes"), DefaultValue(0)]
        public int SpacingBetweenNodes = 0;

        [XmlAttribute("AllowDragDrop"), DefaultValue(false)]
        public bool AllowDragDrop = false;

        [XmlAttribute("AllowPlusMinusAnimation"), DefaultValue(false)]
        public bool AllowPlusMinusAnimation = false;

        [XmlAttribute("TriStateMode"), DefaultValue(false)]
        public bool TriStateMode = false;

        [XmlAttribute("CheckBoxes"), DefaultValue(false)]
        public bool CheckBoxes = false;

        [XmlAttribute("ItemHeight"), DefaultValue(20)]
        public int ItemHeight = 20;

        [XmlAttribute("LabelEdit"), DefaultValue(false)]
        public bool LabelEdit = false;

        [XmlIgnore]
        public Color LineColor = Color.Gray;

        [XmlAttribute("LineColor"), DefaultValue("Gray")]
        public string XmlLineColor
        {
            get
            {
                return XmlTreeNode.ColorConverter.ConvertToString(this.LineColor);
            }
            set
            {
                this.LineColor = (Color)XmlTreeNode.ColorConverter.ConvertFromString(value);
            }
        }

        [XmlAttribute("MouseDownEditDelay"), DefaultValue(2000)]
        public int MouseDownEditDelay = 2000;

        [XmlAttribute("TreeIndent"), DefaultValue(20)]
        public int TreeIndent = 20;

        [XmlAttribute("ShowPlusMinus"), DefaultValue(true)]
        public bool ShowPlusMinus = true;

        [XmlAttribute("ShowRootLines"), DefaultValue(true)]
        public bool ShowRootLines = true;

        [XmlElement("Nodes")]
        public List<XmlTreeNode> Nodes = new List<XmlTreeNode>();

        [XmlAttribute("CausesValidation"), DefaultValue(false)]
        public bool CausesValidation = false;

        [XmlAttribute("ThemeName"), DefaultValue("")]
        public string ThemeName;

        [XmlAttribute("ThemeClassName"), DefaultValue("Telerik.WinControls.UI.RadTreeView")]
        public string ThemeClassName;

        [XmlAttribute("AllowDrop"), DefaultValue(false)]
        public bool AllowDrop;

        [XmlAttribute("ShowExpandCollapse"), DefaultValue(true)]
        public bool ShowExpandCollapse = true;

        [XmlAttribute("RightToLeft"), DefaultValue(RightToLeft.No)]
        public RightToLeft RightToLeft;

        #endregion

        #region Constructor

        public void Deserialize(RadTreeView treeView)
        {
            if (treeView == null)
            {
                return;
            }

            treeView.BeginUpdate();

            treeView.AllowDragDrop = this.AllowDragDrop;
            treeView.AllowDrop = this.AllowDrop;
            treeView.BackColor = this.BackColor;
            treeView.CausesValidation = this.CausesValidation;
            treeView.CheckBoxes = this.CheckBoxes;
            treeView.AllowIncrementalSearch = this.AllowIncrementalSearch;
            treeView.MultiSelect = this.MultiSelect;
            treeView.FullRowSelect = this.FullRowSelect;
            treeView.ItemHeight = this.ItemHeight;
            treeView.AllowEdit = this.LabelEdit;
            treeView.LineColor = this.LineColor;
            treeView.LineStyle = this.LineStyle;
            treeView.PathSeparator = this.PathSeparator;
            treeView.ShowLines = this.ShowLines;
            treeView.ShowExpandCollapse = this.ShowExpandCollapse;
            treeView.ShowRootLines = this.ShowRootLines;
            treeView.ThemeClassName = this.ThemeClassName;
            treeView.ThemeName = this.ThemeName;
            treeView.TreeIndent = this.TreeIndent;
            treeView.TriStateMode = this.TriStateMode;
            treeView.ExpandAnimation = this.ExpandAnimation;
            treeView.AllowArbitraryItemHeight = this.AllowArbitraryItemHeight;
            treeView.SpacingBetweenNodes = this.SpacingBetweenNodes;
            treeView.RightToLeft = this.RightToLeft;

            treeView.Nodes.Clear();

            foreach (XmlTreeNode node in this.Nodes)
            {
                treeView.Nodes.Add(node.Deserialize());
            }

            treeView.EndUpdate();
        }

        public virtual void ReadUnknownAttribute(XmlAttribute attribute)
        {
            bool booleanResult = false;

            switch (attribute.Name)
            {
                case "AllowMultiselect":
                    if (bool.TryParse(attribute.Value, out booleanResult))
                    {
                        this.MultiSelect = booleanResult;
                    }
                    break;
                case "AllowDragDropBetweenTreeViews":
                    if (bool.TryParse(attribute.Value, out booleanResult))
                    {
                        this.AllowDragDrop = booleanResult;
                    }
                    break;
            }
        }

        #endregion
    }
}
