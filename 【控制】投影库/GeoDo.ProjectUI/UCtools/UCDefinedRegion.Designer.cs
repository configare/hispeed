namespace GeoDo.ProjectUI
{
    partial class UCDefinedRegion
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
            this.tvDefinedRegion = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // tvDefinedRegion
            // 
            this.tvDefinedRegion.CheckBoxes = true;
            this.tvDefinedRegion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvDefinedRegion.Location = new System.Drawing.Point(0, 0);
            this.tvDefinedRegion.Name = "tvDefinedRegion";
            this.tvDefinedRegion.Size = new System.Drawing.Size(240, 200);
            this.tvDefinedRegion.TabIndex = 0;
            this.tvDefinedRegion.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvDefinedRegion_AfterCheck);
            // 
            // UCDefinedRegion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvDefinedRegion);
            this.Name = "UCDefinedRegion";
            this.Size = new System.Drawing.Size(240, 200);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvDefinedRegion;
    }
}
