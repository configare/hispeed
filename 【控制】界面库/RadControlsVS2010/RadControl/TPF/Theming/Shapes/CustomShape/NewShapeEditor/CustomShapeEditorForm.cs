using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Telerik.WinControls
{
    public partial class CustomShapeEditorForm : Form
    {
        private DialogResult result = DialogResult.None;

        public DialogResult Result
        {
            get { return result; }
        }

        public CustomShapeEditorForm()
        {
            InitializeComponent();

            shapeEditorControl1.propertyGrid = propertyGrid1;
        }

        public CustomShape EditShape(CustomShape shape)
        {
            if (shape == null) return null;

            CustomShape newShape = shape.Clone();

            shapeEditorControl1.Reset();

            if (!newShape.DoFixDimension())
                newShape.CreateRectangleShape(0, 0, 200, 120);

            shapeEditorControl1.Shape = newShape.Shape;
            shapeEditorControl1.Dimension = newShape.Dimension;

            if ( (result = this.ShowDialog()) == DialogResult.OK)
            {
                newShape.Dimension = shapeEditorControl1.Dimension;
                return newShape;
            }
            else return shape;
        }

        public CustomShape CreateShape()
        {
            shapeEditorControl1.Reset();
            CustomShape newShape = new CustomShape();

            if (!newShape.DoFixDimension())
                newShape.CreateRectangleShape(0, 0, 200, 120);

            shapeEditorControl1.Shape = newShape.Shape;
            shapeEditorControl1.Dimension = newShape.Dimension;

            if ((result = this.ShowDialog()) == DialogResult.OK)
            {
                newShape.Dimension = shapeEditorControl1.Dimension;
                return newShape;
            }
            else
            {
                return null;
            }
        }

        // The following method is to be removed
        public RadShapeEditorControl EditorControl
        {
            get { return shapeEditorControl1; }
        }

        private void OnSnapChanged(object sender, SnapChangedEventArgs args)
        {
            if ((args.param & RadShapeEditorControl.SnapTypes.SnapToGrid) != 0)
            {
                if (checkBox_GridSnap.Checked != shapeEditorControl1.GridSnap)
                    checkBox_GridSnap.Checked = shapeEditorControl1.GridSnap;
            }

            if ((args.param & RadShapeEditorControl.SnapTypes.SnapToCtrl) != 0)
            {
                if (checkBox_CtrlSnap.Checked != shapeEditorControl1.CtrlPointsSnap)
                    checkBox_CtrlSnap.Checked = shapeEditorControl1.CtrlPointsSnap;
            }

            if ((args.param & RadShapeEditorControl.SnapTypes.SnapToCurves) != 0)
            {
                if (checkBox_CurveSnap.Checked != shapeEditorControl1.CurvesSnap)
                    checkBox_CurveSnap.Checked = shapeEditorControl1.CurvesSnap;
            }

            if ((args.param & RadShapeEditorControl.SnapTypes.SnapToExtensions) != 0)
            {
                if (checkBox_ExtSnap.Checked != shapeEditorControl1.ExtensionsSnap)
                    checkBox_ExtSnap.Checked = shapeEditorControl1.ExtensionsSnap;
            }
        }

        private void checkBoxSnap_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == null) return;

            switch ((sender as CheckBox).Name)
            {
                case "checkBox_GridSnap":
                    shapeEditorControl1.GridSnap = checkBox_GridSnap.Checked;
                    break;

                case "checkBox_CtrlSnap":
                    shapeEditorControl1.CtrlPointsSnap = checkBox_CtrlSnap.Checked;
                    break;

                case "checkBox_CurveSnap":
                    shapeEditorControl1.CurvesSnap = checkBox_CurveSnap.Checked;
                    break;

                case "checkBox_ExtSnap":
                    shapeEditorControl1.ExtensionsSnap = checkBox_ExtSnap.Checked;
                    break;
            };
        }

        private void ZoomCombo_TextChanged(object sender, EventArgs e)
        {
            float zoomFactor;

            Regex regEx = new Regex("(?<value>([0-9]+(\\.[0-9]*)?)|([0-9]*\\.[0-9]+))\\%?");
            string res = regEx.Match(comboBox1.Text).Groups["value"].Value;

            if (res.Length < 1) return;

            zoomFactor = float.Parse(res) / 100;

            shapeEditorControl1.ZoomCenter(zoomFactor);
        }

        private void OnZoomChanged(object sender, ZoomChangedEventArgs args)
        {
            if (ActiveControl == comboBox1) return;

            comboBox1.Text = Convert.ToString(Math.Round(args.zoomCoef * 100, 2));
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            shapeEditorControl1.GridSize = (int)numericUpDown1.Value;
        }

        private void Button_ExtToFit_Click(object sender, EventArgs e)
        {
            shapeEditorControl1.ExtendBoundsToFitShape();
        }

        private void Button_FitBoundsToEditor_Click(object sender, EventArgs e)
        {
            shapeEditorControl1.FitBoundsToScreen();
        }

        private void Button_FitShapeToEditor_Click(object sender, EventArgs e)
        {
            shapeEditorControl1.FitShapeToScreen();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            shapeEditorControl1.UpdateShape();
        }
    }
}