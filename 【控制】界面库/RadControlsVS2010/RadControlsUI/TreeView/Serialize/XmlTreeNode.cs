using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using Telerik.WinControls.Enumerations;

namespace Telerik.WinControls.UI
{
    public class XmlTreeNode : IXmlTreeSerializable
    {
        #region Static Fields

        private static ColorConverter colorConverter = null;

        public static ColorConverter ColorConverter
        {
            get
            {
                if (XmlTreeNode.colorConverter == null)
                {
                    XmlTreeNode.colorConverter = new ColorConverter();
                }

                return XmlTreeNode.colorConverter;
            }
        }

        #endregion

        #region Constructors

        public XmlTreeNode()
        {
        }

        public XmlTreeNode(RadTreeNode node)
        {
            this.Visible = node.Visible;
            this.Expanded = node.Expanded;
            this.Name = node.Name;
            this.ImageKey = node.ImageKey;
            this.ImageIndex = node.ImageIndex;
            this.ItemHeight = node.ItemHeight;
            this.Text = node.Text;
            this.Tag = node.Tag;
            this.CheckState = node.CheckState;
            this.CheckType = node.CheckType;
            this.BackColor = node.BackColor;
            this.BackColor2 = node.BackColor2;
            this.BackColor3 = node.BackColor3;
            this.BackColor4 = node.BackColor4;
            this.BorderColor = node.BorderColor;
            this.GradientAngle = node.GradientAngle;
            this.GradientPercentage = node.GradientPercentage;
            this.GradientPercentage2 = node.GradientPercentage2;
            this.GradientStyle = node.GradientStyle;
            this.NumberOfColors = node.NumberOfColors;
            this.TextAlignment = node.TextAlignment;

            foreach (RadTreeNode childNode in node.Nodes)
            {
                this.Nodes.Add(new XmlTreeNode(childNode));
            }
        }

        #endregion

        #region Properties

        [XmlAttribute("AdditionalTextStartPosition"), DefaultValue(-1)]
        public int AdditionalTextStartPosition = -1;

        [XmlAttribute("AdditionalTextEndPosition"), DefaultValue(-1)]
        public int AdditionalTextEndPosition = -1;

        [XmlAttribute("Name"), DefaultValue("")]
        public string Name = "";

        [XmlElement("Nodes")]
        public List<XmlTreeNode> Nodes = new List<XmlTreeNode>();

        [XmlAttribute("AllowDrop"), DefaultValue(true)]
        public bool AllowDrop = true;

        [XmlIgnore]
        public Color ForeColor = Color.Empty;

        [XmlAttribute("ForeColor"), DefaultValue("")]
        public string XmlForeColor
        {
            get
            {
                return XmlTreeNode.ColorConverter.ConvertToString(this.ForeColor);
            }
            set
            {
                this.ForeColor = (Color)XmlTreeNode.ColorConverter.ConvertFromString(value);
            }
        }

        [XmlIgnore]
        public Color ForeColor2 = Color.Empty;

        [XmlAttribute("ForeColor2"), DefaultValue("")]
        public string XmlForeColor2
        {
            get
            {
                return XmlTreeNode.ColorConverter.ConvertToString(this.ForeColor2);
            }
            set
            {
                this.ForeColor2 = (Color)XmlTreeNode.ColorConverter.ConvertFromString(value);
            }
        }

        [XmlAttribute("ImageKey"), DefaultValue(null)]
        public string ImageKey = null;

        [XmlAttribute("ImageInFill"), DefaultValue(true)]
        public bool ImageInFill = true;

        [XmlAttribute("RightImageKey"), DefaultValue(null)]
        public string RightImageKey = null;

        [XmlAttribute("SelectedImageKey"), DefaultValue(null)]
        public string SelectedImageKey = null;

        [XmlAttribute("SelectedRightImageKey"), DefaultValue(null)]
        public string SelectedRightImageKey = null;

        [XmlAttribute("ImageIndex"), DefaultValue(-1)]
        public int ImageIndex = -1;

        [XmlAttribute("RightImageIndex"), DefaultValue(-1)]
        public int RightImageIndex = -1;

        [XmlAttribute("StateImageKey"), DefaultValue(null)]
        public string StateImageKey = null;

        [XmlAttribute("StateRightImageKey"), DefaultValue(null)]
        public string StateRightImageKey = null;

        [XmlAttribute("StateRightImageIndex"), DefaultValue(-1)]
        public int StateRightImageIndex = -1;

        [XmlAttribute("StateImageIndex"), DefaultValue(-1)]
        public int StateImageIndex = -1;

        [XmlAttribute("SelectedImageIndex"), DefaultValue(-1)]
        public int SelectedImageIndex = -1;

        [XmlAttribute("ShowRightImage"), DefaultValue(true)]
        public bool ShowRightImage = true;

        [XmlAttribute("SelectedRightImageIndex"), DefaultValue(-1)]
        public int SelectedRightImageIndex = -1;

        [XmlAttribute("Expanded"), DefaultValue(false)]
        public bool Expanded = false;

        [XmlAttribute("Visible"), DefaultValue(true)]
        public bool Visible = true;

        [XmlAttribute("ItemHeight"), DefaultValue(-1)]
        public int ItemHeight = -1;

