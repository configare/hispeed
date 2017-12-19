using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    internal partial class UCFetClassStatInfo : UserControl
    {
        public UCFetClassStatInfo()
        {
            InitializeComponent();
        }

        public void Apply(StatInfoFetClass info)
        {
            txtShapeType.Text = GetShapeTypeString(info.ShapeType);
            txtFeatureCount.Text = info.FeatureCount.ToString();
            if (info.Envelope != null)
            {
                txtMinX.Text = info.Envelope.MinX.ToString("0.######");
                txtMinY.Text = info.Envelope.MinY.ToString("0.######");
                txtMaxX.Text = info.Envelope.MaxX.ToString("0.######");
                txtMaxY.Text = info.Envelope.MaxY.ToString("0.######");
            }
        }

        private string GetShapeTypeString(enumShapeType enumShapeType)
        {
            return ShapeTypeToString.GetStringByShapeType(enumShapeType);
        }
    }
}
