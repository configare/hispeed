namespace Telerik.WinControls
{
    partial class CustomShapeEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Telerik.WinControls.ShapeLinesCollection shapeLinesCollection1 = new Telerik.WinControls.ShapeLinesCollection();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.generalTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.Button_FitShapeToEditor = new System.Windows.Forms.Button();
            this.Button_FitBoundsToEditor = new System.Windows.Forms.Button();
            this.Button_ExtToFit = new System.Windows.Forms.Button();
            this.checkBox_ExtSnap = new System.Windows.Forms.CheckBox();
            this.checkBox_CurveSnap = new System.Windows.Forms.CheckBox();
            this.checkBox_CtrlSnap = new System.Windows.Forms.CheckBox();
            this.checkBox_GridSnap = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.shapeEditorControl1 = new Telerik.WinControls.RadShapeEditorControl();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(613, 1);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(201, 444);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // generalTooltip
            // 
            this.generalTooltip.Tag = "";
            this.generalTooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.generalTooltip.ToolTipTitle = "Information";
            // 
            // Button_FitShapeToEditor
            // 
            this.Button_FitShapeToEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Button_FitShapeToEditor.Image = global::Telerik.WinControls.Properties.Resources.FitShape;
            this.Button_FitShapeToEditor.Location = new System.Drawing.Point(398, 447);
            this.Button_FitShapeToEditor.Name = "Button_FitShapeToEditor";
            this.Button_FitShapeToEditor.Size = new System.Drawing.Size(25, 24);
            this.Button_FitShapeToEditor.TabIndex = 8;
            this.generalTooltip.SetToolTip(this.Button_FitShapeToEditor, "Fit the shape to the visible editor area");
            this.Button_FitShapeToEditor.UseVisualStyleBackColor = true;
            this.Button_FitShapeToEditor.Click += new System.EventHandler(this.Button_FitShapeToEditor_Click);
            // 
            // Button_FitBoundsToEditor
            // 
            this.Button_FitBoundsToEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Button_FitBoundsToEditor.Image = global::Telerik.WinControls.Properties.Resources.FitBounds;
            this.Button_FitBoundsToEditor.Location = new System.Drawing.Point(367, 447);
            this.Button_FitBoundsToEditor.Name = "Button_FitBoundsToEditor";
            this.Button_FitBoundsToEditor.Size = new System.Drawing.Size(25, 24);
            this.Button_FitBoundsToEditor.TabIndex = 8;
            this.generalTooltip.SetToolTip(this.Button_FitBoundsToEditor, "Fit the bounding rectangle to the visible editor area.");
            this.Button_FitBoundsToEditor.UseVisualStyleBackColor = true;
            this.Button_FitBoundsToEditor.Click += new System.EventHandler(this.Button_FitBoundsToEditor_Click);
            // 
            // Button_ExtToFit
            // 
            this.Button_ExtToFit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Button_ExtToFit.Image = global::Telerik.WinControls.Properties.Resources.extToFit;
            this.Button_ExtToFit.Location = new System.Drawing.Point(336, 447);
            this.Button_ExtToFit.Name = "Button_ExtToFit";
            this.Button_ExtToFit.Size = new System.Drawing.Size(25, 24);
            this.Button_ExtToFit.TabIndex = 8;
            this.generalTooltip.SetToolTip(this.Button_ExtToFit, "Extends bounds to fit the whole shape");
            this.Button_ExtToFit.UseVisualStyleBackColor = true;
            this.Button_ExtToFit.Click += new System.EventHandler(this.Button_ExtToFit_Click);
            // 
            // checkBox_ExtSnap
            // 
            this.checkBox_ExtSnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_ExtSnap.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ExtSnap.BackgroundImage = global::Telerik.WinControls.Properties.Resources.snapToExtBtn;
            this.checkBox_ExtSnap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.checkBox_ExtSnap.Checked = true;
            this.checkBox_ExtSnap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_ExtSnap.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBox_ExtSnap.Location = new System.Drawing.Point(52, 446);
            this.checkBox_ExtSnap.Margin = new System.Windows.Forms.Padding(0);
            this.checkBox_ExtSnap.Name = "checkBox_ExtSnap";
            this.checkBox_ExtSnap.Size = new System.Drawing.Size(25, 25);
            this.checkBox_ExtSnap.TabIndex = 2;
            this.generalTooltip.SetToolTip(this.checkBox_ExtSnap, "Toggles Snap To Tangents/Extensions (shortcut: E)");
            this.checkBox_ExtSnap.UseVisualStyleBackColor = true;
            this.checkBox_ExtSnap.CheckedChanged += new System.EventHandler(this.checkBoxSnap_CheckedChanged);
            // 
            // checkBox_CurveSnap
            // 
            this.checkBox_CurveSnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_CurveSnap.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_CurveSnap.BackgroundImage = global::Telerik.WinControls.Properties.Resources.snapToCurveBtn;
            this.checkBox_CurveSnap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.checkBox_CurveSnap.Checked = true;
            this.checkBox_CurveSnap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_CurveSnap.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBox_CurveSnap.Location = new System.Drawing.Point(26, 446);
            this.checkBox_CurveSnap.Margin = new System.Windows.Forms.Padding(0);
            this.checkBox_CurveSnap.Name = "checkBox_CurveSnap";
            this.checkBox_CurveSnap.Size = new System.Drawing.Size(25, 25);
            this.checkBox_CurveSnap.TabIndex = 2;
            this.generalTooltip.SetToolTip(this.checkBox_CurveSnap, "Toggles Snap To Lines/Curves (shortcut: L)");
            this.checkBox_CurveSnap.UseVisualStyleBackColor = true;
            this.checkBox_CurveSnap.CheckedChanged += new System.EventHandler(this.checkBoxSnap_CheckedChanged);
            // 
            // checkBox_CtrlSnap
            // 
            this.checkBox_CtrlSnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_CtrlSnap.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_CtrlSnap.BackgroundImage = global::Telerik.WinControls.Properties.Resources.snapToCtrlBtn;
            this.checkBox_CtrlSnap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.checkBox_CtrlSnap.Checked = true;
            this.checkBox_CtrlSnap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_CtrlSnap.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBox_CtrlSnap.Location = new System.Drawing.Point(0, 446);
            this.checkBox_CtrlSnap.Margin = new System.Windows.Forms.Padding(0);
            this.checkBox_CtrlSnap.Name = "checkBox_CtrlSnap";
            this.checkBox_CtrlSnap.Size = new System.Drawing.Size(25, 25);
            this.checkBox_CtrlSnap.TabIndex = 2;
            this.generalTooltip.SetToolTip(this.checkBox_CtrlSnap, "Toggles Snap To Control Points (shortcut: C)");
            this.checkBox_CtrlSnap.UseVisualStyleBackColor = true;
            this.checkBox_CtrlSnap.CheckedChanged += new System.EventHandler(this.checkBoxSnap_CheckedChanged);
            // 
            // checkBox_GridSnap
            // 
            this.checkBox_GridSnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_GridSnap.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_GridSnap.BackgroundImage = global::Telerik.WinControls.Properties.Resources.snapToGridBtn;
            this.checkBox_GridSnap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.checkBox_GridSnap.Checked = true;
            this.checkBox_GridSnap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_GridSnap.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBox_GridSnap.Location = new System.Drawing.Point(78, 446);
            this.checkBox_GridSnap.Margin = new System.Windows.Forms.Padding(0);
            this.checkBox_GridSnap.Name = "checkBox_GridSnap";
            this.checkBox_GridSnap.Size = new System.Drawing.Size(25, 25);
            this.checkBox_GridSnap.TabIndex = 2;
            this.generalTooltip.SetToolTip(this.checkBox_GridSnap, "Toggles Snap To Grid (shortcut: G)");
            this.checkBox_GridSnap.UseVisualStyleBackColor = true;
            this.checkBox_GridSnap.CheckedChanged += new System.EventHandler(this.checkBoxSnap_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1600%",
            "800%",
            "400%",
            "200%",
            "100%",
            "75%",
            "50%",
            "25%",
            "10%"});
            this.comboBox1.Location = new System.Drawing.Point(146, 449);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(74, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.TextChanged += new System.EventHandler(this.ZoomCombo_TextChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(613, 448);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(717, 448);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(109, 452);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Zoom:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 452);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Grid size:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numericUpDown1.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(278, 449);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(52, 20);
            this.numericUpDown1.TabIndex = 7;
            this.numericUpDown1.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // shapeEditorControl1
            // 
            this.shapeEditorControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.shapeEditorControl1.BackColor = System.Drawing.Color.White;
            this.shapeEditorControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.shapeEditorControl1.CtrlPointsSnap = true;
            this.shapeEditorControl1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.shapeEditorControl1.CurvesSnap = true;
            this.shapeEditorControl1.Dimension = new System.Drawing.Rectangle(64, 64, 512, 256);
            this.shapeEditorControl1.ExtensionsSnap = true;
            this.shapeEditorControl1.GridSize = 32F;
            this.shapeEditorControl1.GridSnap = true;
            this.shapeEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.shapeEditorControl1.MinimumSize = new System.Drawing.Size(60, 60);
            this.shapeEditorControl1.Name = "shapeEditorControl1";
            this.shapeEditorControl1.Shape = shapeLinesCollection1;
            this.shapeEditorControl1.Size = new System.Drawing.Size(607, 445);
            this.shapeEditorControl1.TabIndex = 0;
            this.shapeEditorControl1.SnapChanged += new Telerik.WinControls.SnapChangedEventHandler(this.OnSnapChanged);
            this.shapeEditorControl1.ZoomChanged += new Telerik.WinControls.ZoomChangedEventHandler(this.OnZoomChanged);
            // 
            // CustomShapeEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 471);
            this.Controls.Add(this.Button_FitShapeToEditor);
            this.Controls.Add(this.Button_FitBoundsToEditor);
            this.Controls.Add(this.Button_ExtToFit);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.checkBox_ExtSnap);
            this.Controls.Add(this.checkBox_CurveSnap);
            this.Controls.Add(this.checkBox_CtrlSnap);
            this.Controls.Add(this.checkBox_GridSnap);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.shapeEditorControl1);
            this.MinimumSize = new System.Drawing.Size(638, 200);
            this.Name = "CustomShapeEditorForm";
            this.Text = "Custom Shape Editor";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RadShapeEditorControl shapeEditorControl1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.CheckBox checkBox_GridSnap;
        private System.Windows.Forms.CheckBox checkBox_CtrlSnap;
        private System.Windows.Forms.CheckBox checkBox_CurveSnap;
        private System.Windows.Forms.CheckBox checkBox_ExtSnap;
        private System.Windows.Forms.ToolTip generalTooltip;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button Button_ExtToFit;
        private System.Windows.Forms.Button Button_FitBoundsToEditor;
        private System.Windows.Forms.Button Button_FitShapeToEditor;
    }
}