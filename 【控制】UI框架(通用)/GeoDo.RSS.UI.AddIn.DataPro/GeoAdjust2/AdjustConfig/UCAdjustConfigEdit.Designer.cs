namespace GeoDo.RSS.UI.AddIn.DataPro
{
    partial class UCAdjustConfigEdit
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
            this.ckbIsOpenResult = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ckbIsOpenResult
            // 
            this.ckbIsOpenResult.AutoSize = true;
            this.ckbIsOpenResult.Checked = true;
            this.ckbIsOpenResult.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbIsOpenResult.Location = new System.Drawing.Point(10, 13);
            this.ckbIsOpenResult.Name = "ckbIsOpenResult";
            this.ckbIsOpenResult.Size = new System.Drawing.Size(156, 16);
            this.ckbIsOpenResult.TabIndex = 3;
            this.ckbIsOpenResult.Text = "校正结束后打开结果文件";
            this.ckbIsOpenResult.UseVisualStyleBackColor = true;
            // 
            // UCAdjustConfigEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ckbIsOpenResult);
            this.Name = "UCAdjustConfigEdit";
            this.Size = new System.Drawing.Size(539, 97);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbIsOpenResult;
    }
}
