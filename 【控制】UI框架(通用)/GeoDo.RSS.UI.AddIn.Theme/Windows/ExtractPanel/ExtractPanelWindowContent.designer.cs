namespace GeoDo.RSS.UI.AddIn.Theme
{
    partial class ExtractPanelWindowContent
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExtract = new System.Windows.Forms.Button();
            this.ckIntimeExtracting = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.brnReset = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.brnReset);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnExtract);
            this.panel1.Controls.Add(this.ckIntimeExtracting);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 354);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(277, 44);
            this.panel1.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(148, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 28);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "保存";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtract.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExtract.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExtract.Location = new System.Drawing.Point(210, 8);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(60, 28);
            this.btnExtract.TabIndex = 0;
            this.btnExtract.Text = "生成";
            this.btnExtract.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // ckIntimeExtracting
            // 
            this.ckIntimeExtracting.AutoSize = true;
            this.ckIntimeExtracting.Location = new System.Drawing.Point(4, 16);
            this.ckIntimeExtracting.Name = "ckIntimeExtracting";
            this.ckIntimeExtracting.Size = new System.Drawing.Size(48, 16);
            this.ckIntimeExtracting.TabIndex = 2;
            this.ckIntimeExtracting.Text = "实时";
            this.ckIntimeExtracting.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(277, 354);
            this.panel2.TabIndex = 1;
            // 
            // brnReset
            // 
            this.brnReset.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.brnReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.brnReset.Location = new System.Drawing.Point(52, 8);
            this.brnReset.Name = "brnReset";
            this.brnReset.Size = new System.Drawing.Size(72, 28);
            this.brnReset.TabIndex = 1;
            this.brnReset.Text = "默认参数";
            this.brnReset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.brnReset.UseVisualStyleBackColor = true;
            this.brnReset.Click += new System.EventHandler(this.brnReset_Click);
            // 
            // ExtractPanelWindowContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ExtractPanelWindowContent";
            this.Size = new System.Drawing.Size(277, 398);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox ckIntimeExtracting;
        private System.Windows.Forms.Button brnReset;
    }
}
