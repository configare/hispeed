using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RasterProject;
using System.Xml.Linq;

namespace GeoDo.ProjectUI
{
    public partial class UCDefinedRegion : UserControl
    {
        //public delegate void ValueChangedFinishedHandler(object sender, int barIndex, double value);
        
        private const string FILEPATH=@"XMLDefinedRegion.xml";
        private const string ROOT = "分幅";
        private XElement _xElemRoot;
        private IEnumerable<XElement> _xElementItems;
        private PrjEnvelope _prjEnvelope;
        private List<PrjEnvelope> _prjEnvelopeList;


        public UCDefinedRegion()
        {
            InitializeComponent();
            if (DesignMode)
                return;
            Init();
        }

        public List<PrjEnvelope> PrjEnvelopeList
        {
            get { return _prjEnvelopeList; }
            set { _prjEnvelopeList = value; }
        }

        private void Init()
        {
            _prjEnvelopeList = new List<PrjEnvelope>();
            _xElemRoot = XElement.Load(FILEPATH, LoadOptions.None);
            _xElementItems = _xElemRoot.Elements("SpecRegion");
            tvDefinedRegion.Nodes.Add(ROOT);
            InitTree(_xElementItems, tvDefinedRegion.Nodes[0]);
        }

        private void InitTree(IEnumerable<XElement> items, TreeNode node)
        {
            if (items == null)
            {
                return;
            }
            foreach (XElement item in items)
            {
                node.Nodes.Add(item.FirstAttribute.Value);
                InitTree(item.Elements("SpecRegion"), node.LastNode);
            }
        }

        private void tvDefinedRegion_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                AddCheckedNodes(_xElementItems, e.Node);
            }
        }

        private void AddCheckedNodes(IEnumerable<XElement> items, TreeNode node)
        {
            if (items == null)
            {
                return;
            }
            foreach (XElement item in items)
            {
                if (item.FirstAttribute.Value == node.Text)
                {
                    if (node.Parent.Checked == true || node.Parent.Text != "分幅" || (item.Attributes().Count()) == 1)
                    {
                        return;
                    }
                    _prjEnvelope = new PrjEnvelope(Convert.ToDouble(item.Attribute("minLongitude").Value), Convert.ToDouble(item.Attribute("maxLongitude").Value), Convert.ToDouble(item.Attribute("minLatitude").Value), Convert.ToDouble(item.Attribute("maxLatitude").Value));
                    _prjEnvelopeList.Add(_prjEnvelope);
                }
                AddCheckedNodes(item.Elements("SpecRegion"),node);
            }
        }
    }
}
