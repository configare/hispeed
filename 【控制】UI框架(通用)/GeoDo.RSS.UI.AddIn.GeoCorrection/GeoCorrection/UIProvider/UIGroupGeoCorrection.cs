using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;

namespace GeoDo.RSS.UI.AddIn.GeoCorrection
{
    public partial class UIGroupGeoCorrection : UserControl, IUIGroupProvider
    {
        private ISmartSession _smartSession = null;
        private RadRibbonBarGroup _group = null;
        private object[] _arguments = null;

        public UIGroupGeoCorrection()
        {
            InitializeComponent();
            CreateMenuGroup();
        }

        private void CreateMenuGroup()
        {
            RadButtonElement geoCorrectionTool = new RadButtonElement();
            geoCorrectionTool.Image = imageList1.Images[0];
            geoCorrectionTool.Text = "几何精校正";
            geoCorrectionTool.ImageAlignment = ContentAlignment.MiddleCenter;
            geoCorrectionTool.TextAlignment = ContentAlignment.MiddleCenter;
            geoCorrectionTool.TextImageRelation = TextImageRelation.ImageAboveText;
            geoCorrectionTool.Click += new EventHandler(geoCorrectionTool_Click);

            _group = new RadRibbonBarGroup();
            _group.Text = "几何精校正工具";
            _group.Items.Add(geoCorrectionTool);
        }

        void geoCorrectionTool_Click(object sender, EventArgs e)
        {
            ExecuteCommand(40001);
        }

        private void ExecuteCommand(int id)
        {
            ICommand cmd = _smartSession.CommandEnvironment.Get(id);
            if (cmd != null)
                cmd.Execute();
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
    }
}
