using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using GeoDo.RSS.Core.DF;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RasterProject;

namespace Geodo.ProjectUI
{
    public partial class UCSelectBands : UserControl
    {
        private const string STRNODENAME = "band_";
        private const string GETALIASNAME = "File Alias Name";
        private const string XMLFILEPATH = @"XMLDataProBands.xml"; 
        private IRasterDataProvider _provider;
        //private List<BandMap> _bandMapList;
        private XElement _xElemRoot;
        private XElement _xElement;
        private string _sensorName;
        private IBandProvider _bandProvider;
        private Dictionary<string, string> _fileAttrs;
        private bool _isShowButton = true;

        public UCSelectBands()
        {
            InitializeComponent();
            if (DesignMode)
                return;
            Init();
        }

        private void Init()
        {
            panelBtn.Visible = _isShowButton;
            _bandMapList = new List<BandMap>();
        }

        public bool IsShowButton
        {
            get { return _isShowButton; }
            set { _isShowButton = value; }
        }

        public IRasterDataProvider Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        public void InitTreeView(IRasterDataProvider provider)
        {
            tvBands.Nodes.Clear();
            if (provider != null)
            {
                _provider = provider;
                GetDataType();
                AddtvBandsNodes(_xElement);
                AddAll();
            }
        }

        private void GetDataType()
        {
            _bandProvider = _provider.BandProvider;
            _fileAttrs = _bandProvider.GetAttributes();
            _fileAttrs.TryGetValue(GETALIASNAME, out _sensorName);
            _xElemRoot = XElement.Load(XMLFILEPATH, LoadOptions.None);
            IEnumerable<XElement> items = _xElemRoot.Descendants("File");
            CompareType(items);
        }

        private void CompareType(IEnumerable<XElement> items)
        {
            //if (_sensorName == "")
            //    _sensorName = "";
            //else if (_sensorName == "")
            //    _sensorName = "";
            foreach (XElement item in items)
            {
                if (item.Attribute("name").Value == _sensorName)
                {
                    _xElement = item;
                }
            }
        }

        private void AddtvBandsNodes(XElement item)
        {
            foreach (XElement subElem in item.Descendants())
            {
                tvBands.Nodes.Add(subElem.Value, STRNODENAME + subElem.Value + " " + subElem.Attribute("description").Value);
            }
        }

        private void tvBands_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //if (_bandMapList != null)
            //{
            //    _bandMapList.Clear();
            //}
            //else
            //{
            //    _bandMapList = new List<BandMap>();
            //}
            //foreach (TreeNode node in tvBands.Nodes)
            //{
            //    if (node.Checked == true)
            //    {
            //        foreach (XElement item in _xElement.Elements("Band"))
            //        {
            //            if (item.Value == (node.Index + 1).ToString())
            //            {
            //                _bandMapList.Add(new BandMap() { DatasetName = item.Attribute("datasetname").Value, File = _provider, BandIndex = Convert.ToInt32(item.Attribute("bandindex").Value) });
            //            }
            //        }
            //    }
            //}
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            AddAll();
        }

        private void btnNone_Click(object sender, EventArgs e)
        {
            RemoveAll();
        }

        public void AddAll()
        {
            if (tvBands.Nodes.Count == 0)
                return;
            foreach (TreeNode node in tvBands.Nodes)
            {
                node.Checked = true;
            }
        }

        public void RemoveAll()
        {
            if (tvBands.Nodes.Count == 0)
                return;
            foreach (TreeNode node in tvBands.Nodes)
            {
                node.Checked = false;
            }
        }

        //public List<BandMap> GetBandMapList()
        //{
        //    return _bandMapList;
        //}

        public void Clear()
        {
            _bandMapList = null;
            tvBands.Nodes.Clear();
        }
    }
}
