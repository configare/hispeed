namespace GeoDo.RSS.MIF.UI
{
    partial class UCWorkspace
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCWorkspace));
            this.workspaceImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // workspaceImages
            // 
            this.workspaceImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("workspaceImages.ImageStream")));
            this.workspaceImages.TransparentColor = System.Drawing.Color.Transparent;
            this.workspaceImages.Images.SetKeyName(0, "DBLVFile.png");
            this.workspaceImages.Images.SetKeyName(1, "ExcelFile.png");
            this.workspaceImages.Images.SetKeyName(2, "cmdOpen.png");
            this.workspaceImages.Images.SetKeyName(3, "Category.png");
            this.workspaceImages.Images.SetKeyName(4, "GXDFile.png");
            // 
            // UCWorkspace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "UCWorkspace";
            this.Size = new System.Drawing.Size(820, 522);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ImageList workspaceImages;

    }
}
