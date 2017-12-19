namespace GeoDo.RSS.UI.AddIn.Theme
{
    partial class UCMagicWandArg
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ckIsContinued = new System.Windows.Forms.CheckBox();
            this.txtTolerance = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.txtTolerance)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "容差:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(189, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "(像素)";
            // 
            // ckIsContinued
            // 
            this.ckIsContinued.AutoSize = true;
            this.ckIsContinued.Checked = true;
            this.ckIsContinued.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckIsContinued.Location = new System.Drawing.Point(274, 11);
            this.ckIsContinued.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckIsContinued.Name = "ckIsContinued";
            this.ckIsContinued.Size = new System.Drawing.Size(51, 21);
            this.ckIsContinued.TabIndex = 3;
            this.ckIsContinued.Text = "连续";
            this.ckIsContinued.UseVisualStyleBackColor = true;
            // 
            // txtTolerance
            // 
            this.txtTolerance.Location = new System.Drawing.Point(65, 7);
            this.txtTolerance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtTolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtTolerance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtTolerance.Name = "txtTolerance";
            this.txtTolerance.Size = new System.Drawing.Size(117, 23);
            this.txtTolerance.TabIndex = 4;
            this.txtTolerance.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // UCMagicWandArg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtTolerance);
            this.Controls.Add(this.ckIsContinued);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UCMagicWandArg";
            this.Size = new System.Drawing.Size(403, 36);
            ((System.ComponentModel.ISupportInitialize)(this.txtTolerance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox ckIsContinued;
        private System.Windows.Forms.NumericUpDown txtTolerance;
    }
}
