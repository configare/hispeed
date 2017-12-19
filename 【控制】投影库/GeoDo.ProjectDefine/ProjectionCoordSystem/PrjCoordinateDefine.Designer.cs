namespace GeoDo.ProjectDefine
{
    partial class PrjCoordinateDefine
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtPrjName = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancle = new System.Windows.Forms.Button();
            this.projectionParam = new GeoDo.ProjectDefine.ProjectionParamUI(_spatialReference,_controlType);
            this.linearUnit = new GeoDo.ProjectDefine.LinearUnitUC(_spatialReference, _controlType);
            this.geoCoordParamDisplay = new GeoDo.ProjectDefine.GeoCoordParamDisplayUI(_spatialReference, _controlType);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名称:";
            // 
            // txtPrjName
            // 
            this.txtPrjName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrjName.Location = new System.Drawing.Point(78, 17);
            this.txtPrjName.Name = "txtPrjName";
            this.txtPrjName.Size = new System.Drawing.Size(301, 21);
            this.txtPrjName.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(223, 579);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancle
            // 
            this.btnCancle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancle.Location = new System.Drawing.Point(308, 579);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(75, 23);
            this.btnCancle.TabIndex = 6;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // projectionParam
            // 
            this.projectionParam.CurrentEnviPrjInfoArgDefs = null;
            this.projectionParam.Location = new System.Drawing.Point(12, 44);
            this.projectionParam.Name = "projectionParam";
            this.projectionParam.ProjectName = null;
            this.projectionParam.ProjectParams = null;
            this.projectionParam.Size = new System.Drawing.Size(367, 276);
            this.projectionParam.TabIndex = 7;
            // 
            // linearUnit
            // 
            this.linearUnit.LinearUnitName = "Meter";
            this.linearUnit.LinearUnitValue = 1D;
            this.linearUnit.Location = new System.Drawing.Point(13, 327);
            this.linearUnit.Name = "linearUnit";
            this.linearUnit.Size = new System.Drawing.Size(366, 93);
            this.linearUnit.TabIndex = 8;
            // 
            // geoCoordParamDisplay
            // 
            this.geoCoordParamDisplay.GeoCoordSystem = null;
            this.geoCoordParamDisplay.Location = new System.Drawing.Point(12, 426);
            this.geoCoordParamDisplay.Name = "geoCoordParamDisplay";
            this.geoCoordParamDisplay.Size = new System.Drawing.Size(366, 147);
            this.geoCoordParamDisplay.TabIndex = 9;
            // 
            // PrjCoordinateDefine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 604);
            this.Controls.Add(this.geoCoordParamDisplay);
            this.Controls.Add(this.linearUnit);
            this.Controls.Add(this.projectionParam);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtPrjName);
            this.Controls.Add(this.label1);
            this.Name = "PrjCoordinateDefine";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "投影坐标定义";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPrjName;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancle;
        private ProjectionParamUI projectionParam;
        private LinearUnitUC linearUnit;
        private GeoCoordParamDisplayUI geoCoordParamDisplay;
    }
}