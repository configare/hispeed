using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.Project;

namespace GeoDo.ProjectDefine
{
    public partial class ProjectionParamUI : UserControl
    {
        private ISpatialReference _spatialReference = null;
        private enumControlType _controlType = enumControlType.Modify;
        private string _projectName = null;
        private NameValuePair[] _projectParams = null;
        private EnviPrjInfoArgDef[] _enviPrjInfoArgDefs = null;
        private EnviPrjInfoArgDef _currentEnviPrjInfoArgDef = null;

        public ProjectionParamUI()
        {
            InitializeComponent();
        }

        public ProjectionParamUI(ISpatialReference SpatialReference, enumControlType ControlType)
        {
            _spatialReference = SpatialReference;
            _controlType = ControlType;
            InitializeComponent();
            Init();
        }

        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; }
        }

        public NameValuePair[] ProjectParams
        {
            get { return _projectParams; }
            set { _projectParams = value; }
        }

        public EnviPrjInfoArgDef  CurrentEnviPrjInfoArgDefs
        {
            get { return _currentEnviPrjInfoArgDef; }
            set { _currentEnviPrjInfoArgDef = value; }
        }

        private void Init()
        {
            cmbPrjName.ItemHeight = 36;
            CreatDataGrid();
            InitPrjName();
            if (_spatialReference!=null&&_controlType == enumControlType.Modify)
                ShowSpatialReference();
        }

        private void InitPrjName()
        {
            using (PrjStdsMapTableParser parser = new PrjStdsMapTableParser())
            {
                if (parser.EnviPrjInfoArgDefs != null)
                {
                    _enviPrjInfoArgDefs = parser.EnviPrjInfoArgDefs;
                    for (int i = 2; i < _enviPrjInfoArgDefs.Length; i++)
                    {
                        cmbPrjName.Items.Add(_enviPrjInfoArgDefs[i].PrjName);
                    }
                }
            }
        }

        /// <summary>
        /// 初始化DataGrid
        /// </summary>
        private void CreatDataGrid()
        {
            dgvParams.BackgroundColor = Color.White;
            dgvParams.AdjustedTopLeftHeaderBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            dgvParams.AdjustedTopLeftHeaderBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            dgvParams.AdjustedTopLeftHeaderBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Outset;
            dgvParams.AdjustedTopLeftHeaderBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.OutsetDouble;

            dgvParams.ColumnCount = 2;
            dgvParams.Columns[0].Name = "参数";
            dgvParams.Columns[1].Name = "值";
            dgvParams.Columns[0].Width = 200;
            dgvParams.Columns[1].Width = 110;

            dgvParams.Columns[0].ReadOnly = true;
            dgvParams.Columns[0].Width = dgvParams.Width / 2;
            dgvParams.Columns[1].Width = dgvParams.Width / 2;
        }

        /// <summary>
        /// 显示投影参数信息
        /// </summary>
        private void ShowSpatialReference()
        {
            cmbPrjName.Text = _spatialReference.ProjectionCoordSystem.Name.Name;
            NameValuePair[] prjParams = _spatialReference.ProjectionCoordSystem.Parameters;
            FillDataGrid(prjParams);
        }

        /// <summary>
        /// 将投影坐标系统的参数填入界面数据表中
        /// </summary>
        private void FillDataGrid(NameValuePair[] prjParams)
        {
            dgvParams.Rows.Clear();
            int count = prjParams.Length;
            DataGridViewRowCollection rows = this.dgvParams.Rows;
            string[] paramString = new string[2];
            for (int i = 0; i < count; i++)
            {
                paramString[0] = prjParams[i].Name.WktName;
                paramString[1] = prjParams[i].Value.ToString();
                rows.Add(paramString);
            }
        }

        private void FillDataGrid(NameMapItem[] prjParamsName)
        {
            dgvParams.Rows.Clear();
            int count = prjParamsName.Length;
            DataGridViewRowCollection rows = this.dgvParams.Rows;
            string[] paramString = new string[2];
            for (int i = 0; i < count; i++)
            {
                paramString[0] = prjParamsName[i].WktName;
                paramString[1] = "0.0000000";
                rows.Add(paramString);
            }
        }

        public void CollectionAguments()
        {
            if (!String.IsNullOrEmpty(cmbPrjName.Text) && cmbPrjName.Text != "<自定义>")
                _projectName = cmbPrjName.Text;
            else
                throw new ArgumentNullException("地理坐标系统名字为空");
            GetDataGridValue();
        }

        /// <summary>
        /// 从界面数据表中获取参数信息
        /// </summary>
        private void GetDataGridValue()
        {
            NameValuePair prjParam = null;
            int count = dgvParams.RowCount;
            _projectParams = new NameValuePair[count]; //这里原为count-1，是错误的
            PrjStdsMapTableParser parser = new PrjStdsMapTableParser();
            string name;
            double value;
            NameMapItem item;
            for (int i = 0; i < count; i++)          //这里原为count-1，是错误的
            {
                DataGridViewRow row = dgvParams.Rows[i];
                name = row.Cells[0].Value.ToString();
                //if (parser.GetPrjParamterItemByWktName(name) == null)
                //    throw new Exception();
                //if (row.Cells[1].Value == null)
                //    throw new Exception();
                try
                {
                    item = parser.GetPrjParamterItemByWktName(name);
                    value = double.Parse(row.Cells[1].Value.ToString());
                    prjParam = new NameValuePair(item, value);
                    _projectParams[i] = prjParam;
                }
                catch
                {
                    MessageBox.Show("请输入数字!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }                
            }
        }

        /// <summary>
        /// 通过投影名获取投影参数(prj4)，然后再获取参数名。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPrjName_SelectedIndexChanged(object sender, EventArgs e)
        {            
            GetPrjNameItem();
        }

        public void GetPrjNameItem()
        {
            string[] argNames = null;
            using (PrjStdsMapTableParser parser = new PrjStdsMapTableParser())
            {
                for (int i = 2; i < _enviPrjInfoArgDefs.Length; i++)
                {
                    if (_enviPrjInfoArgDefs[i].PrjName ==cmbPrjName.Text)// prjName)
                    {
                        _currentEnviPrjInfoArgDef = _enviPrjInfoArgDefs[i];      //当前选定的投影参数数组
                        argNames = _currentEnviPrjInfoArgDef.Args;
                        GetPrjArgNameList(argNames);
                        break;
                    }
                }
            }
            if(_currentEnviPrjInfoArgDef == null)
                MessageBox.Show("投影参数名错误!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 通过投影参数名数组获取投影参数名
        /// </summary>
        /// <param name="argNames"></param>
        private void GetPrjArgNameList(string[] argNames)
        {
            int count;
            NameMapItem[] prjNames;
            using (PrjStdsMapTableParser parser = new PrjStdsMapTableParser())
            {
                if (argNames[0] == "a")
                {
                    count = argNames.Length - 2;
                    prjNames = new NameMapItem[count];
                    //通过Envi名获取投影参数列表
                    for (int i = 0; i < count; i++)
                        prjNames[i] = parser.GetPrjParamterItemByEnviName(argNames[i + 2]);
                    if (prjNames == null)
                        return;
                    FillDataGrid(prjNames);
                }
                else if (argNames[0] == "r")
                {
                    count = argNames.Length - 1;
                    prjNames = new NameMapItem[count];
                    for (int i = 0; i < count; i++)
                        prjNames[i] = parser.GetPrjParamterItemByEnviName(argNames[i + 1]);
                    if (prjNames == null)
                        return;
                    FillDataGrid(prjNames);
                }
            }
        }
    }
}
