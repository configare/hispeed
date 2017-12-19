using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmWeightedAveFilterArgEditor : frmRgbArgsEditor
    {
        private RgbWndProcessorArg _actualArg = null;
        private Dictionary<int, int[]> _storedWeights = new Dictionary<int, int[]>();

        public frmWeightedAveFilterArgEditor()
        {
            InitializeComponent();
            Load += new EventHandler(frmMiddleFilterArgEditor_Load);
        }

        void frmMiddleFilterArgEditor_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            _actualArg = _arg as RgbWndProcessorArg;
            InitCollectWindow();
        }

        private void InitCollectWindow()
        {
            cbxWindowRadius.Items.Add(3);
            cbxWindowRadius.Items.Add(5);
            cbxWindowRadius.Items.Add(7);
            cbxWindowRadius.Items.Add(9);
            cbxWindowRadius.SelectedIndexChanged += new EventHandler(collectedWindowRadius_SelectedIndexChanged);
            cbxWindowRadius.SelectedIndex = 0;
        }

        private void collectedWindowRadius_SelectedIndexChanged(object sender, EventArgs e)
        {
            _actualArg.WndHeight = _actualArg.WndWidth = int.Parse(cbxWindowRadius.Text);
            ConstructUIs();
            if (_storedWeights.ContainsKey(_actualArg.WndWidth))
            {
                int[] weights = _storedWeights[_actualArg.WndWidth];
                int idx = 0;
                for (int row = 0; row < _actualArg.WndHeight; row++)
                {
                    for (int col = 0; col < _actualArg.WndHeight; col++)
                    {
                        NumericUpDown box = GetBox(row, col);
                        box.Value = weights[idx];
                        idx++;
                    }
                }
            }
            TryApply();
        }

        private NumericUpDown GetBox(int row, int col)
        {
            foreach (Control box in panel1.Controls)
            {
                if (box.Name == "box_" + row.ToString() + "_" + col.ToString())
                {
                    return box as NumericUpDown;
                }
            }
            return null;
        }

        private void ConstructUIs()
        {
            panel1.Controls.Clear();
            int x = 6;
            int y = 6;
            for (int row = 0; row < _actualArg.WndHeight; row++, y += 28, x = 6)
            {
                for (int col = 0; col < _actualArg.WndHeight; col++, x += 48)
                {
                    NumericUpDown box = new NumericUpDown();
                    box.Left = x;
                    box.Top = y;
                    box.Width = 40;
                    box.Name = "box_" + row.ToString() + "_" + col.ToString();
                    box.Value = 1;
                    box.Minimum = 1;
                    panel1.Controls.Add(box);
                }
            }
        }

        /// <summary>
        /// 将控件中的参数值赋值到参数对象中
        /// </summary>
        protected override void CollectArguments()
        {
            _actualArg.WndHeight = _actualArg.WndWidth = int.Parse(cbxWindowRadius.SelectedItem.ToString());
            CollectWeights();
        }

        private void CollectWeights()
        {
            int[] weights = new int[_actualArg.WndWidth * _actualArg.WndHeight];
            foreach (Control box in panel1.Controls)
            {
                if (box.Name.Contains("box_"))
                {
                    int row = 0, col = 0;
                    string[] parts = box.Name.Split('_');
                    row = int.Parse(parts[1]);
                    col = int.Parse(parts[2]);
                    weights[row * _actualArg.WndHeight + col] = (int)(box as NumericUpDown).Value;
                }
            }
            _storedWeights[_actualArg.WndHeight] = weights;
            (_actualArg as WeightedAveFilterArg).Weight = weights;
        }

        public override bool IsSupport(System.Type type)
        {
            return type.Equals(typeof(RgbProcessorWeightedAveFilter));
        }

    }
}
