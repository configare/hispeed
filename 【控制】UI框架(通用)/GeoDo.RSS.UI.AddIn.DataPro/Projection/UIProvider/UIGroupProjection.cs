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
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;
using GeoDo.ProjectDefine;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class UIGroupProjection : UserControl, IUIGroupProvider
    {
        private ISmartSession _session = null;
        private object[] _arguments = null;
        private RadRibbonBarGroup _group = null;
        private bool _hasAOI = false;
        private bool _hasBlock = false;

        public UIGroupProjection()
        {
            InitializeComponent();
            CreateMenuGroup();
            FileProjector.LoadAllFileProjectors();
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            _arguments = arguments;

            mosaicPrj.Image = _session.UIFrameworkHelper.GetImage("system:L1Proj32_32.png");
            batchMosaicPrj.Image = _session.UIFrameworkHelper.GetImage("system:L1Projs32_32.png");
            prjsElement.Image = _session.UIFrameworkHelper.GetImage("system:globe32_32.png"); // imageList1.Images[1];//.png
            prjSet.Image = _session.UIFrameworkHelper.GetImage("system:globeSetting32_32.png");// imageList1.Images[2];//.png
        }

        RadDropDownButtonElement prjsElement;
        RadButtonElement mosaicPrj;
        RadDropDownButtonElement prjSet;
        RadButtonElement batchMosaicPrj;

        private void CreateMenuGroup()
        {
            prjsElement = new RadDropDownButtonElement();
            prjsElement.Text = "常用投影";
            prjsElement.AutoToolTip = true;
            prjsElement.ImageAlignment = ContentAlignment.MiddleCenter;
            prjsElement.TextAlignment = ContentAlignment.BottomCenter;
            prjsElement.TextImageRelation = TextImageRelation.ImageAboveText;
            prjsElement.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            prjsElement.Click += new EventHandler(customPrjsElement_Click);
            CreateUserDefProjection(prjsElement);

            mosaicPrj = new RadButtonElement();
            mosaicPrj.Text = "拼接投影";
            mosaicPrj.ImageAlignment = ContentAlignment.MiddleCenter;
            mosaicPrj.TextAlignment = ContentAlignment.MiddleCenter;
            mosaicPrj.TextImageRelation = TextImageRelation.ImageAboveText;
            mosaicPrj.Click += new EventHandler(mosaicPrj_Click);

            prjSet = new RadDropDownButtonElement();
            prjSet.Text = "投影选项";
            prjSet.AutoToolTip = true;
            prjSet.ImageAlignment = ContentAlignment.MiddleCenter;
            prjSet.TextAlignment = ContentAlignment.BottomCenter;
            prjSet.TextImageRelation = TextImageRelation.ImageAboveText;
            prjSet.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            RadMenuItem prjSetting = new RadMenuItem("投影配置");
            prjSetting.Image = imageList2.Images[1];
            prjSetting.Click += new EventHandler(prjSetting_Click);
            RadMenuItem prjDef = new RadMenuItem("投影定义");
            prjDef.Image = imageList2.Images[0];
            prjDef.Click += new EventHandler(prjDef_Click);
            RadMenuItem prjChacheClear = new RadMenuItem("投影缓存清理");
            prjChacheClear.Image = imageList2.Images[0];
            prjChacheClear.Click += new EventHandler(prjChacheClear_Click);
            RadMenuItem blockDef = new RadMenuItem("预定义分幅设置");
            blockDef.Image = imageList2.Images[0];
            blockDef.Click += new EventHandler(blockDef_Click);
            RadMenuItem rename = new RadMenuItem("文件重命名");
            rename.Image = imageList2.Images[0];
            rename.Click += new EventHandler(rename_Click);

            //prjSet.Items.Add(prjSetting);
            prjSet.Items.Add(blockDef);
            prjSet.Items.Add(prjDef);
            prjSet.Items.Add(new RadMenuSeparatorItem());
            prjSet.Items.Add(prjChacheClear);
            prjSet.Items.Add(new RadMenuHeaderItem("投影辅助工具"));
            prjSet.Items.Add(rename);

            batchMosaicPrj = new RadButtonElement();
            batchMosaicPrj.Text = "批量拼接投影";
            batchMosaicPrj.ImageAlignment = ContentAlignment.MiddleCenter;
            batchMosaicPrj.TextAlignment = ContentAlignment.MiddleCenter;
            batchMosaicPrj.TextImageRelation = TextImageRelation.ImageAboveText;
            batchMosaicPrj.Click += new EventHandler(batchMosaicPrj_Click);

            _group = new RadRibbonBarGroup();
            _group.Text = "投影工具";
            _group.Items.Add(mosaicPrj);
            _group.Items.Add(batchMosaicPrj);
            _group.Items.Add(prjsElement);
            _group.Items.Add(prjSet);
        }

        void rename_Click(object sender, EventArgs e)
        {
            ExecuteCommand(4007);
        }

        void blockDef_Click(object sender, EventArgs e)
        {
            ExecuteCommand(4006);
        }

        void prjChacheClear_Click(object sender, EventArgs e)
        {
            ExecuteCommand(4004);
        }

        void mosaicPrj_Click(object sender, EventArgs e)
        {
            ExecuteCommand(4003);
        }

        void batchMosaicPrj_Click(object sender, EventArgs e)
        {
            ExecuteCommand(4005);
        }

        void customPrjsElement_Click(object sender, EventArgs e)
        {
            CreateUserDefProjection(sender as RadDropDownButtonElement);
        }

        private void CreateUserDefProjection(RadDropDownButtonElement customPrjsElement)
        {
            customPrjsElement.Items.Clear();
            RadMenuHeaderItem header = new RadMenuHeaderItem("常用自定义投影");
            customPrjsElement.Items.Add(header);

            ISpatialReference[] customSpatialReferences = SpatialReferenceSelection.CustomSpatialReferences;
            if (customSpatialReferences != null && customSpatialReferences.Length != 0)
            {
                List<RadItem> otherPrjsBtnList = new List<RadItem>();
                foreach (ISpatialReference prj in customSpatialReferences)
                {
                    RadMenuItem btnPrj = new RadMenuItem();
                    btnPrj.Text = prj.Name;
                    //btnPrj.Image = imageList2.Images[0];
                    btnPrj.TextAlignment = ContentAlignment.MiddleLeft;
                    btnPrj.TextImageRelation = TextImageRelation.ImageBeforeText;
                    btnPrj.Tag = prj;
                    btnPrj.Click += new EventHandler(btnPrjOther_Click);
                    otherPrjsBtnList.Add(btnPrj);
                }
                customPrjsElement.Items.AddRange(otherPrjsBtnList.ToArray());
            }
            RadMenuHeaderItem pl = new RadMenuHeaderItem();

            RadMenuItem morePrj = new RadMenuItem();
            morePrj.Text = "更多投影...";
            //morePrj.Image = imageList2.Images[0];
            morePrj.TextAlignment = ContentAlignment.MiddleLeft;
            morePrj.TextImageRelation = TextImageRelation.ImageBeforeText;
            morePrj.Click += new EventHandler(morePrj_Click);

            customPrjsElement.Items.Add(pl);
            customPrjsElement.Items.Add(morePrj);
        }

        void prjDef_Click(object sender, EventArgs e)
        {
            using (SpatialReferenceSelection frmSp = new SpatialReferenceSelection())
            {
                if (frmSp.ShowDialog() == DialogResult.OK)
                {
                    if (frmSp.SpatialReference != null)
                        Projection(frmSp.SpatialReference);
                }
            }
        }

        void prjSetting_Click(object sender, EventArgs e)
        {
            try
            {
                ICommand cmd = _session.CommandEnvironment.Get(4000);
                cmd.Execute();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //常用投影
        void btnPrjOther_Click(object sender, EventArgs e)
        {
            try
            {
                _group.Enabled = false;
                RadMenuItem send = (sender as RadMenuItem);
                ISpatialReference projItem = send.Tag as ISpatialReference;
                Projection(projItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                _group.Enabled = true;
            }
        }

        //更多投影
        void morePrj_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = CheckActiverFile();
                using (SpatialReferenceSelection frmSp = new SpatialReferenceSelection())
                {
                    if (frmSp.ShowDialog() == DialogResult.OK)
                    {
                        if (frmSp.SpatialReference != null)
                            Projection(frmSp.SpatialReference);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                _group.Enabled = true;
            }
        }

        //挑选通道
        void selectGLL_Click(object sender, EventArgs e)
        {
            try
            {
                _group.Enabled = false;
                RadButtonElement send = (sender as RadButtonElement);
                ProjectionItem projItem = send.Tag as ProjectionItem;
                ICommand cmd = _session.CommandEnvironment.Get(4001);
                cmd.Execute("TTT#" + projItem.WktProjection);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                _group.Enabled = true;
            }
        }

        public object Content
        {
            get { return _group; }
        }

        public void UpdateStatus()
        {
        }

        private void Projection(ISpatialReference proj)
        {
            if (proj.ProjectionCoordSystem == null)
                ProjGLL();
            else
                ExecuteCommand(4001, "#" + proj.ToWKTString());
        }

        private void ProjGLL()
        {
            try
            {
                string fileName = CheckActiverFile();
                using (frmProjectionMode frm = new frmProjectionMode(_hasAOI, _hasBlock))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        _group.Enabled = false;
                        switch (frm.ProjectionMode)
                        {
                            case 0:
                                ExecuteCommand(4001);//整轨
                                break;
                            case 1:
                                ExecuteCommand(4021);//AOI
                                break;
                            case 2:
                                ExecuteCommand(4022);//分幅
                                break;
                            case 3:
                                ExecuteCommand(4023);//交互
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                _group.Enabled = true;
            }
        }

        private string CheckActiverFile()
        {
            ICanvasViewer canViewer = _session.SmartWindowManager.ActiveCanvasViewer;
            if (canViewer == null)
                throw new Exception("未获得激活的数据窗口");
            IAOIProvider aoiProvider = canViewer.AOIProvider;
            if (aoiProvider == null)
                throw new Exception("未从激活的数据窗口中获取感兴趣区域");
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope geoEnvelope = aoiProvider.GetGeoRect();
            if (geoEnvelope == null)
                _hasAOI = false;
            else
                _hasAOI = true;
            AOIItem[] aoiItems = aoiProvider.GetAOIItems();
            if (aoiItems == null || aoiItems.Length == 0)
                _hasBlock = false;
            else
                _hasBlock = true;
            IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
            if (rd == null)
                throw new Exception("未从激活的数据窗口中获取数据提供者");
            IRasterDataProvider rdp = rd.DataProvider;
            if (rdp == null)
                throw new Exception("未从激活的数据窗口中获取数据提供者");
            return rdp.fileName;
        }

        private void ExecuteCommand(int id)
        {
            ICommand cmd = _session.CommandEnvironment.Get(id);
            if (cmd != null)
                cmd.Execute();
        }

        private void ExecuteCommand(int id, string argument)
        {
            ICommand cmd = _session.CommandEnvironment.Get(id);
            if (cmd != null)
                cmd.Execute(argument);
        }
    }
}