        [XmlAttribute("Label"), DefaultValue(null)]
        public string Label = null;

        [XmlAttribute("Text"), DefaultValue(null)]
        public string Text = null;

        [XmlAttribute("CheckState"), DefaultValue(ToggleState.Off)]
        public Telerik.WinControls.Enumerations.ToggleState CheckState = ToggleState.Off;

        [XmlAttribute("CheckType"), DefaultValue(CheckType.None)]
        public CheckType CheckType = CheckType.None;

        [XmlIgnore]
        public Color BackColor = Color.Empty;

        [XmlAttribute("BackColor"), DefaultValue("")]
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

        [XmlIgnore]
        public Color BackColor2 = Color.Empty;

        [XmlAttribute("BackColor2"), DefaultValue("")]
        public string XmlBackColor2
        {
            get
            {
                return XmlTreeNode.ColorConverter.ConvertToString(this.BackColor2);
            }
            set
            {
                this.BackColor2 = (Color)XmlTreeNode.ColorConverter.ConvertFromString(value);
            }
        }

        [XmlIgnore]
        public Color BackColor3 = Color.Empty;

        [XmlAttribute("BackColor3"), DefaultValue("")]
        public string XmlBackColor3
        {
            get
            {
                return XmlTreeNode.ColorConverter.ConvertToString(this.BackColor3);
            }
            set
            {
                this.BackColor3 = (Color)XmlTreeNode.ColorConverter.ConvertFromString(value);
            }
        }

        [XmlIgnore]
        public Color BackColor4 = Color.Empty;

        [XmlAttribute("BackColor4"), DefaultValue("")]
        public string XmlBackColor4
        {
            get
            {
                return XmlTreeNode.ColorConverter.ConvertToString(this.BackColor4);
            }
            set
            {
                this.BackColor4 = (Color)XmlTreeNode.ColorConverter.ConvertFromString(value);
            }
        }

        [XmlIgnore]
        public Color BorderColor = Color.Empty;

        [XmlAttribute("BorderColor"), DefaultValue("")]
        public string XmlBorderColor
        {
            get
            {
                return XmlTreeNode.ColorConverter.ConvertToString(this.BorderColor);
            }
            set
            {
                this.BorderColor = (Color)XmlTreeNode.ColorConverter.ConvertFromString(value);
            }
        }

        [XmlAttribute("GradientAngle"), DefaultValue(90.0f)]
        public float GradientAngle = 90.0f;

        [XmlAttribute("GradientPercentage"), DefaultValue(0.5f)]
        public float GradientPercentage = 0.5f;

        [XmlAttribute("GradientPercentage2"), DefaultValue(0.5f)]
        public float GradientPercentage2 = 0.5f;

        [XmlAttribute("GradientStyle"), DefaultValue(GradientStyles.Linear)]
        public Telerik.WinControls.GradientStyles GradientStyle = GradientStyles.Linear;

        [XmlAttribute("NumberOfColors"), DefaultValue(4)]
        public int NumberOfColors = 4;

        [XmlAttribute("TextAlignment"), DefaultValue(ContentAlignment.MiddleLeft)]
        public System.Drawing.ContentAlignment TextAlignment = ContentAlignment.MiddleLeft;

        [XmlElement("Tag"), DefaultValue(null)]
        public object Tag = null;

        #endregion

        #region Methods

        public RadTreeNode Deserialize()
        {
            RadTreeNode node = new RadTreeNode();

            node.Visible = this.Visible;
            node.Expanded = this.Expanded;
            node.Name = this.Name;
            node.ImageKey = this.ImageKey;
            node.ImageIndex = this.ImageIndex;
            node.ItemHeight = this.ItemHeight;
            node.Text = this.Text;
            node.Tag = this.Tag;
            node.CheckType = this.CheckType;

            node.Style.ForeColor = this.ForeColor;
            node.Style.BackColor = this.BackColor;
            node.Style.BackColor2 = this.BackColor2;
            node.Style.BackColor3 = this.BackColor3;
            node.Style.BackColor4 = this.BackColor4;
            node.Style.BorderColor = this.BorderColor;
            node.Style.GradientAngle = this.GradientAngle;
            node.Style.GradientPercentage = this.GradientPercentage;
            node.Style.GradientPercentage2 = this.GradientPercentage2;
            node.Style.GradientStyle = this.GradientStyle;
            node.Style.NumberOfColors = this.NumberOfColors;
            node.Style.TextAlignment = this.TextAlignment;

            foreach (XmlTreeNode childNode in this.Nodes)
            {
                node.Nodes.Add(childNode.Deserialize());
            }

            return node;
        }

        public virtual void ReadUnknownAttribute(System.Xml.XmlAttribute attribute)
        {
            bool booleanResult = false;

            switch (attribute.Name)
            {
                case "ShowCheckBox":
                    if (bool.TryParse(attribute.Value, out booleanResult))
                    {
                        this.CheckType = booleanResult ? CheckType.CheckBox : UI.CheckType.None;
                    }
                    break;
                case "ShowRadioButton":
                    if (bool.TryParse(attribute.Value, out booleanResult))
                    {
                        this.CheckType = booleanResult ? CheckType.RadioButton : UI.CheckType.None;
                    }
                    break;
            }
        }

        #endregion
    }
}
