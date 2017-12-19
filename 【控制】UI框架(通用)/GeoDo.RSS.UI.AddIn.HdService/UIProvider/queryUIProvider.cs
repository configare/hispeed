using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;
using GeoDo.ProjectDefine;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public partial class queryUIProvider : UserControl,IUIGroupProvider
    {
        private ISmartSession _smartSession = null;
        private object[] _arguments = null;
        private RadRibbonBarGroup _group = null;

        public queryUIProvider()
        {
            InitializeComponent();
            CreateMenuGroup();
        }

        RadDateTimePickerElement dtBeginTime;
        RadDateTimePickerElement dtEndTime;

        RadRadioButtonElement sate1;
        RadRadioButtonElement sate2;
        
        RadRadioButtonElement sensor1;
        RadRadioButtonElement sensor2;

        RadRadioButtonElement dataType1;
        RadRadioButtonElement dataType2;
        RadRadioButtonElement dataType3;

        private void CreateMenuGroup()
        {
            #region 时间
            dtBeginTime = new RadDateTimePickerElement();
            dtBeginTime.Format = DateTimePickerFormat.Custom;
            dtBeginTime.CustomFormat = "yyyy年MM月dd日 HH:mm:ss";
            dtBeginTime.Value = DateTime.Today;
            dtBeginTime.Size = new System.Drawing.Size(180, 21);
            dtBeginTime.MaxSize = new System.Drawing.Size(180, 21);
            dtBeginTime.MinSize = new System.Drawing.Size(180, 21);
            dtBeginTime.Alignment = ContentAlignment.TopLeft;

            RadLabelElement lbBeginTime = new RadLabelElement();
            lbBeginTime.Text = "开始时间";
            lbBeginTime.MinSize = new System.Drawing.Size(40, 21);

            RadRibbonBarButtonGroup gpBeginTime = new RadRibbonBarButtonGroup();
            gpBeginTime.Items.AddRange(new RadItem[] { lbBeginTime, dtBeginTime });
            gpBeginTime.Orientation = Orientation.Horizontal;
            gpBeginTime.ShowBackColor = false;
            //gpBeginTime.ShowBorder = false;
            gpBeginTime.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);

            dtEndTime = new RadDateTimePickerElement();
            dtEndTime.Format = DateTimePickerFormat.Custom;
            dtEndTime.CustomFormat = "yyyy年MM月dd日 HH:mm:ss";
            dtEndTime.Value = DateTime.Today.AddDays(1).AddSeconds(-1);
            dtEndTime.Size = new System.Drawing.Size(180, 21);
            dtEndTime.MaxSize = new System.Drawing.Size(180, 21);
            dtEndTime.MinSize = new System.Drawing.Size(180, 21);
            dtEndTime.Alignment = ContentAlignment.TopLeft;

            RadLabelElement lbEndTime = new RadLabelElement();
            lbEndTime.Text = "结束时间";
            lbEndTime.MinSize = new System.Drawing.Size(40, 21);

            RadRibbonBarButtonGroup gpEndTime = new RadRibbonBarButtonGroup();
            gpEndTime.Items.AddRange(new RadItem[] { lbEndTime, dtEndTime });
            gpEndTime.Orientation = Orientation.Horizontal;
            gpEndTime.ShowBackColor = false;
            //gpEndTime.ShowBorder = false;
            gpEndTime.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);

            RadRibbonBarButtonGroup gpTime = new RadRibbonBarButtonGroup();
            gpTime.Items.AddRange(new RadItem[] { gpBeginTime, gpEndTime });
            gpTime.Orientation = Orientation.Vertical;
            gpTime.ShowBackColor = false;
            gpTime.ShowBorder = false;
            gpTime.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            #endregion

            #region 卫星传感器
            sate1 = new RadRadioButtonElement();
            sate1.Text = "FY-3A";
            //sate1.Font = new System.Drawing.Font("宋体", 11);
            sate1.MinSize = new System.Drawing.Size(50, 21);
            sate1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            sate1.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;

            sate2 = new RadRadioButtonElement();
            sate2.Text = "FY-3B";
            //sate2.Font = new System.Drawing.Font("宋体", 11);
            sate2.MinSize = new System.Drawing.Size(50, 21);
            sate2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);

            RadRibbonBarButtonGroup gpSatellite = new RadRibbonBarButtonGroup();
            gpSatellite.Items.AddRange(new RadItem[] { sate1, sate2 });
            gpSatellite.Orientation = Orientation.Vertical;
            gpSatellite.ShowBackColor = false;
            //gpSatellite.ShowBorder = false;
            gpSatellite.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);

            sensor1 = new RadRadioButtonElement();
            sensor1.Text = "VIRR";
            //sensor1.Font = new System.Drawing.Font("宋体", 11);
            sensor1.MinSize = new System.Drawing.Size(60, 21);
            sensor1.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            sensor1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);

            sensor2 = new RadRadioButtonElement();
            sensor2.Text = "MERSI";
            //sensor2.Font = new System.Drawing.Font("宋体", 11);
            sensor2.MinSize = new System.Drawing.Size(60, 21);
            sensor2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);

            RadRibbonBarButtonGroup gpSensor = new RadRibbonBarButtonGroup();
            gpSensor.Items.AddRange(new RadItem[] { sensor1, sensor2 });
            gpSensor.Orientation = Orientation.Vertical;
            gpSensor.ShowBackColor = false;
            //gpSensor.ShowBorder = false;
            gpSensor.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);

            RadRibbonBarButtonGroup gpSatelliteSensor = new RadRibbonBarButtonGroup();
            gpSatelliteSensor.Items.AddRange(new RadItem[] { gpSatellite, gpSensor });
            gpSatelliteSensor.Orientation = Orientation.Horizontal;
            gpSatelliteSensor.ShowBackColor = false;
            gpSatelliteSensor.ShowBorder = false;

            #endregion

            #region 数据类型
            dataType1 = new RadRadioButtonElement();
            dataType1.Text = "投影数据";
            dataType1.MinSize = new System.Drawing.Size(60, 21);
            dataType1.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            dataType1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);

            dataType2 = new RadRadioButtonElement();
            dataType2.Text = "投影拼接数据";
            dataType2.MinSize = new System.Drawing.Size(60, 21);
            dataType2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            dataType3 = new RadRadioButtonElement();
            dataType3.Text = "分幅数据";
            dataType3.MinSize = new System.Drawing.Size(60, 21);
            dataType3.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);

            RadRibbonBarButtonGroup gpDataType = new RadRibbonBarButtonGroup();
            gpDataType.Items.AddRange(new RadItem[] { dataType1, dataType2, dataType3 });
            gpDataType.Orientation = Orientation.Vertical;
            gpDataType.ShowBackColor = false;
            //gpDataType.ShowBorder = false;
            gpDataType.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            #endregion

            RadButtonElement btnSearch = new RadButtonElement();
            btnSearch.Image = GeoDo.RSS.UI.AddIn.HdService.Properties.Resources.search_green;
            btnSearch.Text = "数据查询";
            btnSearch.ImageAlignment = ContentAlignment.MiddleCenter;
            btnSearch.TextAlignment = ContentAlignment.MiddleCenter;
            btnSearch.TextImageRelation = TextImageRelation.ImageAboveText;
            btnSearch.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            btnSearch.Click += new EventHandler(btnSearch_Click);

            _group = new RadRibbonBarGroup();
            _group.Text = "数据查询";
            _group.Items.Add(gpSatelliteSensor);
            _group.Items.Add(gpDataType);
            _group.Items.Add(gpTime);
            _group.Items.Add(btnSearch);
        }

        void btnSearch_Click(object sender, EventArgs e)
        {
            ExecuteCommand(39000, getArgument());
        }

        public object Content
        {
            get { return _group; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _smartSession = session;
            _arguments = arguments;
        }

        public void UpdateStatus()
        {
        }

        private string getArgument()
        {
            string argument =
                "BeginTime=" + dtBeginTime.Value + ";"
                + "EndTime=" + dtEndTime.Value + ";"
                + "Satellite=" + GetSatellite() + ";"
                + "Sensor=" + GetSensor() + ";"
                + "DataType=" + GetDataType();
            return argument;
        }

        private string GetDataType()
        {
            if (dataType1.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
                return "Projection";
            if (dataType2.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
                return "Mosaic";
            else
                return "Block";
        }

        private string GetSensor()
        {
            if (sensor1.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
                return "VIRR";
            else
                return "MERSI";
        }

        private string GetSatellite()
        {
            if (sate1.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
                return "FY3A";
            else
                return "FY3B";
        }

        private void ExecuteCommand(int id)
        {
            ICommand cmd = _smartSession.CommandEnvironment.Get(id);
            cmd.Execute();
        }

        private void ExecuteCommand(int id, string argument)
        {
            ICommand cmd = _smartSession.CommandEnvironment.Get(id);
            cmd.Execute(argument);
        }
    }
}
