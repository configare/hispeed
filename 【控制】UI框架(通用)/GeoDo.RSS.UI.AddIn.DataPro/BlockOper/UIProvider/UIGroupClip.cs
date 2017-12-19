using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI;
using Telerik.WinControls;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class UIGroupClip : UserControl, IUIGroupProvider
    {
        private ISmartSession _session = null;
        private object[] _arguments = null;
        private RadRibbonBarGroup _g = null;

        public UIGroupClip()
        {
            InitializeComponent();
            CreateGroupBar();
        }

        private void CreateGroupBar()
        {
            _g = new RadRibbonBarGroup();
            _g.Text = "遥感基础工具";
            RadDropDownButtonElement ddBtnImg = new RadDropDownButtonElement();
            ddBtnImg.Image = imageList1.Images[0];
            ddBtnImg.ImageAlignment = ContentAlignment.MiddleCenter;
            ddBtnImg.TextImageRelation = TextImageRelation.ImageAboveText;
            ddBtnImg.Text = "影像裁切";
            RadMenuItem btnImgAoi = new RadMenuItem("AOI");
            btnImgAoi.Click += new EventHandler(btnCut_Click);
            ddBtnImg.Items.Add(btnImgAoi);
            RadMenuItem btnImgClip = new RadMenuItem("AOI外包矩形");
            btnImgClip.Click += new EventHandler(btnClipTool_Click);
            ddBtnImg.Items.Add(btnImgClip);
            RadDropDownButtonElement ddbtnRst = new RadDropDownButtonElement();
            ddbtnRst.Image = imageList1.Images[0];
            ddbtnRst.ImageAlignment = ContentAlignment.MiddleCenter;
            ddbtnRst.TextImageRelation = TextImageRelation.ImageAboveText;
            ddbtnRst.Text = "栅格裁切";
            RadMenuItem btnRstAoi = new RadMenuItem("AOI");
            btnRstAoi.Click += new EventHandler(btnCut_Click);
            ddbtnRst.Items.Add(btnRstAoi);
            RadMenuItem btnRstClip = new RadMenuItem("AOI外包矩形");
            btnRstClip.Click += new EventHandler(btnClipTool_Click);
            ddbtnRst.Items.Add(btnRstClip);
            RadButtonElement btnSplitTool = new RadButtonElement("分幅");
            btnSplitTool.Image = imageList1.Images[0];
            btnSplitTool.ImageAlignment = ContentAlignment.MiddleCenter;
            btnSplitTool.TextImageRelation = TextImageRelation.ImageAboveText;
            btnSplitTool.Click += new EventHandler(btnClipTool_Click);
            RadButtonElement btnMoTool = new RadButtonElement("拼接/镶嵌");
            btnMoTool.Image = imageList1.Images[1];
            btnMoTool.ImageAlignment = ContentAlignment.MiddleCenter;
            btnMoTool.TextImageRelation = TextImageRelation.ImageAboveText;
            //btnMoTool.Visibility = ElementVisibility.Collapsed;
            btnMoTool.Click += new EventHandler(btnMoTool_Click);
            _g.Items.AddRange(new RadItem[] {ddBtnImg,ddbtnRst, btnSplitTool, btnMoTool });
        }

        void btnMoTool_Click(object sender, EventArgs e)
        {
            ExcuteCommand(4201);
        }

        void btnCut_Click(object sender,EventArgs e)
        {
            ExcuteCommand(4999);
        }

        void btnClipTool_Click(object sender, EventArgs e)
        {
            ExcuteCommand(4200);
        }

        public object Content
        {
            get { return _g; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            _arguments = arguments;
        }

        public void UpdateStatus()
        {
        }

        private void ExcuteCommand(int id)
        {
            try
            {
                ICommand cmd = _session.CommandEnvironment.Get(id);
                cmd.Execute();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
