using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.RasterDrawing;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    public partial class UCRasterInfoTree : UserControl
    {
        public event EventHandler OnBandNoClicked = null;

        public UCRasterInfoTree()
        {
            InitializeComponent();
            treeView1.MouseDown += new MouseEventHandler(treeView1_MouseDown);
        }

        void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode node = treeView1.GetNodeAt(e.Location);
            if (node == null || node.Tag == null)
                return;
            int b = 0;
            if (int.TryParse(node.Tag.ToString(), out b))
            {
                if (OnBandNoClicked != null)
                    OnBandNoClicked(b.ToString(), null);
            }
        }

        public void Apply(IRasterDrawing drawing)
        {
            treeView1.Nodes.Clear();
            if (drawing == null)
                return;
            //
            TreeNode fileNode = new TreeNode(Path.GetFileName(drawing.FileName));
            fileNode.ToolTipText = drawing.FileName;
            treeView1.Nodes.Add(fileNode);
            int bandCount = drawing.DataProvider.BandCount;
            for (int iband = 1; iband <= bandCount; iband++)
            {
                TreeNode bandNode = new TreeNode(GetBandName(iband, drawing));
                bandNode.Tag = iband;
                fileNode.Nodes.Add(bandNode);
            }
            //
            TreeNode prjNode = new TreeNode("投影信息");
            fileNode.Nodes.Add(prjNode);
            TreeNode prjTypeNode = new TreeNode("坐标类型 : " + GetCoordType(drawing));
            prjNode.Nodes.Add(prjTypeNode);
            TreeNode resolutionNode = new TreeNode("分辨率 : " + drawing.DataProvider.ResolutionX.ToString("0.####") + " , " + drawing.DataProvider.ResolutionY.ToString("0.####"));
            prjNode.Nodes.Add(resolutionNode);
            prjNode.Nodes.Add("MinX  : " + drawing.DataProvider.CoordEnvelope.MinX.ToString("0.####"));
            prjNode.Nodes.Add("MaxX : " + drawing.DataProvider.CoordEnvelope.MaxX.ToString("0.####"));
            prjNode.Nodes.Add("MinY  : " + drawing.DataProvider.CoordEnvelope.MinY.ToString("0.####"));
            prjNode.Nodes.Add("MaxY : " + drawing.DataProvider.CoordEnvelope.MaxY.ToString("0.####"));
            //
            TreeNode sizeNode = new TreeNode("基本信息");
            fileNode.Nodes.Add(sizeNode);
            TreeNode widthNode = new TreeNode("宽度: " + drawing.DataProvider.Width.ToString());
            sizeNode.Nodes.Add(widthNode);
            TreeNode heightNode = new TreeNode("高度 : " + drawing.DataProvider.Height.ToString());
            sizeNode.Nodes.Add(heightNode);
            TreeNode dataTypeNode = new TreeNode("数据类型 : " + drawing.DataProvider.DataType.ToString());
            sizeNode.Nodes.Add(dataTypeNode);
            TreeNode satelliteNode = new TreeNode("卫星 : " + GetSatellite(drawing.DataProvider.DataIdentify));
            sizeNode.Nodes.Add(satelliteNode);
            TreeNode sensorNode = new TreeNode("传感器 : " + GetSensor(drawing.DataProvider.DataIdentify));
            sizeNode.Nodes.Add(sensorNode);
            TreeNode orbitTimeNode = new TreeNode("时间 : " + GetOrbitTime(drawing.DataProvider.DataIdentify));
            sizeNode.Nodes.Add(orbitTimeNode);
            //
            fileNode.ExpandAll();
        }

        private string GetSensor(DataIdentify dataIdentify)
        {
            return dataIdentify != null ? (dataIdentify.Sensor ?? string.Empty) : string.Empty;
        }

        private string GetSatellite(DataIdentify dataIdentify)
        {
            return dataIdentify != null ? (dataIdentify.Satellite ?? string.Empty) : string.Empty;
        }

        private string GetOrbitTime(DataIdentify dataIdentify)
        {
            return dataIdentify != null ? (dataIdentify.OrbitDateTime == DateTime.MinValue ? string.Empty : dataIdentify.OrbitDateTime.ToString("yyy-MM-dd HH:mm:ss")) : string.Empty;
        }

        private string GetCoordType(IRasterDrawing drawing)
        {
            switch (drawing.DataProvider.CoordType)
            {
                case enumCoordType.GeoCoord:
                    return "地理坐标";
                case enumCoordType.PrjCoord:
                    return "投影坐标";
                case enumCoordType.Raster:
                    return "栅格坐标";
            }
            return "未知坐标";
        }

        private string GetBandName(int b, IRasterDrawing drawing)
        {
            IRasterBand band = drawing.DataProvider.GetRasterBand(b);
            if (!string.IsNullOrEmpty(band.Description))
                return string.Format("通道 {0}({1})", b, band.Description ?? string.Empty);
            return string.Format("通道 {0}", b);
        }
    }
}
