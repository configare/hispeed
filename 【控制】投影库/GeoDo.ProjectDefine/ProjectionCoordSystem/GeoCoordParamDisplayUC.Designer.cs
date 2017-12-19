namespace GeoDo.ProjectDefine
{
    partial class GeoCoordParamDisplayUI
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
            this.grpGeoCoordDisplay = new System.Windows.Forms.GroupBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnModify = new System.Windows.Forms.Button();
            this.btnCreatGeoCoord = new System.Windows.Forms.Button();
            this.txtShow = new System.Windows.Forms.TextBox();
            this.grpGeoCoordDisplay.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpGeoCoordDisplay
            // 
            this.grpGeoCoordDisplay.Controls.Add(this.btnImport);
            this.grpGeoCoordDisplay.Controls.Add(this.btnSelect);
            this.grpGeoCoordDisplay.Controls.Add(this.btnModify);
            this.grpGeoCoordDisplay.Controls.Add(this.btnCreatGeoCoord);
            this.grpGeoCoordDisplay.Controls.Add(this.txtShow);
            this.grpGeoCoordDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpGeoCoordDisplay.Location = new System.Drawing.Point(0, 0);
            this.grpGeoCoordDisplay.Name = "grpGeoCoordDisplay";
            this.grpGeoCoordDisplay.Size = new System.Drawing.Size(384, 214);
            this.grpGeoCoordDisplay.TabIndex = 0;
            this.grpGeoCoordDisplay.TabStop = false;
            this.grpGeoCoordDisplay.Text = "地理坐标系";
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Location = new System.Drawing.Point(303, 94);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 4;
            this.btnImport.Text = "导入";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(303, 69);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 3;
            this.btnSelect.Text = "选择";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnModify
            // 
            this.btnModify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnModify.Location = new System.Drawing.Point(303, 45);
            this.btnModify.Name = "btnModify";
            this.btnModify.Size = new System.Drawing.Size(75, 23);
            this.btnModify.TabIndex = 2;
            this.btnModify.Text = "修改";
            this.btnModify.UseVisualStyleBackColor = true;
            this.btnModify.Click += new System.EventHandler(this.btnModify_Click);
            // 
            // btnCreatGeoCoord
            // 
            this.btnCreatGeoCoord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreatGeoCoord.Location = new System.Drawing.Point(303, 20);
            this.btnCreatGeoCoord.Name = "btnCreatGeoCoord";
            this.btnCreatGeoCoord.Size = new System.Drawing.Size(75, 23);
            this.btnCreatGeoCoord.TabIndex = 1;
            this.btnCreatGeoCoord.Text = "新建";
            this.btnCreatGeoCoord.UseVisualStyleBackColor = true;
            this.btnCreatGeoCoord.Click += new System.EventHandler(this.btnCreatGeoCoord_Click);
            // 
            // txtShow
            // 
            this.txtShow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShow.Location = new System.Drawing.Point(6, 20);
            this.txtShow.Multiline = true;
            this.txtShow.Name = "txtShow";
            this.txtShow.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtShow.Size = new System.Drawing.Size(291, 188);
            this.txtShow.TabIndex = 0;
            // 
            // GeoCoordParamDisplayUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpGeoCoordDisplay);
            this.Name = "GeoCoordParamDisplayUI";
            this.Size = new System.Drawing.Size(384, 214);
            this.grpGeoCoordDisplay.ResumeLayout(false);
            this.grpGeoCoordDisplay.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpGeoCoordDisplay;
        private System.Windows.Forms.TextBox txtShow;
        private System.Windows.Forms.Button btnModify;
        private System.Windows.Forms.Button btnCreatGeoCoord;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnSelect;
    }
}
