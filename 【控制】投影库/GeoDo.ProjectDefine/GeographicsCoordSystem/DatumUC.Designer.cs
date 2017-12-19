using GeoDo.Project;
namespace GeoDo.ProjectDefine
{
    partial class DatumUC
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.grpDatum = new System.Windows.Forms.GroupBox();
            this.cmbDatumName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.paramSpheroid = new GeoDo.ProjectDefine.SpheroidUC(_spatialReference,_controlType);
            this.grpDatum.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDatum
            // 
            this.grpDatum.Controls.Add(this.paramSpheroid);
            this.grpDatum.Controls.Add(this.cmbDatumName);
            this.grpDatum.Controls.Add(this.label1);
            this.grpDatum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDatum.Location = new System.Drawing.Point(0, 0);
            this.grpDatum.Name = "grpDatum";
            this.grpDatum.Size = new System.Drawing.Size(347, 224);
            this.grpDatum.TabIndex = 0;
            this.grpDatum.TabStop = false;
            this.grpDatum.Text = "基准面";
            // 
            // cmbDatumName
            // 
            this.cmbDatumName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDatumName.FormattingEnabled = true;
            this.cmbDatumName.Location = new System.Drawing.Point(86, 24);
            this.cmbDatumName.Name = "cmbDatumName";
            this.cmbDatumName.Size = new System.Drawing.Size(222, 20);
            this.cmbDatumName.TabIndex = 1;
            this.cmbDatumName.Text = "<自定义>";
            this.cmbDatumName.SelectedIndexChanged += new System.EventHandler(this.cmbDatumName_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名称:";
            // 
            // spheroidUC1
            // 
            this.paramSpheroid.Location = new System.Drawing.Point(6, 50);
            this.paramSpheroid.Name = "spheroidUC1";
            this.paramSpheroid.Size = new System.Drawing.Size(329, 164);
            this.paramSpheroid.SpheroidInverseFlattening = 0D;
            this.paramSpheroid.SpheroidName = null;
            this.paramSpheroid.SpheroidSemimajorAxis = 0D;
            this.paramSpheroid.SpheroidSemiminorAxis = 0D;
            this.paramSpheroid.TabIndex = 2;
            // 
            // DatumUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpDatum);
            this.Name = "DatumUC";
            this.Size = new System.Drawing.Size(347, 224);
            this.grpDatum.ResumeLayout(false);
            this.grpDatum.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDatum;
        private System.Windows.Forms.ComboBox cmbDatumName;
        private System.Windows.Forms.Label label1;
        private SpheroidUC paramSpheroid;
    }
}
